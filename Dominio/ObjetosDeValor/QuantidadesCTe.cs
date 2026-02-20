namespace Dominio.ObjetosDeValor
{
    public class QuantidadesCTe
    {
        public string Status { get; set; }
        public int Quantidade { get; set; }
        public string DescricaoStatus
        {
            get
            {
                switch (Status)
                {
                    case "P":
                        return "Pendente";
                    case "E":
                        return "Enviado";
                    case "R":
                        return "Rejeição";
                    case "A":
                        return "Autorizado";
                    case "C":
                        return "Cancelado";
                    case "I":
                        return "Inutilizado";
                    case "D":
                        return "Denegado";
                    case "S":
                        return "Em Digitação";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    default:
                        return string.Empty;
                }
            }
        }
        public string Cor
        {
            get
            {
                switch (Status)
                {
                    case "P":
                        return "#3366CC";
                    case "E":
                        return "#0099c6";
                    case "R":
                        return "#990099";
                    case "A":
                        return "#329262";
                    case "C":
                        return "#DC3912";
                    case "I":
                        return "#FF9900";
                    case "D":
                        return "#000000";
                    case "S":
                        return "#BDCFEF";
                    case "K":
                        return "#F27C5F";
                    case "L":
                        return "#FFC875";
                    default:
                        return string.Empty;
                }
            }
        }


    }
}
