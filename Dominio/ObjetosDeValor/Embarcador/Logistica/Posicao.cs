using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Posicao
    {
        public string Descricao { get; set; }
        public string Placa { get; set; }
        public int CodigoVeiculo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Velocidade { get; set; }
        public int Ignicao { get; set; }
        public string IDEquipamento { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataCadastro { get; set; }
        public Int64 ID { get; set; }
        public DateTime DataVeiculo { get; set; }
        public decimal? Temperatura { get; set; }
        public bool? SensorTemperatura { get; set; }
        public bool? EmAlvo { get; set; }
        public bool? EmLocal { get; set; }
        public string CodigosClientesAlvo { get; set; }
        public decimal? NivelBateria { get; set; }
        public decimal? NivelSinalGPS { get; set; }
        public TimeSpan Tempo { get; set; }
        public double km { get; set; }
        public EnumTecnologiaRastreador Rastreador { get; set; }
        public EnumTecnologiaGerenciadora Gerenciadora { get; set; }

        //sensores tratados
        public bool? SensorDeDesengate { get; set; }

        public int CodigoMonitoramento { get; set; }
    }
}
