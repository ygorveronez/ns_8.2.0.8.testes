namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum TipoServicoMultisoftware
    {
        MultiEmbarcador = 1,
        MultiTMS = 2,
        MultiCTe = 3,
        CallCenter = 4,
        Terceiros = 5,
        MultiNFe = 6,
        MultiNFeAdmin = 7,
        Fornecedor = 8,
        MultiMobile = 9,
        MultiBus = 10,
        MultiBusTransportador = 11,
        TransportadorTerceiro = 12,

    }

    public static class TipoServicoMultisoftwareHelper
    {
        public static string ObterDescricao(this TipoServicoMultisoftware tipoServico)
        {
            switch (tipoServico)
            {
                case TipoServicoMultisoftware.MultiEmbarcador: return "Multi Embarcador";
                case TipoServicoMultisoftware.MultiTMS: return "Multi TMS";
                case TipoServicoMultisoftware.MultiCTe: return "Multi CT-e";
                case TipoServicoMultisoftware.CallCenter: return "Call Center";
                case TipoServicoMultisoftware.Terceiros: return "Terceiros";
                case TipoServicoMultisoftware.MultiNFe: return "Multi NF-e";
                case TipoServicoMultisoftware.MultiNFeAdmin: return "Multi NF-e Admin";
                case TipoServicoMultisoftware.Fornecedor: return "Fornecedor";
                case TipoServicoMultisoftware.MultiMobile: return "Multi Mobile";
                case TipoServicoMultisoftware.MultiBus: return "MultiBus";
                case TipoServicoMultisoftware.MultiBusTransportador: return "MultiBus Transportador";
                case TipoServicoMultisoftware.TransportadorTerceiro: return "Transportador Terceiro";
                default: return string.Empty;
            }
        }
    }
}
