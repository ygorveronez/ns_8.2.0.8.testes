using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_CARROCERIA", EntityName = "ModeloCarroceria", Name = "Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria", NameType = typeof(ModeloCarroceria))]
    public class ModeloCarroceria : EntidadeBase, IEquatable<ModeloCarroceria>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MCA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdicionalFrete", Column = "MCA_PERCENTUAL_ADICIONAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualAdicionalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MCA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade{ get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "MCA_OBRIGATORIO_INFORMAR_DATA_VALIDADE_ADICIONAL_CARROCERIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarDataValidadeAdicionalCarroceria { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "MCA_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string CodigoIntegracao { get; set; }

		public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(ModeloCarroceria other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
