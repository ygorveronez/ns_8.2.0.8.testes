using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class GeoLocalizacaoClienteSubArea
    {
        public double ClienteCPFCNPJ { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string NomeFantasia { get; set; }
        public string AreaCliente { get; set; }
        public int RaioCliente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea TipoAreaCliente { get; set; }
        public string SubAreaCliente { set { if (value != null) SubAreas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SubArea>>(value); } }
        public List<SubArea> SubAreas { get; set; }
    }

    public class SubArea
    {
        public string Area { get; set; }
    }
}
