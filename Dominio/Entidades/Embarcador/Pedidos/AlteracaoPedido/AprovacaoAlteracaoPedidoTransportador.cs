using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALTERACAO_PEDIDO_TRANSPORTADOR", EntityName = "AprovacaoAlteracaoPedidoTransportador", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador", NameType = typeof(AprovacaoAlteracaoPedidoTransportador))]
    public class AprovacaoAlteracaoPedidoTransportador : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APA_SITUACAO", TypeType = typeof(SituacaoAlcadaRegra), NotNull = true)]
        public virtual SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedido", Column = "ALP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AlteracaoPedido AlteracaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual bool IsPermitirAprovacaoOuReprovacao(int codigoEmpresa)
        {
            return (Transportador.Codigo == codigoEmpresa) && (Situacao == SituacaoAlcadaRegra.Pendente);
        }

        #endregion
    }
}
