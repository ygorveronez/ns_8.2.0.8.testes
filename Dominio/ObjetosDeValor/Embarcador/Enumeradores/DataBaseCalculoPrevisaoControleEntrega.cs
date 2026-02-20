namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DataBaseCalculoPrevisaoControleEntrega
    {
        DataCriacaoCarga = 1,
        DataPrevisaoTerminoCarga = 2,
        DataInicioViagemPrevista = 3,
        DataCarregamentoCarga = 4,
        DataInicioCarregamentoJanela = 5
    }

    public static class DataBaseCalculoPrevisaoControleEntregaHelper
    {
        public static  string ObterDescricao(this DataBaseCalculoPrevisaoControleEntrega dataBaseCalculo)
        {
            switch (dataBaseCalculo)
            {
                case DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga: return "Data Criação da Carga";
                case DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga: return "Data Previsão Termino Carga";
                case DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela: return "Data Inicio Janela Carregamento";
                case DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga: return "Data Carregamento Carga";
                case DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista: return "Data Inicio Viagem Prevista";
                default: return string.Empty;
            }
        }
    }
}
