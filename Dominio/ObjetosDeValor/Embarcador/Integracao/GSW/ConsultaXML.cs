using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GSW
{
    public class ConsultaXML
    {
        [JsonProperty(PropertyName = "tipoXml", Required = Required.Always)]
        public int TipoXML { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long CodigoInicial { get; set; }
    }
}
