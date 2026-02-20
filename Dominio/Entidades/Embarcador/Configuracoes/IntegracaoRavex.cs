namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_RAVAX", EntityName = "IntegracaoRavex", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex", NameType = typeof(IntegracaoRavex))]

    public class IntegracaoRavex : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIR_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracao", Column = "CIR_URL", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string UrlIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIR_USUARIO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIR_SENHA", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Senha { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Ravex";
            }
        }
    }
}
