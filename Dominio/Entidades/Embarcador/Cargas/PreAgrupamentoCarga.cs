using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_AGRUPAMENTO", EntityName = "PreAgrupamentoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga", NameType = typeof(PreAgrupamentoCarga))]
    public class PreAgrupamentoCarga : EntidadeBase, IEquatable<PreAgrupamentoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PAC_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamento", Column = "PAC_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "PAC_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoViagem", Column = "PAC_CODIGO_VIAGEM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjEmitente", Column = "PAC_CNPJ_EMITENTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CnpjEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNota", Column = "PAC_NUMERO_NOTA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoEncaixe", Column = "PAC_PEDIDO_ENCAIXE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoEncaixe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoReentrega", Column = "PAC_PEDIDO_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoViagem", Column = "PAC_TIPO_VIAGEM", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string TipoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreAgrupamentoCargaAgrupador", Column = "PAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreAgrupamentoCargaAgrupador Agrupador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_REDESPACHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjRecebedor", Column = "PAR_CNPJ_RECEBEDOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CnpjRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjExpedidor", Column = "PAR_CNPJ_EXPEDIDOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CnpjExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedido CargaPedidoEncaixe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoInicioViagem", Column = "PAC_DATA_PREVISAO_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoInicioViagem { get; set; }

        public virtual string Descricao
        {
            get { return $"Item do Agrupamento {CodigoViagem}"; }
        }

        public virtual bool Equals(PreAgrupamentoCarga other)
        {
            return (this.Codigo == other.Codigo);
        }

        public virtual string ObterCorLinha()
        {
            if (this.Carga?.PendenteGerarCargaDistribuidor ?? false)
                return "#FFFACD";

            if (Agrupador.Situacao == SituacaoPreAgrupamentoCarga.ProblemaCarregamento)
                return "#ff9999";

            if (Carga != null)
            {
                if (Carga.CargaDePreCarga)
                    return "#7b9a78";
                else
                    return "#85de7b";
            }


            if (CargaRedespacho != null)
                return "#cce6ff";

            return "";
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga)this.MemberwiseClone();
        }
    }
}
