using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class JustificativaAutorizacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga>
    {
        public JustificativaAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao.Value);

            return query;
        }

        #endregion
    }
}
