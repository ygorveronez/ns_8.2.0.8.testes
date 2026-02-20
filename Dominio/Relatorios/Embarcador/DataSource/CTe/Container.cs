using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class Container
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string ContainerDescricao { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string Viagem { get; set; }
        public string NavioTransbordo { get; set; }
        public string PortoOrigem { get; set; }
        public string TerminalOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string TerminalDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string TerminalTransbordo { get; set; }
        public decimal PesoBruto { get; set; }
        public string TipoContainer { get; set; }

        public string CPFCNPJExpedidor { get; set; }
        public string Expedidor { get; set; }

        public string CNPJRemetente { get; set; }
        public string Remetente { get; set; }

        public string CNPJDestinatario { get; set; }
        public string Destinatario { get; set; }

        public string CNPJRecebedor { get; set; }
        public string Recebedor { get; set; }

        public string CPFCNPJTomador { get; set; }
        public string Tomador { get; set; }

        public string NumeroLacre { get; set; }
        public decimal Tara { get; set; }
        public int NumeroCTe { get; set; }
        public string NumeroControle { get; set; }
        public string NumeroNota { get; set; }
        public int QuantidadeNota { get; set; }
        public string TipoOperacao { get; set; }
        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        public string CargaIMO { get; set; }
        public string TipoProposta { get; set; }
        public string NumeroProposta { get; set; }
        private string SituacaoCarga { get; set; }
        public string PossuiNotas { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorNotas { get; set; }
        public decimal ValorICMS { get; set; }

        public string StatusCTe { get; set; }
        public int SerieCTe { get; set; }
        public string NumeroCarga { get; set; }
        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataAutorizacao { get; set; }
        private DateTime DataOperacaoNavio { get; set; }
        public string DataVencimento { get; set; }
        public string DataEntrega { get; set; }
        public string CodigoInicioPrestacao { get; set; }
        public string CodigoFimPrestacao { get; set; }
        public int CFOP { get; set; }
        public string CST { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal ValorSemImposto { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorPrestacao { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public string DataColeta { get; set; }
        public string DataPrevistaEntrega { get; set; }
        public string CodigoPortoOrigem { get; set; }
        public string CodigoPortoDestino { get; set; }
        public string NumeroContainer { get; set; }
        public string CodigoNavio { get; set; }
        public string CNPJTransportador { get; set; }
        public string Transportador { get; set; }
        public decimal Taxa { get; set; }
        public string ETS { get; set; }
        public string ETA { get; set; }
        public string ETATransbordo1 { get; set; }
        public string ChaveCTe { get; set; }
        public string ChaveCTeMultimodal { get; set; }
        public string ChaveCTeSVM { get; set; }
        public string CTeAnulado { get; set; }
        public string SomenteCTeSubstituido { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroCEMercante { get; set; }
        public string Commodity { get; set; }
        private bool Afretamento { get; set; }
        public string NumeroProtocoloANTAQ { get; set; }
        public string FFE { get; set; }
        public string TEU { get; set; }
        public string NumeroManifestoFEEDER { get; set; }
        public string NumeroCEFEEDER { get; set; }
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
        public decimal ValorSemTributo { get; set; }
        public string Observacao { get; set; }
        public string BookingReferente { get; set; }
        public string Balsa { get; set; }

        #endregion

        #region Propriedades de Componentes

        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        public decimal ValorComponente16 { get; set; }
        public decimal ValorComponente17 { get; set; }
        public decimal ValorComponente18 { get; set; }
        public decimal ValorComponente19 { get; set; }
        public decimal ValorComponente20 { get; set; }
        public decimal ValorComponente21 { get; set; }
        public decimal ValorComponente22 { get; set; }
        public decimal ValorComponente23 { get; set; }
        public decimal ValorComponente24 { get; set; }
        public decimal ValorComponente25 { get; set; }
        public decimal ValorComponente26 { get; set; }
        public decimal ValorComponente27 { get; set; }
        public decimal ValorComponente28 { get; set; }
        public decimal ValorComponente29 { get; set; }
        public decimal ValorComponente30 { get; set; }
        public decimal ValorComponente31 { get; set; }
        public decimal ValorComponente32 { get; set; }
        public decimal ValorComponente33 { get; set; }
        public decimal ValorComponente34 { get; set; }
        public decimal ValorComponente35 { get; set; }
        public decimal ValorComponente36 { get; set; }
        public decimal ValorComponente37 { get; set; }
        public decimal ValorComponente38 { get; set; }
        public decimal ValorComponente39 { get; set; }
        public decimal ValorComponente40 { get; set; }
        public decimal ValorComponente41 { get; set; }
        public decimal ValorComponente42 { get; set; }
        public decimal ValorComponente43 { get; set; }
        public decimal ValorComponente44 { get; set; }
        public decimal ValorComponente45 { get; set; }
        public decimal ValorComponente46 { get; set; }
        public decimal ValorComponente47 { get; set; }
        public decimal ValorComponente48 { get; set; }
        public decimal ValorComponente49 { get; set; }
        public decimal ValorComponente50 { get; set; }
        public decimal ValorComponente51 { get; set; }
        public decimal ValorComponente52 { get; set; }
        public decimal ValorComponente53 { get; set; }
        public decimal ValorComponente54 { get; set; }
        public decimal ValorComponente55 { get; set; }
        public decimal ValorComponente56 { get; set; }
        public decimal ValorComponente57 { get; set; }
        public decimal ValorComponente58 { get; set; }
        public decimal ValorComponente59 { get; set; }
        public decimal ValorComponente60 { get; set; }

        #endregion

        #region Propriedades com Regras

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

        public string AfretamentoDescricao
        {
            get
            {
                return Afretamento ? "Sim" : "Não";
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAutorizacaoFormatada
        {
            get { return DataAutorizacao != DateTime.MinValue ? DataAutorizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoTipoCTe
        {
            get { return TipoCTe.ObterDescricao(); }
        }

        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DataOperacaoNavioFormatada
        {
            get { return DataOperacaoNavio != DateTime.MinValue ? DataOperacaoNavio.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
