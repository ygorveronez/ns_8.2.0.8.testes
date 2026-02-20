using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_TABELA_FRETE_LINHA", EntityName = "ImportacaoTabelaFreteLinha", Name = "Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha", NameType = typeof(ImportacaoTabelaFreteLinha))]
    public class ImportacaoTabelaFreteLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoTabelaFrete", Column = "ITF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IML_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IML_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IML_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirSolicitarAprovacao", Column = "ITL_PERMITIR_SOLICITAR_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSolicitarAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPORTACAO_TABELA_FRETE_LINHA_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ITL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoTabelaFreteLinhaColuna", Column = "ITC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> Colunas { get; set; }
    }
}
