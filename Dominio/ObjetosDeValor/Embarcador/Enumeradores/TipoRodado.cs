namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRodado
    {
        NaoAplicavel = 0,
        Truck = 1,
        Toco = 2,
        CavaloMecanico = 3,
        VAN = 4,
        Utilitario = 5,
        Outros = 6
    }

    public static class TipoRodadoHelper
    {
        public static string ObterDescricao(this TipoRodado situacao)
        {
            switch (situacao)
            {
                case TipoRodado.NaoAplicavel: return "Não Aplicável";
                case TipoRodado.Truck: return "Truck";
                case TipoRodado.Toco: return "Toco";
                case TipoRodado.CavaloMecanico: return "Cavalo Mecânico";
                case TipoRodado.VAN: return "VAN";
                case TipoRodado.Utilitario: return "Utilitário";
                case TipoRodado.Outros: return "Outros";
                default: return null;
            }
        }
    }
}
