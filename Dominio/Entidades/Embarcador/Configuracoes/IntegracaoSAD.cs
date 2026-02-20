namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_SAD", EntityName = "IntegracaoSAD", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD", NameType = typeof(IntegracaoSAD))]
    public class IntegracaoSAD : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroDescarregamento CentroDescarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_TOKEN", Type = "StringClob", NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_SAD", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSADBuscarSenha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_SAD_FINALIZAR_AGENDA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSADFinalizarAgenda { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_SAD_CANCELAR_AGENDA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSADCancelarAgenda { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CentroDescarregamento.Descricao;
            }
        }
    }
}
