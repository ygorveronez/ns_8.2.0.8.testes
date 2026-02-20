using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ControleColeta
{
    [Obsolete("O controle de coleta não existe mais na aplicação")]
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_COLETA", EntityName = "CargaColeta", Name = "Dominio.Entidades.Embarcador.Cargas.ControleColeta.CargaColeta", NameType = typeof(CargaColeta))]
    public class CargaColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_REJEITADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_COLETA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColetaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_COLETA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColetaReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_INICIO_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_INICIO_COLETA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioColetaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_INICIO_COLETA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioColetaReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CCO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoColeta), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoColeta Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DISTANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Distancia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoSituacao => ObjetosDeValor.Embarcador.Enumeradores.SituacaoColetaHelper.ObterDescricao(Situacao);

        public virtual string Descricao { get { return Codigo.ToString(); } }

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        public virtual int? DiferencaColeta
        {
            get
            {
                return DiffTimeMinutes(this.DataColetaPrevista, this.DataColeta);
            }
        }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COLETA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaColetaPedido", Column = "CCP_CODIGO")]
        public virtual ICollection<CargaColetaPedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COLETA_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaColetaNotaFiscal", Column = "CCF_CODIGO")]
        public virtual ICollection<CargaColetaNotaFiscal> NotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fotos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COLETA_FOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaColeta", Column = "CCF_CODIGO")]
        public virtual ICollection<CargaColetaFoto> Fotos { get; set; }

    }
}
