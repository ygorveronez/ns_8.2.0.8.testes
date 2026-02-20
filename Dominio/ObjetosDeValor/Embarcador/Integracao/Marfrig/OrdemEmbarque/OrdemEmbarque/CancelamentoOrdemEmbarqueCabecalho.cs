using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class CancelamentoOrdemEmbarqueCabecalho
    {
        [JsonProperty(PropertyName = "empresaCargaCNPJ", Required = Required.Always)]
        public string EmpresaCnpj { get; set; }

        [JsonProperty(PropertyName = "motivo", Required = Required.Always)]
        public string Motivo { get; set; }

        [JsonProperty(PropertyName = "numeroOrdemEmbarque", Required = Required.Always)]
        public int NumeroOrdemEmbarque { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSCarga", Required = Required.Always)]
        public int ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "usuarioAD", Required = Required.AllowNull)]
        public string UsuarioAD { get; set; }
    }
}
