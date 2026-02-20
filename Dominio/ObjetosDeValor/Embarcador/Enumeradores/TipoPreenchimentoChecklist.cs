using System;
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPreenchimentoChecklist
    {
        PreenchimentoObrigatorio = 0,
        PreenchimentoDesabilitado = 2,
    }

    public static class TipoPreenchimentoChecklistHelper
    {
        public static string ObterDescricao(this TipoPreenchimentoChecklist status)
        {
            switch (status)
            {
                case TipoPreenchimentoChecklist.PreenchimentoObrigatorio: return "Preenchimento do Checklist Obrigatório";
                case TipoPreenchimentoChecklist.PreenchimentoDesabilitado: return "Desabilitar a Etapa de Checklist";
                default: return string.Empty;
            }
        }
    }
}
