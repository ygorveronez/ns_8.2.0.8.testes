namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParametroAjusteTabelaFrete
    {
        TipoCarga = 1,
        ModeloReboque = 2,
        ModeloTracao = 3,
        ComponenteFrete = 4,
        NumeroEntrega = 5,
        Peso = 6,
        Distancia = 7,
        Rota = 8,
        ParametrosOcorrencia = 9,
        Pallets = 10,
        Tempo = 11,
        Ajudante = 12,
        ValorFreteLiquido = 13,
        ValorMinimo = 14,
        ValorMaximo = 15,
        ValorBase = 16,
        PesoExcedente = 17,
        DistanciaExcedente = 18,
        NumeroEntregaExcedente = 19,
        PalletExcedente = 21,
        AjudanteExcedente = 22,
        TipoEmbalagem = 23
    }

    public static class TipoParametroAjusteTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoParametroAjusteTabelaFrete tipoParametroAjusteTabelaFrete)
        {
            switch (tipoParametroAjusteTabelaFrete)
            {
                case TipoParametroAjusteTabelaFrete.TipoCarga:
                    return "Tipo de Carga";
                case TipoParametroAjusteTabelaFrete.ModeloReboque:
                    return "Modelo do Reboque";
                case TipoParametroAjusteTabelaFrete.ModeloTracao:
                    return "Modelo da Tração";
                case TipoParametroAjusteTabelaFrete.ComponenteFrete:
                    return "Componente de Frete";
                case TipoParametroAjusteTabelaFrete.NumeroEntrega:
                    return "Número de Entrega";
                case TipoParametroAjusteTabelaFrete.TipoEmbalagem:
                    return "Tipo de Embalagem";
                case TipoParametroAjusteTabelaFrete.Peso:
                    return "Quantidade";
                case TipoParametroAjusteTabelaFrete.Distancia:
                    return "Distância";
                case TipoParametroAjusteTabelaFrete.Rota:
                    return "Rota";
                case TipoParametroAjusteTabelaFrete.ParametrosOcorrencia:
                    return "Ocorrência";
                case TipoParametroAjusteTabelaFrete.Pallets:
                    return "Pallet";
                case TipoParametroAjusteTabelaFrete.Tempo:
                    return "Tempo";
                case TipoParametroAjusteTabelaFrete.Ajudante:
                    return "Ajudante";
                case TipoParametroAjusteTabelaFrete.ValorMinimo:
                    return "Valor Mínimo";
                case TipoParametroAjusteTabelaFrete.ValorMaximo:
                    return "Valor Máximo";
                case TipoParametroAjusteTabelaFrete.ValorBase:
                    return "Valor Base";
                default:
                    return "Nenhuma";
            }
        }
    }
}
