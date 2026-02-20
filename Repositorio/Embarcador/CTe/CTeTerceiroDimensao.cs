using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroDimensao : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao>
    {
        public CTeTerceiroDimensao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao> BuscarPorCTeParaSubContratacao(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao> BuscarPorCTeParaSubContratacao(List<int> cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao>();
            var result = query.Where(obj => cteParaSubContratacao.Contains(obj.CTeTerceiro.Codigo));
            return result.ToList();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroDimensao obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroDimensao obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                    .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
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

        #endregion
    }
}
