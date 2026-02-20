using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class ConsultaCanhotos
    {
        #region Propriedades

        public int CodigoLocalArmazenamento { get; set; }

        public string DescricaoLocalArmazenamento { get; set; }

        public DateTime DataEnvioCanhoto { get; set; }

        public DateTime DataDigitalizacao { get; set; }

        public DateTime DataEntregaCliente { get; set; }

        public int Codigo { get; set; }

        public string Chave { get; set; }

        public int Numero { get; set; }

        public string Serie { get; set; }

        public int CodigoFilial { get; set; }

        public string Filial { get; set; }

        public DateTime DataNotaFiscal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }

        public string NumeroCarga { get; set; }

        public string TipoCarga { get; set; }

        public string Motorista { get; set; }

        public double? CPFCNPJEmitente { get; set; }

        public string NomeEmitente { get; set; }

        public string TipoEmitente { get; set; }

        public string Destinatario { get; set; }

        public double CNPJDestinatario { get; set; }

        public string Empresa { get; set; }

        public DateTime DataEmissao { get; set; }

        public string NomeArquivo { get; set; }

        public int PacoteArmazenado { get; set; }

        public int PosicaoNoPacote { get; set; }

        public string GuidNomeArquivo { get; set; }

        public int Valor { get; set; }

        public string Observacao { get; set; }

        public string NumeroCTe { get; set; }

        public string NumeroDocumentoOriginario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto SituacaoPgtoCanhoto { get; set; }

        public string Veiculo { get; set; }

        public string TipoDestinatario { get; set; }

        public bool ObrigarInformarDataEntregaClienteAoBaixarCanhotos { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoNotaFiscal { get; set; }

        public int NumeroProtocolo { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string CentroResultadoCarga { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRejeicaoPelaIA TipoRejeicaoPelaIA { get; set; }
        public int? TipoSituacaoIA { get; set; }
        public int? OrigemSituacaoDigitalizacaoCanhoto { get; set; }

        public string ChaveNotaFiscalVinculoCanhoto { get; set; }

        public string ChaveAcessoCTeTerceiroVinculoCanhoto { get; set; }

        public bool PossuiIntegracaoComprovei { get; set; }

        public bool ValidacaoCanhotoComprovei { get; set; }

        public bool ValidacaoNumeroComprovei { get; set; }

        public bool ValidacaoEncontrouDataComprovei { get; set; }

        public bool ValidacaoAssinaturaComprovei { get; set; }

        public string EscritorioVendasComplementar { get; set; }

        public string MatrizComplementar { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }

        public bool EnvioCanhotoFaturaHabilitado { get; set; }

        public bool DigitalizacaoIntegrada { get; set; }

        public bool ValidacaoCanhoto { get; set; }
        public bool CancelarAtendimentoAutomaticamente { get; set; }

        private int _quantidadeEnvioDigitalizacaoCanhoto;

        public int QuantidadeEnvioDigitalizacaoCanhoto
        {
            get => _quantidadeEnvioDigitalizacaoCanhoto;
            set => _quantidadeEnvioDigitalizacaoCanhoto = value;
        }

        public string QuantidadeEnvioDigitalizacaoCanhotoFormatado
        {
            get => QuantidadeEnvioDigitalizacaoCanhoto > 0
                ? QuantidadeEnvioDigitalizacaoCanhoto.ToString()
                : "-";
        }

        public string ChamadosEMotivos { get; set; }

        #endregion

        #region Propriedades com Regras


        public string CNPJDestinatarioFormatado
        {
            get
            {
                if (TipoDestinatario.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return TipoDestinatario.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatario) : String.Format(@"{0:000\.000\.000\-00}", CNPJDestinatario);
                }
            }
        }

        public string CPFCNPJEmitenteFormatado
        {
            get
            {
                if (TipoEmitente == "E")
                    return "00.000.000/0000-00";


                return TipoEmitente == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJEmitente) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJEmitente);
            }
        }

        public bool CargaEncerrada => this.SituacaoCarga == SituacaoCarga.Encerrada || this.SituacaoCarga == SituacaoCarga.Anulada;

        public string DescricaoSituacaoNotaFiscal => this.SituacaoNotaFiscal.ObterDescricao();

        public string DescricaoSituacaoPgtoCanhoto
        {
            get { return this.SituacaoPgtoCanhoto.ObterDescricao(); }
        }

        public string DescricaoTipoCanhoto => this.TipoCanhoto.ObterDescricao();

        public string DescricaoTipoRejeicaoPelaIA => this.TipoRejeicaoPelaIA.ObterDescricao();

        public string DescricaoOrigemSituacaoDigitalizacaoCanhoto
        {
            get
            {
                return this.OrigemSituacaoDigitalizacaoCanhoto.HasValue ? ((Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoDigitalizacaoCanhoto)this.OrigemSituacaoDigitalizacaoCanhoto).ObterDescricao() : "-";
            }
        }

        public string DescricaoSituacaoIA
        {
            get
            {
                return this.TipoSituacaoIA.HasValue ? ((Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao)this.TipoSituacaoIA).ObterDescricao() : "-";
            }
        }

        public string DataEnvioCanhotoDescricao => this.DataEnvioCanhoto != DateTime.MinValue ? this.DataEnvioCanhoto.ToString("g") : string.Empty;

        public string DataDigitalizacaoDescricao => this.DataDigitalizacao != DateTime.MinValue ? this.DataDigitalizacao.ToString("g") : string.Empty;

        public string DataEntregaClienteDescricao => this.DataEntregaCliente != DateTime.MinValue ? this.DataEntregaCliente.ToString("g") : string.Empty;

        public string DataNotaFiscalDescricao => this.DataNotaFiscal != DateTime.MinValue ? this.DataNotaFiscal.ToString("g") : string.Empty;

        public string DataEmissaoDescricao => this.DataEmissao != DateTime.MinValue ? this.DataEmissao.ToString("g") : string.Empty;

        public string DescricaoSituacao => this.SituacaoCanhoto.ObterDescricao();

        public string DescricaoDigitalizacao => this.SituacaoDigitalizacaoCanhoto.ObterDescricao();

        public string DescricaoTipoNotaFiscalIntegrada => this.TipoNotaFiscalIntegrada.ObterDescricao();

        public string DT_RowColor
        {
            get
            {
                if (this.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada)
                {
                    return CorGrid.Vermelho;
                }
                else
                {
                    return this.SituacaoCanhoto.ObterCorLinha();
                }
            }
        }

        public string DT_FontColor => this.SituacaoCanhoto.ObterCorFonte();

        public string Emitente
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeEmitente) || !CPFCNPJEmitente.HasValue)
                    return string.Empty;

                return $"{NomeEmitente} ({CPFCNPJEmitenteFormatado})";
            }
        }

        public bool ExibirOpcaoAlterarDataEntrega
        {
            get { return (this.ObrigarInformarDataEntregaClienteAoBaixarCanhotos && this.DataEntregaCliente != DateTime.MinValue); }
        }

        public string DigitalizacaoIntegradaDescricao { get { return !DigitalizacaoIntegrada ? "Sim" : "Não"; } }

        public string DigitalizacaoCanhotoValidadoIA { get { return !ValidacaoCanhoto ? "Sim" : "Não"; } }

        #endregion
    }
}
