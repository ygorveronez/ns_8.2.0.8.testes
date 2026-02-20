namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOrdemEmbarque
    {
        Registrada = 1,
        Cancelada = 2,
        Embarcado = 3,
        EmOperacao = 4,
        EmTransito = 5,
        Fechada = 6,
        AguardandoOperacao = 7,
        AguardandoAprovacao = 8,
        OperacaoConcluida = 9,
        EntradaAutorizada = 10,
        Carregada = 12,
        LiberadaParaSaida = 13,
        EmDivergencia = 14,
        EmFaturamento = 15,
        EmCertificacao = 16,
        AguardandoAprovacaoDivergencia = 17,
        LiberadoParaFaturamento = 18,
        FaturamentoConcluido = 19,
        AguardandoInicioOperacao = 20,
        AguardandoChamadaDoca = 21,
        LiberadaConclusaoComAutorizacao = 22,
        AguardandoAprovacaoLiberacaoSemTermografo = 23,
        RegistradaSeguradoraBoiadParaCarga = 24,
        TransmissaoSeguradoraBoiadParaCarga = 25,
        AguardandoTransmissaoSeguradoraBoiadParaNF = 26,
        TransmissaoSeguradoraBoiadParaNF = 27,
        FechadaSeguradoraBoiad = 28,
        AguardandoLiberacaoPesagemManualPorPlaca = 29,
        PesagemManualPorPlacaRejeitada = 30,
        PesagemManualPorPlacaAprovada = 31,
        EmProcessoComercial = 32,
        AguardandoAprovacaoSemListaSeparacao = 33,
        AguardandoAprovacaoRecebimentoManual = 34,
        CheckInVeiculoRealizado = 35,
        CheckInVeiculoCancelado = 36,
        CheckOutVeiculoRealizado = 37,
        CheckOutVeiculoEstornado = 38,
        PesagemEntrada = 39,
        PesagemSaida = 40
    }

    public static class SituacaoOrdemEmbarqueHelper
    {
        public static string ObterDescricao(this SituacaoOrdemEmbarque situacao)
        {
            switch (situacao)
            {
                case SituacaoOrdemEmbarque.Registrada: return "Registrada";
                case SituacaoOrdemEmbarque.Cancelada: return "Cancelada";
                case SituacaoOrdemEmbarque.Embarcado: return "Embarcado";
                case SituacaoOrdemEmbarque.EmOperacao: return "Em Operação";
                case SituacaoOrdemEmbarque.EmTransito: return "Em Trânsito";
                case SituacaoOrdemEmbarque.Fechada: return "Fechada";
                case SituacaoOrdemEmbarque.AguardandoOperacao: return "Aguardando Operação";
                case SituacaoOrdemEmbarque.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoOrdemEmbarque.OperacaoConcluida: return "Operação Concluída";
                case SituacaoOrdemEmbarque.EntradaAutorizada: return "Entrada Autorizada";
                case SituacaoOrdemEmbarque.Carregada: return "Carregada";
                case SituacaoOrdemEmbarque.LiberadaParaSaida: return "Liberada para Saída";
                case SituacaoOrdemEmbarque.EmDivergencia: return "Em Divergência";
                case SituacaoOrdemEmbarque.EmFaturamento: return "Em Faturamento";
                case SituacaoOrdemEmbarque.EmCertificacao: return "Em Certificação";
                case SituacaoOrdemEmbarque.AguardandoAprovacaoDivergencia: return "Aguardando Aprovação Divergência";
                case SituacaoOrdemEmbarque.LiberadoParaFaturamento: return "Liberado para Faturamento";
                case SituacaoOrdemEmbarque.FaturamentoConcluido: return "Faturamento Concluído";
                case SituacaoOrdemEmbarque.AguardandoInicioOperacao: return "Aguardando Início da Operação";
                case SituacaoOrdemEmbarque.AguardandoChamadaDoca: return "Aguardando Chamada da Doca";
                case SituacaoOrdemEmbarque.LiberadaConclusaoComAutorizacao: return "Liberada a Conclusão c/ Autorização";
                case SituacaoOrdemEmbarque.AguardandoAprovacaoLiberacaoSemTermografo: return "Aguardando Aprovação Liberação sem Termógrafo";
                case SituacaoOrdemEmbarque.RegistradaSeguradoraBoiadParaCarga: return "Registrada Seguradora Boiad (Carga)";
                case SituacaoOrdemEmbarque.TransmissaoSeguradoraBoiadParaCarga: return "Transmissão Seguradora Boiad (Carga)";
                case SituacaoOrdemEmbarque.AguardandoTransmissaoSeguradoraBoiadParaNF: return "Aguardando Transmissão Seguradora Boiad (NF)";
                case SituacaoOrdemEmbarque.TransmissaoSeguradoraBoiadParaNF: return "Transmissão Seguradora Boiad (NF)";
                case SituacaoOrdemEmbarque.FechadaSeguradoraBoiad: return "Fechada Seguradora Boiad";
                case SituacaoOrdemEmbarque.AguardandoLiberacaoPesagemManualPorPlaca: return "Aguardando Liberação Pesagem Manual por Placa";
                case SituacaoOrdemEmbarque.PesagemManualPorPlacaRejeitada: return "Pesagem Manual por Placa Rejeitada";
                case SituacaoOrdemEmbarque.PesagemManualPorPlacaAprovada: return "Pesagem Manual por Placa Aprovada";
                case SituacaoOrdemEmbarque.EmProcessoComercial: return "Em Processo Comercial";
                case SituacaoOrdemEmbarque.AguardandoAprovacaoSemListaSeparacao: return "Aguardando Aprovação Sem Lista Separação";
                case SituacaoOrdemEmbarque.AguardandoAprovacaoRecebimentoManual: return "Aguardando Aprovação Recebimento Manual";
                case SituacaoOrdemEmbarque.CheckInVeiculoRealizado: return "Check In do Veículo Realizado";
                case SituacaoOrdemEmbarque.CheckInVeiculoCancelado: return "Check In do Veículo Cancelado";
                case SituacaoOrdemEmbarque.CheckOutVeiculoRealizado: return "Check Out do Veículo Realizado";
                case SituacaoOrdemEmbarque.CheckOutVeiculoEstornado: return "Check Out do Veículo Estornado";
                case SituacaoOrdemEmbarque.PesagemEntrada: return "Pesagem de Entrada";
                case SituacaoOrdemEmbarque.PesagemSaida: return "Pesagem de Saída";
                default: return string.Empty;
            }
        }
    }
}
