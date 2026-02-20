namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCnpjConciliacaoTransportador
    {
        RaizCNPJ = 1,
        CNPJCompleto = 2,
    }

    public static class TipoCnpjConciliacaoTransportadorHelper
    {
        public static string ObterDescricao(this TipoCnpjConciliacaoTransportador tipoCnpj)
        {
            switch (tipoCnpj)
            {
                // Bimestral
                case TipoCnpjConciliacaoTransportador.RaizCNPJ: return "A raiz do CNPJ do transportador";
                case TipoCnpjConciliacaoTransportador.CNPJCompleto: return "O CNPJ completo do transportador";

                default: return string.Empty;
            }
        }
    }
}
