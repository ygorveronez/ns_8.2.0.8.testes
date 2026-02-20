using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class CargaVeiculoContainerAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo>
    {
        #region Construtores

        public CargaVeiculoContainerAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo> BuscarPorCarga(int carga)
        {
            var consultaCargaVeiculoContainerAnexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo>()
                .Where(o => o.EntidadeAnexo.Carga.Codigo == carga);

            return consultaCargaVeiculoContainerAnexo
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo> BuscarPorCargas(List<int> cargas)
        {
            var consultaCargaVeiculoContainerAnexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo>()
                .Where(o => cargas.Contains(o.EntidadeAnexo.Carga.Codigo));

            return consultaCargaVeiculoContainerAnexo
                .ToList();
        }

        #endregion
    }
}
