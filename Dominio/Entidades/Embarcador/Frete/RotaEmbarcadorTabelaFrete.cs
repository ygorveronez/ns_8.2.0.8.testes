namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ROTA_EMBARCADOR", EntityName = "RotaEmbarcadorTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete", NameType = typeof(RotaEmbarcadorTabelaFrete))]
    public class RotaEmbarcadorTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixoRota", Column = "TFR_VALOR_FIXO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorFixoRota { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdicionalFixoPorRota", Column = "TFR_VALOR_ADICIONAL_FIXO_POR_ROTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValorAdicionalFixoPorRota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.RotaFrete.Descricao;
            }
        }

    }
}
