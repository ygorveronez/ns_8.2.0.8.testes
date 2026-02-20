using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Creditos
{
    public class CreditoDisponivelObtido : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido>
    {
        public CreditoDisponivelObtido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }



    }
}
