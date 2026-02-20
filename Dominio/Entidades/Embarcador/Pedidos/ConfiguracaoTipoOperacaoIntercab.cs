using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_INTERCAB", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoIntercab", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntercab", NameType = typeof(ConfiguracaoTipoOperacaoIntercab))]
    public class ConfiguracaoTipoOperacaoIntercab : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_TOMADOR", TypeType = typeof(TipoTomadorCabotagem), NotNull = false)]
        public virtual TipoTomadorCabotagem Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_MODAL_PROPOSTA", TypeType = typeof(TipoModalPropostaCabotagem), NotNull = false)]
        public virtual TipoModalPropostaCabotagem ModalProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_TIPO_PROPOSTA", TypeType = typeof(TipoPropostaCabotagem), NotNull = false)]
        public virtual TipoPropostaCabotagem TipoProposta { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações Intercab"; }
        }
    }
}