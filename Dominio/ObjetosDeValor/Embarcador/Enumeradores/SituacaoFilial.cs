namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFilial
    {
        SemSitaucao = 0,
        RecibidoDeMRDR = 10,
        RequisitoDeCompra = 40,
        PrecoPadraoEstimado = 45,
        ProntoTodosUsuarios = 60,
        IntencaoDescontinuar = 70,
        EmDescontinuacao = 80,
        Descontinuado = 90,
        DesativadoCriadoComErro = 95
    }

    public static class SituacaoFilialHelper
    {
        public static string ObterDescricao(this SituacaoFilial situacao)
        {
            switch (situacao)
            {
                case SituacaoFilial.SemSitaucao: return "Sem Situação";
                case SituacaoFilial.RecibidoDeMRDR: return "Recibido de MRDR";
                case SituacaoFilial.RequisitoDeCompra: return "Requisito de compra";
                case SituacaoFilial.PrecoPadraoEstimado: return "Preco padrão estimado";
                case SituacaoFilial.ProntoTodosUsuarios: return "Pronto para todos os usuário";
                case SituacaoFilial.IntencaoDescontinuar: return "Intenção em descontinuar";
                case SituacaoFilial.EmDescontinuacao: return "Em descontinuação";
                case SituacaoFilial.Descontinuado: return "Descontinuado";
                case SituacaoFilial.DesativadoCriadoComErro: return "Desativado criado com error";
                default: return string.Empty;
            }
        }
    }
}
