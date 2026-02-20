using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/ComissaoFuncionario")]
    public class ComissaoFuncionarioMotoristaController : BaseController
    {
		#region Construtores

		public ComissaoFuncionarioMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork);
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosComissaoMotorista()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");


            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            try
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados))
                {
                    unitOfWork.Start();
                    int codigo = 0, numeroDiasEmViagem = 0;
                    decimal valorNormativo = 0;
                    int.TryParse(Request.Params("Codigo"), out codigo);
                    int.TryParse(Request.Params("NumeroDiasEmViagem"), out numeroDiasEmViagem);
                    decimal.TryParse(Request.Params("ValorNormativo"), out valorNormativo);
                    bool atingiuMedia = Request.GetBoolParam("AtingiuMedia");

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo, true);

                    if (comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                        && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                    {
                        comissaoFuncionarioMotorista.AtingiuMedia = atingiuMedia;
                        comissaoFuncionarioMotorista.NumeroDiasEmViagem = numeroDiasEmViagem;
                        comissaoFuncionarioMotorista.ValorNormativo = valorNormativo;
                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista, Auditado);
                        var retorno = serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(comissaoFuncionarioMotorista);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionarioMotorista.ComissaoFuncionario, null, "Alterou Dados do Motorista " + comissaoFuncionarioMotorista.Descricao + ".", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite que ela seja alterada ");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dias em viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarComissao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            try
            {

                unitOfWork.Start();
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados))
                {
                    int codigo = 0;
                    int.TryParse(Request.Params("Codigo"), out codigo);

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo);

                    if (comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                        && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                    {
                        comissaoFuncionarioMotorista.GerarComissao = true;
                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
                        var retorno = serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(comissaoFuncionarioMotorista);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionarioMotorista.ComissaoFuncionario, null, "Gerou Comissão do Motorista " + comissaoFuncionarioMotorista.Descricao + ".", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite liberar a geração da comissão para o motorista. ");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar a geração da comissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> NaoGerarComissao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            try
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados))
                {
                    unitOfWork.Start();

                    int codigo = 0;
                    int.TryParse(Request.Params("Codigo"), out codigo);

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo);

                    if (comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                        && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                    {
                        comissaoFuncionarioMotorista.GerarComissao = false;
                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
                        var retorno = serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(comissaoFuncionarioMotorista);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionarioMotorista.ComissaoFuncionario, null, "Não Georou Comissão do Motorista " + comissaoFuncionarioMotorista.Descricao + ".", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite não gerar a comissão para o motorista. ");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar a geração da comissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarMedia()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            try
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermitirAlterarMedia))
                {
                    unitOfWork.Start();
                    int codigo = 0;
                    int.TryParse(Request.Params("Codigo"), out codigo);
                    decimal media = Request.GetDecimalParam("Media");

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo, true);

                    if (comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                        && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                    {
                        comissaoFuncionarioMotorista.MediaFinal = media;
                        if (comissaoFuncionarioMotorista.MediaFinal > 0 && comissaoFuncionarioMotorista.MediaIdeal <= comissaoFuncionarioMotorista.MediaFinal)
                            comissaoFuncionarioMotorista.AtingiuMedia = true;

                        if (comissaoFuncionarioMotorista.FaturamentoMinimo < comissaoFuncionarioMotorista.ValoBaseCalculo)
                        {
                            comissaoFuncionarioMotorista.ValoBaseCalculo = Math.Round(comissaoFuncionarioMotorista.ValoBaseCalculo - comissaoFuncionarioMotorista.FaturamentoMinimo, 2, MidpointRounding.AwayFromZero);
                            if (comissaoFuncionarioMotorista.ValoBaseCalculo <= 0)
                                comissaoFuncionarioMotorista.ValoBaseCalculo = 0;

                            decimal percentualPadrao = comissaoFuncionarioMotorista.Motorista.CargoMotorista != null && comissaoFuncionarioMotorista.Motorista.CargoMotorista.ComissaoPadrao > 0 ? comissaoFuncionarioMotorista.Motorista.CargoMotorista.ComissaoPadrao : ConfiguracaoEmbarcador.PercentualComissaoPadrao;

                            comissaoFuncionarioMotorista.ValorComissao = Math.Round(comissaoFuncionarioMotorista.ValoBaseCalculo * (percentualPadrao / 100), 2, MidpointRounding.AwayFromZero);

                            decimal valorComissaoOriginal = comissaoFuncionarioMotorista.ValorComissao;
                            if (!comissaoFuncionarioMotorista.AtingiuMedia && comissaoFuncionarioMotorista.PercentualMedia > 0)
                            {
                                decimal reducaoMedia = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualMedia / 100), 2, MidpointRounding.AwayFromZero);
                                comissaoFuncionarioMotorista.ValorComissao -= reducaoMedia;
                            }
                            if (!comissaoFuncionarioMotorista.NaoHouveAdvertencia && comissaoFuncionarioMotorista.PercentualAdvertencia > 0)
                            {
                                decimal reducaoAdvertencia = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualAdvertencia / 100), 2, MidpointRounding.AwayFromZero);
                                comissaoFuncionarioMotorista.ValorComissao -= reducaoAdvertencia;
                            }
                            if (!comissaoFuncionarioMotorista.NaoHouveSinitro && comissaoFuncionarioMotorista.PercentualSinistro > 0)
                            {
                                decimal reducaoSinistro = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualSinistro / 100), 2, MidpointRounding.AwayFromZero);
                                comissaoFuncionarioMotorista.ValorComissao -= reducaoSinistro;
                            }

                            if (comissaoFuncionarioMotorista.ValorComissao <= 0)
                                comissaoFuncionarioMotorista.ValorComissao = 0;
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionarioMotorista.ComissaoFuncionario, null, "Alterou a média do Motorista " + comissaoFuncionarioMotorista.Descricao + ".", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true, true, "Sucesso");
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite que ela seja alterada ");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dias em viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BaixarRelatorioMotorista()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unidadeTrabalho);
                Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unidadeTrabalho);
                int codigo = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo);
                Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unidadeTrabalho);

                IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> comissoesFuncionarioMotorista = repComissaoFuncionarioMotoristaDocumento.ConsultaRelatorio(comissaoFuncionarioMotorista.ComissaoFuncionario.Codigo, comissaoFuncionarioMotorista.Codigo);
                IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> comissaoFuncionarioMotoristaAbastecimento = repComissaoFuncionarioMotoristaDocumento.ConsultaRelatorioAbastecimento(comissaoFuncionarioMotorista.ComissaoFuncionario.Codigo, comissaoFuncionarioMotorista.Codigo);
                if (comissaoFuncionarioMotoristaAbastecimento == null || comissaoFuncionarioMotoristaAbastecimento.Count == 0)
                {
                    comissaoFuncionarioMotoristaAbastecimento = new List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento>();
                    comissaoFuncionarioMotoristaAbastecimento.Add(new Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento { CodigoComissaoFuncionarioMotorista = -1 });
                }
                string mensagem = "";
                string nomeCliente = ClienteAcesso.Cliente.RazaoSocial;



                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao =
                    new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao(); 
                
                byte[] relatorio = ReportRequest.WithType(ReportType.ComissaoMotoristas)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoComissao", comissaoFuncionarioMotorista.ComissaoFuncionario.Codigo)
                    .AddExtraData("nomeCliente",nomeCliente )
                    .AddExtraData("comissoesFuncionarioMotorista", comissoesFuncionarioMotorista.ToJson())
                    .AddExtraData("utilizarComissaoPorCargo", ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                    .AddExtraData("comissaoFuncionarioMotoristaAbastecimento", comissaoFuncionarioMotoristaAbastecimento.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("retornaArrayByte", true)
                    .CallReport()
                    .GetContentFile(); 
                
                    return Arquivo(relatorio, "application/pdf", "Comissão de " + comissaoFuncionarioMotorista.Motorista.Nome + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Models.Grid.EditableCell editableDiasEmViagem = null;
            Models.Grid.EditableCell editableAtingiuMedia = null;
            Models.Grid.EditableCell editableNormativo    = null;

            if (!exportacao)
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");

                editableAtingiuMedia = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aBool);

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados)) { 
                    editableDiasEmViagem = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt, 3);
                    editableNormativo    = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal);
                }
            }

            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);

            int codigoCargoMotorista = Request.GetIntParam("CargoMotorista");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoComissaoFuncionario = Request.GetIntParam("ComissaoFuncionario");
            bool motoristaComDoisModelos = Request.GetBoolParam("MotoristaComDoisModelos");

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("ValorDiaria", false);
            grid.AdicionarCabecalho("GerarComissao", false);
            grid.AdicionarCabecalho("CodigoMotorista", false);
            grid.AdicionarCabecalho("MediaFinal", false);
            grid.AdicionarCabecalho("DT_Enable", false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 30, Models.Grid.Align.left, true);
            if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
            {
                grid.AdicionarCabecalho("Nº Frota", "PrimeiraFrota", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ValorProdutividade", false);
                grid.AdicionarCabecalho("NumeroDiasEmViagem", false);
            }
            else
            {
                grid.AdicionarCabecalho("PrimeiraFrota", false);
                grid.AdicionarCabecalho("Produtividade", "ValorProdutividade", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Dias em Viagem", "NumeroDiasEmViagem", 8, Models.Grid.Align.center, true, false, false, false, true, editableDiasEmViagem);
            }

            grid.AdicionarCabecalho("Total Frete", "ValorTotalFrete", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Frete Líquido", "ValoFreteLiquido", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("B. Calculo", "ValoBaseCalculo", 8, Models.Grid.Align.right, true);
            if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                grid.AdicionarCabecalho("PercentualComissao", false);
            else
                grid.AdicionarCabecalho("% Comissão", "PercentualComissao", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Comissão", "ValorComissao", 8, Models.Grid.Align.right, true);
            if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                grid.AdicionarCabecalho("", "ValorNormativo", 8, Models.Grid.Align.left, false, false, false, false, true, editableNormativo);
            else
                grid.AdicionarCabecalho("Normativo", "ValorNormativo", 8, Models.Grid.Align.right, true, false, false, false, true, editableNormativo);
            grid.AdicionarCabecalho("Total", "Total", 8, Models.Grid.Align.right, false);
            if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                grid.AdicionarCabecalho("AtingiuMedia", false);
            else
                grid.AdicionarCabecalho("Atingiu a Média?", "AtingiuMedia", 10, Models.Grid.Align.center, false, false, false, false, true, editableAtingiuMedia);

            string propOrdena = grid.header[grid.indiceColunaOrdena].data;

            if (propOrdena == "Motorista")
                propOrdena += ".Nome";
            else if (propOrdena == "ValorProdutividade")
                propOrdena = "TabelaProdutividadeValores.Valor";

            List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista> listaComissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.Consultar(motoristaComDoisModelos, codigoCargoMotorista, codigoMotorista, codigoComissaoFuncionario, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repComissaoFuncionarioMotorista.ContarConsulta(motoristaComDoisModelos, codigoCargoMotorista, codigoMotorista, codigoComissaoFuncionario));
            var lista = (from obj in listaComissaoFuncionarioMotorista select serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(obj)).ToList();
            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        #endregion
    }
}
