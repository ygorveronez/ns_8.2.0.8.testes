using System;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_REQUISICAO_MERCADORIA", EntityName = "AprovacaoAlcadaRequisicaoMercadoria", Name = "Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria", NameType = typeof(AprovacaoAlcadaRequisicaoMercadoria))]
    public class AprovacaoAlcadaRequisicaoMercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RequisicaoMercadoria", Column = "RME_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria RequisicaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasRequisicaoMercadoria", Column = "RRM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria RegraRequisicaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DELEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Delegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.RequisicaoMercadoria?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }
    }

}