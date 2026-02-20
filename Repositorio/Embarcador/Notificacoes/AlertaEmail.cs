using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Notificacoes
{
    public sealed class AlertaEmail : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail>
    {
        #region Construtores

        public AlertaEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail filtroPesquisa)
        {
            var consultaAlertaEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail>();

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.DataHoraInicio.HasValue)
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.DataHoraInicio.Date >= filtroPesquisa.DataHoraInicio.Value.Date);

            if (filtroPesquisa.DataHoraFim.HasValue)
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.DataHoraFim.Value.Date <= filtroPesquisa.DataHoraFim.Value.Date);

            if (filtroPesquisa.Setor.Count > 0)
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Setor.Any(o => filtroPesquisa.Setor.Contains(o.Codigo)));

            if (filtroPesquisa.Portfolio.Count > 0)
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Portfolio.Any(o => filtroPesquisa.Portfolio.Contains(o.Codigo)));

            if (filtroPesquisa.Irregularidade.Count > 0)
                consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Irregularidade.Any(o => filtroPesquisa.Irregularidade.Contains(o.Codigo)));


            return consultaAlertaEmail;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail filtrosPesquisa)
        {
            var consultaAlertaEmail = Consultar(filtrosPesquisa);

            return ObterLista(consultaAlertaEmail, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail filtrosPesquisa)
        {
            var consultaAlertaEmail = Consultar(filtrosPesquisa);

            return consultaAlertaEmail.Count();
        }

        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail)
        {
            var consultaAlertaEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail>();

            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Codigo != alertaEmail.Codigo);
            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Descricao.Equals(alertaEmail.Descricao));
            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Setor.All(o => alertaEmail.Setor.Contains(o)));
            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Portfolio.All(o => alertaEmail.Portfolio.Contains(o)));
            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Irregularidade.All(o => alertaEmail.Irregularidade.Contains(o)));
            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Usuarios.All(o => alertaEmail.Usuarios.Contains(o)));

            if (!consultaAlertaEmail.Any())
                return ExisteDataSobreposta(alertaEmail);
            return true;
        }

        private bool ExisteDataSobreposta(Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail)
        {
            var consultaAlertaEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail>();

            consultaAlertaEmail = consultaAlertaEmail.Where(obj => obj.Codigo != alertaEmail.Codigo);
            consultaAlertaEmail = consultaAlertaEmail.Where
                (obj => ((obj.DataHoraFim.HasValue && ((alertaEmail.DataHoraInicio >= obj.DataHoraInicio && alertaEmail.DataHoraInicio <= obj.DataHoraFim)
                                        || (alertaEmail.DataHoraFim >= obj.DataHoraInicio && alertaEmail.DataHoraFim <= obj.DataHoraFim)
                                        || (alertaEmail.DataHoraInicio <= obj.DataHoraInicio && alertaEmail.DataHoraFim >= obj.DataHoraFim)))
            || ((obj.DataHoraFim.HasValue && !alertaEmail.DataHoraFim.HasValue) && (alertaEmail.DataHoraInicio <= obj.DataHoraFim))
            || (!obj.DataHoraFim.HasValue && !alertaEmail.DataHoraFim.HasValue)
            || ((!obj.DataHoraFim.HasValue && alertaEmail.DataHoraFim.HasValue) && ((alertaEmail.DataHoraInicio >= obj.DataHoraInicio) || (alertaEmail.DataHoraFim >= obj.DataHoraInicio)))));

            return consultaAlertaEmail.Any();
        }
        #endregion
    }
}
