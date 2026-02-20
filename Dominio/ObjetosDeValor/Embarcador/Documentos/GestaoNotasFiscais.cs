using System;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class GestaoNotasFiscais
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public int NumeroCTe { get; set; }
        public string DescricaoNumeroCTe
        {
            get
            {
                return CodigoCTe > 0 ? NumeroCTe.ToString() : string.Empty;
            }
        }
        public decimal ValorCTe { get; set; }
        public string DescricaoValorCTe
        {
            get
            {
                return CodigoCTe > 0 ? ValorCTe.ToString("n2") : string.Empty;
            }
        }
        public string Produto { get; set; }
        public string SituacaoCTe { get; set; }
        public string DescricaoSituacaoCTe
        {
            get
            {
                switch (SituacaoCTe)
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
        public int NumeroNFe { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public string ChaveNFe { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string TipoEmitente { get; set; }
        public double CPFCNPJEmitente { get; set; }
        public string CPFCNPJEmitenteFormatado
        {
            get
            {
                if (TipoEmitente == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return TipoEmitente == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJEmitente) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJEmitente);
                }
            }
        }
        public string NomeEmitente { get; set; }
        public string ClienteOrigem { get; set; }
        public string CidadeOrigem { get; set; }
        public string UFOrigem { get; set; }
        public string ClienteDestino { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public decimal Peso { get; set; }
        public decimal Valor { get; set; }
        public string DT_RowColor
        {
            get
            {
                return CodigoCTe > 0 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : string.Empty;
            }
        }
    }
}
