namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GESTAO_ENTREGA", EntityName = "ConfiguracaoGestaoEntrega", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega", NameType = typeof(ConfiguracaoGestaoEntrega))]
    public class ConfiguracaoGestaoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_OCULTAR_FLUXO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarFluxoCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Gestão Entregas";
            }
        }
    }
}
