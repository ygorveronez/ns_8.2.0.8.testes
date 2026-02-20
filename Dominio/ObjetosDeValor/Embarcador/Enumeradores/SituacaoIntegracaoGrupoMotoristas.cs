namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoGrupoMotoristas
    {
        AguardandoIntegracoes = 0,
        FalhaNasIntegracoes = 1,
        Finalizado = 2
    }

    public static class SituacaoIntegracaoGrupoMotoristasHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoGrupoMotoristas situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracaoGrupoMotoristas.AguardandoIntegracoes:
                    return "Aguardando Integrações";
                case SituacaoIntegracaoGrupoMotoristas.FalhaNasIntegracoes:
                    return "Falha nas Integrações";
                case SituacaoIntegracaoGrupoMotoristas.Finalizado:
                    return "Finalizado";
                default:
                    return string.Empty;
            }
        }
    }
}