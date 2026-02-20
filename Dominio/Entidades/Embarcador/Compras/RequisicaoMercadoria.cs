using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REQUISICAO_MERCADORIA", EntityName = "RequisicaoMercadoria", Name = "Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria", NameType = typeof(RequisicaoMercadoria))]
    public class RequisicaoMercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MER_NUMERO", TypeType = typeof(int))]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Filiais.Filial Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCompra", Column = "MCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.MotivoCompra MotivoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RME_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RME_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RME_MODO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria Modo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RME_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_REQUISITADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioRequisitado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Mercadorias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RME_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Mercadoria", Column = "MER_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.Mercadoria> Mercadorias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Autorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_REQUISICAO_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RME_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaRequisicaoMercadoria", Column = "AAA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> Autorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }
    }
}
