using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/DatasPreferenciaisDescarga")]
    public class DataPreferenciaDescargaController : BaseController
    {
		#region Construtores

		public DataPreferenciaDescargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga = new Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga();

                InserirEntidadePrincipal(dataPreferencialDescarga, unitOfWork);
                InserirFornecedoresCategorias(dataPreferencialDescarga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, excecao.Message);
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
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga = new Repositorio.Embarcador.Logistica.DataPreferencialDescarga(unitOfWork).BuscarPorCodigo(Request.GetIntParam("Codigo"), false);

                if (dataPreferencialDescarga == null)
                    return new JsonpResult("O registro não foi encontrado.");

                InserirEntidadePrincipal(dataPreferencialDescarga, unitOfWork);
                InserirFornecedoresCategorias(dataPreferencialDescarga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, excecao.Message);
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

                Repositorio.Embarcador.Logistica.DataPreferencialDescarga repositorio = new Repositorio.Embarcador.Logistica.DataPreferencialDescarga(unitOfWork);
                Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorioCategoriaFornecedor = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga = repositorio.BuscarPorCodigo(codigo, false);

                if (dataPreferencialDescarga == null)
                    return new JsonpResult("O registro não foi encontrado.");

                List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> listaCategoriaFornecedor = repositorioCategoriaFornecedor.BuscarPorDataPreferencialDescargaComFetch(dataPreferencialDescarga.Codigo);

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        dataPreferencialDescarga.Codigo,
                        CentroDescarregamento = new { dataPreferencialDescarga.CentroDescarregamento.Codigo, dataPreferencialDescarga.CentroDescarregamento.Descricao },
                        DataPreferencial = dataPreferencialDescarga.DiaPreferencial,
                        DiasAnterioresBloqueados = dataPreferencialDescarga.DiasBloqueio
                    },
                    ListaFornecedorCategoria = (from obj in listaCategoriaFornecedor
                                                select new
                                                {
                                                    obj.Codigo,
                                                    CodigoFornecedor = obj.Fornecedor?.CPF_CNPJ ?? 0,
                                                    CodigoGrupoFornecedor = obj.GrupoFornecedor?.Codigo ?? 0,
                                                    CodigoCategoria = obj.Categoria.Codigo,
                                                    Fornecedor = obj.Fornecedor?.Descricao ?? "",
                                                    GrupoFornecedor = obj.GrupoFornecedor?.Descricao ?? "",
                                                    Categoria = obj.Categoria.Descricao
                                                })
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

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.DataPreferencialDescarga repositorio = new Repositorio.Embarcador.Logistica.DataPreferencialDescarga(unitOfWork);
                Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorioCategoriaFornecedor = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga = repositorio.BuscarPorCodigo(codigo, false);

                if (dataPreferencialDescarga == null)
                    return new JsonpResult("O registro não foi encontrado.");

                List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> listaCategoriaFornecedor = repositorioCategoriaFornecedor.BuscarPorDataPreferencialDescarga(codigo);

                foreach (Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria registro in listaCategoriaFornecedor)
                    repositorioCategoriaFornecedor.Deletar(registro);

                repositorio.Deletar(dataPreferencialDescarga);

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

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();
                List<dynamic> registros = new List<dynamic>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, registros, ((dados) =>
                {
                    return ObterRegistro(dados);
                }));

                if (retorno == null || registros?.Count <= 0)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                RetornoImportacao retornoImportacao = ImportarRegistro(registros, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridExportacao();

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void InserirEntidadePrincipal(Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DataPreferencialDescarga repositorioDataPreferencialDescarga = new Repositorio.Embarcador.Logistica.DataPreferencialDescarga(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            dataPreferencialDescarga.CentroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(Request.GetIntParam("CentroDescarregamento"));
            dataPreferencialDescarga.DiaPreferencial = Request.GetIntParam("DataPreferencial");
            dataPreferencialDescarga.DiasBloqueio = Request.GetIntParam("DiasAnterioresBloqueados");

            if (dataPreferencialDescarga.Codigo > 0)
                repositorioDataPreferencialDescarga.Atualizar(dataPreferencialDescarga);
            else
                repositorioDataPreferencialDescarga.Inserir(dataPreferencialDescarga);
        }

        private void InserirFornecedoresCategorias(Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorio = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            dynamic listaFornecedorCategoria = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFornecedorCategoria"));

            List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> registrosDeletar = repositorio.BuscarPorDataPreferencialDescarga(dataPreferencialDescarga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria registro in registrosDeletar)
                repositorio.Deletar(registro);

            foreach (dynamic item in listaFornecedorCategoria)
            {
                double codigoFornecedor = ((string)item.CodigoFornecedor).ToDouble();
                int codigoGrupoFornecedor = ((string)item.CodigoGrupoFornecedor).ToInt();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repositorioProdutoEmbarcador.BuscarPorCodigo(((string)item.CodigoCategoria).ToInt());
                Dominio.Entidades.Cliente fornecedor = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoFornecedor = null;

                if (codigoFornecedor > 0)
                    fornecedor = repositorioCliente.BuscarPorCPFCNPJ(codigoFornecedor);

                else
                    grupoFornecedor = repositorioGrupoPessoa.BuscarPorCodigo(codigoGrupoFornecedor);

                Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria fornecedorCategoria = SalvarFornecedorCategoria(dataPreferencialDescarga, produtoEmbarcador, fornecedor, grupoFornecedor, unitOfWork);

                repositorio.Inserir(fornecedorCategoria);
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria SalvarFornecedorCategoria(Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Dominio.Entidades.Cliente fornecedor, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoFornecedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorio = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria fornecedorCategoria = new Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria();

            if (repositorio.BuscarContagemPorCPFCategoriaCentroDescarregamento(fornecedor?.CPF_CNPJ ?? 0, grupoFornecedor?.Codigo ?? 0, produtoEmbarcador.Codigo, dataPreferencialDescarga.CentroDescarregamento.Codigo) >= 2)
                throw new ControllerException($"Já existem 2 datas cadastradas para esse CNPJ/Portfólio. ({fornecedor.CPF_CNPJ_Formatado} / {produtoEmbarcador.Descricao})");

            fornecedorCategoria.DataPreferencialDescarga = dataPreferencialDescarga;
            fornecedorCategoria.Categoria = produtoEmbarcador;
            fornecedorCategoria.Fornecedor = fornecedor;
            fornecedorCategoria.GrupoFornecedor = grupoFornecedor;

            return fornecedorCategoria;
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
                grid.AdicionarCabecalho("Centro Descarregamento", "CentroDescarregamento", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Fornecedor", "GrupoFornecedor", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Categoria", "Categoria", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia Preferencial", "DiaPreferencial", 50, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Dias Bloqueio", "DiasBloqueio", 50, Models.Grid.Align.right, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorioFornecedorCategoria = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repositorioFornecedorCategoria.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> registrosFornecedorCategoria = totalRegistros > 0 ? repositorioFornecedorCategoria.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

                var listaRetornar = (
                    from registro in registrosFornecedorCategoria
                    select new
                    {
                        registro.DataPreferencialDescarga.Codigo,
                        registro.DataPreferencialDescarga.DiaPreferencial,
                        CentroDescarregamento = registro.DataPreferencialDescarga.CentroDescarregamento.Descricao,
                        registro.DataPreferencialDescarga.DiasBloqueio,
                        Fornecedor = registro.Fornecedor?.Descricao ?? "",
                        GrupoFornecedor = registro.GrupoFornecedor?.Descricao ?? "",
                        Categoria = registro.Categoria.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception excecao)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridExportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Centro Descarregamento", "CentroDescarregamento", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia Preferencial", "DiaPreferencial", 50, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Dias Bloqueio", "DiasBloqueio", 50, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Categoria", "Categoria", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 50, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorio = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> registros = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

                var listaRetornar = (
                    from registro in registros
                    select new
                    {
                        registro.Codigo,
                        registro.DataPreferencialDescarga.DiaPreferencial,
                        CentroDescarregamento = registro.DataPreferencialDescarga.CentroDescarregamento.Descricao,
                        registro.DataPreferencialDescarga.DiasBloqueio,
                        Categoria = registro.Categoria.Descricao,
                        Fornecedor = registro.Fornecedor.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception excecao)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga()
            {
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
                DiaPreferencial = Request.GetIntParam("DiaPreferencial")
            };
        }

        private List<ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ Fornecedor", Propriedade = "CNPJFornecedor", Tamanho = tamanho, Obrigatorio = false });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Categoria", Propriedade = "Categoria", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Centro de Descarregamento", Propriedade = "CentroDescarregamento", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Dia Preferencial", Propriedade = "DiaPreferencial", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Dias Bloqueio", Propriedade = "DiasBloqueio", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Raiz Grupo Fornecedor", Propriedade = "RaizGrupoFornecedor", Tamanho = tamanho, Obrigatorio = false });

            return configuracoes;
        }

        private dynamic ObterRegistro(Dictionary<string, dynamic> dados)
        {
            double CNPJFornecedor = 0;

            if (dados.TryGetValue("CNPJFornecedor", out dynamic cnpj))
                CNPJFornecedor = cnpj != null ? ((string)cnpj).ToString().ObterSomenteNumeros().ToDouble() : 0;

            string raizGrupoFornecedor = string.Empty;

            if (dados.TryGetValue("RaizGrupoFornecedor", out dynamic dynRaizGrupoFornecedor))
                raizGrupoFornecedor = !string.IsNullOrWhiteSpace(dynRaizGrupoFornecedor) ? ((string)dynRaizGrupoFornecedor).ToString().ObterSomenteNumeros() : "";

            string categoria = string.Empty;

            if (dados.TryGetValue("Categoria", out dynamic descricaoCategoria))
                categoria = ((string)descricaoCategoria);

            double destinatarioCentroDescarregamento = 0;

            if (dados.TryGetValue("CentroDescarregamento", out dynamic dynCentroDescarregamento))
                destinatarioCentroDescarregamento = ((string)dynCentroDescarregamento).ToString().ObterSomenteNumeros().ToDouble();

            int diaPreferencial = 0;

            if (dados.TryGetValue("DiaPreferencial", out dynamic dynDiaPreferencial))
                diaPreferencial = ((string)dynDiaPreferencial).ToInt();

            int diasBloqueio = 0;

            if (dados.TryGetValue("DiasBloqueio", out dynamic dynDiasBloqueio))
                diasBloqueio = ((string)dynDiasBloqueio).ToInt();

            return new
            {
                CNPJFornecedor = CNPJFornecedor,
                Categoria = categoria,
                DestinatarioCentroDescarregamento = destinatarioCentroDescarregamento,
                DiaPreferencial = diaPreferencial,
                DiasBloqueio = diasBloqueio,
                RaizGrupoFornecedor = raizGrupoFornecedor
            };
        }

        public RetornoImportacao ImportarRegistro(dynamic linhas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DataPreferencialDescarga repositorioDataPreferencialDescarga = new Repositorio.Embarcador.Logistica.DataPreferencialDescarga(unitOfWork);
            Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorioDataPreferencialDescargaFornecedor = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoFornecedor = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            RetornoImportacao retornoImportacao = new RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<RetonoLinha>();
            int contador = 0;

            List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga> listaDatasPreferenciaisExistentes = new List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga>();

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    dynamic linha = linhas[i];
                    string retorno = "";

                    if (linha.DestinatarioCentroDescarregamento <= 0 || linha.DiaPreferencial <= 0 || string.IsNullOrWhiteSpace(linha.Categoria))
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Registro ignorado", i));
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga dataPreferencialDescarga = listaDatasPreferenciaisExistentes.Where(obj => obj.CentroDescarregamento.Destinatario.CPF_CNPJ == linha.DestinatarioCentroDescarregamento && obj.DiasBloqueio == linha.DiasBloqueio && obj.DiaPreferencial == linha.DiaPreferencial)?.FirstOrDefault() ?? repositorioDataPreferencialDescarga.BuscarPorCentroDescarregamentoDiasBloqueioDiaPreferencial(linha.DestinatarioCentroDescarregamento, linha.DiasBloqueio, linha.DiaPreferencial);
                    Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria fornecedorCategoria = new Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria();
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;
                    Dominio.Entidades.Cliente fornecedor = null;
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoFornecedor = null;

                    if (dataPreferencialDescarga == null)
                        dataPreferencialDescarga = new Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga();
                    else if (!listaDatasPreferenciaisExistentes.Contains(dataPreferencialDescarga))
                        listaDatasPreferenciaisExistentes.Add(dataPreferencialDescarga);

                    dataPreferencialDescarga.CentroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatario(linha.DestinatarioCentroDescarregamento);

                    if (dataPreferencialDescarga.CentroDescarregamento == null)
                        retorno = "Centro de Descarregamento não encontrado.";

                    dataPreferencialDescarga.DiaPreferencial = linha.DiaPreferencial;
                    dataPreferencialDescarga.DiasBloqueio = linha.DiasBloqueio;

                    double cpfCnpjFornecedor = linha.CNPJFornecedor;
                    string raizGrupoFornecedor = linha.RaizGrupoFornecedor;

                    produtoEmbarcador = repositorioProdutoEmbarcador.BuscarPorDescricao(linha.Categoria);

                    if (cpfCnpjFornecedor > 0)
                        fornecedor = repositorioCliente.BuscarPorCPFCNPJ(linha.CNPJFornecedor);
                    else
                        grupoFornecedor = repositorioGrupoFornecedor.BuscarPorRaizCNPJ(raizGrupoFornecedor);

                    if (produtoEmbarcador == null)
                        retorno = "Produto não encontrado.";

                    if (fornecedor == null && grupoFornecedor == null)
                        retorno = "Fornecedor/Grupo do Fornecedor não encontrado.";

                    fornecedorCategoria = string.IsNullOrWhiteSpace(retorno) ? SalvarFornecedorCategoria(dataPreferencialDescarga, produtoEmbarcador, fornecedor, grupoFornecedor, unitOfWork) : null;

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                    }
                    else
                    {
                        if (dataPreferencialDescarga.Codigo > 0)
                            repositorioDataPreferencialDescarga.Atualizar(dataPreferencialDescarga);
                        else
                        {
                            repositorioDataPreferencialDescarga.Inserir(dataPreferencialDescarga);
                            listaDatasPreferenciaisExistentes.Add(dataPreferencialDescarga);
                        }

                        repositorioDataPreferencialDescargaFornecedor.Inserir(fornecedorCategoria);

                        contador++;
                        RetonoLinha retornoLinha = new RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, dataPreferencialDescarga, "Registro salvo via importação de planilha.", unitOfWork);

                        unitOfWork.CommitChanges();
                    }

                }
                catch (ControllerException excecao)
                {
                    unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(excecao.Message, i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        private RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            RetonoLinha retorno = new RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion
    }
}
