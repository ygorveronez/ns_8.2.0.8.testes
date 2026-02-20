using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Monitoramento
    {
        public int Codigo { get; set; }
        public virtual DateTime? DataCriacao { get; set; }
        public virtual DateTime? DataInicio { get; set; }
        public bool Critico { get; set; }
        public SituacaoCarga Situacao { get; set; }
        public string NomeMotorista { get; set; }
        public string PlacaTracao { get; set; }
        public string PlacaReboque { get; set; }
        public virtual DateTime? DataPosicaoAtual { get; set; }
        public double? LatitudePosicaoAtual { get; set; }
        public double? LongitudePosicaoAtual { get; set; }
        public decimal PercentualViagem { get; set; }
        public decimal? KmTotal { get; set; }
        public string TipoOperacao { get; set; }
        public Distancia Distancias { get; set; }
        public IList<Historico> Historicos { get; set; }
    }
}