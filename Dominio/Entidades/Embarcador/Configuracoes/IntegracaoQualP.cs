namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_QUALP", EntityName = "IntegracaoQualP", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP", NameType = typeof(IntegracaoQualP))]
    public class IntegracaoQualP : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracao", Column = "CIQ_URL", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string UrlIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIQ_TOKEN", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao1", Column = "CIQ_OBSERVACAO_1", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIQ_DISTANCIA_MINIMA_QUADRANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaMinimaQuadrante { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração QualP";
            }
        }
    }
}
