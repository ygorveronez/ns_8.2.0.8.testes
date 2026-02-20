using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos.ImportacaoTakePay
{
    public class ImportacaoTakePayLinha : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>
    {
        public ImportacaoTakePayLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarCodigosLinhasPendentesGeracaoPedido(int codigoImportacaoTakePay)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();

            query = query.Where(o => o.ImportacaoTakePay.Codigo == codigoImportacaoTakePay && o.Pedido == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> BuscarPorImportacaoTakePay(int codigoImportacaoTakePay)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();

            query = query.Where(o => o.ImportacaoTakePay.Codigo == codigoImportacaoTakePay);

            return query.Fetch(o => o.Pedido).Fetch(o => o.Carga).OrderBy(o => o.Numero).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> BuscarSemCargaPorImportacaoTakePay(int codigoImportacaoTakePay)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();

            query = query.Where(o => o.ImportacaoTakePay.Codigo == codigoImportacaoTakePay && o.Carga == null && o.Pedido != null);

            return query.Fetch(o => o.Pedido).ThenFetch(o => o.TipoOperacao)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.Veiculos)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.CTesTerceiro)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> BuscarPorPedidos(List<int> codigosPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();
            query = query.Where(o => codigosPedidos.Contains(o.Pedido.Codigo));
            return query.ToList();
        }

        public int ContarPedidosPorImportacaoTakePay(int codigoImportacaoTakePay)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();

            query = query.Where(o => o.ImportacaoTakePay.Codigo == codigoImportacaoTakePay && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Pedido.Codigo).Distinct().Count();
        }

        public int ContarCargasPorImportacaoTakePay(int codigoImportacaoTakePay)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha>();

            query = query.Where(o => o.ImportacaoTakePay.Codigo == codigoImportacaoTakePay && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Carga.Codigo).Distinct().Count();
        }

        public void SetarCargaLinhas(int codigoLinha, int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoTakePayLinha linha SET Carga = :codigoCarga WHERE Codigo = :codigoLinha").SetParameter("codigoLinha", codigoLinha).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ImportacaoTakePayLinha linha SET Carga = :codigoCarga WHERE Codigo = :codigoLinha").SetParameter("codigoLinha", codigoLinha).SetParameter("codigoCarga", codigoCarga).ExecuteUpdate();

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
