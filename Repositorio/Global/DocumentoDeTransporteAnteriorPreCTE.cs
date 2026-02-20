using System;
using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class DocumentoDeTransporteAnteriorPreCTE : RepositorioBase<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE>
    {
        public DocumentoDeTransporteAnteriorPreCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public string BuscarChaveCTeAnteriorPorPreCte(int codigoPreCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE>();
            var result = from obj in query where obj.PreCTE.Codigo == codigoPreCTe select obj.Chave;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> BuscarPorPreCte(int codigoPreCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE>();
            var result = from obj in query where obj.PreCTE.Codigo == codigoPreCTe select obj;
            return result.ToList();
        }
        
        public void DeletarPorPreCTe(int codigoPreCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE DocumentoDeTransporteAnteriorPreCTE obj WHERE obj.PreCTE.Codigo = :codigoPreCTe")
                                     .SetInt32("codigoPreCTe", codigoPreCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE DocumentoDeTransporteAnteriorPreCTE obj WHERE obj.PreCTE.Codigo = :codigoPreCTe")
                                    .SetInt32("codigoPreCTe", codigoPreCTe)
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
