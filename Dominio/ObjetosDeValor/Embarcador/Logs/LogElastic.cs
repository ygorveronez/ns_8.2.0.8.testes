using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logs
{
    public class LogElastic
    {
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServico { get; set; }
        public string Cliente { get; set; }
        public string DataAtual { get; set; }
        public string DescricaoAlerta { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic CodigoTipoAlerta { get; set; }
        public int ValorAlerta { get; set; }
        public string CaminhoLocal { get; set; }
    }
}
