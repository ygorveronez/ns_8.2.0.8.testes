namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class RetornoProcessamentoIntegracaoEDIFTP
    {
        public bool Sucesso { get; set; }
        public int Total { get; set; }
        public int Importados { get; set; }
        public string MensagemAviso { get; set; }
        public object Retorno { get; set; }


    }
}
