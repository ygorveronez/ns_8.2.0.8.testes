using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoRecusaCTE : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>
    {
        #region Construtor
        public CargaPedidoRecusaCTE(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion


        #region Metodos Publicos

        //Crie uma função para buscar CTE por codigo Pedido
        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigoPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido);
            return query.Select(x => x.CTe).FirstOrDefault();
        }


        public bool ExisteRecusaPorPedidoECarga(int procoloPedido, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.Pedido.Protocolo == procoloPedido && o.CargaRecusaOrigem.Codigo == carga);

            return query.Any();
        }

        public bool ExisteRecusaCte(int codigoCte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.CTe.Codigo == codigoCte);

            return query.Any();
        }

        public bool ExisteRecusaPorPedidoECargaGerada(int procoloPedido, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.Pedido.Protocolo == procoloPedido && o.CargaRecusaGerada.Codigo == carga);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE BuscarRecusaPorPedido(int procoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.Pedido.Protocolo == procoloPedido);

            return query.OrderByDescending(x => x.Codigo).FirstOrDefault();
        }

        public bool ExisteRecusaPorPedido(int procoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.Pedido.Protocolo == procoloPedido);

            return query.Any();
        }

        public bool ExisteRecusaPorListaPedidos(List<int> procoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => procoloPedido.Contains(o.Pedido.Protocolo));

            return query.Any();
        }

        public bool CargaPossuiRecusaCte(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE>();

            query = query.Where(o => o.CargaRecusaOrigem.Codigo == carga && o.PedidoRemovido != true);

            return query.Any();
        }


        public void DeletarCargaCTERecusado(int codigoCargaCTE)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AUTORIZACAO_ALCADA_RECUSA_CHECKIN WHERE CCT_CODIGO = {codigoCargaCTE}").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_INTEGRACAO_ARQUIVO_ARQUIVO WHERE CCI_CODIGO in (select CCI_CODIGO FROM T_CARGA_CTE_INTEGRACAO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_INTEGRACAO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_COMPONENTES_FRETE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_RATEIO_CARGA_PEDIDO_PRODUTO_COMPONTENTE_FRETE WHERE RNC_CODIGO in (select RNC_CODIGO FROM T_RATEIO_CARGA_PEDIDO_CTE_PRODUTO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_RATEIO_CARGA_PEDIDO_CTE_PRODUTO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_MDFE_MANUAL_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_HISTORICO_IRREGULARRIDADE WHERE COD_CODIGO in (select COD_CODIGO FROM T_CONTROLE_DOCUMENTO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CONTROLE_DOCUMENTO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AUTORIZACAO_ALCADA_RECUSA_CHECKIN WHERE CCT_CODIGO = {codigoCargaCTE}").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_INTEGRACAO_ARQUIVO_ARQUIVO WHERE CCI_CODIGO in (select CCI_CODIGO FROM T_CARGA_CTE_INTEGRACAO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_INTEGRACAO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE_COMPONENTES_FRETE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_RATEIO_CARGA_PEDIDO_PRODUTO_COMPONTENTE_FRETE WHERE RNC_CODIGO in (select RNC_CODIGO FROM T_RATEIO_CARGA_PEDIDO_CTE_PRODUTO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_RATEIO_CARGA_PEDIDO_CTE_PRODUTO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_MDFE_MANUAL_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_HISTORICO_IRREGULARRIDADE WHERE COD_CODIGO in (select COD_CODIGO FROM T_CONTROLE_DOCUMENTO WHERE CCT_CODIGO = {codigoCargaCTE})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CONTROLE_DOCUMENTO WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_CTE WHERE CCT_CODIGO = {codigoCargaCTE} ").ExecuteUpdate(); // SQL-INJECTION-SAFE

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
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }

        #endregion Metodos Publicos
    }
}
