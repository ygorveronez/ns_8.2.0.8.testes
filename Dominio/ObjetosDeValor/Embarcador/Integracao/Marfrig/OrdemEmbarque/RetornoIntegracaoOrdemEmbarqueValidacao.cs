namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class RetornoIntegracaoOrdemEmbarqueValidacao
    {
        public string CampoReferencia { get; set; }

        public string CampoValidado { get; set; }

        public string Mensagem { get; set; }

        public string NumeroReferencia { get; set; }

        public string TipoValidacao { get; set; }
    }
}
