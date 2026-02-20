using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao
    {
        public virtual int Codigo { get; set; }
        
        public virtual int DiaInicial { get; set; }
   
        public virtual int DiaFinal { get; set; }
        
        public virtual int IntervaloHora { get; set; }
    }
}
