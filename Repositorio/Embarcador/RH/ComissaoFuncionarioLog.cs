using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.RH
{
    public class ComissaoFuncionarioLog : RepositorioBase<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog>
    {
        public ComissaoFuncionarioLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
