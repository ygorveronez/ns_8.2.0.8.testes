namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum IconesNotificacao
    {
        cifra = 1,
        confirmado = 2,
        rejeitado = 3,
        agConfirmacao = 4,
        estornado = 5,
        pdf = 6,
        excel = 7,
        falha = 8,
        janelaCargaDisponibilizada = 9,
        janelaMarcouInteresse = 10,
        janelaTransportadorEscolhido = 11,
        sucesso = 12,
        arquivoCompactado = 13,
        atencao = 14,
        ocorrenciaPedido = 15
    }

    public static class IconesNotificacaoDescricao
    {
        public static string ObterDescricao(this IconesNotificacao icone)
        {
            switch (icone)
            {
                case IconesNotificacao.atencao: return "fa-exclamation-circle";
                case IconesNotificacao.cifra: return "fa-dollar-sign";
                case IconesNotificacao.confirmado: return "fa-thumbs-up";//return "fa-check";
                case IconesNotificacao.rejeitado: return "fa-thumbs-down";//return "fa-ban";
                case IconesNotificacao.agConfirmacao: return "fa-unlock-alt";
                case IconesNotificacao.estornado: return "fa-reply-all";
                case IconesNotificacao.pdf: return "fa-file-pdf";
                case IconesNotificacao.excel: return "fa-file-excel";
                case IconesNotificacao.falha: return "fa-exclamation";
                case IconesNotificacao.sucesso: return "fa-check";
                case IconesNotificacao.janelaCargaDisponibilizada: return "fa-hand-paper";
                case IconesNotificacao.janelaMarcouInteresse: return "fa-thumbs-up";
                case IconesNotificacao.janelaTransportadorEscolhido: return "fa-thumbs-up";
                case IconesNotificacao.arquivoCompactado: return "fa-file-archive";
                case IconesNotificacao.ocorrenciaPedido: return "fa-comment";
                default: return "";
            }
        }

        public static SmartAdminBgColor ObterCorFundoPadrao(this IconesNotificacao icone)
        {
            switch (icone)
            {
                case IconesNotificacao.agConfirmacao:
                    return SmartAdminBgColor.orangeDark;

                case IconesNotificacao.arquivoCompactado:
                    return SmartAdminBgColor.greenLight;

                case IconesNotificacao.atencao:
                    return SmartAdminBgColor.orange;

                case IconesNotificacao.cifra:
                case IconesNotificacao.janelaCargaDisponibilizada:
                    return SmartAdminBgColor.teal;

                case IconesNotificacao.confirmado:
                case IconesNotificacao.janelaTransportadorEscolhido:
                case IconesNotificacao.sucesso:
                    return SmartAdminBgColor.green;

                case IconesNotificacao.falha:
                case IconesNotificacao.pdf:
                case IconesNotificacao.rejeitado:
                    return SmartAdminBgColor.red;

                case IconesNotificacao.estornado:
                    return SmartAdminBgColor.redLight;

                case IconesNotificacao.excel:
                    return SmartAdminBgColor.greenDark;

                case IconesNotificacao.janelaMarcouInteresse:
                    return SmartAdminBgColor.yellow;

                default:
                    return SmartAdminBgColor.black;
            }
        }
    }
}
