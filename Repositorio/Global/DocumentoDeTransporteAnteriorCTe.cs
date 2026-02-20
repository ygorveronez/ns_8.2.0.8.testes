using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DocumentoDeTransporteAnteriorCTe : RepositorioBase<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>, Dominio.Interfaces.Repositorios.DocumentoDeTransporteAnteriorCTe
    {
        public DocumentoDeTransporteAnteriorCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> BuscarPorCTeComLimite(int codigoEmpresa, int codigoCTe, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>()
                .Where(obj => obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa)
                .Take(limite)
                .AsEnumerable()
                .GroupBy(obj => obj.Chave)
                .Select(g => g.First())
                .ToList();

            return query;
        }

        public Dominio.Entidades.DocumentoDeTransporteAnteriorCTe BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarChaveDocumentosEletronicosPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            query = query.Where(obj => obj.CTe.Codigo == codigoCTe && obj.Chave != null && obj.Chave != string.Empty);
            return query.Select(o => o.Chave).ToList();
        }

        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> BuscarDocumentosEletronicosPorCTe(int codigoEmpresa, int codigoCTe)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>()
            .Where(obj => obj.CTe.Codigo == codigoCTe &&
                          obj.CTe.Empresa.Codigo == codigoEmpresa &&
                          obj.Chave != null &&
                          obj.Chave != string.Empty)
            .Fetch(obj => obj.Emissor)
            .ToList();
        }

        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> BuscarDocumentosPapelPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa && (obj.Chave == null || obj.Chave == string.Empty) select obj;
            return result.ToList();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE DocumentoDeTransporteAnteriorCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE DocumentoDeTransporteAnteriorCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
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
        public List<string> BuscarChavesCteAnterioresPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj.Chave;
            return result.Distinct().ToList();
        }
    }
}
