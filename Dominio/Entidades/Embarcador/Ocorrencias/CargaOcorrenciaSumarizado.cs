namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_SUMARIZADO", EntityName = "CargaOcorrenciaSumarizado", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado", NameType = typeof(CargaOcorrenciaSumarizado))]
    public class CargaOcorrenciaSumarizado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_VALOR_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_QUANTIDADE_CARGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_QUANTIDADE_DOCUMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocumentos{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_QUANTIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Descricao ?? string.Empty;
            }
        }
    }
}
