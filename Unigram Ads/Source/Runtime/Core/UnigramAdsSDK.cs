using System;
using UnigramAds.Common;
using UnigramAds.Core.Bridge;
using UnigramAds.Utils;

namespace UnigramAds.Core
{
    public sealed class UnigramAdsSDK
    {
        public string AppId { get; private set; }
    
        public bool IsInitialized { get; private set; }
        public bool IsTestMode { get; private set; }

        public AdTypes DebugAdType { get; private set; }

        private UnigramAdsSDK(Builder builder)
        {
            this.IsInitialized = builder.IsInitialized;
            this.IsTestMode = builder.IsTestMode;
        }

        public sealed class Builder
        {
            internal string AppId { get; private set; }

            internal bool IsInitialized { get; private set; }
            internal bool IsTestMode { get; private set; }

            internal AdTypes DebugAdType { get; private set; }

            public Builder(string appId)
            {
                this.AppId = appId;
            }

            public Builder WithTestMode(AdTypes adType)
            {
                if (adType == AdTypes.None)
                {
                    UnityEngine.Debug.Log($"{UnigramAdsLogger.PREFIX} " +
                        $"Unsuppored ad type, test mode disabled");

                    return this;
                }

                this.IsTestMode = true;
                this.DebugAdType = adType;

                UnigramAdsLogger.Enabled();

                UnigramAdsLogger.Log($"Enabled test mode for ad type: {adType}");

                return this;
            }

            public UnigramAdsSDK Build(Action<bool> initializationFnished)
            {
                if (!UnigramUtils.IsSupporedPlatform())
                {
                    return null;
                }

                AdsGramBridge.Init(AppId, IsTestMode,
                    DebugAdType, (isSuccess) =>
                {
                    initializationFnished?.Invoke(isSuccess);

                    if (isSuccess)
                    {
                        IsInitialized = true;
                    }
                });

                return new UnigramAdsSDK(this);
            }
        }
    }
}