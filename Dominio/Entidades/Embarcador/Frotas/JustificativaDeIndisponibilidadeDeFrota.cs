namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA_INDISPONIBILIDADE_FROTA", EntityName = "JustificativaDeIndisponibilidadeDeFrota", Name = "Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota", NameType = typeof(JustificativaDeIndisponibilidadeDeFrota))]
    public class JustificativaDeIndisponibilidadeDeFrota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JIF_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "JIF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return string.Empty;
                }
            }
        }
    }
}
