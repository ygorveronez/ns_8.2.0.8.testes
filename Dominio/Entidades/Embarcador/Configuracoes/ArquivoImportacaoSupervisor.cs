using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_SUPERVISOR", EntityName = "ArquivoImportacaoSupervisor", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoSupervisor", NameType = typeof(ArquivoImportacaoSupervisor))]
    public class ArquivoImportacaoSupervisor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AIS_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "AIS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Campos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ARQUIVO_IMPORTACAO_SUPERVISOR_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AIS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ArquivoImportacaoSupervisorCampo", Column = "ASC_CODIGO")]
        public virtual IList<ArquivoImportacaoSupervisorCampo> Campos { get; set; }
    }
}
