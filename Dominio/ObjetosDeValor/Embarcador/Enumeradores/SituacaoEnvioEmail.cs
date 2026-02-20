namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEnvioEmail
    {
        AguardandoEnvio = 1,
        Enviado = 2
    }

    public static class SituacaoEnvioEmailHelper
    {
        public static string ObterDescricao(this SituacaoEnvioEmail situacao)
        {
            switch (situacao)
            {
                case SituacaoEnvioEmail.AguardandoEnvio: return "Aguardando Envio";
                case SituacaoEnvioEmail.Enviado: return "Enviado";
                default: return string.Empty;
            }
        }
    }
}
