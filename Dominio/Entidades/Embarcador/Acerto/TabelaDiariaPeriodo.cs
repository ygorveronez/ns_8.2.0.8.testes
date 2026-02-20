using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_DIARIA_PERIODO", EntityName = "TabelaDiariaPeriodo", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo", NameType = typeof(TabelaDiariaPeriodo))]
    public class TabelaDiariaPeriodo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TDP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaDiaria", Column = "TAD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaDiaria TabelaDiaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDP_HORA_INCIAL", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan ?HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDP_HORA_FINAL", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan ?HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual bool Equals(TabelaDiariaPeriodo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}