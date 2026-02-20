using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoCausas : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas>
    {
        public MotivoChamadoCausas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> BuscarPorMotivoChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas>()
                .Where(o => o.MotivoChamado.Codigo == codigo)
                .OrderBy(o => o.Codigo)
                .ToList();

            return query;
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> BuscarPorMotivosChamadoAtivos(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas>()
                .Where(o => o.MotivoChamado.Codigo == codigo && o.Ativo == true)
                .OrderBy(o => o.Codigo)
                .ToList();

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas>();


            if (filtrosPesquisa.BuscarTodasCausasDesconsiderandoMotivoChamado)
            {
                query = query.Where(obj =>
                                    obj.Ativo && (string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao) || obj.Descricao.Contains(filtrosPesquisa.Descricao)));
            }
            else
            {
                query = query.Where(obj =>
                                    obj.Ativo && (string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao) || obj.Descricao.Contains(filtrosPesquisa.Descricao)) &&
                                    obj.MotivoChamado.Codigo == filtrosPesquisa.CodigoMotivoChamado);
            }

            return query;
        }

        #endregion
    }
}
