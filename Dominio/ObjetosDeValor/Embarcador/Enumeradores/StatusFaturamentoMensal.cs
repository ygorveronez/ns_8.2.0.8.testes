namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusFaturamentoMensal
    {
        Iniciada = 1,
        GeradoDocumentos = 2,
        DocumentosAutorizados = 3,
        GeradoBoletos = 4,
        Finalizado = 5,
        Cancelado = 6,
        AguardandoEnvioEmail = 7,
        AguardandoAutorizacaoDocumento = 8,
        EmGeracaoEnvioEmail = 9,
        EmGeracaoAutorizacaoDocumento = 10
    }
}
