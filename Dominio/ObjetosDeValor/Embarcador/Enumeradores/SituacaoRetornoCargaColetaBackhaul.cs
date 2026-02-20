namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRetornoCargaColetaBackhaul
    {
        AguardandoGerarCarga = 1,
        GerandoCarga = 2,
        CargaGerada = 3,
        RetornoCancelado = 4
    }

    public static class SituacaoRetornoCargaColetaBackhaulHelper
    {
        public static string ObterDescricao(this SituacaoRetornoCargaColetaBackhaul situacao)
        {
            switch (situacao)
            {
                case SituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga: return "Aguardando Gerar a Carga";
                case SituacaoRetornoCargaColetaBackhaul.CargaGerada: return "Carga Gerada";
                case SituacaoRetornoCargaColetaBackhaul.GerandoCarga: return "Gerando Carga";
                case SituacaoRetornoCargaColetaBackhaul.RetornoCancelado: return "Retorno Cancelado";
                default: return string.Empty;
            }
        }
    }
}
