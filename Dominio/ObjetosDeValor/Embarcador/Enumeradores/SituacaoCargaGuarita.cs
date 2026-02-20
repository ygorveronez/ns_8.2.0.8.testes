namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaGuarita
    {
        AguardandoLiberacao = 0,
        Liberada = 1,
        AgChegadaVeiculo = 2,
        SaidaLiberada = 3
    }

    public static class SituacaoCargaGuaritaHelper
    {
        public static string ObterDescricao(this SituacaoCargaGuarita situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaGuarita.AgChegadaVeiculo: return "Ag. Chegada do Veículo";
                case SituacaoCargaGuarita.AguardandoLiberacao: return "Aguardando Entrada";
                case SituacaoCargaGuarita.Liberada: return "Entrada Liberada";
                case SituacaoCargaGuarita.SaidaLiberada: return "Veículo Saiu";
                default: return string.Empty;
            }
        }
    }
}
