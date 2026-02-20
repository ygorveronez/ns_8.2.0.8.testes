namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServicoMultimodal
    {
        Nenhum = 0,
        Normal = 1,
        Subcontratacao = 2,
        RedespachoIntermediario = 3,
        VinculadoMultimodalTerceiro = 4,
        VinculadoMultimodalProprio = 5,
        Redespacho = 6
    }

    public static class TipoServicoMultimodalHelper
    {
        public static string ObterDescricao(this TipoServicoMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoServicoMultimodal.Normal: return "1 - Normal";
                case TipoServicoMultimodal.Subcontratacao: return "2 - Subcontratação";
                case TipoServicoMultimodal.RedespachoIntermediario: return "3 - Redespacho Intermediário";
                case TipoServicoMultimodal.VinculadoMultimodalTerceiro: return "4 - Vinculado Multimodal Terceiro";
                case TipoServicoMultimodal.VinculadoMultimodalProprio: return "5 - Vinculado Multimodal Próprio";
                case TipoServicoMultimodal.Redespacho: return "6 - Redespacho";
                default: return string.Empty;
            }
        }
    }
}
