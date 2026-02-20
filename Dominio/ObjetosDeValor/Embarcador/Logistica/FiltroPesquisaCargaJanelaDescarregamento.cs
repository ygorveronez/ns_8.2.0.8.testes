using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaCargaJanelaDescarregamento
    {
        public int CodigoCentroDescarregamento { get; set; }

        public int CodigoSituacao { get; set; }

        public DateTime DataDescarregamento { get; set; }

        public DateTime DataDescarregamentoInicial { get; set; }

        public DateTime DataDescarregamentoFinal { get; set; }

        public string SenhaAgendamento { get; set; }

        public List<SituacaoCargaJanelaDescarregamento> Situacao { get; set; }

        public SituacaoAgendamentoPallet? SituacaoAgendamentoPallet { get; set; }

        public double CodigoFornecedor { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroPedido { get; set; }

        public DateTime DataLancamento { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }

        public bool UtilizarDadosDosPedidos { get; set; }

        public string NumeroNF { get; set; }

        public string NumeroCTe { get; set; }

        public string NumeroLacre { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTransportador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao ExcedenteDescarregamento { get; set; }
    }
}