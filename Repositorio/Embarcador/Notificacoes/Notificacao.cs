using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Notificacoes
{
    public class Notificacao : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.Notificacao>
    {
        public Notificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Notificacoes.Notificacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Notificacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.Notificacao> Consultar(int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao situacaoNotificacao, DateTime dataInicio, DateTime dataFim, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao> TiposNotificacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Notificacao>();

            var result = from obj in query select obj;

            if (situacaoNotificacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas)
                result = result.Where(obj => obj.SituacaoNotificacao == situacaoNotificacao);

            if (usuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == usuario);


            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataNotificacao.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataNotificacao.Date <= dataFim);

            if (TiposNotificacao != null && TiposNotificacao.Count > 0)
                result = result.Where(obj => TiposNotificacao.Contains(obj.TipoNotificacao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao situacaoNotificacao, DateTime dataInicio, DateTime dataFim, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao> TiposNotificacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Notificacao>();

            var result = from obj in query select obj;

            if (situacaoNotificacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas)
                result = result.Where(obj => obj.SituacaoNotificacao == situacaoNotificacao);

            if (usuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == usuario);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataNotificacao.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataNotificacao.Date <= dataFim);

            if (TiposNotificacao != null && TiposNotificacao.Count > 0)
                result = result.Where(obj => TiposNotificacao.Contains(obj.TipoNotificacao));

            return result.Count();
        }
    }
}
