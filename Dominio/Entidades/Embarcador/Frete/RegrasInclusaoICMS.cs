namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_INCLUSAO_ICMS", EntityName = "RegrasInclusaoICMS", Name = "Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS", NameType = typeof(RegrasInclusaoICMS))]
    public class RegrasInclusaoICMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "RFA_TIPO_PESSOA", TypeType = typeof(int), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
       public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }
       

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "RFA_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        public virtual string Descricao { get { return TipoOperacao?.Descricao; }  }

    }

}
