using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_CONTABILIZACAO", EntityName = "LoteContabilizacao", Name = "Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao", NameType = typeof(LoteContabilizacao))]
    public class LoteContabilizacao: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "LCO_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_DATA_GERACAO_LOTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCO_GEROU_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_CONTABILIZACAO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LoteContabilizacaoIntegracao", Column = "LCI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_EXPORTACAO_CONTABIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoExportacaoContabil", Column = "DEC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> Documentos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}
