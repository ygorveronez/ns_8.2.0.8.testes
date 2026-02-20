namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_CONTRATO_VEICULO_ANEXO", EntityName = "OcorrenciaContratoVeiculoAnexo", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo", NameType = typeof(OcorrenciaContratoVeiculoAnexo))]
    public class OcorrenciaContratoVeiculoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaContratoVeiculo", Column = "OCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaContratoVeiculo OcorrenciaContratoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OVA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "OVA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "OVA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }
    }
}