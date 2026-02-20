namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_ADVERTENCIA_TRANSPORTADOR", EntityName = "MotivoAdvertenciaTransportador", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador", NameType = typeof(MotivoAdvertenciaTransportador))]
    public class MotivoAdvertenciaTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MAT_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MAT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MAT_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "MAT_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Pontuacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoAdvertenciaTransportador other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
