using System;

namespace Dominio.Entidades.Embarcador.Login
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TENTATIVA_LOGIN_MOTORISTA", EntityName = "TentativaLoginMotorista", DynamicUpdate = false, Name = "Dominio.Entidades.Embarcador.Login.TentativaLoginMotorista", NameType = typeof(TentativaLoginMotorista))]
    public class TentativaLoginMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TLM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCarga", Column = "TLM_CODIGO_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMotorista", Column = "TLM_CODIGO_MOTORISTA", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "TLM_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }
    }
}
