namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retCartao
    {
        public int? id { get; set; }
        public int? solicitacaoID { get; set; }
        public int? statusID { get; set; }
        public string privateLabel { get; set; }
        public string panMask { get; set; }
        public string cartaBerco { get; set; }
        public string dataInclusao { get; set; }
        public string dataAtualizacao { get; set; }
        public string dataVencimento { get; set; }
        public string numeroCartao { get; set; }
        public string idCardExterno { get; set; }
        public string cartaoNumeroCompleto { get; set; }
    }
}