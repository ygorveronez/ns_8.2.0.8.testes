using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DOCUMENTO", EntityName = "GestaoDocumento", Name = "Dominio.Entidades.Embarcador.Documentos.GestaoDocumento", NameType = typeof(GestaoDocumento))]
    public class GestaoDocumento : EntidadeBase, IEntidade
    {
        /// <summary>
        /// No banco está como System.Int64, foi alterado somente aqui para usar as alçadas padrão #Fernando
        /// </summary>
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTeComplementoInfo CargaCTeComplementoInfo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoGestaoDocumento", Column = "GED_SITUACAO_GESTAO_DOCUMENTO", TypeType = typeof(SituacaoGestaoDocumento), NotNull = true)]
        public virtual SituacaoGestaoDocumento SituacaoGestaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoInconsistenciaGestaoDocumento", Column = "GED_MOTIVO_INCONSISTENCIA_GESTAO_DOCUMENTO", TypeType = typeof(MotivoInconsistenciaGestaoDocumento), NotNull = true)]
        public virtual MotivoInconsistenciaGestaoDocumento MotivoInconsistenciaGestaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAprovacao", Column = "GED_OBSERVACAO_APROVACAO", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string ObservacaoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalhesInconsistencia", Column = "GED_DETALHES_INCONSISTENCIA", Type = "StringClob", NotNull = false)]
        public virtual string DetalhesInconsistencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "GED_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = true, Scale = 2, Precision = 18)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFeRecebida", Column = "GED_NUMERO_NFE_RECEBIDA", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string NFeRecebida { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeImportacoesCTe", Column = "GED_QUANTIDADE_IMPORTACOES_CTE", TypeType = typeof(int), NotNull = false)]
		public virtual int QuantidadeImportacoesCTe { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacaoCTe", Column = "GED_DATA_IMPORTACAO_CTE", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataImportacaoCTe { get; set; }

		public virtual string Descricao
        {
            get { return CTe.Numero.ToString(); }
        }

        public virtual PreConhecimentoDeTransporteEletronico PreCTe
        {
            get
            {
                if (this.CargaCTe != null)
                    return this.CargaCTe.PreCTe;

                return this.CargaCTeComplementoInfo?.PreCTe;
            }
        }

        public virtual Cargas.Carga Carga
        {
            get
            {
                if (this.CargaCTe != null)
                    return this.CargaCTe.Carga;

                return this.CargaCTeComplementoInfo?.CargaOcorrencia?.Carga;
            }
        }

        public virtual bool SituacaoGestaoDocumentoAprovada
        {
            get
            {
                return (SituacaoGestaoDocumento == SituacaoGestaoDocumento.Aprovado) || (SituacaoGestaoDocumento == SituacaoGestaoDocumento.AprovadoComDesconto);
            }
        }
    }
}
