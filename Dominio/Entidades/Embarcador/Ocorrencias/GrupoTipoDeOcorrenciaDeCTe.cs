namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_TIPO_OCORRENCIA", EntityName = "GrupoTipoDeOcorrenciaDeCTe", Name = "Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe", NameType = typeof(GrupoTipoDeOcorrenciaDeCTe))]
    public class GrupoTipoDeOcorrenciaDeCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GTO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GTO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "GTO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        #region Propriedades Virtuais
        
        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        #endregion
    }
}
