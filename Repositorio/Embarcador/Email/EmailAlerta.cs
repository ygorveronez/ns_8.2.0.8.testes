using System.Linq;

namespace Repositorio.Embarcador.Email
{
    public class EmailAlerta : RepositorioBase<Dominio.Entidades.Embarcador.Email.EmailAlerta>
    {
        public EmailAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Email.EmailAlerta BuscarPorTipoAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaEmail tipoAlerta)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailAlerta>();

            return consulta.Where(emailAlerta => emailAlerta.TipoAlerta == tipoAlerta).FirstOrDefault();
        }
    }
}