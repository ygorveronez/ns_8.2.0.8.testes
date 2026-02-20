using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class DocumentoMunicipioDescarregamentoMDFeProdPerigosos : RepositorioBase<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos>, Dominio.Interfaces.Repositorios.DocumentoMunicipioDescarregamentoMDFeProdPerigosos
    {
        public DocumentoMunicipioDescarregamentoMDFeProdPerigosos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos> BuscarPorDocumento(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos>();
            var result = from obj in query where obj.DocumentoMunicipioDescarregamentoMDFe.Codigo == codigoDocumento select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos>> BuscarPorDocumentoAsync(int codigoDocumento, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos>();
            var result = from obj in query where obj.DocumentoMunicipioDescarregamentoMDFe.Codigo == codigoDocumento select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.ObjetosDeValor.MDFeProdutosPerigosos> BuscarPorDocumentoParaMDFe(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoPerigosoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoDocumento select obj;
            return result.Select(o => new Dominio.ObjetosDeValor.MDFeProdutosPerigosos()
            {
                Id = 0,
                NumeroONU = o.NumeroONU.ToString(),
                NomeApropriado = o.NomeApropriado,
                ClasseRisco = o.ClasseRisco,
                GrupoEmbalagem = o.Grupo,
                QuantidadeTotal = o.Quantidade,
                QuantidadeETipo = o.Volumes,
                Excluir = false
            }).ToList();
        }

        public void DeletarPorDocumento(int codigoDocumento)
        {
            try
            {
                string sqlDelete = @"DELETE DocumentoMunicipioDescarregamentoMDFeProdPerigosos obj WHERE obj.DocumentoMunicipioDescarregamentoMDFe.Codigo = :codigoDocumento";

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery(sqlDelete)
                                     .SetInt32("codigoDocumento", codigoDocumento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery(sqlDelete)
                                            .SetInt32("codigoDocumento", codigoDocumento)
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