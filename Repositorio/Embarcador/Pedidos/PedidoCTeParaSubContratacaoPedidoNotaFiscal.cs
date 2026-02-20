using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoCTeParaSubContratacaoPedidoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>
    {
        public PedidoCTeParaSubContratacaoPedidoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExistePorCTeSubcontratacaoENota(int xmlNotaFiscal, int pedidoCteSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == xmlNotaFiscal && obj.PedidoCTeParaSubContratacao.Codigo == pedidoCteSubContratacao select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo == carga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarAtivosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo == codigoCarga && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorPedidoCTeParaSubcontratacao(int pedidoCteParaSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCteParaSubContratacao && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.PedidoCTeParaSubcontratacaoPedidoXMLNotaFiscal> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>()
                .Where(o => o.PedidoCTeParaSubContratacao.CargaPedido.Codigo == codigoCargaPedido && o.PedidoCTeParaSubContratacao.CTeTerceiro.Ativo);

            return query
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.PedidoCTeParaSubcontratacaoPedidoXMLNotaFiscal()
                {
                    Codigo = o.Codigo,
                    CodigoPedidoCTeParaSubcontratacao = o.PedidoCTeParaSubContratacao.Codigo,
                    CodigoPedidoXMLNotaFiscal = o.PedidoXMLNotaFiscal.Codigo
                }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorPedidosCTeParaSubcontratacao(List<int> pedidosCteParaSubContratacao)
        {
            if (pedidosCteParaSubContratacao.Count < 2000)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
                var result = from obj in query where pedidosCteParaSubContratacao.Contains(obj.PedidoCTeParaSubContratacao.Codigo) && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
                return result
                    .Fetch(obj => obj.PedidoXMLNotaFiscal)
                    .ThenFetch(obj => obj.XMLNotaFiscal)
                    .ThenFetch(obj => obj.Canhoto)
                    .ToList();
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> listaRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            List<int> listaOriginal = pedidosCteParaSubContratacao;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>()
                      .Where(obj => lote.Contains(obj.PedidoCTeParaSubContratacao.Codigo) && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva);

                listaRetornar.AddRange(
                    query
                    .Fetch(obj => obj.PedidoXMLNotaFiscal)
                    .ThenFetch(obj => obj.XMLNotaFiscal)
                    .ThenFetch(obj => obj.Canhoto)
                    .ToList()
                    );

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorPedidoCTeParaSubcontratacao(List<int> pedidoCteParaSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where pedidoCteParaSubContratacao.Contains(obj.PedidoCTeParaSubContratacao.Codigo) && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorCTeTerceiro(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosCTesPorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();

            query = query.Where(obj => obj.PedidoCTeParaSubContratacao.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.PedidoXMLNotaFiscal.CTes.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public Task<List<int>> BuscarCodigosCTesPorCTeTerceiroAsync(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>()
                .Where(obj => obj.PedidoCTeParaSubContratacao.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.PedidoXMLNotaFiscal.CTes.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> BuscarPorChaveCTeTerceiro(string chave, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query
                         where obj.PedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso == chave
       && obj.PedidoCTeParaSubContratacao.CTeTerceiro.Ativo && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva
                         select obj;

            if (carga > 0)
                result = result.Where(obj => obj.PedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo == carga);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCodigoPedidoXMLNotaFiscal(int[] codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result = from obj in query where codigoPedidoXMLNotaFiscal.Contains(obj.PedidoXMLNotaFiscal.Codigo) && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj.PedidoCTeParaSubContratacao;
            return result.ToList();
        }

        public (List<int> codigosPedidosCTeSub, List<int> codigosPedidosXML) BuscarCodigosPedidoXMLNotaFiscalPorCargaPedido(int codigoCargaPedido)
        {
            var query1 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result1 =
                from obj in query1
                where codigoCargaPedido == obj.PedidoCTeParaSubContratacao.CargaPedido.Codigo
                && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva
                select obj.Codigo;

            var query2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            var result2 =
                from obj in query2
                where codigoCargaPedido == obj.PedidoCTeParaSubContratacao.CargaPedido.Codigo
                && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva
                select obj.PedidoXMLNotaFiscal.Codigo;

            return ValueTuple.Create(result1.ToList(), result2.ToList());
        }

        public void DeletarPorCodigosEntidade(List<int> codigosEntidade)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL WHERE PSN_CODIGO IN (:codigosEntidade)")
                                                  .SetParameterList("codigosEntidade", codigosEntidade)
                                                  .SetTimeout(6000).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL WHERE PSN_CODIGO IN (:codigosEntidade)")
                                                      .SetParameterList("codigosEntidade", codigosEntidade)
                                                      .SetTimeout(6000).ExecuteUpdate();

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

        public void DeletarPorPedidosXMLNotaFiscal(List<int> codigosPedidoXMLNotaFiscal)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM PedidoCTeParaSubContratacaoPedidoNotaFiscal c WHERE c.PedidoXMLNotaFiscal.Codigo in (:codigosPedidoXMLNotaFiscal)").SetParameterList("codigosPedidoXMLNotaFiscal", codigosPedidoXMLNotaFiscal).SetTimeout(6000).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM PedidoCTeParaSubContratacaoPedidoNotaFiscal c WHERE c.PedidoXMLNotaFiscal.Codigo in (:codigosPedidoXMLNotaFiscal)").SetParameterList("codigosPedidoXMLNotaFiscal", codigosPedidoXMLNotaFiscal).SetTimeout(6000).ExecuteUpdate();

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
