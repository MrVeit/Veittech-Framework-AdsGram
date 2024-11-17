const adSonarBridge = {
    $adSonar: {
        AdsController: null,

        isAvailableSDK: function()
        {
            return !!this.AdsController;
        },

        init: function(callback)
        {
            AdsController = window.Sonar;

            if (!this.isAvailableSDK())
            {
                console.warn(`Failed to initialize Ad Sonar bridge`);

                dynCall('vi', callback, [0]);

                return;
            }

            console.log(`Ads Sonar bridge initialized`);

            dynCall('vi', callback, [1]);
        },

        showAd: function(adUnit, successCallback, errorCallback)
        {
            if (!this.isAvailableSDK())
            {
                console.warn('Ad Sonar sdk is not initialized');

                return;
            }

            const adPlacement = UTF8ToString(adUnit);

            AdsController.show({ adUnit: adPlacement }).then((result) =>
            {
                if (result.status === 'error')
                {
                    console.error(`Failed to show ad`);
                        
                    const errorPtr = allocate(intArrayFromString(
                            result.message), 'i8', ALLOC_NORMAL);

                    dynCall('vi', errorCallback, [errorPtr]);

                    _free(errorPtr);

                    return;
                }

                console.log(`Ad successfully shown, status: ${result.status}`);

                dynCall('v', successCallback);
            })
            .catch((error) =>
            {
                const errorPtr = allocate(
                        intArrayFromString(error), 'i8', ALLOC_NORMAL);

                dynCall('vi', errorCallback, [errorPtr]);

                _free(errorPtr);
            });
        },

        removeAd: function(adUnit, successCallback, errorCallback)
        {
            if (!this.isAvailableSDK())
            {
                console.warn(`Ad Sonar sdk is not initialized`);

                return;
            } 

            const adPlacement = UTF8ToString(adUnit);

            AdsController.remove({ adUnit: adUnit }).then((result) =>
            {   
                if (result.status === 'error')
                {
                    console.error(`Failed to remove ad unit`);
                        
                    const errorPtr = allocate(intArrayFromString(
                            result.message), 'i8', ALLOC_NORMAL);

                    dynCall('vi', errorCallback, [errorPtr]);

                    _free(errorPtr);

                    return;
                }

                console.log(`Ad unit successfully removed, status: ${result.status}`);

                dynCall('v', successCallback);
            })  
            .catch((error) =>
            {
                const errorPtr = allocate(
                        intArrayFromString(error), 'i8', ALLOC_NORMAL);

                dynCall('vi', errorCallback, [errorPtr]);

                _free(errorPtr);
            });
        }
    },

    Init: function(callback)
    {
        adSonar.init(callback);
    },

    ShowAd: function(adUnit, adShown, adShowFailed)
    {
        adSonar.showAd(adUnit, adShown, adShowFailed);
    },

    RemoveAd: function(adUnit, adRemoved, adRemoveFailed)
    {
        adSonar.removeAd(adUnit, adRemoved, adRemoveFailed);
    }
};

autoAddDeps(adSonarBridge, `$adSonar`);
mergeInto(LibraryManager.library, adSonarBridge);