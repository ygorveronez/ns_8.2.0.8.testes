namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OperadoraCIOT
    {
        eFrete = 1,
        Repom = 2,
        Pamcard = 3,
        Pagbem = 4,
        Target = 5,
        Extratta = 6,
        BBC = 7,
        Ambipar = 8,
        Rodocred = 9,
        RepomFrete = 10,
        TruckPad = 11
    }

    public static class OperadoraCIOTHelper
    {
        public static string ObterDescricao(this OperadoraCIOT operadoraCIOT)
        {
            switch (operadoraCIOT)
            {
                case OperadoraCIOT.eFrete: return "e-Frete";
                case OperadoraCIOT.Repom: return "Repom";
                case OperadoraCIOT.Pamcard: return "Pamcard";
                case OperadoraCIOT.Pagbem: return "Pagbem";
                case OperadoraCIOT.Target: return "Target";
                case OperadoraCIOT.Extratta: return "Extratta";
                case OperadoraCIOT.BBC: return "BBC";
                case OperadoraCIOT.Ambipar: return "Ambipar";
                case OperadoraCIOT.Rodocred: return "Rodocred";
                case OperadoraCIOT.RepomFrete: return "Repom Frete";
                case OperadoraCIOT.TruckPad: return "TruckPad";
                default: return "";
            }
        }
    }
}