using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Email
{
    [CustomAuthorize("Email/EmailDocumentacaoCarga")]
    public class EmailDocumentacaoCargaController : BaseController
    {
		#region Construtores

		public EmailDocumentacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = new Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga();

                PreencherDados(emailDocumentacaoCarga, unitOfWork);

                if (emailDocumentacaoCarga.EnviarCTe && !(emailDocumentacaoCarga.EnviarCTeXML || emailDocumentacaoCarga.EnviarCTePDF))
                    throw new ControllerException("Necessário escolher pelo menos um tipo de arquivo quando o envio de CTe esta marcado!");

                repEmailDocumentacaoCarga.Inserir(emailDocumentacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repEmailDocumentacaoCarga.BuscarPorCodigo(codigo);

                PreencherDados(emailDocumentacaoCarga, unitOfWork);

                if (emailDocumentacaoCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (emailDocumentacaoCarga.EnviarCTe && !(emailDocumentacaoCarga.EnviarCTeXML || emailDocumentacaoCarga.EnviarCTePDF))
                    throw new ControllerException("Necessário escolher pelo menos um tipo de arquivo quando o Documento Fiscal/CTe está marcado!");

                unitOfWork.Start();

                repEmailDocumentacaoCarga.Atualizar(emailDocumentacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repEmailDocumentacaoCarga.BuscarPorCodigo(codigo, true);


                if (emailDocumentacaoCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    emailDocumentacaoCarga.Codigo,
                    Pessoa = new { emailDocumentacaoCarga.Pessoa?.Descricao, emailDocumentacaoCarga.Pessoa?.Codigo },
                    TiposOperacoes = (
                    from obj in emailDocumentacaoCarga.TiposOperacao
                    orderby obj.Descricao
                    select new
                    {
                        Tipo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                     ).ToList(),
                    emailDocumentacaoCarga.Emails,
                    emailDocumentacaoCarga.EnviarCTe,
                    emailDocumentacaoCarga.EnviarCTeXML,
                    emailDocumentacaoCarga.EnviarCTePDF,
                    emailDocumentacaoCarga.EnviarMDFe,
                    emailDocumentacaoCarga.EnviarContratoFrete,
                    EnviarCIOT = emailDocumentacaoCarga.EnviarCIOT ?? true,
                    emailDocumentacaoCarga.AgruparEnvioEmUmUnicoEmail
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga = repEmailDocumentacaoCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (emailDocumentacaoCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repEmailDocumentacaoCarga.Deletar(emailDocumentacaoCarga, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("E-mail", "Emails", 30, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Email.EmailDocumentacaoCarga repEmailDocumentacaoCarga = new Repositorio.Embarcador.Email.EmailDocumentacaoCarga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repEmailDocumentacaoCarga.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga> listaEmailDocumentacaoCarga = totalRegistros > 0 ? repEmailDocumentacaoCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga>();

                var lista = (from p in listaEmailDocumentacaoCarga
                             select new
                             {
                                 p.Codigo,
                                 Pessoa = p.Pessoa?.Nome + " (" + p.Pessoa?.CPF_CNPJ_Formatado + ")",
                                 Emails = p.Emails,
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga()
            {
                CodigoPessoa = Request.GetDoubleParam("Pessoa"),
                Emails = Request.GetStringParam("Emails"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
            };
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            double codigoPessoa = Request.GetDoubleParam("Pessoa");


            emailDocumentacaoCarga.Codigo = Request.GetIntParam("Codigo");
            emailDocumentacaoCarga.Pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);
            emailDocumentacaoCarga.Emails = Request.GetStringParam("Emails");
            emailDocumentacaoCarga.EnviarCTe = Request.GetBoolParam("EnviarCTe");
            emailDocumentacaoCarga.EnviarCTeXML = Request.GetBoolParam("EnviarCTeXML");
            emailDocumentacaoCarga.EnviarCTePDF = Request.GetBoolParam("EnviarCTePDF");
            emailDocumentacaoCarga.EnviarMDFe = Request.GetBoolParam("EnviarMDFe");
            emailDocumentacaoCarga.EnviarContratoFrete = Request.GetBoolParam("EnviarContratoFrete");
            emailDocumentacaoCarga.EnviarCIOT = Request.GetBoolParam("EnviarCIOT");
            emailDocumentacaoCarga.AgruparEnvioEmUmUnicoEmail = Request.GetBoolParam("AgruparEnvioEmUmUnicoEmail");

            SalvarTiposOperacao(emailDocumentacaoCarga, unitOfWork);
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacoes"));

            if (emailDocumentacaoCarga.TiposOperacao == null)
                emailDocumentacaoCarga.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.Tipo?.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = emailDocumentacaoCarga.TiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    emailDocumentacaoCarga.TiposOperacao.Remove(tipoOperacaoDeletar);
            }

            foreach (var tipoOperacao in tiposOperacao)
            {
                if (emailDocumentacaoCarga.TiposOperacao.Any(o => o.Codigo == (int)tipoOperacao.Tipo?.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoObj = repositorioTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Tipo.Codigo);
                emailDocumentacaoCarga.TiposOperacao.Add(tipoOperacaoObj);
            }
        }


        #endregion
    }
}
