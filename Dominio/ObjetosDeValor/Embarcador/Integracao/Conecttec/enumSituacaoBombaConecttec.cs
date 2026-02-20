using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{
    public enum SituacaoBombaConecttec
    {
        NOT_RESPONDING = 0,
        BLOCKED = 1,
        AUTHORISED = 2,
        CALLING = 3,
        DELIVERING = 4,
        TEMP_STOPED = 5,
        DELIVERY_END = 6,
        AUTH_INTERRUPTED = 7
    }

    public static class SituacaoBombaConecttecHelper
    {
        public static string ObterDescricao(this SituacaoBombaConecttec situacao)
        {
            switch (situacao)
            {
                case SituacaoBombaConecttec.NOT_RESPONDING: return "Bico NÃO RESPONDE. Indica que não há comunicação entre na automação e o bico";
                case SituacaoBombaConecttec.BLOCKED: return "Bico BLOQUEADO. Quando o bico se encontra posicionado no descanso da bomba.";
                case SituacaoBombaConecttec.AUTHORISED: return "Bico AUTORIZADO. Momento em que o frentista retira o bico do descanso e a bomba está liberada para abastecer.Também é um status que pode aparecer em casos em que há identificação de frentista, esse status se manifesta após a identificação do frentista que irá abastecer.";
                case SituacaoBombaConecttec.CALLING: return "Bico CHAMANDO. Esse status ocorre em casos em que a liberação da bomba depende de identificação via cartão de frentista. No momento em que o bico é levantado pelo frentista e não é realizada a identificação, o bico fica com status CALLING até o momento em que seja identificado.";
                case SituacaoBombaConecttec.DELIVERING: return "Bico ABASTECENDO. Esse status ocorre no momento em que o bico está abastecendo, ou seja, despejando combustível.";
                case SituacaoBombaConecttec.TEMP_STOPED: return "Bico DESABILITADO TEMPORÁRIAMENTE. O bico pode napresentar esse status caso seja desabilitado (não excluído) via configuração, também pode ter recebido uma parada inesperada por parte de um sistema de gestão.";
                case SituacaoBombaConecttec.DELIVERY_END: return "ABASTECIMENTO FINALIZADO. Indica que o abastecimento nchegou ao fim.";
                case SituacaoBombaConecttec.AUTH_INTERRUPTED: return "Autorização da bomba interrompida";
                default: return string.Empty;
            }
        }
    }
}
