using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasFuncionarioAlteracao(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionarioAlteracao>(unitOfWork, cancellationToken)
    {

        public async Task LimparAlteracoesPorCodigoGrupoMotoristaAsync(
            int codigoGrupoMotorista,
            CancellationToken cancellationToken = default)
        {
            var query = @$"
                DELETE FROM T_GRUPO_MOTORISTAS_FUNCIONARIO_ALTERACAO 
                WHERE GMO_CODIGO = {codigoGrupoMotorista}"; // SQL-INJECTION-SAFE
            await SessionNHiBernate.CreateSQLQuery(query)
                .SetTimeout(600)
                .ExecuteUpdateAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao>> BuscarFuncionariosAlteracaoMotoristaAsync(int idGrupoMotorista, CancellationToken cancellationToken)
        {
            string query = @$"select 
                        GFA.GFA_ACAO AS Acao
                        ,FUN.FUN_CPF AS CPF
                        ,FUN.FUN_NOME AS Nome
                        ,FUN.FUN_CELULAR As Celular
                    from T_GRUPO_MOTORISTAS_FUNCIONARIO_ALTERACAO  GFA
                    JOIN T_FUNCIONARIO FUN ON GFA.FUN_CODIGO = FUN.FUN_CODIGO
                    WHERE GFA.GMO_CODIGO = {idGrupoMotorista}";

            var result = await SessionNHiBernate.CreateSQLQuery(query)
                .SetTimeout(600)
                .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao>())
                .ListAsync<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao>(cancellationToken)
                .ConfigureAwait(false);

            return result.ToList();
        }

    }
}
