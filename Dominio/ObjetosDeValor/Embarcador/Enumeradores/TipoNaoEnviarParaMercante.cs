namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNaoEnviarParaMercante
    {
        InclusaoNFe = 1,
        AnulacaoValoresRecusa = 2,
        Outros = 9
    }

    public static class TipoNaoEnviarParaMercanteHelper
    {
        public static string ObterDescricao(this TipoNaoEnviarParaMercante tipoNaoEnviarParaMercante)
        {
            switch (tipoNaoEnviarParaMercante)
            {
                case TipoNaoEnviarParaMercante.InclusaoNFe: return "Inclusão de NF-e";
                case TipoNaoEnviarParaMercante.AnulacaoValoresRecusa: return "Anulação de valores / Recusa";
                case TipoNaoEnviarParaMercante.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
