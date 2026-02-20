using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class AgendaCancelada
    {
        public string Carga { get; set; }
        public int Codigo { get; set; }
        public DateTime DataAgenda { get; set; }
        public DateTime DataCancelamento { get; set; }
        public string Senha { get; set; }
        public string NumeroPedido { get; set; }
        public string Fornecedor { get; set; }
        public string Destinatario { get; set; }
        public string Solicitante { get; set; }
        public int QuantidadeCaixas { get; set; }
        public string MotivoCancelamento { get; set; }
        public SituacaoCargaJanelaDescarregamento SituacaoJanela { get; set; }
        public SituacaoAgendamentoColeta SituacaoAgendamento { get; set; }

        public string DescricaoSituacaoAgendamento
        {
            get
            {
                return SituacaoAgendamento.ObterDescricao();
            }
        }

        public string SituacaoJanelaFormatado
        {
            get
            {
                return SituacaoJanela.ObterDescricao();
            }
        }
    }
}
