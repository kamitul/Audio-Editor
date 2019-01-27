using DSPEditor.Audio;
using DSPEditor.AudioEffects;
using DSPEditor.DSPAudioEffects;
using DSPEditor.Utility;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        DoWorkEventHandler[] doWorkEventHandlers;

        private OutputLogWriter outputLogWriter = new OutputLogWriter();
        public static Action<string> WriteToOutputLog;

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
            if (WriteToOutputLog != null)
                WriteToOutputLog(e.Result as string);
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
            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                MainWindow.progressBar.Value = 0; // Do all the ui thread updates here
            }));
        }

        private void SetMaxProgressBar()
        {
            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                MainWindow.progressBar.Maximum = 100; // Do all the ui thread updates here
            }));
        }

        public void AddDelayEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[0];
            worker.RunWorkerAsync();
        }


        public void AddFlangerEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[6];
            worker.RunWorkerAsync();
        }


        public void AddTremoloEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[4];
            worker.RunWorkerAsync();
        }

        public void AddReverbEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[2];
            worker.RunWorkerAsync();
        }

        public void AddDistortionEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[1];
            worker.RunWorkerAsync();
        }

        public void AddWahWahEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[5];
            worker.RunWorkerAsync();
        }

        public void AddSineWaveEffect()
        {
            DisablePreviousWorks();
            worker.DoWork += doWorkEventHandlers[3];
            worker.RunWorkerAsync();
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
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                AudioWahWahEffect.AutoWahInit(2000,  /*Effect rate 2000*/
                     16000, /*Sampling Frequency*/
                     1000,  /*Maximum frequency*/
                     500,   /*Minimum frequency*/
                     4,     /*Q*/
                     0.707, /*Gain factor*/
                     10     /*Frequency increment*/);


                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                long beginIndex = (long)((AudioItemManager.Instance.GetBeginSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
                long endIndex = (long)((AudioItemManager.Instance.GetEndSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);

                for (long i = beginIndex; i < endIndex; ++i)
                {
                    samplesToProcess[i] = AudioWahWahEffect.AutoWahProcess(samplesToProcess[i]);
                    AudioWahWahEffect.AutoWahSweep();
                    if (i % 8000 == 0 && i != 0)
                    {
                        var first = i - beginIndex;
                        var range = endIndex - beginIndex;
                        var x = (double)first / range;
                        worker.ReportProgress((int)(x * 100));
                        System.Threading.Thread.Sleep(50);
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Wah-Wah effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void TremoloWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                audioItem = AudioItemManager.GetAudioItem();
            }));

            if (audioItem != null)
            {
                SetMaxProgressBar();

                AudioTremoloEffect.TremoloInit(4000, 1);



                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                long beginIndex = (long)((AudioItemManager.Instance.GetBeginSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
                long endIndex = (long)((AudioItemManager.Instance.GetEndSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);

                for (long i = beginIndex; i < endIndex; ++i)
                {
                    samplesToProcess[i] = AudioTremoloEffect.TremoloProcess(samplesToProcess[i]);
                    AudioTremoloEffect.TremoloSweep();
                    if (i % 8000 == 0 && i != 0)
                    {
                        var first = i - beginIndex;
                        var range = endIndex - beginIndex;
                        var x = (double)first / range;
                        worker.ReportProgress((int)(x * 100));
                        System.Threading.Thread.Sleep(50);
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Tremolo effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void SineWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                AudioSineWaveEffect.SineWaveInit(1000, 0.25f);

                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                AudioSineWaveEffect.AddSineWave(samplesToProcess, audioItem.WaveFormat.SampleRate);

                this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
            }));

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                ResetProgressBar();

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed SineWave effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void ReverbEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                AudioReverbEffect.ReverbInit(3000, 0.5f);


                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                AudioReverbEffect.AddReverb(samplesToProcess);

                this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
            }));

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                ResetProgressBar();

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Reverb effect on audio sample!";
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


                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                long beginIndex = (long)((AudioItemManager.Instance.GetBeginSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
                long endIndex = (long)((AudioItemManager.Instance.GetEndSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);

                float biggest = samplesToProcess.Max();

                AudioDistortionEffect.DistortionInit(biggest / 1.5f);

                for (long i = beginIndex; i < endIndex; ++i)
                {
                    samplesToProcess[i] = AudioDistortionEffect.DistortionProcess(samplesToProcess[i]);
                    if (i % 8000 == 0 && i != 0)
                    {
                        var first = i - beginIndex;
                        var range = endIndex - beginIndex;
                        var x = (double)first / range;
                        worker.ReportProgress((int)(x * 100));
                        System.Threading.Thread.Sleep(50);
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Distortion effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void DelayEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                BackgroundWorker worker = sender as BackgroundWorker;

                AudioDelayEffect.DelayInit(85.6, 0.7, 0.7, 1);


                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                long beginIndex = (long)((AudioItemManager.Instance.GetBeginSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
                long endIndex = (long)((AudioItemManager.Instance.GetEndSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);

                for (long i = beginIndex; i < endIndex; ++i)
                {
                    samplesToProcess[i] = AudioDelayEffect.DelayTask(samplesToProcess[i]);
                    if (i % 8000 == 0 && i != 0)
                    {
                        var first = i - beginIndex;
                        var range = endIndex - beginIndex;
                        var x = (double)first / range;
                        worker.ReportProgress((int)(x * 100));
                        System.Threading.Thread.Sleep(50);
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Delay effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }

        private void FlangerEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            audioItem = AudioItemManager.GetAudioItem();

            if (audioItem != null)
            {
                SetMaxProgressBar();

                AudioFlangerEffect.FlangerInit(500, 16000, 70, 2, 0.3, 1, 0.3);


                float[] samplesToProcess = audioItem.OriginalAudioBuffer;

                long beginIndex = (long)((AudioItemManager.Instance.GetBeginSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);
                long endIndex = (long)((AudioItemManager.Instance.GetEndSpan().Seconds / AudioItemManager.Instance.GetActiveStream().TotalTime.TotalSeconds) * AudioItemManager.Instance.GetActiveStream().Length);

                for (long i = beginIndex; i < endIndex; ++i)
                {
                    samplesToProcess[i] = AudioFlangerEffect.FlangerProcess(samplesToProcess[i]);
                    AudioFlangerEffect.FlangerSweep();
                    if (i % 8000 == 0 && i != 0)
                    {
                        var first = i - beginIndex;
                        var range = endIndex - beginIndex;
                        var x = (double)first / range;
                        worker.ReportProgress((int)(x * 100));
                        System.Threading.Thread.Sleep(50);
                    }
                }

                ResetProgressBar();

                audioItem.ProcessedAudioBuffer = samplesToProcess;

                AudioItemManager.SetAudioItem(audioItem);

                e.Result = "Selection : " + AudioItemManager.Instance.GetBeginSpan().Seconds.ToString() + "-" + AudioItemManager.Instance.GetEndSpan().Seconds.ToString() + " " + " -> Processed Flanger effect on audio sample!";
            }
            else
            {
                e.Result = "Audio not loaded!";
            }
        }


        public byte[] GetSamplesWaveData(float[] samples, int samplesCount)
        {
            var pcm = new byte[samplesCount * 2];
            int sampleIndex = 0,
                pcmIndex = 0;

            while (sampleIndex < samplesCount)
            {
                var outsample = (short)(samples[sampleIndex] * short.MaxValue);
                pcm[pcmIndex] = (byte)(outsample & 0xff);
                pcm[pcmIndex + 1] = (byte)((outsample >> 8) & 0xff);

                sampleIndex++;
                pcmIndex += 2;
            }

            return pcm;
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
