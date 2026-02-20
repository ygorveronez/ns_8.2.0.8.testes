using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEmbarcadorPedidoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal>
    {
        public CargaIntegracaoEmbarcadorPedidoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> BuscarPorCargaIntegracaoEmbarcadorPedido(long codigoCargaIntegracaoEmbarcadorPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal>();

            query = query.Where(o => o.CargaIntegracaoEmbarcadorPedido.Codigo == codigoCargaIntegracaoEmbarcadorPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> BuscarPorCargaIntegracaoEmbarcador(long codigoCargaIntegracaoEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal>();

            query = query.Where(o => o.CargaIntegracaoEmbarcadorPedido.CargaIntegracaoEmbarcador.Codigo == codigoCargaIntegracaoEmbarcador);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> BuscarPorCargaIntegracaoEmbarcadorPedidoComOperacaoMunicipal(long codigoCargaIntegracaoEmbarcadorPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal>();

            query = query.Where(o => o.CargaIntegracaoEmbarcadorPedido.Codigo == codigoCargaIntegracaoEmbarcadorPedido && !o.PossuiCTe && o.PossuiNFS && !o.PossuiNFSManual);

            return query.ToList();
        }

        public void DeletarPorCargaIntegracaoEmbarcadorPedido(long codigoCargaIntegracaoEmbarcadorPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaIntegracaoEmbarcadorPedidoNotaFiscal obj WHERE obj.CargaIntegracaoEmbarcadorPedido.Codigo = :codigoCargaIntegracaoEmbarcadorPedido").SetInt64("codigoCargaIntegracaoEmbarcadorPedido", codigoCargaIntegracaoEmbarcadorPedido).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaIntegracaoEmbarcadorPedidoNotaFiscal obj WHERE obj.CargaIntegracaoEmbarcadorPedido.Codigo = :codigoCargaIntegracaoEmbarcadorPedido").SetInt64("codigoCargaIntegracaoEmbarcadorPedido", codigoCargaIntegracaoEmbarcadorPedido).ExecuteUpdate();

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
