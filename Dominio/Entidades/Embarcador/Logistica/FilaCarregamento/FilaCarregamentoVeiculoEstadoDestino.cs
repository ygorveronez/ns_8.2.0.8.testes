namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_ESTADO_DESTINO", EntityName = "FilaCarregamentoVeiculoEstadoDestino", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino", NameType = typeof(FilaCarregamentoVeiculoEstadoDestino))]
    public class FilaCarregamentoVeiculoEstadoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Column = "FLV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }
    }
}
