using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class FiltroPesquisaRelatorioCargaEntregaChecklist
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoCheckListTipo { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public DateTime? DataCarregamentoInicial { get; set; }

        public DateTime? DataCarregamentoFinal { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public List<int> CodigosMotoristas { get; set; }

        public List<int> Filiais { get; set; }

        public List<double> Recebedores { get; set; }

        public int CodigoRemetentePecuarista { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public DateTime? DataCargaInicial { get; set; }

        public DateTime? DataCargaFinal { get; set; }
    }
}