using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class GrupoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>
    {
        #region Construtores

        public GrupoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();
            query = query.Where(obj => obj.Ativo);
            return query.OrderBy(obj => obj.Ordem).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao BuscarPorDescricao(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();
            query = query.Where(o => o.Descricao == descricao && o.Ativo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTipoOperacao = Consultar(filtrosPesquisa);
            return consultaTipoOperacao
                .OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"))
                .Skip(parametrosConsulta.InicioRegistros)
                .Take(parametrosConsulta.LimiteRegistros)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao filtrosPesquisa)
        {
            var consultaTipoOperacao = Consultar(filtrosPesquisa);
            return consultaTipoOperacao.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao filtrosPesquisa)
        {
            var consultaTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => !o.Ativo);

            return consultaTipoOperacao;
        }

        #endregion

    }
}
