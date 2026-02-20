using System;

namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_PAGAMENTO_AGREGADO", EntityName = "AprovacaoAlcadaPagamentoAgregado", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado", NameType = typeof(AprovacaoAlcadaPagamentoAgregado))]
    public class AprovacaoAlcadaPagamentoAgregado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoAgregado PagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPagamentoAgregado", Column = "RPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraPagamentoAgregado RegraPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAP_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAP_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.PagamentoAgregado?.Descricao ?? string.Empty;
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
