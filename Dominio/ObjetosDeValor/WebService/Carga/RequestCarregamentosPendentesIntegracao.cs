namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class RequestCarregamentosPendentesIntegracao
    {
        public int Inicio { get; set; }
        public int Limite { get; set; }
        public string CodigoFilial { get; set; }
        public string CodigoRegiao { get; set; }
    }
}
