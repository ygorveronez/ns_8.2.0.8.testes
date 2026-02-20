using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioMDFesEmitidosAverbacoes
    {
        public int CodigoMDFe { get; set; }

        public string Placa { get; set; }

        public string MotoristaNome { get; set; }

        public string MotoristaCPF { get; set; }

        public string NomeTransportadora { get; set; }

        public DateTime? DataEmissao { get; set; }

        public DateTime? DataAutorizacao { get; set; }

        public DateTime? DataEncerramento { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal PesoBrutoMercadoria { get; set; }

        public Dominio.Enumeradores.UnidadeMedidaMDFe UnidadeMedida { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string Chave { get; set; }        

        public string UFCarregamento { get; set; }

        public string UFDescarregamento { get; set; }

        public string DescricaoUFCarregamento { get; set; }

        public string DescricaoUFDescarregamento { get; set; }

        public Dominio.Enumeradores.StatusMDFe Status { get; set; }

        public Dominio.Enumeradores.TipoResponsavelSeguroMDFe ResponsavelSeguro { get; set; }

        public string CNPJSeguradora { get; set; }

        public string NomeSeguradora { get; set; }

        public string NumeroApolice { get; set; }

        public string NumeroAverbacao { get; set; }

        public string DescricaoUnidadeMedida
        {
            get
            {
                return this.UnidadeMedida.ToString("G");
            }
        }

        public string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusMDFe.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusMDFe.Autorizado:
                        return "Autorizado";
                    case Enumeradores.StatusMDFe.EmCancelamento:
                        return "Em Cancelamento";
                    case Enumeradores.StatusMDFe.EmDigitacao:
                        return "Em Digitação";
                    case Enumeradores.StatusMDFe.EmEncerramento:
                        return "Em Encerramento";
                    case Enumeradores.StatusMDFe.Encerrado:
                        return "Encerrado";
                    case Enumeradores.StatusMDFe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusMDFe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusMDFe.Rejeicao:
                        return "Rejeição";
                    default:
                        return "";
                }
            }
        }

        public string DescricaoResponsavelSeguro
        {
            get
            {
                switch (this.ResponsavelSeguro)
                {
                    case Enumeradores.TipoResponsavelSeguroMDFe.Contratante:
                        return "Contratante";
                    case Enumeradores.TipoResponsavelSeguroMDFe.Emitente:
                        return "Emitente";
                    default:
                        return "";
                }
            }
        }
    }
}

