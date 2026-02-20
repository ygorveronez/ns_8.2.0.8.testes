using System;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe
{
    public class ConsultaMDFeGeracaoCargaEmbarcador
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
        public string Veiculos { get; set; }
        public string Segmento { get; set; }
    }
}
