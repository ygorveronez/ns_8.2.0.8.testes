namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RegraOfertaCarga
    {
        ValorFrete = 0,
        Share = 1,
        NivelServico = 2,
    }

    public static class RegraOfertaCargaHelper
    {
        public static string ObterDescricao(this RegraOfertaCarga regraOfertaCarga)
        {
            switch (regraOfertaCarga)
            {
                case RegraOfertaCarga.ValorFrete: return "Valor de frete";
                case RegraOfertaCarga.Share: return "Valor share";
                case RegraOfertaCarga.NivelServico: return "Nível de serviço";
                default: return string.Empty;
            }
        }
    }
}
