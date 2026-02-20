using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class HistoricoEstoque
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoStatus { get; set; }
        private CategoriaProduto Categoria { get; set; }
        private DateTime Data { get; set; }
        public string TipoMovimento { get; set; }
        public decimal Quantidade { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public decimal QuantidadeEstoque { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal Custo { get; set; }
        public string Empresa { get; set; }
        public string GrupoProduto { get; set; }
        public decimal QuantidadeAcumulada { get; set; }
        public string LocalArmazenamento { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoCategoria
        {
            get { return Categoria.ObterDescricao(); }
        }

        public string QuantidadeFormatado
        {

            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
                return Quantidade.ToString("n4");
            }
        }

        public string QuantidadeEstoqueFormatado
        {
            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
                return QuantidadeEstoque.ToString("n4");
            }
        }

        public string QuantidadeAcumuladaFormatada
        {
            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
                return QuantidadeAcumulada.ToString("n4");
            }
        }

        #endregion
    }
}
