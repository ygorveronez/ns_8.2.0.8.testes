using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Operacional
{
    public class OperadorTipoCargaModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular>
    {
         public OperadorTipoCargaModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

         public List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular> BuscarPorOperadorTipoCarga(int codigoOperadorTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular>();
            var result = from obj in query where obj.OperadorTipoCarga.Codigo == codigoOperadorTipoCarga select obj;
            return result.ToList();
        }
    }
}
