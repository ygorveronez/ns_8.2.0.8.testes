using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSICAO_ALVO_SUBAREA", EntityName = "PosicaoAlvoSubarea", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea", NameType = typeof(PosicaoAlvoSubarea))]
    public class PosicaoAlvoSubarea : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "PosicaoAlvo", Class = "PosicaoAlvo", Column = "POA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo PosicaoAlvo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "SubareaCliente", Class = "SubareaCliente", Column = "SAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.SubareaCliente SubareaCliente { get; set; }

    }
}
