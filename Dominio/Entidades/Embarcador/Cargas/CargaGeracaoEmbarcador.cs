using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_GERACAO_EMBARCADOR", EntityName = "CargaGeracaoEmbarcador", Name = "Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador", NameType = typeof(CargaGeracaoEmbarcador))]
    public class CargaGeracaoEmbarcador: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Tracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_GERACAO_EMBARCADOR_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_GERACAO_EMBARCADOR_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_GERACAO_EMBARCADOR_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_GERACAO_EMBARCADOR_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Carga?.CodigoCargaEmbarcador ?? string.Empty;
            }
        }
    }
}
