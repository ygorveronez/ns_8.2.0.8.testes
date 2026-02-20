using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CIOTSigaFacilLogXML : RepositorioBase<Dominio.Entidades.CIOTSigaFacilLogXML>
    {
        public CIOTSigaFacilLogXML(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CIOTSigaFacilLogXML BuscarPorCodigo(int codigoEmpresa, int integracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacilLogXML>();
            var result = from obj in query where obj.CIOT.Empresa.Codigo == codigoEmpresa && obj.Codigo == integracao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CIOTSigaFacilLogXML> Consultar(int codigoEmpresa, int ciot, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacilLogXML>();
            var result = from obj in query where obj.CIOT.Empresa.Codigo == codigoEmpresa && obj.CIOT.Codigo == ciot select obj;

            return result.OrderByDescending(o => o.DataHora).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacilLogXML>();
            var result = from obj in query where obj.CIOT.Empresa.Codigo == codigoEmpresa && obj.CIOT.Codigo == ciot select obj;

            return result.Count();
        }
    }
}
