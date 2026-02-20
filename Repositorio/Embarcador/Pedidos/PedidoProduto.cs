using Dominio.Excecoes.Embarcador;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>
    {
        public PedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PedidoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPorCodigoFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.GrupoProduto)
                .Fetch(obj => obj.Pedido)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorCodigos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var registrosPagina = from obj in query where tmp.Contains(obj.Codigo) select obj;
                result.AddRange(registrosPagina.ToList());

                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>> BuscarPorCodigosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 1000;
            int start = 0;

            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                var registrosPagina = from obj in query where tmp.Contains(obj.Codigo) select obj;

                result.AddRange(await registrosPagina.ToListAsync(CancellationToken));

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarProdutosNaoIntegrados(int pedido, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == pedido && !codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorPedidos(List<int> codigosPedido)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(x => x.Pedido)
                                      .Fetch(obj => obj.Pedido)
                                      .Fetch(x => x.LinhaSeparacao)
                                      .Fetch(x => x.Produto)
                                      .ThenFetch(x => x.GrupoProduto)
                                      .Fetch(x => x.EnderecoProduto)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>> BuscarPorPedidosAsync(List<int> codigosPedido)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 2000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(x => x.Pedido)
                                      .Fetch(obj => obj.Pedido)
                                      .Fetch(x => x.LinhaSeparacao)
                                      .Fetch(x => x.Produto)
                                      .ThenFetch(x => x.GrupoProduto)
                                      .Fetch(x => x.EnderecoProduto)
                                      .ToListAsync(CancellationToken));

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>> BuscarPorPedidosSemFetchAsync(List<int> codigosPedido)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.ToListAsync(CancellationToken));

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorPedidosEProdutos(List<int> codigosPedido, List<int> codigosProdutos, List<int> codigosLinhasSeparacao)
        {
            if (codigosProdutos == null)
                codigosProdutos = new List<int>();

            if (codigosLinhasSeparacao == null)
                codigosLinhasSeparacao = new List<int>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                if (codigosProdutos.Count > 0)
                    filter = filter.Where(obj => codigosProdutos.Contains(obj.Produto.Codigo));

                if (codigosLinhasSeparacao.Count > 0)
                    filter = filter.Where(obj => codigosLinhasSeparacao.Contains(obj.LinhaSeparacao.Codigo));

                result.AddRange(filter.Fetch(x => x.Pedido)
                                      .Fetch(obj => obj.Pedido)
                                      .Fetch(x => x.LinhaSeparacao)
                                      .Fetch(x => x.Produto)
                                      .ThenFetch(x => x.GrupoProduto)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarProdutosPorPedidos(List<int> codigosPedido)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            //var result = from obj in query where codigosPedido.Contains(obj.Pedido.Codigo) select obj.Produto;
            //return result.ToList();

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> result = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj.Produto;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        public bool ExisteProdutoPedido(int codigoPedido, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Produto.Codigo == codigoProduto select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorPedidoComFetch(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.GrupoProduto)
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorCargaEntrega(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            queryCargaPedido = queryCargaPedido.Where(c => c.CargaEntrega.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(p => queryCargaPedido.Any(c => c.CargaPedido.Pedido == p.Pedido));
            return query.ToList();
        }
        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>> BuscarPorCargaAsync(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));
            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPrimeiroPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(p => queryCargaPedido.Any(c => c.Pedido == p.Pedido));
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));
            return query.ToList();
        }

        public List<(int CodigoPedido, int CodigoProduto, bool Desmembrado)> BuscarPedidosProdutosDesmembradosPorPedido(List<int> codigosPedido)
        {
            const int tamanhoPagina = 2000;
            List<(int CodigoPedido, int CodigoProduto, bool Desmembrado)> resultados = new List<(int CodigoPedido, int CodigoProduto, bool Desmembrado)>();

            for (int i = 0; i < codigosPedido.Count; i += tamanhoPagina)
            {
                List<int> codigos = codigosPedido.Skip(i).Take(tamanhoPagina).ToList();

                string sql = @"SELECT 
	                            PedidoProduto.PED_CODIGO        AS CodigoPedido,
	                            PedidoProduto.PRO_CODIGO        AS CodigoProduto,
	                            PedidoProduto.PRP_DESMEMBRAR    AS Desmembrado
                            FROM 
	                            T_PEDIDO_PRODUTO PedidoProduto
                            WHERE 
                                PedidoProduto.PRP_DESMEMBRAR = 1 AND
	                            PedidoProduto.PED_CODIGO IN (:codigos)";

                NHibernate.IQuery consultaPedidosDesmembrados = this.SessionNHiBernate.CreateSQLQuery(sql)
                    .SetParameterList("codigos", codigos);

                consultaPedidosDesmembrados.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoPedido, int CodigoProduto, bool Desmembrado)).GetConstructors().FirstOrDefault()));
                resultados.AddRange(consultaPedidosDesmembrados.List<(int codigoPedido, int codigoProduto, bool desmembrado)>());
            }

            return resultados;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPorPedidoProduto(int codigoPedido, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Produto.Codigo == codigoProduto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPorPedidoProdutoEmbarcador(int codigoPedido, string codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Produto.CodigoProdutoEmbarcador == codigoProdutoEmbarcador select obj;
            return result.FirstOrDefault();
        }

        public decimal PesoTotal(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.Sum(obj => obj.PesoUnitario * obj.Quantidade);
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> Consultar(int codigoPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (propOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Produto)
                .ToList();

        }

        public int ContarConsulta(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ConsultarPorPedidos(List<int> codigosPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            var result = from obj in query where codigosPedido.Contains(obj.Pedido.Codigo) select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (propOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Produto)
                .ToList();
        }

        public int ContarConsultaPorPedidos(List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            var result = from obj in query where codigosPedido.Contains(obj.Pedido.Codigo) select obj;

            return result.Count();
        }


        public void DeletarPorPedido(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoProduto obj WHERE obj.Pedido.Codigo = :codigo")
                                     .SetInt32("codigo", codigo)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoProduto obj WHERE obj.Pedido.Codigo = :codigo")
                                .SetInt32("codigo", codigo)
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

        /// <summary>
        /// Consulta os pedidos/produtos que ainda não foram atendidos totalmente em "CARGA"
        /// </summary>
        /// <param name="codigoPedido"></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosPedidoNaoAtendidosTotalmente(int codigoPedido)
        {
            //Pedido produto
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(x => x.Pedido.Codigo == codigoPedido);

            //Carregemento pedido produto finalizados,...
            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            var resultSubQuery = from obj in subQuery
                                 where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado
                                 select obj;
            // + x.PesoTotalEmbalagem 
            query = query.Where(x => x.Quantidade * x.PesoUnitario > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
                                     || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));
            return query.ToList();
        }

        /// <summary>
        /// Consulta os pedidos/produtos que ainda não foram atendidos totalmente em "CARGA" de uma lista de pedidos...
        /// </summary>
        /// <param name="codigosPedidos">Lista de pedidos a ser consultadda...</param>
        /// <returns>Produtos que ainda não foram carregados totalmente...</returns>
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosPedidosNaoAtendidosTotalmente(List<int> codigosPedidos)
        {
            //Pedido produto
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                //Filtando nos pedidos take...
                query = query.Where(x => tmp.Contains(x.Pedido.Codigo));

                //Carregemento pedido produto finalizados,...
                var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                var resultSubQuery = from obj in subQuery
                                     where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado
                                     select obj;

                query = query.Where(x => x.Quantidade * x.PesoUnitario > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
                                         || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));

                result.AddRange(query.ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosNaoAtendidos(int codigoSessaoRoteirizacao, int codigoFilial, long codigoDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(x => x.Pedido.Filial.Codigo == codigoFilial &&
                                     x.Pedido.Destinatario.CPF_CNPJ == codigoDestinatario);
            //x.Produto.LinhaSeparacao.Roteiriza);
            //&& (x.Produto.LinhaSeparacao == null || x.Produto.LinhaSeparacao.Roteiriza));

            var subQuerySessao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
            var resultSubQuerySessao = from obj in subQuerySessao
                                       where obj.SessaoRoteirizador.Codigo == codigoSessaoRoteirizacao &&
                                             obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                                       select obj;

            query = query.Where(p => resultSubQuerySessao.Any(s => s.Pedido.Codigo == p.Pedido.Codigo));

            //// TODO: Comentado essa subquery por questao de performance.. vamos tratar no saldo > 0
            //var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            //var resultSubQuery = from obj in subQuery
            //                     where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
            //                        && obj.CarregamentoPedido.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizacao
            //                     select obj;

            ////Se opeso total do produto é maior que a soma do peso no s carregamentos... ou não exista nenhum carregamento para o pedido produto.
            //query = query.Where(x => x.Quantidade * x.PesoUnitario + x.PesoTotalEmbalagem > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
            //                         || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));

            query = query.Fetch(x => x.Produto)
                         .ThenFetch(x => x.LinhaSeparacao)
                         .Fetch(x => x.Produto)
                         .ThenFetch(x => x.GrupoProduto)
                         .Fetch(x => x.Pedido)
                         .ThenFetch(x => x.Destinatario)
                         .Fetch(x => x.Pedido)
                         .ThenFetch(x => x.CanalEntrega);

            return query.WithOptions(x => x.SetTimeout(6000)).ToList();
        }

        /// <summary>
        /// Procedimento para consultar os produtos dos pedidos que ainda não foi foram carregados completamente...
        /// </summary>
        /// <param name="codigosPedidos">Lista d ecódigo dos pedidos..</param>
        /// <returns>Apenas as listas de produtos que ainda não foram carregados complementamente em carregamentos.</returns>
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosNaoAtendidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            query = query.Where(x => codigosPedidos.Contains(x.Pedido.Codigo));

            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            var resultSubQuery = from obj in subQuery
                                 where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                                 select obj;

            //Se opeso total do produto é maior que a soma do peso no s carregamentos... ou não exista nenhum carregamento para o pedido produto.
            query = query.Where(x => x.Quantidade * x.PesoUnitario + x.PesoTotalEmbalagem > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
                                     || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));

            query = query.Fetch(x => x.Produto)
                         .ThenFetch(x => x.LinhaSeparacao)
                         .Fetch(x => x.Pedido)
                         .ThenFetch(x => x.Destinatario)
                         .Fetch(x => x.Pedido)
                         .ThenFetch(x => x.CanalEntrega);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosNaoAtendidos(List<int> codigosPedidos, List<int> codigosProdutos, List<int> codigosLinhasSeparacao)
        {
            if (codigosProdutos == null)
                codigosProdutos = new List<int>();

            if (codigosLinhasSeparacao == null)
                codigosLinhasSeparacao = new List<int>();


            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmpCodigosPedido = codigosPedidos.Skip(start).Take(take).ToList();

                if (codigosProdutos.Count == 0)
                {
                    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                    query = query.Where(x => tmpCodigosPedido.Contains(x.Pedido.Codigo));

                    if (codigosLinhasSeparacao.Count > 0)
                        query = query.Where(x => codigosLinhasSeparacao.Contains(x.LinhaSeparacao.Codigo));

                    var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                    var resultSubQuery = from obj in subQuery
                                         where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                                         select obj;

                    //Se opeso total do produto é maior que a soma do peso no s carregamentos... ou não exista nenhum carregamento para o pedido produto.
                    query = query.Where(x => x.Quantidade * x.PesoUnitario + x.PesoTotalEmbalagem > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
                                             || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));

                    query = query.Fetch(x => x.Produto)
                                 .ThenFetch(x => x.LinhaSeparacao)
                                 .Fetch(x => x.Pedido)
                                 .ThenFetch(x => x.Destinatario)
                                 .Fetch(x => x.Pedido)
                                 .ThenFetch(x => x.CanalEntrega);

                    result.AddRange(query.ToList());
                }
                else
                {
                    int takeProduto = 1000;
                    int startProduto = 0;
                    while (startProduto < codigosProdutos?.Count)
                    {
                        List<int> tmpCodigosProdutos = codigosProdutos.Skip(startProduto).Take(takeProduto).ToList();

                        var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                        query = query.Where(x => tmpCodigosPedido.Contains(x.Pedido.Codigo));

                        query = query.Where(x => tmpCodigosProdutos.Contains(x.Produto.Codigo));

                        if (codigosLinhasSeparacao.Count > 0)
                            query = query.Where(x => codigosLinhasSeparacao.Contains(x.LinhaSeparacao.Codigo));

                        var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                        var resultSubQuery = from obj in subQuery
                                             where obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                                             select obj;

                        //Se opeso total do produto é maior que a soma do peso no s carregamentos... ou não exista nenhum carregamento para o pedido produto.
                        query = query.Where(x => x.Quantidade * x.PesoUnitario + x.PesoTotalEmbalagem > resultSubQuery.Where(p => p.PedidoProduto.Codigo == x.Codigo).Sum(c => c.Peso)
                                                 || !resultSubQuery.Any(p => p.PedidoProduto.Codigo == x.Codigo));

                        query = query.Fetch(x => x.Produto)
                                     .ThenFetch(x => x.LinhaSeparacao)
                                     .Fetch(x => x.Pedido)
                                     .ThenFetch(x => x.Destinatario)
                                     .Fetch(x => x.Pedido)
                                     .ThenFetch(x => x.CanalEntrega);

                        result.AddRange(query.ToList());

                        startProduto += takeProduto;
                    }
                }
                start += take;
            }

            return result;
        }

        public void DeletarPedidoProdutoPorCodigoPedidoViaQuery(int codigoPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_PEDIDO_PRODUTO WHERE PED_CODIGO = {codigoPedido}").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_PEDIDO_PRODUTO WHERE PED_CODIGO = {codigoPedido}").ExecuteUpdate(); // SQL-INJECTION-SAFE

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
                        throw new RepositorioException("O registro já possui Vínculos dentro do Sistema, sendo assim não possível sua exclusão.");
                }

                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarPorPedidosProdutosEmbarcador(int codigoPedido, List<string> codigosProdutosEmbarcador)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            int take = 1000;
            int start = 0;

            while (start < codigosProdutosEmbarcador?.Count)
            {
                List<string> tmp = codigosProdutosEmbarcador.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                var registrosPagina = query
                    .Where(obj => obj.Pedido.Codigo == codigoPedido && tmp.Contains(obj.Produto.CodigoProdutoEmbarcador))
                    .Fetch(o => o.Produto)
                    .ToList();

                result.AddRange(registrosPagina);
                start += take;
            }

            return result;
        }

        #region Relatório de Produtos por Pedido

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoProduto> ConsultarRelatorioPedidoProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSelectConsultaRelatorioPedidoProduto(filtrosPesquisa, false, propriedades, parametrosConsulta);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoProduto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoProduto>();
        }

        public int ContarConsultaRelatorioPedidoProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioPedidoProduto(filtrosPesquisa, true, propriedades, null);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioPedidoProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioPedidoProduto(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioPedidoProduto(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                {
                    SetarSelectRelatorioConsultaRelatorioPedidoProduto(parametrosConsulta.PropriedadeAgrupar, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(parametrosConsulta.PropriedadeAgrupar))
                        orderBy = parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                {
                    if (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta.PropriedadeAgrupar && select.Contains(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeOrdenar != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_PEDIDO Pedido ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros.ToString() + " ROWS FETCH NEXT " + parametrosConsulta.LimiteRegistros.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioPedidoProduto(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Pedido.PED_CODIGO Codigo, ";
                        groupBy += "Pedido.PED_CODIGO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "Pedido.PED_NUMERO Numero, ";
                        groupBy += "Pedido.PED_NUMERO, ";
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {
                        select += "Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, ";
                        groupBy += "Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ";
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        if (!joins.Contains(" PedidoProduto "))
                            joins += " JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.PED_CODIGO = Pedido.PED_CODIGO ";

                        if (!joins.Contains(" Produto "))
                            joins += " JOIN T_PRODUTO_EMBARCADOR Produto ON Produto.PRO_CODIGO = PedidoProduto.PRO_CODIGO";

                        select += "Produto.GRP_DESCRICAO Descricao, ";
                        groupBy += "Produto.GRP_DESCRICAO, ";
                    }
                    break;
                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        if (!joins.Contains(" PedidoProduto "))
                            joins += " JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.PED_CODIGO = Pedido.PED_CODIGO ";

                        select += "PedidoProduto.PRP_QUANTIDADE Quantidade, ";
                        groupBy += "PedidoProduto.PRP_QUANTIDADE, ";
                    }
                    break;
                case "CNPJCPFRemetenteFormatado":
                    if (!select.Contains(" CNPJCPFRemetente, "))
                    {
                        if (!joins.Contains(" Remetente "))
                            joins += " LEFT OUTER JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE";

                        select += "Remetente.CLI_CGCCPF CNPJCPFRemetente, Remetente.CLI_FISJUR TipoRemetente, ";
                        groupBy += "Remetente.CLI_CGCCPF, Remetente.CLI_FISJUR, ";
                    }
                    break;
                case "RazaoRemetente":
                    if (!select.Contains(" RazaoRemetente, "))
                    {
                        if (!joins.Contains(" Remetente "))
                            joins += " LEFT OUTER JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE";

                        select += "Remetente.CLI_NOME RazaoRemetente, ";
                        groupBy += "Remetente.CLI_NOME, ";
                    }
                    break;
                case "EstadoOrigem":
                    if (!select.Contains(" EstadoOrigem, "))
                    {
                        if (!joins.Contains(" Origem "))
                            joins += " LEFT OUTER JOIN T_LOCALIDADES Origem ON Origem.LOC_CODIGO = Pedido.LOC_CODIGO_ORIGEM";

                        select += "Origem.UF_SIGLA EstadoOrigem, ";
                        groupBy += "Origem.UF_SIGLA, ";
                    }
                    break;
                case "CNPJCPFDestinatarioFormatado":
                    if (!select.Contains(" CNPJCPFDestinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT OUTER JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO";

                        select += "Destinatario.CLI_CGCCPF CNPJCPFDestinatario, Destinatario.CLI_FISJUR TipoDestinatario, ";
                        groupBy += "Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ";
                    }
                    break;
                case "RazaoDestinatario":
                    if (!select.Contains(" RazaoDestinatario, "))
                    {
                        if (!joins.Contains(" Destinatario "))
                            joins += " LEFT OUTER JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO";

                        select += "Destinatario.CLI_NOME RazaoDestinatario, ";
                        groupBy += "Destinatario.CLI_NOME, ";
                    }
                    break;
                case "EstadoDestino":
                    if (!select.Contains(" EstadoDestino, "))
                    {
                        if (!joins.Contains(" Destino "))
                            joins += " LEFT OUTER JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO";

                        select += "Destino.UF_SIGLA EstadoDestino, ";
                        groupBy += "Destino.UF_SIGLA, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioPedidoProduto(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != null)
                where += " AND Pedido.PED_DATA_INICIAL_COLETA >= '" + filtrosPesquisa.DataInicial.Value.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinal != null)
                where += " AND Pedido.PED_DATA_FINAL_COLETA <= '" + filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.Remetente > 0)
                where += " AND Pedido.CLI_CODIGO_REMETENTE = " + filtrosPesquisa.Remetente;

            if (filtrosPesquisa.Destinatario > 0)
                where += " AND Pedido.CLI_CODIGO = " + filtrosPesquisa.Destinatario;

            if (filtrosPesquisa.CodigosProduto.Count > 0)
            {
                if (!joins.Contains(" PedidoProduto "))
                    joins += " JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.PED_CODIGO = Pedido.PED_CODIGO ";

                where += " AND PedidoProduto.PRO_CODIGO IN (" + string.Join(", ", filtrosPesquisa.CodigosProduto) + ")";
            }
        }

        #endregion
    }
}
