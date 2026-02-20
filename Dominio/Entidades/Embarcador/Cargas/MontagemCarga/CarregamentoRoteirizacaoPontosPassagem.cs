using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_ROTEIRIZACAO_PONTOS_PASSAGEM", EntityName = "CarregamentoRoteirizacaoPontosPassagem", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem", NameType = typeof(CarregamentoRoteirizacaoPontosPassagem))]
    public class CarregamentoRoteirizacaoPontosPassagem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CarregamentoRoteirizacao", Column = "CRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CarregamentoRoteirizacao CarregamentoRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Locais", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.Locais PontoDeApoio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPontoPassagem", Column = "CPP_TIPO_PONTOS_PASSAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPontoPassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CPP_LATITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CPP_LONGITUDE", TypeType = typeof(decimal), NotNull = true, Scale = 10, Precision = 18)]
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Tempo em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "CPP_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        /// <summary>
        /// Distância em metros.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "CPP_DISTANCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Distancia { get; set; }

        /// <summary>
        /// Distância em metros entre o ponto de partida/origem até esse ponto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaDireta", Column = "CPP_DISTANCIA_DIRETA", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaDireta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CPP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

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

        public virtual bool Equals(CarregamentoRoteirizacaoPontosPassagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
