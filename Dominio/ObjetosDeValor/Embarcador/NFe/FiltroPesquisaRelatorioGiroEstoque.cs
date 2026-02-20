using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioGiroEstoque
    {
        public int CodigoProduto { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
    }
}
