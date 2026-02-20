using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public enum TipoServico
    {
        Normal,
        Redespacho,
        Complemento,
        Reentrega,
        Diaria,
        Descarga,
        Pallet,
        NFSE
    }

    public static class TipoServicoHelper
    {
        public static string ObterDescricao(TipoServico tipo)
        {
            return tipo switch
            {
                TipoServico.Normal => "A",
                TipoServico.Redespacho => "E",
                TipoServico.Complemento => "C",
                TipoServico.Reentrega => "R",
                TipoServico.Diaria => "I",
                TipoServico.Descarga => "G",
                TipoServico.Pallet => "P",
                TipoServico.NFSE => "N",
                _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null)
            };
        }

        public static string ObterDescricaoPorCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoCarga? tipo)
        {
            if (tipo == null)
                return "A";

            return tipo switch
            {
                TipoServicoCarga.Normal => "A",
                TipoServicoCarga.Redespacho => "E",
                _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null)
            };
        }
    }
}
