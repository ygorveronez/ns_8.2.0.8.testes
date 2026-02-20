using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class BaixaFinanceiro
    {
        /// <summary>
        /// D - diário
        /// M - mensal
        /// </summary>
        public string TipoLote { get; set; }
        public DateTime DataLote { get; set; }
        public decimal TotalLote { get; set; }
        public string Descricao { get; set; }
        public string Origem { get; set; }
        public string Identificador { get; set; }
        /// <summary>
        /// L - liberado
        /// N - não liberado
        /// </summary>
        public string SituacaoLote { get; set; }
        public string CPFCNPJEstabelecimento { get; set; }
        public int Sequencia { get; set; }

        public List<BaixaFinanceiroLancamento> Lancamentos { get; set; }
    }
}
