using DSPEditor.Audio;
using DSPEditor.AudioEffects;
using DSPEditor.DSPAudioEffects;
using DSPEditor.Utility;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DSPEditor.AudioManager
{
    class AudioEffectManager
    {
        private static AudioEffectManager instance = null;
        private static readonly object padlock = new object();
        private AudioItem audioItem;

        private Dispatcher disp = Dispatcher.CurrentDispatcher;
        private BackgroundWorker worker = new BackgroundWorker();
        private DoWorkEventHandler[] doWorkEventHandlers;

        private OutputLogWriter outputLogWriter = new OutputLogWriter();
        public static Action<string> WriteToOutputLog;

        private int threadsValue;
        private int samplesCountForThreads;
        private int[] samplesThreadToProcess;
        private List<Thread> threads = new List<Thread>();

        private Stopwatch stopWatch = new Stopwatch();

        public static AudioEffectManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AudioEffectManager();
                    }
                    return instance;
                }
            }
        }


        AudioEffectManager()
        {
            doWorkEventHandlers = new DoWorkEventHandler[]
            {
                DelayEffectWorkHandler,
                DistortionEffectWorkHandler,
                ReverbEffectWorkHandler,
                SineWaveEffectWorkHandler,
                TremoloWaveEffectWorkHandler,
                WahWahEffectWorkHandler,
                FlangerEffectWorkHandler
            };
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressBarChange);
            worker.RunWorkerCompleted += AudioEffectAdditionCompleted;

            outputLogWriter.SubscribeToAudioEffectEvent();
        }

        private void AudioEffectAdditionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopWatch.Stop();
            if (WriteToOutputLog != null)
                WriteToOutputLog(e.Result as string + "\nC# processing call time elapsed : " + stopWatch.ElapsedMilliseconds + " msec");
        }

        private void ProgressBarChange(object sender, ProgressChangedEventArgs e)
        {
            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                MainWindow.progressBar.Value = e.ProgressPercentage; // Do all the ui thread updates here
            }));
        }

        private void ResetProgressBar()
        {
            this.disp.Invoke(DispatcherPriority.Background, new Action(delegate ()
            {
                MainWindow.progressBar.Value = 0; // Do all the ui thread updates here
            }));
        }

        private void SetMaxProgressBar()
        {
            this.disp.Invoke(DispatcherPriority.Background, new Action(delegate ()
            {
                MainWindow.progressBar.Maximum = 100; // Do all the ui thread updates here
            }));
        }

        public void AddDelayEffect(params object[] list)
        {
            List<object> passParams = list.ToList();

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[0];
            worker.RunWorkerAsync(passParams);
        }


        public void AddFlangerEffect(params object[] list)
        {
            List<object> passParams = list.ToList();

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[6];
            worker.RunWorkerAsync(passParams);
        }


        public void AddTremoloEffect(params object[] list)
        {
            List<object> passParams = list.ToList();

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[4];
            worker.RunWorkerAsync(passParams);
        }

        public void AddReverbEffect(params object[] list)
        {
            List<object> passParams = list.ToList();

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[2];
            worker.RunWorkerAsync(passParams);
        }

        public void AddDistortionEffect()
        {
            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[1];
            worker.RunWorkerAsync();
        }

        public void AddWahWahEffect(params object[] list)
        {
            List<object> passParams = list.ToList();

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[5];
            worker.RunWorkerAsync(passParams);
        }

        public void AddSineWaveEffect(int freq, double ampl)
        {
            List<object> passParams = new List<object>();
            passParams.Add(freq);
            passParams.Add(ampl);

            DisablePreviousWorks();
            stopWatch = Stopwatch.StartNew();
            worker.DoWork += doWorkEventHandlers[3];
            worker.RunWorkerAsync(passParams);
        }

        private void DisablePreviousWorks()
        {
            for(int i = 0; i < doWorkEventHandlers.Length; ++i)
            {
                worker.DoWork -= doWorkEventHandlers[i];
            }
        }

        private void WahWahEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            int effectRate = (int)passedParams[0];
            int maxF = (int)passedParams[1];
            int minF = (int)passedParams[2];
            int Q = (int)passedParams[3];
            double gainFactor = (double)passedParams[4];


            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int timeElapsed = 0;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                AudioWahWahEffect.AutoWahInit((short)effectRate,  /*Effect rate 2000*/
                     16000, /*Sampling Frequency*/
                     (short)maxF,  /*Maximum frequency*/
                     (short)minF,   /*Minimum frequency*/
                     (short)Q,     /*Q*/
                     gainFactor, /*Gain factor*/
                     10     /*Frequency increment*/);

                AudioWahWahEffect.AutoWahSweep();


                if (threadsValue > 1)
                {
                    samplesCountForThreads = (int)((endIndex - beginIndex) / threadsValue);
                    samplesThreadToProcess = new int[threadsValue];

                    for (int i = 0; i < threadsValue; i++)
                    {
                        samplesThreadToProcess[i] = (int)beginIndex + (samplesCountForThreads * i);
                    }
                    samplesThreadToProcess[threadsValue - 1] = (int)endIndex - 1;

                    for (int x = 0; x < threadsValue - 1; x++)
                    {
                        int startIndex, stopIndex;
                        startIndex = samplesThreadToProcess[x];
                        stopIndex = samplesThreadToProcess[x + 1];


                        threads.Add(new Thread(() =>
                        {
                            for (int i = startIndex; i < stopIndex + 1; i++)
                            {
                                samplesToProcess[i] = AudioWahWahEffect.AutoWahProcess(samplesToProcess[i], ref timeElapsed);
                                AudioWahWahEffect.AutoWahSweep();
                                fullTime += timeElapsed;
                            }
                        }));

                    }

                    for (int i = 0; i < threadsValue - 1; i++)
                    {
                        threads[i].Start();
                        threads[i].Join();
                        var first = i;
                        var range = threadsValue - 1;
                        var z = (double)first / range;
                        worker.ReportProgress((int)(z * 100));
                    }
                }
                else
                {
                    for (long i = beginIndex; i < endIndex; ++i)
                    {
                        samplesToProcess[i] = AudioWahWahEffect.AutoWahProcess(samplesToProcess[i], ref timeElapsed);
                        AudioWahWahEffect.AutoWahSweep();
                        if (i % 8000 == 0 && i != 0)
                        {
                            var first = i - beginIndex;
                            var range = endIndex - beginIndex;
                            var x = (double)first / range;
                            worker.ReportProgress((int)(x * 100));
                            System.Threading.Thread.Sleep(50);
                        }
                        fullTime += timeElapsed;
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + seconds + ":" + miliseconds + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Wah-Wah effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void TremoloWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            int effectRate = (int)passedParams[0];
            double depthRate = (double)passedParams[1];

            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                audioItem = AudioItemManager.GetAudioItem();
            }));

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int timeElapsed = 0;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                AudioTremoloEffect.TremoloInit((short)effectRate, depthRate);

                if (threadsValue > 1)
                {
                    samplesCountForThreads = (int)((endIndex - beginIndex) / threadsValue);
                    samplesThreadToProcess = new int[threadsValue];

                    for (int i = 0; i < threadsValue; i++)
                    {
                        samplesThreadToProcess[i] = (int)beginIndex + (samplesCountForThreads * i);
                    }
                    samplesThreadToProcess[threadsValue - 1] = (int)endIndex - 1;

                    for (int x = 0; x < threadsValue - 1; x++)
                    {
                        int startIndex, stopIndex;
                        startIndex = samplesThreadToProcess[x];
                        stopIndex = samplesThreadToProcess[x + 1];


                        threads.Add(new Thread(() =>
                        {
                            for (int i = startIndex; i < stopIndex + 1; i++)
                            {
                                samplesToProcess[i] = AudioTremoloEffect.TremoloProcess(samplesToProcess[i], ref timeElapsed);
                                AudioTremoloEffect.TremoloSweep();
                                fullTime += timeElapsed;
                            }
                        }));

                    }
                    for (int i = 0; i < threadsValue - 1; i++)
                    {
                        threads[i].Start();
                        threads[i].Join();
                        var first = i;
                        var range = threadsValue - 1;
                        var z = (double)first / range;
                        worker.ReportProgress((int)(z * 100));
                    }
                }
                else
                {
                    for (long i = beginIndex; i < endIndex; ++i)
                    {
                        samplesToProcess[i] = AudioTremoloEffect.TremoloProcess(samplesToProcess[i], ref timeElapsed);
                        AudioTremoloEffect.TremoloSweep();
                        if (i % 8000 == 0 && i != 0)
                        {
                            var first = i - beginIndex;
                            var range = endIndex - beginIndex;
                            var x = (double)first / range;
                            worker.ReportProgress((int)(x * 100));
                            System.Threading.Thread.Sleep(50);
                        }
                        fullTime += timeElapsed;
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + seconds + ":" + miliseconds + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Tremolo effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void SineWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            int freq = (int)passedParams[0];
            double ampl = (double)passedParams[1];

            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                int begIndex = (int)beginIndex;
                int enIndex = (int)endIndex;

                AudioSineWaveEffect.SineWaveInit((short)freq, (float)ampl);
                AudioSineWaveEffect.AddSineWave(samplesToProcess, audioItem.WaveFormat.SampleRate, begIndex, enIndex, ref fullTime);

                this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
                }));

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                ResetProgressBar();

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetBeginSpan().Milliseconds.ToString() + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed SineWave effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void ReverbEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            int delay = (int)passedParams[0];
            double decay = (double)passedParams[1];

            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                AudioReverbEffect.ReverbInit((short)delay, (float)decay);

                int begIndex = (int)beginIndex;
                int enIndex = (int)endIndex;

                AudioReverbEffect.AddReverb(samplesToProcess, begIndex, enIndex, ref fullTime);

                this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
                }));

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                ResetProgressBar();

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetBeginSpan().Milliseconds.ToString() + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Reverb effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        
        private void DistortionEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int elapsedTime = 0;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);
                float biggest = samplesToProcess.Max();

                AudioDistortionEffect.DistortionInit(biggest / 5f);

                if (threadsValue > 1)
                {
                    samplesCountForThreads = (int)((endIndex - beginIndex) / threadsValue);
                    samplesThreadToProcess = new int[threadsValue];

                    for (int i = 0; i < threadsValue; i++)
                    {
                        samplesThreadToProcess[i] = (int)beginIndex + (samplesCountForThreads * i);
                    }
                    samplesThreadToProcess[threadsValue - 1] = (int)endIndex - 1;

                    for (int x = 0; x < threadsValue - 1; x++)
                    {
                        int startIndex, stopIndex;
                        startIndex = samplesThreadToProcess[x];
                        stopIndex = samplesThreadToProcess[x + 1];


                        threads.Add(new Thread(() =>
                        {
                            for (int i = startIndex; i < stopIndex + 1; i++)
                            {
                                samplesToProcess[i] = AudioDistortionEffect.DistortionProcess(samplesToProcess[i], ref elapsedTime);
                                fullTime += elapsedTime;
                            }
                        }));

                    }

                    for (int i = 0; i < threadsValue - 1; i++)
                    {
                        threads[i].Start();
                        threads[i].Join();
                        var first = i;
                        var range = threadsValue - 1;
                        var z = (double)first / range;
                        worker.ReportProgress((int)(z * 100));
                    }
                }
                else
                {
                    for (long i = beginIndex; i < endIndex; ++i)
                    {
                        samplesToProcess[i] = AudioDistortionEffect.DistortionProcess(samplesToProcess[i], ref elapsedTime);
                        if (i % 8000 == 0 && i != 0)
                        {
                            var first = i - beginIndex;
                            var range = endIndex - beginIndex;
                            var x = (double)first / range;
                            worker.ReportProgress((int)(x * 100));
                            System.Threading.Thread.Sleep(50);
                        }
                        fullTime += elapsedTime;
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + seconds + ":" + miliseconds + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Distortion effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void DelayEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            double feedbackLevel = (double)passedParams[0];
            double delayLevel = (double)passedParams[1];

            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int elapsedTime = 0;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                AudioDelayEffect.DelayInit((float)feedbackLevel, (float)delayLevel);
                if (threadsValue > 1)
                {
                    samplesCountForThreads = (int)((endIndex - beginIndex) / threadsValue);
                    samplesThreadToProcess = new int[threadsValue];

                    for (int i = 0; i < threadsValue; i++)
                    {
                        samplesThreadToProcess[i] = (int)beginIndex + (samplesCountForThreads * i);
                    }
                    samplesThreadToProcess[threadsValue - 1] = (int)endIndex - 1;

                    for (int x = 0; x < threadsValue - 1; x++)
                    {
                        int startIndex, stopIndex;
                        startIndex = samplesThreadToProcess[x];
                        stopIndex = samplesThreadToProcess[x + 1];


                        threads.Add(new Thread(() =>
                        {
                            for (int i = startIndex; i < stopIndex + 1; i++)
                            {
                                samplesToProcess[i] = AudioDelayEffect.DelayProcess(samplesToProcess[i], ref elapsedTime);
                                fullTime += elapsedTime;
                            }
                        }));

                    }

                    for (int i = 0; i < threadsValue - 1; i++)
                    {
                        threads[i].Start();
                        threads[i].Join();
                        var first = i;
                        var range = threadsValue - 1;
                        var z = (double)first / range;
                        worker.ReportProgress((int)(z * 100));
                    }
                }
                else
                {
                    for (long i = beginIndex; i < endIndex; ++i)
                    {
                        samplesToProcess[i] = AudioDelayEffect.DelayProcess(samplesToProcess[i], ref elapsedTime);
                        if (i % 8000 == 0 && i != 0)
                        {
                            var first = i - beginIndex;
                            var range = endIndex - beginIndex;
                            var x = (double)first / range;
                            worker.ReportProgress((int)(x * 100));
                            System.Threading.Thread.Sleep(50);
                        }
                        fullTime += elapsedTime;
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + seconds + ":" + miliseconds + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Delay effect on audio sample! " + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void FlangerEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            List<object> passedParams = e.Argument as List<object>;
            int effectRate = (int)passedParams[0];
            int maxD = (int)passedParams[1];
            int minD = (int)passedParams[2];
            double fwv = (double)passedParams[3];
            double step = (double)passedParams[4];
            double fbv = (double)passedParams[5];

            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();
                float[] samplesToProcess;
                long beginIndex, endIndex;
                int seconds, miliseconds, minutes;
                int elapsedTime = 0;
                int fullTime = 0;
                SetDataForProcessing(out samplesToProcess, out minutes, out beginIndex, out endIndex, out seconds, out miliseconds);

                AudioFlangerEffect.FlangerInit((short)effectRate, 16000, (short)maxD, (short)minD, fwv, step, fbv);

                if (threadsValue > 1)
                {
                    samplesCountForThreads = (int)((endIndex - beginIndex) / threadsValue);
                    samplesThreadToProcess = new int[threadsValue];

                    for (int i = 0; i < threadsValue; i++)
                    {
                        samplesThreadToProcess[i] = (int)beginIndex + (samplesCountForThreads * i);
                    }
                    samplesThreadToProcess[threadsValue - 1] = (int)endIndex - 1;

                    for (int x = 0; x < threadsValue - 1; x++)
                    {
                        int startIndex, stopIndex;
                        startIndex = samplesThreadToProcess[x];
                        stopIndex = samplesThreadToProcess[x + 1];


                        threads.Add(new Thread(() =>
                        {
                            for (int i = startIndex; i < stopIndex + 1; i++)
                            {
                                samplesToProcess[i] = AudioFlangerEffect.FlangerProcess(samplesToProcess[i], ref elapsedTime);
                                AudioFlangerEffect.FlangerSweep();
                                fullTime += elapsedTime;
                            }
                        }));

                    }

                    for (int i = 0; i < threadsValue - 1; i++)
                    {
                        threads[i].Start();
                        threads[i].Join();
                        var first = i;
                        var range = threadsValue - 1;
                        var z = (double)first / range;
                        worker.ReportProgress((int)(z * 100));
                    }
                }
                else
                {
                    for (long i = beginIndex; i < endIndex; ++i)
                    {
                        samplesToProcess[i] = AudioFlangerEffect.FlangerProcess(samplesToProcess[i], ref elapsedTime);
                        AudioFlangerEffect.FlangerSweep();
                        if (i % 8000 == 0 && i != 0)
                        {
                            var first = i - beginIndex;
                            var range = endIndex - beginIndex;
                            var x = (double)first / range;
                            worker.ReportProgress((int)(x * 100));
                            System.Threading.Thread.Sleep(50);
                        }
                        fullTime += elapsedTime;
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                var milisecondsTime = (double)fullTime / 1000;

                e.Result = "Selection : " + minutes + ":" + seconds + ":" + miliseconds + " - " + AudioItemManager.Instance.GetEndSpan().Minutes.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + ":" + AudioItemManager.Instance.GetEndSpan().Milliseconds.ToString() + "  " + " -> Processed Flanger effect on audio sample!" + "\nProcessing range length: " + samplesToProcess.Length.ToString() + "\nCpp library time elapsed: " + milisecondsTime.ToString() + " msec" + "\nThreads count: " + threadsValue.ToString();
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void SetDataForProcessing(out float[] samplesToProcess, out int minutes, out long beginIndex, out long endIndex, out int seconds, out int miliseconds)
        {
            samplesToProcess = new float[audioItem.OriginalAudioBuffer.Length];
            if (audioItem.ProcessedAudioBuffer != null)
            {
                samplesToProcess = audioItem.ProcessedAudioBuffer;
            }
            else
            {
                samplesToProcess = audioItem.OriginalAudioBuffer;
            }

            if(AudioItemManager.Instance.GetBeginSpan().Seconds < 0)
            {
                seconds = 0;
            }
            else
            {
                seconds = AudioItemManager.Instance.GetBeginSpan().Seconds;
            }

            if (AudioItemManager.Instance.GetBeginSpan().Milliseconds < 0)
            {
                miliseconds = 0;
            }
            else
            {
                miliseconds = AudioItemManager.Instance.GetBeginSpan().Milliseconds;
            }

            if(AudioItemManager.Instance.GetBeginSpan().Minutes < 0)
            {
                minutes = 0;
            }
            else
            {
                minutes = AudioItemManager.Instance.GetBeginSpan().Minutes;
            }


            beginIndex = (long)(((minutes * 60 + seconds + miliseconds/1000) / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
            endIndex = (long)(((AudioItemManager.Instance.GetEndSpan().Minutes * 60 + AudioItemManager.Instance.GetEndSpan().Seconds + AudioItemManager.Instance.GetEndSpan().Milliseconds/1000) / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
            beginIndex = (long)(beginIndex / 4);
            endIndex = (long)(endIndex / 4);
        }

        private float[] SetDataForProcessing()
        {
            float[] samplesToProcess = new float[audioItem.OriginalAudioBuffer.Length];

            if (audioItem.ProcessedAudioBuffer != null)
            {
                samplesToProcess = audioItem.ProcessedAudioBuffer;
            }
            else
            {
                samplesToProcess = audioItem.OriginalAudioBuffer;
            }

            return samplesToProcess;
        }

        public void SetThreadValue(int? value)
        {
            threadsValue = value.Value;
            threads.Clear();
        }

        public void AddChorusEffect()
        {
            //AudioChorusEffect.ChorusInit(0.5f, 0.1f);

            //audioItem = AudioItemManager.GetAudioItem();

            //float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            //for (int i = 0; i < samplesToProcess.Length; ++i)
            //{
            //    samplesToProcess[i] = AudioChorusEffect.ChorusProcess(samplesToProcess[i]);
            //}

            //audioItem.ProcessedAudioBuffer = samplesToProcess;

            //AudioItemManager.SetAudioItem(audioItem);
        }

        public void AddPhaserEffect()
        {
            //AudioPhaserEffect.PhaserInit(0.3f, 0.8f);

            //audioItem = AudioItemManager.GetAudioItem();

            //float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            //for (int i = 0; i < samplesToProcess.Length; ++i)
            //{
            //    samplesToProcess[i] = AudioPhaserEffect.PhaserProcess(samplesToProcess[i]);
            //}

            //audioItem.ProcessedAudioBuffer = samplesToProcess;

            //AudioItemManager.SetAudioItem(audioItem);
        }

    }
}
