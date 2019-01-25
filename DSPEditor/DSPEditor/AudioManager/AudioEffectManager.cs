using DSPEditor.Audio;
using DSPEditor.AudioEffects;
using DSPEditor.DSPAudioEffects;
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
            worker.DoWork += doWorkEventHandlers[0];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[0];
        }


        public void AddFlangerEffect()
        {
            worker.DoWork += doWorkEventHandlers[6];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[6];
        }


        public void AddTremoloEffect()
        {
            worker.DoWork += doWorkEventHandlers[4];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[4];
        }

        public void AddReverbEffect()
        {
            worker.DoWork += doWorkEventHandlers[2];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[2];
        }

        public void AddDistortionEffect()
        {
            worker.DoWork += doWorkEventHandlers[1];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[1];
        }

        public void AddWahWahEffect()
        {
            worker.DoWork += doWorkEventHandlers[5];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[5];
        }

        public void AddSineWaveEffect()
        {
            worker.DoWork += doWorkEventHandlers[3];
            worker.RunWorkerAsync();
            worker.DoWork -= doWorkEventHandlers[3];
        }

        private void WahWahEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            AudioWahWahEffect.AutoWahInit(2000,  /*Effect rate 2000*/
                 16000, /*Sampling Frequency*/
                 1000,  /*Maximum frequency*/
                 500,   /*Minimum frequency*/
                 4,     /*Q*/
                 0.707, /*Gain factor*/
                 10     /*Frequency increment*/);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            for (int i = 0; i < samplesToProcess.Length; ++i)
            {
                samplesToProcess[i] = AudioWahWahEffect.AutoWahProcess(samplesToProcess[i]);
                AudioWahWahEffect.AutoWahSweep();
                if (i % 10000 == 0 && i != 0)
                {
                    var x = (double)i / samplesToProcess.Length;
                    worker.ReportProgress((int)(x * 100));
                    System.Threading.Thread.Sleep(50);
                }
            }

            ResetProgressBar();

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);
        }

        private void TremoloWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            AudioTremoloEffect.TremoloInit(4000, 1);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            for (int i = 0; i < samplesToProcess.Length; ++i)
            {
                samplesToProcess[i] = AudioTremoloEffect.TremoloProcess(samplesToProcess[i]);
                AudioTremoloEffect.TremoloSweep();
                if (i % 10000 == 0 && i != 0)
                {
                    var x = (double)i / samplesToProcess.Length;
                    worker.ReportProgress((int)(x * 100));
                    System.Threading.Thread.Sleep(50);
                }
            }

            ResetProgressBar();

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);
        }

        private void SineWaveEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            AudioSineWaveEffect.SineWaveInit(1000, 0.25f);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            AudioSineWaveEffect.AddSineWave(samplesToProcess, audioItem.WaveFormat.SampleRate);

            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
            }));

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);

            ResetProgressBar();
        }

        private void ReverbEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            AudioReverbEffect.ReverbInit(3000, 0.5f);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            AudioReverbEffect.AddReverb(samplesToProcess);

            this.disp.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                MainWindow.progressBar.Value = 100; // Do all the ui thread updates here
            }));

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);

            ResetProgressBar();
        }

        private void DistortionEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            float biggest = samplesToProcess.Max();

            AudioDistortionEffect.DistortionInit(biggest / 1.5f);

            for (int i = 0; i < samplesToProcess.Length; ++i)
            {
                samplesToProcess[i] = AudioDistortionEffect.DistortionProcess(samplesToProcess[i]);
                if (i % 10000 == 0 && i != 0)
                {
                    var x = (double)i / samplesToProcess.Length;
                    worker.ReportProgress((int)(x * 100));
                    System.Threading.Thread.Sleep(50);
                }
            }

            ResetProgressBar();

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);
        }

        private void DelayEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            BackgroundWorker worker = sender as BackgroundWorker;

            AudioDelayEffect.DelayInit(85.6, 0.7, 0.7, 1);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            for (int i = 0; i < samplesToProcess.Length; ++i)
            {
                samplesToProcess[i] = AudioDelayEffect.DelayTask(samplesToProcess[i]);
                if (i % 10000 == 0 && i != 0)
                {
                    var x = (double)i / samplesToProcess.Length;
                    worker.ReportProgress((int)(x * 100));
                    System.Threading.Thread.Sleep(50);
                }
            }

            ResetProgressBar();

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);
        }

        private void FlangerEffectWorkHandler(object sender, DoWorkEventArgs e)
        {
            SetMaxProgressBar();

            AudioFlangerEffect.FlangerInit(500, 16000, 70, 2, 0.3, 1, 0.3);

            audioItem = AudioItemManager.GetAudioItem();

            float[] samplesToProcess = audioItem.OriginalAudioBuffer;

            for (int i = 0; i < samplesToProcess.Length; ++i)
            {
                samplesToProcess[i] = AudioFlangerEffect.FlangerProcess(samplesToProcess[i]);
                AudioFlangerEffect.FlangerSweep();
                if (i % 10000 == 0 && i != 0)
                {
                    var x = (double)i / samplesToProcess.Length;
                    worker.ReportProgress((int)(x * 100));
                    System.Threading.Thread.Sleep(50);
                }
            }

            ResetProgressBar();

            audioItem.ProcessedAudioBuffer = samplesToProcess;

            AudioItemManager.SetAudioItem(audioItem);
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
