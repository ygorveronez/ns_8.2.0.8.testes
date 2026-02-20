using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CargaCancelamento
{
    public class EnvioCancelamentoCarga
    {
        public int ProtocoloCarga { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string MotivoCancelamento { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga Tipo { get; set; }
        public string Usuario { get; set; }
        public bool EnviouAverbacoesCTesParaCancelamento { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaNoCancelamento { get; set; }
        public bool CancelarDocumentosEmitidosNoEmbarcador { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga Situacao { get; set; }
        public bool DuplicarCarga { get; set; }
        public Embarcador.Financeiro.Justificativa Justificativa { get; set; }
        public JustificativaCancelamento JustificativaCancelamento { get; set; }
        public string OperadorResponsavel { get; set; }
        public bool LiberarPedidosParaMontagemCarga { get; set; }
        public List<EnvioCancelamentoCTe> CTes { get; set; }
        public List<EnvioCancelamentoAverbacaoCTe> AverbacaoCTes { get; set; }

    }
}
