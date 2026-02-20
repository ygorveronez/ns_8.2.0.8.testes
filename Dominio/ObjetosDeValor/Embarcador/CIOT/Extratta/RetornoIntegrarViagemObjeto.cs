using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class RetornoIntegrarViagemObjeto
    {
        [JsonProperty(PropertyName = "IdViagem", Required = Required.Default)]
        public int CodigoViagem { get; set; }

        //Montar os demais caso precisar

        [JsonProperty(PropertyName = "Pedagio", Required = Required.Default)]
        public RetornoIntegrarViagemPedagio Pedagio { get; set; }
    }
}
