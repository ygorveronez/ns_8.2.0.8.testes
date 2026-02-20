using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONSULTA_CNPJ", EntityName = "ConsultaCNPJ", Name = "Dominio.Entidades.ConsultaCNPJ", NameType = typeof(ConsultaCNPJ))]
    public class ConsultaCNPJ : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "CCN_CNPJ", TypeType = typeof(string), Length = 14, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Estado", Column = "CCN_ESTADO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCN_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusConsultaCNPJ), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusConsultaCNPJ Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoEstadual", Column = "CCN_IE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string InscricaoEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusIE", Column = "CCN_IE_STATUS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string StatusIE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimeTributario", Column = "CCN_REGIME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RegimeTributario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "CCN_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ErroConsulta", Column = "CCN_ERRO_CONSULTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ErroConsulta { get; set; }
    }
}
