namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoNotaFiscal
    {
        Todas = 0,
        Pendente = 1,
        Processando = 2,
        Sucesso = 3,
        Erro = 4,
        Cancelado = 5
    }

    public static class SituacaoImportacaoNotaFiscalHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoNotaFiscal situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoNotaFiscal.Todas:
                    return "Todas";
                case SituacaoImportacaoNotaFiscal.Pendente:
                    return "Pendente";
                case SituacaoImportacaoNotaFiscal.Processando:
                    return "Processando";
                case SituacaoImportacaoNotaFiscal.Sucesso:
                    return "Sucesso";
                case SituacaoImportacaoNotaFiscal.Erro:
                    return "Erro";
                case SituacaoImportacaoNotaFiscal.Cancelado:
                    return "Cancelado";
                default:
                    return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoImportacaoNotaFiscal situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoNotaFiscal.Erro: 
                    return CorGrid.Vermelho;
                case SituacaoImportacaoNotaFiscal.Sucesso: 
                    return CorGrid.Verde;
                case SituacaoImportacaoNotaFiscal.Cancelado: 
                    return CorGrid.Vermelho;
                case SituacaoImportacaoNotaFiscal.Pendente: 
                    return CorGrid.Amarelo;
                case SituacaoImportacaoNotaFiscal.Processando: 
                    return CorGrid.Amarelo;
                default:
                    return string.Empty;
            }
        }
    }
}
