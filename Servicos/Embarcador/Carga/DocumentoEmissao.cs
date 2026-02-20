using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class DocumentoEmissao
    {
        #region Atributos

        private readonly bool _permitirDeletarSemValidarSituacaoCarga;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public DocumentoEmissao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, permitirDeletarSemValidarSituacaoCarga: false) { }

        public DocumentoEmissao(Repositorio.UnitOfWork unitOfWork, bool permitirDeletarSemValidarSituacaoCarga)
        {
            _unitOfWork = unitOfWork;
            _permitirDeletarSemValidarSituacaoCarga = permitirDeletarSemValidarSituacaoCarga;
        }

        #endregion Construtores

        #region Métodos Privados

        private void Deletar(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal)
        {
            Repositorio.Embarcador.Canhotos.CanhotoAvulso repositorioCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente repositorioCargaPedidoXMLNotaFiscalTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repositorioCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repositorioCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repositorioPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repositorioPedidoXMLNotaFiscalContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidoXMLNotaFiscal.CargaPedido;
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento> cargaPreviaDocumentos = repositorioCargaPreviaDocumento.ObterPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento cargaPreviaDocumento in cargaPreviaDocumentos)
            {
                cargaPreviaDocumento.Documentos = null;
                repositorioCargaPreviaDocumento.Deletar(cargaPreviaDocumento);
            }

            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicoesFrete = repositorioCargaComposicaoFrete.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicaoFrete in cargaComposicoesFrete)
            {
                cargaComposicaoFrete.PedidoXMLNotasFiscais = null;
                repositorioCargaComposicaoFrete.Deletar(cargaComposicaoFrete);
            }

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentosProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalECarga(xmlNotaFiscal.Codigo, cargaPedido.CargaOrigem.Codigo);

            if (documentosProvisao != null)
            {
                if ((documentosProvisao.Provisao != null) && (documentosProvisao.Provisao.Situacao != SituacaoProvisao.Cancelado))
                    throw new ServicoException($"A nota fiscal está provisionada na provisão de número {documentosProvisao.Provisao.Numero}. Para  excluir a nota é necesário cancelar a provisão antes.");

                repositorioDocumentoProvisao.Deletar(documentosProvisao);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentes = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete componente in componentes)
                repositorioPedidoXMLNotaFiscalComponenteFrete.Deletar(componente);

            repositorioCargaPedidoXMLNotaFiscalTabelaFreteCliente.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarTodasPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
                repositorioCargaEntregaNotaFiscal.Deletar(cargaEntregaNotaFiscal);

            repositorioPedidoXMLNotaFiscalContaContabilContabilizacao.DeletarPorPedidoNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> canhotosAvulsos = repositorioCanhotoAvulso.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso in canhotosAvulsos)
            {
                canhotoAvulso.PedidosXMLNotasFiscais.Remove(pedidoXMLNotaFiscal);
                repositorioCanhotoAvulso.Atualizar(canhotoAvulso);
            }

            repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
            repositorioPedidoXMLNotaFiscal.Deletar(pedidoXMLNotaFiscal);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotasFiscaisParciais = repositorioCargaPedidoXMLNotaFiscalParcial.ConsultarPorNFe(xmlNotaFiscal.Codigo, cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial in cargaPedidoXMLNotasFiscaisParciais)
                repositorioCargaPedidoXMLNotaFiscalParcial.Deletar(cargaPedidoXMLNotaFiscalParcial);

            if (repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNFesParciaisPelaChave = repositorioCargaPedidoXMLNotaFiscalParcial.ConsultarPorChave(cargaPedido?.Codigo ?? 0, xmlNotaFiscal?.Chave ?? string.Empty);

                if ((cargaPedidoXMLNFesParciaisPelaChave != null) && !cargaPedidoXMLNotasFiscaisParciais.Contains(cargaPedidoXMLNFesParciaisPelaChave))
                    repositorioCargaPedidoXMLNotaFiscalParcial.Deletar(cargaPedidoXMLNFesParciaisPelaChave);
            }

            if (!repositorioPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
            {
                cargaPedido.SituacaoEmissao = SituacaoNF.Nova;
                repositorioCargaPedido.Atualizar(cargaPedido);
            }
        }

        private void DeletarPorNotasFiscais(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<int> codigosNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido };
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            if ((carga.SituacaoCarga != SituacaoCarga.AgNFe) && !_permitirDeletarSemValidarSituacaoCarga)
                throw new ServicoException("Não é possível excluir as notas fiscais na situação atual da carga.");

            if (carga.ProcessandoDocumentosFiscais && (carga.SituacaoCarga == SituacaoCarga.AgNFe))
                throw new ServicoException("A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

            if (carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.Agrupadora)
                throw new ServicoException("Não é possível excluir as notas fiscais na carga agrupadora de trechos.");

            if (carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga)
            {
                Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaDT = repositorioStageAgrupamento.BuscarPrimeiraCargaDTPorCargaGerada(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDT = repositorioCargaPedido.BuscarPorCargaEPedido(cargaDT.Codigo, cargaPedido.Pedido.Codigo);

                cargaPedidos.Add(cargaPedidoDT);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoProximoTrecho = cargaPedido.CargaPedidoProximoTrecho;

                while (cargaPedidoProximoTrecho != null)
                {
                    cargaPedidos.Add(cargaPedidoProximoTrecho);
                    cargaPedidoProximoTrecho = cargaPedidoProximoTrecho.CargaPedidoProximoTrecho;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrechoAnterior = cargaPedido.CargaPedidoTrechoAnterior;

                while (cargaPedidoTrechoAnterior != null)
                {
                    cargaPedidos.Add(cargaPedidoTrechoAnterior);
                    cargaPedidoTrechoAnterior = cargaPedidoTrechoAnterior.CargaPedidoTrechoAnterior;
                }
            }

            List<int> codigoCargaPedidos = cargaPedidos.Select(o => o.Codigo).ToList();

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidosENotasFiscais(codigoCargaPedidos, codigosNotasFiscais);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = pedidoXMLNotasFiscais.Select(notaPedido => notaPedido.XMLNotaFiscal).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                Deletar(pedidoXMLNotaFiscal);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlNotasFiscais)
            {
                xmlNotaFiscal.SemCarga = true;
                repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                servicoCargaPedido.AlterarDadosSumarizadosCargaPedido(cargaPedido, xmlNotaFiscal.Volumes, 0);
                repositorioCargaPedido.Atualizar(cargaPedido);

                if (!repositorioPedidoXMLNotaFiscal.VerificarSeExisteEmOutroPedido(xmlNotaFiscal.Codigo, cargaPedido.Codigo))
                    servicoCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, _unitOfWork);

                new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(_unitOfWork, auditado).CancelarMovimentacaoPallet(xmlNotaFiscal, cargaPedido);

            }

            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            servicoHubCarga.InformarCargaAtualizada(carga.Codigo, TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void DeletarPorNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int codigoNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (cargaPedido == null)
                throw new ServicoException("Pedido não encontrado.");

            DeletarPorNotasFiscais(cargaPedido, new List<int>() { codigoNotaFiscal }, auditado);
        }

        public void DeletarTodos(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (cargaPedido == null)
                throw new ServicoException("Pedido não encontrado.");

            if (cargaPedido.NotasFiscais.Count == 0)
                throw new ServicoException("Nenhuma nota encontrada para o pedido.");


            var lstNotasFiscais = cargaPedido.NotasFiscais.Select(o => o.XMLNotaFiscal.Codigo).ToList();
            if (lstNotasFiscais.Count < 2000)
                DeletarPorNotasFiscais(cargaPedido, lstNotasFiscais, auditado);
            else
            {
                decimal decimalBlocos = Math.Ceiling(((decimal)lstNotasFiscais.Count) / 1000);
                int blocos = (int)Math.Truncate(decimalBlocos);

                for (int i = 0; i < blocos; i++)
                {
                    DeletarPorNotasFiscais(cargaPedido, lstNotasFiscais.Skip(i * 1000).Take(1000).ToList(), auditado);
                }
            }
        }

        public void DeletarNotasFiscaisViculadasPorRotaFacility(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<int> listaNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (cargaPedido == null)
                throw new ServicoException("Pedido não encontrado.");

            if (listaNotaFiscal.Count == 0)
                throw new ServicoException("Nenhuma nota encontrada para o pedido.");

            DeletarPorNotasFiscais(cargaPedido, listaNotaFiscal, auditado);

        }

        #endregion Métodos Públicos
    }
}
