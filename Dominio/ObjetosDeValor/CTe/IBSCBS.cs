using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.CTe
{
    public class IBSCBS
    {
        public virtual string NBS { get; set; }

        public virtual string CodigoIndicadorOperacao { get; set; }

        public virtual int CodigoOutrasAliquotas { get; set; }

        public virtual string CSTIBSCBS { get; set; }

        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        public virtual decimal BaseCalculoIBSCBS { get; set; }

        public virtual decimal AliquotaIBSEstadual { get; set; }

        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        public virtual decimal ValorIBSEstadual { get; set; }

        public virtual decimal AliquotaIBSMunicipal { get; set; }

        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        public virtual decimal ValorIBSMunicipal { get; set; }

        public virtual decimal AliquotaCBS { get; set; }

        public virtual decimal PercentualReducaoCBS { get; set; }

        public virtual decimal ValorCBS { get; set; }
    }
}
