using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaGestaoCarga
    {
        public List<int> CodigosCentroResultado { get; set; }
        public List<int> CodigosGrupoPessoa { get; set; }
        public List<double> CNPJsTomador { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusGestaoCarga StatusGestaoCarga { get; set; }
        public bool IncluirOperacoesDeslocamentoVazio { get; set; }
        public int NumeroNF { get; set; }
        public int NumeroCTe { get; set; }
    }
}
