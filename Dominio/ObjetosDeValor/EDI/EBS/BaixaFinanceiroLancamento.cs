using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class BaixaFinanceiroLancamento
    {
        public DateTime DataLancamento { get; set; }
        public int ContaDebito { get; set; }
        public int ContaCredito { get; set; }
        public int Historico { get; set; }
        public string Complemento { get; set; }
        public decimal ValorLancamento { get; set; }
        public int CentroCustos { get; set; }
        public string ClassificacaoADebito { get; set; }
        public string ClassificacaoACredito { get; set; }
        public int Sequencia { get; set; }
        public string Observacoes { get; set; }
        public int CentroCustos2 { get; set; }
        public string Identificador { get; set; }
        public string ComplementoSAGE { get; set; }

        public List<BaixaFinanceiroLancamentoHistorico> Historicos { get; set; }
    }
}
