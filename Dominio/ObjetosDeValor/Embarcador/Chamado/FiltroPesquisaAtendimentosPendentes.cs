using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public sealed class FiltroPesquisaAtendimentosPendentes
    {
        #region Propriedades

        public int NumeroInicial { get; set; }
        public int CodigoLote { get; set; }
        public int CodigoOrdenacao { get; set; }
        public SituacaoLoteChamadoOcorrencia? SituacaoLote { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime DataCriacaoInicial { get; set; }
        public DateTime DataCriacaoFinal { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoGrupoMotivoAtendimento { get; set; }
        public int CodigoMotivoChamado { get; set; }
        public int CodigoTransportador { get; set; }
        public double CodigoCliente { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoFilial { get; set; }
        public int NotaFiscal { get; set; }

        #endregion

    }
}
