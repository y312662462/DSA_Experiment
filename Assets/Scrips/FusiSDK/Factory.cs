using System;
using System.Collections.Generic;
using System.Text;

namespace FusiSDK
{
    class Factory
    {
        internal static FirmwareUpdateStatus GetFirmwareUpdateStatus(OTAStatus status, int progress)
        {
            switch (status)
            {
                case OTAStatus.OTA_BEGIN:
                    return new FirmwareUpdateStatus(FirmwareUpdateStage.Begin, "Firmware update begin", progress);
                case OTAStatus.OTA_PREPARING:
                    return new FirmwareUpdateStatus(FirmwareUpdateStage.Starting, "Firmware update entering OTA mode", progress);
                case OTAStatus.OTA_CHECKING_FIRMWARE_INFO:
                    return new FirmwareUpdateStatus(FirmwareUpdateStage.CheckingFirmwareInfo, "Validating new firmware information", progress);
                case OTAStatus.OTA_TRANSMITING_FIRMWARE_DATA:
                    return new FirmwareUpdateStatus(FirmwareUpdateStage.TransmittingFirmwareData, "Transmitting firmware data", progress);
                case OTAStatus.OTA_COMPLETE:
                    return new FirmwareUpdateStatus(FirmwareUpdateStage.Complete, "Complete", progress);
                default:
                    throw new ArgumentException("Unknown OTAStatus");
            }
        }

        internal static FusiHeadbandError GetFirmwareUpdateError(OTAStatus error)
        {
            string msg;
            switch (error)
            {
                case OTAStatus.ERROR_FW_DATA:
                    msg = "Firmware data error";
                    break;
                case OTAStatus.ERROR_FW_DATA_OTHER:
                    msg = "Firmware data unknown error";
                    break;
                case OTAStatus.ERROR_FW_INFO:
                    msg = "Firmware information error";
                    break;
                case OTAStatus.ERROR_FW_INFO_OTHER:
                    msg = "Firmware information unknown error";
                    break;
                case OTAStatus.ERROR_PACKET_PARSING:
                    msg = "OTA packet parsing error";
                    break;
                case OTAStatus.ERROR_OTA_CRC:
                    msg = "OTA crc validation error";
                    break;
                case OTAStatus.ERROR_OTA_PARAM:
                    msg = "OTA parameter error";
                    break;
                case OTAStatus.ERROR_OTA_CMD:
                    msg = "OTA command error";
                    break;
                case OTAStatus.ERROR_OTA_DISABLED:
                    msg = "OTA mode is disabled";
                    break;
                case OTAStatus.ERROR_OTA_AP_NOT_ALLOWED:
                    msg = "OTA update is not allowed under AP mode";
                    break;
                case OTAStatus.ERROR_BATTERY_LOW:
                    msg = "Battery is too low";
                    break;
                case OTAStatus.ERROR_CONNECTION_TIMEOUT:
                    msg = "OTA update connection timeout";
                    break;
                default:
                    throw new ArgumentException("Unknown error OTAStatus:" + ((int)error).ToString());
            }
            return new FusiHeadbandError((int)error, msg);
        }
    }
}
