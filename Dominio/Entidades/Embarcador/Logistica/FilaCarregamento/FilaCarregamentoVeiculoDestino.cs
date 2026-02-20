namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_DESTINO", EntityName = "FilaCarregamentoVeiculoDestino", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino", NameType = typeof(FilaCarregamentoVeiculoDestino))]
    public class FilaCarregamentoVeiculoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FLD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Column = "FLV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }
    }
}
