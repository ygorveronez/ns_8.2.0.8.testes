using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.RH
{
    public class FiltroPesquisaDocumenotosComissaoFuncionario
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

        public List<int> Motorista { get; set; }

        public int codigoComissaoMotorista { get; set; }
    }
}
