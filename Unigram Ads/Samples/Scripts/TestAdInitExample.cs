using UnityEngine;
using UnityEngine.UI;
using UnigramAds.Core;
using UnigramAds.Core.Adapters;
using UnigramAds.Common;
using UnigramAds.Utils;

namespace UnigramAds.Demo
{
    public sealed class TestAdInitExample : MonoBehaviour
    {
        [SerializeField, Space] private Button _watchAdButton;
        [SerializeField] private Button _watchRewaardAdButton;
        [SerializeField, Space] private Text _fakeBalanceBar;

        private UnigramAdsSDK _unigramAds;

        private IVideoAd _videoAd;
        private IRewardVideoAd _rewardAd;

        private readonly int REWARD_AMOUNT = 15;
        private readonly string FAKE_BALANCE_SAVE_KEY = UnigramUtils.FAKE_BALANCE_SAVE_KEY;

        private void OnDestroy()
        {
            var currentBalance = int.Parse(_fakeBalanceBar.text);

            PlayerPrefs.SetInt(FAKE_BALANCE_SAVE_KEY, currentBalance);

            _watchAdButton.onClick.RemoveListener(WatchAd);
            _watchRewaardAdButton.onClick.RemoveListener(WatchRewardAd);
        }

        private void Start()
        {
            var loadedBalance = PlayerPrefs.GetInt(FAKE_BALANCE_SAVE_KEY, 0);

            SetBalance(loadedBalance);

            _watchAdButton.onClick.AddListener(WatchAd);
            _watchRewaardAdButton.onClick.AddListener(WatchRewardAd);

            _unigramAds = new UnigramAdsSDK.Builder("demo_inter", 
                "demo_reward", "demo_banner")
                .WithTestMode(AdTypes.RewardedVideo)
                .Build((isSuccess) =>
                {
                    Debug.Log($"Sdk initialized with status: {isSuccess}");

                    _videoAd = new RewardAdAdapter();
                    _rewardAd = new RewardAdAdapter();
                });
        }

        private void WatchAd()
        {
            if (_videoAd == null)
            {
                Debug.LogWarning("Video ad adapter is not exist");

                return;
            }

            _videoAd.Show();
        }

        private void WatchRewardAd()
        {
            if (_rewardAd == null)
            {
                Debug.LogWarning("Reward ad adapter is not exist");

                return;
            }

            _rewardAd.Show(OnRewardAdFinished);
        }

        private void SetBalance(int amount)
        {
            _fakeBalanceBar.text = amount.ToString();
        }

        private void OnRewardAdFinished()
        {
            Debug.Log("Ad watched, start fetching reward");

            var currentBalance = int.Parse(_fakeBalanceBar.text);

            Debug.Log($"Current balance: {currentBalance}");

            currentBalance += REWARD_AMOUNT;

            Debug.Log($"Updated balance: {currentBalance}");

            SetBalance(currentBalance);
        }
    }
}