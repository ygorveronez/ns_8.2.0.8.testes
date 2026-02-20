using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_ROTEIRIZACAO", EntityName = "CarregamentoRoteirizacao", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao", NameType = typeof(CarregamentoRoteirizacao))]
    public class CarregamentoRoteirizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaKM", Column = "CRT_DISTANCIA_KM", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal DistanciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CRT_TIPO_ROTA", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "CRT_TIPO_ULTIMO_PONTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "PolilinhaRota", Column = "CRT_POLINHA_ROTA", Type = "StringClob", NotNull = false)]
        public virtual string PolilinhaRota { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "PontosDaRota", Column = "CRT_PONTOS_ROTA", Type = "StringClob", NotNull = false)]
        public virtual string PontosDaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeViagemEmMinutos", Column = "CRT_TEMPO_VIAGEM_EM_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeViagemEmMinutos { get; set; }


        public virtual bool Equals(CarregamentoRoteirizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
