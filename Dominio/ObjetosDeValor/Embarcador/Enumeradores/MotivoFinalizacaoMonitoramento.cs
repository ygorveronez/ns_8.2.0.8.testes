using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MotivoFinalizacaoMonitoramento
    {
        FinalizacaoManual = 0,
        FinalizadoAoGerarCarga = 1,
        FinalizadoNoFluxoPatio = 2,
        FinalizadoAoMarcarFilaCarregamento = 3,
        FinalizadoAoChegarOrigemRetorno = 4,
        FinalizouControleEntregaCarga = 5,
        FinalizadoAoFimDaViagem = 6,
        FinalizadoAoCancelarMonitoramento = 7,
        StatusViagemConcluida = 8,
        FinalizadoPorSubstituicao = 9,
    }

    public static class MotivoFinalizacaoMonitoramentoHelper
    {
        public static string ObterDescricao(this MotivoFinalizacaoMonitoramento motivoFinalizacao)
        {
            switch (motivoFinalizacao)
            {
                case MotivoFinalizacaoMonitoramento.FinalizacaoManual: return "Finalização Manual";
                case MotivoFinalizacaoMonitoramento.FinalizadoAoGerarCarga: return "Finalizado ao gerar carga";
                case MotivoFinalizacaoMonitoramento.FinalizadoNoFluxoPatio: return "Finalizado no fluxo pátio";
                case MotivoFinalizacaoMonitoramento.FinalizadoAoMarcarFilaCarregamento: return "Finalizado ao marcar fila carregamento";
                case MotivoFinalizacaoMonitoramento.FinalizadoAoChegarOrigemRetorno: return "Finalizado ao chegar origem retorno";
                case MotivoFinalizacaoMonitoramento.FinalizouControleEntregaCarga: return "Finalizou controle entrega carga";
                case MotivoFinalizacaoMonitoramento.FinalizadoAoFimDaViagem: return "Finalizado ao fim da viagem";
                case MotivoFinalizacaoMonitoramento.FinalizadoAoCancelarMonitoramento: return "Finalizado ao cancelar monitoramento";
                case MotivoFinalizacaoMonitoramento.StatusViagemConcluida: return "Status viagem concluída";
                case MotivoFinalizacaoMonitoramento.FinalizadoPorSubstituicao: return "Finalizado por substituição";
                default: return string.Empty;
            }


        }

    }
}
