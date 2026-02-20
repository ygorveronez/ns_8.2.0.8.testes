namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_COMPRA", EntityName = "MotivoCompra", Name = "Dominio.Entidades.Embarcador.Compras.MotivoCompra", NameType = typeof(MotivoCompra))]
    public class MotivoCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_EXIGE_INFORMAR_VEICULO_OBRIGATORIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeInformarVeiculoObrigatoriamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria Forma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_GERAR_IMPRESSAO_OC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarImpressaoOC { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
