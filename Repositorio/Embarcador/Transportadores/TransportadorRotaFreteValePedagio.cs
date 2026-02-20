using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorRotaFreteValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio>
    {
        public TransportadorRotaFreteValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio BuscarPorEmpresaERotaFrete(int codigoEmpresa, int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.RotaFrete.Codigo == codigoRotaFrete select obj;

            return result.FirstOrDefault();
        }
    }
}
