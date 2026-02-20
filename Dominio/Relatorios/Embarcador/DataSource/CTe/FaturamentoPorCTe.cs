using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class FaturamentoPorCTe
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string StatusCTe { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroNotaFiscal { get; set; }
        private StatusTitulo StatusTitulo { get; set; }
        public int NumeroFatura { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataFatura { get; set; }
        private DateTime DataVencimentoBoleto { get; set; }
        public string DataVencimentoTitulo { get; set; }

        public string CPFCNPJRemetente { get; set; }
        public string Remetente { get; set; }

        public string CPFCNPJExpedidor { get; set; }
        public string Expedidor { get; set; }

        public string CPFCNPJRecebedor { get; set; }
        public string Recebedor { get; set; }

        public string CPFCNPJDestinatario { get; set; }
        public string Destinatario { get; set; }

        public string CPFCNPJTomador { get; set; }
        public string Tomador { get; set; }

        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorFrete { get; set; }
        public string GrupoTomador { get; set; }
        public string ChaveCTe { get; set; }
        public decimal ValorPrestacao { get; set; }
        private Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        private string SituacaoCarga { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public int QuantidadeNF { get; set; }
        public string NumeroLacre { get; set; }
        public decimal Tara { get; set; }
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public string NumeroBoleto { get; set; }
        private DateTime DataBoleto { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string NavioTransbordo { get; set; }

        private DateTime DataVencimentoFatura { get; set; }
        private DateTime DataEnvioEmailFaturaIntegracao { get; set; }
        private string EmailsFaturaIntegracao { get; set; }
        private string SituacaoEmailFaturaIntegracao { get; set; }
        public string FaturamentoAVista { get; set; }
        public string NumeroTitulo { get; set; }
        public string Viagem { get; set; }
        public decimal PesoNotaFiscal { get; set; }
        public string DiaSemana { get; set; }
        public string DiaMes { get; set; }
        private string TipoPrazoFaturamento { get; set; }
        public decimal ValorComponenteBAF { get; set; }
        public decimal ValorComponenteAdValorem { get; set; }
        public string TipoOperacao { get; set; }
        public string ETS { get; set; }
        public string CTeAnulado { get; set; }
        public string SomenteCTeSubstituido { get; set; }
        public string CNPJRemetente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string CNPJRecebedor { get; set; }
        public string TipoProposta { get; set; }
        public TipoServicoMultimodal TipoServico { get; set; }

        public string NumeroCTeSubstituto { get; set; }
        public string NumeroCTeAnulacao { get; set; }
        public string NumeroCTeComplementar { get; set; }
        public string NumeroCTeDuplicado { get; set; }
        public string NumeroCTeOriginal { get; set; }
        public string NumeroControleCTeSubstituto { get; set; }
        public string NumeroControleCTeAnulacao { get; set; }
        public string NumeroControleCTeComplementar { get; set; }
        public string NumeroControleCTeDuplicado { get; set; }
        public string NumeroControleCTeOriginal { get; set; }
        #endregion

        #region Propriedades com Regras
        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DescricaoTipoCTe
        {
            get { return TipoCTe.ObterDescricao(); }
        }
        public string DescricaoStatusTitulo
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string SituacaoCargaFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoCarga))
                    return SituacaoCarga;

                string[] listaSituacoes = SituacaoCarga.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoCargaHelper.ObterDescricao((SituacaoCarga)situacao.ToInt())));
            }
        }

        public string TipoPrazoFaturamentoFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoPrazoFaturamento))
                    return TipoPrazoFaturamento;

                string[] listaTiposPrazo = TipoPrazoFaturamento.Split(',');
                IEnumerable<string> lista = listaTiposPrazo.Distinct();

                return string.Join(", ", (from tipoPrazo in lista select TipoPrazoFaturamentoHelper.ObterDescricao((TipoPrazoFaturamento)tipoPrazo.ToInt())));
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFaturaFormatada
        {
            get { return DataFatura != DateTime.MinValue ? DataFatura.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoBoletoFormatada
        {
            get { return DataVencimentoBoleto != DateTime.MinValue ? DataVencimentoBoleto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataBoletoFormatada
        {
            get { return DataBoleto != DateTime.MinValue ? DataBoleto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoFaturaFormatada
        {
            get { return DataVencimentoFatura != DateTime.MinValue ? DataVencimentoFatura.ToString("dd/MM/yyyy") : string.Empty; }
        }

        private DateTime DataEnvioFatura
        {
            get { return string.IsNullOrWhiteSpace(NumeroBoleto) ? DataEnvioEmailFaturaIntegracao : DateTime.MinValue; }
        }

        private DateTime DataEnvioBoleto
        {
            get { return !string.IsNullOrWhiteSpace(NumeroBoleto) ? DataEnvioEmailFaturaIntegracao : DateTime.MinValue; }
        }

        public string DataEnvioFaturaFormatada
        {
            get { return DataEnvioFatura != DateTime.MinValue ? DataEnvioFatura.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEnvioBoletoFormatada
        {
            get { return DataEnvioBoleto != DateTime.MinValue ? DataEnvioBoleto.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoEmailFatura
        {
            get { return string.IsNullOrWhiteSpace(NumeroBoleto) ? SituacaoEmailFaturaIntegracao : string.Empty; }
        }

        public string SituacaoEmailBoleto
        {
            get { return !string.IsNullOrWhiteSpace(NumeroBoleto) ? SituacaoEmailFaturaIntegracao : string.Empty; }
        }

        public string EmailFatura
        {
            get { return string.IsNullOrWhiteSpace(NumeroBoleto) ? EmailsFaturaIntegracao : string.Empty; }
        }

        public string EmailBoleto
        {
            get { return !string.IsNullOrWhiteSpace(NumeroBoleto) ? EmailsFaturaIntegracao : string.Empty; }
        }

        public decimal PesoBrutoNotaFiscal
        {
            get { return PesoNotaFiscal + Tara; }
        }

        #endregion
    }
}
