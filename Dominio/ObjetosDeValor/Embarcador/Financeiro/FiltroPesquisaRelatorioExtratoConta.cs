using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioExtratoConta
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataBaseInicial { get; set; }
        public DateTime DataBaseFinal { get; set; }
        public List<int> CodigosPlanoContaAnalitica { get; set; }
        public int CodigoMovimento { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoColaborador { get; set; }
        public List<int> CodigosGrupoPessoa { get; set; }
        public List<double> CnpjPessoa { get; set; }
        public string NumeroDocumento { get; set; }
        public DebitoCredito TipoDebitoCredito { get; set; }
        public string CentroResultado { get; set; }
        public bool CentroResultadoPai { get; set; }
        public List<string> PlanosContaAnalitica { get; set; }
        public int CodigoEmpresa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public int CodigoPlanoContaSintetica { get; set; }
        public int CodigoPlanoContaContrapartida { get; set; }
        public string PlanoContaSintetica { get; set; }
        public MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }
    }
}
