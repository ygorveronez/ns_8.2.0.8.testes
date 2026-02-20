namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoMontagemCarregamentoBloco
    {
        Pendente = 0,
        GerandoSimulacao = 1,
        Sucesso = 2,
        GeradoParcial = 3,
        CadastroIncompleto = 4,
        ErroCalcularFrete = 5,
        ValorMinimoCargaNaoAtendido = 6,
        Erro = 9
    }

    public static class SituacaoMontagemCarregamentoBlocoHelper
    {
        public static string ObterDescricao(this SituacaoMontagemCarregamentoBloco situacao)
        {
            switch (situacao)
            {
                case SituacaoMontagemCarregamentoBloco.Pendente: return "Pendente";
                case SituacaoMontagemCarregamentoBloco.GerandoSimulacao: return "Gerando Simulação de Frete";
                case SituacaoMontagemCarregamentoBloco.Sucesso: return "Sucesso";
                case SituacaoMontagemCarregamentoBloco.GeradoParcial: return "Gerado Parcial";
                case SituacaoMontagemCarregamentoBloco.CadastroIncompleto: return "Cadastro Incompleto";
                case SituacaoMontagemCarregamentoBloco.ErroCalcularFrete: return "Erro Cálculo Frete";
                case SituacaoMontagemCarregamentoBloco.ValorMinimoCargaNaoAtendido: return "Valor mínimo da carga não atendido";
                case SituacaoMontagemCarregamentoBloco.Erro: return "Erro ao Simular Frete";
                default: return "";
            }
        }
    }
}
