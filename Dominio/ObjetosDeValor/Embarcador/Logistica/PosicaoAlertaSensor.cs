using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PosicaoAlertaSensor
    {
        public Int64 ID { get; set; }
        public Int64 IDPosicao { get; set; }
        public int IDVeiculo { get; set; }
        public DateTime DataVeiculo { get; set; }
        public DateTime DataCadastro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor TipoSensor { get; set; }
        public bool? ValorSensor { get; set; }
    }
}
