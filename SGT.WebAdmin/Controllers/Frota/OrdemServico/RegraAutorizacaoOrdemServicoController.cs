using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize(new string[] { }, "Frota/RegraAutorizacaoOrdemServico")]
    public class RegraAutorizacaoOrdemServicoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico>
    {
		#region Construtores

		public RegraAutorizacaoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                UsarRegraPorFornecedor = regra.RegraPorFornecedor,
                UsarRegraPorOperador = regra.RegraPorOperador,
                UsarRegraPorValor = regra.RegraPorValor,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFornecedor = (from alcada in regra.AlcadasFornecedor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaFornecedor, Dominio.Entidades.Cliente>(alcada)).ToList(),
                AlcadasOperador = (from alcada in regra.AlcadasOperador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaOperador, Dominio.Entidades.Usuario>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaValor, decimal>(alcada)).ToList()
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repositorioTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);   

                Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico regraAutorizacaoOrdemServico = new Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico();

                PreencherRegra(regraAutorizacaoOrdemServico, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFornecedor = Request.GetBoolParam("UsarRegraPorFornecedor");
                    regra.RegraPorOperador = Request.GetBoolParam("UsarRegraPorOperador");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
                    regra.RegraPorTipoOrdemServico = Request.GetBoolParam("UsarRegraPorTipoServico");
                }));

                repositorioRegra.Inserir(regraAutorizacaoOrdemServico, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaFornecedor, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasFornecedor", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Fornecedor não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaOperador, Dominio.Entidades.Usuario>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasOperador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Usuario operador = repositorioUsuario.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = operador ?? throw new ControllerException("Operador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaTipoOrdemServico, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasTipoOrdemServico", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = repositorioTipoOrdemServico.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOrdemServico ?? throw new ControllerException("Tipo de Ordem de Serviço não encontrada.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Produto repositorioProduto = new Repositorio.Produto(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico regraAutorizacaoOrdemServico = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoOrdemServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoOrdemServico, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFornecedor = Request.GetBoolParam("UsarRegraPorFornecedor");
                    regra.RegraPorOperador = Request.GetBoolParam("UsarRegraPorOperador");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaFornecedor, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasFornecedor, "AlcadasFornecedor", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Fornecedor não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaOperador, Dominio.Entidades.Usuario>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasOperador, "AlcadasOperador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Usuario operador = repositorioUsuario.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = operador ?? throw new ControllerException("Operador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoOrdemServico, Auditado);

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

        #endregion
    }
}