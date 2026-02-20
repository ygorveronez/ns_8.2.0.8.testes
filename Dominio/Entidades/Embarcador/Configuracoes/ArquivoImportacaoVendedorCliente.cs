using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR_CLIENTE", EntityName = "ArquivoImportacaoVendedorCliente", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedorCliente", NameType = typeof(ArquivoImportacaoVendedorCliente))]
    public class ArquivoImportacaoVendedorCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AIC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "AIC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Campos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR_CLIENTE_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ArquivoImportacaoVendedorClienteCampo", Column = "ACC_CODIGO")]
        public virtual IList<ArquivoImportacaoVendedorClienteCampo> Campos { get; set; }
    }
}
