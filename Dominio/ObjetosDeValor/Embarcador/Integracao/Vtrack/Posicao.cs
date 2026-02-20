using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack
{
    public class Posicao
    {
        public string id { get; set; }
        public string condutor { get; set; }
        public string cod_client { get; set; }
        public string placa { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string velocidade { get; set; }
        public string data { get; set; }
        public string hodometro { get; set; }
        public string ligado { get; set; }

        public string adicionais { get; set; }
        public string sensor_temp1 { get; set; }

        public string sensor_temp2 { get; set; }

        public string sensor_temp3 { get; set; }
    }

    public class PosicaoVtrack
    {
        public int total { get; set; }
        public List<Posicao> Posicoes { get; set; }
    }


}
