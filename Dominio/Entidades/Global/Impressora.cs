namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPRESSORAS", EntityName = "Impressora", Name = "Dominio.Entidades.Impressora", NameType = typeof(Impressora))]
    public class Impressora : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "IMP_NUMERO_UNIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeImpressora", Column = "IMP_IMPRESSORA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeImpressora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "IMP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "IMP_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        /// <summary>
        /// A - ATIVO
        /// I - INATIVO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "IMP_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        /// <summary>
        /// C - CTE E MDFE
        /// N - NFE E BOLETO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "IMP_DOCUMENTO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Documento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NomeImpressora;
            }
        }
    }
}
