using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoMotorista : RepositorioBase<Dominio.Entidades.IntegracaoMotorista>
    {
        public IntegracaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoMotorista BuscarPorCodigo(int codigo, int codigoCCe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoMotorista> BuscarPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMotorista>();
            var result = from obj in query where obj.Usuario.Codigo == codigoMotorista select obj;
            return result.ToList();
        }
    }
}
