using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows;
using UnityEngine;

namespace FusiSDK
{
    public class FusiHeadband
    {
        public string Mac { get; }
        public string Name { get; private set; }
        public string IP { get; private set; }

        public HeadbandConnectionState ConnectionState { get; private set; } = HeadbandConnectionState.Disconnected;
        public bool IsImpedanceTestEnabled { get; private set; } = true;

        private static Dictionary<string, IntPtr> devicePtrDict = new Dictionary<string, IntPtr>();
        private static Dictionary<string, FusiHeadband> deviceDict = new Dictionary<string, FusiHeadband>();

        private IFusiHeadbandListener Listener;

        private FusiHeadband(IntPtr cDeviceInfo)
        {
            FusiDeviceInfo info = (FusiDeviceInfo)Marshal.PtrToStructure(cDeviceInfo, typeof(FusiDeviceInfo));
            this.Mac = info.mac;
            this.Name = info.name;
            this.IP = info.ip;
        }

        private FusiHeadband(string mac, string name, string ip)
        {
            this.Mac = mac;
            this.Name = name;
            this.IP = ip;
        }

        public static FusiHeadband CreateFusiHeadband(string mac, string name, string ip)
        {

            if (deviceDict.ContainsKey(mac))
            {
                var device = deviceDict[mac];
                device.Name = name;
                if (!device.IP.Equals(ip))
                {
                    device.IP = ip;
                    if (devicePtrDict.ContainsKey(mac))
                    {
                        var devicePtr = devicePtrDict[mac];
                        Bridging.set_ip(devicePtr, ip);
                    }
                    if (device.ConnectionState != HeadbandConnectionState.Disconnected) {
                        device.Disconnect();
                        Task.Delay(500).ContinueWith( _ => { device.Connect(); });
                    }
                }
                return device;
            }
            var headband = new FusiHeadband(mac, name, ip);
            deviceDict[mac] = headband;
            return headband;
        }

        public void UnsetListener() {
            SetListener(null);
        }
        public void SetListener(IFusiHeadbandListener listener)
        {
            Listener = listener;
            //Debug.Log(devicePtrDict.ContainsKey(Mac));
            if (devicePtrDict.ContainsKey(Mac))
            {
                var devicePtr = devicePtrDict[Mac];
                //Debug.Log(listener==null);
                if (listener != null)
                {
                    Bridging.set_attention_callback(devicePtr, OnAttentionInternalCallback);

                    Bridging.set_meditation_callback(devicePtr, OnMeditationInternalCallback);

                    Bridging.set_blink_callback(devicePtr, OnBlinkInternalCallback);

                    Bridging.set_powerband_noise_callback(devicePtr, OnSignalQualityWarningInternalCallback);

                    Bridging.set_eeg_data_callback(devicePtr, OnEEGDataInternalCallback);

                    Bridging.set_eeg_stats_callback(devicePtr, OnEEGStatsInternalCallback);

                    Bridging.set_device_orientation_callback(devicePtr, OnOrientationChangeInternalCallback);

                    Bridging.set_error_callback(devicePtr, OnErrorInternalCallback);

                    Bridging.set_device_contact_state_callback(devicePtr, OnContactStateChangeInternalCallback);
                    // Update firmware callbacks are set after calling UpdateFirmware.
                } else {
                    Bridging.set_attention_callback(devicePtr, null);

                    Bridging.set_meditation_callback(devicePtr, null);

                    Bridging.set_blink_callback(devicePtr, null);

                    Bridging.set_powerband_noise_callback(devicePtr, null);

                    Bridging.set_eeg_data_callback(devicePtr, null);

                    Bridging.set_eeg_stats_callback(devicePtr, null);

                    Bridging.set_device_orientation_callback(devicePtr, null);

                    Bridging.set_error_callback(devicePtr, null);

                    Bridging.set_device_contact_state_callback(devicePtr, null);
                    // Update firmware callbacks are set after calling UpdateFirmware.
                }

            }
        }

        public double GetBatteryLevel()
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.get_battery_level(devicePtr);
            }
            else
            {
                throw new NullReferenceException("FusiHeadband:calling GetBatteryLevel before connecting to device.");
            }
        }

        public string GetFirmwareInfo()
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                string cVersion = Bridging.get_firmware_info(devicePtr);
                return String.Copy(cVersion);
            }
            else
            {
                throw new NullReferenceException("FusiHeadband:calling GetFirmwareInfo before connecting to device.");
            }
        }

        public string GetFirmwareVersion(){
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                string cVersion = Bridging.get_firmware_version(devicePtr);
                return String.Copy(cVersion);
            }
            else
            {
                throw new NullReferenceException("FusiHeadband:calling GetFirmwareVersion before connecting to device.");
            }
        }


        public double GetAlphaPeak() {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.get_alpha_peak(devicePtr);
            }else{
                return -1;
            }
        }

        public HeadbandMode GetHeadbandMode()
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.get_device_mode(devicePtr);
            }
            return HeadbandMode.EEG;
        }

        public void SetImpedanceTestEnabled(bool enabled)
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IsImpedanceTestEnabled = enabled;
                int impedanceTestEnabled = enabled ? 1 : 0;

                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                Bridging.set_impedance_test_enabled(devicePtr, impedanceTestEnabled);
            }
            else
            {
                throw new NullReferenceException("FusiHeadband:calling GetOrientation before connecting device.");
            }
        }

        public bool IsCharging()
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.is_charging(devicePtr) > 0;
            } else {
                throw new NullReferenceException("FusiHeadband:calling GetOrientation before connecting device.");
            }
        }

        public HeadbandOrientation GetOrientation()
        {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.get_device_orientation(devicePtr);
            }
            else
            {
                throw new NullReferenceException("FusiHeadband:calling GetOrientation before connecting device.");
            }
        }

        public HeadbandContactState GetContactState() {
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                return Bridging.get_device_contact_state(devicePtr);
            } else {
                throw new NullReferenceException("FusiHeadband:calling GetContactState before connecting device.");
            }
        }

        public bool SetForeheadLEDColor(uint red, uint green, uint blue)
        {

            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                Debug.Log("FusiHeadband.devicePtrDict.ContainsKey(Mac)");
                if (ConnectionState == HeadbandConnectionState.Connected)
                {
                    IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                    uint color = (red << 24) ^ (green << 16) ^ (blue << 8);
                    Bridging.fusi_set_forehead_led_color(devicePtr, color);
                    return true;
                } else {
                    Console.WriteLine("FusiHeadband:SetForeheadLEDColor:Failed, device connection is interrupted.");
                    Debug.Log("FusiHeadband:SetForeheadLEDColor:Failed, device connection is interrupted.");
                    return false;
                }
            } else {
                throw new NullReferenceException("Error:SetForeheadLEDColor:headband unavailable for:" + Mac);
            }
        }

        public void Connect()
        {
            if (ConnectionState == HeadbandConnectionState.Disconnected)
            {
                Console.WriteLine("FusiHeadband connecting ...");
                Debug.Log("FusiHeadband connecting ...");
                IntPtr devicePtr;
                if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
                {
                    Debug.Log("ContainsKey(Mac)");
                    devicePtr = FusiHeadband.devicePtrDict[Mac];
                } else {
                    Debug.Log("Dont_ContainsKey(Mac)");
                    FusiDeviceInfo info = new FusiDeviceInfo { mac = Mac, name = Name, ip = IP };
                    //Debug.Log("info name:"+info.mac);
                    devicePtr = Bridging.fusi_device_create(info);
                    //Debug.Log("create info"+devicePtr.ToString());
                    int enabled = IsImpedanceTestEnabled ? 1 : 0;
                    //Debug.Log("enabled:"+enabled);
                    Bridging.set_impedance_test_enabled(devicePtr, enabled);
                    //Debug.Log("set_impedance_test_enabled");
                    FusiHeadband.devicePtrDict[Mac] = devicePtr;
                    //Debug.Log("put into devicePtrDict");
                }
                Bridging.fusi_connect(devicePtr, OnConnectionChangeInternalCallback, 1);
            }
            else {
                Console.WriteLine("FusiHeadband:Connect:Device already connected");
                Debug.Log("FusiHeadband:Connect:Device already connected");
            }
            //Debug.Log(Listener==null);
            if(Listener != null) SetListener(Listener);
        }

        public void Disconnect()
        {
            if (ConnectionState != HeadbandConnectionState.Disconnected)
            {
                Console.WriteLine("FusiHeadband disconnecting...");
                if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
                {
                    IntPtr devicePtr = FusiHeadband.devicePtrDict[Mac];
                    Bridging.fusi_disconnect(devicePtr);
                    ConnectionState = HeadbandConnectionState.Disconnected;
                } else {
                    throw new InvalidProgramException("Device connected by device pointer is lost");
                }
            } else  {
                Console.WriteLine("FusiHeadband:Disconnect:Device is already disconnected...");
            }
        }
    
        private static OnWifiScanFinished OnWifiScanFinishedCallback = null; // Only one scan is allowed
        private static OnWifiScanError OnWifiScanErrorCallback = null;
        private static OnWifiConfigFinished OnWifiConfigFinishedCallback = null;

        private static readonly on_wifi_scan_finished OnWifiScanFinishedInternalCallback = new on_wifi_scan_finished(OnWifiScanFinishedInternal);

        private static void OnWifiScanFinishedInternal(IntPtr wifiInfo, int count, IntPtr error)
        {
            if(count < 0)
            {
                Wifi[] wifiList = new Wifi[0];
                FusiHeadbandError scanError = new FusiHeadbandError(error);
                OnWifiScanFinishedCallback?.Invoke(wifiList);
                OnWifiScanErrorCallback?.Invoke(scanError);
            }
            else
            {
                WifiInfo[] wifiInfoList = Bridging.MarshalArray<WifiInfo>(wifiInfo, count);
                Wifi[] wifiList = wifiInfoList.Select(info => new Wifi(info.ssid, info.mac, info.encryption)).ToArray();
                OnWifiScanFinishedCallback?.Invoke(wifiList);
            }
            OnWifiScanFinishedCallback = null;
            OnWifiScanErrorCallback = null;
        }

        public void ScanWifi(OnWifiScanFinished onFinish, OnWifiScanError onError){
            OnWifiScanFinishedCallback = onFinish;
            OnWifiScanErrorCallback = onError;

            FusiDeviceInfo info = new FusiDeviceInfo { mac = Mac, name = Name, ip = IP };
            Bridging.fusi_scan_wifi(info, OnWifiScanFinishedInternalCallback);
        }

        private static readonly on_switched_wifi OnWifiConfigFinishedInternalCallback = new on_switched_wifi(OnWifiConfigFinishedInternal);
        private static void OnWifiConfigFinishedInternal(int result)
        {
            OnWifiConfigFinishedCallback?.Invoke(result == 0);
            OnWifiConfigFinishedCallback = null;
        }

        public void SwitchWifi(Wifi wifi, string password, OnWifiConfigFinished onFinish)
        {
            OnWifiConfigFinishedCallback = onFinish;
            WifiInfo wifiInfo = new WifiInfo { ssid = wifi.SSID, mac = wifi.Mac, encryption = wifi.Encryption, password = password };
            FusiDeviceInfo deviceInfo = new FusiDeviceInfo { mac = Mac, name = Name, ip = IP };
            Bridging.fusi_switch_wifi(deviceInfo, wifiInfo, OnWifiConfigFinishedInternalCallback);
        }

        private static OnSetStationModeFinished OnSetStationModeFinishedCallback = null;
        // TODO: Impliment
        internal void SetStationMode(HeadbandStationMode mode, OnSetStationModeFinished onFinish)
        {
            OnSetStationModeFinishedCallback = onFinish;
            Console.WriteLine("FusiHeadband:SetStationMode:Setting station mode to:" + mode.ToString());
        }
        // TODO: Impliment
        internal void Restart() { }

        public void UpdateFirmware(byte[] data)
        {
            IntPtr devicePtr;
            if (FusiHeadband.devicePtrDict.ContainsKey(Mac))
            {
                devicePtr = devicePtrDict[Mac];
            } else {
                FusiDeviceInfo info = new FusiDeviceInfo { mac = Mac, name = Name, ip = IP};
                devicePtr = Bridging.fusi_device_create(info);
                int enabled = IsImpedanceTestEnabled ? 1 : 0;
                Bridging.set_impedance_test_enabled(devicePtr, enabled);
                FusiHeadband.devicePtrDict[Mac] = devicePtr;
                /*
                IntPtr cInfo = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                try
                {
                    Marshal.StructureToPtr(info, cInfo, false);
                    devicePtr = Bridging.fusi_device_create(cInfo);
                    int enabled = IsImpedanceTestEnabled ? 1 : 0;
                    Bridging.set_impedance_test_enabled(devicePtr, enabled);
                    FusiHeadband.devicePtrDict[Mac] = devicePtr;
                } finally {
                    Marshal.FreeHGlobal(cInfo);
                }
                */
            }

            IntPtr unmanagedFirmwareData = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, unmanagedFirmwareData, data.Length);
            Bridging.fusi_update_firmware(devicePtr, unmanagedFirmwareData, data.Length, OnFirmwareUpdateStatusChangeInternalCallback);

            Marshal.FreeHGlobal(unmanagedFirmwareData);
            
        }

        private static readonly on_attention OnAttentionInternalCallback = new on_attention(OnAttentionInternal);
        private void OnAttention(double attention)
        {
            Console.WriteLine("Attention:" + attention.ToString());
            Debug.Log("Attention:" + attention.ToString());
        }
        private static void OnAttentionInternal(string deviceMac, double attention)
        {
            Debug.Log("AAA0");
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnAttention(attention);
                Debug.Log(headband.Listener==null);
                if (headband.Listener != null) headband.Listener.OnAttention(attention);
            }
            else
            {
                throw new NullReferenceException("Error:OnAttention:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_meditation OnMeditationInternalCallback = new on_meditation(OnMeditationInternal);
        private void OnMeditation(double meditation)
        {
            Console.WriteLine("Meditation:" + meditation.ToString());
        }
        private static void OnMeditationInternal(string deviceMac, double meditation)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnMeditation(meditation);
                if (headband.Listener != null) headband.Listener.OnMeditation(meditation);
            }
            else
            {
                throw new NullReferenceException("Error:OnMeditation:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_blink OnBlinkInternalCallback = new on_blink(OnBlinkInternal);
        private void OnBlink()
        {
            Console.WriteLine("Blink:");
        }
        private static void OnBlinkInternal(string deviceMac)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnBlink();
                if (headband.Listener != null) headband.Listener.OnBlink();
            }
            else
            {
                throw new NullReferenceException("Error:OnBlink:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_powerband_noise OnSignalQualityWarningInternalCallback = new on_powerband_noise(OnSignalQualityWarningInternal);
        private void OnSignalQualityWarning()
        {
            Console.WriteLine("SignalQualityWarning:");
        }
        private static void OnSignalQualityWarningInternal(string deviceMac)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnSignalQualityWarning();
                if (headband.Listener != null) headband.Listener.OnSignalQualityWarning();
            }
            else
            {
                throw new NullReferenceException("Error:OnSignalQualityWarning:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_eeg_data OnEEGDataInternalCallback = new on_eeg_data(OnEEGDataInternal);
        private void OnEEGData(EEG data)
        {
            Console.WriteLine("EEG data:" + data.ToString());
        }
        private static void OnEEGDataInternal(string deviceMac, IntPtr data)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                EEG eegData = new EEG(data);
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnEEGData(eegData);
                if (headband.Listener != null) headband.Listener.OnEEGData(eegData);
            }
            else
            {
                throw new NullReferenceException("Error:OnEEGStatsInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_eeg_stats OnEEGStatsInternalCallback = new on_eeg_stats(OnEEGStatsInternal);
        private void OnBrainWave(BrainWave wave)
        {
            Console.WriteLine("Brain wave:" + wave.ToString());
        }
        private static void OnEEGStatsInternal(string deviceMac, IntPtr stats)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                BrainWave wave = new BrainWave(stats);
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnBrainWave(wave);
                if (headband.Listener != null) headband.Listener.OnBrainWave(wave);
            }
            else
            {
                throw new NullReferenceException("Error:OnEEGStatsInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_error OnErrorInternalCallback = new on_error(OnErrorInternal);
        private void OnError(FusiHeadbandError error)
        {
            Console.WriteLine("Error:" + error.Message);
        }
        private static void OnErrorInternal(string deviceMac, IntPtr error)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadbandError headbandError = new FusiHeadbandError(error);
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnError(headbandError);
                if (headband.Listener != null) headband.Listener.OnError(headbandError);
            }
            else
            {
                throw new NullReferenceException("Error:OnErrorInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_device_connection_change OnConnectionChangeInternalCallback = new on_device_connection_change(OnConnectionChangeInternal);
        private void OnConnectionChange(HeadbandConnectionState state)
        {
            ConnectionState = state;
            Console.WriteLine("Connection state:" + state.ToString());
            Debug.Log("Connection state:" + state.ToString());
        }
        private static void OnConnectionChangeInternal(string deviceMac, HeadbandConnectionState state)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnConnectionChange(state);
                if (headband.Listener != null) headband.Listener.OnConnectionChange(state);
            }
            else
            {
                throw new NullReferenceException("Error:OnConnectionChangeInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_orientation_change OnOrientationChangeInternalCallback = new on_orientation_change(OnOrientationChangeInternal);
        private void OnOrientationChange(HeadbandOrientation orientation)
        {
            Console.WriteLine("Headband orientation:" + orientation.ToString());
        }
        private static void OnOrientationChangeInternal(string deviceMac, HeadbandOrientation orientation)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnOrientationChange(orientation);
                if (headband.Listener != null) headband.Listener.OnOrientationChange(orientation);
            }
            else
            {
                throw new NullReferenceException("Error:OnOrientationChangeInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_device_contact_state_change OnContactStateChangeInternalCallback = new on_device_contact_state_change(OnContactStateChangeInternal);
        private void OnContactStateChange(HeadbandContactState state)
        {
            Console.WriteLine("Contact state:" + state.ToString());
        }
        private static void OnContactStateChangeInternal(string deviceMac, HeadbandContactState state)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                headband.OnContactStateChange(state);
                if (headband.Listener != null) headband.Listener.OnContactStateChange(state);
            }
            else
            {
                throw new NullReferenceException("Error:OnContactStateChangeInternal:headband unavailable for:" + deviceMac);
            }
        }

        private static readonly on_firmware_update_status OnFirmwareUpdateStatusChangeInternalCallback = new on_firmware_update_status(OnFirmwareUpdateStatusChangeInternal);

        // MARK: UpdateFirmwareInternal bridging calls
        private static void OnFirmwareUpdateStatusChangeInternal(string deviceMac, OTAStatus status, int progress)
        {
            if (FusiHeadband.deviceDict.ContainsKey(deviceMac))
            {
                FusiHeadband headband = FusiHeadband.deviceDict[deviceMac];
                if (headband.Listener != null)
                {
                    if ((int)status < 0)
                    {
                        FusiHeadbandError error = Factory.GetFirmwareUpdateError(status);
                        headband.Listener.OnFirmwareUpdateError(error);
                    }
                    else
                    {
                        FirmwareUpdateStatus updateStatus = Factory.GetFirmwareUpdateStatus(status, progress);
                        headband.Listener.OnFirmwareUpdateStatusChange(updateStatus);
                    }
                }
            } else {
                throw new NullReferenceException("Error:OnFirmwareUpdateStatusChangeInternal:headband unavailable for:" + deviceMac);
            }
        }
    }
}
