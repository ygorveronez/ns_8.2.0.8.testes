using Dominio.Entidades.Embarcador.Frota.Sinistro;
using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_DADOS", EntityName = "SinistroDados", Name = "Dominio.Entidades.Embarcador.Frota.SinistroDados", NameType = typeof(SinistroDados))]
    public class SinistroDados : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_ETAPA", TypeType = typeof(EtapaSinistro), NotNull = true)]
        public virtual EtapaSinistro Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_SITUACAO", TypeType = typeof(SituacaoEtapaFluxoSinistro), NotNull = true)]
        public virtual SituacaoEtapaFluxoSinistro Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_CAUSADOR_SINISTRO", TypeType = typeof(CausadorSinistro), NotNull = true)]
        public virtual CausadorSinistro CausadorSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_NUMERO_BOLETIM_OCORRENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroBoletimOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_DATA_FLUXO_SINISTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLancamentoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_DATA_SINISTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_LOCAL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Local { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_ENDERECO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_REBOQUE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoReboque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoSinistro", Column = "TSI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoSinistro TipoSinistro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GravidadeSinistro", Column = "TGS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GravidadeSinistro GravidadeSinistro { get; set; }

        #region Etapa Manutenção

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramada", Column = "SDS_DATA_PROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_LOCAL_MANUTENCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOS", Column = "SDS_OBSERVACAO_OS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoOS { get; set; }

        #endregion

        #region Etapa Indicador do Pagador

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDS_INDICADOR_PAGADOR", TypeType = typeof(IndicadorPagadorSinistro), NotNull = false)]
        public virtual IndicadorPagadorSinistro? IndicadorPagador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TITULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente PessoaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoTitulo", Column = "SDS_DATA_EMISSAO_TITULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoTitulo", Column = "SDS_DATA_VENCIMENTO_TITULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalTitulo", Column = "SDS_VALOR_ORIGINAL_TITULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "SDS_FORMA_TITULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoTitulo", Column = "SDS_TIPO_DOCUMENTO_TITULO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TipoDocumentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoTitulo", Column = "SDS_NUMERO_DOCUMENTO_TITULO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string NumeroDocumentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinhaDigitavelBoleto", Column = "SDS_LINHA_DIGITAVEL_BOLETO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LinhaDigitavelBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumeroBoleto", Column = "SDS_NOSSO_NUMERO_BOLETO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NossoNumeroBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTitulo", Column = "SDS_OBSERVACAO_TITULO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTitulo { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEventoFolhaLancamento", Column = "SDS_NUMERO_EVENTO_FOLHA_LANCAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEventoFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContratoFolhaLancamento", Column = "SDS_NUMERO_CONTRATO_FOLHA_LANCAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroContratoFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialFolhaLancamento", Column = "SDS_DATA_INICIAL_FOLHA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalFolhaLancamento", Column = "SDS_DATA_FINAL_FOLHA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoFolhaLancamento", Column = "SDS_DESCRICAO_FOLHA_LANCAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string DescricaoFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseFolhaLancamento", Column = "SDS_BASE_FOLHA_LANCAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReferenciaFolhaLancamento", Column = "SDS_REFERENCIA_FOLHA_LANCAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReferenciaFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFolhaLancamento", Column = "SDS_VALOR_FOLHA_LANCAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompetenciaFolhaLancamento", Column = "SDS_DATA_COMPETENCIA_FOLHA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompetenciaFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_FOLHA_LANCAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioFolhaLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FolhaInformacao", Column = "FOI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RH.FolhaInformacao FolhaInformacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FolhaLancamento", Column = "FOL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RH.FolhaLancamento FolhaLancamento { get; set; }

        #endregion

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get { return $"Fluxo de Sinistro N° {Numero}"; }
        }

        public virtual bool PossuiTitulo
        {
            get { return DataEmissaoTitulo.HasValue; }
        }

        #endregion
    }
}
