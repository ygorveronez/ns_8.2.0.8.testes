using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaBonificacaoTransportador
    {
        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }
    }
}
