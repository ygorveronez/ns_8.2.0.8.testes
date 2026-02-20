using System.Collections.Generic;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioConciliacaoBancaria
    {
        public List<int> CodigoPlano { get; set; }
        public DateTime DataInicial { get; set; } 
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito DebitoCredito { get; set; }
        public string Observacao { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public string NumeroDocumento { get; set; }
        public int CodigoMovimento { get; set; }
    }
}
