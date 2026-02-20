using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_DESPESA_FINANCEIRA", EntityName = "GrupoDespesaFinanceira", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira", NameType = typeof(GrupoDespesaFinanceira))]
    public class GrupoDespesaFinanceira : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TGD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TGD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TGD_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TGD_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(GrupoDespesaFinanceira other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
