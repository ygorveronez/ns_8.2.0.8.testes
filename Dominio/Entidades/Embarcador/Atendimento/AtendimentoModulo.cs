using System;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_MODULO", EntityName = "AtendimentoModulo", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo", NameType = typeof(AtendimentoModulo))]
    public class AtendimentoModulo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ATM_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATM_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoSistema", Column = "ATS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoSistema AtendimentoSistema { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(AtendimentoModulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
