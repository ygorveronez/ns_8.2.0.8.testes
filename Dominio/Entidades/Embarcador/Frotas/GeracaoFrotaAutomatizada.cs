using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GERACAO_FROTA_AUTOMATIZADA", EntityName = "GeracaoFrotaAutomatizada", Name = "Dominio.Entidades.Embarcador.Frota.GeracaoFrotaAutomatizada", NameType = typeof(GeracaoFrotaAutomatizada))]
    public class GeracaoFrotaAutomatizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GFA_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_GERACAO_FROTA_AUTOMATIZADA_FILIAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filiais.Filial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_GERACAO_FROTA_AUTOMATIZADA_TIPO_OPERACOES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TipoOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularesCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_GERACAO_FROTA_AUTOMATIZADA_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosVeicularesCarga { get; set; }


    }
}
