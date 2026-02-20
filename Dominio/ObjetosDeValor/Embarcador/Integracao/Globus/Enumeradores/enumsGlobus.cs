using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public enum enumTipoWS
    {
        POST = 1,
        GET = 2,
        PATCH = 3,
        PUT = 4,
        DELETE = 5,
    }

    public enum enumEspecificaAPI
    {
        Contabilidade = 1,
        EscritaFiscal = 2,
        ContasPagar = 3,
        ContasReceber = 4
    }
}
