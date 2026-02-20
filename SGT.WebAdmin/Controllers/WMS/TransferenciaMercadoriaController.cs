using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/TransferenciaMercadoria")]
    public class TransferenciaMercadoriaController : BaseController
    {
		#region Construtores

		public TransferenciaMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPosicaoAtuals()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

                int codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador, codigoProdutoEmbarcadorLote, inicio, limite;

                int.TryParse(Request.Params("Inicio"), out inicio);
                int.TryParse(Request.Params("Limite"), out limite);

                int.TryParse(Request.Params("DepositoPosicao"), out codigoDepositoPosicao);
                int.TryParse(Request.Params("DepositoBloco"), out codigoDepositoBloco);
                int.TryParse(Request.Params("DepositoRua"), out codigoDepositoRua);
                int.TryParse(Request.Params("Deposito"), out codigoDeposito);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProdutoEmbarcador);
                int.TryParse(Request.Params("ProdutoEmbarcadorLote"), out codigoProdutoEmbarcadorLote);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> produtos = repProdutoEmbarcadorLote.Consultar(codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador, codigoProdutoEmbarcadorLote, "Codigo", "desc", inicio, limite);
                int quantidade = repProdutoEmbarcadorLote.ContarConsulta(codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador, codigoProdutoEmbarcadorLote);

                var retorno = new
                {
                    Quantidade = quantidade,
                    Registros = (from obj in produtos select ObterDetalhesLoteProduto(obj)).ToList()
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPosicaoTransferir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                int codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador, inicio, limite;

                int.TryParse(Request.Params("Inicio"), out inicio);
                int.TryParse(Request.Params("Limite"), out limite);

                int.TryParse(Request.Params("DepositoPosicao"), out codigoDepositoPosicao);
                int.TryParse(Request.Params("DepositoBloco"), out codigoDepositoBloco);
                int.TryParse(Request.Params("DepositoRua"), out codigoDepositoRua);
                int.TryParse(Request.Params("Deposito"), out codigoDeposito);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProdutoEmbarcador);

                List<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> posicoes = repDepositoPosicao.Consultar(true, codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador, "Codigo", "desc", inicio, limite);
                int quantidade = repDepositoPosicao.ContarConsulta(true, codigoDepositoPosicao, codigoDepositoBloco, codigoDepositoRua, codigoDeposito, codigoProdutoEmbarcador);

                var retorno = new
                {
                    Quantidade = quantidade,
                    Registros = (from obj in posicoes select ObterDetalhesPosicao(obj)).ToList()
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        public async Task<IActionResult> TransferirLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoLote = 0, codigoDestino = 0;
                int.TryParse(Request.Params("CodigoLote"), out codigoLote);
                int.TryParse(Request.Params("CodigoDestino"), out codigoDestino);

                if (codigoLote == 0 || codigoDestino == 0)
                    return new JsonpResult(false, "Não foi possível identificar o lote e o destino.");

                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
                Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
                Repositorio.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote repTransferenciaProdutoEmbarcadorLote = new Repositorio.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote = repProdutoEmbarcadorLote.BuscarPorCodigo(codigoLote);
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = repDepositoPosicao.BuscarPorCodigo(codigoDestino);

                if (lote.PesoAtual > posicao.PesoMaximo && posicao.PesoMaximo > 0)
                    return new JsonpResult(false, "O local de armazenamento selecionado excede o peso máximo.");
                if (lote.MetroCubicoAtual > posicao.MetroCubicoMaximo && posicao.MetroCubicoMaximo > 0)
                    return new JsonpResult(false, "O local de armazenamento selecionado excede o M³ máximo.");
                if (lote.QuantidadePaletAtual > posicao.QuantidadePaletes && posicao.QuantidadePaletes > 0)
                    return new JsonpResult(false, "O local de armazenamento selecionado excede o palets máximo.");
                if (lote.DepositoPosicao.Codigo == posicao.Codigo)
                    return new JsonpResult(false, "O local de armazenamento selecionado é o mesmo do anterior.");

                lote.DepositoPosicao = posicao;
                repProdutoEmbarcadorLote.Atualizar(lote);

                Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote transferencia = new Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote
                {
                    Data = DateTime.Now,
                    DepositoPosicaoDestino = posicao,
                    DepositoPosicaoOrigem = lote.DepositoPosicao,
                    Quantidade = lote.QuantidadeAtual,
                    Usuario = this.Usuario
                };

                repTransferenciaProdutoEmbarcadorLote.Inserir(transferencia);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public dynamic ObterDetalhesLoteProduto(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote)
        {
            Servicos.Embarcador.WMS.LoteProdutoEmbarcador serWMS = new Servicos.Embarcador.WMS.LoteProdutoEmbarcador();
            return serWMS.ObterDetalhesLoteProduto(lote);
        }

        public dynamic ObterDetalhesPosicao(Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao)
        {
            Servicos.Embarcador.WMS.LoteProdutoEmbarcador serWMS = new Servicos.Embarcador.WMS.LoteProdutoEmbarcador();
            return serWMS.ObterDetalhesPosicao(posicao);
        }
    }
}
