using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaJanelaCarregamentoTransportador
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoDestino { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoRota { get; set; }

        public int CodigoTransportador { get; set; }

        public long CodigoClienteTerceiro { get; set; }

        public List<int> CodigosCargasVinculadas { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public bool ExibirCargasFiliais { get; set; }

        public bool ExibirCargaSemValorFrete { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public string NumeroExp { get; set; }

        public SituacaoCargaJanelaCarregamentoTransportador? Situacao { get; set; }

        public TipoLiberacaoCargaJanelaCarregamento? TipoLiberacao { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public List<int> CodigosFiliais { get; set; }

        public bool NaoRetornarCargasMarcadoSemInteresse { get; set; }
    }
}
