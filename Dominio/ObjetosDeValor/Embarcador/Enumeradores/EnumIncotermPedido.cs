namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumIncotermPedido
    {
        EXW = 1,
        FCA = 2,
        FAS = 3,
        FOB = 4,
        CFR = 5,
        CIF = 6,
        CPT = 7,
        CIP = 8,
        DAP = 9,
        DPU = 10,
        DDP = 11,
        OCV = 12, 
        C_F = 13,
        C_I = 14, 
    }
    public static class EnumIncotermPedidoHelper
    {
        public static string ObterDescricao(this EnumIncotermPedido tipo)
        {
            switch (tipo)
            {
                case EnumIncotermPedido.EXW: return "EXW";
                case EnumIncotermPedido.FCA: return "FCA";
                case EnumIncotermPedido.FAS: return "FAS";
                case EnumIncotermPedido.FOB: return "FOB";
                case EnumIncotermPedido.CFR: return "CFR";
                case EnumIncotermPedido.CIF: return "CIF";
                case EnumIncotermPedido.CPT: return "CPT";
                case EnumIncotermPedido.CIP: return "CIP";
                case EnumIncotermPedido.DAP: return "DAP";
                case EnumIncotermPedido.DPU: return "DPU";
                case EnumIncotermPedido.DDP: return "DDP";
                case EnumIncotermPedido.OCV: return "OCV";
                case EnumIncotermPedido.C_F: return "C+F";
                case EnumIncotermPedido.C_I: return "C+S";
                default: return "";
            }
        }
    }
}
