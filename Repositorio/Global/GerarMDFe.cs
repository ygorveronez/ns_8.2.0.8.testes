using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class GerarMDFe : RepositorioBase<Dominio.Entidades.GerarMDFe>, Dominio.Interfaces.Repositorios.GerarMDFe
    {
        public GerarMDFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.GerarMDFe BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.GerarMDFe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.GerarMDFe> BuscarPorStatus(Dominio.Enumeradores.StatusMDFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.GerarMDFe>();

            var result = from obj in query where obj.Status == status select obj;

            return result.ToList();
        }
    }
}
