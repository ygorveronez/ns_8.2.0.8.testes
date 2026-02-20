using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR", EntityName = "ArquivoImportacaoVendedor", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedor", NameType = typeof(ArquivoImportacaoVendedor))]
    public class ArquivoImportacaoVendedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AIV_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "AIV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Campos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR_CAMPO_LISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AIV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ArquivoImportacaoVendedorCampo", Column = "AVC_CODIGO")]
        public virtual IList<ArquivoImportacaoVendedorCampo> Campos { get; set; }
    }
}
