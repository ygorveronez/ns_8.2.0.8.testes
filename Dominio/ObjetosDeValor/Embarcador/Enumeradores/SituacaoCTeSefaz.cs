namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCTeSefaz
    {
        Autorizada = 1,
        Cancelada = 2,
        Denegada = 3,
        Rejeitada = 4,
        Pendente = 5,
        Enviada = 6,
        Inutilizada = 7,
        EmDigitacao = 8,
        EmCancelamento = 9,
        EmInutilizacao = 10,
        Anulado = 11,
        AguardandoAssinatura = 12,
        AguardandoAssinaturaCancelamento = 13,
        AguardandoAssinaturaInutilizacao = 14,
        AguardandoEmissaoEmail = 15,
        AguardandoNFSe = 16,
        ContingenciaFSDA = 17,
        AnuladoGerencialmente = 18
    }

    public static class SituacaoCTeSefazHelper
    {
        public static string ObterDescricao(this SituacaoCTeSefaz situacao)
        {
            switch (situacao)
            {
                case SituacaoCTeSefaz.Autorizada: return "Autorizado";
                case SituacaoCTeSefaz.Cancelada: return "Cancelado";
                case SituacaoCTeSefaz.Denegada: return "Denegado";
                case SituacaoCTeSefaz.Rejeitada: return "Rejeitado";
                case SituacaoCTeSefaz.Pendente: return "Pendente";
                case SituacaoCTeSefaz.Enviada: return "Enviado";
                case SituacaoCTeSefaz.Inutilizada: return "Inutilizado";
                case SituacaoCTeSefaz.EmDigitacao: return "Em Digitação";
                case SituacaoCTeSefaz.EmCancelamento: return "Em Cancelamento";
                case SituacaoCTeSefaz.EmInutilizacao: return "Em Inutilização";
                case SituacaoCTeSefaz.Anulado: return "Anulado";
                case SituacaoCTeSefaz.AguardandoAssinatura: return "Ag. Assinatura";
                case SituacaoCTeSefaz.AguardandoAssinaturaCancelamento: return "Ag. Assinatura Cancelamento";
                case SituacaoCTeSefaz.AguardandoAssinaturaInutilizacao: return "Ag. Assinatura Inutilização";
                case SituacaoCTeSefaz.AguardandoEmissaoEmail: return "Ag. Emissão e-mail";
                case SituacaoCTeSefaz.AguardandoNFSe: return "Ag. NFS-e";
                case SituacaoCTeSefaz.ContingenciaFSDA: return "Contingência FSDA";
                case SituacaoCTeSefaz.AnuladoGerencialmente: return "Anulado Gerencialmente";
                default: return "";
            }
        }
    }
}
