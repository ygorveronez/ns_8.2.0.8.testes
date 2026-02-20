namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SubGrupoNC
    {
        NaoSelecionado = 0,
        DTOCCancelada = 1,
        EtapaDivergente = 2,
        NotaFiscalCancelada = 3,
        CNPJDivergente = 4,
        CodigoNodeDivergente = 5,
        TransportadoraDivergente = 6,
        PedidoNaoEncontrado = 7,
        CentroSAPDivergente = 8,
        DivergenciaCarregamento = 9,
        ChaveamentoPendente = 10,
        DeParaNaoLocalizado = 11,
        StatusInativo = 12,
        ItemNaoExpandido = 13,
        edidoSemSaldo = 14,
        PesoNaoLocalizado = 15,
        CapacidadeExcedida = 16
    }

    public static class SubGrupoNCHelper
    {
        public static string ObterDescricao(this SubGrupoNC status)
        {
            switch (status)
            {
                case SubGrupoNC.DTOCCancelada: return "DT/OC Cancelada";
                case SubGrupoNC.EtapaDivergente: return "Etapa Divergente";
                case SubGrupoNC.NotaFiscalCancelada: return "Nota Fiscal Cancelada";
                case SubGrupoNC.CNPJDivergente: return "CNPJ Divergente";
                case SubGrupoNC.CodigoNodeDivergente: return "Código Node Divergente";
                case SubGrupoNC.TransportadoraDivergente: return "Transportadora Divergente";
                case SubGrupoNC.PedidoNaoEncontrado: return "Pedido não Encontrado";
                case SubGrupoNC.CentroSAPDivergente: return "Centro SAP Divergente";
                case SubGrupoNC.DivergenciaCarregamento: return "Divergência Carregamento";
                case SubGrupoNC.ChaveamentoPendente: return "Chaveamento Pendente";
                case SubGrupoNC.DeParaNaoLocalizado: return "De/Para não Localizado";
                case SubGrupoNC.StatusInativo: return "Status Inativo";
                case SubGrupoNC.ItemNaoExpandido: return "Item não Expandido";
                case SubGrupoNC.edidoSemSaldo: return "Pedido sem Saldo";
                case SubGrupoNC.PesoNaoLocalizado: return "Peso não Localizado";
                case SubGrupoNC.CapacidadeExcedida: return "Capacidade Excedida";
                default: return string.Empty;
            }
        }
    }
}
