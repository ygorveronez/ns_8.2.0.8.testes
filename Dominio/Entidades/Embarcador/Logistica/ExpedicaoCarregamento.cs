using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_CONTROLE_EXPEDICAO", EntityName = "ExpedicaoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento", NameType = typeof(ExpedicaoCarregamento))]
    public class ExpedicaoCarregamento : EntidadeBase
    { 
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EXC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "EXC_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EXC_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModelosVeicularesCargaExclusivo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_EXPEDICAO_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EXC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExpedicaoCarregamentoModeloVeicularCarga", Column = "EMV_CODIGO")]
        public virtual ICollection<ExpedicaoCarregamentoModeloVeicularCarga> ModelosVeicularesCargaExclusivo { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (this.ClienteDestino == null)
                    return $"Expedição: [Produto] {this.ProdutoEmbarcador.Descricao}";

                return $"Expedição: [Produto] {this.ProdutoEmbarcador.Descricao} | [Destino] {this.ClienteDestino.Descricao}";
            }
        }
    }
}
