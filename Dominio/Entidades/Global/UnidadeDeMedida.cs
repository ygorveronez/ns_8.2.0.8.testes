namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_UNIDADE_MEDIDA", EntityName = "UnidadeDeMedida", Name = "Dominio.Entidades.UnidadeDeMedida", NameType = typeof(UnidadeDeMedida))]
    public class UnidadeDeMedida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UNI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "UNI_DESCRICAO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "UNI_SIGLA", TypeType = typeof(string), Length = 5, NotNull = true)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDaUnidade", Column = "UNI_CODIGO_UNIDADE", TypeType = typeof(string), Length = 5, NotNull = true)]
        public virtual string CodigoDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "UNI_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirExpedicaoEmTempoReal", Column = "UNI_EXIBIR_EXPEDICAO_TEMPO_REAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirExpedicaoEmTempoReal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCasasDecimais", Column = "UNI_NUMERO_CASAS_DECIMAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCasasDecimais { get; set; }

        public virtual Dominio.Enumeradores.UnidadeMedida UnidadeMedida {
            get {
                switch (this.Sigla)
                {
                    case "KG":
                        return Enumeradores.UnidadeMedida.KG;
                    case "LT":
                        return Enumeradores.UnidadeMedida.LT;
                    case "M3":
                        return Enumeradores.UnidadeMedida.M3;
                    case "MMBTU":
                        return Enumeradores.UnidadeMedida.MMBTU ;
                    case "TON":
                        return Enumeradores.UnidadeMedida.TON;
                    case "UN":
                        return Enumeradores.UnidadeMedida.UN;
                    default:
                       return Enumeradores.UnidadeMedida.KG;
                }

            }
        }
        public virtual string UnidadeMedidaSuperApp
        {
            get
            {
                switch (this.Sigla)
                {
                    case "KG":
                        return "kg";
                    case "LT":
                        return "l";
                    case "M3":
                        return "m³";
                    case "TON":
                        return "t";
                    default:
                        return "";
                }
            }
        }
    }
}
