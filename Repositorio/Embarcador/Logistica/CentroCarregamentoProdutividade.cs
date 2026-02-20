using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoProdutividade : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade>
    {
        public CentroCarregamentoProdutividade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> BuscarPorCentroDeCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade>();

            var result = from obj in query select obj;
            result = result.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return result.ToList();
        }
    }
}