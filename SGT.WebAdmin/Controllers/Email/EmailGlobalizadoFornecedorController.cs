using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Email
{
    [CustomAuthorize("Email/EmailGlobalizadoFornecedor")]
    public class EmailGlobalizadoFornecedorController : BaseController
    {
		#region Construtores

		public EmailGlobalizadoFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Assunto", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data envio", "DataEnvio", 55, Models.Grid.Align.left, true);

                if (!filtrosPesquisa.Situacao.HasValue)
                    grid.AdicionarCabecalho("Situacao", "DescricaoSituacaoEnvio", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor> listaEmailGlobalizadoFornecedor = repEmailGlobalizadoFornecedor.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repEmailGlobalizadoFornecedor.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaEmailGlobalizadoFornecedor
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DataEnvio,
                                 DescricaoSituacaoEnvio = p.Situacao.ObterDescricao()
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

                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor = new Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor();

                PreencherEmailGlobalizadoFornecedor(emailGlobalizadoFornecedor);

                repEmailGlobalizadoFornecedor.Inserir(emailGlobalizadoFornecedor, Auditado);

                if(!emailGlobalizadoFornecedor.EnviarTodosFornecedores)
                    SalvarFornecedores(emailGlobalizadoFornecedor, unitOfWork);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    emailGlobalizadoFornecedor.Codigo
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor = repEmailGlobalizadoFornecedor.BuscarPorCodigo(codigo);

                if (emailGlobalizadoFornecedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEmailGlobalizadoFornecedor(emailGlobalizadoFornecedor);

                SalvarFornecedores(emailGlobalizadoFornecedor, unitOfWork);

                repEmailGlobalizadoFornecedor.Atualizar(emailGlobalizadoFornecedor, Auditado);

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

                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);
                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente repEmailGlobalizadoFornecedorCliente = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor = repEmailGlobalizadoFornecedor.BuscarPorCodigo(codigo);

                if (emailGlobalizadoFornecedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente> fornecedores = repEmailGlobalizadoFornecedorCliente.BuscarPorEmailGlobalizado(codigo);

                var dynContratoNotaFiscal = new
                {
                    emailGlobalizadoFornecedor.Codigo,
                    emailGlobalizadoFornecedor.Descricao,
                    emailGlobalizadoFornecedor.CorpoEmail,
                    emailGlobalizadoFornecedor.Situacao,
                    emailGlobalizadoFornecedor.EnviarTodosFornecedores,
                    DataEnvio = emailGlobalizadoFornecedor.DataEnvio.ToString("dd/MM/yyyy HH:ss"),
                    Fornecedores = (
                        from obj in fornecedores
                        select new
                        {
                            Codigo = obj.Fornecedor.CPF_CNPJ,
                            obj.Fornecedor.Descricao
                        }).ToList(),
                    Anexos = (
                        from anexo in emailGlobalizadoFornecedor.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }).ToList(),
                };

                return new JsonpResult(dynContratoNotaFiscal);
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

                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);
                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente repEmailGlobalizadoFornecedorCliente = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente(unitOfWork);

                Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor = repEmailGlobalizadoFornecedor.BuscarPorCodigo(codigo);

                if (emailGlobalizadoFornecedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente> fornecedores = repEmailGlobalizadoFornecedorCliente.BuscarPorEmailGlobalizado(codigo);
                foreach (Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente fornecedor in fornecedores)
                    repEmailGlobalizadoFornecedorCliente.Deletar(fornecedor);

                repEmailGlobalizadoFornecedor.Deletar(emailGlobalizadoFornecedor, Auditado);

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

        private void PreencherEmailGlobalizadoFornecedor(Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor)
        {
            emailGlobalizadoFornecedor.Descricao = Request.GetStringParam("Descricao");
            emailGlobalizadoFornecedor.DataEnvio = Request.GetDateTimeParam("DataEnvio");
            emailGlobalizadoFornecedor.Situacao = Request.GetEnumParam<SituacaoEnvioEmail>("Situacao");
            emailGlobalizadoFornecedor.EnviarTodosFornecedores = Request.GetBoolParam("EnviarTodosFornecedores");
            emailGlobalizadoFornecedor.CorpoEmail = Request.GetStringParam("CorpoEmail");
        }

        private void SalvarFornecedores(Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailGlobalizadoFornecedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);
            Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente repEmailGlobalizadoFornecedorCliente = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynFornecedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Fornecedores"));

            List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente> emailGlobalizadoFornecedorClientes = repEmailGlobalizadoFornecedorCliente.BuscarPorEmailGlobalizado(emailGlobalizadoFornecedor.Codigo);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (emailGlobalizadoFornecedorClientes.Count > 0)
            {
                List<double> codigos = new List<double>();

                foreach (dynamic dynFornecedor in dynFornecedores)
                {
                    double codigo = ((string)dynFornecedor.Codigo).ToDouble();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente> listaDeletar = (from obj in emailGlobalizadoFornecedorClientes where !codigos.Contains(obj.Fornecedor.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Fornecedor",
                        De = $"{deletar.Fornecedor.Descricao}",
                        Para = ""
                    });

                    repEmailGlobalizadoFornecedorCliente.Deletar(deletar);
                }
            }

            foreach (dynamic dynFornecedor in dynFornecedores)
            {
                double codigoFornecedor = ((string)dynFornecedor.Codigo).ToDouble();

                Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente emailGlobalizadoFornecedorCliente = repEmailGlobalizadoFornecedorCliente.BuscarPorEmailGlobalizadoECliente(emailGlobalizadoFornecedor.Codigo, codigoFornecedor);

                if (emailGlobalizadoFornecedorCliente == null)
                {
                    emailGlobalizadoFornecedorCliente = new Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente();

                    emailGlobalizadoFornecedorCliente.EmailGlobalizadoFornecedor = emailGlobalizadoFornecedor;
                    emailGlobalizadoFornecedorCliente.Fornecedor = repCliente.BuscarPorCPFCNPJ(codigoFornecedor);

                    repEmailGlobalizadoFornecedorCliente.Inserir(emailGlobalizadoFornecedorCliente);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Transportador",
                        De = "",
                        Para = $"{emailGlobalizadoFornecedorCliente.Fornecedor.Descricao}"
                    });
                }
            }

            emailGlobalizadoFornecedor.SetExternalChanges(alteracoes);
        }

        private Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailGlobalizadoFornecedor()
            {
                Descricao = Request.GetStringParam("Descricao"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Situacao = Request.GetNullableEnumParam<SituacaoEnvioEmail>("Situacao"),
            };
        }

        #endregion
    }
}
