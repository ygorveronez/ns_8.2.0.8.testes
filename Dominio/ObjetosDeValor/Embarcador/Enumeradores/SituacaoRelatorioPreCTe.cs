using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRelatorioPreCTe
    {
        Pendente = 1,
        CTeRecebido = 2
    }

    public static class SituacaoRelatorioPreCTeHelper
    {
        public static string ObterDescricao(this SituacaoRelatorioPreCTe situacao)
        {
            switch (situacao)
            {
                case SituacaoRelatorioPreCTe.CTeRecebido: return "CT-e Recebido";
                case SituacaoRelatorioPreCTe.Pendente: return "Pendente";
                default: return string.Empty;
            }
        }
    }
}