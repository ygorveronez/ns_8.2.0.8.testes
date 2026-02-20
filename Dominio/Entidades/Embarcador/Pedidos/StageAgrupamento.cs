namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STAGE_AGRUPAMENTO", EntityName = "StageAgrupamento", Name = "Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento", NameType = typeof(StageAgrupamento))]
    public class StageAgrupamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_GERADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_REBOQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Reboque { get; set; } 
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_SEGUNDO_REBOQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo SegundoReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTotal", Column = "STG_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponentes", Column = "STG_VALOR_COMPONENTES_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoDadosFrete", Column = "STG_MENSAGEM_RETORNO_FRETE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetornoDadosFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Processado", Column = "STG_PROCESSADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Processado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoProcessamento", Column = "STG_RETORNO_PROCESSAMENTO", Length = 450, NotNull = false)]
        public virtual string RetornoProcessamento { get; set; }  
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroVeiculo", Column = "STG_NUMERO_VEICULO", NotNull = false)]
        public virtual int NumeroVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCalculoFrete", Column = "STG_BLOQUEAR_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessadoPorPrechekin", Column = "STG_PROCESSADO_PRE_CHEKIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessadoPorPrechekin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacasConfirmadas", Column = "STG_PLACAS_CONFIRMADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PlacasConfirmadas { get; set; }

    }
}
