using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_DIARIA", EntityName = "AcertoDiaria", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoDiaria", NameType = typeof(AcertoDiaria))]
    public class AcertoDiaria : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaDiariaPeriodo", Column = "TDP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo TabelaDiariaPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ACD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancadoManualmente", Column = "ACD_LANCADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ACD_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoColaborador", Column = "ACD_SITUACAO_COLABORADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador? SituacaoColaborador { get; set; }

        public virtual bool Equals(AcertoDiaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
