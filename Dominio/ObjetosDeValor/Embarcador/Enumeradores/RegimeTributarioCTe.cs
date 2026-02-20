namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RegimeTributarioCTe
    {
        SimplesNacional = 1,
        SimplesNacionalExcessoReceita = 2,
        RegimeNormal = 3,
        SimplesNacionalMEI = 4
    }

    public static class CodigoRegimeTributarioCTeHelper
    {
        public static string ObterDescricao(this RegimeTributarioCTe regime)
        {
            return regime switch
            {
                RegimeTributarioCTe.SimplesNacional => "Simples Nacional",
                RegimeTributarioCTe.SimplesNacionalExcessoReceita => "Simples Nacional, excesso sublimite de receita bruta",
                RegimeTributarioCTe.RegimeNormal => "Regime Normal",
                RegimeTributarioCTe.SimplesNacionalMEI => "Simples Nacional, Microempreendedor Individual MEI",
                _ => string.Empty,
            };
        }
    }
}
