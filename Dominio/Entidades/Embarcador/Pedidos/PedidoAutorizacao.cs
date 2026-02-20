using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_AUTORIZACAO", EntityName = "PedidoAutorizacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao", NameType = typeof(PedidoAutorizacao))]
    public class PedidoAutorizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PEA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "PEA_MOTIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEA_SITUACAO", TypeType = typeof(SituacaoOcorrenciaAutorizacao), NotNull = false)]
        public virtual SituacaoOcorrenciaAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEA_ETAPA_AUTORIZACAO", TypeType = typeof(EtapaAutorizacaoOcorrencia), NotNull = false)]
        public virtual EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasPedido", Column = "RPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasPedido RegrasPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoPedido", Column = "PM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoPedido MotivoPedido { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case SituacaoOcorrenciaAutorizacao.Aprovada:
                        return "Aprovada";
                    case SituacaoOcorrenciaAutorizacao.Pendente:
                        return "Pendente";
                    case SituacaoOcorrenciaAutorizacao.Rejeitada:
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
                    case EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia:
                        return "Aprovação do Pedido";
                    case EtapaAutorizacaoOcorrencia.EmissaoOcorrencia:
                        return "Emissão do Pedido";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(PedidoAutorizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual PedidoAutorizacao Clonar()
        {
            return (PedidoAutorizacao)this.MemberwiseClone();
        }
    }
}
