using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ClienteBuscaAutomatica")]
    public class ClienteBuscaAutomaticaController : BaseController
    {
        #region Construtores
        public ClienteBuscaAutomaticaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]

        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<Models.Grid.Grid> ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteBuscaAutomatica filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cidade de Origem", "Origem", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repBuscaCliente = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica> clienteBuscas = await repBuscaCliente.ConsultarAsync(filtrosPesquisa, parametrosConsulta);
                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ClienteBuscaAutomatica/Pesquisa", "grid-pesquisa-clientes");

                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                grid.setarQuantidadeTotal(await repBuscaCliente.ContarConsultaAsync(filtrosPesquisa));

                var lista = (from obj in clienteBuscas
                             select new
                             {
                                 obj.Codigo,
                                 Filial = obj.Filial?.Descricao ?? string.Empty,
                                 Descricao = obj?.Descricao ?? string.Empty,
                                 Remetente = obj.Remetente?.Nome ?? string.Empty,
                                 Destinatario = obj.Destinatario?.Nome ?? string.Empty,
                                 Cliente = obj.Cliente?.Nome ?? string.Empty,
                                 Origem = obj.Origem?.DescricaoCidadeEstado ?? string.Empty,
                                 Situacao = obj.Situacao.ObterDescricao() ?? string.Empty,
                             }).ToList();

                grid.AdicionaRows(lista);
                return grid;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repBuscaCliente = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica clienteBusca = await repBuscaCliente.BuscarPorCodigoAsync(codigo, false);
                if (clienteBusca == null)
                    return new JsonpResult(false, true, "Não foi possível localizar o registro");

                var dynClienteBusca = new
                {
                    clienteBusca.Codigo,
                    Filial = clienteBusca.Filial != null ? new { clienteBusca.Filial.Codigo, clienteBusca.Filial.Descricao } : null,
                    Descricao = clienteBusca?.Descricao ?? string.Empty,
                    Remetente = clienteBusca.Remetente != null ? new { clienteBusca.Remetente.Codigo, clienteBusca.Remetente.Descricao } : null,
                    Destinatario = clienteBusca.Destinatario != null ? new { clienteBusca.Destinatario.Codigo, clienteBusca.Destinatario.Descricao } : null,
                    Cliente = clienteBusca.Cliente != null ? new { clienteBusca.Cliente.Codigo, clienteBusca.Cliente.Descricao } : null,
                    Origem = clienteBusca.Origem != null ? new { clienteBusca.Origem.Codigo, Descricao = clienteBusca.Origem.DescricaoCidadeEstado } : null,
                    Situacao = (int)clienteBusca.Situacao == 1 ? true : false,
                    clienteBusca.TipoParticipante,
                };

                return new JsonpResult(dynClienteBusca);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica clienteBuscaAutomatica = new Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica();
                Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repClienteBuscaAutomatica = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);

                PreencherBuscaAutomatica(clienteBuscaAutomatica, unitOfWork);

                await unitOfWork.StartAsync(cancellationToken);

                await repClienteBuscaAutomatica.InserirAsync(clienteBuscaAutomatica, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repBuscaCliente = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);
                var clienteBusca = await repBuscaCliente.BuscarPorCodigoAsync(codigo, true);

                if (clienteBusca == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                PreencherBuscaAutomatica(clienteBusca, unitOfWork);

                await unitOfWork.StartAsync(cancellationToken);

                await repBuscaCliente.AtualizarAsync(clienteBusca, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Excluir(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repBuscaCliente = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);
                var clienteBusca = await repBuscaCliente.BuscarPorCodigoAsync(codigo, true);

                if (clienteBusca == null)
                    return new JsonpResult(false, "Registro não encontrado.");


                await unitOfWork.StartAsync(cancellationToken);

                await repBuscaCliente.DeletarAsync(clienteBusca, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }


        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "*CNPJ Remetente", Propriedade = "CNPJRementente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ Destinatário", Propriedade = "CNPJDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Cidade Origem", Propriedade = "CidadeOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "UF Origem", Propriedade = "UFOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Situação", Propriedade = "Situacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Cliente", Propriedade = "Cliente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Filial", Propriedade = "Filial", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Tipo de Participante", Propriedade = "TipoParticipante", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });


            return new JsonpResult(configuracoes.ToList());
        }
        public async Task<IActionResult> Importar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                Servicos.Embarcador.Pessoa.ClienteBuscaAutomatica servicoClienteBuscaAutomatica = new Servicos.Embarcador.Pessoa.ClienteBuscaAutomatica(unitOfWork, Auditado, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = await servicoClienteBuscaAutomatica.ImportarAsync(linhas);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion


        private void PreencherBuscaAutomatica(Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica clienteBusca, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalicade = new Repositorio.Localidade(unitOfWork);

            int.TryParse(Request.Params("Origem"), out int codigoCidadeOrigem);
            int.TryParse(Request.Params("Filial"), out int codigoFilial);
            double.TryParse(Request.Params("Remetente"), out double codigoRemetente);
            double.TryParse(Request.Params("Destinatario"), out double codigoDestinatario);
            double.TryParse(Request.Params("Cliente"), out double codigoCliente);


            clienteBusca.Descricao = Request.GetStringParam("Descricao");
            clienteBusca.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
            clienteBusca.Remetente = codigoRemetente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRemetente) : null;
            clienteBusca.Destinatario = codigoDestinatario > 0 ? repCliente.BuscarPorCPFCNPJ(codigoDestinatario) : null;
            clienteBusca.Cliente = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;
            clienteBusca.Origem = codigoCidadeOrigem > 0 ? repLocalicade.BuscarPorCodigo(codigoCidadeOrigem) : null;
            clienteBusca.Situacao = Request.GetBoolParam("Situacao") ? SituacaoAtivoPesquisa.Ativo : SituacaoAtivoPesquisa.Inativo;
            clienteBusca.TipoParticipante = Request.GetEnumParam<TipoParticipante>("TipoParticipante");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteBuscaAutomatica ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteBuscaAutomatica filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteBuscaAutomatica()
            {
                CodigoCliente = Request.GetDoubleParam("Cliente"),
                CodigoDestinatario = Request.GetDoubleParam("Destinatario"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoRemetente = Request.GetDoubleParam("Remetente"),
                CodigoLocalidadeOrigem = Request.GetIntParam("Origem"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Situacao"),
            };

            return filtrosPesquisa;
        }
    }
}
