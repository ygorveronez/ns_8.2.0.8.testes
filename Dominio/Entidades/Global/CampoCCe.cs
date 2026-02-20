namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CAMPO_CCE", EntityName = "CampoCCe", Name = "Dominio.Entidades.CampoCCe", NameType = typeof(CampoCCe))]
    public class CampoCCe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CCC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCampo", Column = "CCC_NOME_CAMPO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoCampo", Column = "CCC_GRUPO_CAMPO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string GrupoCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCC_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorRepeticao", Column = "CCC_INDICADOR_REPETICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IndicadorRepeticao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampo", Column = "CCC_TIPO_CAMPO", TypeType = typeof(Enumeradores.TipoCampoCCe), NotNull = true)]
        public virtual Enumeradores.TipoCampoCCe TipoCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeInteiros", Column = "CCC_QTD_INTEIROS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeInteiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDecimais", Column = "CCC_QTD_DECIMAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDecimais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaracteres", Column = "CCC_QTD_CARACTERES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaracteres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampoCCeAutomatico", Column = "CCC_TIPO_CAMPO_CCE_AUTOMATICO", TypeType = typeof(Enumeradores.TipoCampoCCeAutomatico), NotNull = false)]
        public virtual Enumeradores.TipoCampoCCeAutomatico TipoCampoCCeAutomatico { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
