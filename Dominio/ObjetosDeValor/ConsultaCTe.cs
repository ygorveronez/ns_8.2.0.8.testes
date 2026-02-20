using System;

namespace Dominio.ObjetosDeValor
{
    public class ConsultaCTe
    {
        public int Codigo { get; set; }
        public string Status { get; set; }
        public int Numero { get; set; }
        public int Empresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int Serie { get; set; }
        public string Documento { get; set; }
        public string Placa { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        public Entidades.ParticipanteCTe Remetente { get; set; }
        public Entidades.ParticipanteCTe Destinatario { get; set; }
        public Entidades.ParticipanteCTe Tomador { get; set; }
        public decimal Valor { get; set; }
        public Entidades.ErroSefaz MensagemStatus { get; set; }
        public string MensagemRetornoSefaz { get; set; }
        public string TerminoPrestacao { get; set; }
        public string StatusIntegracao { get; set; }

        public virtual string DescricaoStatus
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
                        return "Aguardando Finalizar carga Integração";
                    case "N":
                        return "Aguardando NFSe";
                    case "Z":
                        return "Anulado Gerencialmente";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipoServico
        {
            get
            {
                switch (TipoServico)
                {
                    case Enumeradores.TipoServico.Normal:
                        return "Normal";
                    case Enumeradores.TipoServico.Redespacho:
                        return "Redespacho";
                    case Enumeradores.TipoServico.RedIntermediario:
                        return "Red. Intermediário";
                    case Enumeradores.TipoServico.SubContratacao:
                        return "Subcontratação";
                    case Enumeradores.TipoServico.ServVinculadoMultimodal:
                        return "Serv. Vinculado Multimodal";
                    case Enumeradores.TipoServico.TransporteDePessoas:
                        return "Transporte de Pessoas";
                    case Enumeradores.TipoServico.TransporteDeValores:
                        return "Transporte de Valores";
                    case Enumeradores.TipoServico.ExcessoDeBagagem:
                        return "Excesso de Bagagem";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoCTE
        {
            get
            {
                switch (TipoCTe)
                {
                    case Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Enumeradores.TipoCTE.Complemento:
                        return "Complementar";
                    case Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Enumeradores.TipoCTE.Substituto:
                        return "Substituição";
                    default:
                        return "";
                }
            }
        }
    }
}
