using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaOrdemCompra
    {
        public int CodigoEmpresa { get; set; }
        public int Numero { get; set; }
        public int Operador { get; set; }
        public int Produto { get; set; }
        public double Fornecedor { get; set; }
        public double Transportador { get; set; }
        public DateTime DataGeracaoInicio { get; set; }
        public DateTime DataGeracaoFim { get; set; }
        public DateTime DataRetornoInicio { get; set; }
        public DateTime DataRetornoFim { get; set; }
        public SituacaoOrdemCompra? Situacao { get; set; }
        public int Veiculo { get; set; }
        public int NumeroCotacao { get; set; }
        public int NumeroRequisicao { get; set; }
    }
}
