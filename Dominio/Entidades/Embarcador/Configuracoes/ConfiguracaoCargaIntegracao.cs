namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CARGA_INTEGRACAO", EntityName = "ConfiguracaoCargaIntegracao", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoCargaIntegracao", NameType = typeof(ConfiguracaoCargaIntegracao))]
    public class ConfiguracaoCargaIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Se estiver ativada, essa flag impede que o cadastro da Carga seja cancelada pelos motivos de
        /// não haver um produto ou grupo de produto.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCI_ACEITAR_PEDIDOS_COM_PENDENCIA_DE_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AceitarPedidosComPendenciasDeProdutos { get; set; }

        /// <summary>
        /// Não retorna os documentos anteriores por Web Service.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCI_NAO_RETORNAR_DOCUMENTOS_ANTERIORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarDocumentosAnteriores { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para dados transporte da carga";
            }
        }
    }
}
