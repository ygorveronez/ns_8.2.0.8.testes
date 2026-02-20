namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCadastroVeiculo
    {
        Todos = 0,
        Aprovado = 1,
        Pendente = 2,
        Rejeitada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoCadastroVeiculoHelper
    {
        public static string ObterDescricao(this SituacaoCadastroVeiculo situacao)
        {
            switch (situacao)
            {
                case SituacaoCadastroVeiculo.Aprovado: return "Aprovado";
                case SituacaoCadastroVeiculo.Pendente: return "Pendente";
                case SituacaoCadastroVeiculo.Rejeitada: return "Rejeitada";
                case SituacaoCadastroVeiculo.SemRegraAprovacao: return "Sem Regra";
                default: return string.Empty;
            }
        }
    }
}
