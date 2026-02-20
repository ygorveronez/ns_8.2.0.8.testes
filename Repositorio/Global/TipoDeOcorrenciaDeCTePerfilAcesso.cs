using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class TipoDeOcorrenciaDeCTePerfilAcesso : RepositorioBase<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso>
    {
        public TipoDeOcorrenciaDeCTePerfilAcesso(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int DeletarTodosPorTipoDeOcorrenciaDeCTe(int codigoTipoOcorrenciaDeCTe)
        {
            if (codigoTipoOcorrenciaDeCTe <= 0)
                return 0;

            string sql = $"DELETE FROM T_TIPO_OCORRENCIA_CTE_PERFIL_ACESSO WHERE OCO_CODIGO = {codigoTipoOcorrenciaDeCTe}"; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            
            return query.ExecuteUpdate();
        }
        
        public List<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso> BuscarPorTipoDeOcorrenciaDeCTe(int codigoTipoOcorrenciaDeCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso>()
                .Where(obj => obj.TipoDeOcorrenciaDeCTe.Codigo == codigoTipoOcorrenciaDeCTe);

            return query
                .Fetch(obj => obj.PerfilAcesso)
                .ToList();
        }
    }
}
