using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESCALA_ORIGEM_DESTINO", EntityName = "EscalaOrigemDestino", Name = "Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino", NameType = typeof(EscalaOrigemDestino))]
    public class EscalaOrigemDestino : EntidadeBase, IEquatable<EscalaOrigemDestino>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EOD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExpedicaoEscala", Column = "EXE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExpedicaoEscala ExpedicaoEscala { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "EOD_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EscalaVeiculosEscalados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ESCALA_VEICULO_ESCALADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EOD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EscalaVeiculoEscalado", Column = "EVE_CODIGO")]
        public virtual IList<EscalaVeiculoEscalado> EscalaVeiculosEscalados { get; set; }

        public virtual string Descricao
        {
            get { return this.ClienteDestino?.Descricao ?? ""; }
        }

        public virtual bool Equals(EscalaOrigemDestino other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
