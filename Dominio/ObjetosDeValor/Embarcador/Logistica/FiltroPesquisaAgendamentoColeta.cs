using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAgendamentoColeta
    {
        public DateTime? DataColeta { get; set; }

        public DateTime? DataEntrega { get; set; }

        public DateTime? DataCriacao { get; set; }

        public DateTime? DataAgendamento { get; set; }

        public int TipoCarga { get; set; }

        public List<int> TipoCargas { get; set; }

        public List<int> TipoOperacoes { get; set; }

        public List<int> ModelosVeiculares { get; set; }

        public List<int> Transportadores { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public double Destinatario { get; set; }

        public double Recebedor { get; set; }

        public Enumeradores.SituacaoCargaJanelaCarregamento? Situacao { get; set; }

        public Enumeradores.SituacaoCarga? Etapa { get; set; }

        public string Carga { get; set; }

        public double CodigoRemetente { get; set; }

        public string Senha { get; set; }

        public string Pedido { get; set; }

        public Enumeradores.SituacaoCargaJanelaDescarregamento? SituacaoJanelaDescarregamento { get; set; }

        public string PedidoEmbarcador { get; set; }

        public int CodigoEmpresaLogada { get; set; }

        public bool OcultarDescargaCancelada { get; set; }
    }
}
