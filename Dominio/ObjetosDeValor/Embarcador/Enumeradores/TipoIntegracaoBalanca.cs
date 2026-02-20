namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoBalanca
    {
        CadastroVeiculo = 0,
        PesagemInicial = 2,
        Encerrado = 3,
        AguardandoLiberacao = 4,
        PesagemFinal = 5,
        Bloqueado = 10,

        //Diferente dos estados do WS
        RefazerPesagem = 20
    }

    public static class TipoIntegracaoBalancaHelper
    {
        public static string ObterDescricao(this TipoIntegracaoBalanca tipoIntegracaoBalanca)
        {
            switch (tipoIntegracaoBalanca)
            {
                case TipoIntegracaoBalanca.CadastroVeiculo: return "Cadastro Veículo";
                case TipoIntegracaoBalanca.PesagemInicial: return "Pesagem Inicial";
                case TipoIntegracaoBalanca.PesagemFinal: return "Pesagem Final";
                case TipoIntegracaoBalanca.Bloqueado: return "Bloqueado";
                case TipoIntegracaoBalanca.AguardandoLiberacao: return "Aguardando Liberação";
                case TipoIntegracaoBalanca.Encerrado: return "Encerrado";
                case TipoIntegracaoBalanca.RefazerPesagem: return "Refazer Pesagem";
                default: return string.Empty;
            }
        }
    }
}

/*Estados do Ticket da Toledo:
 CadastroVeiculo = 0, PreCadastro = 1, Pesagem Inicial = 2, Encerrado = 3, Aguardando Liberação = 4, PesagemFinal = 5, 
Cancelado = 6, Acesso Veiculo = 7, Pesagem Avulsa = 8, Verificação = 9, Bloqueado = 10, Pesagem Eixos = 11, Pesagem Dosagem = 12*/