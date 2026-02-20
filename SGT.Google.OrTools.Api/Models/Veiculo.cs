namespace Google.OrTools.Api.Models
{
    public class Veiculo
    { 
        public int Codigo { get; set; }

        public int CodigoModelo { get; set; }

        public string Descricao { get; set; }

        public int Quantidade { get; set; }

        public long Capacidade { get; set; }

        public int QtdeMaximaEntregas { get; set; }

        public int QtdeMinimaEntregas { get; set; }
    }
}