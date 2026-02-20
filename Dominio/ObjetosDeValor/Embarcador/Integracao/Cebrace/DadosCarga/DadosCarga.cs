using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class DadosCarga
    {
        [JsonProperty(PropertyName = "protocolointegracaoCarga", Required = Required.Default)]
        public int ProtocoloIntegracaoCarga { get; set; }

        [JsonProperty(PropertyName = "numeroCarga", Required = Required.Default)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "placaTracao", Required = Required.Default)]
        public string PlacaTracao { get; set; }

        [JsonProperty(PropertyName = "dataAgendamento", Required = Required.Default)]
        public string DataAgendamento { get; set; }

        [JsonProperty(PropertyName = "horaAgendamento", Required = Required.Default)]
        public string HoraAgendamento { get; set; }

        [JsonProperty(PropertyName = "filial", Required = Required.Default)]
        public Filial Filial { get; set; }

        [JsonProperty(PropertyName = "transportador", Required = Required.Default)]
        public Transportador Transportador { get; set; }

        [JsonProperty(PropertyName = "ModeloVeicular", Required = Required.Default)]
        public ModeloVeicular ModeloVeicular { get; set; }

        [JsonProperty(PropertyName = "motorista", Required = Required.Default)]
        public Motorista Motorista { get; set; }

        [JsonProperty(PropertyName = "pedidos", Required = Required.Default)]
        public List<Pedido> Pedidos { get; set; }

        [JsonProperty(PropertyName = "TipoOperacao", Required = Required.Default)]
        public TipoOperacao TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "placaCarreta", Required = Required.Default)]
        public string PlacaCarreta { get; set; }

    }
}