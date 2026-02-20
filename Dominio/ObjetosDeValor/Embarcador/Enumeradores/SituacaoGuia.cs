namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGuia
    {
        NaoEmitido = 0,
        AguardandoEnvio = 1,
        AguardandoRetorno = 2,
        Gerada = 3,
        Cancelada = 4
    }

    public static class SituacaoGuiaHelper
    {
        public static string ObterDescricao(this SituacaoGuia outros)
        {
            switch (outros)
            {
                case SituacaoGuia.AguardandoEnvio:
                    return "Aguardando Envio";
                case SituacaoGuia.AguardandoRetorno:
                    return "Aguardando Retorno";
                case SituacaoGuia.Gerada:
                    return "Gerada";
                case SituacaoGuia.Cancelada:
                    return "Cancelada";
                default:
                    return "";
            }
        }
    }
}
