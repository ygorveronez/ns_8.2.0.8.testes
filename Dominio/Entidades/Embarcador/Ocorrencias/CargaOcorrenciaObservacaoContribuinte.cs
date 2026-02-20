namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_OBSERVACAO_CONTRIBUINTE", EntityName = "CargaOcorrenciaObservacaoContribuinte", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte", NameType = typeof(CargaOcorrenciaObservacaoContribuinte))]
    public class CargaOcorrenciaObservacaoContribuinte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Texto", Column = "COO_TEXTO", TypeType = typeof(string), Length = 160, NotNull = true)]
        public virtual string Texto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOcorrencia Ocorrencia { get; set; }

        public virtual ObjetosDeValor.CTe.Observacao ObterObservacaoCTe()
        {
            return new ObjetosDeValor.CTe.Observacao()
            {
                Descricao = Texto,
                Identificador = Identificador
            };
        }
    }
}
