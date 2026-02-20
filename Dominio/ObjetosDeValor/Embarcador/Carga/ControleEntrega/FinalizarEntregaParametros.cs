using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class FinalizarEntregaParametros
    {
        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega;
        public DateTime dataInicioEntrega;
        public DateTime dataTerminoEntrega;
        public DateTime? dataConfirmacao;

        // Confirmação de chegada
        public DateTime? dataConfirmacaoChegada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada;

        public DateTime? dataSaidaRaio;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDescarga;
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos;
        public int motivoRetificacao;
        public int motivoFalhaNotaFiscal;
        public string justificativaEntregaForaRaio;
        public int motivoFalhaGTA;
        public int OrdemRealizada;
        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador;
        public Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega;
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
        public Dominio.ObjetosDeValor.Enumerador.OrigemAuditado sistemaOrigem;
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor dadosRecebedor;
        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware;
        public List<string> handlingUnitIds;
        public List<string> chavesNFe;

        // Motorista que finalizou
        public Dominio.Entidades.Usuario motorista;

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega;
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega? OrigemSituacaoFimViagem;
        public double ClienteAreaRedex;
        public int Container;
        public DateTime? DataColetaContainer;
        public int avaliacaoColetaEntrega;
        //public bool GerarOcorrenciaPedido;
        public string observacao;

        public bool ExecutarValidacoes = true;

        public bool FinalizandoViagem;
        public bool FinalizandoEntregaTransbordo;
        public bool TornarFinalizacaoDeEntregasAssincrona = false;
        public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado;

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega;
        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega;
        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro;
        public Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer;
        public DateTime? dataConfirmacaoEntrega;
    }

    public class FinalizarEntregaAssincronaParametros
    {
        public int cargaEntrega;
        public int configuracaoEmbarcador;
        public int motorista;
        public int clienteMultisoftware;
        public int configuracaoControleEntrega;
        public int configuracaoTipoOperacaoControleEntrega;
        public int tipoOperacaoParametro;
        public int retiradaContainer;

        public DateTime dataInicioEntrega;
        public DateTime dataTerminoEntrega;
        public DateTime? dataConfirmacao;
        public DateTime? dataConfirmacaoChegada;
        public DateTime? dataSaidaRaio;
        public DateTime? DataColetaContainer;
        public DateTime? dataConfirmacaoEntrega;

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;

        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDescarga;
        public Dominio.ObjetosDeValor.Enumerador.OrigemAuditado sistemaOrigem;
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor dadosRecebedor;
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega;
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega? OrigemSituacaoFimViagem;
        public Dominio.ObjetosDeValor.Embarcador.Auditoria.AuditadoAssincrono auditado;

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos;
        public List<string> handlingUnitIds;
        public List<string> chavesNFe;

        public bool TornarFinalizacaoDeEntregasAssincrona;
        public bool ExecutarValidacoes = true;
        public double ClienteAreaRedex;
        public int motivoRetificacao;
        public int motivoFalhaNotaFiscal;
        public int motivoFalhaGTA;
        public int OrdemRealizada;
        public int Container;
        public int avaliacaoColetaEntrega;
        public string justificativaEntregaForaRaio;
        public string observacao;
        public bool finalizandoViagem;

    }
}
