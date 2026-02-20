namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPOS_CAUSADORES_OCORRENCIA", EntityName = "TiposCausadoresOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia", NameType = typeof(TiposCausadoresOcorrencia))]
    public class TiposCausadoresOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPO_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TPO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TPO_ATIVO", TypeType = typeof(bool), NotNull = true)]
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
