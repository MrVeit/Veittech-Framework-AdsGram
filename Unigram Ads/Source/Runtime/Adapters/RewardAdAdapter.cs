using System;
using UnigramAds.Common;
using UnigramAds.Core.Bridge;
using UnigramAds.Utils;

namespace UnigramAds.Core.Adapters
{
    public sealed class RewardAdAdapter : IRewardVideoAd, IDisposable
    {
        public event Action OnShowFinished;
        public event Action OnShowStarted;

        public event Action<string> OnShowFailed;
        public event Action OnSkipped;

        public event Action OnNotAvailable;
        public event Action OnShowSpamDetected;

        public RewardAdAdapter()
        {
            AdsGramBridge.SubscribeToEvent(
                AdEventsTypes.Started, (eventType) =>
            {
                UnigramAdsLogger.Log("Current reward ad started");

                OnShowStarted?.Invoke();
            });

            AdsGramBridge.SubscribeToEvent(
                AdEventsTypes.Skipped, (eventType) =>
            {
                UnigramAdsLogger.LogWarning("Current reward ad skipped");

                OnSkipped?.Invoke();
            });

            AdsGramBridge.SubscribeToEvent(
                AdEventsTypes.NotAvailable, (eventType) =>
            {
                UnigramAdsLogger.Log("Reward ad is not available");

                OnNotAvailable?.Invoke();
            });

            AdsGramBridge.SubscribeToEvent(
                AdEventsTypes.TryNonStopWatch, (eventType) =>
            {
                UnigramAdsLogger.Log($"Spam for show reward ad detected");

                OnShowSpamDetected?.Invoke();
            });
        }

        public void Show()
        {
            if (!UnigramUtils.IsSupporedPlatform())
            {
                return;
            }

            Show(() => { OnShowFinished?.Invoke(); });
        }

        public void Show(Action adFinished)
        {
            AdsGramBridge.ShowAd(() =>
            {
                UnigramAdsLogger.Log("Current reward ad shown");

                adFinished?.Invoke();
            },
            (errorMessage) =>
            {
                UnigramAdsLogger.LogWarning($"Failed to show current reward ad");

                OnShowFailed?.Invoke(errorMessage);
            });
        }

        public void Destroy()
        {
            AdsGramBridge.DestroyAd();
        }

        public void Dispose()
        {
            AdsGramBridge.UnSubscribeFromEvent(
                AdEventsTypes.Started, (eventType) =>
            {
                UnigramAdsLogger.Log($"Current ad unsubscribed by {eventType} event");
            });

            AdsGramBridge.UnSubscribeFromEvent(
                AdEventsTypes.Skipped, (eventType) =>
            {
                UnigramAdsLogger.Log($"Current ad unsubscribed by {eventType} event");
            });

            AdsGramBridge.UnSubscribeFromEvent(
                AdEventsTypes.NotAvailable, (eventType) =>
            {
                UnigramAdsLogger.Log($"Current ad unsubscribed by {eventType} event");
            });

            AdsGramBridge.UnSubscribeFromEvent(
                AdEventsTypes.TryNonStopWatch, (eventType) =>
            {
                UnigramAdsLogger.Log($"Current ad unsubscribed by {eventType} event");
            });
        }
    }
}