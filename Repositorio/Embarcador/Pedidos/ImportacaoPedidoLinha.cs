using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ImportacaoPedidoLinha : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>
    {
        public ImportacaoPedidoLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarCodigosLinhasPendentesGeracaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Pedido == null);

            return query.Select(o => o.Codigo).ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> BuscarLinhasPendentesGeracaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Pedido == null);
            
            return query.Fetch(o => o.Colunas).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> BuscarPorImportacaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido);

            return query.Fetch(o => o.Pedido).Fetch(o => o.Carga).OrderBy(o => o.Numero).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> BuscarSemCargaPorImportacaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Carga == null && o.Pedido != null);

            return query.Fetch(o => o.Pedido).ThenFetch(o => o.TipoOperacao)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.Veiculos)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.CTesTerceiro)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> BuscarPorPedidos(List<int> codigosPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();
            query = query.Where(o => codigosPedidos.Contains(o.Pedido.Codigo));
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();
            query = query.Where(o => codigoPedido == o.Pedido.Codigo);
            return query.FirstOrDefault();
        }


        public int ContarPedidosPorImportacaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Pedido.Codigo).Distinct().Count();
        }

        public int ContarCargasPorImportacaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            query = query.Where(o => o.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Carga.Codigo).Distinct().Count();
        }

        public void SetarCargaLinhas(List<int> codigosLinhas, int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPedidoLinha linha SET Carga =: codigoCarga WHERE Codigo IN (:codigosLinhas)").SetParameterList("codigosLinhas", codigosLinhas).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPedidoLinha linha SET Carga =: codigoCarga WHERE Codigo IN (:codigosLinhas)").SetParameterList("codigosLinhas", codigosLinhas).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();

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

        public void SetarCargaLinha(int codigoPedido, int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPedidoLinha linha SET Carga =: codigoCarga WHERE PED_CODIGO = :codigoPedido").SetParameter("codigoPedido", codigoPedido).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoPedidoLinha linha SET Carga =: codigoCarga WHERE PED_CODIGO = :codigoPedido").SetParameter("codigoPedido", codigoPedido).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();

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
