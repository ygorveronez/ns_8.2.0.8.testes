using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento
{
    public class RetornoPosicoesDev
    {
        public body body { get; set; }

        //public List<Posicoes> resposta_fornecedor_json { get; set; }
    }


    public class body
    {
        public List<PontosRastreamento> pontos_rastreamento { get; set; }
        public string identificador { get; set; }

        //public List<Posicoes> resposta_fornecedor_json { get; set; }
    }

    public class PontosRastreamento
    {
        public int numero { get; set; }
        public DateTime data_ocorrencia { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string tecnologia { get; set; }
        public string id_device { get; set; }
        public string id_veiculo { get; set; }
        public int velocidade_veiculo { get; set; }
        public decimal? temperatura { get; set; }
        public string odometro_veiculo { get; set; }
        public string ignicao_veiculo { get; set; }
        public string fonte_alimentacao { get; set; }
        public string nivel_bateria { get; set; }
    }


}
