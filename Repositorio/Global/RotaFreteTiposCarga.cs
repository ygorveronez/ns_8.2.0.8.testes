using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class RotaFreteTiposCarga : RepositorioBase<Dominio.Entidades.RotaFreteTiposCarga>
    {
        public RotaFreteTiposCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteTiposCarga BuscarPorCodigo(int codigo)
        {
            var consultaRotaFreteTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteTiposCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaRotaFreteTipoCarga.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFreteTiposCarga> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteTiposCarga>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteTipoCarga.ToList();
        }

        public Dominio.Entidades.RotaFreteTiposCarga BuscarPorRotaFreteETipoCarga(int codigoRotaFrete, int codigoTipoCarga)
        {
            var consultaRotaFreteTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteTiposCarga>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete && o.TipoDeCarga.Codigo == codigoTipoCarga);

            return consultaRotaFreteTipoCarga.FirstOrDefault();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteTiposCarga c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFreteTiposCarga c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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
