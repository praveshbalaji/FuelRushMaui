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
            BeepAsync(880, 120); // High A pitch chime
        }

        public void PlayCoinPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(1046, 80); // High C pitch ding
        }

        public void PlayNitroBoost()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(587, 200); // Low roar
        }

        public void PlayEngineRev()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(700, 150); // Engine rev tone
        }

        public void PlayShieldPickup()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(1318, 150); // E pitch
        }

        public void PlayCrash()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(150, 350); // Low crash thud
        }

        public void PlayLowFuelAlert()
        {
            if (!_storageService.IsSoundEnabled()) return;
            BeepAsync(440, 100); // Warning tone
        }

        private void BeepAsync(int frequency, int durationMs)
        {
            Task.Run(() =>
            {
                try
                {
#if WINDOWS
                    Console.Beep(frequency, durationMs);
#endif
                }
                catch
                {
                    // Fail gracefully if system audio device is busy
                }
            });
        }
    }
}
