using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/Veiculo", "Veiculos/VeiculoLicenca")]
    public class VeiculoLicencaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo, Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>
    {
		#region Construtores

		public VeiculoLicencaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}