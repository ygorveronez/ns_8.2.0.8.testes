using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRotaCarga
    {
        Nenhuma,
        Distribuicao ,
        Transbordo,
        Praca,
        Retorno
    }

    public static class TipoRotaCargaHelper
    {
        public static string ObterDescricao (this TipoRotaCarga tipoRotaCarga)
        {
            return tipoRotaCarga switch
            {
                TipoRotaCarga.Distribuicao => "Distribuição",
                TipoRotaCarga.Transbordo => "Transbordo",
                TipoRotaCarga.Praca => "Praça",
                TipoRotaCarga.Retorno => "Retorno",
                TipoRotaCarga.Nenhuma => "Nenhuma",
                _ => throw new ArgumentOutOfRangeException(nameof(tipoRotaCarga), tipoRotaCarga, null)
            };
        }

        public static string ObterTipoRotaTag(this TipoRotaCarga tipoRotaCarga)
        {
            return tipoRotaCarga switch
            {
                TipoRotaCarga.Distribuicao => "D",
                TipoRotaCarga.Transbordo => "T",
                TipoRotaCarga.Praca => "P",
                TipoRotaCarga.Retorno => "R",
                _ => throw new ArgumentOutOfRangeException(nameof(tipoRotaCarga), tipoRotaCarga, null)
            };
        }
    }
}
