namespace Dominio.ObjetosDeValor.WebService.Pallet
{
    public class MovimentacaoPallet
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoPallet TipoMovimentacaoPallet { get; set; }        
        public Embarcador.Pessoas.Empresa Transportador { get; set; }
        public Embarcador.Filial.Filial Filial { get; set; }
        public int ProtocoloCarga { get; set; }
        public string DataMovimentacao { get; set; }
        public string Observacao { get; set; }
        public string NumeroDocumento { get; set; }
        public int Quantidade { get; set; }
    }
}
