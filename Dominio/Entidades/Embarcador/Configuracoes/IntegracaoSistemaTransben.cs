namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SISTEMA_TRANSBEN", EntityName = "IntegracaoSistemaTransben", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben", NameType = typeof(IntegracaoSistemaTransben))]

    public class IntegracaoSistemaTransben : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CST_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CST_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CST_ENVIAR_DADOS_CARGA_PARA_SISTEMA_TRANSBEN", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EnviarDadosCargaParaSistemaTransben { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLSistemaTransben", Column = "CST_URL_SISTEMA_TRANSBEN", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLSistemaTransben { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CST_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CST_SENHA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Senha { get; set; }

    }
}
