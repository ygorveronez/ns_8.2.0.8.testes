using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaPedidoPacoteImportacao
    {
        public string LogKey { get; set; }
        public DateTime DataRecebimento {  get; set; }
        public double Origem {  get; set; }
        public double Destino { get; set; }
        public int Pedido { get; set; }
    }
}
