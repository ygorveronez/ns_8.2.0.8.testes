using System;

namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
    public sealed class FiltroPesquisaProdutoAvaria
    {
        public int CodigoProduto { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
    }
}
