using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainerJustificativa : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa>
    {
        #region Construtores

        public ColetaContainerJustificativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Metodos Publicos

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public virtual List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa> BuscarPorColetaContainer(int codigoColetaContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa>();

            var result = from obj in query where obj.ColetaContainer.Codigo == codigoColetaContainer select obj;

            return result.ToList();
        }

        public virtual bool PossuiRegistroPorColetaContainerEStatus(int codigoColetaContainer, StatusColetaContainer statusColetaContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa>();

            var result = from obj in query where obj.ColetaContainer.Codigo == codigoColetaContainer && obj.Status == statusColetaContainer select obj;

            return result.Any();
        }

        #endregion
    }
}