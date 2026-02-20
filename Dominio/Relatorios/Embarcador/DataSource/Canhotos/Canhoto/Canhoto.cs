using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto
{
    public class Canhoto
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string ChaveNF { get; set; }
        public int Numero { get; set; }

        public TipoCanhoto TipoCanhoto { get; set; }
        public TipoRejeicaoPelaIA TipoRejeicaoPelaIA { get; set; }
        public string Serie { get; set; }
        public decimal Valor { get; set; }
        public decimal PesoBruto { get; set; }
        public DateTime DataEmissao { get; set; }

        public ModalidadePagamentoFrete ModalidadeFrete { get; set; }
        public string NaturezaOP { get; set; }
        public DateTime DataLiberacaoPagamento { get; set; }

        public string TipoDestinatario { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string ResponsavelDigitalizacao { get; set; }
        public string ResponsavelEnvioFisico { get; set; }
        public string ResponsavelLiberacaoPagamento { get; set; }
        public string TipoEmitente { get; set; }
        public double CNPJEmitente { get; set; }
        public string Emitente { get; set; }
        public CanhotoOrigemDigitalizacao OrigemDigitalizacao { get; set; }

        public string TipoRecebedor { get; set; }
        public string TipoRemetente { get; set; }
        public double CpfCnpjRecebedor { get; set; }

        public string Recebedor { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public string Remetente { get; set; }

        public string Filial { get; set; }
        public string CNPJTransportador { get; set; }
        public string Empresa { get; set; }
        public SituacaoCanhoto SituacaoCanhoto { get; set; }
        public SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public SituacaoPgtoCanhoto SituacaoPagamentoCanhoto { get; set; }
        public string LocalArmazenamentoCanhoto { get; set; }
        public DateTime DataEnvioCanhoto { get; set; }
        public DateTime DataDigitalizacao { get; set; }

        public string TipoTerceiroResponsavel { get; set; }
        public double CNPJTerceiroResponsavel { get; set; }
        public string TerceiroResponsavel { get; set; }

        public string NumeroCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string Justificativa { get; set; }
        public string ChaveCTe { get; set; }
        public string NumeroCTe { get; set; }
        public string Veiculo { get; set; }
        public string PlacaVeiculoResponsavelEntrega { get; set; }
        public string Frota { get; set; }
        public string Motorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public decimal ValorCTe { get; set; }
        public int Pacote { get; set; }
        public int Posicao { get; set; }
        public string UFOrigem { get; set; }
        public string UFDestino { get; set; }
        public string MotivoRejeicaoDigitalizacao { get; set; }
        public DateTime DataConfirmacaoEntrega { get; set; }
        public DateTime DataConfirmacaoEntregaTransportador { get; set; }
        public DateTime DataEntregaNotaCliente { get; set; }

        public string GrupoPessoasEmitente { get; set; }
        public string GrupoPessoasDestinatario { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int NumeroProtocolo { get; set; }
        public string GrupoPessoa
        {
            get
            {
                if (ModalidadeFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                    return GrupoPessoasEmitente;
                else
                    return GrupoPessoasDestinatario;
            }
        }
        public string DescricaoSituacaoPagamento
        {
            get
            {
                return SituacaoPagamentoCanhoto.ObterDescricao();
            }
        }
        public string Usuario { get; set; }
        public string ObservacaoRecebimentoFisico { get; set; }
        private DateTime DataIntegracaoEntrega { get; set; }

        public string Protocolo { get; set; }
        public DateTime DataMalote { get; set; }
        public string Operador { get; set; }
        public string NumeroLoteLiberado { get; set; }
        public string NumeroLoteBloqueado { get; set; }

        public string Tomador { get; set; }
        public Dominio.Enumeradores.TipoPessoa TipoTomador { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string TipoCTe { get; set; }
        private string CidadeOrigem { get; set; }
        private string EstadoOrigem { get; set; }
        private string CidadeDestino { get; set; }
        private string EstadoDestino { get; set; }
        public string RazaoExpedidor { get; set; }
        public string NomeFantasiaExpedidor { get; set; }
        public decimal ValorFreteNF { get; set; }
        public string CodigoDestino { get; set; }
        public string EnderecoDeOrigem { get; set; }
        public string EnderecoDeDestino { get; set; }
        public string CodigoDaTransportadora { get; set; }
        public DateTime DataDigitacaoAprovacao { get; set; }
        public DateTime DataRecebimentoFisico { get; set; }
        public string CPFMotorista { get; set; }
        public int MaloteProtocolo { get; set; }
        public DateTime DataEmissaoCte { get; set; }
        public SituacaoNotaFiscal SituacaoNotaFiscal { get; set; }
        private DateTime DataAlteracao { get; set; }
        public string CodigoRastreio { get; set; }
        private bool ValidacaoViaOCR { get; set; }
        private StatusViagemControleEntrega SituacaoViagem { get; set; }
        public string CentroResultadoCarga { get; set; }
        public string MDFesCarga { get; set; }
        public DateTime DataEmissaoMdfe { get; set; }
        public string SerieCTe { get; set; }
        public string OrigemDaCarga { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoOrigemDigitalizacao
        {
            get
            {
                return OrigemDigitalizacao.ObterDescricao();
            }
        }

        public string DescricaoDataConfirmacaoEntrega
        {
            get
            {
                return DataConfirmacaoEntrega != DateTime.MinValue ? DataConfirmacaoEntrega.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DescricaoDataConfirmacaoEntregaTransportador
        {
            get
            {
                return DataConfirmacaoEntregaTransportador != DateTime.MinValue ? DataConfirmacaoEntregaTransportador.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DataEntregaNotaClienteFormatada
        {
            get
            {
                return DataEntregaNotaCliente != DateTime.MinValue ? DataEntregaNotaCliente.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }

        public string DataIntegracaoEntregaFormatada
        {
            get { return DataIntegracaoEntrega != DateTime.MinValue ? DataIntegracaoEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataDigitalizacaoFormatada
        {
            get { return DataDigitalizacao != DateTime.MinValue ? DataDigitalizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoTipoCanhoto
        {
            get { return TipoCanhoto.ObterDescricao(); }
        }

        public string DescricaoTipoRejeicaoPelaIA
        {
            get { return TipoRejeicaoPelaIA.ObterDescricao(); }
        }

        public string DescricaoDataEmissao
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataEmissaoMdfe
        {
            get { return DataEmissaoMdfe != DateTime.MinValue ? DataEmissaoMdfe.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoModalidadeFrete
        {
            get { return ModalidadeFrete.ObterDescricao(); }
        }

        public string DataLiberacaoPagamentoFormatada
        {
            get { return DataLiberacaoPagamento != DateTime.MinValue ? DataLiberacaoPagamento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataMaloteFormatada
        {
            get
            {
                return DataMalote != DateTime.MinValue ? DataMalote.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string CpfCnpjRecebedorFormatado
        {
            get { return CpfCnpjRecebedor > 0d ? CpfCnpjRecebedor.ToString().ObterCpfOuCnpjFormatado(TipoRecebedor) : ""; }
        }

        public string CpfCnpjRemetenteFormatado
        {
            get { return CpfCnpjRemetente > 0d ? CpfCnpjRemetente.ToString().ObterCpfOuCnpjFormatado(TipoRemetente) : ""; }
        }

        public string CPFCNPJDestinatarioFormatado
        {
            get { return CPFCNPJDestinatario.ToString().ObterCpfOuCnpjFormatado(TipoDestinatario); }
        }

        public string CPFCNPJEmitenteFormatado
        {
            get { return CNPJEmitente.ToString().ObterCpfOuCnpjFormatado(TipoEmitente); }
        }

        public string CPFCNPJTerceiroResponsavelFormatado
        {
            get { return CNPJTerceiroResponsavel.ToString().ObterCpfOuCnpjFormatado(TipoTerceiroResponsavel); }
        }

        public string CPFCNPJTomadorFormatado
        {
            get { return CPFCNPJTomador?.ToString().ObterCpfOuCnpjFormatado((TipoTomador == Dominio.Enumeradores.TipoPessoa.Fisica ? "F" : "J")) ?? ""; }
        }

        public string CNPJTransportadorFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJTransportador) ? CNPJTransportador.ObterCnpjFormatado() : string.Empty; }
        }

        public string DescricaoSituacaoCanhoto
        {
            get { return SituacaoCanhoto.ObterDescricao(); }
        }

        public string DescricaoSituacaoDigitalizacaoCanhoto
        {
            get { return SituacaoDigitalizacaoCanhoto.ObterDescricao(); }
        }

        public string SituacaoNotaFiscalDescricao
        {
            get { return SituacaoNotaFiscal.ObterDescricao(); }
        }

        public string DescricaoDataEnvioCanhoto
        {
            get { return DataEnvioCanhoto != DateTime.MinValue ? DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataHistorico { get; set; }

        public string Origem
        {
            get { return !string.IsNullOrWhiteSpace(CidadeOrigem) ? $"{CidadeOrigem} - {EstadoOrigem}" : string.Empty; }
        }

        public string Destino
        {
            get { return !string.IsNullOrWhiteSpace(CidadeDestino) ? $"{CidadeDestino} - {EstadoDestino}" : string.Empty; }
        }

        public string DataRecebimentoFormatada
        {
            get { return DataRecebimento != DateTime.MinValue ? DataRecebimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoCteFormatada
        {
            get { return DataEmissaoCte != DateTime.MinValue ? DataEmissaoCte.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataAlteracaoFormatada
        {
            get
            {
                return DataAlteracao != DateTime.MinValue ? DataAlteracao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string ValidacaoViaOCRFormatado
        {
            get { return ValidacaoViaOCR ? "Sim" : "Não"; }
        }

        public string SituacaoViagemFormatada
        {
            get { return SituacaoViagem.ObterDescricao(); }
        }

        #endregion
    }
}