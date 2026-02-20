using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainerHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>
    {
        #region Construtores

        public ColetaContainerHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public virtual List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> BuscarPorColetaContainer(int codigoColetaContainer)
        {
            var consultaColetaContainerHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>()
                .Where(coletaContainerHistorico => coletaContainerHistorico.ColetaContainer.Codigo == codigoColetaContainer);

            return consultaColetaContainerHistorico.ToList();
        }

        public virtual List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> BuscarPorContainer(int codigoContainer)
        {
            var consultaColetaContainerHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>()
                .Where(coletaContainerHistorico => coletaContainerHistorico.ColetaContainer.Container.Codigo == codigoContainer);

            return consultaColetaContainerHistorico.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico BuscarUltimoHistoricoCargaAnteriorPorColetaContainer(int codigoColetaContainer, int codigoCargaAtual)
        {
            var consultaColetaContainerHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>()
                .Where(coletaContainerHistorico => coletaContainerHistorico.ColetaContainer.Codigo == codigoColetaContainer && coletaContainerHistorico.Carga.Codigo != codigoCargaAtual);

            return consultaColetaContainerHistorico
                .OrderByDescending(o => o.DataHistorico)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico BuscarUltimoHistoricoPorColetaContainer(int codigoColetaContainer)
        {
            var consultaColetaContainerHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>()
                .Where(coletaContainerHistorico => coletaContainerHistorico.ColetaContainer.Codigo == codigoColetaContainer);

            return consultaColetaContainerHistorico
                .OrderByDescending(o => o.DataHistorico)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico BuscarHistoricoColetaContainerPorCodigoCarga(int codigoCarga)
		{
            var consultaColetaContainerHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico>()
                .Where(coletaContainerHistorico => coletaContainerHistorico.Carga.Codigo == codigoCarga);

            return consultaColetaContainerHistorico.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
