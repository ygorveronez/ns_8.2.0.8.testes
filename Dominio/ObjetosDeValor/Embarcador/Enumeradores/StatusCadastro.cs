namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusCadastro
    {
        Cadastrado = 0,
        Pendente = 1,
        Treinado = 2,
        NaoSeAplica = 3
    }

    public static class StatusCadastroHelper
    {
        public static string ObterDescricao(this StatusCadastro tipoCobranca)
        {
            switch (tipoCobranca)
            {
                case StatusCadastro.Cadastrado: return "Cadastrado";
                case StatusCadastro.Pendente: return "Pendente";
                case StatusCadastro.Treinado: return "Treinado";
                case StatusCadastro.NaoSeAplica: return "Não se aplica";
                default: return "";
            }
        }
    }
}
