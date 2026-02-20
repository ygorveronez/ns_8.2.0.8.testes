using System;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaRelatorioNotaEntradaOrdemCompra
    {
        
        public DateTime DataEntrada { get; set; }
        public int CodigoNota { get; set; }
        public int Nota { get; set; }
        public int CodigoOrdem { get; set; }
        public int CodigoProduto { get; set; }
        public double Fornecedor { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
