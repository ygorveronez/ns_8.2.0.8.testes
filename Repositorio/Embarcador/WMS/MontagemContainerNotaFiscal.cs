using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.WMS
{
    public class MontagemContainerNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal>
    {
        public MontagemContainerNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> BuscarPorMontagemContainer(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal>()
                .Where(obj => obj.MontagemContainer.Codigo == codigo);

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Emitente)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.WMS.MontagemContainer BuscarPorNotaFiscal(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal>()
                .Where(obj => obj.XMLNotaFiscal.Chave == chave);

            return query
                .Select(c => c.MontagemContainer)
                .FirstOrDefault();
        }

        #endregion

        #region Métodos Privados


        #endregion
    }
}
