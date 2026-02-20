using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Integracao.Monitoramento
{
    public class ConfiguracaoIntegracaoTecnologiaMonitoramentoConta : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta>
    {
        public ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> BuscarPorConfiguracao(Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta>();

            query = query.Where(o => o.Configuracao == configuracao);

            return query.ToList();
        }

        public bool ExistePorConfiguracao(Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta>();

            query = query.Where(o => o.Configuracao == configuracao);

            return query.Select(o => o.Codigo).Any();
        }

        public void DeletarPorConfiguracao(Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ConfiguracaoIntegracaoTecnologiaMonitoramentoConta obj WHERE obj.Configuracao = :configuracao")
                                     .SetEntity("configuracao", configuracao)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ConfiguracaoIntegracaoTecnologiaMonitoramentoConta obj WHERE obj.Configuracao = :configuracao")
                                     .SetEntity("configuracao", configuracao)
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
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}
