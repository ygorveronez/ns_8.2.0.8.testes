using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class OutraAliquota
    {
        public int Codigo { get; set; }

        public string CST { get; set; }

        public string ClassificacaoTributaria { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public string CodigoIndicadorOperacao { get; set; }

        public List<OutraAliquotaImposto> Impostos { get; set; }

        public bool ZerarBase { get; set; }

        public bool Exportacao { get; set; }

        public bool SomarImpostosDocumento { get; set; }    
    }
}
