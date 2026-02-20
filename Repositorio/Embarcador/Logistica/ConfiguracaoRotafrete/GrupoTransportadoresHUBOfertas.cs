using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete
{
    public class GrupoTransportadoresHUBOfertas : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas>
    {
        #region Construtores

        public GrupoTransportadoresHUBOfertas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoTransportadoresHUBOfertas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos
        public int ContarGruposPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            var consultaGrupoTransportadores = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas>();

            if (codigoConfiguracaoRotaFrete > 0)
            {
                consultaGrupoTransportadores = consultaGrupoTransportadores
                    .Where(o => o.ConfiguracaoRotaFrete.Codigo == codigoConfiguracaoRotaFrete);
            }

            return consultaGrupoTransportadores.Count();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas>> BuscarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            var consultaGrupoTransportadores = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas>()
                .Where(o => o.ConfiguracaoRotaFrete.Codigo == codigoConfiguracaoRotaFrete);

            return consultaGrupoTransportadores.ToList();
        }

        public void DeletarPorConfiguracaoRotaFrete(int codigoConfiguracaoRotaFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete from GrupoTransportadoresHUBOfertas where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                        .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete from GrupoTransportadoresHUBOfertas where ConfiguracaoRotaFrete.Codigo = :codigoConfiguracaoRotaFrete ")
                            .SetInt32("codigoConfiguracaoRotaFrete", codigoConfiguracaoRotaFrete)
                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion Métodos Públicos

    }
}
