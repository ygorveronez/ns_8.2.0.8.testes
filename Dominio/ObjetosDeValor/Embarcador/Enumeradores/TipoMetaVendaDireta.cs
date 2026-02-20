namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMetaVendaDireta
    {
        Todos = 0,
        Agendamento = 1,
        Servico = 2,
        Produto = 3,
        Validacao = 4
    }

    public static class TipoMetaVendaDiretaHelper
    {
        public static string ObterDescricao(this TipoMetaVendaDireta tipoCobranca)
        {
            switch (tipoCobranca)
            {
                case TipoMetaVendaDireta.Agendamento: return "Agendamento";
                case TipoMetaVendaDireta.Servico: return "Venda de Serviço";
                case TipoMetaVendaDireta.Produto: return "Venda de Produto";
                case TipoMetaVendaDireta.Validacao: return "Validação";                
                default: return string.Empty;
            }
        }
    }
}
