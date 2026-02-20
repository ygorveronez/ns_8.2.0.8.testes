namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_REBOQUE", EntityName = "ReboqueMDFe", Name = "Dominio.Entidades.ReboqueMDFe", NameType = typeof(ReboqueMDFe))]
    public class ReboqueMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "MDR_PLACA", TypeType = typeof(string), Length = 7, NotNull = true)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tara", Column = "MDR_TARA", TypeType = typeof(int), NotNull = true)]
        public virtual int Tara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeKG", Column = "MDR_CAPACIDADE_KG", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeM3", Column = "MDR_CAPACIDADE_M3", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeM3 { get; set; }

        /// <summary>
        /// 00 - não aplicável;
        /// 01 - Aberta;
        /// 02 - Fechada/Baú;
        /// 03 - Granelera;
        /// 04 - Porta Container;
        /// 05 - Sider
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarroceria", Column = "MDR_TIPO_CARROCERIA", TypeType = typeof(string), Length = 2, NotNull = true)]
        virtual public string TipoCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeProprietario", Column = "MDR_NOME_PROPRIETARIO", TypeType = typeof(string), Length = 60, NotNull = false)]
        virtual public string NomeProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJProprietario", Column = "MDR_CPF_CNPJ_PROPRIETARIO", TypeType = typeof(string), Length = 14, NotNull = false)]
        virtual public string CPFCNPJProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IEProprietario", Column = "MDR_IE_PROPRIETARIO", TypeType = typeof(string), Length = 14, NotNull = false)]
        virtual public string IEProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProprietario", Column = "MDR_TIPO_PROPRIETARIO", TypeType = typeof(string), Length = 1, NotNull = false)]
        virtual public string TipoProprietario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_VEICULO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        virtual public Estado UF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_PROPRIETARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        virtual public Estado UFProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "MDR_RNTRC", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RENAVAM", Column = "MDR_RENAVAM", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string RENAVAM { get; set; }
    }
}
