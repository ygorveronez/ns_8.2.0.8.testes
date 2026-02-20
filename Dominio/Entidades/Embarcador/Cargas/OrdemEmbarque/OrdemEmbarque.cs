namespace Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_EMBARQUE", EntityName = "OrdemEmbarque", Name = "Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque", NameType = typeof(OrdemEmbarque))]
    public class OrdemEmbarque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "OEM_NUMERO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_VEICULO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemEmbarqueSituacao", Column = "OES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemEmbarqueSituacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReboque", Column = "OEM_NUMERO_REBOQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque NumeroReboque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_RESPONSAVEL_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario ResponsavelCancelamento { get; set; }

        public virtual string Descricao
        {
            get { return $"{Numero} ({Veiculo.Placa})"; }
        }
    }
}
