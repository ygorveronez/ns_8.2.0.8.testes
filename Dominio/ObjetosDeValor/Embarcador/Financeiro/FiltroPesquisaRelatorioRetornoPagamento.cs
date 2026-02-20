using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioRetornoPagamento
    {
        public int CodigoTitulo { get; set; }
        public double CnpjFornecedor { get; set; }
        public int Fatura { get; set; }
        public int CodigoEmpresa { get; set; }
        public List<int> GruposPessoas { get; set; }
        public DateTime DataInicialImportacao { get; set; }
        public DateTime DataFinalImportacao { get; set; }
        public DateTime DataInicialPagamento { get; set; }
        public DateTime DataFinalPagamento { get; set; }
        public string Comando { get; set; }
        public List<int> CodigosBanco { get; set; }
        public List<int> CodigosBancoPessoa { get; set; }
        public string CodigosRetornos { get; set; }
        public bool Paginar { get; set; }
        public int CodigoConfiguracaoBoleto { get; set; }
    }
}
