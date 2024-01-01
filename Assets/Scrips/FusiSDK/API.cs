using System;
using System.Collections.Generic;
using System.Linq;

namespace FusiSDK
{
    
    public class API
    {
        private static OnSearchFinished OnSearchFinishedCallback;
        private static OnSearchError OnSearchErrorCallback;
        private static on_found_fusi_devices OnSearchFinishedCallbackInternal = new on_found_fusi_devices(OnSearchFinishedInternal);

        public static void SearchDevices(OnSearchFinished onSearchFinished, OnSearchError onError)
        {
            OnSearchFinishedCallback = onSearchFinished;
            OnSearchErrorCallback = onError;

            Bridging.fusi_devices_search(OnSearchFinishedCallbackInternal, Constant.HEADBAND_SCAN_TIMEOUT);
        }

        public static void Config(Dictionary<string, string> config)
        {
            if (config.ContainsKey("headbandScanTimeout"))
            {
                string timeoutStr = config["headbandScanTimeout"];
                double timeout;
                if (Double.TryParse(timeoutStr, out timeout)) Constant.HEADBAND_SCAN_TIMEOUT = (int)timeout;
                else  Console.WriteLine("FusiSDK:Config:unable to parse headbandScanTimeout:{0}.", timeoutStr);
            }
        }
        
        private static void OnSearchFinishedInternal(IntPtr devices, int count, IntPtr err)
        {

            if(count < 0)
            {
                FusiHeadband[] deviceList = new FusiHeadband[0];
                
                OnSearchFinishedCallback?.Invoke(deviceList);
                OnSearchFinishedCallback = null;
                OnSearchErrorCallback?.Invoke(new FusiHeadbandError(err));
                OnSearchErrorCallback = null;

            } else {

                FusiDeviceInfo[] deviceInfoList = Bridging.MarshalArray<FusiDeviceInfo>(devices, count);
                FusiHeadband[] deviceList = deviceInfoList.Select(info => FusiHeadband.CreateFusiHeadband(info.mac, info.name, info.ip)).ToArray();
                OnSearchFinishedCallback?.Invoke(deviceList);
                OnSearchFinishedCallback = null;
                OnSearchErrorCallback = null;
            }
        }
    }
}
