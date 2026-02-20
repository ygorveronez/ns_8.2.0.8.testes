using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioAgendaCancelada
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public double Fornecedor { get; set; }
        public double Destinatario { get; set; }
        public int TipoDeCarga { get; set; }
        public int Filial { get; set; }
        public string NumeroCarga { get; set; }
        public string Pedido { get; set; }
        public string Senha { get; set; }
        public List<SituacaoAgendamentoColeta> SituacaoAgendamento { get; set; }
    }
}
