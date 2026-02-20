namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFretePagamentoFreteInformacoesObrigatoriasQuitacao
    {
        public bool dataEntrega { get; set; }
        public bool peso { get; set; }
        public bool ticketBalanca { get; set; }
        public bool avaria { get; set; }
        public bool canhotoNFe { get; set; }
        public bool comprovantePedagio { get; set; }
        public bool DACTE { get; set; }
        public bool contratoTransporte { get; set; }
        public bool dataDesembarque { get; set; }
        public bool relatorioInspecaoDesembarque { get; set; }
    }
}
