using System;
using System.Collections.Generic;

namespace Servicos.DTO
{
    public class ParametrosFechamentoAbastecimento
    {
        public int CodigoFechamento { get; set; }
        public double CodigoPosto { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEquipamento { get; set; }
        public DateTime DataInicio { get; set; }
        public  DateTime DataFim { get; set; }
        public List<int> CodigosEmpresa { get; set; }
    }
}
