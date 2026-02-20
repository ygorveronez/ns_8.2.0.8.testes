using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador
{
    public class Posicao
    {
        public int VehicleCode { get; set; }
        public string VehicleName { get; set; }
        public int AccountNumber { get; set; }
        public int VehicleAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PositionTime { get; set; }
        public DateTime PositionTimeDT { get { return DateTime.Parse(PositionTime); } }
        public int VehicleIgnition { get; set; }
        public int Houmeter { get; set; }
        public decimal Hodometer { get; set; }
        public string LicensePlate { get; set; }
        public string County { get; set; }
        public string Uf { get; set; }
        public string CountryDescription { get; set; }
                

        public string Descricao
        {
            get
            {
                if (string.IsNullOrEmpty(County) && string.IsNullOrEmpty(Uf) && string.IsNullOrEmpty(CountryDescription))
                    return string.Empty;

                string retorno = "";

                if (!string.IsNullOrEmpty(County))
                    retorno = County;

                if (!string.IsNullOrEmpty(Uf))
                    retorno = retorno + " - " + Uf;

                if (!string.IsNullOrEmpty(CountryDescription))
                    retorno = retorno + " - " + CountryDescription;

                return retorno;
            }
        }
    }
}
