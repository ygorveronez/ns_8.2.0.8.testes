using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_AVON", EntityName = "CargaIntegracaoAvon", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon", NameType = typeof(CargaIntegracaoAvon))]
    public class CargaIntegracaoAvon : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "CIA_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMinuta", Column = "CIA_NUMERO_MINUTA", TypeType = typeof(long), NotNull = true)]
        public virtual long NumeroMinuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDocumentos", Column = "CIA_QUANTIDADE_DOCUMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalDocumentos", Column = "CIA_PESO_TOTAL_DOCUMENTOS", Scale = 6, Precision = 18, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PesoTotalDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDocumentos", Column = "CIA_VALOR_TOTAL_DOCUMENTOS", Scale = 6, Precision = 18, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CIA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_SALVA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Manual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CIA_MENSAGEM", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GUID", Column = "CIA_GUID", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string GUID { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Sucesso:
                        return "Sucesso";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.SalvandoNotasFiscais:
                        return "Salvando Notas Fiscais";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Problemas:
                        return "Problemas";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Excluida:
                        return "Excluída";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon)this.MemberwiseClone();
        }
    }
}
