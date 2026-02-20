using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento
{
    public sealed class CancelamentoCarga
    {
        public int Codigo { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public string Carga { get; set; }

        public string Remetente { get; set; }

        public string Destinatario { get; set; }

        public SituacaoCancelamentoCarga Situacao { get; set; }

        public string Usuario { get; set; }

        public string MensagemRejeicaoCancelamento { get; set; }

        public string PortoOrigem { get; set; }

        public string PortoDestino { get; set; }

        public string Viagem { get; set; }

        public string Origens { get; set; }

        public string Destinos { get; set; }

        public string MotivoCancelamento { get; set; }

        public TipoCancelamentoCargaDocumento TipoCancelamentoCargaDocumento { get; set; }

        public string SituacaoFormatada
        {
            get { return Situacao.Descricao(); }
        }

        public string TipoCancelamentoCargaDocumentoFormatada
        {
            get { return TipoCancelamentoCargaDocumento.ObterDescricao(); }
        }
    }
}
