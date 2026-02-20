using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Notificacoes
{
    public class Chat : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.Chat>
    {
        public Chat(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Notificacoes.Chat> BuscarPorConversa(int codigoUsuarioLogado, int codigoUsuarioConversa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Chat>();

            var result = from obj in query
                         where
                            (obj.UsuarioEnvio.Codigo == codigoUsuarioLogado || obj.UsuarioRecebedor.Codigo == codigoUsuarioLogado) &&
                            (obj.UsuarioEnvio.Codigo == codigoUsuarioConversa || obj.UsuarioRecebedor.Codigo == codigoUsuarioConversa)
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.Chat> BuscarMensagensNaoLidaPorConversa(int codigoUsuarioLogado, int codigoUsuarioConversa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Chat>();

            var result = from obj in query where !obj.Lida && obj.UsuarioRecebedor.Codigo == codigoUsuarioLogado && obj.UsuarioEnvio.Codigo == codigoUsuarioConversa select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.Chat> BuscarMensagensNaoLidaParaEnvioEmail()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.Chat>();

            var result = from obj in query where !obj.Lida && !obj.EnviadoAvisoEmail select obj;

            return result.ToList();
        }

        #endregion
    }
}