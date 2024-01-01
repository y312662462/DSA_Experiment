using System;
using System.Runtime.InteropServices;

namespace FusiSDK
{
    public delegate void OnSearchFinished(FusiHeadband[] headbands);
    public delegate void OnSearchError(FusiHeadbandError error);

    public delegate void OnWifiScanFinished(Wifi[] wifis);
    public delegate void OnWifiScanError(FusiHeadbandError error);
    public delegate void OnWifiConfigFinished(bool success);
    public delegate void OnSetStationModeFinished(bool success);

    public enum HeadbandConnectionState : int {
        Connecting = 0,
        Connected = 1,
        Interrupted = 2,
        Disconnected = 3
    };

    public enum HeadbandContactState
    {
        Contact = 0,
        NoContact = 1,
        Checking = 2
    };

    public enum HeadbandOrientation: int
    {
        Unknown = 0,
        BottomUp = 1,
        LeftArmEndUp = 2,
        LeftArmEndDown = 3,
        TopUp = 4,
        LeftFaceUp = 5,
        LeftFaceDown = 6
    };
    public enum HeadbandMode : int
    {
        OTA = -1,
        EEG = 0,
        ImpedanceTestREF = 1,
        ImpedanceTestRLD = 2
    };

    public enum HeadbandStationMode : int
    {
        AP = 0,
        STA = 1,
        APSTA = 2
    };

    public enum FirmwareUpdateStage : int
    {
        Begin = 0,
        Starting = 1,
        CheckingFirmwareInfo = 2,
        TransmittingFirmwareData = 3,
        Complete = 4
    }
    public enum FirmwareUpdateError : int
    {
        FWGeneral = -13,
        FWData = -12,
        FWDataOther = -11,
        FWInfo = -10,
        FWInfoOther = -9,
        PacketParsing = -8,
        OTACRC = -7,
        OTAParam = -6,
        OTACMD = -5,
        OTAAPNotAllowed = -4,
        OTADisabled = -3,
        BatteryLow = -2,
        ConnectionTimeout = -1
    }

    public class FusiHeadbandError
    {
        public int Code { get; }
        public string Message { get; }

        internal FusiHeadbandError(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        internal FusiHeadbandError(IntPtr cError)
        {
            FusiError error = (FusiError)Marshal.PtrToStructure(cError, typeof(FusiError));

            this.Code = error.code;
            this.Message = String.Copy(error.desc);
        }
    }

    public class Wifi
    {
        public string SSID { get; }
        public string Mac { get; }
        public string Encryption { get; }

        public Wifi(string ssid, string mac, string encryption)
        {
            this.SSID = ssid;
            this.Mac = mac;
            this.Encryption = encryption;
        }
        internal Wifi(IntPtr cInfo)
        {
            WifiInfo info = (WifiInfo)Marshal.PtrToStructure(cInfo, typeof(WifiInfo));

            this.SSID = String.Copy(info.ssid);
            this.Mac = String.Copy(info.mac);
            this.Encryption = String.Copy(info.encryption);
        }
    }
    public class EEG
    {
        public double SampleRate { get; }
        public double[] Data { get; }
        public double PGA { get; }

        internal EEG(IntPtr eegData)
        {
            EEGData data = (EEGData)Marshal.PtrToStructure(eegData, typeof(EEGData));
            SampleRate = data.sample_rate;
            PGA = data.pga;
            Data = new double[data.size];
            Marshal.Copy(data.data, Data, 0, data.size);

        }
    }

    public class BrainWave
    {
        public double Delta { get; }
        public double Theta { get; }
        public double Alpha { get; }
        public double LowBeta { get; }
        public double HighBeta { get; }
        public double Gamma { get; }

        internal BrainWave(IntPtr eegStats)
        {
            EEGStats stats = (EEGStats)Marshal.PtrToStructure(eegStats, typeof(EEGStats));
            Delta = stats.delta;
            Theta = stats.theta;
            Alpha = stats.alpha;
            LowBeta = stats.low_beta;
            HighBeta = stats.high_beta;
            Gamma = stats.gamma;
        }
    }

    public class FirmwareUpdateStatus
    {
        public FirmwareUpdateStage Stage { get; }
        public string Message { get; }
        public int Progress { get; }

        internal FirmwareUpdateStatus(FirmwareUpdateStage stage, string message, int progress)
        {
            Stage = stage;
            Message = message;
            Progress = progress;
        }
    }

    
}