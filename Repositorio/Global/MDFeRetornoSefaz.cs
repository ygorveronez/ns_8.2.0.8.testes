using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class MDFeRetornoSefaz : RepositorioBase<Dominio.Entidades.MDFeRetornoSefaz>, Dominio.Interfaces.Repositorios.CTeRetornoSefaz
    {
        public MDFeRetornoSefaz(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.MDFeRetornoSefaz> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeRetornoSefaz>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public int BuscarSequencialEventosPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeRetornoSefaz>();
            var result = from obj in query
                         where obj.MDFe.Codigo == codigoMDFe
                               && obj.Tipo.Equals("I") 
                               && (obj.MensagemRetorno.Contains("132") || obj.MensagemRetorno.Contains("134") || obj.MensagemRetorno.Contains("135") || obj.MensagemRetorno.Contains("136"))
                         select obj;
            return result.Count();
        }

        public bool ExisteEventoInclusaoMotorista(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeRetornoSefaz>();
            var result = from obj in query
                         where obj.MDFe.Codigo == codigoMDFe
                               && obj.Tipo.Equals("I")
                               && obj.MensagemRetorno.Contains("135") 
                         select obj;
            return result.Count() > 0;
        }
    }
}
