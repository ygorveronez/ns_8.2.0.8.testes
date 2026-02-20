using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Preferencias
{
    public class PreferenciaGrid : RepositorioBase<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid>
    {
        #region Construtores públicos

        public PreferenciaGrid(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos públicos

        public Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid BuscarPorUsuarioUrlGrid(int codigUsuario, string url, string grid)
        {
            var consultaPreferenciaGrid = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid>()
                .Where(o => o.Usuario.Codigo == codigUsuario && o.URL == url && o.Grid == grid);

            return consultaPreferenciaGrid.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid> BuscarModeloPorUrlGrid(string url, string grid)
        {
            var consultaPreferenciaGrid = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid>()
                .Where(o => o.URL == url && o.Grid == grid && o.ModeloGrid != null && o.Usuario == null);

            return consultaPreferenciaGrid.ToList();
        }

        public Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid BuscarModeloPadraoPorUrlGrid(string url, string grid)
		{
            var consultaPreferenciaGrid = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid>()
                .Where(o => o.URL == url && o.Grid == grid && o.ModeloGrid != null && o.Usuario == null && o.ModeloGrid.ModeloPadrao);

            return consultaPreferenciaGrid.FirstOrDefault();
        }

        #endregion
    }
}
