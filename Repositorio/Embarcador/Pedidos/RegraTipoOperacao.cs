using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>
    {
        #region Metodos Publicos

        public RegraTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>()
                .Where(a => a.Codigo == codigo)
                .FirstOrDefault();

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>()
                .Where(a => a.Ativo);

            return query.ToList();
        }
        
        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>> BuscarTodosAtivosAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>()
                .Where(a => a.Ativo);

            return await query.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool ExisteRegraDuplicada(Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao regraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>();

            if (regraTipoOperacao.TipoDocumentoTransporte != null)
                query = query.Where(r => r.TipoDocumentoTransporte.Codigo == regraTipoOperacao.TipoDocumentoTransporte.Codigo);

            if (regraTipoOperacao.Categoria != null)
                query = query.Where(r => r.Categoria.Codigo == regraTipoOperacao.Categoria.Codigo);

            query = query.Where(r => r.TipoOperacao.Codigo == regraTipoOperacao.TipoOperacao.Codigo && r.QuantidadeEtapas == regraTipoOperacao.QuantidadeEtapas && r.Codigo != regraTipoOperacao.Codigo);
            var result = query.FirstOrDefault();
            return result == null ? false : true;

        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao BuscarRegraPorParamentros(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao quantidadeEtapas, int codigoCanalEntrega, int codigoCanalVenda, int codigoCartegoriaPessoa, int codigTipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>();

            if (quantidadeEtapas > 0)
                query = query.Where(r => r.QuantidadeEtapas == quantidadeEtapas);

            if (codigoCartegoriaPessoa > 0)
                query = query.Where(r => r.Categoria.Codigo == codigoCanalVenda);

            if (codigTipoDocumento > 0)
                query = query.Where(r => r.TipoDocumentoTransporte.Codigo == codigTipoDocumento);

            return query.FirstOrDefault();
        }

        #endregion
        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraTipoOperacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao>();

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                query = query.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.QuantidadeEtapas.HasValue)
            {
                if (filtrosPesquisa.QuantidadeEtapas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                    query = query.Where(o => o.QuantidadeEtapas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim);
                else if (filtrosPesquisa.QuantidadeEtapas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao)
                    query = query.Where(o => o.QuantidadeEtapas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao);
            }

            if (filtrosPesquisa.QuantidadeEtapas > 0)
                query = query.Where(o => o.QuantidadeEtapas > 0);

            if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                query = query.Where(obj => obj.Categoria.Codigo == filtrosPesquisa.CodigoCategoriaPessoa);

            if (filtrosPesquisa.CodigoTipoDocumentoTransporte > 0)
                query = query.Where(obj => obj.TipoDocumentoTransporte.Codigo == filtrosPesquisa.CodigoTipoDocumentoTransporte);

            if (filtrosPesquisa.Ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.Ativo);
            }


            return query;
        }
    }
    #endregion
}
