namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaPagamento
    {
        Nenhum = 0,
        Avista = 1,
        GerarTituloAutomaticamente = 2,
    }

    public static class FormaPagamentoHelper
    {
        public static string ObterDescricao(this FormaPagamento situacao)
        {
            switch (situacao)
            {
                case FormaPagamento.Avista: return "Á vista";
                case FormaPagamento.GerarTituloAutomaticamente: return "Gerar Título Automaticamente";
                default: return "Nenhum";
            }
        }      
    }
}
