namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_EPI", EntityName = "TipoEPI", Name = "Dominio.Entidades.Embarcador.Pessoas.TipoEPI", NameType = typeof(TipoEPI))]
    public class TipoEPI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TEP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TEP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaEPI", Column = "MAE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaEPI MarcaEPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tamanho", Column = "TEP_TAMANHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Tamanho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasRevisao", Column = "TEP_DIAS_REVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiasRevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasValidade", Column = "TEP_DIAS_VALIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiasValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descartavel", Column = "TEP_DESCARTAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Descartavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InstrucaoUso", Column = "TEP_INSTRUCAO_USO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InstrucaoUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TEP_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCertificado", Column = "TEP_NUMERO_CERTIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Caracteristica", Column = "TEP_CARACTERISTICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Caracteristica { get; set; }

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
