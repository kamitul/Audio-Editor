using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.Utility
{
    public class SampleAggregator
    {
        private float volumeLeftMaxValue;
        private float volumeLeftMinValue;
        private float volumeRightMaxValue;
        private float volumeRightMinValue;
        private Complex[] channelData;
        private int bufferSize;
        private int binaryExponentitation;
        private int channelDataPosition;

        public SampleAggregator(int bufferSize)
        {
            this.bufferSize = bufferSize;
            binaryExponentitation = (int)Math.Log(bufferSize, 2);
            channelData = new Complex[bufferSize];
        }

        public void Clear()
        {
            volumeLeftMaxValue = float.MinValue;
            volumeRightMaxValue = float.MinValue;
            volumeLeftMinValue = float.MaxValue;
            volumeRightMinValue = float.MaxValue;
            channelDataPosition = 0;
        }

        public void Add(float leftValue, float rightValue)
        {
            if (channelDataPosition == 0)
            {
                volumeLeftMaxValue = float.MinValue;
                volumeRightMaxValue = float.MinValue;
                volumeLeftMinValue = float.MaxValue;
                volumeRightMinValue = float.MaxValue;
            }

            channelData[channelDataPosition].X = (leftValue + rightValue) / 2.0f;
            channelData[channelDataPosition].Y = 0;
            channelDataPosition++;

            volumeLeftMaxValue = Math.Max(volumeLeftMaxValue, leftValue);
            volumeLeftMinValue = Math.Min(volumeLeftMinValue, leftValue);
            volumeRightMaxValue = Math.Max(volumeRightMaxValue, rightValue);
            volumeRightMinValue = Math.Min(volumeRightMinValue, rightValue);

            if (channelDataPosition >= channelData.Length)
            {
                channelDataPosition = 0;
            }
        }

        public float LeftMaxVolume
        {
            get { return volumeLeftMaxValue; }
        }

        public float LeftMinVolume
        {
            get { return volumeLeftMinValue; }
        }

        public float RightMaxVolume
        {
            get { return volumeRightMaxValue; }
        }

        public float RightMinVolume
        {
            get { return volumeRightMinValue; }
        }
    }
}
