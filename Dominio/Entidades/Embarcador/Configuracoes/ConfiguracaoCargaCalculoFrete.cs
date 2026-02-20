namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CARGA_CALCULO_FRETE", EntityName = "ConfiguracaoCargaCalculoFrete", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoCargaCalculoFrete", NameType = typeof(ConfiguracaoCargaCalculoFrete))]
    public class ConfiguracaoCargaCalculoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALOR_MAXIMO_CALCULO_FRETE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMaximoCalculoFrete { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração geral de cálculo de frete";
            }
        }
    }
}
