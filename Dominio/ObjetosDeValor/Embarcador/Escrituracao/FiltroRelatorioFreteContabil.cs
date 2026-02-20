using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroRelatorioFreteContabil
    {
        public int Filial { get; set; }

        public int Transportador { get; set; }

        public DateTime DataEmissaoInicial { get; set; }

        public DateTime DataEmissaoFinal { get; set; }

        public double Tomador { get; set; }

        public List<int> CentroResultado { get; set; }

        public DateTime DataLancamentoInicial { get; set; }

        public DateTime DataLancamentoFinal { get; set; }

        public List<int> ContaContabil { get; set; }

        public int CodigoCarga { get; set; }
    }
}
