namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos
{
    public class Ocorrencia
    {
        public int Codigo { get; set; }
        public int NumeroOcorrencia { get; set; }
        public string TipoOcorrencia { get; set; }
        public string DescricaoSituacaoOcorrencia { get; set; }
        public string OrigemOcorrencia { get; set; }
        public string DestinoOcorrencia { get; set; }
        public decimal ValorOcorrencia { get; set; }
        public string ObservacaoOcorrencia { get; set; }
        public decimal ParametroHoras { get; set; }
    }
}
