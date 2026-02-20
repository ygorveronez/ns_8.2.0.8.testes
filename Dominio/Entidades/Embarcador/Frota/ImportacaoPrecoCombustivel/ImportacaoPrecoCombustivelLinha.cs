using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_PRECO_COMBUSTIVEL_LINHA", EntityName = "ImportacaoPrecoCombustivelLinha", Name = "Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha", NameType = typeof(ImportacaoPrecoCombustivelLinha))]

    public class ImportacaoPrecoCombustivelLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoPrecoCombustivel", Column = "IPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel ImportacaoPrecoCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IPL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IPL_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PostoCombustivelTabelaValores", Column = "MOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores PostoCombustivelTabelaValores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPORTACAO_PRECO_COMBUSTIVEL_LINHA_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoPrecoCombustivelLinhaColuna", Column = "ILC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> Colunas { get; set; }


    }
}