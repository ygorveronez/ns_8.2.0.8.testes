namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DEPOSITO_POSICAO", EntityName = "DepositoPosicao", Name = "Dominio.Entidades.Embarcador.WMS.DepositoPosicao", NameType = typeof(DepositoPosicao))]
    public class DepositoPosicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "DepositoBloco", Column = "DEB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.DepositoBloco Bloco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_ABREVIACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Abreviacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_QUANTIDADE_PALETES", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_METRO_CUBICO_MAXIMO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubicoMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPO_PESO_MAXIMO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoMaximo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoAtual", Formula = @"(SELECT ISNULL(SUM(P.PEL_PESO), 0) * ISNULL(SUM(P.PEL_QUANTIDADE_ATUAL), 0) FROM T_PRODUTO_EMBARCADOR_LOTE P WHERE P.DPO_CODIGO = DPO_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal PesoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePaletesAtual", Formula = @"(SELECT ISNULL(SUM(P.PEL_QUANTIDADE_PALET), 0) * ISNULL(SUM(P.PEL_QUANTIDADE_ATUAL), 0) FROM T_PRODUTO_EMBARCADOR_LOTE P WHERE P.DPO_CODIGO = DPO_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal QuantidadePaletesAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubicoAtual", Formula = @"(SELECT ISNULL(SUM(P.PEL_METRO_CUBICO), 0) * ISNULL(SUM(P.PEL_QUANTIDADE_ATUAL), 0) FROM T_PRODUTO_EMBARCADOR_LOTE P WHERE P.DPO_CODIGO = DPO_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal MetroCubicoAtual { get; set; }

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
