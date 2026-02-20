using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class EPI : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.EPI>
    {
        #region Construtores

        public EPI(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public EPI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.EPI> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.EPI>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion

        #region Métodos Públicos

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.EPI>> BuscarTodosAtivos(int limite = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.EPI>();

            query = query.Where(o => o.Ativo);

            if (limite > 0)
                query = query.Take(limite);

            return await query.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.EPI> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion
    }
}
