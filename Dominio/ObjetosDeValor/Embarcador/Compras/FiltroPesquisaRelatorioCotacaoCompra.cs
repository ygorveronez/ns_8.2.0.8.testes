using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaRelatorioCotacaoCompra
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CodigosProduto { get; set; }
        public List<double> CodigosFornecedor { get; set; }
    }
}
