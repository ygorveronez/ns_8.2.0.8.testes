using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class AcertoDeViagemAnexos : RepositorioBase<Dominio.Entidades.AcertoDeViagemAnexos>, Dominio.Interfaces.Repositorios.OcorrenciaDeCTeAnexos
    {
        public AcertoDeViagemAnexos(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.AcertoDeViagemAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagemAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.AcertoDeViagemAnexos BuscarPorCodigoEAcerto(int codigo, int codigoAcerto, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagemAnexos>();
            var result = from obj in query
                         where
                            obj.Codigo == codigo &&
                            obj.AcertoDeViagem.Codigo == codigoAcerto &&
                            obj.AcertoDeViagem.Empresa.Codigo == codigoEmpresa
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AcertoDeViagemAnexos> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagemAnexos>();
            var result = from obj in query where obj.AcertoDeViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }
    }
}
