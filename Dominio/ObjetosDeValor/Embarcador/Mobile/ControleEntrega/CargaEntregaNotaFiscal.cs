using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public sealed class CargaEntregaNotaFiscal
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }

        public bool DevolucaoParcial { get; set; }

        public bool DevolucaoTotal { get; set; }

        public List<Produto> Produtos { get; set; }

        public Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega MotivoDevolucaoEntrega { get; set; }
    }
}
