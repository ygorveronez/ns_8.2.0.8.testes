using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.WMS
{
    public class MontagemContainer : RepositorioBase<Dominio.Entidades.Embarcador.WMS.MontagemContainer>
    {
        public MontagemContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MontagemContainer(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.WMS.MontagemContainer> Consultar(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = _Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer filtrosPesquisa)
        {
            var query = _Consultar(filtrosPesquisa);

            return query.Count();
        }

        public int BuscarProximoIdSequencial()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainer>()
                .OrderByDescending(obj => obj.Id)
                .Select(obj => obj.Id)
                .FirstOrDefault() + 1;
        }

        public bool BuscarSeExisteCadastrado()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainer>();

            return query.Any();
        }

        public Task<bool> BuscarSeExisteCadastradoAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainer>();

            return query.AnyAsync(CancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.WMS.MontagemContainer> _Consultar(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainer>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                query = query.Where(obj => obj.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (filtrosPesquisa.TipoContainer > 0)
                query = query.Where(obj => obj.TipoContainer.Codigo == filtrosPesquisa.TipoContainer);

            if (filtrosPesquisa.Container > 0)
                query = query.Where(obj => obj.Container.Codigo == filtrosPesquisa.Container);

            if (filtrosPesquisa.IdMontagemContainer > 0)
                query = query.Where(obj => obj.Id == filtrosPesquisa.IdMontagemContainer);

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusMontagemContainer.Todos)
                query = query.Where(obj => obj.Status == filtrosPesquisa.Status);

            return query;
        }

        #endregion
    }
}
