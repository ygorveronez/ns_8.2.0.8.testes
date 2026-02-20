using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class SessaoRoteirizadorPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>
    {
        #region Construtores

        public SessaoRoteirizadorPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public List<int> BuscarSessoesPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>().Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.Select(o => o.SessaoRoteirizador.Codigo).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> BuscarSessoesPorPedidos(List<int> codigosPedidos)
        {
            var sqlQuery = @"
SELECT DISTINCT PED_CODIGO as Codigo
     , SRO_CODIGO as Total
  FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP
 WHERE SRP.PED_CODIGO IN ( :codigos )
   AND (SRP.SRP_SITUACAO <> :situacao 
        OR EXISTS (SELECT 1
		             FROM T_CARREGAMENTO_PEDIDO CRP INNER JOIN T_CARREGAMENTO CRG ON CRG.CRG_CODIGO = CRP.CRG_CODIGO
					WHERE CRP.PED_CODIGO = SRP.PED_CODIGO
					  AND CRG.SRO_CODIGO = SRP.SRO_CODIGO
					  AND CRG.CRG_SITUACAO <> :situacaoCarregamento ))
 ORDER BY 1; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameterList("codigos", codigosPedidos);
            query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);
            query.SetParameter("situacaoCarregamento", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var result = from obj in query
                         where obj.Pedido.Codigo == codigoPedido &&
                               obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> BuscarSessaoRoteirizadorPorPedidos(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
                query = (from obj in query
                         where
                            tmp.Contains(obj.Pedido.Codigo) && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                            && obj.SessaoRoteirizador.SituacaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Cancelada
                         select obj);

                result.AddRange(query.Fetch(x => x.SessaoRoteirizador)
                                     .ToList());
                start += take;
            }

            return result;
        }

        //public int QtdePedidosSessaoRoteirizador(int codigoSessao, bool pedidosSemCarga = false)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>().Where(p => p.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
        //                                                                                                                                      p.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado && 
        //                                                                                                                                      p.Pedido.PedidoTotalmenteCarregado == false &&
        //                                                                                                                                      p.Pedido.SaldoVolumesRestante > 0);
        //    var result = from obj in query where obj.SessaoRoteirizador.Codigo == codigoSessao select obj;

        //    if (pedidosSemCarga)
        //    {
        //        var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
        //        var resultSubQuery = from obj in subQuery where obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;
        //        result = result.Where(x => !resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));
        //    }

        //    return result.Count();
        //}

        public int QtdePedidosSessaoRoteirizadorCarga(int codigoSessao, bool pedidosSemCarga = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var result = from obj in query
                         where obj.SessaoRoteirizador.Codigo == codigoSessao &&
                               obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                               obj.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado &&
                               obj.Pedido.PedidoTotalmenteCarregado == false &&
                               obj.Pedido.SaldoVolumesRestante > 0
                         select obj;

            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultSubQuery = from obj in subQuery where obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;

            if (pedidosSemCarga)
                result = result.Where(x => !resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));
            else
                result = result.Where(x => resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> QuantidadePedidosSessoesRoteirizador(List<int> codigosSessoes)
        {
            var sqlQuery = @"
SELECT srp.SRO_CODIGO as Codigo
     , count(1) as Total
  FROM T_SESSAO_ROTEIRIZADOR_PEDIDO srp
     , T_PEDIDO PED
 WHERE PED.PED_CODIGO = srp.PED_CODIGO
   AND PED.PED_SITUACAO <> :situacaoPedido 
   AND srp.SRP_SITUACAO <> :situacao 
   AND srp.SRO_CODIGO in ( :codigos )
 GROUP BY srp.SRO_CODIGO
 ORDER BY 1 ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("situacaoPedido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);
            query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);
            query.SetParameterList("codigos", codigosSessoes);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> PedidosSessaoRoteirizador(int codigoSessao, bool pedidosSemCarga = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var result = from obj in query
                         where obj.SessaoRoteirizador.Codigo == codigoSessao &&
                               obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;

            if (pedidosSemCarga)
            {
                var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var resultSubQuery = from obj in subQuery where obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;
                result = result.Where(x => !resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));
            }

            result = result.Fetch(x => x.Pedido)
                           .ThenFetch(x => x.Destinatario);

            return result.ToList();
        }

        public string CargasPedidosSessaoRoteirizadorCarga(int codigoSessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada select obj;

            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var resultSubQuery = from obj in subQuery
                                 where obj.SessaoRoteirizador.Codigo == codigoSessao &&
                                       obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                                 select obj;
            result = result.Where(x => resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));

            return string.Join(",", (from item in result select item.Carga.CodigoCargaEmbarcador).Distinct().ToList());
        }

        public bool PedidoNaSessao(int codigoSessao, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>().Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);
            query = query.Where(x => x.SessaoRoteirizador.Codigo == codigoSessao && x.Pedido.Codigo == codigoPedido);
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> BuscarSessaoRoteirizadorPedidos(int codigoSessao, List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
                query = query.Where(o => tmp.Contains(o.Pedido.Codigo) &&
                                         o.SessaoRoteirizador.Codigo == codigoSessao &&
                                         o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);

                result.AddRange(query.Fetch(x => x.Pedido)
                                     .ToList());
                start += take;
            }
            return result;
        }

        public bool RelacionarPedidosSessao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao,
                                            List<int> codigosPedidos,
                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador opcao,
                                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos,
                                            ref int qtdeAddSessao,
                                            ref int qtdeProdutosAddSessao,
                                            Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                //Vamos filtrar somente os pedidos que não estão na sessão
                if (opcao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO)
                    codigosPedidos = BuscarCodigosPedidosNaoEncontradosNaSessao(sessao, codigosPedidos);

                if (codigosPedidos?.Count > 0)
                {
                    unitOfWork.Start();

                    string sqlQuery = @"
INSERT INTO T_SESSAO_ROTEIRIZADOR_PEDIDO (SRO_CODIGO, PED_CODIGO, SRP_SITUACAO) 
SELECT :SRO_CODIGO, PED_CODIGO, :SRP_SITUACAO
  FROM T_PEDIDO     PED
 WHERE PED_CODIGO IN ( :codigos )
   AND NOT EXISTS (SELECT 1 
                     FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP 
                    WHERE SRP.SRO_CODIGO = :SRO_CODIGO 
                      AND SRP.PED_CODIGO = PED.PED_CODIGO 
                      AND SRP.SRP_SITUACAO <> 4);";

                    int take = 1000;
                    int start = 0;

                    while (start < codigosPedidos.Count)
                    {

                        List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                        var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                        query.SetParameter("SRO_CODIGO", sessao.Codigo);
                        query.SetParameter("SRP_SITUACAO", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.OK);
                        query.SetParameterList("codigos", tmp);
                        query.SetTimeout(180).ExecuteUpdate();
                        start += take;

                        qtdeAddSessao += tmp.Count;
                    }

                    List<int> codigosPedidosProdutos = (from pedidoProduto in pedidosProdutos
                                                        select pedidoProduto.Codigo).Distinct().ToList();

                    sqlQuery = @"
                            INSERT INTO T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO (SRP_CODIGO, PRP_CODIGO) 
                            SELECT SRP.SRP_CODIGO, PRP_CODIGO
                              FROM T_PEDIDO_PRODUTO     PPR 
                                 , T_SESSAO_ROTEIRIZADOR_PEDIDO SRP
                             WHERE PPR.PRP_CODIGO IN ( :codigos )
                               AND PPR.PED_CODIGO = SRP.PED_CODIGO
                               AND SRP.SRP_SITUACAO <> 4
                               AND SRP.SRO_CODIGO = :SRO_CODIGO
                               AND NOT EXISTS (SELECT 1 
                                                 FROM T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO SRI 
                                                WHERE SRI.SRP_CODIGO = SRP.SRP_CODIGO
                                                  AND SRI.PRP_CODIGO = PPR.PRP_CODIGO );";

                    start = 0;

                    while (start < codigosPedidosProdutos?.Count)
                    {
                        List<int> tmp = codigosPedidosProdutos.Skip(start).Take(take).ToList();
                        var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                        query.SetParameterList("codigos", tmp);
                        query.SetParameter("SRO_CODIGO", sessao.Codigo);
                        query.ExecuteUpdate();
                        start += take;

                        qtdeProdutosAddSessao += tmp.Count;
                    }

                    unitOfWork.CommitChanges();
                }

                return true;
            }
            catch
            {
                if (codigosPedidos?.Count > 0)
                    unitOfWork.Rollback();
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> PedidosInconsistentesCarregamentoAutomatico(int codigoSessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var result = from obj in query
                         where obj.SessaoRoteirizador.Codigo == codigoSessao &&
                               obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.OK &&
                               obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;

            //Verificando os pedidos que não estão em nenhum carregamento da sessão
            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            var resultSubQuery = from obj in subQuery
                                 where obj.Carregamento.SessaoRoteirizador.Codigo == codigoSessao &&
                                       obj.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                                 select obj;

            result = result.Where(x => !resultSubQuery.Any(c => c.Pedido.Codigo == x.Pedido.Codigo));
            result = result.Fetch(x => x.Pedido);

            return result.ToList();
        }

        public void AtualizarSituacao(int codigoSessao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido situacao)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery(@"
Update SessaoRoteirizadorPedido 
SET Situacao                  = :situacao 
WHERE SessaoRoteirizador.Codigo = :cod_sessao 
AND Situacao                 <> :removido ").SetParameter("situacao", situacao)
                                            .SetInt32("cod_sessao", codigoSessao)
                                            .SetParameter("removido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao)
                                            .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery(@"
Update SessaoRoteirizadorPedido 
SET Situacao                  = :situacao 
WHERE SessaoRoteirizador.Codigo = :cod_sessao 
AND Situacao                 <> :removido ").SetParameter("situacao", situacao)
                                        .SetInt32("cod_sessao", codigoSessao)
                                        .SetParameter("removido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao)
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

        public void AtualizarSituacao(int codigoSessao, List<int> pedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido situacao)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                int take = 1000;
                int start = 0;
                while (start < pedidos?.Count)
                {
                    List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                    UnitOfWork.Sessao.CreateQuery(@"
Update SessaoRoteirizadorPedido 
SET Situacao                  = :situacao 
WHERE SessaoRoteirizador.Codigo = :cod_sessao 
AND Situacao                 <> :removido 
AND Pedido.Codigo            in ( :pedidos )").SetParameter("situacao", situacao)
                                            .SetInt32("cod_sessao", codigoSessao)
                                            .SetParameter("removido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao)
                                            .SetParameterList("pedidos", tmp)
                                            .ExecuteUpdate();
                    start += take;
                }
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    int take = 1000;
                    int start = 0;
                    while (start < pedidos?.Count)
                    {
                        List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                        UnitOfWork.Sessao.CreateQuery(@"
Update SessaoRoteirizadorPedido 
SET Situacao                  = :situacao 
WHERE SessaoRoteirizador.Codigo = :cod_sessao 
AND Situacao                 <> :removido 
AND Pedido.Codigo            in ( :pedidos )").SetParameter("situacao", situacao)
                                        .SetInt32("cod_sessao", codigoSessao)
                                        .SetParameter("removido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao)
                                        .SetParameterList("pedidos", tmp)
                                        .ExecuteUpdate();
                        start += take;
                    }
                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void DeletarTodos(int codigoSessao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM SessaoRoteirizadorPedidoProduto WHERE SessaoRoteirizadorPedido.Codigo in (SELECT c.Codigo FROM SessaoRoteirizadorPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessao).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM SessaoRoteirizadorPedido WHERE Codigo in (SELECT c.Codigo FROM SessaoRoteirizadorPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessao).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM SessaoRoteirizadorPedidoProduto WHERE SessaoRoteirizadorPedido.Codigo in (SELECT c.Codigo FROM SessaoRoteirizadorPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessao).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM SessaoRoteirizadorPedido WHERE Codigo in (SELECT c.Codigo FROM SessaoRoteirizadorPedido c WHERE c.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessao).ExecuteUpdate();

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

        public List<int> BuscarCodigosPedidosNaoEncontradosNaSessao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao, List<int> codigosPedidos)
        {
            if (sessao == null)
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = this.BuscarSessaoRoteirizadorPedidos(sessao.Codigo, codigosPedidos);
            List<int> codigosPedidosForaSessao = codigosPedidos.Where(codigoPedido => !pedidosSessao.Exists(pedidoSessao => pedidoSessao.Pedido.Codigo == codigoPedido)).ToList();

            return codigosPedidosForaSessao;
        }
    }
}
