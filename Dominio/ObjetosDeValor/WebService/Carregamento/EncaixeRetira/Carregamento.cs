using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira
{
    [DataContract]
    public sealed class Carregamento
    {
        [DataMember]
        public List<Pedido> Pedidos { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Empresa Transportador { get; set; }

        [DataMember]
        public string DataHoraEncaixe { get; set; }

        public DateTime? DataHoraEnxcaixeConvertido => DataHoraEncaixe.ToNullableDateTime();

        [DataMember]
        public Embarcador.Carga.Motorista Motorista { get; set; }

        [DataMember]
        public Embarcador.Frota.Veiculo Veiculo { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }

        [DataMember]
        public Embarcador.Carga.ModeloVeicular ModeloVeicular { get; set; }

        [DataMember]
        public Embarcador.Filial.Filial Filial { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoCargaEmbarcador TipoCarga { get; set; }
    }
}
