namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber
{
    public class RetornoCteTituloAcrescimosDecrescimos
    {
        public int status { get; set; }
        public string mensagem { get; set; }
        public RetornoCteTituloAcrescimosDecrescimosItem[] itens { get; set; }

    }
    public class RetornoCteTituloAcrescimosDecrescimosItem
    {
        public string descricao { get; set; }
        public string historico { get; set; }
        public string id { get; set; }
        public string tipo { get; set; }
        public decimal valor { get; set; }
    }

}
