namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCobrancaVendaDireta
    {
        NaoInformado = -1,
        Todos = 0,
        Boleto = 1,
        Cartao = 2,
        Avista = 3,
        PIX = 4,
        Bonificado = 5,
        MaqCartao = 6,
        Site = 7,
        Confirmar = 8
    }

    public static class TipoCobrancaVendaDiretaHelper
    {
        public static string ObterDescricao(this TipoCobrancaVendaDireta tipoCobranca)
        {
            switch (tipoCobranca)
            {
                case TipoCobrancaVendaDireta.NaoInformado: return "Não informado";
                case TipoCobrancaVendaDireta.Boleto: return "Boleto";
                case TipoCobrancaVendaDireta.Cartao: return "Cartão";
                case TipoCobrancaVendaDireta.Avista: return "À vista";
                case TipoCobrancaVendaDireta.PIX: return "PIX";
                case TipoCobrancaVendaDireta.Bonificado: return "Bonificado";
                case TipoCobrancaVendaDireta.MaqCartao: return "Maq de Cartão";
                case TipoCobrancaVendaDireta.Site: return "Site";
                case TipoCobrancaVendaDireta.Confirmar: return "Confirmar";
                default: return string.Empty;
            }
        }
    }
}

