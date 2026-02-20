namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_VALE_PEDAGIO", EntityName = "CargaIntegracaoEmbarcadorValePedagio", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagio", NameType = typeof(CargaIntegracaoEmbarcadorValePedagio))]
    public class CargaIntegracaoEmbarcadorValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoValePedagioEmbarcador", Column = "CVP_CODIGO_VALE_PEDAGIO_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoValePedagioEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoEmbarcador", Column = "CIE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador CargaIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_FORNECEDOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_CODIGO_AGENDAMENTO_PORTO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string CodigoAgendamentoPorto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagioEmbarcador", Column = "CVP_CODIGO_INTEGRACAO_VALE_PEDAGIO_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoIntegracaoValePedagioEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCompra", Column = "CVP_TIPO_COMPRA", TypeType = typeof(Dominio.Enumeradores.TipoCompraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "CVP_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncluirMDFe", Column = "CVP_NAO_INCLUIR_MDFE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoIncluirMDFe { get; set; }
    }
}