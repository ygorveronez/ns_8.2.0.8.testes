using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaTabelaFreteRota : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota>
    {
        public CargaTabelaFreteRota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota BuscarPorCarga(int carga, bool tabelaFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaTabelaFreteRota obj WHERE obj.Carga.Codigo = :codigoCarga ")
                                     .SetInt32("codigoCarga", codigoCarga)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaTabelaFreteRota obj WHERE obj.Carga.Codigo = :codigoCarga ")
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
