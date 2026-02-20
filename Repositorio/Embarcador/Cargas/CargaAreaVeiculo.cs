using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaAreaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo>
    {
        #region Construtores

        public CargaAreaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaAreaVeiculo
                .Fetch(o => o.AreaVeiculo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> BuscarPorCargas(List<int> codigosCarga)
        {
            var consultaCargaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return consultaCargaAreaVeiculo
                .Fetch(o => o.AreaVeiculo)
                .ToList();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete CargaAreaVeiculo where Carga.Codigo = :codigoCarga")
                    .SetInt32("codigoCarga", codigoCarga)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery($"delete CargaAreaVeiculo where Carga.Codigo = :codigoCarga")
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

        public bool ExistePorCarga(int codigoCarga)
        {
            var consultaCargaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaAreaVeiculo.Count() > 0;
        }

        #endregion
    }
}
