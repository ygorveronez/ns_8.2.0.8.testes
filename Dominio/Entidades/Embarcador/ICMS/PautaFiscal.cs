using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAUTA_FISCAL", EntityName = "PautaFiscal", Name = "Dominio.Entidades.Embarcador.ICMS.PautaFiscal", NameType = typeof(PautaFiscal))]
    public class PautaFiscal : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_TARIFA", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Tarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_DISTANCIA_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaKMInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_DISTANCIA_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaKMFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_VALOR_TONELADA", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal ValorTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_VALOR_VIAGEM", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal ValorViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_VALOR_M3", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal ValorM3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PFS_VALOR_VOLUME_MST", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal ValorVolumeMST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PFS_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao => $"{(Estado?.Descricao ?? string.Empty)} - {Tarifa}";

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
