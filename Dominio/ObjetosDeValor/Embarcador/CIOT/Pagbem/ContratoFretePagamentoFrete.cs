using System.Collections.Generic;


namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFretePagamentoFrete
    {
        public ContratoFretePagamentoFreteFreteQuitacaoRegras freteQuitacaoRegras { get; set; }
        public ContratoFretePagamentoFreteInformacoesObrigatoriasQuitacao informacoesObrigatoriasQuitacao { get; set; }
        public string meioPagamentoFrete { get; set; }
        public string localQuitacao { get; set; }
        public string localAdiantamento { get; set; }
        public List<ContratoFretePagamentoFreteAdiantamento> adiantamento { get; set; }
        public ContratoFretePagamentoFreteParticipacao participacao { get; set; }
        public bool destinacaoComercial { get; set; }
    }
}
