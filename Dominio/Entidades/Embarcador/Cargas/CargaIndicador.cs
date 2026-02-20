namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INDICADOR", EntityName = "CargaIndicador", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaIndicador", NameType = typeof(CargaIndicador))]
    public class CargaIndicador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaIndicadorTransportador", Column = "CIC_INDICADOR_TRANSPORTADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorTransportador CargaIndicadorTransportador { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaIndicadorVeiculoMotorista", Column = "CIC_INDICADOR_VEICULO_MOTORISTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorVeiculoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorVeiculoMotorista CargaIndicadorVeiculoMotorista { get; set; }
    }
}
