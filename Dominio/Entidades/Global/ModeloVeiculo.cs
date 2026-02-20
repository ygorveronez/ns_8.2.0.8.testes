namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MODELO", EntityName = "ModeloVeiculo", Name = "Dominio.Entidades.ModeloVeiculo", NameType = typeof(ModeloVeiculo))]
    public class ModeloVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "VMO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VMO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEixo", Column = "VMO_NUMERO_EIXO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VMO_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaVeiculo", Column = "VMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaVeiculo MarcaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiArla32", Column = "VMO_MOTOR_ARLA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SimNao? PossuiArla32 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaPadrao", Column = "VMO_MEDIA_PADRAO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaPadraoVazio", Column = "VMO_MEDIA_PADRAO_VAZIO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaPadraoVazio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaEmMetros", Column = "VMO_ALTURA_METROS", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal AlturaEmMetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFIPE", Column = "VMO_CODIGO_FIPE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoFIPE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaMinima", Column = "VMO_MEDIA_MINIMA", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaMaxima", Column = "VMO_MEDIA_MAXIMA", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaMaxima { get; set; }

        public virtual string DescricaoPossuiArla32
        {
            get
            {
                switch (this.PossuiArla32)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim:
                        return "Sim";
                    case ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao:
                        return "Não";
                    default:
                        return "";
                }
            }
        }

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
                        return "";
                }
            }
        }


    }
}
