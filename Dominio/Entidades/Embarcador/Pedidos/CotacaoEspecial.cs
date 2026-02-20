using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_ESPECIAL", EntityName = "CotacaoEspecial", Name = "Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial", NameType = typeof(CotacaoEspecial))]
    public class CotacaoEspecial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_COTACAO_ESPECIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pedido", Column = "PED_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "COE_TIPO_MODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModalCotacaoEspecial), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModalCotacaoEspecial? TipoModal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSolicitacao", Column = "COE_DATA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusCotacaoEspecial", Column = "COE_STATUS_COTACAO_ESPECIAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoEspecial), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoEspecial? StatusCotacaoEspecial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "COE_PESO_TOTAL_COTACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNotasFiscais", Column = "COE_VALOR_TOTAL_NOTAS_FISCAIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCotacao", Column = "COE_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial)this.MemberwiseClone();
        }

        public virtual bool Equals(CotacaoEspecial other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
