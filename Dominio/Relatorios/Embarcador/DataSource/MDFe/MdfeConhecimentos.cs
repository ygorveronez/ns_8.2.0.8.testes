using System;

namespace Dominio.Relatorios.Embarcador.DataSource.MDFe
{
    public class MdfeConhecimentos
    {
        public int CodigoMDFe { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime DataHoraEmissao { get; set; }
        public string Chave { get; set; }
        public string Status { get; set; }
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
                    case "Z":
                        return "Anulado";
                    case "X":
                        return "Aguardando Assinatura";
                    case "V":
                        return "Aguardando Assinatura Cancelamento";
                    case "B":
                        return "Aguardando Assinatura Inutilização";
                    case "M":
                        return "Aguardando Emissão e-mail";
                    case "F":
                        return "Contingência FSDA";
                    case "Q":
                        return "Contingência EPEC";
                    case "Y":
                        return "Aguardando Finalizar Carga Integração";
                    default:
                        return string.Empty;
                }
            }
        }
        public decimal ValorReceber { get; set; }
    }
}
