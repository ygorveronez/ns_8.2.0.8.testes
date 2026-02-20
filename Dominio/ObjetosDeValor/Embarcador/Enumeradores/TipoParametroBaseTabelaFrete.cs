namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParametroBaseTabelaFrete
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
        //ValorBase = 14,
        //ExcedenteEntrega = 15,
        Hora = 16,
        TipoEmbalagem = 17,
        Pacote = 18,
    }

    public static class TipoParametroBaseTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoParametroBaseTabelaFrete tipoParametroBaseTabelaFrete)
        {
            switch (tipoParametroBaseTabelaFrete)
            {
                case TipoParametroBaseTabelaFrete.TipoCarga:
                    return "Tipo de Carga";
                case TipoParametroBaseTabelaFrete.ModeloReboque:
                    return "Modelo do Reboque";
                case TipoParametroBaseTabelaFrete.ModeloTracao:
                    return "Modelo da Tração";
                case TipoParametroBaseTabelaFrete.ComponenteFrete:
                    return "Componente de Frete";
                case TipoParametroBaseTabelaFrete.NumeroEntrega:
                    return "Número de Entrega";
                case TipoParametroBaseTabelaFrete.Peso:
                    return "Quantidade/Peso";
                case TipoParametroBaseTabelaFrete.Distancia:
                    return "Distância";
                case TipoParametroBaseTabelaFrete.Rota:
                    return "Rota";
                case TipoParametroBaseTabelaFrete.ParametrosOcorrencia:
                    return "Ocorrência";
                case TipoParametroBaseTabelaFrete.Pallets:
                    return "Pallet";
                case TipoParametroBaseTabelaFrete.Tempo:
                    return "Tempo";
                case TipoParametroBaseTabelaFrete.Ajudante:
                    return "Ajudante";
                case TipoParametroBaseTabelaFrete.Hora:
                    return "Hora";
                case TipoParametroBaseTabelaFrete.TipoEmbalagem:
                    return "Tipo de Embalagem";
                case TipoParametroBaseTabelaFrete.Pacote:
                    return "Pacote";
                default: return "Nenhuma";
            }
        }
    }
}
