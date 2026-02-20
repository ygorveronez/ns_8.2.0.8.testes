namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSTO_FISCAL_ROTA_FRETE", EntityName = "PostoFiscalRotaFrete", Name = "Dominio.Entidades.Embarcador.Logistica.PostoFiscalRotaFrete", NameType = typeof(PostoFiscalRotaFrete))]
    public class PostoFiscalRotaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Tempo estimado de permanência do motorista nesse ponto, em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimadoPermanencia", Column = "PFR_TEMPO_ESTIMADO_PERMANENCIA_MINUTOS", Type = "int", NotNull = true)]
        public virtual int TempoEstimadoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }


        public virtual string TempoEstimadoPermanenciaFormatado
        {
            get { return $"{TempoEstimadoPermanencia / 60:D2}:{TempoEstimadoPermanencia % 60:D2}"; }
        }

        public virtual string Descricao
        {
            get { return $"{Cliente.Descricao}"; }
        }

        public virtual Dominio.Entidades.Localidade ObterLocalidade()
        {
            return Cliente?.Localidade;
        }

        public virtual string ObterLatitude()
        {
            return Cliente?.Latitude;
        }

        public virtual string ObterLongitude()
        {
            return Cliente?.Longitude;
        }
    }
}
