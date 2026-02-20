using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMPROVANTE_CARGA", EntityName = "ComprovanteCarga", Name = "Dominio.Entidades.Embarcador.ComprovanteCarga.ComprovanteCarga", NameType = typeof(ComprovanteCarga))]
    public class ComprovanteCarga : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoComprovante", Column = "CTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_SITUACAO", TypeType = typeof(SituacaoComprovanteCarga), NotNull = true)]
        public virtual SituacaoComprovanteCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "COC_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataJustificativa", Column = "COC_DATA_JUSTIFICATIVA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_MOTIVO_JUSTIFICATIVA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NOME_ARQUIVO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        public virtual string Descricao
        {
            get { return this.TipoComprovante.Descricao + " " + this.Carga.CodigoCargaEmbarcador; }
        }
    }
}
