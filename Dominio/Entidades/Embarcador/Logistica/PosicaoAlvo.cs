using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSICAO_ALVO", EntityName = "PosicaoAlvo", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo", NameType = typeof(PosicaoAlvo))]
    public class PosicaoAlvo : EntidadeBase, IEquatable<PosicaoAlvo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "POA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Posicao", Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Cliente", Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        public virtual bool Equals(PosicaoAlvo other)
        {
            if (other == null) return false;
            return (this.Codigo == other.Codigo) || (this.Posicao.Codigo == other.Posicao.Codigo && this.Cliente.Codigo == other.Cliente.Codigo);
        }
    }
}
