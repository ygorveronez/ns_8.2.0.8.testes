using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    public class AplicarAcrescimoDescontoNoTituloController : BaseController
    {
		#region Construtores

		public AplicarAcrescimoDescontoNoTituloController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo repAplicarAcrescimoDescontoNoTitulo = new Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo aplicaoAcrescimo = new Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo();
                Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarPrimeiroRegistro();

                string error = string.Empty;

                PreencherAplicarAcrescimoDescontoNoTitulo(aplicaoAcrescimo, unitOfWork, ref error);

                if(!string.IsNullOrEmpty(error))
                    return new JsonpResult(false, error);

                repAplicarAcrescimoDescontoNoTitulo.Inserir(aplicaoAcrescimo);

                DateTime dataMovimentacaoFinanceira = ((configuracaoFinanceiro?.GerarMovimentoPelaDataVencimentoContratoFinanceiro ?? false) || (configuracaoFinanceiro?.UtilizarDataVencimentoTituloMovimentoContrato ?? false)) ? (aplicaoAcrescimo.Titulo.DataVencimento.HasValue ? aplicaoAcrescimo.Titulo.DataVencimento.Value : aplicaoAcrescimo.Titulo.DataEmissao.Value) : aplicaoAcrescimo.Titulo.DataEmissao.Value;

                svcMovimentoFinanceiro.GerarMovimentacao(aplicaoAcrescimo.Justificativa.TipoMovimentoUsoJustificativa, dataMovimentacaoFinanceira, aplicaoAcrescimo.Valor, aplicaoAcrescimo.Titulo.Codigo.ToString(), aplicaoAcrescimo.Observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0,null, null, aplicaoAcrescimo.Titulo.Codigo, null, aplicaoAcrescimo.Titulo.Pessoa, aplicaoAcrescimo.Titulo.GrupoPessoas);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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


        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo repAplicarAcrescimoDescontoNoTitulo = new Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo aplicaoAcrescimo = repAplicarAcrescimoDescontoNoTitulo.BuscarPorCodigo(codigo);
               
                if (aplicaoAcrescimo == null)
                    return new JsonpResult(false, "Registro não existe.");


                dynamic retorno = new {
                    aplicaoAcrescimo.Codigo,
                    Titulo = new { Descricao = aplicaoAcrescimo.Titulo.Descricao, Codigo = aplicaoAcrescimo.Titulo.Codigo },
                    Justificativa = new { Descricao = aplicaoAcrescimo.Justificativa.Descricao, Codigo = aplicaoAcrescimo.Justificativa.Codigo },
                    aplicaoAcrescimo.Valor,
                    aplicaoAcrescimo.Observacao,
                    aplicaoAcrescimo.RemoverProvisao,
                    DataAutorizacao = aplicaoAcrescimo.DataAutorizacao != DateTime.MinValue ? aplicaoAcrescimo.DataAutorizacao?.ToString("dd/MM/yyyy") : string.Empty 
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo repAplicarAcrescimoDescontoNoTitulo = new Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo aplicaoAcrescimo = repAplicarAcrescimoDescontoNoTitulo.BuscarPorCodigo(codigo);

                if (aplicaoAcrescimo == null)
                    return new JsonpResult(false, "Registro não existe.");

                DateTime dataMovimentacaoFinanceira = ((configuracaoFinanceiro?.GerarMovimentoPelaDataVencimentoContratoFinanceiro ?? false) || (configuracaoFinanceiro?.UtilizarDataVencimentoTituloMovimentoContrato ?? false)) ? (aplicaoAcrescimo.Titulo.DataVencimento.HasValue ? aplicaoAcrescimo.Titulo.DataVencimento.Value : aplicaoAcrescimo.Titulo.DataEmissao.Value) : aplicaoAcrescimo.Titulo.DataEmissao.Value;

                svcMovimentoFinanceiro.GerarMovimentacao(null, dataMovimentacaoFinanceira, aplicaoAcrescimo.Titulo.ValorOriginal, aplicaoAcrescimo.Titulo.Codigo.ToString(), aplicaoAcrescimo.Observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, aplicaoAcrescimo.Justificativa.TipoMovimentoReversaoUsoJustificativa?.PlanoDeContaDebito, aplicaoAcrescimo?.Justificativa?.TipoMovimentoReversaoUsoJustificativa?.PlanoDeContaCredito, aplicaoAcrescimo.Titulo.Codigo, null, aplicaoAcrescimo?.Titulo?.Pessoa,null );

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = aplicaoAcrescimo.Titulo;

                if (aplicaoAcrescimo.RemoverProvisao)
                    aplicaoAcrescimo.Titulo.Provisao = false;

                if (aplicaoAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                    titulo.Acrescimo -= aplicaoAcrescimo.Valor;

                if (aplicaoAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                    titulo.Desconto -= aplicaoAcrescimo.Valor;

                repTitulo.Atualizar(titulo);

                repAplicarAcrescimoDescontoNoTitulo.Deletar(aplicaoAcrescimo);



                return new JsonpResult(true, true, "Registro excluido com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo repAplicarAcrescimoDescontoNoTitulo = new Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoTitulo = Request.GetIntParam("Titulo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                var grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                
 

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Titulo", "Titulo", 50, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 50, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Valor", "Valor", 50, Models.Grid.Align.left);
                grid.AdicionarCabecalho("DataAutorizacao", "DataAutorizacao", 50, Models.Grid.Align.left);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                var listaAplicarAcrescimoDescontoNoTitulo = repAplicarAcrescimoDescontoNoTitulo.BuscarPorCodigo(codigo, codigoTitulo, codigoJustificativa,  propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repAplicarAcrescimoDescontoNoTitulo.ContarBuscar(codigo, codigoTitulo, codigoJustificativa);

                var listaAplicarAcrescimoDescontoNoTituloRetornar = (
                    from item in listaAplicarAcrescimoDescontoNoTitulo
                    select new
                    {
                        item.Codigo,
                        Titulo = item?.Titulo?.Descricao ?? string.Empty,
                        Justificativa = item?.Justificativa?.Descricao ?? string.Empty,
                        Valor = item?.Valor ?? 0,
                        DataAutorizacao = item.DataAutorizacao != DateTime.MinValue ? item.DataAutorizacao?.ToString("dd/MM/yyyy") : null,

                    }
                ).ToList();

                grid.AdicionaRows(listaAplicarAcrescimoDescontoNoTituloRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherAplicarAcrescimoDescontoNoTitulo(Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo aplicaoAcrescismo, Repositorio.UnitOfWork unitOfWork, ref string error)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            int codigoTitulo = Request.GetIntParam("Titulo");
            int codigoJustificativa = Request.GetIntParam("Justificativa");

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
            Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

            aplicaoAcrescismo.Justificativa = justificativa;
            aplicaoAcrescismo.Titulo = titulo;
            aplicaoAcrescismo.Valor = Request.GetDecimalParam("Valor");
            aplicaoAcrescismo.DataAutorizacao = Request.GetNullableDateTimeParam("DataAutorizacao");
            titulo.DataAutorizacao = Request.GetNullableDateTimeParam("DataAutorizacao");
            aplicaoAcrescismo.RemoverProvisao = Request.GetBoolParam("RemoverProvisaoTitulo");
            aplicaoAcrescismo.Observacao = Request.GetStringParam("Observacao");

            if (aplicaoAcrescismo.RemoverProvisao)
                aplicaoAcrescismo.Titulo.Provisao = false;

            if (justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                titulo.Acrescimo += aplicaoAcrescismo.Valor;
            
            if (justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                titulo.Desconto += aplicaoAcrescismo.Valor;

            if (aplicaoAcrescismo.Justificativa.TipoJustificativa == TipoJustificativa.Desconto && aplicaoAcrescismo.Valor > aplicaoAcrescismo.Titulo.ValorOriginal)
                error += "Valor do desconto não pode ser maior que o valor do título";

            if(string.IsNullOrEmpty(error))
                repTitulo.Atualizar(titulo);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Valor")
                return "Valor";

            return propriedadeOrdenar;
        }
        #endregion

    }
}
