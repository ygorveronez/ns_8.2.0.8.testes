using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { }, "Fretes/RegraAutorizacaoTabelaFrete")]
    public class RegraAutorizacaoTabelaFreteController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>
    {
		#region Construtores

		public RegraAutorizacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete regra)
        {
            List<Dominio.Entidades.Usuario> aprovadores = (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regra.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();

            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                regra.TipoAprovadorRegra,
                regra.EnviarLinkParaAprovacaoPorEmail,
                TabelaFrete = new { Codigo = regra.TabelaFrete?.Codigo ?? 0, Descricao = regra.TabelaFrete?.Descricao ?? "" },
                Aprovadores = (from aprovador in aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                UsarRegraPorAdValorem = regra.RegraPorAdValorem,
                UsarRegraPorDestino = regra.RegraPorDestino,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorOrigem = regra.RegraPorOrigem,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                UsarRegraPorValorFrete = regra.RegraPorValorFrete,
                UsarRegraPorValorPedagio = regra.RegraPorValorPedagio,
                AlcadasAdValorem = (from alcada in regra.AlcadasAdValorem select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaAdValorem, decimal>(alcada)).ToList(),
                AlcadasDestino = (from alcada in regra.AlcadasDestino select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaDestino, Dominio.Entidades.Localidade>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasOrigem = (from alcada in regra.AlcadasOrigem select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaOrigem, Dominio.Entidades.Localidade>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasValorFrete = (from alcada in regra.AlcadasValorFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorFrete, decimal>(alcada)).ToList(),
                AlcadasValorPedagio = (from alcada in regra.AlcadasValorPedagio select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorPedagio, decimal>(alcada)).ToList(),
            });
        }

        protected override void PreencherAprovadores(Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete regra, Repositorio.UnitOfWork unitOfWork)
        {
            regra.Aprovadores = new List<Dominio.Entidades.Usuario>();

            if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                regra.Aprovadores = ObterAprovadores(regra, unitOfWork);
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
                var repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                var repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoTabelaFrete = new Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete();

                PreencherRegra(regraAutorizacaoTabelaFrete, unitOfWork, ((regra) => {
                    int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");

                    regra.RegraPorAdValorem = Request.GetBoolParam("UsarRegraPorAdValorem");
                    regra.RegraPorDestino = Request.GetBoolParam("UsarRegraPorDestino");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorOrigem = Request.GetBoolParam("UsarRegraPorOrigem");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                    regra.RegraPorValorPedagio = Request.GetBoolParam("UsarRegraPorValorPedagio");
                    regra.TabelaFrete = codigoTabelaFrete > 0 ? repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete) : null;
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                }));

                repositorioRegra.Inserir(regraAutorizacaoTabelaFrete, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaAdValorem, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasAdValorem", ((valorPropriedade, alcada) =>
                {
                    decimal adValorem = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (adValorem > 0) ? adValorem : throw new ControllerException("O valor de Ad Valorem deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaDestino, Dominio.Entidades.Localidade>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasDestino", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Localidade destino = repositorioLocalidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = destino ?? throw new ControllerException("Destino não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaOrigem, Dominio.Entidades.Localidade>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasOrigem", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Localidade origem = repositorioLocalidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = origem ?? throw new ControllerException("Origem não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de Operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valorFrete = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (valorFrete > 0) ? valorFrete : throw new ControllerException("O valor de frete deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorPedagio, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, "AlcadasValorPedagio", ((valorPropriedade, alcada) =>
                {
                    decimal valorPedagio = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (valorPedagio > 0) ? valorPedagio : throw new ControllerException("O valor de pedágio deve ser maior do que zero.");
                }));

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public override IActionResult Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
                var repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                var repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoTabelaFrete = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoTabelaFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoTabelaFrete.Initialize();

                PreencherRegra(regraAutorizacaoTabelaFrete, unitOfWork, ((regra) => {
                    int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");

                    regra.RegraPorAdValorem = Request.GetBoolParam("UsarRegraPorAdValorem");
                    regra.RegraPorDestino = Request.GetBoolParam("UsarRegraPorDestino");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorOrigem = Request.GetBoolParam("UsarRegraPorOrigem");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                    regra.RegraPorValorPedagio = Request.GetBoolParam("UsarRegraPorValorPedagio");
                    regra.TabelaFrete = codigoTabelaFrete > 0 ? repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete) : null;
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaAdValorem, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasAdValorem, "AlcadasAdValorem", ((valorPropriedade, alcada) =>
                {
                    decimal adValorem = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (adValorem > 0) ? adValorem : throw new ControllerException("O valor de Ad Valorem deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaDestino, Dominio.Entidades.Localidade>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasDestino, "AlcadasDestino", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Localidade destino = repositorioLocalidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = destino ?? throw new ControllerException("Destino não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaOrigem, Dominio.Entidades.Localidade>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasOrigem, "AlcadasOrigem", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Localidade origem = repositorioLocalidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = origem ?? throw new ControllerException("Origem não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de Operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasValorFrete, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valorFrete = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (valorFrete > 0) ? valorFrete : throw new ControllerException("O valor de frete deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaValorPedagio, decimal>(unitOfWork, regraAutorizacaoTabelaFrete, regraAutorizacaoTabelaFrete.AlcadasValorPedagio, "AlcadasValorPedagio", ((valorPropriedade, alcada) =>
                {
                    decimal valorPedagio = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = (valorPedagio > 0) ? valorPedagio : throw new ControllerException("O valor de pedágio deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoTabelaFrete, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos Sobrescritos
    }
}