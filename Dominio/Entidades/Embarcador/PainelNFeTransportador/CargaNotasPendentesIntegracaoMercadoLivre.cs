using System;

namespace Dominio.Entidades.Embarcador.PainelNFeTransportador
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NOTAS_PENDENTES_INTEGRACAO_MERCADO_LIVRE", EntityName = "CargaNotasPendentesIntegracaoMercadoLivre", Name = "Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasIntegracaoMercadoLivre", NameType = typeof(CargaNotasPendentesIntegracaoMercadoLivre))]
    public class CargaNotasPendentesIntegracaoMercadoLivre : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNPIML_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusao", Column = "CNPIML_DATA_INCLUSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDownloadNotas", Column = "CNPIML_SITUACAO_DOWLOAD_NOTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre SituacaoDownloadNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CNPIML_MENSAGEM_RETORNO", Type = "StringClob", NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        public virtual string SituacaoDownloadNotasCarga
        {
            get
            {
                return ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivreHelper.ObterDescricao(this.SituacaoDownloadNotas);
            }
        }

        public virtual string NumeroCarga
        {
            get
            {
                return Carga.CodigoCargaEmbarcador;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return "Carga com Notas Pendentes Integração Mercado Livre";
            }
        }
    }
}
