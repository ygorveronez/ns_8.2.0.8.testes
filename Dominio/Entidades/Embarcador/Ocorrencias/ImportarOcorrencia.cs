using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTAR_OCORRENCIA", DynamicUpdate = true, EntityName = "ImportarOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia", NameType = typeof(ImportarOcorrencia))]
    public class ImportarOcorrencia : EntidadeBase, IEquatable<ImportarOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTe", Column = "IMO_NUMERO_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCTe", Column = "IMO_SERIE_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOcorrencia", Column = "IMO_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "IMO_OBSERVACAO", Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoImpressa", Column = "IMO_OBSERVACAO_IMPRESSA", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoImpressa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Booking", Column = "IMO_NUMERO_BOOKING", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Booking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemServico", Column = "IMO_NUMERO_ORDEM_SERVICO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarga", Column = "IMO_NUMERO_CARGA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoOcorrencia", Column = "IMO_CODIGO_TIPO_OCORRENCIA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string CodigoTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoComponenteFrete", Column = "IMO_CODIGO_COMPONENTE_FRETE", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string CodigoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOcorrencia", Column = "IMO_VALOR_OCORRENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "IMO_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoImportacao", Column = "IMO_RETORNO_IMPORTACAO", Type = "StringClob", NotNull = false)]
        public virtual string RetornoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "IMO_NOME_ARQUIVO", Type = "StringClob", NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoImportarOcorrencia", Column = "COC_SITUACAO_OCORRENCIA", TypeType = typeof(SituacaoImportarOcorrencia), NotNull = false)]
        public virtual SituacaoImportarOcorrencia SituacaoImportarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "IMO_CST", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string CST { get; set; }

        public virtual string Descricao
        {
            get { return this.Codigo.ToString(); }
        }

        public virtual bool Equals(ImportarOcorrencia other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
