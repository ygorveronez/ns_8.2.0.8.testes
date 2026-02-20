using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_INFORMACAO_DESCARGA", EntityName = "InformacaoDescarga", Name = "Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga", NameType = typeof(InformacaoDescarga))]
    public class InformacaoDescarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "IDE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDE_HORA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan Hora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDE_EMPRESA", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaFiscal", Column = "IDE_NOTA_FISCAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "IDE_SERIE", TypeType = typeof(int), NotNull = false)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDE_PLACA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoDescarga", Column = "IDE_PESO_DESCARGA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "IDE_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataImportacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Empresa + " " + this.Placa;
            }
        }

    }
}
