namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CODIGO_EXTERNO", EntityName = "IntegracaoCodigoExterno", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno", NameType = typeof(IntegracaoCodigoExterno))]
    public class IntegracaoCodigoExterno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "ICE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF_CNPJ", Column = "ICE_CPF_CNPJ", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPF_CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeiculo Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaVeiculo", Column = "VMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaVeiculo Marca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExterno", Column = "ICE_CODIGO_EXTERNO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoExterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCodigoExternoIntegracao", Column = "ICE_TIPO_CODIGO_EXTERNO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCodigoExternoIntegracao? TipoCodigoExternoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "ICE_TIPO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? TipoIntegracao { get; set; }
    }
}
