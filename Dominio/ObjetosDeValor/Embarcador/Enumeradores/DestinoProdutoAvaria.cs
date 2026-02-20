namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DestinoProdutoAvaria
    {
        Descartada = 1,
        Vendida = 2,
        DescontadaMotorista = 3,
        DevolvidaCliente = 4,
        DescontoFatura = 5,
    }

    public static class DestinoProdutoAvariaHelper
    {
        public static string ObterDescricao(this DestinoProdutoAvaria destino)
        {
            switch (destino)
            {
                case DestinoProdutoAvaria.Descartada: return "Descartada";
                case DestinoProdutoAvaria.Vendida: return "Vendida";
                case DestinoProdutoAvaria.DescontadaMotorista: return "Descontada do Motorista";
                case DestinoProdutoAvaria.DevolvidaCliente: return "Devolvida ao Cliente";
                case DestinoProdutoAvaria.DescontoFatura: return "Desconto em Fatura";
                default: return string.Empty;
            }
        }
    }
}
