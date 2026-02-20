using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ConsultaCTeGeracaoCargaEmbarcador
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Empresa { get; set; }
        public string UFOrigem { get; set; }
        public string UFDestino { get; set; }
        public string Origem { get; set; }        
        public string Destino { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Veiculo { get; set; }
    }
}
