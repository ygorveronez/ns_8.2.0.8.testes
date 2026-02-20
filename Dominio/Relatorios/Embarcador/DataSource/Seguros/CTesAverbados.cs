using System;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Seguros
{
    public class CTesAverbados
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public string Serie { get; set; }
        private DateTime DataEmissao { get; set; }
        public string InicioPrestacao { get; set; }
        public string TerminoPrestacao { get; set; }
        public string Carga { get; set; }
        public string TipoCarga { get; set; }
        public string Veiculo { get; set; }
        public string NumeroAverbacao { get; set; }
        public string Transportador { get; set; }
        public string Seguradora { get; set; }
        public string Apolice { get; set; }
        private DateTime DataAverbacao { get; set; }
        public string TipoOperacao { get; set; }
        public string MensagemRetorno { get; set; }
        private Dominio.Enumeradores.StatusAverbacaoCTe StatusAverbacaoCTe { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal ValorCTe { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal Desconto { get; set; }
        private string StatusCTe { get; set; }
        private SituacaoAverbacaoFechamento StatusFechamento { get; set; }
        private SeguradoraAverbacao TipoSeguradoraAverbacao { get; set; }
        private string CNPJTransportador { get; set; }
        public string Tomador { get; set; }
        public Dominio.Enumeradores.TipoPessoa TipoTomador { get; set; }
        private string CPFCNPJTomador { get; set; }
        public string Container { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        private FormaAverbacaoCTE FormaAverbacao { get; set; }
        public string Cliente { get; set; }
        public string ClienteProvedorOS { get; set; }
        public string ModeloDocumentoFiscal { get; set; }
        private DateTime DataServico { get; set; }
        public string CNPJFilial { get; set; }
        public string Filial { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy hh:mm") : string.Empty; }
        }

        public string DataAverbacaoFormatada
        {
            get { return DataAverbacao != DateTime.MinValue ? DataAverbacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoStatusCTe
        {
            get
            {
                switch (StatusCTe)
                {
                    case "P": return "Pendente";
                    case "E": return "Enviado";
                    case "R": return "Rejeição";
                    case "A": return "Autorizado";
                    case "C": return "Cancelado";
                    case "I": return "Inutilizado";
                    case "D": return "Denegado";
                    case "S": return "Em Digitação";
                    case "K": return "Em Cancelamento";
                    case "L": return "Em Inutilização";
                    case "Z": return "Anulado";
                    case "X": return "Aguardando Assinatura";
                    case "V": return "Aguardando Assinatura Cancelamento";
                    case "B": return "Aguardando Assinatura Inutilização";
                    case "M": return "Aguardando Emissão e-mail";
                    case "F": return "Contingência FSDA";
                    case "Q": return "Contingência EPEC";
                    case "Y": return "Aguardando Finalizar Carga Integração";
                    default:
                        return string.Empty;
                }
            }
        }

        public string StatusFechamentoFormatada
        {
            get { return StatusFechamento.ObterDescricao(); }
        }

        public string StatusAverbacaoCTeFormatada
        {
            get { return Dominio.Enumeradores.StatusAverbacaoCTeHelper.ObterDescricao(StatusAverbacaoCTe); }
        }

        public string TipoSeguradoraAverbacaoFormatada
        {
            get { return TipoSeguradoraAverbacao.Descricao(); }
        }

        public string CNPJTransportadorFormatada
        {
            get { return !string.IsNullOrEmpty(CNPJTransportador) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJTransportador)) : string.Empty; }
        }

        public string CPFCNPJTomadorFormatada
        {
            get { return CPFCNPJTomador.ObterCpfOuCnpjFormatado(); }
        }

        public string FormaAverbacaoFormatada
        {
            get { return FormaAverbacaoCTEHelper.ObterDescricao(FormaAverbacao); }
        }

        public string DataServicoFormatada
        {
            get { return DataServico != DateTime.MinValue ? DataServico.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
