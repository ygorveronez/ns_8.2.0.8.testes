using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CONTRATO_FRETE", EntityName = "AprovacaoAlcadaContratoFreteTransportador", Name = "Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador", NameType = typeof(AprovacaoAlcadaContratoFreteTransportador))]
    public class AprovacaoAlcadaContratoFreteTransportador : EntidadeBase
    {
        public AprovacaoAlcadaContratoFreteTransportador() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraContratoFreteTransportador", Column = "RCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraContratoFreteTransportador RegraContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_SITUACAO", TypeType = typeof(SituacaoAlcadaRegra), NotNull = true)]
        public virtual SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DELEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Delegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ContratoFreteTransportador?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }
    }
}
