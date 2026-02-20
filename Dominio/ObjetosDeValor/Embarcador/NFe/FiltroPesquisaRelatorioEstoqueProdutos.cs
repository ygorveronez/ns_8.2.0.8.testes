using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioEstoqueProdutos
    {
        public int CodigoProduto { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public int CodigoMarca { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodProduto { get; set; }
        public string CodigoNCM { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public CategoriaProduto Categoria { get; set; }
        public DateTime DataPosicaoEstoque { get; set; }
        public bool EstoqueReservado { get; set; }
    }
}
