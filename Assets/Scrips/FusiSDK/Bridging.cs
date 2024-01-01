using System;
using System.Runtime.InteropServices;

namespace FusiSDK
{
    delegate void on_found_fusi_devices(IntPtr deviceInfoList, int count, IntPtr error);
    delegate void on_switched_wifi(int result);
    delegate void on_wifi_scan_finished(IntPtr wifiInfoList, int count, IntPtr error);

    delegate void on_attention(string deviceMac, double attention);
    delegate void on_meditation(string deviceMac, double meditation);
    delegate void on_eeg_data(string deviceMac, IntPtr data);
    delegate void on_eeg_stats(string deviceMac, IntPtr stats);
    delegate void on_error(string deviceMac, IntPtr error);
    delegate void on_orientation_change(string deviceMac, HeadbandOrientation orientation);
    delegate void on_device_connection_change(string deviceMac, HeadbandConnectionState orientation);
    delegate void on_device_contact_state_change(string deviceMac, HeadbandContactState orientation);
    delegate void on_firmware_update_status(string deviceMac, OTAStatus status, int progress);

    delegate void on_blink(string deviceMac);
    delegate void on_powerband_noise(string deviceMac);

    delegate void on_log(string log);


    [StructLayout(LayoutKind.Sequential)]
    struct FusiDeviceInfo
    {
        internal string mac;
        internal string name;
        internal string ip;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct WifiInfo
    {
        internal string ssid;
        internal string mac;
        internal string encryption;
        internal string password;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FusiError
    {
        IntPtr device;
        internal int code;
        internal string desc;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct EEGData
    {
        internal int size;
        internal IntPtr data;
        internal double sample_rate;
        internal double pga;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct EEGStats
    {
        IntPtr device;
        internal double delta;
        internal double theta;
        internal double alpha;
        internal double low_beta;
        internal double high_beta;
        internal double gamma;
    }

    internal enum OTAStatus{
        ERROR_FW_GENERAL = -13,
        ERROR_FW_DATA = -12,
        ERROR_FW_DATA_OTHER = -11,
        ERROR_FW_INFO = -10,
        ERROR_FW_INFO_OTHER = -9,
        ERROR_PACKET_PARSING = -8,
        ERROR_OTA_CRC = -7,
        ERROR_OTA_PARAM = -6,
        ERROR_OTA_CMD = -5,
        ERROR_OTA_AP_NOT_ALLOWED = -4,
        ERROR_OTA_DISABLED = -3,
        ERROR_BATTERY_LOW = -2,
        ERROR_CONNECTION_TIMEOUT = -1,
        OTA_BEGIN = 0,
        OTA_PREPARING = 1,
        OTA_CHECKING_FIRMWARE_INFO = 2,
        OTA_TRANSMITING_FIRMWARE_DATA = 3,
        OTA_COMPLETE = 4
    };

    internal class Bridging
    {
        private const string LIBNAME = "libfusi_x64";
        internal static string PtrToString(IntPtr ptr) { return Marshal.PtrToStringAnsi(ptr); }
        internal static bool Is64() { return IntPtr.Size == 8; }
        internal static T[] MarshalArray<T>(IntPtr ptr, int count)
        {
            T[] objects = new T[count];
            long longPtr = ptr.ToInt64(); // Must work both on x86 and x64
            for (int i = 0; i < count; i++)
            {
                IntPtr thisPtr = new IntPtr(longPtr);
                objects[i] = (T)Marshal.PtrToStructure(thisPtr, typeof(T));
                longPtr += Marshal.SizeOf(typeof(T));
            }
            return objects;
        }

        [DllImport(LIBNAME, EntryPoint="hello")]
        internal static extern int hello(int num);

        [DllImport(LIBNAME, EntryPoint = "fusi_devices_search")]
        internal static extern int fusi_devices_search(on_found_fusi_devices cb, Int32 duration);

        [DllImport(LIBNAME, EntryPoint = "is_charging")]
        internal static extern int is_charging(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "set_ip")]
        internal static extern int set_ip(IntPtr devicePtr, string ip);

        [DllImport(LIBNAME, EntryPoint = "get_battery_level")]
        internal static extern double get_battery_level(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_alpha_peak")]
        internal static extern double get_alpha_peak(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_device_mode")]
        internal static extern HeadbandMode get_device_mode(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "is_impedance_test_enabled")]
        internal static extern int is_impedance_test_enabled(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "set_impedance_test_enabled")]
        internal static extern void set_impedance_test_enabled(IntPtr device, int enabled);

        [DllImport(LIBNAME, EntryPoint = "get_name")]
        internal static extern string get_name(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_firmware_info")]
        internal static extern string get_firmware_info(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_firmware_version")]
        internal static extern string get_firmware_version(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_device_orientation")]
        internal static extern HeadbandOrientation get_device_orientation(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "get_device_contact_state")]
        internal static extern HeadbandContactState get_device_contact_state(IntPtr device);

        [DllImport(LIBNAME, EntryPoint="set_attention_callback")]
        internal static extern int set_attention_callback(IntPtr device, on_attention cb);

        [DllImport(LIBNAME, EntryPoint="set_meditation_callback")]
        internal static extern int set_meditation_callback(IntPtr device, on_meditation cb);
        
        [DllImport(LIBNAME, EntryPoint="set_eeg_data_callback")]
        internal static extern int set_eeg_data_callback(IntPtr device, on_eeg_data cb);

        [DllImport(LIBNAME, EntryPoint= "set_eeg_stats_callback")]
        internal static extern int set_eeg_stats_callback(IntPtr device, on_eeg_stats cb);
        
        [DllImport(LIBNAME, EntryPoint="set_error_callback")]
        internal static extern int set_error_callback(IntPtr device, on_error cb);

        [DllImport(LIBNAME, EntryPoint="set_device_orientation_callback")]
        internal static extern int set_device_orientation_callback(IntPtr device, on_orientation_change cb);
        
        [DllImport(LIBNAME, EntryPoint="set_device_connection_callback")]
        internal static extern int set_device_connection_callback(IntPtr device, on_device_connection_change cb);

        [DllImport(LIBNAME, EntryPoint="set_device_contact_state_callback")]
        internal static extern int set_device_contact_state_callback(IntPtr device, on_device_contact_state_change cb);

        [DllImport(LIBNAME, EntryPoint="set_blink_callback")]
        internal static extern int set_blink_callback(IntPtr device, on_blink cb);

        [DllImport(LIBNAME, EntryPoint="set_blink_callback")]
        internal static extern int set_powerband_noise_callback(IntPtr device, on_powerband_noise cb);

        [DllImport(LIBNAME, EntryPoint="set_log_callback")]
        internal static extern int set_log_callback(on_log cb);

        [DllImport(LIBNAME, EntryPoint = "fusi_device_create")]
        internal static extern IntPtr fusi_device_create(FusiDeviceInfo info);   // internal static extern IntPtr fusi_device_create(IntPtr info);

        [DllImport(LIBNAME, EntryPoint = "fusi_switch_wifi")]
        internal static extern int fusi_switch_wifi(FusiDeviceInfo deviceInfo, WifiInfo wifiInfo, on_switched_wifi cb);

        [DllImport(LIBNAME, EntryPoint = "fusi_scan_wifi")]
        internal static extern int fusi_scan_wifi(FusiDeviceInfo info, on_wifi_scan_finished cb);

        [DllImport(LIBNAME, EntryPoint = "fusi_connect")]
        internal static extern int fusi_connect(IntPtr device, on_device_connection_change cb, int auto_reconnect);

        [DllImport(LIBNAME, EntryPoint = "fusi_disconnect")]
        internal static extern int fusi_disconnect(IntPtr device);

        [DllImport(LIBNAME, EntryPoint = "fusi_set_forehead_led_color")]
        internal static extern int fusi_set_forehead_led_color(IntPtr device, uint color);

        // Firmware update APIs
        [DllImport(LIBNAME, EntryPoint = "fusi_update_firmware")]
        internal static extern int fusi_update_firmware(IntPtr device, IntPtr firmwareData, int size, on_firmware_update_status statusCb);

    }
}
