namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_MOTORISTA", EntityName = "MotoristaPreCTE", Name = "Dominio.Entidades.MotoristaPreCTE", NameType = typeof(MotoristaPreCTE))]
    public class MotoristaPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotorista", Column = "PMO_NOME_MOTORISTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFMotorista", Column = "PMO_CPF_MOTORISTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPFMotorista { get; set; }  
    }
}
