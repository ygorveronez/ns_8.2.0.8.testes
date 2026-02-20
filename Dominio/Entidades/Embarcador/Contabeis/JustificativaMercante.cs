namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA_MERCANTE", EntityName = "JustificativaMercante", Name = "Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante", NameType = typeof(JustificativaMercante))]

    public class JustificativaMercante : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JME_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "JME_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

    }
}
