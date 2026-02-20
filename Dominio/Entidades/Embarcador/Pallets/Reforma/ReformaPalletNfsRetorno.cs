namespace Dominio.Entidades.Embarcador.Pallets.Reforma
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_REFORMA_NFS_RETORNO", EntityName = "ReformaPalletNfsRetorno", Name = "Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno", NameType = typeof(ReformaPalletNfsRetorno))]
    public class ReformaPalletNfsRetorno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ReformaPallet", Column = "PAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ReformaPallet ReformaPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "PNS_DATA_EMISSAO", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual System.DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIss", Column = "PNS_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIss { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirIssBaseCalculo", Column = "PNS_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIssBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PNS_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "PNS_OBSERVACOES", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencao", Column = "PNS_PERCENTUAL_RETENCAO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "PNS_SERIE", TypeType = typeof(int), NotNull = false)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseCalculo", Column = "PNS_VALOR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIss", Column = "PNS_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIss { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoServico", Column = "PNS_VALOR_PRESTACAO_SERVICO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetido", Column = "PNS_VALOR_RETIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetido { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(ReformaPalletNfsRetorno other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
