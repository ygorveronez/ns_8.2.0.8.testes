namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemAlteracaoFilaCarregamento
    {
        Sistema = 0,
        Embarcador = 1,
        Transportador = 2,
        Motorista = 3,
        WebService = 4
    }

    public static class OrigemFilaCarregamentoVeiculoHistoricohelper
    {
        public static string ObterDescricao(this OrigemAlteracaoFilaCarregamento origem)
        {
            switch (origem)
            {
                case OrigemAlteracaoFilaCarregamento.Sistema: return "Sistema";
                case OrigemAlteracaoFilaCarregamento.Embarcador: return "Embarcador";
                case OrigemAlteracaoFilaCarregamento.Transportador: return "Transportador";
                case OrigemAlteracaoFilaCarregamento.Motorista: return "Motorista";
                case OrigemAlteracaoFilaCarregamento.WebService: return "Web Service";
                default: return string.Empty;
            }
        }
    }
}
