using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_EXPORTACAO_CONTABIL", EntityName = "DocumentoExportacaoContabil", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil", NameType = typeof(DocumentoExportacaoContabil))]
    public class DocumentoExportacaoContabil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiro", Column = "MOV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro MovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteContabilizacao", Column = "LCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao LoteContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupado", Column = "TIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado TituloBaixaAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupadoDocumento", Column = "TBD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento TituloBaixaAgrupadoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupadoDocumentoAcrescimoDesconto", Column = "TAD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto TituloBaixaAgrupadoDocumentoAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloDocumentoAcrescimoDesconto", Column = "TDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto TituloDocumentoAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoExportacaoContabil), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoExportacaoContabil Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_NUMERO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEC_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_EXPORTACAO_CONTABIL_CONTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoExportacaoContabilConta", Column = "DCC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta> Contas { get; set; }
    }
}
