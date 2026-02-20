using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ChatMensagemPromotor : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor>
    {
        public ChatMensagemPromotor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return resut.ToList();
        }

        public bool ExistePorCargaEPromotor(int codigoCarga, int codigoPromotor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.FuncionarioVendedor.Codigo == codigoPromotor select obj;
            return resut.Any();
        }


        public Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor BuscarPorCargaEPromotor(int codigoCarga, int codigoPromotor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.FuncionarioVendedor.Codigo == codigoPromotor select obj;
            return resut.FirstOrDefault();
        }
    }
}
