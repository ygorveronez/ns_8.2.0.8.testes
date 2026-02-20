namespace Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_CAMPO", EntityName = "TipoOperacaoCampo", Name = "Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo", NameType = typeof(TipoOperacaoCampo))]
    public class TipoOperacaoCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TOC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

    }
}
