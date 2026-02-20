using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracoes.IntegracaoAssincrona
{
    public class FiltroPesquisaIntegracaoAssincrona
    {
        public string Pedido { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroCarregamento { get; set; }

        public int? NumeroCarga { get; set; }

        public DateTime? DataInicialIntegracao { get; set; }

        public DateTime? DataFinalIntegracao { get; set; }

        public StatusTarefa? StatusTarefa { get; set; }

        public TipoRequest? TipoRequest { get; set; }

        public TipoEtapaTarefa? TipoEtapaAtual { get; set; }

        public string JobId { get; set; }
    }
}