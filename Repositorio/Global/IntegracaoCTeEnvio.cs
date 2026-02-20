using System.Linq;

namespace Repositorio
{
    public class IntegracaoCTeEnvio : RepositorioBase<Dominio.Entidades.IntegracaoCTeEnvio>, Dominio.Interfaces.Repositorios.IntegracaoCTeEnvio
    {
        public IntegracaoCTeEnvio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoCTeEnvio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeEnvio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCTeEnvio BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeEnvio>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }
    }
}
