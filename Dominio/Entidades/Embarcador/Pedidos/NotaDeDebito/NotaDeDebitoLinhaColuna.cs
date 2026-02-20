namespace Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_NOTA_DEBITO_COLUNA", EntityName = "NotaDeDebitoLinhaColuna", Name = "Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna", NameType = typeof(NotaDeDebitoLinhaColuna))]
    public class NotaDeDebitoLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaDeDebitoLinha", Column = "NDL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinha Linha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NDC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NDC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
