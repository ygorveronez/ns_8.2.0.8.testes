using System;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_RESPOSTA", EntityName = "AtendimentoResposta", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta", NameType = typeof(AtendimentoResposta))]
    public class AtendimentoResposta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Resposta", Column = "ATR_RESPOSTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Resposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "ATR_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATR_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atendimento", Column = "ATE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.Atendimento Atendimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        public virtual bool Equals(AtendimentoResposta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
