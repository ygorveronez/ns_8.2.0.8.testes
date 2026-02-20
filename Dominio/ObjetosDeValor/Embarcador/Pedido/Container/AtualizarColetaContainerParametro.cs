using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class AtualizarColetaContainerParametro
    {
        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;

        public DateTime DataAtualizacao;

        public ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer Status;

        public Dominio.Entidades.Cliente LocalAtual;

        public Dominio.Entidades.Embarcador.Cargas.Carga CargaDaColeta;

        public Dominio.Entidades.Cliente LocalEmbarque;

        public Dominio.Entidades.Cliente LocalColeta;

        public DateTime? DataEmbarque;

        public DateTime? DataColeta;

        public DateTime? DataEmbarqueNavio;

        public int DiasFreeTime;

        public decimal ValorDiaria;

        public Dominio.Entidades.Embarcador.Pedidos.Container Container;

        public Dominio.Entidades.Usuario Usuario;

        public Dominio.Entidades.Cliente AreaEsperaVazio { get; set; }
        public Enumeradores.OrigemMovimentacaoContainer OrigemMonimentacaoContainer { get; set; }
        public Enumeradores.InformacaoOrigemMovimentacaoContainer InformacaoOrigemMonimentacaoContainer { get; set; }

    }
}
