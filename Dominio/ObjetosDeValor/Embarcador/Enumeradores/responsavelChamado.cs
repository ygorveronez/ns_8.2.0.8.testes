namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ResponsavelChamado
    {
        Todos = 0,
        Backhall = 1,
        Comercial = 2,
        GA = 3,
        ADM = 4,
        CD = 5,
        Cliente = 6,
        Transportador = 7
    }

    public static class ResponsavelChamadoHelper
    {
        public static string ObterDescricao(this ResponsavelChamado responsavelChamado)
        {
            switch (responsavelChamado)
            {
                case ResponsavelChamado.Backhall: return "Backhaul";
                case ResponsavelChamado.Comercial: return "Comercial";
                case ResponsavelChamado.GA: return "GA";
                case ResponsavelChamado.ADM: return "ADM";
                case ResponsavelChamado.CD: return "CD";
                case ResponsavelChamado.Cliente: return "Cliente";
                case ResponsavelChamado.Transportador: return "Transportador";
                default: return string.Empty;
            }
        }
    }
}
