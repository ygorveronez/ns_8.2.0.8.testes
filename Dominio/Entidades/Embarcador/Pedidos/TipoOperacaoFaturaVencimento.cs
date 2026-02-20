
namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONF_TIPO_OPERACAO_FATURA_VENC", DynamicUpdate = true, EntityName = "TipoOperacaoFaturaVencimento", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento", NameType = typeof(TipoOperacaoFaturaVencimento))]
    public class TipoOperacaoFaturaVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaInicial", Column = "OFV_DIA_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFinal", Column = "OFV_DIA_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimento", Column = "OFV_DIA_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
    }
}

