using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSICAO_LOCAL", EntityName = "PosicaoLocal", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoLocal", NameType = typeof(PosicaoLocal))]
    public class PosicaoLocal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "POL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Posicao", Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Local", Class = "Locais", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Locais Local { get; set; }

    }
}
