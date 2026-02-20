using System.Collections.Generic;
using AdminMultisoftware.Dominio.Enumeradores;
using AdminMultisoftware.Repositorio;
using Dominio.Entidades.Embarcador.Pedidos;

namespace Dominio.ObjetosDeValor.Embarcador.Regras
{
    public class FiltroRegraTomador
    {
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public bool PossuiRegra { get; set; }
        public List<Dominio.Entidades.Embarcador.Filiais.Filial> Filiais { get; set; }
        public List<RegraTomador> RegrasTomadores { get; set; }
        public List<RegraTomador> RegrasTomadoresSemTomador { get; set; }
    }
}
