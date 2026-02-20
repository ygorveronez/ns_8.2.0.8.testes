using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class FilialModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga>
    {
        #region Construtores

        public FilialModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> BuscarPorFilial(int codigoFilial)
        {
            var consultaFilialModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga>()
                .Where(o => o.Filial.Codigo == codigoFilial);

            return consultaFilialModeloVeicularCarga
                .Fetch(o => o.ModeloVeicularCarga)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga BuscarPorFilialEModeloVeicularCarga(int codigoFilial, int codigoModeloVeicularCarga)
        {
            var consultaFilialModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return consultaFilialModeloVeicularCarga
                .FirstOrDefault();
        }

        #endregion
    }
}
