using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteEmpresa : RepositorioBase<Dominio.Entidades.RotaFreteEmpresa>
    {
        public RotaFreteEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.RotaFreteEmpresa> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            return BuscarPorRotaFrete(codigoRotaFrete, codigoModeloVeicularCarga: 0);
        }

        public List<Dominio.Entidades.RotaFreteEmpresa> BuscarPorRotaFrete(int codigoRotaFrete, int codigoModeloVeicularCarga)
        {
            var consultaRotaFreteEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresa>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            if (codigoModeloVeicularCarga > 0)
                consultaRotaFreteEmpresa = consultaRotaFreteEmpresa.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return consultaRotaFreteEmpresa
                .ToList();
        }

        public Dominio.Entidades.RotaFreteEmpresa BuscarPorRotaFreteTransportadorModeloVeicularCarga(int codigoRotaFrete, int codigoEmpresa, int codigoModeloVeicular)
        {
            var consultaRotaFreteEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresa>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete && o.Empresa.Codigo == codigoEmpresa && o.ModeloVeicularCarga.Codigo == codigoModeloVeicular);

            return consultaRotaFreteEmpresa.FirstOrDefault();
        }
    }
}