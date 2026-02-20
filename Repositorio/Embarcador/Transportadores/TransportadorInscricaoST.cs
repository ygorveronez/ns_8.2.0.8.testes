using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorInscricaoST : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST>
    {
        public TransportadorInscricaoST(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST BuscarPorEmpresaEEstado(int codigoEmpresa, string estado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Estado.Sigla.Equals(estado) || obj.Estado == null) select obj;

            return result.FirstOrDefault();
        }
    }
}
