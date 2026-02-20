using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUIA_NACIONAL_RECOLHIMENTO_TRIBUTO_ESTADUDAL", EntityName = "GuiaNacionalRecolhimentoTributoEstadual", Name = "Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual", NameType = typeof(GuiaNacionalRecolhimentoTributoEstadual))]
    public class GuiaNacionalRecolhimentoTributoEstadual : Integracao.Integracao, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GNR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NroGuia", Column = "GNR_NUMERO_GUIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NroGuia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "GNR_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "GNR_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GNR_SITUACAO", TypeType = typeof(SituacaoGuia), NotNull = false)]
        public virtual SituacaoGuia Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDigitalizacaoComprovante", Column = "GNR_SITUACAO_DIGITALIZACAO_COMPROVANTE", TypeType = typeof(SituacaoDigitalizacaoGuiaComprovante), NotNull = false)]
        public virtual SituacaoDigitalizacaoGuiaComprovante? SituacaoDigitalizacaoComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDigitalizacaoGuiaRecolhimento", Column = "GNR_SITUACAO_DIGITALIZACAO_GUIA", TypeType = typeof(SituacaoDigitalizacaoGuiaRecolhimento), NotNull = false)]
        public virtual SituacaoDigitalizacaoGuiaRecolhimento? SituacaoDigitalizacaoGuiaRecolhimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico Cte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUIA_NACIONAL_RECOLHIMENTO_TRIBUTO_ESTADUDAL_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GNR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuiaValidadaManualmente", Column = "GNR_GUIA_VALIDADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GuiaValidadaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprovanteValidadoManualmente", Column = "GNR_COMPROVANTE_VALIDADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ComprovanteValidadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidouTodasInformacoesManualmente", Column = "GNR_VALIDOU_TODAS_INFORMACOES_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidouTodasInformacoesManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuiaValidadaAutomaticamente", Column = "GNR_GUIA_VALIDADA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuiaValidadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GNR_OBSERVACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarraComprovante", Column = "GNR_CODIGO_BARRA_COMPROVANTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoBarraComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComprovante", Column = "GNR_VALOR_COMPROVANTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarraGuia", Column = "GNR_CODIGO_BARRA_GUIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoBarraGuia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorGuia", Column = "GNR_VALOR_GUIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorGuia { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLeituraOCRGuia", Column = "GNR_SITUACAO_LEITURA_GUIA", TypeType = typeof(SituacaoLeituraOCR), NotNull = false)]
        public virtual SituacaoLeituraOCR SituacaoLeituraOCRGuia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLeituraOCRComprovante", Column = "GNR_SITUACAO_LEITURA_COMPROVANTE", TypeType = typeof(SituacaoLeituraOCR), NotNull = false)]
        public virtual SituacaoLeituraOCR SituacaoLeituraOCRComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTodasAsInformacoes", Column = "GNR_SITUACAO_TODAS_AS_INFORMACOES", TypeType = typeof(SituacaoLeituraOCR), NotNull = false)]
        public virtual SituacaoLeituraOCR SituacaoTodasAsInformacoes { get; set; }

        public virtual string Descricao
        {
            get { return this.NroGuia; }
        }

        public virtual bool InformacoesCodigoBarraComprovanteXGuiaValidas
        {
            get { return (!string.IsNullOrEmpty(this.CodigoBarraComprovante) && !string.IsNullOrEmpty(this.CodigoBarraGuia)) ? this.CodigoBarraComprovante == this.CodigoBarraGuia : false; }
        }

        public virtual string TodosOsDadosValidadosOCR
        {
            get
            {
                if (this.SituacaoTodasAsInformacoes == SituacaoLeituraOCR.Rejeitado)
                    return this.SituacaoTodasAsInformacoes.ObterDescricao();

                if (this.ValidouTodasInformacoesManualmente.HasValue && this.ValidouTodasInformacoesManualmente.Value)
                    return SituacaoLeituraOCR.Validado.ObterDescricao();

                if (this.SituacaoLeituraOCRGuia != SituacaoLeituraOCR.Validado)
                    return SituacaoLeituraOCR.Inconsistente.ObterDescricao();

                if (this.SituacaoLeituraOCRComprovante != SituacaoLeituraOCR.Validado)
                    return SituacaoLeituraOCR.Inconsistente.ObterDescricao();

                bool valoresValidos = this.Valor == this.ValorComprovante && this.ValorComprovante == this.ValorGuia && this.Valor == this.ValorGuia && this.CodigoBarraComprovante == this.CodigoBarraGuia;

                return valoresValidos ? SituacaoLeituraOCR.Validado.ObterDescricao() : SituacaoLeituraOCR.Inconsistente.ObterDescricao();
            }
        }
    }
}
