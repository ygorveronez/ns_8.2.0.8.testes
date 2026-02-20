using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_PIRACANJUBA", EntityName = "IntegracaoPiracanjuba", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba", NameType = typeof(IntegracaoPiracanjuba))]
    public class IntegracaoPiracanjuba : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_URL_INTEGRACAO_CANHOTO_PIRACANJUBA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCanhotoPiracanjuba { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_URL_INTEGRACAO_CANHOTO_PIRACANJUBA_CONTINGENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCanhotoPiracanjubaContingencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFaturamentoNota", Column = "CIP_DATA_FATURAMENTO_NOTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamentoNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_URL_INTEGRACAO_CARGA_PIRACANJUBA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCargaPiracanjuba { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_AMBIENTE_PRODUCAO_PIRACANJBA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AmbienteProducaoPiracanjuba { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_STRING_AMBIENTE_PIRACANJUBA", TypeType = typeof(string), NotNull = false)]
        public virtual string StringAmbientePiracanjuba { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIP_CLIENT_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIP_CLIENT_SECRET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientSecret { get; set; }
    }
}
