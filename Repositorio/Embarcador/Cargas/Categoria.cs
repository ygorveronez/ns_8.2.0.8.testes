using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class Categoria : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Categoria>
    {
        public Categoria(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Categoria(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Cargas.Categoria>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterListaAsync(result, parametroConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.CountAsync(CancellationToken);
        }

        public Task<bool> ExisteDuplicadoAsync(Dominio.Entidades.Embarcador.Cargas.Categoria categoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Categoria>();

            query = query.Where(obj => (obj.Codigo != categoria.Codigo) && obj.Situacao);

            if (!string.IsNullOrWhiteSpace(categoria.CodigoIntegracao))
                query = query.Where(obj => obj.Descricao == categoria.Descricao || obj.CodigoIntegracao == categoria.CodigoIntegracao);
            else
                query = query.Where(obj => obj.Descricao == categoria.Descricao);

            return query.AnyAsync(CancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Categoria> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Categoria>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Situacao);

            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Situacao);

            return result;
        }

        #endregion
    }
}
