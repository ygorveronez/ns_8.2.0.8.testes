namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TAXA_DESCARGA", EntityName = "ConfiguracaoTaxaDescarga", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga", NameType = typeof(ConfiguracaoTaxaDescarga))]
    public class ConfiguracaoTaxaDescarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativa", Column = "CTD_ATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativa { get; set; }

        public virtual string DescricaoAtiva
        {
            get
            {
                if (this.Ativa)
                    return "Ativa";
                else
                    return "Inativa";
            }
        }
    }
}
