using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex
{
    public class Posicao
    {
        public long IdPosicao { get; set; }
        public int IdRastreador { get; set; }
        public int IdVeiculo { get; set; }
        public string Placa { get; set; }
        public int IdEvento { get; set; }
        public DateTime Evento_Datahora { get; set; }
        public double GPS_Latitude { get; set; }
        public double GPS_Longitude { get; set; }
        public int GPS_Direcao { get; set; }
        public int GPS_Velocidade { get; set; }
        public int Temperatura1 { get; set; }
        public int Temperatura2 { get; set; }
        public bool Ignicao { get; set; }
    }

    public class PosicaoResponse
    {
        public List<Posicao> ListaPosicoes { get; set; }
        public int RetornoWSSituacao { get; set; }
        public string RetornoWSMensagem { get; set; }
        public string Message { get; set; }
    }

}
