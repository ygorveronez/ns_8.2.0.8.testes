using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoDocumentoCTeComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete>
    {
        public CargaPedidoDocumentoCTeComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> BuscarPorCargaPedidoDocumentoCTe(int codigoCargaPedidoDocumentoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete>();

            query = query.Where(o => o.CargaPedidoDocumentoCTe.Codigo == codigoCargaPedidoDocumentoCTe);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> BuscarPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete>();

            query = query.Where(o => o.CargaPedidoDocumentoCTe.CargaPedido == cargaPedido);

            return query.Fetch(o => o.CargaPedidoDocumentoCTe)
                        .Fetch(o => o.ComponentePrestacaoCTe).ThenFetch(o => o.ComponenteFrete)
                        .Fetch(o => o.ComponenteFrete)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete>();

            query = query.Where(o => o.CargaPedidoDocumentoCTe.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery("DELETE CargaPedidoDocumentoCTeComponenteFrete obj WHERE obj.CargaPedidoDocumentoCTe.Codigo IN (SELECT cargaPedidoDocumentoCTe.Codigo FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                        .SetInt32("codigoCargaPedido", codigoCargaPedido)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("DELETE CargaPedidoDocumentoCTeComponenteFrete obj WHERE obj.CargaPedidoDocumentoCTe.Codigo IN (SELECT cargaPedidoDocumentoCTe.Codigo FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                            .SetInt32("codigoCargaPedido", codigoCargaPedido)
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
