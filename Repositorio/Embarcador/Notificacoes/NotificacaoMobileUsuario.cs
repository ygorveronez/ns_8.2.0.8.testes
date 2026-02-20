using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Notificacoes
{
    public sealed class NotificacaoMobileUsuario : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>
    {
        #region Construtores

        public NotificacaoMobileUsuario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> ConsultarPorNotificacao(int codigoNotificacao)
        {
            var consultaNotificacaoUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>()
                .Where(o => o.Notificacao.Codigo == codigoNotificacao);

            return consultaNotificacaoUsuario;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> ConsultarPorNotificacao(int codigoNotificacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNotificacaoUsuario = ConsultarPorNotificacao(codigoNotificacao);

            return ObterLista(consultaNotificacaoUsuario, parametrosConsulta);
        }

        public int ContarConsultaPorNotificacao(int codigoNotificacao)
        {
            var consultaNotificacaoUsuario = ConsultarPorNotificacao(codigoNotificacao);

            return consultaNotificacaoUsuario.Count();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> ObterNotificacoesPorUsuario(int codigoUsuario, bool somenteNaoLidas)
        {
            var consultaNotificacaoUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>()
                .Where(o => o.Usuario.Codigo == codigoUsuario);

            if (somenteNaoLidas)
                consultaNotificacaoUsuario = consultaNotificacaoUsuario.Where(o => !o.DataLeitura.HasValue);

            return consultaNotificacaoUsuario.OrderByDescending(o => o.Codigo).ToList();
        }

        #endregion
    }
}
