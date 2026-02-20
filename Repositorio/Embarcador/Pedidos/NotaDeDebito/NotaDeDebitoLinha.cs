using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos.NotaDeDebito
{
    public class NotaDeDebitoLinha : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>
    {
        public NotaDeDebitoLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarCodigosLinhasPendentesGeracaoPedido(int codigoImportacaoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();

            query = query.Where(o => o.NotaDeDebito.Codigo == codigoImportacaoNotaDebito && o.Pedido == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> BuscarPorNota(int codigoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();

            query = query.Where(o => o.NotaDeDebito.Codigo == codigoNotaDebito);

            return query.Fetch(o => o.Pedido).Fetch(o => o.Carga).OrderBy(o => o.Numero).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> BuscarSemCargaPorNota(int codigoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();

            query = query.Where(o => o.NotaDeDebito.Codigo == codigoNotaDebito && o.Carga == null && o.Pedido != null);

            return query.Fetch(o => o.Pedido).ThenFetch(o => o.TipoOperacao)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.Veiculos)
                        .Fetch(o => o.Pedido).ThenFetch(o => o.CTesTerceiro)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> BuscarPorPedidos(List<int> codigosPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();
            query = query.Where(o => codigosPedidos.Contains(o.Pedido.Codigo));
            return query.ToList();
        }

        public int ContarPedidosPorNotaDebito(int codigoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();

            query = query.Where(o => o.NotaDeDebito.Codigo == codigoNotaDebito && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Pedido.Codigo).Distinct().Count();
        }

        public int ContarCargasPorNotaDebito(int codigoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha>();

            query = query.Where(o => o.NotaDeDebito.Codigo == codigoNotaDebito && o.Pedido != null && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Carga.Codigo).Distinct().Count();
        }

        public void SetarCargaLinhas(int codigoLinha, int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE NotaDeDebitoLinha linha SET Carga = :codigoCarga WHERE Codigo = :codigoLinha")
                        .SetParameter("codigoLinha", codigoLinha)
                        .SetParameter("codigoCarga", codigoCarga)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE NotaDeDebitoLinha linha SET Carga = :codigoCarga WHERE Codigo = :codigoLinha")
                            .SetParameter("codigoLinha", codigoLinha)
                            .SetParameter("codigoCarga", codigoCarga)
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
                        throw new Exception("O registro possui dependências e não pode ser excluído.", ex);
                    }
                }
                throw;
            }
        }
    }
}
