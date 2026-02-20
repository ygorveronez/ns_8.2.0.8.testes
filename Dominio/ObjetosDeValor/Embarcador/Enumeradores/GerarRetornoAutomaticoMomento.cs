namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GerarRetornoAutomaticoMomento
    {
        Nenhum = 0,
        ConfirmacaoViagem = 1,
        FinalizarEmissaoDocumentos = 2
    }

    public static class GerarRetornoAutomaticoMomentoHelper
    {
        public static string ObterDescricao(this GerarRetornoAutomaticoMomento momentoGerarCargaRetorno)
        {
            return momentoGerarCargaRetorno switch
            {
                GerarRetornoAutomaticoMomento.ConfirmacaoViagem => "Confirmar viagem",
                GerarRetornoAutomaticoMomento.FinalizarEmissaoDocumentos => "Finalizar emissão documentos",
                _ => string.Empty,
            };
        }
    }
}
