namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENDERECO_PRODUTO", DynamicUpdate = true, EntityName = "EnderecoProduto", Name = "Dominio.Entidades.Embarcador.Produtos.EnderecoProduto", NameType = typeof(EnderecoProduto))]
    public class EnderecoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CEP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CEP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// Atributo para definir a prioridade no processo de fechamento de cargas (MontagemCarga - ASSAI)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelPrioridade", Column = "CEP_NIVEL_PRIORIDADE", TypeType = typeof(System.Int32), NotNull = false)]
        public virtual int NivelPrioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CEP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

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
    }
}
