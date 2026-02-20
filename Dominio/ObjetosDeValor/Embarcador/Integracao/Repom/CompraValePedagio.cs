using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CompraValePedagio
    {
        [JsonProperty(PropertyName = "processoCodigoCliente", Required = Required.AllowNull)]
        public string ProcessoCodigoCliente { get; set; }

        [JsonProperty(PropertyName = "placa", Required = Required.Always)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "numeroEixos", Required = Required.Always)]
        public int NumeroEixos { get; set; }

        [JsonProperty(PropertyName = "carretaPlaca", Required = Required.AllowNull)]
        public string CarretaPlaca { get; set; }

        [JsonProperty(PropertyName = "carretaNumeroEixos", Required = Required.AllowNull)]
        public int CarretaNumeroEixos { get; set; }

        [JsonProperty(PropertyName = "numeroCartao", Required = Required.AllowNull)]
        public string NumeroCartao { get; set; }

        [JsonProperty(PropertyName = "nomeTransportador", Required = Required.AllowNull)]
        public string NomeTransportador { get; set; }

        [JsonProperty(PropertyName = "documentoTransportador", Required = Required.AllowNull)]
        public string DocumentoTransportador { get; set; }

        [JsonProperty(PropertyName = "nomeMotorista", Required = Required.AllowNull)]
        public string NomeMotorista { get; set; }

        [JsonProperty(PropertyName = "documentoMotorista", Required = Required.AllowNull)]
        public string DocumentoMotorista { get; set; }

        [JsonProperty(PropertyName = "cnpjFilial", Required = Required.AllowNull)]
        public string CnpjFilial { get; set; }

        [JsonProperty(PropertyName = "usuarioEmissao", Required = Required.AllowNull)]
        public string UsuarioEmissao { get; set; }

        [JsonProperty(PropertyName = "roteiro", Required = Required.Always)]
        public CompraValePedagioRoteiro Roteiro { get; set; }

        [JsonProperty(PropertyName = "documento", Required = Required.AllowNull)]
        public List<CompraValePedagioDocumento> Documento { get; set; }

        [JsonProperty(PropertyName = "pracas", Required = Required.AllowNull)]
        public List<int> Pracas { get; set; }

        [JsonProperty(PropertyName = "configuracao", Required = Required.AllowNull)]
        public CompraValePedagioConfiguracao Configuracao { get; set; }

        [JsonProperty(PropertyName = "utilizaCarreta", Required = Required.AllowNull)]
        public string UtilizaCarreta { get; set; }
    }
}
