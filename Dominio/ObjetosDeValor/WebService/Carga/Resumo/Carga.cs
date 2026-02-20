using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.Resumo
{
    public class Carga
    {
        public int Protocolo { get; set; }

        public string NumeroCarga { get; set; }

        public string Situacao { get; set; }

        public List<Pedido> Pedidos { get; set; }

    }
}
