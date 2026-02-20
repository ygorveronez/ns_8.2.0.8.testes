using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades.Embarcador.Veiculos;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_PATIO", EntityName = "FluxoGestaoPatio", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio", NameType = typeof(FluxoGestaoPatio))]
    public class FluxoGestaoPatio : EntidadeCargaBase, IEntidade
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FGP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Equipamento Equipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FINALIZACAO_FLUXO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_VEICULO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoAtivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "FGP_TEMPERATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull =  false)]
        public virtual decimal? Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_SITUACAO_ETAPA_FLUXO_GESTAO", TypeType = typeof(SituacaoEtapaFluxoGestaoPatio), NotNull = false)]
        public virtual SituacaoEtapaFluxoGestaoPatio SituacaoEtapaFluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAtual", Column = "FGP_ETAPA_ATUAL", TypeType = typeof(int), NotNull = true)]
        public virtual int EtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAtualLiberada", Column = "FGP_ETAPA_ATUAL_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaAtualLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_FLUXO_GESTAO_ATUAL", TypeType = typeof(EtapaFluxoGestaoPatio), NotNull = false)]
        public virtual EtapaFluxoGestaoPatio EtapaFluxoGestaoPatioAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_TIPO", TypeType = typeof(TipoFluxoGestaoPatio), NotNull = false)]
        public virtual TipoFluxoGestaoPatio Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Etapas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_GESTAO_PATIO_ETAPAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FGP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FluxoGestaoPatioEtapas", Column = "FGE_CODIGO")]
        public virtual ICollection<FluxoGestaoPatioEtapas> Etapas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoCargaLiberada", Column = "FGP_EMISSAO_CARGA_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoCargaLiberada { get; set; }

        public virtual string Descricao
        {
            get { return this.Carga?.CodigoCargaEmbarcador ?? string.Empty; }
        }

        public virtual string DescricaoSituacao
        {
            get { return this.SituacaoEtapaFluxoGestaoPatio.ObterDescricao(); }
        }

        #endregion

        #region Propriedades da Etapa Informar Doca

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DOCA_INFORMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocaInformada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DOCA_INFORMADA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocaInformadaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DOCA_INFORMADA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocaInformadaReprogramada { get; set; }

        public virtual int? DiferencaDocaInformada
        {
            get { return DiffTimeMinutes(this.DataDocaInformadaPrevista, this.DataDocaInformada); }
        }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_INFORMAR_DOCA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgInformarDoca { get; set; }

        #endregion Propriedades da Etapa Informar Doca

        #region Propriedades da Etapa Chegada de Veículo

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_VEICULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_VEICULO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaVeiculoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_VEICULO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaVeiculoReprogramada { get; set; }

        public virtual int? DiferencaChegadaVeiculo
        {
            get {  return DiffTimeMinutes(this.DataChegadaVeiculoPrevista, this.DataChegadaVeiculo); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_CHEGADA_VEICULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgChegadaVeiculo { get; set; }

        #endregion Propriedades da Etapa Chegada de Veículo

        #region Propriedades da Etapa Guarita de Entrada

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_ENTRADA_GUARITA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntregaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_ENTRADA_GUARITA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntregaGuaritaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_ENTRADA_GUARITA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntregaGuaritaReprogramada { get; set; }

        public virtual int? DiferencaEntregaGuarita
        {
            get { return DiffTimeMinutes(this.DataEntregaGuaritaPrevista, this.DataEntregaGuarita); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_ENTRADA_GUARITA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgEntradaGuarita { get; set; }

        #endregion Propriedades da Etapa Guarita de Entrada

        #region Propriedades da Etapa Checklist

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CHECKLIST", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CHECKLIST_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCheckListPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CHECKLIST_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCheckListReprogramada { get; set; }

        public virtual int? DiferencaFimCheckList
        {
            get { return DiffTimeMinutes(this.DataFimCheckListPrevista, this.DataFimCheckList); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_CHECKLIST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgChecklist { get; set; }

        #endregion Propriedades da Etapa Checklist

        #region Propriedades da Etapa Travamento de Chave

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TRAVA_CHAVE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTravaChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TRAVA_CHAVE_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTravaChavePrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TRAVA_CHAVE_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTravaChaveReprogramada { get; set; }

        public virtual int? DiferencaTravaChave
        {
            get { return DiffTimeMinutes(this.DataTravaChavePrevista, this.DataTravaChave); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_TRAVA_CHAVE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgTravaChave { get; set; }

        #endregion Propriedades da Etapa Travamento de Chave

        #region Propriedades da Etapa Início do Carregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_CARREGAMENTO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioCarregamentoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_CARREGAMENTO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioCarregamentoReprogramada { get; set; }

        public virtual int? DiferencaInicioCarregamento
        {
            get { return DiffTimeMinutes(this.DataInicioCarregamentoPrevista, this.DataInicioCarregamento); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_INICIO_CARREGAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgInicioCarregamento { get; set; }

        #endregion Propriedades da Etapa Início do Carregamento

        #region Propriedades da Etapa Fim do Carregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CARREGAMENTO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCarregamentoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_CARREGAMENTO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimCarregamentoReprogramada { get; set; }

        public virtual int? DiferencaFimCarregamento
        {
            get { return DiffTimeMinutes(this.DataFimCarregamentoPrevista, this.DataFimCarregamento); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_FIM_CARREGAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgFimCarregamento { get; set; }

        #endregion Propriedades da Etapa Fim do Carregamento

        #region Propriedades da Etapa Liberação de Chave

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_LIBERACAO_CHAVE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_LIBERACAO_CHAVE_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoChavePrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_LIBERACAO_CHAVE_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoChaveReprogramada { get; set; }

        public virtual int? DiferencaLiberacaoChave
        {
            get { return DiffTimeMinutes(this.DataLiberacaoChavePrevista, this.DataLiberacaoChave); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_LIBERACAO_CHAVE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgLiberacaoChave { get; set; }

        #endregion Propriedades da Etapa Liberação de Chave

        #region Propriedades da Etapa Faturamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FATURAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FATURAMENTO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamentoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FATURAMENTO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamentoReprogramada { get; set; }

        public virtual int? DiferencaFaturamento
        {
            get { return DiffTimeMinutes(this.DataFaturamentoPrevista, this.DataFaturamento); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_FATURAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgFaturamento { get; set; }

        #endregion Propriedades da Etapa Faturamento

        #region Propriedades da Etapa Início da Viagem

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SAIDA_GUARITA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SAIDA_GUARITA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SAIDA_GUARITA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemReprogramada { get; set; }

        public virtual int? DiferencaInicioViagem
        {
            get { return DiffTimeMinutes(this.DataInicioViagemPrevista, this.DataInicioViagem); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_INICIO_VIAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgInicioViagem { get; set; }

        #endregion Propriedades da Etapa Início da Viagem

        #region Propriedades da Etapa Posicao

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_POSICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_POSICAO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicaoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_POSICAO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicaoReprogramada { get; set; }

        public virtual int? DiferencaPosicao
        {
            get { return DiffTimeMinutes(this.DataPosicaoPrevista, this.DataPosicao); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_POSICAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgPosicao { get; set; }

        #endregion Propriedades da Etapa Posicao

        #region Propriedades da Etapa Chegada na Loja

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_LOJA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_LOJA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaLojaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_CHEGADA_LOJA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaLojaReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_CHEGADA_LOJA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgChegadaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_NOTA_FISCAL_CHEGADA_LOJA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NotaFiscalChegadaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DEVOLUCAO_CHEGADA_LOJA", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = true)]
        public virtual Dominio.Enumeradores.OpcaoSimNao DevolucaoChegadaLoja { get; set; }

        public virtual int? DiferencaChegadaLoja
        {
            get { return DiffTimeMinutes(this.DataChegadaLojaPrevista, this.DataChegadaLoja); }
        }

        #endregion Propriedades da Etapa Chegada na Loja

        #region Propriedades da Etapa Deslocamento Pátio

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DESLOCAMENTO_PATIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeslocamentoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DESLOCAMENTO_PATIO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeslocamentoPatioPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DESLOCAMENTO_PATIO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeslocamentoPatioReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_DESLOCAMENTO_PATIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgDeslocamentoPatio { get; set; }

        public virtual int? DiferencaDeslocamentoPatio
        {
            get { return DiffTimeMinutes(this.DataDeslocamentoPatioPrevista, this.DataDeslocamentoPatio); }
        }

        #endregion Propriedades da Etapa Deslocamento Pátio

        #region Propriedades da Etapa Saída da Loja

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_SAIDA_LOJA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_SAIDA_LOJA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaLojaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_SAIDA_LOJA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaLojaReprogramada { get; set; }

        public virtual int? DiferencaSaidaLoja
        {
            get { return DiffTimeMinutes(this.DataSaidaLojaPrevista, this.DataSaidaLoja); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_SAIDA_LOJA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgSaidaLoja { get; set; }

        #endregion Propriedades da Etapa Saída da Loja

        #region Propriedades da Etapa Fim da Viagem

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FIM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FIM_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_FIM_VIAGEM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemReprogramada { get; set; }

        public virtual int? DiferencaFimViagem
        {
            get { return DiffTimeMinutes(this.DataFimViagemPrevista, this.DataFimViagem); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_FIM_VIAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgFimViagem { get; set; }

        #endregion Propriedades da Etapa Fim da Viagem

        #region Propriedades da Etapa Início de Higienização

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_HIGIENIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_HIGIENIZACAO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioHigienizacaoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_HIGIENIZACAO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioHigienizacaoReprogramada { get; set; }

        public virtual int? DiferencaInicioHigienizacao
        {
            get { return DiffTimeMinutes(this.DataInicioHigienizacaoPrevista, this.DataInicioHigienizacao); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_INICIO_HIGIENIZACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoInicioHigienizacao { get; set; }

        #endregion Propriedades da Etapa Início de Higienização

        #region Propriedades da Etapa Fim da Higienização

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_HIGIENIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_HIGIENIZACAO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimHigienizacaoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_HIGIENIZACAO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimHigienizacaoReprogramada { get; set; }

        public virtual int? DiferencaFimHigienizacao
        {
            get { return DiffTimeMinutes(this.DataFimHigienizacaoPrevista, this.DataFimHigienizacao); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_FIM_HIGIENIZACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoFimHigienizacao { get; set; }

        #endregion Propriedades da Etapa Fim da Higienização

        #region Propriedades da Etapa Solicitação de Veículo

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SOLICITACAO_VEICULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SOLICITACAO_VEICULO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoVeiculoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SOLICITACAO_VEICULO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoVeiculoReprogramada { get; set; }

        public virtual int? DiferencaSolicitacaoVeiculo
        {
            get { return DiffTimeMinutes(this.DataSolicitacaoVeiculoPrevista, this.DataSolicitacaoVeiculo); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_SOLICITACAO_VEICULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoSolicitacaoVeiculo { get; set; }

        #endregion Propriedades da Etapa Solicitação de Veículo

        #region Propriedades da Etapa Início do Descarregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_DESCARREGAMENTO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioDescarregamentoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_INICIO_DESCARREGAMENTO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioDescarregamentoReprogramada { get; set; }

        public virtual int? DiferencaInicioDescarregamento
        {
            get { return DiffTimeMinutes(this.DataInicioDescarregamentoPrevista, this.DataInicioDescarregamento); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_INICIO_DESCARREGAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoInicioDescarregamento { get; set; }

        #endregion Propriedades da Etapa Início do Descarregamento

        #region Propriedades da Etapa Fim do Descarregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_DESCARREGAMENTO_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimDescarregamentoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_DESCARREGAMENTO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimDescarregamentoReprogramada { get; set; }

        public virtual int? DiferencaFimDescarregamento
        {
            get { return DiffTimeMinutes(this.DataFimDescarregamentoPrevista, this.DataFimDescarregamento); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_FIM_DESCARREGAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoFimDescarregamento { get; set; }

        #endregion Propriedades da Etapa Fim do Descarregamento

        #region Propriedades da Etapa Documento Fiscal

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTO_FISCAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTO_FISCAL_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentoFiscalPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTO_FISCAL_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentoFiscalReprogramada { get; set; }

        public virtual int? DiferencaDocumentoFiscal
        {
            get { return DiffTimeMinutes(this.DataDocumentoFiscalPrevista, this.DataDocumentoFiscal); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_DOCUMENTO_FISCAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoDocumentoFiscal { get; set; }

        #endregion Propriedades da Etapa Documento Fiscal

        #region Propriedades da Etapa Documentos de Transporte

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTOS_TRANSPORTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTOS_TRANSPORTE_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentosTransportePrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_DOCUMENTOS_TRANSPORTE_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentosTransporteReprogramada { get; set; }

        public virtual int? DiferencaDocumentosTransporte
        {
            get { return DiffTimeMinutes(this.DataDocumentosTransportePrevista, this.DataDocumentosTransporte); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoDocumentosTransporte { get; set; }

        #endregion Propriedades da Etapa Documentos de Transporte

        #region Propriedades da Etapa Montagem de Carga

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_MONTAGEM_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_MONTAGEM_CARGA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMontagemCargaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_MONTAGEM_CARGA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMontagemCargaReprogramada { get; set; }

        public virtual int? DiferencaMontagemCarga
        {
            get { return DiffTimeMinutes(this.DataMontagemCargaPrevista, this.DataMontagemCarga); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_MONTAGEM_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoMontagemCarga { get; set; }

        #endregion Propriedades da Etapa Documentos de Transporte

        #region Propriedades da Etapa Separação de Mercadoria

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SEPARACAO_MERCADORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SEPARACAO_MERCADORIA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSeparacaoMercadoriaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_SEPARACAO_MERCADORIA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSeparacaoMercadoriaReprogramada { get; set; }

        public virtual int? DiferencaSeparacaoMercadoria
        {
            get { return DiffTimeMinutes(this.DataSeparacaoMercadoriaPrevista, this.DataSeparacaoMercadoria); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AGUARDANDO_SEPARACAO_MERCADORIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAguardandoSeparacaoMercadoria { get; set; }

        #endregion Propriedades da Etapa Separação de Mercadoria

        #region Propriedades da Etapa Avaliação Descarga

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_AVALIACAO_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimAvaliacaoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_AVALIACAO_DESCARGA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimAvaliacaoDescargaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_DATA_FIM_AVALIACAO_DESCARGA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimAvaliacaoDescargaReprogramada { get; set; }

        public virtual int? DiferencaFimAvaliacaoDescarga
        {
            get { return DiffTimeMinutes(this.DataFimAvaliacaoDescargaPrevista, this.DataFimAvaliacaoDescarga); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGP_TEMPO_AG_AVALIACAO_DESCARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgAvaliacaoDescarga { get; set; }

        #endregion Propriedades da Etapa Avaliação Descarga

        #region Métodos Privados

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        #endregion

        #region Métodos Públicos 

        public virtual List<FluxoGestaoPatioEtapas> GetEtapas()
        {
            if (this.Etapas == null)
                return new List<FluxoGestaoPatioEtapas>();

            return  (from o in this.Etapas orderby o.Ordem ascending select o).ToList();
        }

        public virtual FluxoGestaoPatioEtapas GetEtapaAtual()
        {
            if (this.Etapas == null)
                return null;

            List<FluxoGestaoPatioEtapas> etapas = this.GetEtapas();

            return etapas[this.EtapaAtual];
        }

        #endregion
    }
}
