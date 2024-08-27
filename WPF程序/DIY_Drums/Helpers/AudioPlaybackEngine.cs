using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIY_Drums.Helpers
{
    /// <summary>
    ///  借鉴自https://markheath.net/post/fire-and-forget-audio-playback-with
    /// </summary>
    public class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        private readonly string ASIO_DRIVER_NAME = "ASIO4ALL v2";
        /// <summary>
        /// Audio Playback Engine
        /// </summary>
        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            var asioDriverName = AsioOut.GetDriverNames();
            if (asioDriverName.Length == 0 || !asioDriverName.Contains(ASIO_DRIVER_NAME))
            {
                throw new ArgumentException("The driver for ASIO4ALL is not installed. Please go to the official website to install it");
            }
            outputDevice = new AsioOut(ASIO_DRIVER_NAME);
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        /// <summary>
        /// Fire and forget playback of sound
        /// </summary>
        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        /// <summary>
        /// Fire and forget playback of a cached sound
        /// </summary>
        public void PlaySound(CachedSound sound)
        {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            outputDevice.Dispose();
        }
    }
}
