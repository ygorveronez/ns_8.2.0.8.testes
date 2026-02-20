namespace Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CAMPO", EntityName = "PedidoCampo", Name = "Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo", NameType = typeof(PedidoCampo))]
    public class PedidoCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_CAMPO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Campo { get; set; }
    }
}
