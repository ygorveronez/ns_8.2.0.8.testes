namespace ReportApi.Models.Grid
{
    public class GridPreferencias
	{
		private Repositorio.UnitOfWork _unitOfWork;
		private string _urlGrid;
		private string _idGrid;

		#region Construtores


		public GridPreferencias(Repositorio.UnitOfWork unitOfWork, string urlGrid, string idGrid)
		{
			_unitOfWork = unitOfWork;
			_urlGrid = urlGrid;
			_idGrid = idGrid;
		}

		#endregion

		#region Métodos Públicos

		public Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid ObterPreferenciaGrid(int codigoUsuario, int codigoModelo)
		{
			Repositorio.Embarcador.Preferencias.PreferenciaGrid repositorioPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(_unitOfWork);

			// 1º - Se aplicou um modelo via tela
			if (codigoModelo > 0)
			{
				Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid modelo = repositorioPreferenciaGrid.BuscarPorCodigo(codigoModelo, false);

				if (modelo != null)
					return modelo;
			}

			// 2º - Se existe configuração de preferência pelo usuário
			Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaUsuario = repositorioPreferenciaGrid.BuscarPorUsuarioUrlGrid(codigoUsuario, _urlGrid, _idGrid);
			if (preferenciaUsuario != null)
				return preferenciaUsuario;

			// 3º - Se existe um modelo padrão para a tela
			Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid modeloPadrao = repositorioPreferenciaGrid.BuscarModeloPadraoPorUrlGrid(_urlGrid, _idGrid);
			if (modeloPadrao != null)
				return modeloPadrao;

			// 4º - Se não existe configuração irá carregar o padrão da tela (Controller)
			return null;
		}

		#endregion
	}
}