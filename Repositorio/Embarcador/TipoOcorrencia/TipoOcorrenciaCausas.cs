using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.TipoOcorrencia
{
    public class TipoOcorrenciaCausas : RepositorioBase<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas>
    {
        #region Construtores

        public TipoOcorrenciaCausas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> BuscarPorTipoOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas>();
            var result = from obj in query where obj.TipoOcorrencia.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas>();

            if (filtrosPesquisa.BuscarTodasCausasDesconsiderandoTipoOcorrencia)
            {
                query = query.Where(obj =>
                                    obj.Ativo && (string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao) || obj.Descricao == filtrosPesquisa.Descricao));
            }
            else
            {
                query = query.Where(obj =>
                                    obj.Ativo && (string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao) || obj.Descricao == filtrosPesquisa.Descricao) &&
                                    obj.TipoOcorrencia.Codigo == filtrosPesquisa.CodigoTipoOcorrencia);
            }

            return query;
        }

    }
}
