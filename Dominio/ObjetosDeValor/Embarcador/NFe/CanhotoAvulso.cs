namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class CanhotoAvulso
    {
        public int Numero { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }
        public string DataEmissao { get; set; }
        public string PDF { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
    }
}
