using System;
using System.Threading.Tasks;

namespace FuelRushMaui.Services
{
    public class SoundService
    {
        private readonly StorageService _storageService;

        public SoundService(StorageService storageService)
        {
            _storageService = storageService;
        }

        public void PlayFuelPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(880, 120); // High A pitch chime
        }

        public void PlayCoinPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(1046, 80); // High C pitch ding
        }

        public void PlayNitroBoost()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(587, 250); // Low roar
        }

        public void PlayEngineRev()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(700, 150); // Engine rev tone
        }

        public void PlayShieldPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(1318, 150); // High E pitch
        }

        public void PlayCrash()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(180, 350); // Low crash thud
        }

        public void PlayLowFuelAlert()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(440, 100); // Warning tone
        }

        private void PlayToneAsync(int frequency, int durationMs)
        {
            Task.Run(() =>
            {
                try
                {
#if ANDROID
                    int sampleRate = 22050;
                    int numSamples = sampleRate * durationMs / 1000;
                    double[] sample = new double[numSamples];
                    byte[] generatedSnd = new byte[2 * numSamples];

                    for (int i = 0; i < numSamples; ++i)
                    {
                        sample[i] = Math.Sin(2 * Math.PI * i / (sampleRate / (double)frequency));
                    }

                    int idx = 0;
                    foreach (double dVal in sample)
                    {
                        short val = (short)(dVal * 32767);
                        generatedSnd[idx++] = (byte)(val & 0x00ff);
                        generatedSnd[idx++] = (byte)((val & 0xff00) >> 8);
                    }

                    var audioTrack = new Android.Media.AudioTrack(
                        Android.Media.Stream.Music,
                        sampleRate,
                        Android.Media.ChannelOut.Mono,
                        Android.Media.Encoding.Pcm16bit,
                        generatedSnd.Length,
                        Android.Media.AudioTrackMode.Static);

                    audioTrack.Write(generatedSnd, 0, generatedSnd.Length);
                    audioTrack.Play();

                    Task.Delay(durationMs + 80).ContinueWith(_ =>
                    {
                        try
                        {
                            audioTrack.Stop();
                            audioTrack.Release();
                        }
                        catch { }
                    });
#elif WINDOWS
                    Console.Beep(frequency, durationMs);
#endif
                }
                catch
                {
                    // Fail gracefully if audio device is busy
                }
            });
        }
    }
}
