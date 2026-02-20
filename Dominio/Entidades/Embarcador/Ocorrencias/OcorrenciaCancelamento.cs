using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CANCELAMENTO", EntityName = "OcorrenciaCancelamento", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento", NameType = typeof(OcorrenciaCancelamento))]
    public class OcorrenciaCancelamento : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoCancelamento { get; set; }

        /// <summary>
        /// Quando uma carga é cancelada esse campo registra em que situação ela estava quando foi cancelada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_SITUACAO_NO_CANCELAMENTO", TypeType = typeof(SituacaoOcorrencia), NotNull = true)]
        public virtual SituacaoOcorrencia SituacaoOcorrenciaNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_SITUACAO", TypeType = typeof(SituacaoCancelamentoOcorrencia), NotNull = true)]
        public virtual SituacaoCancelamentoOcorrencia Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_TIPO", TypeType = typeof(TipoCancelamentoOcorrencia), NotNull = false)]
        public virtual TipoCancelamentoOcorrencia Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_REJEICAO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemRejeicaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_ENVIOU_CTES_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouCTesParaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_LIBERAR_CANCEALMENTO_COM_CTE_NAO_INUTILIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComCTeNaoInutilizado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CANCELAMENTO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaCancelamentoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<OcorrenciaCancelamentoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAO_INTEGROU_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrouTransportador { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case TipoCancelamentoOcorrencia.Anulacao:
                        return "Anulação";
                    case TipoCancelamentoOcorrencia.Cancelamento:
                        return "Cancelamento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Ocorrencia?.Descricao ?? string.Empty;
            }
        }
    }
}
