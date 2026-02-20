using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoAVIPED : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>
    {
        public IntegracaoAVIPED(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED> ConsultarPorCarga(int codigoCarga, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga).OrderByDescending(o => o.CargaPedido.Pedido.NumeroPedidoEmbarcador)
                .Skip(inicio).Take(limite);

            return query.ToList();
        }

        public int ContarConsultaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public bool CompararComNotasDoPedido(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

            bool result = query.Any(o => o.Codigo == codigo && o.CargaPedido.NotasFiscais.Contains(o.PedidoXMLNotaFiscal));

            return result;
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where CPE_CODIGO = :CodigoCargaPedido").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where CPE_CODIGO = :CodigoCargaPedido").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();

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
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public void DeletarPorPedidoXMLNotaFiscal(int codigoPedidoXML)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO = :CodigoPedidoXML").SetInt32("CodigoPedidoXML", codigoPedidoXML).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO = :CodigoPedidoXML").SetInt32("CodigoPedidoXML", codigoPedidoXML).ExecuteUpdate();

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
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }
    }
}
