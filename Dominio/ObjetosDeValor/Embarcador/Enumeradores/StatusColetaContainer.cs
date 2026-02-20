using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusColetaContainer
    {
        AgColeta = 0,
        EmDeslocamentoVazio = 1,
        EmAreaEsperaVazio = 2,
        EmDeslocamentoCarregamento = 3,
        EmDeslocamentoCarregado = 4,
        EmAreaEsperaCarregado = 5,
        Porto = 6,
        Cancelado = 7,
        EmCarregamento = 8,
        EmbarcadoNavio = 9
    }

    public static class StatusColetaContainerHelper
    {
        public static string ObterDescricao(this StatusColetaContainer status)
        {
            switch (status)
            {
                case StatusColetaContainer.AgColeta: return "Aguardando Coleta";
                case StatusColetaContainer.EmDeslocamentoVazio: return "Em Deslocamento Vazio";
                case StatusColetaContainer.EmAreaEsperaVazio: return "Em Área de espera Vazio";
                case StatusColetaContainer.EmDeslocamentoCarregamento: return "Em Deslocamento para Carregamento";
                case StatusColetaContainer.EmCarregamento: return "Em Carregamento";
                case StatusColetaContainer.EmDeslocamentoCarregado: return "Em Deslocamento Carregado";
                case StatusColetaContainer.EmAreaEsperaCarregado: return "Em Área de espera Carregado";
                case StatusColetaContainer.Porto: return "Porto";
                case StatusColetaContainer.Cancelado: return "Cancelado";
                case StatusColetaContainer.EmbarcadoNavio : return "Embarcado Navio";
                default: return "";
            }
        }

        public static List<StatusColetaContainer> ObterStatusEmPosse()
        {
            return new List<StatusColetaContainer>()
            {
                StatusColetaContainer.EmDeslocamentoVazio,
                StatusColetaContainer.EmAreaEsperaVazio,
                StatusColetaContainer.EmDeslocamentoCarregamento,
                StatusColetaContainer.EmCarregamento,
                StatusColetaContainer.EmDeslocamentoCarregado,
                StatusColetaContainer.EmAreaEsperaCarregado,
            };
        }

        public static List<StatusColetaContainer> ObterSituacoesAnteriores(this StatusColetaContainer status)
        {
            List<StatusColetaContainer> situacoesColetaCotainer = new List<StatusColetaContainer>()
            {
                StatusColetaContainer.AgColeta,
                StatusColetaContainer.EmDeslocamentoVazio,
                StatusColetaContainer.EmAreaEsperaVazio,
                StatusColetaContainer.EmDeslocamentoCarregamento,
                StatusColetaContainer.EmCarregamento,
                StatusColetaContainer.EmDeslocamentoCarregado,
                StatusColetaContainer.EmAreaEsperaCarregado,
                StatusColetaContainer.Porto,
            };

            List<StatusColetaContainer> situacoesAnteriores = new List<StatusColetaContainer>();

            foreach (StatusColetaContainer situacaoColetaContainer in situacoesColetaCotainer)
            {
                situacoesAnteriores.Add(situacaoColetaContainer);

                if (situacaoColetaContainer == status)
                    break;
            }

            return situacoesAnteriores;
        }
    }
}
