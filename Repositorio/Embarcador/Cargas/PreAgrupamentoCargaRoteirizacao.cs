using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class PreAgrupamentoCargaRoteirizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao>
    {
        #region Construtores

        public PreAgrupamentoCargaRoteirizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao BuscarPorCodigo(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao>()
                .Where(o => o.Codigo == codigo);

            return consultaAgrupador.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao> BuscaPorCodigoAgrupador(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao>()
                .Where(o => o.Agrupador.Codigo == codigo);

            return consultaAgrupador.ToList();
        }


        public void DeletarPorAgrupamento(int codigoAgrupamento)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoCargaRoteirizacao obj WHERE obj.Agrupador.Codigo = :CodigoAgrupamento").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoCargaRoteirizacao obj WHERE obj.Agrupador.Codigo = :CodigoAgrupamento").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();
                            
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
