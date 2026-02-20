using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;
using static Google.Apis.Requests.BatchRequest;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        public void IntegrarPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);

            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repPedidoIntegracao.Atualizar(pedidoIntegracao);
                return;
            }

            string jsonEnvio = string.Empty;
            string jsonRetorno = string.Empty;
            string mensagemErro = string.Empty;
            int idColetaEntrega = 0;
            int idClienteOrigem = 0;
            int idClienteOrigemEndereco = 0;
            int idClienteDestino = 0;
            int idClienteDestinoEndereco = 0;


            if (ObterToken(out mensagemErro) &&
                IntegrarCliente(pedidoIntegracao.Pedido.Remetente, null, out idClienteOrigem, out idClienteOrigemEndereco, out mensagemErro) &&
                IntegrarCliente(pedidoIntegracao.Pedido.Destinatario, null, out idClienteDestino, out idClienteDestinoEndereco, out mensagemErro) &&
                IntegrarColetaEntrega(pedidoIntegracao, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco, out jsonEnvio, out jsonRetorno, out idColetaEntrega, out mensagemErro))
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                pedidoIntegracao.ProblemaIntegracao = "Registro integrado com sucesso.";
                pedidoIntegracao.CodigoIntegracaoIntegradora = idColetaEntrega.ToString();
            }
            else
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }

            repPedidoIntegracao.Atualizar(pedidoIntegracao);

            SalvarArquivosIntegracao(pedidoIntegracao, jsonEnvio, jsonRetorno);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacaoPedido(jsonRequisicao, jsonRetorno, pedidoIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (pedidoIntegracao.ArquivosTransacao == null)
                pedidoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo>();

            pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo AdicionarArquivoTransacaoPedido(string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        #endregion Métodos Privados
    }
}