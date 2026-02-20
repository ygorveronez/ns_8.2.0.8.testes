using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCancelamentoCTeSemCarga
    {
        AgCancelamentoCTe = 1,
        AgCancelamentoIntegracao = 2,
        Cancelado = 3,
        RejeicaoCancelamento = 4
    }

    public static class SituacaoCancelamentoCTeSemCargaHelper
    {
        public static string Descricao(this SituacaoCancelamentoCTeSemCarga situacaoCancelamentoCTeSemCarga)
        {
            switch (situacaoCancelamentoCTeSemCarga)
            {
                case SituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe: return "Cancelando os CT-es";
                case SituacaoCancelamentoCTeSemCarga.AgCancelamentoIntegracao: return "Realizando Integrações";
                case SituacaoCancelamentoCTeSemCarga.Cancelado: return "Cancelado";
                case SituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento: return "Rejeição no Cancelamento";
                default: return string.Empty;
            }
        }
    }
}
