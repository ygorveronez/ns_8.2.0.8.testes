namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESCARTE_LOTE_PRODUTO_EMBARCADOR_ANEXO", EntityName = "DescarteLoteProdutoEmbarcadorAnexo", Name = "Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo", NameType = typeof(DescarteLoteProdutoEmbarcadorAnexo))]
    public class DescarteLoteProdutoEmbarcadorAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "DescarteLoteProdutoEmbarcador", Column = "DPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador Lote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPA_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPA_ATIVO", TypeType = typeof(bool), NotNull = false)]
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
