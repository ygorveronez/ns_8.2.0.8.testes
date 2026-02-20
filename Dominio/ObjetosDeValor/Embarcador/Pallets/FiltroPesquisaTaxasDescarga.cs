using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaTaxasDescarga
    {
        public int CodigoFilial { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public int CodigoTipoCarga { get; set; }
        public bool? Status { get; set; }
        public DateTime DataInicioVigencia { get; set; }
        public DateTime DataFimVigencia { get; set; }
        public double CpfCnpjCliente { get; set; }
        public int CodigoGrupoCliente { get; set; }
        public List<int> CodigoTipoOperacao { get; set; }
        public bool SomenteVigentes { get; set; }
        public SituacaoAjusteConfiguracaoDescargaCliente? CodigoSituacao { get; set; }

        /// <summary>
        /// Usado para filtrar somente registros pertinentes a Empresa do usuário logado no Portal do Transportador
        /// </summary>
        public int CodigoTransportadorPortal { get; set; } 
    }


}
