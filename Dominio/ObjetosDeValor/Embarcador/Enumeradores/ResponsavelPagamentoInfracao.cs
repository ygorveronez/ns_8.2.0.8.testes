namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ResponsavelPagamentoInfracao
    {
        Condutor = 1,
        Empresa = 2,
        Outro = 3,
        Funcionario = 4,
        AgregadoTerceiro = 5
    }

    public static class ResponsavelPagamentoInfracaoHelper
    {
        public static string ObterDescricao(this ResponsavelPagamentoInfracao responsavelPagamentoInfracao)
        {
            switch (responsavelPagamentoInfracao)
            {
                case ResponsavelPagamentoInfracao.Condutor: return "Condutor";
                case ResponsavelPagamentoInfracao.Empresa: return "Empresa";
                case ResponsavelPagamentoInfracao.Outro: return "Outro";
                case ResponsavelPagamentoInfracao.Funcionario: return "Funcionário";
                case ResponsavelPagamentoInfracao.AgregadoTerceiro: return "Agregado/Terceiro";
                default: return string.Empty;
            }
        }
    }
}
