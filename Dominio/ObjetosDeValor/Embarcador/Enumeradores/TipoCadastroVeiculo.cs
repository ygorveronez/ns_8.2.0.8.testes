namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCadastroVeiculo
    {
        Adicionar = 1,
        Atualizar = 2,
    }

    public static class TipoCadastroVeiculoHelper
    {
        public static string ObterDescricao(this TipoCadastroVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoCadastroVeiculo.Adicionar: return "Adicionar";
                case TipoCadastroVeiculo.Atualizar: return "Atualizar";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoAcao(this TipoCadastroVeiculo tipo)
        {
            switch (tipo)
            {
                case TipoCadastroVeiculo.Adicionar: return "adicionou";
                case TipoCadastroVeiculo.Atualizar: return "atualizou";
                default: return string.Empty;
            }
        }
    }
}
