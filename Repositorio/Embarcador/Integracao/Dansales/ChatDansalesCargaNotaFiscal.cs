using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao.Dansales
{
    public class ChatDansalesCargaNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal>
    {
        public ChatDansalesCargaNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal> BuscarPorNotaFiscal(int codigoXMlNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal>();
            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMlNotaFiscal);

            return query.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal> BuscarPorMensagemChat(int codigoMensagemChat)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal>();
            query = query.Where(o => o.chatMobileMensagem.Codigo == codigoMensagemChat);

            return query.ToList();
        }

    }
}
