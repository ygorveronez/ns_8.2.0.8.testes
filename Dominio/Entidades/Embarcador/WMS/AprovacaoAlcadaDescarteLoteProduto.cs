using System;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_DESCARTE_LOTE_PRODUTO_EMBARCADOR", EntityName = "AprovacaoAlcadaDescarteLoteProduto", Name = "Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto", NameType = typeof(AprovacaoAlcadaDescarteLoteProduto))]
    public class AprovacaoAlcadaDescarteLoteProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DescarteLoteProdutoEmbarcador", Column = "DPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador Descarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAD_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAD_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAD_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Descarte?.Descricao ?? string.Empty;
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
