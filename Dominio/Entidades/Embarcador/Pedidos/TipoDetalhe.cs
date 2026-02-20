namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DETALHE", EntityName = "TipoDetalhe", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe", NameType = typeof(TipoDetalhe))]
    public class TipoDetalhe : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TDE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TDE_DESCRICAO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TDE_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPaleteCliente", Column = "TDE_TIPO_PALETE_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente? TipoPaleteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TDE_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? Valor { get; set; }
    }
}
