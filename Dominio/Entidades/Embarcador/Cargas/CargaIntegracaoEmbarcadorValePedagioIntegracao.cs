using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_VALE_PEDAGIO_INTEGRACAO", EntityName = "CargaIntegracaoEmbarcadorValePedagioIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorValePedagioIntegracao", NameType = typeof(CargaIntegracaoEmbarcadorValePedagioIntegracao))]
    public class CargaIntegracaoEmbarcadorValePedagioIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagioEmbarcador", Column = "CVI_CODIGO_INTEGRACAO_VALE_PEDAGIO_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracaoValePedagioEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoEmbarcador", Column = "CIE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador CargaIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoValePedagio", Column = "CIV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio CargaIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoValePedagio", Column = "CVI_SITUACAO_VALE_PEDAGIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio), NotNull = true)]
        public virtual SituacaoValePedagio SituacaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroValePedagio", Column = "CVI_NUMERO_VALE_PEDAGIO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdCompraValePedagio", Column = "CVI_ID_COMPRA_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdCompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "CVI_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao1", Column = "CVI_OBSERVACAO_1", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao2", Column = "CVI_OBSERVACAO_2", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao3", Column = "CVI_OBSERVACAO_3", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao4", Column = "CVI_OBSERVACAO_4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao5", Column = "CVI_OBSERVACAO_5", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao5 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao6", Column = "CVI_OBSERVACAO_6", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao6 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaTemporaria", Column = "CVI_ROTA_TEMPORARIA", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string RotaTemporaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagio", Column = "CVI_CODIGO_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CVI_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCompra", Column = "CVI_TIPO_COMPRA", TypeType = typeof(Dominio.Enumeradores.TipoCompraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVI_VALE_PEDAGIO_RETORNO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CompraComEixosSuspensos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRoteiro", Column = "CVI_CODIGO_ROTEIRO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoRoteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPercurso", Column = "CVI_CODIGO_PERCURSO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "CVI_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecebidoPorIntegracao", Column = "CVI_RECEBIDO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecebidoPorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVI_VALIDACAO_COMPRA_REMOVEU_COMPONENTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidaCompraRemoveuComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTransportador", Column = "CVI_NOME_TRANSPORTADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NomeTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPercursoVP", Column = "CVI_TIPO_PERCURSO_VP", TypeType = typeof(TipoRotaFrete), NotNull = false)]
        public virtual TipoRotaFrete? TipoPercursoVP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjMeioPagamento", Column = "CVI_CNPJ_MEIO_PAGAMENTO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CnpjMeioPagamento { get; set; }
    }
}