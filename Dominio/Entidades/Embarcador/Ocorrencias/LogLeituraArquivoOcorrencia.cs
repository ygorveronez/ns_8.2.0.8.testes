using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_LEITURA_ARQUIVO_OCORRENCIA", EntityName = "LogLeituraArquivoOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia", NameType = typeof(LogLeituraArquivoOcorrencia))]
    public class LogLeituraArquivoOcorrencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LLO_NOME_ARQUIVO", TypeType = typeof(string), NotNull = true, Length = 250)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LLO_GUID_ARQUIVO", TypeType = typeof(string), NotNull = true, Length = 50)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LLO_OCORRENCIAS_GERADAS", TypeType = typeof(string), NotNull = true, Length = 250)]
        public virtual string OcorrenciasGeradas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LLO_MOTIVO_INCONSISTENCIA", TypeType = typeof(string), NotNull = true, Length = 500)]
        public virtual string MotivoInconsistencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "COC_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvioArquivo", Column = "LLO_TIPO_ENVIO_ARQUIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioArquivo TipoEnvioArquivo { get; set; }

        public virtual bool Equals(LogLeituraArquivoOcorrencia other)
        {
            return (this.Codigo == other.Codigo);
        }

        public virtual string Descricao
        {
            get
            {
                return this.NomeArquivo;
            }
        }
    }
}
