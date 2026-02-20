using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaTabelaFreteComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete>
    {
        public CargaTabelaFreteComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }


        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaTabelaFreteComponenteFrete obj WHERE obj.Carga.Codigo = :codigoCarga")
                           .SetInt32("codigoCarga", codigoCarga)
                           .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaTabelaFreteComponenteFrete obj WHERE obj.Carga.Codigo = :codigoCarga")
                            .SetInt32("codigoCarga", codigoCarga)
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
