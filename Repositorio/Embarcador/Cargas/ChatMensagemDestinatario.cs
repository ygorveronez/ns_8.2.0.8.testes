using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ChatMensagemDestinatario : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario>
    {
        public ChatMensagemDestinatario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario BuscarPorCodigoChatEDestinatario(int codigoChat, int codigoMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario>();
            var resut = from obj in query where obj.ChatMobileMensagem.Codigo == codigoChat && obj.Destinatario.CodigoMobile == codigoMobile && !obj.MensagemRecebida select obj;

            return resut
                .Fetch(obj => obj.ChatMobileMensagem)
                .ThenFetch(obj => obj.Remetente)
                .FirstOrDefault();
        }


        public bool VerificarTodasMensagensEntregues(int codigoChat)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario>();
            var resut = from obj in query where obj.ChatMobileMensagem.Codigo == codigoChat && !obj.MensagemRecebida select obj;
            return resut.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario> BuscarMensagemPendentesRecebimento(int codigoMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario>();
            var resut = from obj in query where obj.Destinatario.CodigoMobile == codigoMobile && !obj.MensagemRecebida select obj;

            return resut
                .Fetch(obj => obj.ChatMobileMensagem)
                .OrderBy(obj => obj.Codigo)
                .ToList();
        }
    }
}
