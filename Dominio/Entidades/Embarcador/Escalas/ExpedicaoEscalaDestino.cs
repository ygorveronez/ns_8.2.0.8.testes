using System;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXPEDICAO_ESCALA_DESTINO", EntityName = "ExpedicaoEscalaDestino", Name = "Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino", NameType = typeof(ExpedicaoEscalaDestino))]
    public class ExpedicaoEscalaDestino : EntidadeBase, IEquatable<ExpedicaoEscalaDestino>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EXD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExpedicaoEscala", Column = "EXE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExpedicaoEscala ExpedicaoEscala { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "EXE_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        public virtual string Descricao
        {
            get { return this.ExpedicaoEscala?.Descricao; }
        }

        public virtual bool Equals(ExpedicaoEscalaDestino other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}