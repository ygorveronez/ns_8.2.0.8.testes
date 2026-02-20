using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores
{
    public enum StatusTarefa
    {
        Aguardando = 0,
        EmProcessamento = 1,
        Concluida = 2,
        Falha = 3
    }

    public static class StatusTarefaExtensions
    {
        public static string ObterDescricao(this StatusTarefa statusTarefa)
        {
            switch (statusTarefa)
            {
                case StatusTarefa.Aguardando:
                    return "Aguardando";
                case StatusTarefa.EmProcessamento:
                    return "Em Processamento";
                case StatusTarefa.Concluida:
                    return "Concluída";
                case StatusTarefa.Falha:
                    return "Falha";
                default:
                    return string.Empty;
            }
        }

        public static string ObterCorFonte(this StatusTarefa statusTarefa)
        {
            return statusTarefa switch
            {
                StatusTarefa.Falha => CorGrid.Branco,
                _ => string.Empty
            };
        }

        public static string ObterCorLinha(this StatusTarefa statusTarefa)
        {
            return statusTarefa switch
            {
                StatusTarefa.EmProcessamento => CorGrid.Azul,
                StatusTarefa.Aguardando => CorGrid.Branco,
                StatusTarefa.Concluida => CorGrid.Verde,
                StatusTarefa.Falha => CorGrid.Vermelho,
                _ => string.Empty,
            };
        }
    }
}
