using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OBRAMAX_CTE", EntityName = "IntegracaoObramaxCTE", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE", NameType = typeof(IntegracaoObramaxCTE))]
    public class IntegracaoObramaxCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoObramaxCTE", Column = "CIC_POSSUI_INTEGRACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoObramaxCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointObramaxCTE", Column = "CIC_ENDPOINT_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointObramaxCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenObramaxCTE", Column = "CIC_TOKEN_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenObramaxCTE { get; set; }

        [Obsolete("Nao Utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointPedidoOcorrenciaObramaxCTE", Column = "CIC_ENDPOINT_PEDIDO_OCORRENCIA_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointPedidoOcorrenciaObramaxCTE { get; set; }
    }
}
