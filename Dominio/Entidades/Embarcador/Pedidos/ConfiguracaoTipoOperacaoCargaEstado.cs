namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CARGA_ESTADO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoCargaEstado", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado", NameType = typeof(ConfiguracaoTipoOperacaoCargaEstado))]
    public class ConfiguracaoTipoOperacaoCargaEstado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoCarga", Column = "CCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoCarga Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeInformarIscaNaCargaComValorMaiorQue", Column = "CCG_EXIGE_INFORMAR_ISCA_NA_CARGA_VALOR_MAIOR_QUE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ExigeInformarIscaNaCargaComValorMaiorQue { get; set; }

        public virtual string Descricao
        {
            get { return this.Estado.Sigla + " - " + this.ExigeInformarIscaNaCargaComValorMaiorQue.ToString("c"); }
        }
    }
}
