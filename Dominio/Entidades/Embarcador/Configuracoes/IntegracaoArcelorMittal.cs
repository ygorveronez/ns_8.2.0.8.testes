using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ARCELORMITTAL", EntityName = "IntegracaoArcelorMittal", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal", NameType = typeof(IntegracaoArcelorMittal))]
    public class IntegracaoArcelorMittal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLOcorrencia", Column = "CIA_URL_OCORRENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIG_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [Obsolete("Campo usado somente em outras configurações de integração.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [Obsolete("Campo usado somente em outras configurações de integração.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKey", Column = "CIG_API_KEY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ApiKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIA_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIA_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLConfirmarAvancoTransporte", Column = "CIA_URL_CONFIRMAR_AVANCO_TRANSPORTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConfirmarAvancoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLDadosTransporteSAP", Column = "CIA_URL_DADOS_TRANSPORTE_SAP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLDadosTransporteSAP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAtualizarNFeAprovada", Column = "CIA_URL_ATUALIZAR_NFE_APROVADA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAtualizarNFeAprovada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRetornoAdicionarPedidoEmLote", Column = "CIA_URL_RETORNO_ADICIONAR_PEDIDO_EM_LOTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRetornoAdicionarPedidoEmLote { get; set; }

    }
}
