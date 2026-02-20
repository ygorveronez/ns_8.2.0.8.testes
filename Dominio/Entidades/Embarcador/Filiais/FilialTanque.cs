using System;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_TANQUE", EntityName = "FilialTanque", Name = "Dominio.Entidades.Embarcador.Filiais.FilialTanque", NameType = typeof(FilialTanque))]
    public class FilialTanque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Tanque", Column = "TAN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Tanques.Tanque Tanque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "FIT_VOLUME", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Capacidade", Column = "FIT_CAPACIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Capacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ocupacao", Column = "FIT_OCUPACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Ocupacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "FIT_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vazao", Column = "FIT_VAZAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Vazao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FIT_STATUS", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Status { get; set; }

        public virtual string Descricao { get { return Tanque.ID + " - " + Tanque.Descricao; } }
    }
}
