using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class OrdemEmbarqueCabecalho
    {
        [JsonProperty(PropertyName = "dataProgramacao", Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss", true })]
        public DateTime DataProgramacao { get; set; }

        [JsonProperty(PropertyName = "empresaCargaCNPJ", Required = Required.Always)]
        public string EmpresaCnpj { get; set; }

        [JsonProperty(PropertyName = "recebedorCNPJ", Required = Required.Default)]
        public string RecebedorCNPJ { get; set; }

        [JsonProperty(PropertyName = "gerarVale", Required = Required.Always)]
        public bool GerarValePedagio { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSCarga", Required = Required.Always)]
        public int ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "retornoViagem", Required = Required.Always)]
        public bool RetornoViagem { get; set; }

        [JsonProperty(PropertyName = "usuarioAD", Required = Required.AllowNull)]
        public string UsuarioAD { get; set; }
    }
}
