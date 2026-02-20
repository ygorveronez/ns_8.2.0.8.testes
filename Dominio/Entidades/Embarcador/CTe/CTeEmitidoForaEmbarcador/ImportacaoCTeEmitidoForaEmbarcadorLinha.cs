using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_CTE_EMITIDO_FORA_EMBARCADOR_LINHA", EntityName = "ImportacaoCTeEmitidoForaEmbarcadorLinha", Name = "Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha", NameType = typeof(ImportacaoCTeEmitidoForaEmbarcadorLinha))]
    public class ImportacaoCTeEmitidoForaEmbarcadorLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoCTeEmitidoForaEmbarcador", Column = "ICF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador ImportacaoCTeEmitidoForaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ICL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ICL_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeEmitidoForaEmbarcador", Column = "CFE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador CTeEmitidoForaEmbarcador { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPORTACAO_CTE_EMITIDO_FORA_EMBARCADOR_LINHA_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ICL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna", Column = "ILC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> Colunas { get; set; }


    }
}
