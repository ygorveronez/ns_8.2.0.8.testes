namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_GADLE", EntityName = "IntegracaoGadle", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle", NameType = typeof(IntegracaoGadle))]

    public class IntegracaoGadle : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_INTEGRACAO_GADLE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoGadle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_TOKEN_INTEGRACAO_GADLE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenIntegracaoGadle { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
