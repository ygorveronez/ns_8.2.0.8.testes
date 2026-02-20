using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoDeCargaSensorOpentech : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech>
    {
        public TipoDeCargaSensorOpentech(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech ConsultarPorTipoCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            return result.FirstOrDefault();
        }     
    }
}
