namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusAdagio
    {
        Nenhum = 0,
        NaoAvaliado = 1,
        Aprovado = 2,
        Corrigido = 3,
        Reprovado = 4,
        ReprovadoEBloqueado = 5,
        AprovadoComRessalva  = 6,
        NaoCadastrado = 7,
        Ativo = 8,
        Inativo = 9,
        Pendente = 10,
        Bloqueado = 11
    }

    public static class StatusAdagioHelper
    {
        public static string Descricao(this StatusAdagio status)
        {
            switch (status)
            {
                case StatusAdagio.Nenhum:
                    return "Nenhum";
                case StatusAdagio.NaoAvaliado:
                    return "Não avaliado";
                case StatusAdagio.Aprovado:
                    return "Aprovado";
                case StatusAdagio.Corrigido:
                    return "Corrigido";
                case StatusAdagio.Reprovado:
                    return "Reprovado";
                case StatusAdagio.ReprovadoEBloqueado:
                    return "Reprovado e bloqueado";
                case StatusAdagio.AprovadoComRessalva:
                    return "Aprovado com ressalva";
                case StatusAdagio.NaoCadastrado:
                    return "Não cadastrado";
                case StatusAdagio.Ativo:
                    return "Ativo";
                case StatusAdagio.Inativo:
                    return "Inativo";
                case StatusAdagio.Pendente:
                    return "Pendente";
                case StatusAdagio.Bloqueado:
                    return "Bloqueado";
                default:
                    return "Desconhecido";
            }
        }
    }

}
