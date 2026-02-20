using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Rota
    {
        [JsonProperty("idDocumentoTransporte")]
        public long DocumentoTransporte { get; set; }

        [JsonProperty("cnpj")]
        public string CNPJ { get; set; }

        [JsonProperty("cnpjTransportadora")]
        public string CNPJTransportadora { get; set; }

        [JsonProperty("cnpjCpfSubcontratado")]
        public string CNPJCPFSubcontratado { get; set; }

        [JsonProperty("fimVigencia")]
        public string FimVigencia { get; set; }

        [JsonProperty("inicioVigencia")]
        public string InicioVigencia { get; set; }

        [JsonProperty("numeroCartao")]
        public string NumeroCartao { get; set; }

        [JsonProperty("neixos")]
        public int NumeroEixos { get; set; }

        [JsonProperty("placa")]
        public string Placa { get; set; }

        [JsonProperty("positions")]
        public Posicoes Posicoes { get; set; }
    }
}
