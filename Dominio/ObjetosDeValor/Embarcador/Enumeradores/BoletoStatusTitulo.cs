namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum BoletoStatusTitulo
    {
        Nenhum = 0,
        Emitido = 1,
        Gerado = 2,
        Erro = 3,
        EmGeracao = 4,
        AguardandoRemessa = 5,
        ComRemessaEBoleto = 6
    }

    public static class BoletoStatusTituloHelper
    {
        public static string ObterDescricao(this BoletoStatusTitulo status)
        {
            switch (status)
            {
                case BoletoStatusTitulo.AguardandoRemessa: return "Aguardando Remessa";
                case BoletoStatusTitulo.ComRemessaEBoleto: return "Com Remessa e Boleto";
                case BoletoStatusTitulo.EmGeracao: return "Em Geração";
                case BoletoStatusTitulo.Emitido: return "Emitido";
                case BoletoStatusTitulo.Erro: return "Erro na geração";
                case BoletoStatusTitulo.Gerado: return "Com Boleto";
                case BoletoStatusTitulo.Nenhum: return "Nenhum";
                default: return string.Empty;
            }
        }
    }
}
