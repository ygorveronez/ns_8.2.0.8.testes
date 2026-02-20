using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Produtos
{
    public class Produto
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string DescricaoNotaFiscal { get; set; }
        public UnidadeDeMedida UnidadeDeMedida { get; set; }
        public string CodigoProduto { get; set; }
        public string CodigoNCM { get; set; }
        public string CodigoCEST { get; set; }
        public string CodigoBarrasEAN { get; set; }
        public string CodigoBarras { get; set; }
        public string CodigoAnvisa { get; set; }
        public string CodigoANP { get; set; }
        public string Status { get; set; }
        public CategoriaProduto Categoria { get; set; }
        public OrigemMercadoria OrigemMercadoria { get; set; }
        public GeneroProduto GeneroProduto { get; set; }
        public decimal UltimoCusto { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal MargemLucro { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal ValorMinimoVenda { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public string GrupoProduto { get; set; }
        public string Marca { get; set; }
        public string LocalArmazenamento { get; set; }
        public string GrupoImposto { get; set; }

        public string UnidadeMedidaFormatada
        {
            get { return UnidadeDeMedida.ObterSigla(); }
        }

        public string CategoriaFormatada
        {
            get { return Categoria.ObterDescricao(); }
        }

        public string OrigemMercadoriaFormatada
        {
            get { return OrigemMercadoria.ObterDescricao(); }
        }

        public string GeneroProdutoFormatado
        {
            get { return GeneroProduto.ObterDescricao(); }
        }
    }
}
