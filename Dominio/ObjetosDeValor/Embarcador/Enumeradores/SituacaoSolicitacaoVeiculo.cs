namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSolicitacaoVeiculo
    {
        AguardandoSolicitacaoVeiculo = 1,
        VeiculoSolicitado = 2
    }

    public static class SituacaoSolicitacaoVeiculoHelper
    {
        public static string ObterDescricao(this SituacaoSolicitacaoVeiculo situacao)
        {
            switch (situacao)
            {
                case SituacaoSolicitacaoVeiculo.AguardandoSolicitacaoVeiculo: return "Aguardando Solicitação do Veículo";
                case SituacaoSolicitacaoVeiculo.VeiculoSolicitado: return "Veículo Solicitado";
                default: return string.Empty;
            }
        }
    }
}
