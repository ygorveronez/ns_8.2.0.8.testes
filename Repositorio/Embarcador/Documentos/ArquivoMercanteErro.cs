using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class ArquivoMercanteErro : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro>
    {
        public ArquivoMercanteErro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro> BuscarPorArquivo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro>();
            query = query.Where(o => o.ArquivoMercante.Codigo == codigo);
            return query.ToList();
        }

        public void DeletarPorArquivoMercante(long codigoArquivoMercante)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM ArquivoMercanteErro c WHERE c.ArquivoMercante.Codigo = :codigoArquivoMercante").SetInt64("codigoArquivoMercante", codigoArquivoMercante).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM ArquivoMercanteErro c WHERE c.ArquivoMercante.Codigo = :codigoArquivoMercante").SetInt64("codigoArquivoMercante", codigoArquivoMercante).ExecuteUpdate();

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
