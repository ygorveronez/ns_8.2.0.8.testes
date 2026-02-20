using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroDistribuicao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>
    {
        public CentroDistribuicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool VerificarExiste()
        {
            var consultaCentroDistribuicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>()
                .Where(o => o.Situacao);

            return consultaCentroDistribuicao.Count() > 0;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            return query;
        }
        #endregion
    }
}