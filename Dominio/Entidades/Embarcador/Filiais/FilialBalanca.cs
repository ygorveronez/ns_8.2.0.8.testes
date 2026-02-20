namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_BALANCA", EntityName = "FilialBalanca", Name = "Dominio.Entidades.Embarcador.Filiais.FilialBalanca", NameType = typeof(FilialBalanca))]
    public class FilialBalanca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FBA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MarcaBalanca", Column = "FBA_MARCA_BALANCA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string MarcaBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloBalanca", Column = "FBA_MODELO_BALANCA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string ModeloBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HostConsultaBalanca", Column = "FBA_HOST_CONSULTA_BALANCA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string HostConsultaBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaBalanca", Column = "FBA_PORTA_BALANCA", TypeType = typeof(int), NotNull = true)]
        public virtual int PortaBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoRetornoBalanca", Column = "FBA_TAMANHO_RETORNO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoRetornoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoInicioPesoBalanca", Column = "FBA_POSICAO_INICIO_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoInicioPesoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoFimPesoBalanca", Column = "FBA_POSICAO_FIM_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoFimPesoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CasasDecimaisPesoBalanca", Column = "FBA_CASAS_DECIMAIS_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int CasasDecimaisPesoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeConsultasPesoBalanca", Column = "FBA_QUANTIDADE_CONSULTAS_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeConsultasPesoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesoBalanca", Column = "FBA_PERCENTUAL_TOLERANCIA_PESO_BALANCA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal PercentualToleranciaPesoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesagemEntrada", Column = "FBA_PERCENTUAL_TOLERANCIA_PESAGEM_ENTRADA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal PercentualToleranciaPesagemEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesagemSaida", Column = "FBA_PERCENTUAL_TOLERANCIA_PESAGEM_SAIDA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal PercentualToleranciaPesagemSaida { get; set; }

        public virtual string Descricao
        {
            get { return $"{MarcaBalanca} - {ModeloBalanca}"; }
        }
    }
}
