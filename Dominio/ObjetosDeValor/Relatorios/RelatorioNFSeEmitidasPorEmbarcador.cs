using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioNFSeEmitidasPorEmbarcador
    {
        public int Codigo { get; set; }

        public string CNPJTransportador { get; set; }

        public string Transportador { get; set; }

        public string NaturezaOperacao { get; set; }

        public int RPSNumero { get; set; }

        public string RPSSerie { get; set; }

        public string RPSProtocolo { get; set; }

        public DateTime? RPSData { get; set; }

        public string RPSRetornoCodigo { get; set; }

        public string RPSRetornoMensagem { get; set; }

        public string RPSProtocoloCancelamento { get; set; }

        public DateTime? RPSDataCancelamento { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public int NumeroSubstituicao { get; set; }

        public string SerieSubstituicao { get; set; }

        public string Log { get; set; }
        
        public DateTime? DataIntegracao { get; set; }

        public DateTime? DataEmissao { get; set; }
        
        public decimal ValorServicos { get; set; }

        public decimal ValorDeducoes { get; set; }

        public decimal ValorPIS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorIR { get; set; }

        public decimal ValorCSLL { get; set; }

        public decimal ValorISSRetido { get; set; }

        public decimal ValorOutrasRetencoes { get; set; }

        public decimal ValorDescIncondicionado { get; set; }

        public decimal ValorDescCondicionado { get; set; }

        public decimal AliquotaISS { get; set; }

        public decimal BaseCalculoISS { get; set; }

        public decimal ValorISS { get; set; }

        public string OutrasInformacoes { get; set; }

        public string CPFCNPJTomador { get; set; }

        public string NomeTomador { get; set; }

        public string ItemDescricao { get; set; }

        public string ItemNumero { get; set; }

        public string ItemDiscriminacao { get; set; }

        public decimal ItemQuantidade { get; set; }

        public string ItemMunicipio { get; set; }

        public string ItemMunicipioIncidencia { get; set; }

        public Dominio.Enumeradores.StatusNFSe Status { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusNFSe.Autorizado:
                        return "Autorizado";
                    case Enumeradores.StatusNFSe.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusNFSe.EmCancelamento:
                        return "Em Cancelamento";
                    case Enumeradores.StatusNFSe.EmDigitacao:
                        return "Em Digitação";
                    case Enumeradores.StatusNFSe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusNFSe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusNFSe.Rejeicao:
                        return "Rejeição";
                    default:
                        return string.Empty;
                }
            }
        }

        public string DescricaoDataEmissao
        {
            get
            {
                return this.DataEmissao.HasValue ? this.DataEmissao.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

        public string DescricaoDataIntegracao
        {
            get
            {
                return this.DataIntegracao.HasValue ? this.DataIntegracao.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

        public string DescricaoDataRPS
        {
            get
            {
                return this.RPSData.HasValue ? this.RPSData.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }


        public string DescricaoDataRPSCancelamento
        {
            get
            {
                return this.RPSDataCancelamento.HasValue ? this.RPSDataCancelamento.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

    }
}
