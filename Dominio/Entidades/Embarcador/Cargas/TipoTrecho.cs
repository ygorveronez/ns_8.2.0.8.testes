using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_TRECHO", EntityName = "TipoTrecho", Name = "Dominio.Entidades.Embarcador.Cargas.TipoTrecho", NameType = typeof(TipoTrecho))]
    public class TipoTrecho : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TTR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TTR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_SETOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CategoriasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CATEGORIA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CategoriaPessoa", Column = "CTP_CODIGO")]
        public virtual ICollection<Pessoas.CategoriaPessoa> CategoriasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CategoriasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CATEGORIA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CategoriaPessoa", Column = "CTP_CODIGO")]
        public virtual ICollection<Pessoas.CategoriaPessoa> CategoriasDestino { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "CategoriasExpedidor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CATEGORIA_EXPEDIDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CategoriaPessoa", Column = "CTP_CODIGO")]
        public virtual ICollection<Pessoas.CategoriaPessoa> CategoriasExpedidor { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "CategoriasRecebedor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CATEGORIA_RECEBEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CategoriaPessoa", Column = "CTP_CODIGO")]
        public virtual ICollection<Pessoas.CategoriaPessoa> CategoriasRecebedor { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculares", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<ModeloVeicularCarga> ModelosVeiculares { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CLIENTES_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_TRECHO_CLIENTES_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }
    }    
}
