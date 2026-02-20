using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_CANCELAMENTO", EntityName = "LoteEscrituracaoCancelamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento", NameType = typeof(LoteEscrituracaoCancelamento))]
    public class LoteEscrituracaoCancelamento: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEC_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEC_GERACAO_LOTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEC_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosEscrituracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_ESCRITURACAO_CANCELAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEscrituracaoCancelamento", Column = "DEC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> DocumentosEscrituracao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_ESCRITURACAO_CANCELAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LoteEscrituracaoCancelamentoIntegracao", Column = "ILC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao> Integracoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Numero.ToString();
            }
        }
    }
}
