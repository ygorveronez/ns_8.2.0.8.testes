using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSBORDO", EntityName = "Transbordo", Name = "Dominio.Entidades.Embarcador.Cargas.Transbordo", NameType = typeof(Transbordo))]
    public class Transbordo : EntidadeBase, IEquatable<Transbordo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TRB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTransbordo", Column = "TRB_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_GERADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_TRANSBORDO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade localidadeTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTransbordo", Column = "TRB_DATA_TRANSBORDO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTransbordo", Column = "TRB_SITUACAO_TRANSBORDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo SituacaoTransbordo { get; set; }

        /// <summary>
        /// É o grupo de pessoas para qual o veículo estava vinculado no momento do transbordo (segmento)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "TRB_CODIGO_SEGMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas SegmentoGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoTransbordo", Column = "CAR_MOTIVO_TRANSBORDO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string MotivoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSBORDO_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSBORDO_MOTORISTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "CAR_MOTORISTA")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CargaCTesTransbordados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSBORDO_CARGA_CTES_TRANSBORDADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTesTransbordados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Entregas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSBORDO_CARGA_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntrega", Column = "CEN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> Entregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClonaLancamentoEntregas", Column = "TRB_LANCAMENTO_ENTREGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClonaLancamentoEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClonaLancamentoColetas", Column = "TRB_LANCAMENTO_COLETAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClonaLancamentoColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClonaDataInicioViagemEntrega", Column = "TRB_DATA_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClonaDataInicioViagemEntrega { get; set; }

        public virtual string Descricao
        {
            get { return NumeroTransbordo.ToString(); }
        }

        public virtual bool Equals(Transbordo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
