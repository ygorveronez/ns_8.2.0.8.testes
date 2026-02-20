using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia
{
    public class ChamadoOcorrencia
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Carga { get; set; }

        public int Numero { get; set; }

        public ChamadoResponsavelOcorrencia ResponsavelOcorrencia { get; set; }

        public string Representante { get; set; }

        public ChamadoAosCuidadosDo AosCuidadosDo { get; set; }

        private bool VeiculoCarregado { get; set; }

        public decimal NumeroPallet { get; set; }

        public string MotivoChamado { get; set; }

        public string Transportador { get; set; }

        private DateTime DataCriacao { get; set; }

        private DateTime DataRetorno { get; set; }

        public string Cliente { get; set; }

        public string Tomador { get; set; }

        public string Observacao { get; set; }

        public string Placa { get; set; }

        public string NumeroOcorrencia { get; set; }

        public string MotivoOcorrencia { get; set; }

        private DateTime DataFinalizacao { get; set; }

        public string EnderecoCliente { get; set; }

        public string BairroCliente { get; set; }

        public string CidadeCliente { get; set; }

        public string EstadoCliente { get; set; }

        public string CEPCliente { get; set; }

        public string LatitudeCliente { get; set; }

        public string LongitudeCliente { get; set; }

        private DateTime DataCriacaoCarga { get; set; }

        public string TipoOperacaoCarga { get; set; }

        public string FilialCarga { get; set; }

        private DateTime DataAssumicaoPrimeiroAtendimento { get; set; }

        private int TempoAtendimento { get; set; }

        private DateTime DataCarregamento { get; set; }

        private DateTime DataEntrega { get; set; }

        public string Notas { get; set; }

        public string CTes { get; set; }

        public string Motorista { get; set; }

        private int DiasAtraso { get; set; }

        public decimal ValorChamado { get; set; }

        private ResponsavelChamado ResponsavelChamado { get; set; }

        private SituacaoChamado Situacao { get; set; }

        public string Destinatario { get; set; }

        private string TipoDestinatario { get; set; }

        private double CPFCNPJDestinatario { get; set; }

        public string Responsavel { get; set; }

        public string Operador { get; set; }

        public decimal ValorDesconto { get; set; }

        public string TotalDeHoras { get; set; }

        public string ObservacaoAnalise { get; set; }

        public string FilialVenda { get; set; }

        public decimal QuantidadeTotalDevolvidaNoChamado { get; set; }

        private DateTime DataChegadaDiaria { get; set; }

        private DateTime DataSaidaDiaria { get; set; }

        // Diária automática
        public decimal ValorDiariaAutomatica { get; set; }

        public string TotalDeHorasDiariaAutomatica { get; set; }
        //

        public string GrupoPessoasCliente { get; set; }

        public string GrupoPessoasTomador { get; set; }

        public string GrupoPessoasDestinatario { get; set; }

        public TipoMotivoAtendimento TipoMotivoChamado { get; set; }

        public string CodigoIntegracaoCliente { get; set; }

        public string CodigoIntegracaoTomador { get; set; }

        public string CodigoIntegracaoDestinatario { get; set; }

        public string ModeloVeicularCarga { get; set; }

        private DateTime DataChegadaMotorista { get; set; }

        public SituacaoEntrega TratativaAtendimento { get; set; }

        public string CodigoIntegracaoClienteResponsavel { get; set; }

        private string TipoClienteResponsavel { get; set; }

        private double CPFCNPJClienteResponsavel { get; set; }

        public string ClienteResponsavel { get; set; }

        public string GrupoPessoasResponsavel { get; set; }

        private DateTime DataRetencaoInicio { get; set; }

        private DateTime DataRetencaoFim { get; set; }

        private DateTime DataReentrega { get; set; }

        public decimal PesoCarga { get; set; }

        public string PeriodoJanelaDescarga { get; set; }

        private bool DevolucaoParcial { get; set; }

        public string DescricaoRota { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public double CNPJCliente { get; set; }

        public string TipoCliente { get; set; }

        public string JustificativaOcorrencia { get; set; }

        public string DataPrevistaEntregaRetorno { get; set; }

        public string PossuiAnexoNFSe { get; set; }

        public string TipoCarga { get; set; }

        public string GeneroMotivoChamado { get; set; }

        public string AreaEnvolvidaMotivoChamado { get; set; }

        public string MotivoProcesso { get; set; }

        public int QuantidadeDivergencia { get; set; }

        public string ModeloVeicularCargaEntrega { get; set; }

        public string GrupoTipoOcorrencia { get; set; }

        public string Anexos { get; set; }

        public string MotivoDevolucao { get; set; }

        public string DescricaoSIF { get; set; }

        public string CodigoSIF { get; set; }

        public string CPFMotorista { get; set; }

        public string TipoMotorista { get; set; }

        public int DocumentoComplementar { get; set; }

        public decimal ValorJaIncluso { get; set; }

        public int NFAtendimento { get; set; }

        public decimal ValorDevItem { get; set; }

        public decimal ValorDevNota { get; set; }

        public decimal ValorDevTotal { get; set; }

        public decimal ValorOcorrencia { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string CodigoProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public int QuantidadeDevolucao { get; set; }

        public decimal ValorDevolucao { get; set; }

        public string NumeroNFD { get; set; }

        public string SerieNFD { get; set; }

        public string ChaveNFD { get; set; }

        public string DataEmissaoNFD { get; set; }

        public string ValorTotalProdutoNFD { get; set; }

        public string ValorTotalNFNFD { get; set; }

        public string PesoDevolvidoNFD { get; set; }

        public string NFeOrigem { get; set; }

        public int QuantidadeValePallet { get; set; }

        public SimNao Estadia { get; set; }

        public string SenhaDevolucao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataRetornoFormatado
        {
            get { return DataRetorno != DateTime.MinValue ? DataRetorno.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCriacaoFormatado
        {
            get { return DataCriacao != DateTime.MinValue ? DataCriacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFinalizacaoFormatado
        {
            get { return DataFinalizacao != DateTime.MinValue ? DataFinalizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCarregamentoFormatado
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEntregaFormatado
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataChegadaMotoristaFormatada
        {
            get { return DataChegadaMotorista != DateTime.MinValue ? DataChegadaMotorista.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CNPJDestinatario
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.TipoDestinatario) || this.TipoDestinatario.Equals("E") || this.CPFCNPJDestinatario <= 0)
                    return "";
                else
                    return this.TipoDestinatario.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJDestinatario) : string.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJDestinatario);
            }
        }

        public string CNPJClienteResponsavelFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoClienteResponsavel) || TipoClienteResponsavel.Equals("E") || CPFCNPJClienteResponsavel <= 0)
                    return "";
                else
                    return TipoClienteResponsavel.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJClienteResponsavel) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJClienteResponsavel);
            }
        }

        public string TempoTotal
        {
            get
            {
                DateTime dataFinal = (DataFinalizacao == DateTime.MinValue) ? DateTime.Now : DataFinalizacao;
                TimeSpan tempoTotal = (dataFinal - this.DataCriacao);
                string dias = "";
                if (tempoTotal.Days > 0)
                    dias = tempoTotal.Days + " dia" + (tempoTotal.Days > 1 ? "s" : "") + " ";

                return dias + tempoTotal.ToString(@"hh\:mm");
            }
        }

        public string StatusAtrasoCarga
        {
            get
            {
                if (this.DiasAtraso > 0)
                    return "Atrasado";
                else
                    return "Em Dia";
            }
        }

        public virtual string DescricaoResponsavelOcorrencia
        {
            get { return ResponsavelOcorrencia.ObterDescricao(); }
        }

        public virtual string DescricaoAosCuidadosDo
        {
            get { return AosCuidadosDo.ObterDescricao(); }
        }

        public string DescricaoVeiculoCarregado
        {
            get
            {
                if (VeiculoCarregado)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string DescricaoTipoMotivoChamado
        {
            get { return TipoMotivoChamado.ObterDescricao(); }
        }

        public string DescricaoResponsavelChamado
        {
            get { return ResponsavelChamado.ObterDescricao(); }
        }

        public string TempoAtendimentoFormatado
        {
            get
            {
                TimeSpan tempoAtendimento = TimeSpan.FromSeconds(TempoAtendimento);

                return $"{tempoAtendimento.Days} Dias, {tempoAtendimento.Hours} Horas, {tempoAtendimento.Minutes} Minutos, {tempoAtendimento.Seconds} Segundos";
            }
        }

        public string DataChegadaDiariaFormatada
        {
            get { return DataChegadaDiaria != DateTime.MinValue ? DataChegadaDiaria.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataSaidaDiariaFormatada
        {
            get { return DataSaidaDiaria != DateTime.MinValue ? DataSaidaDiaria.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoTratativaAtendimento
        {
            get { return Situacao.ObterDescricao() != "Aberto" ? TratativaAtendimento.ObterDescricaoTratativaDevolucao() : string.Empty; }
        }

        public string DataRetencaoInicioFormatada
        {
            get { return DataRetencaoInicio != DateTime.MinValue ? DataRetencaoInicio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetencaoFimFormatada
        {
            get { return DataRetencaoFim != DateTime.MinValue ? DataRetencaoFim.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataReentregaFormatada
        {
            get { return DataReentrega != DateTime.MinValue ? DataReentrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TipoDevolucao
        {
            get { return Situacao.ObterDescricao() != "Aberto" ? DevolucaoParcial ? TipoColetaEntregaDevolucao.Parcial.ObterDescricao() : TipoColetaEntregaDevolucao.Total.ObterDescricao() : string.Empty; }
        }

        public string CNPJClienteFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoCliente) || TipoCliente.Equals("E") || CNPJCliente <= 0)
                    return "";
                else
                    return TipoCliente.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCliente) : string.Format(@"{0:000\.000\.000\-00}", CNPJCliente);
            }
        }

        public string DataCriacaoCargaFormatado
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAssumicaoPrimeiroAtendimentoFormatado
        {
            get { return DataAssumicaoPrimeiroAtendimento != DateTime.MinValue ? DataAssumicaoPrimeiroAtendimento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string EstadiaFormatado
        {
            get { return Estadia.ObterDescricao(); }
        }

        #endregion
    }
}
