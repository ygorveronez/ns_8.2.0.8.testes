namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOTORISTA", EntityName = "VeiculoMotorista", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista", NameType = typeof(VeiculoMotorista))]
    public class VeiculoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "VMT_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "VMT_CPF", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Principal", Column = "VMT_PRINCIPAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Principal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        public virtual string Descricao
        {
            get { return !string.IsNullOrWhiteSpace(Nome) ? Nome : Motorista?.Descricao ?? string.Empty; }
        }
    }
}
