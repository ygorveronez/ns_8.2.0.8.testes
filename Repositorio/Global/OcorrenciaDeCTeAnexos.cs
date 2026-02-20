using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class OcorrenciaDeCTeAnexos : RepositorioBase<Dominio.Entidades.OcorrenciaDeCTeAnexos>, Dominio.Interfaces.Repositorios.OcorrenciaDeCTeAnexos
    {
        public OcorrenciaDeCTeAnexos(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OcorrenciaDeCTeAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTeAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.OcorrenciaDeCTeAnexos BuscarPorCodigoEOcorrencia(int codigo, int codigoOcorrencia, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTeAnexos>();
            var result = from obj in query
                         where
                            obj.Codigo == codigo &&
                            obj.Ocorrencia.Codigo == codigoOcorrencia &&
                            obj.Ocorrencia.CTe.Empresa.Codigo == codigoEmpresa
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTeAnexos> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTeAnexos>();
            var result = from obj in query where obj.Ocorrencia.Codigo == codigoOcorrencia select obj;
            return result.ToList();
        }
    }
}
