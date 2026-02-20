namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envVehicle
    {
        public string Country { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleClassification { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleAxles { get; set; }
        public string Type { get; set; }
        public VehicleOwner VehicleOwner { get; set; }
    }

    public class VehicleOwner
    {
        public string Country { get; set; }
        public string NationalId { get; set; }
        public VehicleBrazilianSettings BrazilianSettings { get; set; }
        public string Type { get; set; }
        public VehiclePersonalInformation VehiclePersonalInformation { get; set; }
    }

    public class VehicleBrazilianSettings
    {
        public string RNTRC { get; set; }
        public VehiclePessoaJuridica VehiclePessoaJuridica { get; set; }
    }

    public class VehiclePessoaJuridica
    {
        public string NomeFantasia { get; set; }
    }

    public class VehiclePersonalInformation
    {
        public string Name { get; set; }
    }
}
