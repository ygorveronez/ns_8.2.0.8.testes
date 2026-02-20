using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaRelatorioOrdemCompra
    {
        public DateTime DataGeracaoInicio { get; set; }
        public DateTime DataGeracaoFim { get; set; }
        public DateTime DataPrevisaoInicio { get; set; }
        public DateTime DataPrevisaoFim { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int Produto { get; set; }
        public int Operador { get; set; }
        public double Fornecedor { get; set; }
        public double Transportador { get; set; }
        public SituacaoOrdemCompra Situacao { get; set; }
        public int CodigoEmpresa { get; set; }
        public int Veiculo { get; set; }
    }
}
