namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOcorrenciaInfracao
    {
        Veiculo = 0,
        Funcionario = 1,
        Motorista = 2
    }

    public static class TipoOcorrenciaInfracaoHelper
    {
        public static string ObterDescricao(this TipoOcorrenciaInfracao tipoOcorrenciaInfracao)
        {
            switch (tipoOcorrenciaInfracao)
            {
                case TipoOcorrenciaInfracao.Veiculo:
                    return "Veículo";
                case TipoOcorrenciaInfracao.Funcionario:
                    return "Funcionário";
                case TipoOcorrenciaInfracao.Motorista:
                    return "Motorista";
                default:
                    return string.Empty;
            }
        }
    }
}
