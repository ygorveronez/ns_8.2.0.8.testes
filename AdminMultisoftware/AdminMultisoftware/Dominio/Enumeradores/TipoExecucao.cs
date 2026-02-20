using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum TipoExecucao
    {
        Thread = 1,
        Hangfire = 2
    }

    public static class TipoExecucaoHelper
    {
        public static string ObterDescricao(this TipoExecucao tipoExecucao)
        {
            switch (tipoExecucao)
            {
                case TipoExecucao.Thread: return "Thread";
                case TipoExecucao.Hangfire: return "Hangfire";
                default: return string.Empty;
            }
        }
    }
}
