namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_INTEGRACAO_CARGA_EDI_ALTERACAO", EntityName = "ControleIntegracaoCargaEDIAlteracao", Name = "Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao", NameType = typeof(ControleIntegracaoCargaEDIAlteracao))]
    public class ControleIntegracaoCargaEDIAlteracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleIntegracaoCargaEDI", Column = "CIE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleIntegracaoCargaEDI ControleIntegracaoCargaEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MeioTransporteAnterior", Column = "IEA_MEIO_TRANSPORTE_ANTERIOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MeioTransporteAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MeioTransporteAtual", Column = "IEA_MEIO_TRANSPORTE_ATUAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MeioTransporteAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloVeicularAnterior", Column = "IEA_MODELO_VEICULAR_ANTERIOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ModeloVeicularAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloVeicularAtual", Column = "IEA_MODELO_VEICULAR_ATUAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ModeloVeicularAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaAnterior", Column = "IEA_PLACA_ANTERIOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PlacaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaAtual", Column = "IEA_PLACA_ATUAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PlacaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNfsAnterior", Column = "IEA_QUANTIDADE_NFS_ANTERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNfsAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNfsAtual", Column = "IEA_QUANTIDADE_NFS_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNfsAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteiroAnterior", Column = "IEA_ROTEIRO_ANTERIOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RoteiroAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteiroAtual", Column = "IEA_ROTEIRO_ATUAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RoteiroAtual { get; set; }
    }
}
