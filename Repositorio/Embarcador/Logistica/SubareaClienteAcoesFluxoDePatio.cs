using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class SubareaClienteAcoesFluxoDePatio : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio>
    {
        public SubareaClienteAcoesFluxoDePatio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio> BuscarPorCodigoSubareaCliente(int codigoSubareaCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio>();
            query = query.Where(obj => obj.SubareaCliente.Codigo == codigoSubareaCliente);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio> BuscarPorCodigosSubareaCliente(List<int> codigosSubareaCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio>();
            query = query.Where(obj => codigosSubareaCliente.Contains(obj.SubareaCliente.Codigo));
            return query.ToList();
        }
    }
}
