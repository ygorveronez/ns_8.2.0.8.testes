using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Faturamento
{
    [CustomAuthorize(new string[] { "PesquisaFaturamento" }, "Cargas/Carga")]
    public class CargaFaturamentoController : BaseController
    {
		#region Construtores

		public CargaFaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisaFaturamentoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFatura", false);
                grid.AdicionarCabecalho("CodigoTitulo", false);
                grid.AdicionarCabecalho("N° Fatura", "NumeroFatura", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Boleto(s)", "NumeroBoletos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Título", "NumeroTitulos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pessoa", "Tomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Vencimento", "Vencimento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Emissão", "Emissao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de CT-e", "TipoCTe", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Controles", "NumeroControle", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Fiscais", "NumerosFiscais", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFaturaCarga> listaDocumentoFaturamento = repositorioDocumentoFaturamento.ConsultaFaturaCarga(codigoCarga);
                grid.setarQuantidadeTotal(listaDocumentoFaturamento.Count());

                grid.AdicionaRows(listaDocumentoFaturamento);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar a etapa de faturamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteReAbrirFatura)))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para cancelar a fatura.");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoJustificativaCancelamento = Request.GetIntParam("JustificativaCancelamento");

                string motivo = Request.GetStringParam("Motivo");

                unitOfWork.Start();

                servicoFatura.IniciarCancelamentoFatura(codigo, motivo, Usuario, ConfiguracaoEmbarcador, Auditado, false, null, codigoJustificativaCancelamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCancelamentoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                servicoFatura.ValidarCancelamentoFatura(codigo);

                return new JsonpResult(new
                {
                    Valido = true,
                    PermiteCancelarFatura = true,
                    Mensagem = string.Empty
                });
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(new
                {
                    Valido = false,
                    PermiteCancelarFatura = true,
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar o cancelamento da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoJustificativaCancelamento = Request.GetIntParam("JustificativaCancelamento");
                string motivo = Request.GetStringParam("Motivo");

                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork, TipoServicoMultisoftware, Auditado);

                unitOfWork.Start();

                servicoTitulo.CancelarTitulo(codigo, motivo, codigoJustificativaCancelamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
