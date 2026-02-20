namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLETIZACAO", EntityName = "Palletizacao", Name = "Dominio.Entidades.Embarcador.Configuracoes.Palletizacao", NameType = typeof(Palletizacao))]
    public class Palletizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_CODIGOINTEGRACAO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_MISTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PalletMisto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_TIPO_PALLETIZACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPalletizacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPalletizacao TipoPalletizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAL_TIPO_PESSOA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
