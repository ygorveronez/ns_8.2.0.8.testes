namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusEXP
    {
        NaoDefinido = 0,
        AguardandoDistrib = 1,
        PEGerado = 2,
        EXPCancelada = 3,
        PEParcialGerado = 4,
        EXPEXPParcialDistrib = 5,
        Distrib = 6,
        PVGerado = 7
    }

    public static class StatusEXPHelper
    {
        public static string ObterDescricao(this StatusEXP tipo)
        {
            switch (tipo)
            {
                case StatusEXP.NaoDefinido: return "Não Definido";
                case StatusEXP.AguardandoDistrib: return "Aguardando Distribuição";
                case StatusEXP.PEGerado: return "PE Gerado";
                case StatusEXP.EXPCancelada: return "EXP Cancelada";
                case StatusEXP.PEParcialGerado: return "PE Parcial Gerado";
                case StatusEXP.EXPEXPParcialDistrib: return "EXP Parcial Distribuida";
                case StatusEXP.Distrib: return "Distribuida";
                case StatusEXP.PVGerado: return "PV Gerado";
                default: return string.Empty;
            }
        }
    }
}
