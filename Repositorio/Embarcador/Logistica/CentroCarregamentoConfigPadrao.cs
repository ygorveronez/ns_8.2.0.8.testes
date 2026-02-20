using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoConfigPadrao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao>
    {
        public CentroCarregamentoConfigPadrao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao>();

            var result = from obj in query select obj;
            result = result.Where(cccp => cccp.Codigo == codigo);

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao BuscarPorCentroDeCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao>();

            var result = from obj in query select obj;
            result = result.Where(cccp => cccp.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return result.FirstOrDefault();
        }
    }

}
