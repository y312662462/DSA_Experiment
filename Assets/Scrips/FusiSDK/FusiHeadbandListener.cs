namespace FusiSDK
{
    public interface IFusiHeadbandListener
    {
        void OnAttention(double attention);
        void OnMeditation(double meditation);
        void OnBlink();

        // Neuro feedback training APIs
        void OnEEGData(EEG data);
        void OnBrainWave(BrainWave wave);

        void OnError(FusiHeadbandError error);

        void OnConnectionChange(HeadbandConnectionState connectionState);
        void OnOrientationChange(HeadbandOrientation orientation);
        void OnContactStateChange(HeadbandContactState contactState);
        void OnSignalQualityWarning();

        // Firmware update delegates
        void OnFirmwareUpdateStatusChange(FirmwareUpdateStatus updateStatus);
        void OnFirmwareUpdateError(FusiHeadbandError error);
    }

    public abstract class FusiHeadbandListener : IFusiHeadbandListener
    {
        public virtual void OnAttention(double attention){}

        public virtual void OnEEGData(EEG data) { }

        // Neuro feedback training API
        public virtual void OnBrainWave(BrainWave wave) { }

        public virtual void OnBlink(){}

        public virtual void OnError(FusiHeadbandError error){}

        public virtual void OnMeditation(double meditation){}

        public virtual void OnConnectionChange(HeadbandConnectionState connectionState) { }

        public virtual void OnOrientationChange(HeadbandOrientation orientation) { }

        public virtual void OnContactStateChange(HeadbandContactState contactState) { }

        public virtual void OnSignalQualityWarning(){}

        public virtual void OnFirmwareUpdateStatusChange(FirmwareUpdateStatus updateStatus) { }
        public virtual void OnFirmwareUpdateError(FusiHeadbandError error) { }
    }
}
