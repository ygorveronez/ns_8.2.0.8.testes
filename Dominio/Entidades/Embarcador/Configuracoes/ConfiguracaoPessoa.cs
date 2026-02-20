using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PESSOA", EntityName = "ConfiguracaoPessoa", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa", NameType = typeof(ConfiguracaoPessoa))]
    public class ConfiguracaoPessoa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Não é mais usado pois, foi criado o NaoEnviarXMLCTEPorEmailParaTipoServico")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_NAO_PERMITE_ENVIAR_XML_POR_EMAIL_QUANDO_TIPO_SERVICO_FOR_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteEnviarXMLPorEmailQuandoTipoServicoForSubcontracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_PERMITIR_CADASTRO_DE_TELEFONE_INTERNACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCadastroDeTelefoneInternacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_EXIGE_QUE_SUAS_ENTREGAS_SEJAM_AGENDADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeQueSuasEntregasSejamAgendadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_NAO_ENVIAR_XML_CTE_POR_EMAIL_PARA_TIPO_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarXMLCTEPorEmailParaTipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_NAO_EXIGIR_TROCA_DE_SENHA_CASO_CADASTRO_POR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposServicosCTe", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_TIPO_SERVICOS_CTE_ENVIO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CCP_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual ICollection<Dominio.Enumeradores.TipoServico> TiposServicosCTe { get; set; }
    }
}