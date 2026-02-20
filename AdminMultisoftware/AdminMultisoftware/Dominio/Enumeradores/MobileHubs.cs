using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum MobileHubs
    {
        CargaAtualizada = 1,
        ChamadoAtualizado = 2,
        MensagemChat = 3,
        ChamadoEmAnalise = 4,
        UnificacaoCarga = 5,
        EntregaAdicionada = 6,
        EntregaAlterada = 7,
        EntregaExcluida = 8,
        PushNotificationGenerica = 9,
        ParadaNaoProgramada = 10,
        CargaAtualizadaPorOutroMotorista = 11,
        DocumentosDeTransporteEmitidos = 12,
        EntregaConfirmadaNoEmbarcador = 13,
        EntregaRejeitadaNoEmbarcador = 14,
        EntregaPrevisaoAtualizada = 15,
        CargaMotoristaNecessitaConfirmar = 16, // Usado quando uma carga precisa confirmação do motorista
        NovaCargaMotorista = 17,
        MultiBus_PosicaoVeiculoRecebida = 18,
        MultiBus_AtualizacaoPassageiro = 19,
        NaoConformidade = 20,
        NaoConformidadeColetaAutorizada = 21
    }
}
