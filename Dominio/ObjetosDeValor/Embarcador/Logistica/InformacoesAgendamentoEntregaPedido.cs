using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class InformacoesAgendamentoEntregaPedido
    {
        public DateTime? DataAgendamento { get; set; }
        public string Observacoes { get; set; }
        public List<Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
        public Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega ResponsavelMotivoReagendamentoPedidos { get; set; }
        public string ObservacaoReagendamento { get; set; }
        public bool SalvarComDataRetroativa { get; set; }
        public bool Reagendamento{get;set;}
        public int CodigoMotivoReagendamento { get; set; }
        public bool ExigeSenhaAgendamento {get;set;}
        public string SenhaEntregaAgendamento { get; set; }
    }
}
