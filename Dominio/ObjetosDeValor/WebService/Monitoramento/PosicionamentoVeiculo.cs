using System;

namespace Dominio.ObjetosDeValor.WebService.Monitoramento
{
    public class PosicionamentoVeiculo
    {
        public string Transporte { get; set; }
        public string IdDispositivo { get; set; }
        public DateTime UltimaPosicao { get; set; }
        public string UltimaPosicaoString { get; set; }
        public string Placa { get; set; }
        public string Tipo { get; set; }
    }
}