namespace Dominio.Entidades.Embarcador.Justificativas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENCERRAMENTO_MANUAL_VIAGEM", EntityName = "EncerramentoManualViagem", Name = "Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem", NameType = typeof(EncerramentoManualViagem))]
    public class EncerramentoManualViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "EMV_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "EMV_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "EMV_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual bool Equals(EncerramentoManualViagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
