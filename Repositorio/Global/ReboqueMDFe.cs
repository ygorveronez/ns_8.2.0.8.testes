using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ReboqueMDFe : RepositorioBase<Dominio.Entidades.ReboqueMDFe>, Dominio.Interfaces.Repositorios.ReboqueMDFe
    {
        public ReboqueMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ReboqueMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.ReboqueMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ReboqueMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ReboqueMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ReboqueMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.ReboqueMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ReboqueMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<string> BuscarPlacasPorCarga(int codigoCarga)
        {
            var sql = "select DISTINCT T_MDFE_REBOQUE.MDR_PLACA " +
                      "from T_CARGA_PEDIDO_DOCUMENTO_MDFE " +
                      "INNER JOIN T_MDFE on T_MDFE.MDF_CODIGO = T_CARGA_PEDIDO_DOCUMENTO_MDFE.MDF_CODIGO " +
                      "INNER JOIN T_MDFE_REBOQUE on T_MDFE.MDF_CODIGO = T_MDFE_REBOQUE.MDF_CODIGO " +
                      $"WHERE T_CARGA_PEDIDO_DOCUMENTO_MDFE.CPE_CODIGO in (select CPE_CODIGO FROM T_CARGA_PEDIDO WHERE CAR_CODIGO = {codigoCarga})"; // SQL-INJECTION-SAFE

            var consultaVeiculosVinculados = this.SessionNHiBernate.CreateSQLQuery(sql).SetTimeout(600).List<string>();
            return consultaVeiculosVinculados.ToList();
        }

    }
}
