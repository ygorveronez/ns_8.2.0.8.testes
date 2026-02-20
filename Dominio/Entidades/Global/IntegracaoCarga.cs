using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CARGA", EntityName = "IntegracaoCarga", Name = "Dominio.Entidades.IntegracaoCarga", NameType = typeof(IntegracaoCarga))]
    public class IntegracaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaCarga", Column = "ICA_NUMERO_CARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "ICA_NUMERO_UNIDADE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDaUnidade { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "ICA_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ICA_STATUS", TypeType = typeof(Enumeradores.StatusIntegracaoCarga), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracaoCarga Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCarga", Column = "ICA_CODIGO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaCancelameto", Column = "ICA_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string JustificativaCancelameto { get; set; }
    }
}
