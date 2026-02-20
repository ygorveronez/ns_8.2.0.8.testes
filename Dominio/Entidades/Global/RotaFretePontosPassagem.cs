namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_PONTO_PASSAGEM", EntityName = "RotaFretePontosPassagem", Name = "Dominio.Entidades.RotaFretePontosPassagem", NameType = typeof(RotaFretePontosPassagem))]
    public class RotaFretePontosPassagem : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPontoPassagem", Column = "RFP_TIPO_PONTOS_PASSAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPontoPassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "RFP_LATITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "RFP_LONGITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDeParqueamento", Column = "RFP_LOCAL_DE_PARQUEAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LocalDeParqueamento { get; set; }

        /// <summary>
        /// Tempo em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "RFP_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        /// <summary>
        /// Tempo estimado de esperado de permancência nesse ponto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimadoPermanenencia", Column = "RFP_TEMPO_ESTIMADO_PERMANENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEstimadoPermanenencia { get; set; }

        /// <summary>
        /// Ditancia em metros.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "RFP_DISTANCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (this.TipoPontoPassagem == ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                    return this.PracaPedagio.Descricao;
                else if (this.TipoPontoPassagem == ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                    return "Ponto de Passagem";
                else
                    return this.Cliente?.Descricao ?? string.Empty;
            }
        }

    }
}
