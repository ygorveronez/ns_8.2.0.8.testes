using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SessaoRoteirizadorParametros
    {
        public Enumeradores.TipoMontagemCarregamentoVRP TipoMontagemCarregamentoVRP { get; set; }

        #region " Apresentar somente nos VRP "
        
        public Enumeradores.TipoOcupacaoMontagemCarregamentoVRP TipoOcupacaoMontagemCarregamentoVRP { get; set; }

        public bool ConsiderarTempoDeslocamentoCD { get; set; }

        #endregion

        public bool GerarCarregamentoDoisDias { get; set; }

        public bool GerarCarregamentosAlemDaDispFrota { get; set; }       

        public bool UtilizarDispFrotaCentroDescCliente { get; set; }

        public int QuantidadeMaximaEntregasRoteirizar { get; set; }

        public bool MontagemCarregamentoPedidoProduto { get; set; }

        public int CarregamentoTempoMaximoRota { get; set; }

        public Enumeradores.NivelQuebraProdutoRoteirizar NivelQuebraProdutoRoteirizar { get; set; }

        public bool AgruparPedidosMesmoDestinatario { get; set; }

        public List<SessaoRoteirizadorParametrosDisponibilidadeFrota> DisponibilidadesFrota { get; set; }

        public List<SessaoRoteirizadorParametrosTempoCarregamento> TemposCarregamento { get; set; }

    }
}
