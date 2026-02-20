using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioContratoFinanceiro
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Numero { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroDocumentoEntrada { get; set; }
        public double CpfCnpjFornecedor { get; set; }
        public int CodigoEmpresa { get; set; }
        public List<int> CodigosVeiculos { get; set; }
        public List<SituacaoContratoFinanciamento> Situacoes { get; set; }
    }
}
