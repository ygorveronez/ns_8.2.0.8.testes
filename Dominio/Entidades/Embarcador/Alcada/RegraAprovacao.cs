using System;

namespace Dominio.Entidades.Embarcador.Alcada
{
    public class RegraAprovacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Property(0, Column = "BRA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BRA_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BRA_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BRA_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BRA_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }
    }
}
