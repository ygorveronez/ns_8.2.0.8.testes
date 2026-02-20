using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_ABASTECIMENTO_RESUMO", EntityName = "AcertoResumoAbastecimento", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento", NameType = typeof(AcertoResumoAbastecimento))]
    public class AcertoResumoAbastecimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMInicial", Column = "AAR_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMFinal", Column = "AAR_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Litros", Column = "AAR_LITROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Litros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "AAR_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Media", Column = "AAR_MEDIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Media { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaIdeal", Column = "AAR_MEDIA_IDEAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaIdeal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResumoAprovado", Column = "AAR_APROVADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ResumoAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal", Column = "AAR_KM_TOTAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotalAjustado", Column = "AAR_KM_TOTAL_AJUSTADO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotalAjustado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAjuste", Column = "AAR_PERCENTUAL_AJUSTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAjuste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "AAR_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroInicial", Column = "AAR_HORIMETRO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroFinal", Column = "AAR_HORIMETRO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroTotal", Column = "AAR_HORIMETRO_TOTAL", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroTotalAjustado", Column = "AAR_HORIMETRO_TOTAL_AJUSTADO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroTotalAjustado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAjusteHorimetro", Column = "AAR_PERCENTUAL_AJUSTE_HORIMETRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAjusteHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaHorimetro", Column = "AAR_MEDIA_HORIMETRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaIdealHorimetro", Column = "AAR_MEDIA_IDEAL_HORIMETRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaIdealHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LitrosEquipamento", Column = "AAR_LITROS_EQUIPAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LitrosEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalEquipamento", Column = "AAR_VALOR_TOTAL_EQUIPAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalEquipamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Placa ?? string.Empty;
            }
        }

        public virtual bool Equals(AcertoResumoAbastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
