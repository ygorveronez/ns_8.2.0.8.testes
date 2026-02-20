using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_MOVIMENTO_CENTRO_RESULTADO", EntityName = "TipoMovimentoCentroResultado", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado", NameType = typeof(TipoMovimentoCentroResultado))]
    public class TipoMovimentoCentroResultado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.CentroResultado?.Descricao ?? string.Empty;
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }        

        public virtual bool Equals(TipoMovimentoCentroResultado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
