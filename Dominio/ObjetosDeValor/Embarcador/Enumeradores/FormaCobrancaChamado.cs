namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaCobrancaChamado
    {
        Caixa = 1,
        Chapa = 2,
        Pallet = 3,
        Peso = 4,
        ValorFixo = 5
    }

    public static class FormaCobrancaChamadoHelper
    {
        public static string ObterDescricao(this FormaCobrancaChamado formaTitulo)
        {
            switch (formaTitulo)
            {
                case FormaCobrancaChamado.Caixa: return "Caixas (UN)";
                case FormaCobrancaChamado.Chapa: return "Chapas (UN)";
                case FormaCobrancaChamado.Pallet: return "Pallets (UN)";
                case FormaCobrancaChamado.Peso: return "Peso (TON)";
                case FormaCobrancaChamado.ValorFixo: return "Valor Fixo";
                default: return string.Empty;
            }
        }
    }
}
