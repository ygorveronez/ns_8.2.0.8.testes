namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NivelSatisfacao
    {
        NaoAvaliado = 4,
        Ruim = 1,
        Bom = 2,
        Otimo = 3
    }

    public static class NivelSatisfacaoHelper
    {
        public static string ObterDescricao(this NivelSatisfacao nivelSatisfacao)
        {
            switch (nivelSatisfacao)
            {
                case NivelSatisfacao.Otimo: return "Ótimo";
                case NivelSatisfacao.Bom: return "Bom";
                case NivelSatisfacao.Ruim: return "Ruim";
                case NivelSatisfacao.NaoAvaliado: return "Não Avaliado";
                default: return string.Empty;
            }
        }
    }
}
