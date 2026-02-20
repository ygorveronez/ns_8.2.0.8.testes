using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ControleArquivo", "Contabils/ControleArquivoAnexo")]
    public class ControleArquivoController : BaseController
    {
		#region Construtores

		public ControleArquivoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Anexo.ControleArquivo repositorio = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo = null;
                int codigo = Request.GetIntParam("Codigo");
                if (codigo > 0)
                    controleArquivo = repositorio.BuscarPorCodigo(codigo, true);
                else
                    controleArquivo = new Dominio.Entidades.Embarcador.Anexo.ControleArquivo();

                try
                {
                    PreencherControleArquivo(controleArquivo, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                controleArquivo.Empresa = this.Usuario.Empresa;
                Servicos.Embarcador.Anexo.ControleArquivo servicoControleArquivo = new Servicos.Embarcador.Anexo.ControleArquivo(unitOfWork);

                if (codigo == 0)
                {
                    controleArquivo.GerouAlertaEmpresa = false;
                    controleArquivo.GerouAlertaCliente = false;

                    repositorio.Inserir(controleArquivo, Auditado);

                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCliente(controleArquivo.Cliente.CPF_CNPJ, this.Usuario.Empresa.Codigo);

                    if (usuario != null)
                        servicoControleArquivo.EnviarEmailAlerta(controleArquivo);                    
                }
                else
                    repositorio.Atualizar(controleArquivo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    controleArquivo.Codigo
                });
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.ControleArquivo repositorio = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo = repositorio.BuscarPorCodigo(codigo);

                if (controleArquivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    controleArquivo.Codigo,
                    controleArquivo.Descricao,
                    controleArquivo.Observacao,
                    controleArquivo.GerouAlertaCliente,
                    controleArquivo.GerouAlertaEmpresa,
                    Cliente = new { controleArquivo.Cliente.Codigo, controleArquivo.Cliente.Descricao },
                    DataVencimento = controleArquivo.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                    Anexos = (
                        from anexo in controleArquivo.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                            anexo.RealizouDownload
                        }
                    ).ToList(),
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
                Repositorio.Embarcador.Anexo.ControleArquivo repositorio = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (controleArquivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(controleArquivo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.ControleArquivo repositorio = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo = repositorio.BuscarPorCodigo(codigo);

                if (controleArquivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (controleArquivo.Cliente == null)
                    return new JsonpResult(false, true, "O registro não possui cliente vinculado.");

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCliente(controleArquivo.Cliente.CPF_CNPJ, this.Usuario.Empresa.Codigo);

                if (usuario == null)
                    return new JsonpResult(false, true, "O cliente não está associado a nenhum usuário, favor realize o cadastro antes para enviar o aviso.");

                Servicos.Embarcador.Anexo.ControleArquivo servicoControleArquivo = new Servicos.Embarcador.Anexo.ControleArquivo(unitOfWork);

                if (!servicoControleArquivo.EnviarEmailAlerta(controleArquivo))
                    return new JsonpResult(false, true, "Não foi possível enviar o alerta, verifique os e-mails cadastrados.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, controleArquivo, null, $"Enviou o alerta manualmente ao cliente.", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar o alerta.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        #endregion

        #region Métodos Privados

        private void PreencherControleArquivo(Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            controleArquivo.DataVencimento = Request.GetNullableDateTimeParam("DataVencimento");
            controleArquivo.Descricao = Request.GetNullableStringParam("Descricao");
            controleArquivo.Observacao = Request.GetNullableStringParam("Observacao");
            controleArquivo.Cliente = this.Usuario.Cliente != null ? this.Usuario.Cliente : ObterPessoa(unitOfWork);
            controleArquivo.GerouAlertaEmpresa = Request.GetBoolParam("GerouAlertaEmpresa");
            controleArquivo.GerouAlertaCliente = Request.GetBoolParam("GerouAlertaCliente");
        }

        private Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo()
            {
                CpfCnpjPessoa = this.Usuario.Cliente != null ? this.Usuario.Cliente.CPF_CNPJ : Request.GetDoubleParam("Cliente"),
                Descricao = Request.GetStringParam("Descricao"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

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
                grid.AdicionarCabecalho("Cliente", "Cliente", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 10, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Anexo.FiltroPesquisaControleArquivo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Anexo.ControleArquivo repositorio = new Repositorio.Embarcador.Anexo.ControleArquivo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Anexo.ControleArquivo> listaControleArquivo = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Anexo.ControleArquivo>();

                var listaControleArquivoRetornar = (
                    from controleArquivo in listaControleArquivo
                    select new
                    {
                        controleArquivo.Codigo,
                        Cliente = controleArquivo.Cliente.Descricao,
                        controleArquivo.Descricao,
                        DataVencimento = controleArquivo.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                        DT_RowColor = controleArquivo.Anexos != null && controleArquivo.Anexos.Count > 0 ? controleArquivo.Anexos.Any(o => !o.RealizouDownload) ? "#f0d8d8" : "#d8f0da" : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaControleArquivoRetornar);
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

        private Dominio.Entidades.Cliente ObterPessoa(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjPessoa = Request.GetDoubleParam("Cliente");
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            return repositorio.BuscarPorCPFCNPJ(cpfCnpjPessoa) ?? throw new ControllerException("Cliente não encontrada.");
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            //if (propriedadeOrdenar == "Banco")
            //    return "Banco.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
