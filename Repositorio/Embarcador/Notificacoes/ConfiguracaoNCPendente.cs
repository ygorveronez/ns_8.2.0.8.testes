using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Notificacoes
{
    public sealed class ConfiguracaoNCPendente : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>
    {
        #region Construtores

        public ConfiguracaoNCPendente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();

            if (!string.IsNullOrEmpty(filtroPesquisa.Nome))
                query = query.Where(x => x.Nome.Contains(filtroPesquisa.Nome));

            if (filtroPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
                query = filtroPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo ? query = query.Where(obj => obj.Ativo == true) : query = query.Where(obj => obj.Ativo == false);

            return query;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> Consultar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNC)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();

            query = query.Where(obj => obj.Codigo != configuracaoNC.Codigo);
            query = query.Where(obj => obj.Nome.Equals(configuracaoNC.Nome));
            query = query.Where(obj => obj.Setor.All(o => configuracaoNC.Setor.Contains(o)));
            query = query.Where(obj => obj.Filial.All(o => configuracaoNC.Filial.Contains(o)));
            query = query.Where(obj => obj.TipoOperacao.All(o => configuracaoNC.TipoOperacao.Contains(o)));
            query = query.Where(obj => obj.ItemNaoConformidade.All(o => configuracaoNC.ItemNaoConformidade.Contains(o)));
            query = query.Where(obj => obj.Usuarios.All(o => configuracaoNC.Usuarios.Contains(o)));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> BuscarConfiguracaoPorNaoConformidade(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();

            query = query.Where(obj => obj.ItemNaoConformidade.Contains(naoConformidade.ItemNaoConformidade));
            query = query.Where(obj => obj.TipoOperacao.Contains(naoConformidade.CargaPedido.Carga.TipoOperacao));
            query = query.Where(obj => obj.Filial.Contains(naoConformidade.CargaPedido.Carga.Filial));
            query = query.Where(obj => obj.Tipo == TipoConfiguracaoNCPendente.Individual);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> BuscarConfiguracaoPorNaoConformidades(List<(int codigoNaoConformidade, int codigoTipoOperacao, int codigoFilia)> naoConformidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();

            query = query.Where(x => x.ItemNaoConformidade.Any(p => naoConformidade.Any(w => w.codigoNaoConformidade == p.Codigo)));

            query = query.Where(obj => obj.Tipo == TipoConfiguracaoNCPendente.Resumo);

            return query.ToList();
        }
        #endregion
    }
}
