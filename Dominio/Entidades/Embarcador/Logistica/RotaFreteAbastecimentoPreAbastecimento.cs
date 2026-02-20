namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_ABASTECIMENTO_PRE_ABASTECIMENTO", EntityName = "RotaFreteAbastecimentoPreAbastecimento", Name = "Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento", NameType = typeof(RotaFreteAbastecimentoPreAbastecimento))]
    public class RotaFreteAbastecimentoPreAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFreteAbastecimento", Column = "RFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFreteAbastecimento RotaFreteAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RFP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Litros", Column = "RFP_LITROS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Litros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "RFP_VALOR_UN", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TanqueCheio", Column = "RFP_TANQUE_CHEIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TanqueCheio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "RFP_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        public virtual decimal ValorTotal
        {
            get
            {
                return this.Litros * this.ValorUnitario;
            }
        }

        public virtual string TanqueCheioDescricao
        {
            get {return this.TanqueCheio ? "Sim" : "Não";}
        } 

    }
}
