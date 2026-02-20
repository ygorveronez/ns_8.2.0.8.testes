using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OBRAMAX_NFE", EntityName = "IntegracaoObramaxNFE", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE", NameType = typeof(IntegracaoObramaxNFE))]
    public class IntegracaoObramaxNFE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoObramaxNFE", Column = "CIN_POSSUI_INTEGRACAO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoObramaxNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointObramaxNFE", Column = "CIN_ENDPOINT_NFE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointObramaxNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenObramaxNFE", Column = "CIN_TOKEN_NFE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenObramaxNFE { get; set; }

        [Obsolete("Nao Utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointPedidoOcorrenciaObramaxNFE", Column = "CIN_ENDPOINT_PEDIDO_OCORRENCIA_NFE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointPedidoOcorrenciaObramaxNFE { get; set; }
    }
}
