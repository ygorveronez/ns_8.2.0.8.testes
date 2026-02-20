namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SpyTruck
{
    public class ResponseGetPosicoes
    {

        public int idVeiculo { get; set; }
        public string licensePlate { get; set; }
        public string dhTracking { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string channel { get; set; }
        public double heading { get; set; }
        public int altitude { get; set; }
        public string locationRef { get; set; }
        public int speed { get; set; }
        public bool gpsValid { get; set; }
        public double battery { get; set; }
        public double batteryBck { get; set; }
        public string networkOperator { get; set; }
        public int accelX { get; set; }
        public int accelY { get; set; }
        public int accelZ { get; set; }
        public int eventId { get; set; }
        public string eventName { get; set; }
        public bool ppc { get; set; }
        public bool inp0 { get; set; }
        public bool inp1 { get; set; }
        public bool in1 { get; set; }
        public bool in2 { get; set; }
        public bool in3 { get; set; }
        public bool in4 { get; set; }
        public bool in5 { get; set; }
        public bool in6 { get; set; }
        public bool in7 { get; set; }
        public bool in8 { get; set; }
        public bool in9 { get; set; }
        public bool in10 { get; set; }
        public bool in11 { get; set; }
        public bool in12 { get; set; }
        public bool in13 { get; set; }
        public bool in14 { get; set; }
        public bool driverId { get; set; }
        public string serialDriverId { get; set; }
        public bool mainBat { get; set; }
        public bool accel { get; set; }
        public bool jammerMdm { get; set; }
        public bool jammerGPS { get; set; }
        public bool trip { get; set; }
        public bool out1 { get; set; }
        public bool out2 { get; set; }
        public bool out3 { get; set; }
        public bool out4 { get; set; }
        public bool out5 { get; set; }
        public bool out6 { get; set; }
        public bool out7 { get; set; }
        public bool out8 { get; set; }
        public bool out9 { get; set; }
        public bool out10 { get; set; }
        public bool out11 { get; set; }
        public bool out12 { get; set; }
        public bool out13 { get; set; }
        public bool out14 { get; set; }
        public int temperature1 { get; set; }
        public string ratType { get; set; }
        public int analogIn { get; set; }
        public double analogInVolts { get; set; }
        public int analogIn2 { get; set; }
        public double analogIn2Volts { get; set; }
        public int rssiGSM { get; set; }
        public int snrSat { get; set; }
        public double weight { get; set; }
        public Telemetry telemetry { get; set; }
    }

    public class Telemetry
    {
        public double odometer { get; set; }
        public int rpm { get; set; }
        public int accumFuel { get; set; }
        public int accumHours { get; set; }
        public int fuelLevel1 { get; set; }
        public int fuelLevel2 { get; set; }
        public int oilLevel { get; set; }
        public int tiltAngle { get; set; }
        public double tripDistance { get; set; }
        public int tripFuel { get; set; }
        public int tripTime { get; set; }
        public string vin { get; set; }
        public bool isSpeed { get; set; }
        public bool isRPM { get; set; }
        public bool isCoolantTemperature { get; set; }
        public bool isTripTime { get; set; }
        public bool isHourmeter { get; set; }
        public bool isOdometer { get; set; }
        public bool isTripDistance { get; set; }
        public bool isDistanceSinceDTCsCleared { get; set; }
        public bool isFuelLevel1 { get; set; }
        public bool isFuelLevel2 { get; set; }
        public bool isOilLevel { get; set; }
        public bool isFuelType { get; set; }
        public bool isAlcoholPercent { get; set; }
        public bool isVin { get; set; }
        public bool isFuelEconomy { get; set; }
        public bool isFuelConsumption { get; set; }
        public bool isAmbientTemperature { get; set; }
        public bool isLowResTripFuel { get; set; }
        public bool isLowResAccumFuel { get; set; }
        public bool isHighResTripFuel { get; set; }
        public bool isHighResAccumFuel { get; set; }

    }
}
