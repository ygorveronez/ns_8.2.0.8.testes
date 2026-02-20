namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FreteCliente
    {
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Tabela { get; set; }
        public string ContratoFrete { get; set; }
        public bool PermiteAlterarValorFrete { get; set; }
        public Enumeradores.TipoCalculoTabelaFrete TipoCalculo { get; set; }
    }
}
