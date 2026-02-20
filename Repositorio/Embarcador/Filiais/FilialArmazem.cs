using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais
{
    public sealed class FilialArmazem : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>
    {
        #region Construtores

        public FilialArmazem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public FilialArmazem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> BuscarPorFilial(int codigoFilial)
        {
            var consultaFilialArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>()
                .Where(o => o.Filial.Codigo == codigoFilial);

            return consultaFilialArmazem.ToList();
        }

        public List<int> BuscarFiliaisPorCNPJFiliais(List<string> CNPJFiliais)
        {
            var consultaFilialArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>()
                .Where(o => CNPJFiliais.Contains(o.Filial.CNPJ)).Select(o => o.Filial.Codigo);

            return consultaFilialArmazem.ToList();
        }

        public Task<List<int>> BuscarFiliaisPorCNPJFiliaisAsync(List<string> CNPJFiliais)
        {
            var consultaFilialArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>()
               .Where(o => CNPJFiliais.Contains(o.Filial.CNPJ));

            return consultaFilialArmazem.Select(o => o.Filial.Codigo).ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Filiais.FilialArmazem BuscarPorCodigoIntegracao(string codigoFilial)
        {
            var consultaFilialArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>()
                .Where(o => o.CodigoIntegracao == codigoFilial);

            return consultaFilialArmazem.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.CodigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigosFiliais?.Count > 0)
                result = result.Where(o => filtrosPesquisa.CodigosFiliais.Contains(o.Filial.Codigo));

            return result;
        }

        #endregion
    }
}
