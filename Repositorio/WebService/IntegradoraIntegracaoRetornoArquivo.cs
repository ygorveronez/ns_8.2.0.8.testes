using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.WebService
{
    public class IntegradoraIntegracaoRetornoArquivo : RepositorioBase<Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo>
    {
        public IntegradoraIntegracaoRetornoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo> ();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
