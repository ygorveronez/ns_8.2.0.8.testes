namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRetornoCarga
    {
        Todas = 1,
        AgInformarRetorno = 2,
        GerandoCargaRetorno = 3,
        //FalhaGerarRetorno = 4,
        RetornoGerado = 5,
        CanceladoRetorno = 6,
        GerandoCargaRetornoColetaBackhaul = 7
    }

    public static class SituacaoRetornoCargaHelper
    {
        public static string ObterDescricao(this SituacaoRetornoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoRetornoCarga.AgInformarRetorno: return "Ag. Informar Retorno";
                case SituacaoRetornoCarga.CanceladoRetorno: return "Retorno Cancelado";
                //case SituacaoRetornoCarga.FalhaGerarRetorno: return "Falha ao gerar a carga de Retorno";
                case SituacaoRetornoCarga.GerandoCargaRetorno: return "Gerando Carga de Retorno";
                case SituacaoRetornoCarga.GerandoCargaRetornoColetaBackhaul: return "Gerando Carga de Retorno de Coleta";
                case SituacaoRetornoCarga.RetornoGerado: return "Retorno Gerado";
                case SituacaoRetornoCarga.Todas: return "Todas";
                default: return string.Empty;
            }
        }
    }
}
