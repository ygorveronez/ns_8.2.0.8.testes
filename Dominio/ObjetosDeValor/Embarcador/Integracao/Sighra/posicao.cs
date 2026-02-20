using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra
{
    public class Posicao
    {
        public int IdVeiculo { get; set; }
        public string Placa { get; set; }
        public int IdSequencia { get; set; }
        public int IdAplicacao { get; set; }
        public int IdEvento { get; set; }
        public DateTime DataEvento { get; set; }
        public DateTime DataRecepcao { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string IdPontoReferencia { get; set; }
        public char Ignicao { get; set; }
        public int Velocidade { get; set; }
        public int NumeroSateliteGPS { get; set; }
        public int DirecaoVeiculo { get; set; }
        public int InformacaoBinaria { get; set; }
        public int QualidadeGPS { get; set; }
        public long InformacaoSensores { get; set; }
        public int InformacaoAtuadores { get; set; }
        public int IdMeioComunicacao { get; set; }
        public int IdMacro { get; set; }
        public char EmAlarme { get; set; }
        public int Chip { get; set; }
        public decimal? Temperatura { get; set; }
    }
}
