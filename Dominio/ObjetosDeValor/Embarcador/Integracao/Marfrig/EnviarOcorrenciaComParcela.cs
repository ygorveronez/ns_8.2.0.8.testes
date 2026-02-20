namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class EnviarOcorrenciaComParcela
    {
        public string acao { get; set; }
        public string dataEmissao { get; set; }
        public string numeroND { get; set; }
        public string serieND { get; set; }
        public string cnpjRemetente { get; set; }
        public string cnpjTomador { get; set; }
        public string transportador { get; set; }
        public string ocorrenciaMulti { get; set; }
        public string carga { get; set; }
        public string motivo { get; set; }
        public string descricaomotivo { get; set; }
        public decimal qtdservicos { get; set; }
        public string problema { get; set; }
        public string cteOrigem { get; set; }
        public string serieOrigem { get; set; }
        public string nfeOrigem { get; set; }
        public decimal valorDocumento { get; set; }
        public int qtdParcela { get; set; }
    }
}
