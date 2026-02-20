using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio
{
    public class CTeContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.CTeContaContabilContabilizacao>
    {
        #region Construtores

        public CTeContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.CTeContaContabilContabilizacao> BuscarPorCTe(int codigoCTe)
        {
            var consultaContabilizacao = this.SessionNHiBernate.Query<Dominio.Entidades.CTeContaContabilContabilizacao>()
                .Where(o => o.Cte.Codigo == codigoCTe);

            return consultaContabilizacao
                .Fetch(o => o.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.CTeContaContabilContabilizacao> BuscarPorCTes(List<int> codigosCTe)
        {
            List<Dominio.Entidades.CTeContaContabilContabilizacao> result = new List<Dominio.Entidades.CTeContaContabilContabilizacao>();

            int take = 1000;
            int start = 0;
            while (start < codigosCTe?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = codigosCTe.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeContaContabilContabilizacao>();
                var filter = from o in query
                             where tmp.Contains(o.Cte.Codigo)
                             select o;

                result.AddRange(filter.Fetch(o => o.PlanoConta).ToList());

                start += take;
            }

            return result;
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery("DELETE CTeContaContabilContabilizacao Contabilizacao WHERE Contabilizacao.Cte.Codigo = :codigoCTe")
                        .SetInt32("codigoCTe", codigoCTe)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("DELETE CTeContaContabilContabilizacao Contabilizacao WHERE Contabilizacao.Cte.Codigo = :codigoCTe")
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

        #endregion Métodos Públicos
    }
}
