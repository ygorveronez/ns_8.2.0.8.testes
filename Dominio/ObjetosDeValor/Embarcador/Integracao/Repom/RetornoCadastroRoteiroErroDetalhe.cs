using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCadastroRoteiroErroDetalhe
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.AllowNull)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.AllowNull)]
        public string Descricao { get; set; }
    }
}
