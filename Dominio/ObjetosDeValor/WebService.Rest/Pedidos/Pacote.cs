using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Pedidos
{
    public class Pacote
    {
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime? Data_confirmacao { get; set; }
        public string Loggi_key { get; set; }
        public string Contratante { get; set; }
        public decimal Cubagem { get; set; }
        public decimal Peso { get; set; }
    }
}
