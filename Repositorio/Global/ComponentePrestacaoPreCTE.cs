using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ComponentePrestacaoPreCTE : RepositorioBase<Dominio.Entidades.ComponentePrestacaoPreCTE>
    {
        #region Construtores

        public ComponentePrestacaoPreCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.ComponentePrestacaoPreCTE> BuscarPorPreCTe(int codigoPreCTe)
        {
            var consultaComponentePrestacaoPreCTE = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoPreCTE>()
                .Where(o => o.PreCTE.Codigo == codigoPreCTe);

            return consultaComponentePrestacaoPreCTE
                .Fetch(o => o.ComponenteFrete)
                .ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoPreCTE> BuscarPorPreCTes(List<int> codigosPreCTe)
        {
            List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPrestacaoPreCTE = new List<Dominio.Entidades.ComponentePrestacaoPreCTE>();
            int limiteRegistros = 1000;
            int inicio = 0;

            while (inicio < codigosPreCTe?.Count)
            {
                List<int> cosigosPreCtePaginado = codigosPreCTe.Skip(inicio).Take(limiteRegistros).ToList();
                var consultaComponentePrestacaoPreCTE = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoPreCTE>()
                    .Where(o => cosigosPreCtePaginado.Contains(o.PreCTE.Codigo));

                componentesPrestacaoPreCTE.AddRange(consultaComponentePrestacaoPreCTE.Fetch(o => o.PreCTE).ToList());

                inicio += limiteRegistros;
            }

            return componentesPrestacaoPreCTE;
        }

        public void DeletarPorPreCTE(int codigoPreCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery("DELETE ComponentePrestacaoPreCTE obj WHERE obj.PreCTE.Codigo = :codigoPreCTe")
                        .SetInt32("codigoPreCTe", codigoPreCTe)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("DELETE ComponentePrestacaoPreCTE obj WHERE obj.PreCTE.Codigo = :codigoPreCTe")
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
