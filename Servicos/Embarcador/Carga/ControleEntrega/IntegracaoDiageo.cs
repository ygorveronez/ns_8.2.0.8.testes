using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class IntegracaoDiageo
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private readonly string _urlAcessoCliente;

        #endregion

        #region Construtores

        public IntegracaoDiageo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegracaoDiageo(Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
        {
            _unitOfWork = unitOfWork;
            _urlAcessoCliente = urlAcessoCliente;
        }

        #endregion

        #region Métodos Públicos

        public void GerarIntegracoes()
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repOcorrencias = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork).BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Diageo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ocorrencias = repOcorrencias.BuscarOcorrenciasPorUltimoRegistroIntegradoPorTipoIntegracao(tipoIntegracao.Codigo);

            foreach(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia in ocorrencias)
            {
                GerarIntegracao(ocorrencia, tipoIntegracao, _unitOfWork);
            }
        }



        #endregion

        #region Métodos Privados
        private static void GerarIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao();
            pedidoOcorrenciaColetaEntregaIntegracao.Initialize();
            pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega = pedidoOcorrenciaColetaEntrega;
            pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            pedidoOcorrenciaColetaEntregaIntegracao.DataIntegracao = DateTime.Now;
            pedidoOcorrenciaColetaEntregaIntegracao.NumeroTentativas = 0;
            pedidoOcorrenciaColetaEntregaIntegracao.ProblemaIntegracao = "";
            pedidoOcorrenciaColetaEntregaIntegracao.TipoIntegracao = tipoIntegracao;
            repPedidoOcorrenciaColetaEntregaIntegracao.Inserir(pedidoOcorrenciaColetaEntregaIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoOcorrenciaColetaEntregaIntegracao, pedidoOcorrenciaColetaEntregaIntegracao.GetChanges(), "Registro de integração gerado automaticamente por thread.", unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(pedidoOcorrenciaColetaEntrega, unitOfWork);
        }
        #endregion

    }
}
