using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CategoriaResponsavel : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel>
    {
        #region Construtores

        public CategoriaResponsavel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));

            return consulta;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }        

        public List<Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }

        #endregion
    }

}
