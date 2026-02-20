namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGO_PESSOA", EntityName = "Cargo", Name = "Dominio.Entidades.Embarcador.Pessoas.Cargo", NameType = typeof(Cargo))]
    public class Cargo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CRG_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFaturamento", Column = "CRG_VALOR_FATURAMENTO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ValorFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBonificacao", Column = "CRG_VALOR_BONIFICACAO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ValorBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRG_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComissaoPadrao", Column = "CRG_COMISSAO_PADRAO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ComissaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaEquivalente", Column = "CRG_MEDIA_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal MediaEquivalente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SinistroEquivalente", Column = "CRG_SINISTRO_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal SinistroEquivalente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdvertenciaEquivalente", Column = "CRG_ADVERTENCIA_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal AdvertenciaEquivalente { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo; }
        }
    }
}
