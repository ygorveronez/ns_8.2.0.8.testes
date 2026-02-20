namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ROTA", EntityName = "TabelaFreteRota", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteRota", NameType = typeof(TabelaFreteRota))]
    public class TabelaFreteRota :EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmbarcador", Column = "TFR_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoDestinos", Column = "TBF_DESCRICAO_DESTINOS", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string DescricaoDestinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TFR_ATIVO", TypeType = typeof(bool), NotNull = true)]
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
