using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioRateioDespesaVeiculo
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<long> CodigosTipoDespesa { get; set; }
        public List<long> CodigosGrupoDespesa { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
        public List<int> CodigosSegmentoVeiculo { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public DateTime DataLancamentoInicial { get; set; }
        public DateTime DataLancamentoFinal { get; set; }
        public OrigemRateioDespesaVeiculo? OrigemRateio { get; set; }
    }
}
