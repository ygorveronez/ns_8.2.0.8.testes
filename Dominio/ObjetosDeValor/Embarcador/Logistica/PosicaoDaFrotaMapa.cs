using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PosicaoDaFrotaMapa
    {
        public string Placa { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Descricao { get; set; }
        public string IDEquipamento { get; set; }
        public DateTime DataPosicao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao Status { get; set; }
    }




}


