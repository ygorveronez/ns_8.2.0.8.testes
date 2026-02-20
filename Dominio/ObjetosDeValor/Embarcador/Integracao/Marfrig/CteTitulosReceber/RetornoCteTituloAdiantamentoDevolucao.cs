namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber
{
    public class RetornoCteTituloAdiantamentoDevolucao
    {
        public int status { get; set; }
        public string mensagem { get; set; }
        public RetornoCteTituloAdiantamentoDevolucaoItem[] itens { get; set; }

    }
    public class RetornoCteTituloAdiantamentoDevolucaoItem
    {
        public string agencia { get; set; }
        public string banco { get; set; }
        public string conta { get; set; }
        public string data { get; set; }
        public string documento { get; set; }
        public string historico { get; set; }
        public string motivo { get; set; }
        public decimal valorPago { get; set; }
    }

}
