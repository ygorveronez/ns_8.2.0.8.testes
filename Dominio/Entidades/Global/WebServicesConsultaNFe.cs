using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_WS_CONSULTA_NFE", EntityName = "WebServicesConsultaNFe", Name = "Dominio.Entidades.WebServicesConsultaNFe", NameType = typeof(WebServicesConsultaNFe))]
    public class WebServicesConsultaNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "WCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WebService", Column = "WCN_WEBSERVICE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Consultas", Column = "WCN_CONSULTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Consultas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBloqueio", Column = "WCN_DATA_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "WCN_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita Tipo { get; set; }
    }
}
