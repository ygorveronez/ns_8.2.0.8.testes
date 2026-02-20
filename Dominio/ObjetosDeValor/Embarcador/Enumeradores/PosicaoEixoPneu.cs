namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PosicaoEixoPneu
    {
        DireitoExterno = 1,
        DireitoInterno = 2,
        EsquerdoExterno = 3,
        EsquerdoInterno = 4
    }

    public static class PosicaoEixoPneuHelper
    {
        public static string ObterDescricao(this PosicaoEixoPneu posicao)
        {
            switch (posicao)
            {
                case PosicaoEixoPneu.DireitoExterno: return "Direito Externo";
                case PosicaoEixoPneu.DireitoInterno: return "Direito Interno";
                case PosicaoEixoPneu.EsquerdoExterno: return "Esquerdo Externo";
                case PosicaoEixoPneu.EsquerdoInterno: return "Esquerdo Interno";
                default: return string.Empty;
            }
        }

        public static bool IsLadoDireito(this PosicaoEixoPneu posicao)
        {
            return ((posicao == PosicaoEixoPneu.DireitoExterno) || (posicao == PosicaoEixoPneu.DireitoInterno));
        }
    }
}
