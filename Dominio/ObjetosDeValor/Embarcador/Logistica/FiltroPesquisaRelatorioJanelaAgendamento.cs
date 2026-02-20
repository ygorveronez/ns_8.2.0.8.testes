using System;
using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioJanelaAgendamento
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double Fornecedor { get; set; }
        public OpcaoSimNaoPesquisa JanelaExcedente { get; set; }
        public List<SituacaoAgendamentoColeta> SituacaoAgendamento { get; set; }
        public int TipoDeCarga { get; set; }
        public string RaizCnpjFornecedor { get; set; }
        public List<int> CodigosFilial { get; set; }
        public string NumeroPedido { get; set; }
        public string Senha { get; set; }
        public string NumeroCarga { get; set; }
        public int CentroDescarregamento { get; set; }
    }
}
