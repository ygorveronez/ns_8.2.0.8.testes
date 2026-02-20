using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INDICADOR_INTEGRACAO_NFE", EntityName = "IndicadorIntegracaoNFe", Name = "Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe", NameType = typeof(IndicadorIntegracaoNFe))]
    public class IndicadorIntegracaoNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "IIN_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailRemetente", Column = "IIN_EMAIL_REMETENTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmailRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicao", Column = "IIN_MOTIVO_REJEICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarga", Column = "IIN_NUMERO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "IIN_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IIN_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe Situacao { get; set; }
    }
}
