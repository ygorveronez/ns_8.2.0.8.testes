using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RegraPallet
    {
        Devolucao = 1,
        CanhotoAssinado = 2,
        ValePallet = 3,
        Estoque = 4,
        Emprestimo = 5,
        Transferencia = 6
    }

    public static class RegraPalletHelper
    {
        public static string ObterDescricao(this RegraPallet regraPallet)
        {
            switch (regraPallet)
            {
                case RegraPallet.Devolucao: return "Devolução no Ato";
                case RegraPallet.CanhotoAssinado: return "Canhoto Assinado e Carimbado";
                case RegraPallet.ValePallet: return "Vale Pallet";
                case RegraPallet.Estoque: return "Estoque (Pulmão)";
                case RegraPallet.Emprestimo: return "Estoque (Empréstimo)";
                case RegraPallet.Transferencia: return "Transferência";
                default: return string.Empty;
            }
        }

        public static bool IsRegraCIF(this RegraPallet regraPallet)
        {
            if (regraPallet == RegraPallet.ValePallet || regraPallet == RegraPallet.Devolucao || regraPallet == RegraPallet.CanhotoAssinado)
                return true;

            return false;
        }

        public static bool IsRegraClienteValida(this RegraPallet regraPallet)
        {
            List<RegraPallet> regrasValidas = new List<RegraPallet>()
            {
                RegraPallet.Devolucao,
                RegraPallet.CanhotoAssinado,
                RegraPallet.ValePallet,
                RegraPallet.Estoque
            };

            return regrasValidas.Contains(regraPallet);
        }
    }
}
