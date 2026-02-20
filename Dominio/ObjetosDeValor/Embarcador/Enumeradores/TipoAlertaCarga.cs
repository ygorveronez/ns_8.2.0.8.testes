using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAlertaCarga
    {
        SemAlerta = 0,
        CargaSemTransportador = 1,
        CagraSemVeiculo = 2,
        VeiculoComInsumos = 3,
        VeiculoNaoMonitorado = 4,
        ValidacaoGerenciadoraRisco = 5,
        NaoAtendimentoAgenda = 6,
        AntecedenciaGrade = 7,
        AtrasoColetaDescarga = 8,
        AtrasoInicioViagem = 9,
        InicioViagem = 10,
        FimViagem = 11,
        ConfirmacaoColetaEntrega = 12,
        AtendimentoIniciado = 13
    }

    public static class TipoAlertaCargaHelper
    {

        public static ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga ObterTipoDeAlertaPorDescricao(string Descricao)
        {
            foreach (int i in Enum.GetValues(typeof(TipoAlertaCarga)))
            {
                var tipo = (TipoAlertaCarga)Enum.ToObject(typeof(TipoAlertaCarga), i);
                if (TipoAlertaCargaHelper.ObterDescricao(tipo) == Descricao)
                    return tipo;
            }

            return TipoAlertaCarga.SemAlerta;
        }

        public static bool isAlertaAcompanhamentoCarga(TipoAlertaCarga tipo)
        {
            List<TipoAlertaCarga> alertasAcompanhamentoCarga = new List<TipoAlertaCarga>()
                {
                    TipoAlertaCarga.AtrasoColetaDescarga,
                    TipoAlertaCarga.AtrasoInicioViagem,
                    TipoAlertaCarga.InicioViagem,
                    TipoAlertaCarga.FimViagem,
                    TipoAlertaCarga.ConfirmacaoColetaEntrega,
                    TipoAlertaCarga.AtendimentoIniciado
                };

            if (alertasAcompanhamentoCarga.Contains(tipo))
                return true;
            else
                return false;


        }

        public static string ObterDescricao(this TipoAlertaCarga tipo)
        {
            switch (tipo)
            {
                case TipoAlertaCarga.SemAlerta: return "Sem alerta";
                case TipoAlertaCarga.CargaSemTransportador: return "Carga sem transportador";
                case TipoAlertaCarga.CagraSemVeiculo: return "Carga sem veículo";
                case TipoAlertaCarga.VeiculoComInsumos: return "Veículo com insumos";
                case TipoAlertaCarga.VeiculoNaoMonitorado: return "Veículo não monitorado";
                case TipoAlertaCarga.ValidacaoGerenciadoraRisco: return "Status Gerenciador de Risco";
                case TipoAlertaCarga.NaoAtendimentoAgenda: return "Não atendimento Agenda";
                case TipoAlertaCarga.AntecedenciaGrade: return "Antecendência a Grade Carregamento";
                case TipoAlertaCarga.AtrasoColetaDescarga: return "Atraso na Coleta/Entrega";
                case TipoAlertaCarga.AtrasoInicioViagem: return "Atraso início Viagem";
                case TipoAlertaCarga.InicioViagem: return "Início Viagem";
                case TipoAlertaCarga.FimViagem: return "Fim Viagem";
                case TipoAlertaCarga.ConfirmacaoColetaEntrega: return "Confirmação Coleta/Entrega";
                case TipoAlertaCarga.AtendimentoIniciado: return "Atendimento/Chamado Iniciado";

                default: return string.Empty;
            }
        }
    }


}