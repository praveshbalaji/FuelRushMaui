using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace FuelRushMaui.Services
{
    public class SoundService
    {
        private readonly StorageService _storageService;
        private bool _isBgmPlaying = false;

#if WINDOWS
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern bool PlaySound(byte[] ptr, IntPtr hModule, int flags);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        private static extern long mciSendString(string command, System.Text.StringBuilder? returnString, int returnLength, IntPtr hwndCallback);

        private const int SND_ASYNC = 0x0001;
        private const int SND_MEMORY = 0x0004;
#endif

        public SoundService(StorageService storageService)
        {
            _storageService = storageService;
        }

        public void StartTokyoDriftBgm()
        {
            if (!_storageService.IsSoundEnabled()) return;
            if (_isBgmPlaying) return;

            Task.Run(async () =>
            {
                try
                {
                    string localPath = Path.Combine(FileSystem.CacheDirectory, "tokyo_drift_bgm.wav");
                    if (!File.Exists(localPath))
                    {
                        using var stream = await FileSystem.OpenAppPackageFileAsync("tokyo_drift_bgm.wav");
                        using var dest = File.Create(localPath);
                        await stream.CopyToAsync(dest);
                    }

#if WINDOWS
                    mciSendString("close tokyodrift", null, 0, IntPtr.Zero);
                    mciSendString($"open \"{localPath}\" type waveaudio alias tokyodrift", null, 0, IntPtr.Zero);
                    mciSendString("play tokyodrift repeat", null, 0, IntPtr.Zero);
                    _isBgmPlaying = true;
#elif IOS || MACCATALYST
                    var url = Foundation.NSUrl.FromFilename(localPath);
                    _iosBgmPlayer?.Stop();
                    _iosBgmPlayer?.Dispose();
                    _iosBgmPlayer = AVFoundation.AVAudioPlayer.FromUrl(url);
                    if (_iosBgmPlayer != null)
                    {
                        _iosBgmPlayer.NumberOfLoops = -1;
                        _iosBgmPlayer.Play();
                        _isBgmPlaying = true;
                    }
#endif
                }
                catch
                {
                    // Ignore background music errors if device audio is busy
                }
            });
        }

        private AVFoundation.AVAudioPlayer? _iosBgmPlayer;

        public void StopBgm()
        {
            _isBgmPlaying = false;
#if WINDOWS
            try
            {
                mciSendString("stop tokyodrift", null, 0, IntPtr.Zero);
                mciSendString("close tokyodrift", null, 0, IntPtr.Zero);
            }
            catch { }
#elif IOS || MACCATALYST
            try
            {
                _iosBgmPlayer?.Stop();
                _iosBgmPlayer?.Dispose();
                _iosBgmPlayer = null;
            }
            catch { }
#endif
        }

        public void PlayFuelPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(880, 140); // High A pitch chime
        }

        public void PlayCoinPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(1046, 100); // High C pitch ding
        }

        public void PlayNitroBoost()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(587, 280); // Low roar
        }

        public void PlayEngineRev()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(700, 160); // Engine rev tone
        }

        public void PlayShieldPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(1318, 160); // High E pitch
        }

        public void PlayCrash()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(180, 400); // Low crash thud
        }

        public void PlayLowFuelAlert()
        {
            if (!_storageService.IsSoundEnabled()) return;
            PlayToneAsync(440, 120); // Warning tone
        }

        private void PlayToneAsync(int frequency, int durationMs)
        {
            Task.Run(() =>
            {
                try
                {
                    int sampleRate = 22050;
                    int numSamples = sampleRate * durationMs / 1000;
                    short[] samples = new short[numSamples];

                    for (int i = 0; i < numSamples; ++i)
                    {
                        double t = (double)i / sampleRate;
                        double sine = Math.Sin(2 * Math.PI * frequency * t);
                        // Apply quick envelope decay
                        double env = 1.0 - ((double)i / numSamples);
                        samples[i] = (short)(sine * env * 28000);
                    }

#if WINDOWS
                    byte[] wavHeaderAndData = CreateWavByteArray(samples, sampleRate);
                    PlaySound(wavHeaderAndData, IntPtr.Zero, SND_ASYNC | SND_MEMORY);
#elif ANDROID
                    byte[] generatedSnd = new byte[2 * numSamples];
                    int idx = 0;
                    foreach (short val in samples)
                    {
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
#elif IOS || MACCATALYST
                    byte[] wavHeaderAndData = CreateWavByteArray(samples, sampleRate);
                    var nsData = Foundation.NSData.FromArray(wavHeaderAndData);
                    var player = AVFoundation.AVAudioPlayer.FromData(nsData);
                    if (player != null)
                    {
                        player.Play();
                        Task.Delay(durationMs + 120).ContinueWith(_ =>
                        {
                            try { player.Stop(); player.Dispose(); } catch { }
                        });
                    }
#endif
                }
                catch
                {
                    // Fail gracefully if audio device is busy
                }
            });
        }

        private static byte[] CreateWavByteArray(short[] samples, int sampleRate)
        {
            int subChunk2Size = samples.Length * 2;
            int chunkSize = 36 + subChunk2Size;

            using MemoryStream ms = new MemoryStream();
            using BinaryWriter bw = new BinaryWriter(ms);

            // RIFF header
            bw.Write(new char[] { 'R', 'I', 'F', 'F' });
            bw.Write(chunkSize);
            bw.Write(new char[] { 'W', 'A', 'V', 'E' });

            // fmt subchunk
            bw.Write(new char[] { 'f', 'm', 't', ' ' });
            bw.Write(16); // Subchunk1Size (16 for PCM)
            bw.Write((short)1); // AudioFormat (1 for PCM)
            bw.Write((short)1); // NumChannels (1 for Mono)
            bw.Write(sampleRate); // SampleRate
            bw.Write(sampleRate * 2); // ByteRate (SampleRate * NumChannels * BitsPerSample/8)
            bw.Write((short)2); // BlockAlign
            bw.Write((short)16); // BitsPerSample

            // data subchunk
            bw.Write(new char[] { 'd', 'a', 't', 'a' });
            bw.Write(subChunk2Size);
            foreach (short s in samples)
            {
                bw.Write(s);
            }

            return ms.ToArray();
        }
    }
}
