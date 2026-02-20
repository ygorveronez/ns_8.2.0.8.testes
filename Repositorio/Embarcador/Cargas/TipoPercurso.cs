using Dominio.ObjetosDeValor.Embarcador.Carga;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoPercurso : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoPercurso>
    {
        #region Constructor
        public TipoPercurso(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoPercurso> Consultar(FiltroPesquisaTipoPercurso filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoPercurso>();

            if (!string.IsNullOrEmpty(filtroPesquisa.CodigoIntegracao))
                query = query.Where(t => t.CodigoIntegracao == filtroPesquisa.CodigoIntegracao);

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                query = query.Where(t => t.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.Vazio.HasValue && filtroPesquisa.Vazio.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Vazio.Todos)
                    query = query.Where(t => t.Vazio == filtroPesquisa.Vazio.Value);

            return query;
        }

        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Cargas.TipoPercurso> Consultar(FiltroPesquisaTipoPercurso filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtroPesquisa);
            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(FiltroPesquisaTipoPercurso filtroPesquisa)
        {
            var consulta = Consultar(filtroPesquisa);
            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoPercurso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoPercurso>();

            if (codigo > 0)
                query = query.Where(t => t.Codigo == codigo);

            return query.FirstOrDefault();

        }

        #endregion


    }
}
