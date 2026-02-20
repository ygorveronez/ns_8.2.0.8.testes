using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public sealed class EmpresaIntegracaoKrona : RepositorioBase<Dominio.Entidades.EmpresaIntegracaoKrona>
    {
        #region Construtores

        public EmpresaIntegracaoKrona(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.EmpresaIntegracaoKrona BuscarPorEmpresa(int codigoEmpresa)
        {
            var consultaEmpresaIntegracaoKrona = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracaoKrona>()
                .Where(o => o.Empresa.Codigo == codigoEmpresa);

            return consultaEmpresaIntegracaoKrona.FirstOrDefault();
        }

        #endregion
    }
}
