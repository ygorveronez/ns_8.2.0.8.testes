using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_AGRUPAMENTO_AGRUPADOR", EntityName = "PreAgrupamentoCargaAgrupador", Name = "Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador", NameType = typeof(PreAgrupamentoCargaAgrupador))]
    public class PreAgrupamentoCargaAgrupador : EntidadeBase, IEquatable<PreAgrupamentoCargaAgrupador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAgrupamento", Column = "PAA_CODIGO_AGRUPAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "PAA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_PENDENCIA", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Pendencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PAA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integracao.ArquivoIntegracao ArquivoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Agrupamentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PRE_AGRUPAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PreAgrupamentoCarga", Column = "PAC_CODIGO")]
        public virtual ICollection<PreAgrupamentoCarga> Agrupamentos { get; set; }

        /// <summary>
        /// indica que no agrupamento existem cargas que ainda tem pre cargas deste modo não faz o encaixe de cargas se necessário, tem qeu aguardar ser false para poder encaixar.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPreCargas", Column = "PAA_POSSUI_PRE_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiPreCargas { get; set; }

        public virtual bool IsTodosAgrupamentosPossuemCarga(bool semEncaixe)
        {
            if (!semEncaixe)
                return Agrupamentos.All(o => o.Carga != null);
            else
                return Agrupamentos.All(o => (o.Carga != null) || (o.Carga == null && o.PedidoEncaixe));
        }

        public virtual bool IsTodosAgrupamentosPossuemCargaOuCargaRedespacho(bool semEncaixe)
        {
            if (!semEncaixe)
                return Agrupamentos.All(o => o.Carga != null || o.CargaRedespacho != null);
            else
                return Agrupamentos.All(o => (o.Carga != null || o.CargaRedespacho != null) || (o.Carga == null && o.PedidoEncaixe));
        }

        public virtual string Descricao
        {
            get { return $"Agrupamento {CodigoAgrupamento}"; }
        }

        public virtual bool Equals(PreAgrupamentoCargaAgrupador other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
