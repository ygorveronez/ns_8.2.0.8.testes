namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ControleAlertaTela
    {
        Veiculo = 1,
        Pessoa = 2,
        Motorista = 3,
        TabelaFrete = 4,
        Pedido = 5,
        Manutencao = 6,
        RegraICMS = 7,
        EstoqueMinimo = 8,
        OrdemServicoInterna = 9,
        OrdemServicoExterna = 10,
        CheckList = 11,
    }

    public static class ControleAlertaTelaHelper
    {
        public static string ObterDescricao(this ControleAlertaTela controleAlertaTela)
        {
            switch (controleAlertaTela)
            {
                case ControleAlertaTela.Veiculo: return "Veículo";
                case ControleAlertaTela.Pessoa: return "Pessoa";
                case ControleAlertaTela.Motorista: return "Motorista/Funcionário";
                case ControleAlertaTela.TabelaFrete: return "Tabela de Frete";
                case ControleAlertaTela.Pedido: return "Pedido";
                case ControleAlertaTela.Manutencao: return "Manutenção";
                case ControleAlertaTela.RegraICMS: return "Regra de ICMS";
                case ControleAlertaTela.EstoqueMinimo: return "Estoque Mínimo";
                case ControleAlertaTela.OrdemServicoInterna: return "Ordem de Serviço Interna";
                case ControleAlertaTela.OrdemServicoExterna: return "Ordem de Serviço Externa";
                case ControleAlertaTela.CheckList: return "Checklist";
                default: return string.Empty;
            }
        }
    }
}