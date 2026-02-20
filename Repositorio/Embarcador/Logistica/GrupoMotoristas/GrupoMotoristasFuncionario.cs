using Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas;
using NHibernate;
using Repositorio.Embarcador.Logistica.GrupoMotoristas.Abstrato;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasFuncionario(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : RepositorioRelacionamentoGrupoMotoristas<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario>(
            unitOfWork,
            cancellationToken,
            "T_GRUPO_MOTORISTAS_FUNCIONARIO"
        ), IRepositorioRelacionamentoGrupoMotoristas
    {
        public string ObterNomeVerbosoDaEntidade()
        {
            return "Motorista do Grupo";
        }

        public async Task<List<Dominio.Entidades.Usuario>> BuscarFuncionariosPorGrupoMotoristaAsync(
    int idGrupoMotorista,
    CancellationToken cancellationToken)
        {
            var query = @$"
        SELECT 
            Funcionario.FUN_NOME As Nome, 
            Funcionario.FUN_CPF AS CPF, 
            Funcionario.FUN_CELULAR AS Celular 
        FROM T_GRUPO_MOTORISTAS_FUNCIONARIO GrupoMotoristasFuncionario
        JOIN T_FUNCIONARIO Funcionario 
            ON GrupoMotoristasFuncionario.FUN_CODIGO = Funcionario.FUN_CODIGO
        WHERE  
            GrupoMotoristasFuncionario.GMO_CODIGO = {idGrupoMotorista}"; // SQL-INJECTION-SAFE

            ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Usuario))
            );

            return (List<Dominio.Entidades.Usuario>)await nhQuery
                .SetTimeout(600)
                .ListAsync<Dominio.Entidades.Usuario>(cancellationToken)
                .ConfigureAwait(false);
        }

        public Task ExcluirFuncionariosGrupoMotoristasAsync(int codigoGrupoMotoristas, CancellationToken cancellationToken)
        {
            string sql = @$"DELETE FROM T_GRUPO_MOTORISTAS_FUNCIONARIO WHERE GMO_CODIGO = {codigoGrupoMotoristas}"; // SQL-INJECTION-SAFE
            return SessionNHiBernate.CreateSQLQuery(sql).ExecuteUpdateAsync(cancellationToken);
        }

    }
}
