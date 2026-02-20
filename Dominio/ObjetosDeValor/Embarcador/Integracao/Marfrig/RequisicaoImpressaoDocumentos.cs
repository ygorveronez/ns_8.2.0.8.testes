namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class RequisicaoImpressaoDocumentos
    {
        public int status { get; set; }
        public int numeroOrdemEmbarque { get; set; }
        public string idEmpresa { get; set; }
        public string mensagem { get; set; }
    }
}
