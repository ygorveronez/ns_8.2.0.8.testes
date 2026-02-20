using System;

namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VERSAO_APLICACAO", DynamicUpdate = true, EntityName = "VersaoAplicacao", Name = "Dominio.Entidades.VersaoAplicacao", NameType = typeof(VersaoAplicacao))]
    public class VersaoAplicacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCliente", Column = "VEA_CODIGO_CLIENTE", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoCliente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ambiente", Column = "VEA_AMBIENTE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Ambiente { get; set; }
        
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoEmissaoCTE", Column = "VEA_VERSAO_EMISSAOCTE", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoEmissaoCTE { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoEmissaoCTEWebAdmin", Column = "VEA_VERSAO_EMISSAOCTE_WEB_ADMIN", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoEmissaoCTEWebAdmin { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoEmissaoCTEInternalWebAdmin", Column = "VEA_VERSAO_EMISSAOCTE_INTERNAL_WEB_ADMIN", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoEmissaoCTEInternalWebAdmin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoReportApi", Column = "VEA_VERSAO_REPORT_API", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoReportApi { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoSGTWebAdmin", Column = "VEA_VERSAO_SGT_WEB_ADMIN", TypeType = typeof(string), NotNull = false)]
        public virtual string VersaoSGTWebAdmin { get; set; }
    }
}


