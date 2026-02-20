using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO_AUTORIZACAO", EntityName = "CotacaoPedidoAutorizacao", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao", NameType = typeof(CotacaoPedidoAutorizacao))]
    public class CotacaoPedidoAutorizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CPA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "CPA_MOTIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_ETAPA_AUTORIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoPedido CotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasCotacaoPedido", Column = "RCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasCotacaoPedido RegrasCotacaoPedido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }

        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoEtapaAutorizacaoOcorrencia
        {
            get
            {
                switch (EtapaAutorizacaoOcorrencia)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia:
                        return "Aprovação do Pedido";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia:
                        return "Emissão do Pedido";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(CotacaoPedidoAutorizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
