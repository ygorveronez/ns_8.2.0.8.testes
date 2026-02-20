using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Maxtrack
{
    [XmlRoot(ElementName = "integration")]
    public class Integration__
    {

        [XmlElement(ElementName = "id")]
        public long id { get; set; }

        [XmlElement(ElementName = "idVeic")]
        public long idVeic { get; set; }

        [XmlElement(ElementName = "vehicle")]
        public Vehicle vehicle { get; set; }

        [XmlElement(ElementName = "latitude")]
        public double latitude { get; set; }

        [XmlElement(ElementName = "longitude")]
        public double longitude { get; set; }

        [XmlElement(ElementName = "speed")]
        public int speed { get; set; }

        [XmlElement(ElementName = "altitude")]
        public double altitude { get; set; }

        [XmlElement(ElementName = "direction")]
        public int direction { get; set; }

        [XmlElement(ElementName = "ignition")]
        public bool ignition { get; set; }

        [XmlElement(ElementName = "validGPS")]
        public bool validGPS { get; set; }

        [XmlElement(ElementName = "satelital")]
        public bool satelital { get; set; }

        [XmlElement(ElementName = "bloqueio")]
        public bool bloqueio { get; set; }

        [XmlElement(ElementName = "moduleTime")]
        public DateTime moduleTime { get; set; }

        [XmlElement(ElementName = "serverTime")]
        public DateTime serverTime { get; set; }

        [XmlElement(ElementName = "hodometro")]
        public int hodometro { get; set; }

        [XmlElement(ElementName = "memory")]
        public bool memory { get; set; }

        [XmlElement(ElementName = "endereco")]
        public string endereco { get; set; }

        [XmlElement(ElementName = "codevt")]
        public int codevt { get; set; }

        [XmlElement(ElementName = "descevt")]
        public string descevt { get; set; }

    }






    public class Integration
    {
        public long deviceID { get; set; }
        public long packetID { get; set; }
        public IdentificationPack identificationPack { get; set; }
        public NewReportData newReportData { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class IdentificationPack
    {
        public string deviceIDStr { get; set; }
    }

    public class NewReportData
    {
        public long sequenceNumber { get; set; }
        public int reason { get; set; }
        public long dateTime { get; set; }
        public List<PositionInfo> positionInfo { get; set; }
        public Gps gps { get; set; }
        public Ios ios { get; set; }
        public Flags flags { get; set; }
        public Telemetry telemetry { get; set; }
        public List<Accessory> accessories { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class PositionInfo
    {
        public int positionSource { get; set; }
        public int positionFormat { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int altitude { get; set; }
        public int estimatedPositionError { get; set; }
        public int directionDegrees { get; set; }
        public int directionHeading { get; set; }
        public AddressInfo addressInfo { get; set; }
    }

    public class AddressInfo
    {
        public string street { get; set; }
        public string streetNumber { get; set; }
        public string postalCode { get; set; }
        public string county { get; set; }
        public string region { get; set; }
        public string country { get; set; }
    }

    public class Gps
    {
        public int internalAntenna { get; set; }
        public int fixState { get; set; }
        public int svn { get; set; }
        public int hdop { get; set; }
        public int vdop { get; set; }
        public int pdop { get; set; }
        public int age { get; set; }
        public int filter { get; set; }
        public int fixQuality { get; set; }
        public int averageSnr { get; set; }
        public int gpsPowerState { get; set; }
        public bool jamming { get; set; }
        public bool spoofing { get; set; }
    }

    public class Ios
    {
        public int ignitionState { get; set; }
        public InputInfo inputInfo { get; set; }
        public OutputInfo outputInfo { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class InputInfo
    {
        public int input1State { get; set; }
        public int input2State { get; set; }
        public int input2Peripheral { get; set; }
        public int input3State { get; set; }
        public int input3Peripheral { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class OutputInfo
    {
        public int output1State { get; set; }
        public int output1Peripheral { get; set; }
        public int output2State { get; set; }
        public int output2Peripheral { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Flags
    {
        public DeviceInfo deviceInfo { get; set; }
        public ConnectionInfo connectionInfo { get; set; }
        public OperationalInfo operationalInfo { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class DeviceInfo
    {
        public bool extPowerState { get; set; }
        public int extPowerValue { get; set; }
        public int battState { get; set; }
        public int battPercent { get; set; }
        public bool ignitionWakesUp { get; set; }

        
        // Adicione outras propriedades necessárias aqui
    }

    public class ConnectionInfo
    {
        public int connectionType { get; set; }
        public bool jamming { get; set; }
        public int csq { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class OperationalInfo
    {
        public bool panic { get; set; }
        public int alarm { get; set; }
        public int emergency { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Telemetry
    {
        public FlagsTelemetry flags { get; set; }
        public Odometer odometer { get; set; }
        public Speed speed { get; set; }
        public Ecu ecu { get; set; }
        public Slope slope { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class FlagsTelemetry
    {
        public bool overSpeed { get; set; }
        public bool stoppedExcess { get; set; }
        public bool moving { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Odometer
    {
        public int main { get; set; }
        public int gps { get; set; }
        public int pulse { get; set; }
        public int can { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Speed
    {
        public int main { get; set; }
        public int gps { get; set; }
        public int can { get; set; }
        public int displayLimit { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Ecu
    {
        public Diagnostic diagnostic { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Diagnostic
    {
        // Adicione outras propriedades necessárias aqui
    }

    public class Slope
    {
        public int main { get; set; }
        public int gps { get; set; }
        public int gsensor { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class Accessory
    {
        public long deviceID { get; set; }
        public long sequenceNumber { get; set; }
        public int reason { get; set; }
        public FlagsAccessory flags { get; set; }
        public int product { get; set; }
        public TdInfo tdInfo { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class FlagsAccessory
    {
        public DeviceInfoAccessory deviceInfo { get; set; }
        public ConnectionInfoAccessory connectionInfo { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class DeviceInfoAccessory
    {
        public int hdmi1 { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class TdInfo
    {
        public LightSensor lightSensor { get; set; }
        public bool driverUIApp { get; set; }
        public bool externalApp { get; set; }
        public CameraModeInfo camera01 { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class LightSensor
    {
        public int illuminance { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class CameraModeInfo
    {
        public int cameraState { get; set; }
        public int streamMode { get; set; }
        public int recordMode { get; set; }
        public int cameraVirtualID { get; set; }
        public CropInfo cropInfo { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class CropInfo
    {
        public int type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

    public class ConnectionInfoAccessory
    {
        public int accessoryState { get; set; }
        // Adicione outras propriedades necessárias aqui
    }

}
