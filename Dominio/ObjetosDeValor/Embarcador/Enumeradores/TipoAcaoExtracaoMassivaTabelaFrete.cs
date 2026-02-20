namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcaoExtracaoMassivaTabelaFrete
    {
        RegistroDeAcao = 0,
        SemRegistroDeAcao = 1,
        ValorAdicionado = 2,
        ValorImportadorPorPlanilha = 3,
        ValorAtualizado = 4,
        Removido = 5,
    }

    public static class TipoAcaoExtracaoMassivaTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoAcaoExtracaoMassivaTabelaFrete tipoAcao)
        {
            switch (tipoAcao)
            {
                case TipoAcaoExtracaoMassivaTabelaFrete.RegistroDeAcao: return "Registro de Ação";
                case TipoAcaoExtracaoMassivaTabelaFrete.SemRegistroDeAcao: return "Sem Registro de Ação";
                case TipoAcaoExtracaoMassivaTabelaFrete.ValorAdicionado: return "Adicionado";
                case TipoAcaoExtracaoMassivaTabelaFrete.ValorImportadorPorPlanilha: return "Importou Tabela Frete";
                case TipoAcaoExtracaoMassivaTabelaFrete.ValorAtualizado: return "Atualizado";
                case TipoAcaoExtracaoMassivaTabelaFrete.Removido: return "Removido";
                default: return string.Empty;
            }
        }
    }
}
