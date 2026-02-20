using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class OcorrenciaDeNFSeAnexos : RepositorioBase<Dominio.Entidades.OcorrenciaDeNFSeAnexos>
    {
        public OcorrenciaDeNFSeAnexos(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OcorrenciaDeNFSeAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSeAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.OcorrenciaDeNFSeAnexos BuscarPorCodigoEOcorrencia(int codigo, int codigoOcorrencia, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSeAnexos>();
            var result = from obj in query
                         where
                            obj.Codigo == codigo &&
                            obj.Ocorrencia.Codigo == codigoOcorrencia &&
                            obj.Ocorrencia.NFSe.Empresa.Codigo == codigoEmpresa
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.OcorrenciaDeNFSeAnexos> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSeAnexos>();
            var result = from obj in query where obj.Ocorrencia.Codigo == codigoOcorrencia select obj;
            return result.ToList();
        }
    }
}
