namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class RequestCargasIntegracaoPendentes
    {
        public int Inicio { get; set; }
        public int Limite { get; set; }
        public string CodigoIntegracaoTipoOperacao { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
    }
}
