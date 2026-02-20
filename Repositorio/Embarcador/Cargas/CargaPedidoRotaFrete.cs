using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoRotaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>
    {
        public CargaPedidoRotaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> ObterCodigosRotasFrete(List<int> codigosCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>();
            query = query.Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo));
            return query.Select(obj => obj.Codigo).ToList();
        }

        public List<int> ObterCodigosRotasFretePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>();
            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);
            return query.Select(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query
                .Fetch(obj => obj.RotaFrete)
                .ToList();
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPedidoRotaFrete c WHERE c.CargaPedido.Codigo = :codigoCargaPedido").SetInt32("codigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPedidoRotaFrete c WHERE c.CargaPedido.Codigo = :codigoCargaPedido").SetInt32("codigoCargaPedido", codigoCargaPedido).ExecuteUpdate();

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

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Any();
        }
    }
}
