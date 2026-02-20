using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Integracao.Monitoramento
{
    public class ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar>
    {
        public ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> BuscarPorConfiguracao(Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar>();

            query = query.Where(o => o.Configuracao == configuracao);

            return query.ToList();
        }

        public void DeletarPorConfiguracao(Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar obj WHERE obj.Configuracao = :configuracao")
                                     .SetEntity("configuracao", configuracao)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar obj WHERE obj.Configuracao = :configuracao")
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
