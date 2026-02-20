using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROVISAO_MANUAL", EntityName = "ProvisaoManual", Name = "Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual", NameType = typeof(ProvisaoManual))]
    public class ProvisaoManual : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual>
    {
        public ProvisaoManual() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PVM_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PVM_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProvisionado", Column = "PMV_VALOR_PROVISIONADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorProvisionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PMV_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get { return CentroResultado.Descricao + " (" + ValorProvisionado.ToString("n2") + ")"; }
        }

        public virtual bool Equals(ProvisaoManual other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
