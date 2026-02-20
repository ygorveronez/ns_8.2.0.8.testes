namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REPONSAVEL_AVARIA", EntityName = "ResponsavelAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria", NameType = typeof(ResponsavelAvaria))]
    public class ResponsavelAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAvaria", Column = "SAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria SolicitacaoAvaria { get; set; }

        /* Lucas Mahle:
         * Não lembro pra qq eu criei essa propriedade
         */
        [NHibernate.Mapping.Attributes.Property(0, Column = "RSA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteAutorizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteAutorizacao Situacao { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.Usuario?.Descricao ?? string.Empty;
            }
        }
    }
}
