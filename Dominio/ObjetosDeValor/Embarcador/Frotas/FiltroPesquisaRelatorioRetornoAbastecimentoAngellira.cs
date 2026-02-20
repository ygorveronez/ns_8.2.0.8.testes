using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaRelatorioRetornoAbastecimentoAngellira
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double Fornecedor { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
    }
}
