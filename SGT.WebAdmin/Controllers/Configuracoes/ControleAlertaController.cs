using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ControleAlerta")]
    public class ControleAlertaController : BaseController
    {
		#region Construtores

		public ControleAlertaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarAlertasLicencas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.Alerta repAlerta = new Repositorio.Embarcador.Configuracoes.Alerta(unitOfWork);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                int codigoFuncionario = this.Usuario.Codigo;
                List<Dominio.Entidades.Embarcador.Configuracoes.Alerta> listaAlertas = repAlerta.BuscarAlertasLicencas(codigoEmpresa, codigoFuncionario);

                var lista = (from p in listaAlertas
                             orderby p.Codigo, p.Data
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Data = p.Data.ToString("dd/MM/yyyy")
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os alertas das licenças");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> OcultarAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.Alerta repAlerta = new Repositorio.Embarcador.Configuracoes.Alerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Alerta alerta = repAlerta.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                alerta.Initialize();
                alerta.Ocultar = true;

                repAlerta.Atualizar(alerta);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, alerta, alerta.GetChanges(), "Ocultou alerta", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao ocultar o alerta selecionado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFuncionario = Request.GetIntParam("Funcionario");

                List<ControleAlertaForma> formasAlerta = Request.GetListEnumParam<ControleAlertaForma>("FormaAlerta");
                List<ControleAlertaTela> telasAlerta = Request.GetListEnumParam<ControleAlertaTela>("TelaAlerta");

                SituacaoAtivoPesquisa status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Funcionário", "Funcionario", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Formas", "FormaAlerta", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Telas", "TelaAlerta", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Dias", "QuantidadeDias", 10, Models.Grid.Align.right, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> listaControleAlerta = repControleAlerta.Consulta(codigoFuncionario, codigoEmpresa, status, formasAlerta, telasAlerta, parametrosConsulta);
                grid.setarQuantidadeTotal(repControleAlerta.ContaConsulta(codigoFuncionario, codigoEmpresa, status, formasAlerta, telasAlerta));

                var lista = (from p in listaControleAlerta
                             select new
                             {
                                 p.Codigo,
                                 Funcionario = p.Funcionario?.Nome ?? string.Empty,
                                 FormaAlerta = string.Join(", ", p.FormasAlerta.Select(o => o.ObterDescricao())),
                                 TelaAlerta = string.Join(", ", p.TelasAlerta.Select(o => o.ObterDescricao())),
                                 p.QuantidadeDias
                             }).ToList();

                grid.AdicionaRows(lista);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta = new Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta();

                PreencherControleAlerta(controleAlerta, unitOfWork);

                repControleAlerta.Inserir(controleAlerta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta = repControleAlerta.BuscarPorCodigo(codigo, true);

                if (controleAlerta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherControleAlerta(controleAlerta, unitOfWork);

                repControleAlerta.Atualizar(controleAlerta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta = repControleAlerta.BuscarPorCodigo(codigo);

                if (controleAlerta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic retorno = new
                {
                    controleAlerta.Codigo,
                    controleAlerta.QuantidadeDias,
                    controleAlerta.Status,
                    FormaAlerta = controleAlerta.FormasAlerta.ToList(),
                    TelaAlerta = controleAlerta.TelasAlerta.ToList(),
                    Situacao = controleAlerta.SituacoesOrdemServico.ToList(),
                    Funcionario = controleAlerta.Funcionario != null ? new { controleAlerta.Funcionario.Codigo, Descricao = controleAlerta.Funcionario.Nome } : null,
                    controleAlerta.QuantidadeDiasAlertaOsInterna,
                    controleAlerta.QuantidadeDiasAlertaOsExterna
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta = repControleAlerta.BuscarPorCodigo(codigo, true);

                if (controleAlerta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repControleAlerta.Deletar(controleAlerta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherControleAlerta(Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            int codigoFuncionario = Request.GetIntParam("Funcionario");

            controleAlerta.QuantidadeDias = Request.GetIntParam("QuantidadeDias");
            controleAlerta.Status = Request.GetBoolParam("Status");

            controleAlerta.QuantidadeDiasAlertaOsInterna = Request.GetIntParam("QuantidadeDiasAlertaOsInterna");
            controleAlerta.QuantidadeDiasAlertaOsExterna = Request.GetIntParam("QuantidadeDiasAlertaOsExterna");

            controleAlerta.Funcionario = codigoFuncionario > 0 ? repFuncionario.BuscarPorCodigo(codigoFuncionario) : null;

            if (controleAlerta.Codigo == 0)
                controleAlerta.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null;

            PreencherControleAlertaListas(controleAlerta);
        }

        private void PreencherControleAlertaListas(Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controleAlerta)
        {
            List<ControleAlertaForma> formasAlerta = Request.GetListEnumParam<ControleAlertaForma>("FormaAlerta");
            List<ControleAlertaTela> telasAlerta = Request.GetListEnumParam<ControleAlertaTela>("TelaAlerta");
            List<SituacaoOrdemServicoFrota> situacoes = Request.GetListEnumParam<SituacaoOrdemServicoFrota>("Situacao");

            if (controleAlerta.FormasAlerta == null)
                controleAlerta.FormasAlerta = new List<ControleAlertaForma>();
            else
                controleAlerta.FormasAlerta.Clear();

            foreach (ControleAlertaForma formaAlerta in formasAlerta)
                controleAlerta.FormasAlerta.Add(formaAlerta);

            if (controleAlerta.TelasAlerta == null)
                controleAlerta.TelasAlerta = new List<ControleAlertaTela>();
            else
                controleAlerta.TelasAlerta.Clear();

            foreach (ControleAlertaTela telaAlerta in telasAlerta)
                controleAlerta.TelasAlerta.Add(telaAlerta);

            if (controleAlerta.SituacoesOrdemServico == null)
                controleAlerta.SituacoesOrdemServico = new List<SituacaoOrdemServicoFrota>();
            else
                controleAlerta.SituacoesOrdemServico.Clear();

            foreach (SituacaoOrdemServicoFrota situacao in situacoes)
                controleAlerta.SituacoesOrdemServico.Add(situacao);
        }

        #endregion
    }
}
