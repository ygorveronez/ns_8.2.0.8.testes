using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_NOTA_FISCAL", EntityName = "ArquivoImportacaoNotaFiscal", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal", NameType = typeof(ArquivoImportacaoNotaFiscal))]
    public class ArquivoImportacaoNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AIN_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "AIN_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Campos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ARQUIVO_IMPORTACAO_NOTA_FISCAL_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AIN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ArquivoImportacaoNotaFiscalCampo", Column = "AIC_CODIGO")]
        public virtual IList<ArquivoImportacaoNotaFiscalCampo> Campos { get; set; }
    }
}
