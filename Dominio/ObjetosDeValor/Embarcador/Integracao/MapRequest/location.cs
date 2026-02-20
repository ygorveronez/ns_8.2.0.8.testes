namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest
{
    public class location
    {
        public string street { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string postalCode { get; set; }

        public string adminArea1Type { get; set; }
        public string adminArea1 { get; set; }
        
        public string adminArea3Type { get; set; }
        public string adminArea3 { get; set; }

        public string adminArea4 { get; set; }
        public string adminArea4Type { get; set; }
        
        public string adminArea5Type { get; set; }
        public string adminArea5 { get; set; }

        public string adminArea6Type { get; set; }
        public string adminArea6 { get; set; }

        public string geocodeQualityCode { get; set; }
        public string geocodeQuality { get; set; }

        public string dragPoint { get; set; }

        public string sideOfStreet { get; set; }

        public string linkId { get; set; }

        public string unknownInput { get; set; }

        public latLng latLng { get; set; }
    }
}
