using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaRelatorioDespesaVeiculo
    {
        public int Empresa { get; set; }
        public int Veiculo { get; set; }
        public int Produto { get; set; }
        public int GrupoProduto { get; set; }
        public int GrupoProdutoPai { get; set; }
        public double Fornecedor { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> NaturezaOperacao { get; set; }
    }
}
