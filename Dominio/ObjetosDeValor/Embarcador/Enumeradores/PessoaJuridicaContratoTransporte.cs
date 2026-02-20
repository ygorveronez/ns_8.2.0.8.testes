namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PessoaJuridicaContratoTransporte
    {
        ULBRUnileverBrasilLtda = 0,
        ULBRUnileverBrasilIndustrialLtda = 1,
        ULBRUnileverBrasilGeladosLtda = 2,
    }

    public static class PessoaJuridicaContratoTransporteHelper
    {
        public static string ObterDescricao(this PessoaJuridicaContratoTransporte pessoaJuridica)
        {
            switch (pessoaJuridica)
            {
                case PessoaJuridicaContratoTransporte.ULBRUnileverBrasilLtda: return "ULBR : Unilever Brasil Ltda";
                case PessoaJuridicaContratoTransporte.ULBRUnileverBrasilIndustrialLtda: return "ULBR : Unilever Brasil Industrial Ltda";
                case PessoaJuridicaContratoTransporte.ULBRUnileverBrasilGeladosLtda: return "ULBR : Unilever Brasil Gelados Ltda";
                default: return string.Empty;
            }
        }
    }
}