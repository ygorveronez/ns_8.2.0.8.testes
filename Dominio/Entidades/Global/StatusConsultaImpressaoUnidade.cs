using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STATUS_CONSULTA_IMPRESSAO", EntityName = "StatusConsultaImpressaoUnidade", Name = "Dominio.Entidades.StatusConsultaImpressaoUnidade", NameType = typeof(StatusConsultaImpressaoUnidade))]
    public class StatusConsultaImpressaoUnidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "SCI_NUMERO_UNIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "SCI_DOCUMENTO", TypeType = typeof(Enumeradores.TipoObjetoConsulta), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoObjetoConsulta Documento { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Consultando", Column = "SCI_CONSULTANDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Consultando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SCI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

    }
}
