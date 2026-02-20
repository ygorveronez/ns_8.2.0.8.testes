namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_KMM", EntityName = "IntegracaoKMM", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM", NameType = typeof(IntegracaoKMM))]
    public class IntegracaoKMM : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIK_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIK_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIK_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIK_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIK_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodGestao", Column = "CIK_COD_GESTAO", TypeType = typeof(int), Length = 150, NotNull = false)]
        public virtual int CodGestao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenTimeHours", Column = "CIK_TOKEN_TIME_HOURS", TypeType = typeof(int), Length = 150, NotNull = false)]
        public virtual int TokenTimeHours { get; set; }

    }
}