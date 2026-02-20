using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_EMBARCADOR", EntityName = "CargaIntegracaoEmbarcador", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador", NameType = typeof(CargaIntegracaoEmbarcador))]
    public class CargaIntegracaoEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_PROTOCOLO", TypeType = typeof(int), NotNull = true)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NUMERO_CARGA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DATA_CRIACAO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_ORIGEM", Type = "StringClob", NotNull = false)]
        public virtual string Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DESTINO", Type = "StringClob", NotNull = false)]
        public virtual string Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_PROTOCOLO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ProtocoloCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MDFes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFes { get; set; }

        public virtual string Descricao { get { return this.NumeroCarga; } }

    }
}
