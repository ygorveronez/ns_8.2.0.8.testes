using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroSeguro : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>
    {
        public CTeTerceiroSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> BuscarPorCTeParaSubContratacao(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> BuscarPorCTeParaSubContratacao(List<int> cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>();
            var result = query.Where(obj => cteParaSubContratacao.Contains(obj.CTeTerceiro.Codigo));
            return result.ToList();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;

                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_SEGURO WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroSeguro obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroSeguro obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
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
    }
}
