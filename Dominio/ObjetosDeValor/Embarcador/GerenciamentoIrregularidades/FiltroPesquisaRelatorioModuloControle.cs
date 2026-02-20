using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public sealed class FiltroPesquisaRelatorioModuloControle
    {
        public DateTime? DataInicialEmissao { get; set; }
        public DateTime? DataFinalEmissao { get; set; }
        public DateTime? DataInicialIrregularidade { get; set; }
        public DateTime? DataFinalIrregularidade { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public SituacaoControleDocumento? Situacao { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoSetor { get; set; }
        public int CodigoIrregularidade { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoPortifolio { get; set; }

    }
}
