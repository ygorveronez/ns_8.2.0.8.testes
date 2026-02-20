using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
    public class FiltroPesquisaSolicitacaoAvaria
    {
        public int CodigoUsuario { get; set; }
        public int Transportadora { get; set; }
        public int Filial { get; set; }
        public int Motivo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<int> TiposOperacao { get; set; }
    }
}
