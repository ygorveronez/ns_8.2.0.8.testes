using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaPagamentoAprovacao
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoPagamento? SituacaoPagamento { get; set; }
    }
}
