namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public sealed class RetornoConfirmacaoColeta
    {
        public string NumeroDaCarga { get; set; }
        public string IdExterno { get; set; }
        public string Operacao { get; set; }
        public string ErroConfirmacao { get; set; }
        public string DataConfirmacao { get; set; }
        public string HoraConfirmacao { get; set; }
    }
}
