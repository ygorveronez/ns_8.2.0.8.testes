namespace Servicos.Embarcador.Abastecimento.Interfaces
{
    public interface IFechamentoAbastecimento
    {

        bool GerarFechamentoAbastecimento(Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento, Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento, ref string mensagemErro);
        bool FecharAbastecimento(Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento, ref string mensagemErro);

    }
}
