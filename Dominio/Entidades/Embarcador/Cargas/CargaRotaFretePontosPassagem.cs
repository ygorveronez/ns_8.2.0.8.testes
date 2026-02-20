using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ROTA_FRETE_PONTOS_PASSAGEM", EntityName = "CargaRotaFretePontosPassagem", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem", NameType = typeof(CargaRotaFretePontosPassagem))]
    public class CargaRotaFretePontosPassagem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaRotaFrete CargaRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPontoPassagem", Column = "CRP_TIPO_PONTOS_PASSAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPontoPassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ColetaEquipamento", Column = "CRP_COLETA_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDeParqueamento", Column = "CRP_LOCAL_DE_PARQUEAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LocalDeParqueamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CRP_LATITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CRP_LONGITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Tempo em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "CRP_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        /// <summary>
        /// Distância em metros.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "CRP_DISTANCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Distancia { get; set; }

        /// <summary>
        /// Distância em metros entre o ponto de partida/origem até esse ponto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaDireta", Column = "CRP_DISTANCIA_DIRETA", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaDireta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CRP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                if (this.TipoPontoPassagem == ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                    return this.PracaPedagio?.Descricao;
                else if (this.TipoPontoPassagem == ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                    return "Ponto de Passagem";
                else
                    return this.Cliente?.Descricao;
            }
        }

        public virtual bool Equals(CargaRotaFretePontosPassagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
