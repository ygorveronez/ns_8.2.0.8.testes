using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_JANELA_COLETA", EntityName = "JanelaColeta", Name = "Dominio.Entidades.Embarcador.Logistica.JanelaColeta", NameType = typeof(JanelaColeta))]
    public class JanelaColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JCO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "JCO_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Localidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_JANELA_COLETA_LOCALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Localidade> Localidades { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "Estados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_JANELA_COLETA_UF")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Dominio.Entidades.Estado> Estados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_JANELA_COLETA_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosColeta", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_JANELA_COLETA_PERIODO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoColeta", Column = "PCO_CODIGO")]
        public virtual ICollection<PeriodoColeta> PeriodosColeta { get; set; }



        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "JCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

    }
}
