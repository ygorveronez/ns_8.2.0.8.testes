namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOTORISTAS", EntityName = "VeiculoMotoristas", Name = "Dominio.Entidades.VeiculoMotoristas", NameType = typeof(VeiculoMotoristas))]
    public class VeiculoMotoristas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Veiculo?.Descricao ?? string.Empty) + " - " + (this.Motorista?.Descricao ?? string.Empty);
            }
        }
    }
}
