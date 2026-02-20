namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAgendamentoColeta
    {
        DadosTransporte = 1,
        NFe = 2,
        NFeCancelada = 5,
        Emissao = 3,
        AguardandoAceite = 4,
        DocumentoParaTransporte = 6
    }

    public static class EtapaAgendamentoColetaHelper
    {
        public static string ObterDescricao(this EtapaAgendamentoColeta situacao)
        {
            switch (situacao)
            {
                case EtapaAgendamentoColeta.AguardandoAceite: return "Aguardando Aceite";
                case EtapaAgendamentoColeta.DadosTransporte: return "Dados Transporte";
                case EtapaAgendamentoColeta.NFe: return "NF-e";
                case EtapaAgendamentoColeta.NFeCancelada: return "NF-e Cancelada";
                case EtapaAgendamentoColeta.Emissao: return "Emissão";
                case EtapaAgendamentoColeta.DocumentoParaTransporte: return "Documento Para Transporte";
                default: return string.Empty;
            }
        }
    }
}
