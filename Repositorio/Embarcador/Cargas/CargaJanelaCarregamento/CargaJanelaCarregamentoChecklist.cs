using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class CargaJanelaCarregamentoChecklist : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist>
    {
        #region Construtores

        public CargaJanelaCarregamentoChecklist(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist BuscarPorCargaJanelaCarregamento(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist>();
            query = from obj in query where obj.CargaJanelaCarregamento.Codigo == codigo select obj;

            return query
                .Fetch(o => o.UltimaCarga)
                .ThenFetch(o => o.Anexos)
                .Fetch(o => o.PenultimaCarga)
                .ThenFetch(o => o.Anexos)
                .Fetch(o => o.AntepenultimaCarga)
                .ThenFetch(o => o.Anexos)
                .FirstOrDefault();
        }

        #endregion
    }

    public sealed class CargaJanelaCarregamentoChecklistCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga>
    {
        #region Construtores

        public CargaJanelaCarregamentoChecklistCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga> BuscarPorCargaJanelaCarregamentoChecklist(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga>();
            query = from obj in query where obj.CargaJanelaCarregamentoChecklist.Codigo == codigo select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga> BuscarPorCargaJanelaCarregamento(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga>();
            query = from obj in query where obj.CargaJanelaCarregamentoChecklist.CargaJanelaCarregamento.Codigo == codigo select obj;

            return query.ToList();
        }

        #endregion
    }

    public sealed class CargaJanelaCarregamentoChecklistCargaAnexos : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos>
    {
        #region Construtores

        public CargaJanelaCarregamentoChecklistCargaAnexos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos> BuscarPorCargaJanelaCarregamentoChecklistCarga(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos>();
            query = from obj in query where obj.CargaJanelaCarregamentoChecklistCarga.Codigo == codigo select obj;

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos> BuscarPorCargaJanelaCarregamentoChecklist(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos>();
            query = from obj in query where obj.CargaJanelaCarregamentoChecklistCarga.CargaJanelaCarregamentoChecklist.Codigo == codigo select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos> BuscarPorCargaJanelaCarregamento(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos>();
            query = from obj in query where obj.CargaJanelaCarregamentoChecklistCarga.CargaJanelaCarregamentoChecklist.CargaJanelaCarregamento.Codigo == codigo select obj;

            return query.ToList();
        }

        #endregion
    }
}
