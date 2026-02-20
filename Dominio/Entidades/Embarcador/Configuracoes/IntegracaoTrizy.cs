using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_TRIZY", EntityName = "IntegracaoTrizy", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy", NameType = typeof(IntegracaoTrizy))]
    public class IntegracaoTrizy : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_URL_ENVIO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_ENVIO_CANCELAMENTO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_ENVIO_EVENTOS_PATIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioEventosPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TOKEN_ENVIO_MS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenEnvioMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_VALIDAR_INTEGRACAO_POR_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarIntegracaoPorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_INTEGRAR_APENAS_CARGAS_COM_CONTROLE_DE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarApenasCargasComControleDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_PERMITIR_INTEGRAR_MULTIPLAS_CARGAS_PARA_O_MESMO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirIntegrarMultiplasCargasParaOMesmoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_ENVIAR_PDF_DOCUMENTOS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPDFDocumentosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TIPO_DOCUMENTO_PAIS", TypeType = typeof(TipoDocumentoPaisTrizy), NotNull = false)]
        public virtual TipoDocumentoPaisTrizy TipoDocumentoPais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_DIAS_INTERVALO_TRAKING", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasIntervaloTracking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_ENVIAR_PATCH_ATUALIZACAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPatchAtualizacoesEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_ENVIAR_NOME_FANTASIA_QUANDO_POSSUIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNomeFantasiaQuandoPossuir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_VERSAO_INTEGRACAO", TypeType = typeof(VersaoIntegracaoTrizy), NotNull = false)]
        public virtual VersaoIntegracaoTrizy VersaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_INTEGRAR_OFERTAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IntegrarOfertasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_URL_INTEGRACAO_OFERTAS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string URLIntegracaoOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_URL_INTEGRACAO_GRUPO_MOTORISTAS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string URLIntegracaoGrupoMotoristas { get; set; }
        #endregion

        #region Collections
        [NHibernate.Mapping.Attributes.Set(0, Name = "DocumentosFiscaisEnvioPDF", Table = "T_CONFIGURACAO_INTEGRACAO_TRIZY_DOCUMENTOS_FISCAIS_ENVIO_PDF", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "DFT_DOCUMENTOS_FISCAIS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy> DocumentosFiscaisEnvioPDF { get; set; }
        #endregion

        #region Atributos com Regras

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Trizy";
            }
        }
        #endregion

    }
}
