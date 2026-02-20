using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasTipoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : Abstrato.RepositorioRelacionamentoGrupoMotoristas<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao>(
            unitOfWork,
            cancellationToken,
            "T_GRUPO_MOTORISTAS_TIPO_INTEGRACAO")
    {
        public async Task DeletarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, int codigoGrupoMotoristas)
        {
            var op = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao>();

            await op.Where(o => o.TipoIntegracao == tipo && o.GrupoMotoristas.Codigo == codigoGrupoMotoristas).DeleteAsync();

        }

        public Task ExcluirTipoIntegracaoGrupoMotoristasAsync(int codigoGrupoMotoristas, CancellationToken cancellationToken)
        {
            string sql = @$"DELETE FROM T_GRUPO_MOTORISTAS_TIPO_INTEGRACAO WHERE GMO_CODIGO = {codigoGrupoMotoristas}"; // SQL-INJECTION-SAFE
            return SessionNHiBernate.CreateSQLQuery(sql).ExecuteUpdateAsync(cancellationToken);
        }
    }
}
