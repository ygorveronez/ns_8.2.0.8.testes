using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec
{
    public class nfeocorrencia
    {
        public string embarcador { get; set; }

        public List<nfeocorrencianota> notas { get; set; }
    }

    public class nfeocorrencianota
    {
        public int motivo_ocorrencia { get; set; }

        public string data_problema { get; set; }

        public string motorista { get; set; }

        public string veiculo { get; set; }

        public string placa { get; set; }

        public string data_chegada { get; set; }

        public string chave { get; set; }

        public string transportadora { get; set; }

        public decimal? latitude { set; get; }

        public decimal? longitude { set; get; }
      
    }
}
