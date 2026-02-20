namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum InformacoesRelatorioCarga
    {
        Ambas = 0,
        SomenteCargas = 1,
        SomentePreCargas = 2
    }

    public static class InformacoesRelatorioCargaHelper
    {
        public static string ObterDescricao(this InformacoesRelatorioCarga tipoInformacao)
        {
            switch (tipoInformacao)
            {
                case InformacoesRelatorioCarga.Ambas: return "Cargas e Pré-Cargas";
                case InformacoesRelatorioCarga.SomenteCargas: return "Somente Cargas";
                case InformacoesRelatorioCarga.SomentePreCargas: return "Somente Pré-Cargas";
                default: return string.Empty;
            }
        }
    }
}
