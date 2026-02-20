using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>
    {
        public PedidoComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete BuscarPorCompomente(int pedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool compomenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            var result = from obj in query where obj.Pedido.Codigo == pedido && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == compomenteFilialEmissora select obj;

            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            var result = from obj in query where obj.Pedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorPedidos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(o => o.Pedido)
                                      .Fetch(o => o.ComponenteFrete)
                                      .Fetch(o => o.ModeloDocumentoFiscal)
                                      .Fetch(o => o.RateioFormula).ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>> BuscarPorPedidosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();

            int take = 2000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(o => o.Pedido)
                                      .Fetch(o => o.ComponenteFrete)
                                      .Fetch(o => o.ModeloDocumentoFiscal)
                                      .Fetch(o => o.RateioFormula).ToListAsync());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorPedido(int pedido, bool componenteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            query = query.Where(o => o.Pedido.Codigo == pedido && o.ComponenteFilialEmissora == componenteFilialEmissora);
            return query.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorCodigosPedido(List<int> Codigos, bool componenteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            query = query.Where(o => Codigos.Contains(o.Pedido.Codigo) && o.ComponenteFilialEmissora == componenteFilialEmissora);
            return query.ToList();
        }


        public bool ExisteComponenteNoPedido(int codigoCarga, bool componenteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Pedido.PedidosComponente.Any(comp => comp.ValorComponente > 0m && comp.ComponenteFilialEmissora == componenteFilialEmissora));

            return query.Select(o => o.Codigo).Any();
        }

        public decimal BuscarTotalPreCargaPorCompomente(int preCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            var result = from obj in query where obj.Pedido.PreCarga.Codigo == preCarga && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> BuscarPorCargaPedidoSemComponenteFreteValor(int codigoPedido, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;

            return result.ToList();
        }


        public void DeletarPorPedido(int codigoPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE obj.Pedido.Codigo = :Pedido")
                                     .SetInt32("Pedido", codigoPedido)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE obj.Pedido.Codigo = :Pedido")
                                    .SetInt32("Pedido", codigoPedido)
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
        public void DeletarPorPedido(int codigoPedido, bool componenteFilialEmissora)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE obj.Pedido.Codigo = :Pedido  and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                     .SetInt32("Pedido", codigoPedido)
                                     .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE obj.Pedido.Codigo = :Pedido  and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                    .SetInt32("Pedido", codigoPedido)
                                    .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
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


        public void DeletarPorPedidos(List<int> codigosPedido, bool componenteFilialEmissora)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE  : codigosPedido.Contains(obj.Pedido.Codigo) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                     .SetParameterList("codigosPedido", codigosPedido)
                                     .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                     .ExecuteUpdate();

                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoComponenteFrete obj WHERE  : codigosPedido.Contains(obj.Pedido.Codigo) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                         .SetParameterList("codigosPedido", codigosPedido)
                                         .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
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
