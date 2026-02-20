using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class SugestaoProgramacaoCarga
    {
        public int CodigoConfiguracaoProgramacaoCarga { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public List<int> CodigosCargas { get; set; }

        public List<int> CodigosDestinos { get; set; }

        public List<int> CodigosRegioesDestino { get; set; }

        public DateTime Data { get; set; }

        public decimal Quantidade { get; set; }

        public int QuantidadeValidada { get; set; }

        public List<string> SiglasEstadosDestino { get; set; }

        public SugestaoProgramacaoCarga Clonar()
        {
            return (SugestaoProgramacaoCarga)this.MemberwiseClone();
        }
    }
}
