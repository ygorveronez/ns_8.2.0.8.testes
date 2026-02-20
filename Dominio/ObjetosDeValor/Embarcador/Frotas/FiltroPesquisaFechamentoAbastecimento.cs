using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaFechamentoAbastecimento
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public SituacaoFechamentoAbastecimento Situacao { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoEquipamento { get; set; }
    }
}
