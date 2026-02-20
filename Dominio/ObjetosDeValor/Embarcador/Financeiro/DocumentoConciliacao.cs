using System;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoConciliacao
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Transportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string Status { get; set; }
        public Dominio.Enumeradores.TipoDocumento TipoDocumentoEmissao { get; set; }
        public string NumeroCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public string Filial { get; set; }
        public string NumeroFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataLiquidacao { get; set; }
        public DateTime DataConsulta { get; set; }
        public decimal ValorAReceber { get; set; }
        public decimal ValorDecrescimo { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorLiquidacao { get; set; }
        public string Acrescimos { get; set; }
        public string Decrescimos { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public int Serie { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoSituacao
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string DescricaoDataEmissao
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataVencimento
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataLiquidacao
        {
            get { return DataLiquidacao != DateTime.MinValue ? DataLiquidacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataConsulta
        {
            get { return DataConsulta != DateTime.MinValue ? DataConsulta.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }

        public string DescricaoTipoDocumento
        {
            get { return TipoDocumentoEmissao.ObterDescricao(); }
        }


        public string ValorOriginalFormatado
        {
            get { return ValorAReceber.ToString("n2"); }
        }

        public string ValorAReceberFormatado
        {
            get { return (ValorAReceber + ValorAcrescimo - ValorDecrescimo).ToString("n2"); }
        }

        public string ValorLiquidacaoFormatado
        {
            get { return (ValorLiquidacao).ToString("n2"); }
        }


        public string StatusCarga
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public string CNPJTransportador_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJTransportador));
            }
        }

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
                    case "Z":
                        return "Anulado Gerencialmente";
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

        #endregion
    }
}
