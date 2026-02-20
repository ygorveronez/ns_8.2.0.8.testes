using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioCTeTituloReceber
    {
        public int NumeroCTe { get; set; }
        public int CodigoEmpresa { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool SomenteTitulosLiberados { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public double CnpjCpfRemetente { get; set; }
        public int NumeroFatura { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        public List<double> CodigosRecebedores { get; set; }
    }
}
