namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarGuiaTransporteAnimal
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string codigoBarras { get; set; }
        public string numeroNF { get; set; }
        public string serie { get; set; }
        public string uf { get; set; }
        public int quantidade { get; set; }
    }
}
