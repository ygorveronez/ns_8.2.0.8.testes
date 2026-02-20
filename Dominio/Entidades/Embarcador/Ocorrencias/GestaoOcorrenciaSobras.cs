namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_GESTAO_OCORRENCIA_SOBRAS", EntityName = "GestaoOcorrenciaSobras", Name = "Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras", NameType = typeof(GestaoOcorrenciaSobras))]
    public class GestaoOcorrenciaSobras : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GOS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoSobra", Column = "GOS_CODIGO_SOBRA", TypeType = typeof(string), NotNull = true)]
        public virtual string CodigoSobra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSobra", Column = "GOS_QUANTIDADE_SOBRA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeSobra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoConferencia", Column = "GOS_OBSERVACAO_CONFERENCIA", TypeType = typeof(string), NotNull = false, Length = 255)]
        public virtual string ObservacaoConferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeConferencia", Column = "GOS_QUANTIDADE_CONFERENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeConferencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaColetaEntrega", Column = "OCE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega OcorrenciaColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
