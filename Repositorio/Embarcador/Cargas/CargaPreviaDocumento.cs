using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreviaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>
    {
        public CargaPreviaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> ObterTotalizadorPorNotasFiscais(List<int> codigoPedidoXMLNotasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Documentos.Any(n => codigoPedidoXMLNotasFiscais.Contains(n.PedidoXMLNotaFiscal.Codigo))
                          group obj by obj.ModeloDocumentoFiscal.Codigo
                          into grupo
                          select new Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento()
                          {
                              Codigo = grupo.Key,
                              Total = grupo.Count()
                          };

            return retorno.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> ObterTotalizadorPorCTesTerceiro(List<int> codigoPedidoCTesParaSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Documentos.Any(n => codigoPedidoCTesParaSubcontratacao.Contains(n.PedidoCTeParaSubContratacao.Codigo))
                          group obj by obj.ModeloDocumentoFiscal.Codigo
                          into grupo
                          select new Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento()
                          {
                              Codigo = grupo.Key,
                              Total = grupo.Count()
                          };

            return retorno.ToList();
        }

        public Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> ObterTotalizadorPorCTesTerceiroDictionary(List<int> codigoPedidoCTesParaSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Documentos.Any(n => codigoPedidoCTesParaSubcontratacao.Contains(n.PedidoCTeParaSubContratacao.Codigo))
                          group obj by obj.ModeloDocumentoFiscal
                          into grupo
                          select new
                          {
                              Key = grupo.Key,
                              Count = grupo.Count()
                          };

            return retorno.ToDictionary(t => t.Key, t => t.Count);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> ObterTotalizadorPorCargaPedido(List<int> codigosCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Documentos.Any(n => codigosCargaPedidos.Contains(n.PedidoXMLNotaFiscal.CargaPedido.Codigo))
                          group obj by obj.ModeloDocumentoFiscal.Codigo
                          into grupo
                          select new Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento()
                          {
                              Codigo = grupo.Key,
                              Total = grupo.Count()
                          };

            return retorno.ToList();
        }

        public int ObterQuantidadePorCargaPedidoEModeloDocumento(int codigoCargaPedido, int codigoModeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            query = query.Where(obj => obj.Documentos.Any(n => n.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido));

            if (codigoModeloDocumento > 0)
                query = query.Where(o => o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento);

            return query.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> ObterTotalizadorPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Carga.Codigo == codigoCarga
                          group obj by obj.ModeloDocumentoFiscal.Codigo
                          into grupo
                          select new Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento()
                          {
                              Codigo = grupo.Key,
                              Total = grupo.Count()
                          };

            return retorno.ToList();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPreviaDocumentoDocumento WHERE Codigo IN (SELECT c.Codigo FROM CargaPreviaDocumentoDocumento c WHERE c.CargaPreviaDocumento.Carga.Codigo = :codigoCarga)").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPreviaDocumento c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPreviaDocumentoDocumento WHERE Codigo IN (SELECT c.Codigo FROM CargaPreviaDocumentoDocumento c WHERE c.CargaPreviaDocumento.Carga.Codigo = :codigoCarga)").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPreviaDocumento c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();

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

        public Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoPorNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            var retorno = from obj in query
                          where obj.Documentos.Any(n => n.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal)
                          select obj.ModeloDocumentoFiscal;

            return retorno.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento> ObterPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento>();

            query = query.Where(obj => obj.Documentos.Any(n => n.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal));

            return query.ToList();
        }
    }
}
