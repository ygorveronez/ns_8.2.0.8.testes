namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaControleExpedicao
    {
        Todas = 0,
        AguardandoLiberacao = 1,
        Liberada = 2,
        PlacaDivergente = 3,
        AgInicioCarregamento = 4
    }

    public static class SituacaoCargaControleExpedicaoHelper
    {
        public static string ObterCorFonte(this SituacaoCargaControleExpedicao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaControleExpedicao.PlacaDivergente: return CorGrid.Branco;
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoCargaControleExpedicao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaControleExpedicao.PlacaDivergente: return CorGrid.Vermelho;
                case SituacaoCargaControleExpedicao.Liberada: return CorGrid.Verde;
                default: return CorGrid.Amarelo;
            }
        }

        public static string ObterDescricao(this SituacaoCargaControleExpedicao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaControleExpedicao.AguardandoLiberacao: return "Em Carregamento";
                case SituacaoCargaControleExpedicao.Liberada: return "Liberada";
                case SituacaoCargaControleExpedicao.PlacaDivergente: return "Placa Divergente";
                case SituacaoCargaControleExpedicao.AgInicioCarregamento: return "Ag. Inicio Carregamento";
                default: return string.Empty;
            }
        }
    }
}
