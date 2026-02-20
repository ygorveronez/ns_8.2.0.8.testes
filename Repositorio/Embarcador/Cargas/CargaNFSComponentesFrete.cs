using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaNFSComponentesFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete>
    {
        public CargaNFSComponentesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete> BuscarPorCargaNFS(int codigoCargaNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete>();
            var result = from obj in query where obj.CargaNFS.Codigo == codigoCargaNFS select obj;
            return result.ToList();
        }


        public decimal BuscarTotalCargaPorCompomente(int cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete>();
            var result = from obj in query where obj.CargaNFS.Codigo == cargaCTe && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete> BuscarPorCargaNFSQueGeraMovimentacao(int codigoCargaNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete>();
            var result = from obj in query where obj.CargaNFS.Codigo == codigoCargaNFS && obj.ComponenteFrete.GerarMovimentoAutomatico select obj;
            return result.ToList();
        }
    }
}
