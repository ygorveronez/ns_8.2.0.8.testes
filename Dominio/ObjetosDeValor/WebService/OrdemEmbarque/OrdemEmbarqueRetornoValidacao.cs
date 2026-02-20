namespace Dominio.ObjetosDeValor.WebService.OrdemEmbarque
{
    public sealed class OrdemEmbarqueRetornoValidacao
    {
        public string CampoReferencia { get; set; }

        public string CampoValidado { get; set; }

        public string Mensagem { get; set; }

        public string NumeroReferencia { get; set; }

        public string TipoValidacao { get; set; }
    }
}
