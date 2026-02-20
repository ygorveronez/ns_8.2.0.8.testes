namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class RetornoImpressaoDocumentos
    {
        public bool success { get; set; }
        public string[] errors { get; set; }
        public data data { get; set; }
    }

    public class data
    {
        public int status { get; set; }
        public int idEmpresa { get; set; }
        public int idOrdemTransporte { get; set; }
        public string mensagem { get; set; }
    }
}
