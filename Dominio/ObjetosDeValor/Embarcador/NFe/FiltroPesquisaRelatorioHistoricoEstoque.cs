using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioHistoricoEstoque
    {
        public int CodigoEmpresa { get; set; }
        public int CodigoProduto { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Status { get; set; }
        public CategoriaProduto Categoria { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
    }
}
