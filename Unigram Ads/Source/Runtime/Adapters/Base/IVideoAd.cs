using System;

namespace UnigramAds.Core.Adapters
{
    public interface IVideoAd
    {
        void Show();
        void Destroy();

        event Action OnShowStarted;
        event Action OnShowFinished;

        event Action<string> OnShowFailed;
        event Action OnSkipped;

        event Action OnNotAvailable;
        event Action OnShowSpamDetected;
    }
}