namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPedidoMontagemCarregamento
    {
        Card = 0,
        Tabela = 1
    }

    public enum TipoEdicaoPalletProdutoMontagemCarregamento
    {
        ControlePalletAbertoFechado = 0,    // Controle Pallet (Fechado Não, Aberto Sim) (Valor Padrão)
        EdicaoPermitida = 1,                // Permite editar quantidade de pallet em ambos (Pallet aberto ou fechado)
        EdicaoNaoPermitida = 2              // Não permite editar quantidade pallet
	}
}