using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaIntegracaoCST : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST>
    {
        public FaturaIntegracaoCST(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST BuscarPorCST(string cst)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST>();
            var result = from obj in query where obj.CST == cst select obj;
            return result.FirstOrDefault();
        }
    }
}
