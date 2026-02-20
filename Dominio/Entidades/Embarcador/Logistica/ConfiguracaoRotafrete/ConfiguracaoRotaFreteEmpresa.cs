namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ROTA_FRETE_TRANSPORTADOR", EntityName = "ConfiguracaoRotaFreteEmpresa", Name = "Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa", NameType = typeof(ConfiguracaoRotaFreteEmpresa))]
    public class ConfiguracaoRotaFreteEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCargasDaRota", Column = "CRE_PERCENTUAL_CARGAS_DA_ROTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCargasDaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "CRE_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Configuração de rota de frete {ConfiguracaoRotaFrete.Descricao}";
            }
        }
    }
}
