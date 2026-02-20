using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Notificacoes
{
    public sealed class NotificacaoMobile : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile>
    {
        #region Construtores

        public NotificacaoMobile(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaNofificacaoMobile filtrosPesquisa)
        {
            var consultaNotificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Assunto))
                consultaNotificacao = consultaNotificacao.Where(o => o.Assunto.Contains(filtrosPesquisa.Assunto));

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaNotificacao = consultaNotificacao.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                var consultaNotificacaoUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>()
                    .Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

                consultaNotificacao = consultaNotificacao.Where(o => (
                    from obj in consultaNotificacaoUsuario
                    where obj.Notificacao.Codigo == o.Codigo
                    select obj.Notificacao.Codigo
                ).Contains(o.Codigo));
            }

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaNotificacao = consultaNotificacao.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaNotificacao = consultaNotificacao.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.TipoLancamento.HasValue)
                consultaNotificacao = consultaNotificacao.Where(o => o.TipoLancamento == filtrosPesquisa.TipoLancamento.Value);

            return consultaNotificacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaNofificacaoMobile filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNotificacao = Consultar(filtrosPesquisa);

            return ObterLista(consultaNotificacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaNofificacaoMobile filtrosPesquisa)
        {
            var consultaNotificacao = Consultar(filtrosPesquisa);

            return consultaNotificacao.Count();
        }

        #endregion
    }
}
