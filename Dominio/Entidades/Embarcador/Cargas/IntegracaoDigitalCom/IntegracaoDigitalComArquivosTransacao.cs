using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_DIGITAL_COM_ARQUIVOS_TRANSACAO", EntityName = "IntegracaoDigitalComArquivosTransacao", Name = "Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao", NameType = typeof(IntegracaoDigitalComArquivosTransacao))]
    public class IntegracaoDigitalComArquivosTransacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCTeIntegracaoArquivo CargaCTeIntegracaoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MeioPagamentoDigitalCom", Column = "CDI_MEIO_PAGAMENTO_DIGITALCOM", TypeType = typeof(MeiosPagamentoDigitalCom), NotNull = false)]
        public virtual MeiosPagamentoDigitalCom? MeioPagamentoDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDRetornoDigitalCom", Column = "CDI_ID_RETORNO_DIGITALCOM", TypeType = typeof(int), NotNull = false)]
        public virtual int IDRetornoDigitalCom { get; set; }
    }
}
