using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaEstornoProvisaoAprovacao
    {
        public int NumeroLote { get; set; }

        public int CodigoTransportador { get; set; }

        public string NumeroProvisao { get; set; }

        public int CodigoCarga { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime DataGeracaoLoteInicial { get; set; }

        public DateTime DataGeracaoLoteFinal { get; set; }

        public Enumeradores.SituacaoEstornoProvisaoSolicitacao? SituacaoEstornoProvisaoSolicitacao { get; set; }

    }
}