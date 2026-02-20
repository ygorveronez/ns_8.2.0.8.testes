namespace Servicos.Embarcador.Veiculo
{
    public class Macro
    {
        public static void ProcessarMacroRecebida(Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo macroVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            new GestaoPatio.RastreamentoCarga(unitOfWork).ProcessarMacroRecebida(macroVeiculo);
            new GestaoPatio.FluxoGestaoPatio(unitOfWork).ProcessarMacroRecebida(macroVeiculo, tipoServicoMultisoftware);
        }
    }
}
