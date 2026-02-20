using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Flags
    {
        [JsonProperty(PropertyName = "exige_nota", Required = Required.Default)]
        public string ExigeNota { get; set; }

        [JsonProperty(PropertyName = "reutilizar_nota", Required = Required.Default)]
        public string ReutilizarNota { get; set; }

        [JsonProperty(PropertyName = "tr_disputar_cota", Required = Required.Default)]
        public string TRDisputarCota { get; set; }

        [JsonProperty(PropertyName = "cota_flexivel", Required = Required.Default)]
        public string CotaFlexivel { get; set; }

        [JsonProperty(PropertyName = "fracao_cota_flexivel", Required = Required.Default)]
        public string FracaoCotaFlexivel { get; set; }

        [JsonProperty(PropertyName = "tipo_demanda", Required = Required.Default)]
        public string TipoDemanda { get; set; }

        [JsonProperty(PropertyName = "exige_aceite_transp", Required = Required.Default)]
        public string ExigeAceiteTransp { get; set; }

        [JsonProperty(PropertyName = "exige_aceite_cliente", Required = Required.Default)]
        public string ExigeAceiteCliente { get; set; }

        [JsonProperty(PropertyName = "terminal_resp_transp", Required = Required.Default)]
        public string TerminalRespTransp { get; set; }
    }
}
