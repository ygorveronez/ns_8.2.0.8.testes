using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDesconto")]
    public class ContratoFreteAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public ContratoFreteAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Contrato", "NumeroContrato", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Número CIOT", "NumeroCIOT", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Subcontratado", "NomeSubcontratado", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Motorista", "NomeMotorista", 15, Models.Grid.Align.right, true);
                if (filtrosPesquisa.Situacao == SituacaoContratoFreteAcrescimoDesconto.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true, false);

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> listaContratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repContratoFreteAcrescimoDesconto.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in listaContratoFreteAcrescimoDesconto
                                   select new
                                   {
                                       obj.Codigo,
                                       NumeroContrato = obj?.ContratoFrete?.NumeroContrato ?? 0,
                                       Justificativa = obj.Justificativa.Descricao,
                                       Valor = obj.Valor.ToString("n2"),
                                       Situacao = obj.Situacao.ObterDescricao(),
                                       NumeroCarga = obj?.ContratoFrete?.Carga.CodigoCargaEmbarcador ?? string.Empty,
                                       NomeMotorista = obj?.ContratoFrete?.Carga.NomeMotoristas ?? string.Empty,
                                       NumeroCIOT = obj.ContratoFrete == null ? string.Empty : string.Join(", ", obj.ContratoFrete.Carga.CargaCIOTs.Select(o => o.CIOT.Numero)),
                                       NomeSubcontratado = obj?.ContratoFrete?.TransportadorTerceiro?.Nome ?? string.Empty,
                                       Descricao = obj.Justificativa.Descricao
                                   }).ToList());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                int codigoContratoFrete = Request.GetIntParam("ContratoFrete");
                int codigoJustificativa = Request.GetIntParam("Justificativa");
                decimal valor = Request.GetDecimalParam("Valor");
                bool disponibilizarFechamentoDeAgregado = Request.GetBoolParam("DisponibilizarFechamentoDeAgregado");

                string mensagemErro = string.Empty;
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto();
                if (!serContratoFreteAcrescimoDesconto.ValidarAdicionarContratoFreteAcrescimoDesconto(ref contrato, ref mensagemErro, codigoContratoFrete, codigoJustificativa, valor, disponibilizarFechamentoDeAgregado, unitOfWork))
                    throw new ControllerException(mensagemErro);

                unitOfWork.Start();

                contrato.Data = DateTime.Now;
                contrato.Usuario = Usuario;
                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.SemRegra;
                contrato.Valor = valor;
                contrato.Observacao = Request.GetStringParam("Observacao");
                contrato.DisponibilizarFechamentoDeAgregado = disponibilizarFechamentoDeAgregado;

                repContratoFreteAcrescimoDesconto.Inserir(contrato, Auditado);

                serContratoFreteAcrescimoDesconto.EtapaAprovacao(contrato, TipoServicoMultisoftware);
                repContratoFreteAcrescimoDesconto.Atualizar(contrato);

                unitOfWork.CommitChanges();

                return new JsonpResult(serContratoFreteAcrescimoDesconto.ObterDetalhesContratoFreteAcrescimoDesconto(contrato, unitOfWork));
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato não encontrado.");

                return new JsonpResult(serContratoFreteAcrescimoDesconto.ObterDetalhesContratoFreteAcrescimoDesconto(contrato, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato não encontrado.");

                if (contrato.Situacao.IsPermiteCancelarContratoFreteAcrescimoDesconto())
                    return new JsonpResult(false, true, "Não é possível cancelar na situação atual.");

                List<SituacaoCIOT> listaSituacaoCIOT = new List<SituacaoCIOT>() { SituacaoCIOT.Encerrado, SituacaoCIOT.PagamentoAutorizado, SituacaoCIOT.Cancelado };
                if (contrato.DisponibilizarFechamentoDeAgregado && contrato.CIOT != null && listaSituacaoCIOT.Contains(contrato.CIOT.Situacao))
                    return new JsonpResult(false, true, "Acréscimo/Desconto vinculado ao CIOT Agregado e situação do CIOT não permite cancelamento.");

                unitOfWork.Start();

                serContratoFreteAcrescimoDesconto.ReverterValorNoContratoFrete(contrato, TipoServicoMultisoftware, Auditado, false);
                serContratoFreteAcrescimoDesconto.GerarIntegracaoCancelamentoTotvs(contrato, TipoServicoMultisoftware, Auditado);

                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.Cancelado;
                repContratoFreteAcrescimoDesconto.Atualizar(contrato);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Cancelado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFreteAcrescimoDesconto");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento))
                    return new JsonpResult(false, true, "Você não possui permissões para liberar pagamento.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato não encontrado.");

                if (contrato.DisponibilizarFechamentoDeAgregado)
                    return new JsonpResult(false, true, "Registro configurado para ser utilizado no fechamento de agregado.");

                if (contrato.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                    return new JsonpResult(false, true, "Permitido somente para acréscimo");

                if (contrato.PagamentoAutorizado)
                    return new JsonpResult(false, true, "Já foi liberado o pagamento para esse contrato.");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = repContratoFreteAcrescimoDescontoIntegracao.BuscarPrimeiroPorContratoFreteAcrescimoDesconto(codigo);

                if (contratoIntegracao == null)
                    return new JsonpResult(false, true, "Contrato não possui integração.");

                if (contratoIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    return new JsonpResult(false, true, "Integração do contrato não está integrada.");

                Servicos.Embarcador.CIOT.CIOT serCIOT = new Servicos.Embarcador.CIOT.CIOT();

                if (!serCIOT.IntegrarAutorizacaoPagamentoAcrescimoDesconto(out string mensagem, contratoIntegracao.CIOT, contrato, unitOfWork))
                    return new JsonpResult(false, true, mensagem);

                unitOfWork.Start();

                contrato.PagamentoAutorizado = true;
                repContratoFreteAcrescimoDesconto.Atualizar(contrato);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Liberado Pagamento", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarContratoFreteTacAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContratoFrete = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto servContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                bool tacAgregado = servContratoFreteAcrescimoDesconto.VerificarContratoFreteTacAgregado(codigoContratoFrete, unitOfWork);

                return new JsonpResult(tacAgregado);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDesconto()
            {
                ContratoFrete = Request.GetIntParam("ContratoFrete"),
                Justificativa = Request.GetIntParam("Justificativa"),
                Situacao = Request.GetEnumParam<SituacaoContratoFreteAcrescimoDesconto>("Situacao"),
                CodigosCarga = Request.GetListParam<int>("NumeroCarga"),
                CodigosCIOT = Request.GetListParam<int>("NumeroCIOT"),
                NomeSubcontratado = Request.GetListParam<double>("NomeSubcontratado"),
                NomeMotorista = Request.GetListParam<string>("NomeMotorista"),
                TipoJustificativa = Request.GetEnumParam<TipoJustificativa>("TipoJustificativa"),
                CodigoTransportadorContratoFreteOrigem = Request.GetDoubleParam("CodigoTransportadorContratoFreteOrigem")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("NumeroContrato"))
                propriedadeOrdenar = "ContratoFrete.NumeroContrato";
            else if (propriedadeOrdenar.Equals("Justificativa"))
                propriedadeOrdenar = "Justificativa.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
