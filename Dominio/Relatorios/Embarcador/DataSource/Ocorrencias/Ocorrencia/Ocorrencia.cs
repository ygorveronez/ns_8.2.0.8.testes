using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.Ocorrencia
{
    public sealed class Ocorrencia
    {
        #region Propriedades

        public string Carga { get; set; }
        public int Carencia { get; set; }
        public string Cliente { get; set; }
        public string CNPJEmpresa { get; set; }
        public string Setor { get; set; }
        public string CargaAgrupada { get; set; }
        public int Codigo { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataAprovacao { get; set; }
        public DateTime DataCarga { get; set; }
        public DateTime DataChegada { get; set; }
        public DateTime DataEmbarque { get; set; }
        public DateTime DataChegadaReentrega { get; set; }
        public DateTime DataRetornoReentrega { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime DataRetornoSolicitacaoCredito { get; set; }
        public string DescricaoOcorrencia { get; set; }
        public string CodigoIntegracaoDestinatarios { get; set; }
        public string Destinatarios { get; set; }
        public string GrupoPessoas { get; set; }
        public string JanelaDescarga { get; set; }
        public string JustificativaRejeicao { get; set; }
        public string ObservacaoAprovacao { get; set; }
        public string MotivoCancelamento { get; set; }
        public string MotivoAprovacao { get; set; }
        public string MotivoRejeicao { get; set; }
        public string Motorista { get; set; }
        public string NomeCreditor { get; set; }
        public string NotasFiscais { get; set; }
        public string SerieNotasFiscais { get; set; }
        public string Chamado { get; set; }
        public string NumeroGlog { get; set; }
        public int NumeroOcorrencia { get; set; }
        public string NumeroOcorrenciaCliente { get; set; }
        public string NumerosCTeOriginal { get; set; }
        public string NumerosCTeOcorrencia { get; set; }
        public string NumerosCTes { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoCTeOriginal { get; set; }
        public string ObservacaoCTeComp { get; set; }
        public string Operadores { get; set; }
        public string Placa { get; set; }
        public string ModeloVeicular { get; set; }
        public decimal QuantidadeDeHoras { get; set; }
        public string CodigoIntegracaoRemetentes { get; set; }
        public string Remetentes { get; set; }
        public string Responsavel { get; set; }
        public SituacaoOcorrencia Situacao { get; set; }
        public SituacaoOcorrencia SituacaoCancelamento { get; set; }
        public string Solicitante { get; set; }
        public string TipoVeiculo { get; set; }
        public string CodigoIntegracaoTomador { get; set; }
        public string Tomador { get; set; }
        public string Transportadora { get; set; }
        public decimal Valor { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public string CNPJFilial { get; set; }
        public string Filial { get; set; }
        public string ChaveCTeComp { get; set; }
        public string DataEmissaoCTeComp { get; set; }

        public string CSTIBSCBSCTeComp { get; set; }
        public string ClassTribIBSCBSCTeComp { get; set; }
        public decimal ValorCBSCTeComp { get; set; }
        public decimal ValorIBSMunicipalCTeComp { get; set; }
        public decimal ValorIBSUFCTeComp { get; set; }

        public decimal ValorReceberCTeComp { get; set; }
        public decimal ValorIcmsCTeComp { get; set; }
        public string StatusCTeComp { get; set; }
        public string RetornoSefazCTeComp { get; set; }
        public string CSTICMSCTeComp { get; set; }
        public string TipoOperacaoCarga { get; set; }
        public string TipoCreditoDebito { get; set; }
        public string CargaPeriodo { get; set; }
        public string MesPeriodo { get; set; }
        public string AnoPeriodo { get; set; }
        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public string CentroResultado { get; set; }
        public string Categoria { get; set; }
        private Dominio.Enumeradores.TipoDocumento TipoDocumento { get; set; }
        private AutorizacaoOcorrenciaPagamento Pagamento { get; set; }
        public int NumeroAcerto { get; set; }
        public string NomeFantasiaDestinatarios { get; set; }
        public string Pedidos { get; set; }
        public string Destinos { get; set; }
        public string ParametroOcorrencia1 { get; set; }
        public string ParametroOcorrencia2 { get; set; }
        public string ParametroOcorrencia3 { get; set; }
        public string ParametroOcorrencia4 { get; set; }
        public string ParametroOcorrencia5 { get; set; }
        public string ParametroOcorrencia6 { get; set; }
        public string CNPJDestinatarios { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string DataInicioEstadia { get; set; }
        public string DataFimEstadia { get; set; }
        public string HorasTotaisEstadia { get; set; }
        public string HorasExcedentesEstadia { get; set; }
        public string HorasFreetime { get; set; }
        public string EtapaEstadia { get; set; }
        public decimal ValorOriginal { get; set; }
        private double CPFCNPJCliente { get; set; }
        public string ChavesCTeOriginal { get; set; }
        public string JustificativaOcorrencia { get; set; }
        public string CodigoAprovacao { get; set; }
        public string CPFMotorista { get; set; }
        public string Origem { get; set; }
        public string ProtocoloOcorrencia { get; set; }
        public string GrupoOcorrencia { get; set; }

        #endregion

        #region Propriedades com Regras

        public string MotivoRejeicaoFormatado
        {
            get
            {
                if (string.IsNullOrEmpty(MotivoRejeicao))
                    return "";

                if (MotivoRejeicao.Length > 200)
                    return (Localization.Resources.Ocorrencias.Ocorrencia.ParaVerificarMotivoRejeicaoAcesseMenuAutorizacaoOcorrencias);

                return MotivoRejeicao.Length > 200
                    ? MotivoRejeicao.Substring(0, 200)
                    : MotivoRejeicao;
            }
        }


        public string TipoDocumentoFormatado
        {
            get { return Dominio.Enumeradores.TipoDocumentoHelper.ObterDescricao(TipoDocumento); }
        }

        public string PagamentoFormatado
        {
            get { return Pagamento.ObterDescricao(); }
        }

        public string CNPJFilialFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJFilial) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJFilial)) : string.Empty; }
        }

        public string Operador
        {
            get { return string.IsNullOrWhiteSpace(NomeCreditor) ? Operadores : NomeCreditor; }
        }

        public string CNPJTransportadora
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CNPJEmpresa))
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJEmpresa));
                else
                    return "";
            }
        }

        public string DataAprovacaoFormatada
        {
            get
            {
                if (DataRetornoSolicitacaoCredito != DateTime.MinValue)
                    return DataRetornoSolicitacaoCredito.ToString("dd/MM/yyyy HH:mm:ss");

                if (DataAprovacao != DateTime.MinValue)
                    return DataAprovacao.ToString("dd/MM/yyyy HH:mm:ss");

                return "";
            }
        }

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataChegadaFormatada
        {
            get { return DataChegada != DateTime.MinValue ? DataChegada.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataEmbarqueFormatada
        {
            get { return DataEmbarque != DateTime.MinValue ? DataEmbarque.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataChegadaReentregaFormatada
        {
            get { return DataChegadaReentrega != DateTime.MinValue ? DataChegadaReentrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataRetornoReentregaFormatada
        {
            get { return DataRetornoReentrega != DateTime.MinValue ? DataRetornoReentrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataSaidaFormatada
        {
            get { return DataSaida != DateTime.MinValue ? DataSaida.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string DescricaoSituacaoCancelamento
        {
            get { return SituacaoCancelamento.ObterDescricao(); }
        }

        public string PedidosFormatado
        {
            get { return !string.IsNullOrEmpty(Pedidos) ? (Pedidos.Contains("_") ? Pedidos.Split('_')[1].ToString() : Pedidos) : Pedidos; }
        }

        public string CNPJDestinatariosFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CNPJDestinatarios))
                {
                    string[] cnpjs = CNPJDestinatarios.Trim().Split(',');
                    string cnpjsFormatados = string.Empty;
                    for (var i = 0; i < cnpjs.Length; i++)
                    {
                        if (i > 0)
                            cnpjsFormatados += ", ";

                        cnpjsFormatados += cnpjs[i].ObterCpfOuCnpjFormatado();
                    }

                    return cnpjsFormatados;
                }
                else
                    return "";
            }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CPFCNPJClienteDescricao
        {
            get { return CPFCNPJCliente > 0d ? CPFCNPJCliente.ToString().ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrEmpty(CPFMotorista))
                {
                    string[] cpfs = CPFMotorista.Trim().Split(',');
                    string cpfsFormatados = string.Empty;
                    for (var i = 0; i < cpfs.Length; i++)
                    {
                        if (i > 0)
                            cpfsFormatados += ", ";

                        cpfsFormatados += cpfs[i].Trim().ObterCpfOuCnpjFormatado();
                    }

                    return cpfsFormatados;
                }
                else
                    return string.Empty;
            }
        }

        #endregion
    }
}