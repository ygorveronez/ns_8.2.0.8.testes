using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoStatusViagemTipoRegra
    {
        Todos = 0,
        SemViagem = 1,
        EmViagem = 2,
        Retornando = 3,
        Concluida = 4,
        DeslocamentoParaPlanta = 5,
        AguardandoHorarioCarregamento = 6,
        AguardandoCarregamento = 7,
        EmCarregamento = 8,
        EmLiberacao = 9,
        Transito = 10,
        AguardandoHorarioDescarga = 11,
        AguardandoDescarga = 12,
        Descarga = 13,
        DescargaFinalizada = 14,
        DeslocamentoParaColetarEquipamento = 15,
        DeslocamentoComEquipamentoParaPlanta = 16,
        DeslocamentoComEquipamentoECargaParaEntrega = 17,
        EmColeta = 18,
        Cancelada = 19,
        EmParqueamento = 20,
        EmFronteira = 21
    }

    public static class MonitoramentoStatusViagemTipoRegraHelper
    {
        public static string ObterDescricao(this MonitoramentoStatusViagemTipoRegra status)
        {
            switch (status)
            {
                case MonitoramentoStatusViagemTipoRegra.SemViagem: return "Sem viagem";
                case MonitoramentoStatusViagemTipoRegra.EmViagem: return "Em viagem";
                case MonitoramentoStatusViagemTipoRegra.Retornando: return "Retornando";
                case MonitoramentoStatusViagemTipoRegra.Concluida: return "Concluída";
                case MonitoramentoStatusViagemTipoRegra.Cancelada: return "Cancelada";

                case MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta: return "Deslocamento para a planta";
                case MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento: return "Aguardando horário de carregamento";
                case MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento: return "Aguardando carregamento";
                case MonitoramentoStatusViagemTipoRegra.EmCarregamento: return "Em carregamento";
                case MonitoramentoStatusViagemTipoRegra.EmLiberacao: return "Em liberação";
                case MonitoramentoStatusViagemTipoRegra.Transito: return "Trânsito";
                case MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga: return "Aguardando horário de descarga";
                case MonitoramentoStatusViagemTipoRegra.AguardandoDescarga: return "Aguardando descarga";
                case MonitoramentoStatusViagemTipoRegra.Descarga: return "Descarga";
                case MonitoramentoStatusViagemTipoRegra.DescargaFinalizada: return "Descarga finalizada";

                case MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento: return "Deslocamento para coletar o equipamento";
                case MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta: return "Deslocamento com o equipamento até a planta para coleta da carga";
                case MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega: return "Deslocamento com equipamento e carga para entrega";
                case MonitoramentoStatusViagemTipoRegra.EmColeta: return "Em coleta";
                case MonitoramentoStatusViagemTipoRegra.EmParqueamento: return "Em Parqueamento";
                case MonitoramentoStatusViagemTipoRegra.EmFronteira: return "Em Fronteira";

                default: return string.Empty;
            }
        }

        public static string ObterDescricaoLonga(this MonitoramentoStatusViagemTipoRegra status)
        {
            switch (status)
            {
                case MonitoramentoStatusViagemTipoRegra.SemViagem: return "Ainda não foi iniciado o monitoramento.";
                case MonitoramentoStatusViagemTipoRegra.EmViagem: return "O monitoramento foi iniciado.";
                case MonitoramentoStatusViagemTipoRegra.Retornando: return "Retorno do veículo para a origem após finalizar um monitoramento prévio.";
                case MonitoramentoStatusViagemTipoRegra.Concluida: return "O monitoramento foi finalizado e a viagem concluída.";

                case MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta: return "Iniciou o monitoramento, não chegou na planta da origem para carregar.";
                case MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento: return "Chegou adiantado na origem para carregar e não está na área de carregamento.";
                case MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento: return "Está na origem, após o horário agendado e não está na área de carregamento.";
                case MonitoramentoStatusViagemTipoRegra.EmCarregamento: return "Está na origem e dentro da área de carregamento.";
                case MonitoramentoStatusViagemTipoRegra.EmLiberacao: return "Está na origem, já entrou e saiu da área de carregamento.";
                case MonitoramentoStatusViagemTipoRegra.Transito: return "Viagem iniciada, monitoramento em andamento, saiu da origem tendo passado pelo carregamento.";
                case MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga: return "Chegou adiantado no destino para descarregar e não está na área de descarregamento.";
                case MonitoramentoStatusViagemTipoRegra.AguardandoDescarga: return "Está no destino, após o horário agendado e não está na área de descarregamento.";
                case MonitoramentoStatusViagemTipoRegra.Descarga: return "Está no destino, está na área de descarregamento caso exista.";
                case MonitoramentoStatusViagemTipoRegra.DescargaFinalizada: return "Saiu da área de descarga e ainda está no destino.";

                case MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento: return "Está se deslocando em direção à coleta do equipamento.";
                case MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta: return "Já coletou o equipamento e está se deslocando em direção a planta para carregamento.";
                case MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega: return "O equipamento e a carga foram coletados e está se deslocando para a entrega.";
                case MonitoramentoStatusViagemTipoRegra.EmColeta: return "Está em alguns dos destinos para coleta.";
                case MonitoramentoStatusViagemTipoRegra.EmParqueamento: return "Está em alguns dos destinos de parqueamento.";
                case MonitoramentoStatusViagemTipoRegra.EmFronteira: return "Está na área de Fronteira.";

                default: return string.Empty;
            }
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoPreEmbarque()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoDeslocamentoParaOrigem()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta,
                MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento,
                MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento,
                MonitoramentoStatusViagemTipoRegra.EmCarregamento
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoPreCheckIn()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta,
                MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento,
                MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento,
                MonitoramentoStatusViagemTipoRegra.EmCarregamento
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoEmCarregamento()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento,
                MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento,
                MonitoramentoStatusViagemTipoRegra.EmCarregamento
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoSaidaDaOrigem()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.Transito
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoEmViagem()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.Transito
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoChegadaDestino()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga,
                MonitoramentoStatusViagemTipoRegra.AguardandoDescarga,
                MonitoramentoStatusViagemTipoRegra.Descarga
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoDescarga()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
                MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga,
                MonitoramentoStatusViagemTipoRegra.AguardandoDescarga,
                MonitoramentoStatusViagemTipoRegra.Descarga
            };
        }

        public static List<MonitoramentoStatusViagemTipoRegra> ObterStatusSumarizacaoSaidaDestino()
        {
            return new List<MonitoramentoStatusViagemTipoRegra>()
            {
            };
        }
    }
}



