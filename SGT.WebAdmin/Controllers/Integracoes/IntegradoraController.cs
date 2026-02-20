using Dominio.Entidades.WebService;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/Integradora")]
    public class IntegradoraController : BaseController
    {
        #region Construtores

        public IntegradoraController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora filtrosPesquisa = ObterFiltrosPesquisa();
                Grid grid = ObterGrid(filtrosPesquisa);

                int totalRegistros = await repositorioIntegradora.ContarConsultaAsync(filtrosPesquisa);

                if (totalRegistros == 0)
                {
                    grid.AdicionaRows(new List<dynamic>() { });

                    return new JsonpResult(grid);
                }

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta();
                List<Integradora> listaIntegradora = await repositorioIntegradora.ConsultarAsync(filtrosPesquisa, parametroConsulta);

                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (
                    from p in listaIntegradora
                    select new
                    {
                        p.Codigo,
                        p.Descricao,
                        p.Token,
                        p.DescricaoAtivo,
                    }
                ).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);

                Integradora integradora = new Integradora();

                await PreencherIntegradoraAsync(integradora, unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                await repIntegradora.InserirAsync(integradora, Auditado);

                await SetarClientesAsync(integradora, unitOfWork, cancellationToken);
                await SetarPermissoesWebServiceAsync(integradora, unitOfWork, cancellationToken);
                await SetarIntegracoesAsync(integradora, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, excecao.Message);
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
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);

                int codigoIntegradora = Request.GetIntParam("Codigo");

                Integradora integradora = await repIntegradora.BuscarPorCodigoAsync(codigoIntegradora, true);

                await PreencherIntegradoraAsync(integradora, unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                await repIntegradora.AtualizarAsync(integradora, Auditado);

                await SetarClientesAsync(integradora, unitOfWork, cancellationToken);
                await SetarPermissoesWebServiceAsync(integradora, unitOfWork, cancellationToken);
                await SetarIntegracoesAsync(integradora, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);

                int codigo = int.Parse(Request.Params("Codigo"));

                Integradora integradora = await repIntegradora.BuscarPorCodigoAsync(codigo, auditavel: false);

                List<object> integracoes = await ObterListaIntegracoesAsync(integradora, unitOfWork, cancellationToken);

                var dynIntegradora = new
                {
                    integradora.Codigo,
                    integradora.Token,
                    integradora.Descricao,
                    integradora.Ativo,
                    integradora.TipoAutenticacao,
                    integradora.Usuario,
                    integradora.Senha,
                    integradora.TempoExpiracao,
                    integradora.TodosWebServicesLiberados,
                    ListaPermissoesWebService = ObterListaPermissoesWebServices(integradora),
                    Integracoes = integracoes,
                    Empresa = new { Codigo = integradora.Empresa?.Codigo ?? 0, Descricao = integradora.Empresa?.Descricao ?? "" },
                    GrupoPessoas = new { Codigo = integradora.GrupoPessoas?.Codigo ?? 0, Descricao = integradora.GrupoPessoas?.Descricao ?? "" },
                    Cliente = new { Codigo = integradora.Cliente?.CPF_CNPJ ?? 0, Descricao = integradora.Cliente?.Descricao ?? "" },
                    Clientes = (from obj in integradora.Clientes
                                select new
                                {
                                    obj.CPF_CNPJ,
                                    Codigo = obj.CPF_CNPJ_SemFormato,
                                    obj.Descricao,
                                    obj.Latitude,
                                    obj.Longitude,
                                    obj.Endereco,
                                    obj.Numero,
                                    Localidade = obj.Localidade.DescricaoCidadeEstado,
                                    obj.CEP,
                                    obj.Localidade.CodigoIBGE
                                }).ToList(),
                };

                return new JsonpResult(dynIntegradora);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);
                Repositorio.WebService.PermissaoWebservice repositorioPermissaoWebservice = new Repositorio.WebService.PermissaoWebservice(unitOfWork, cancellationToken);
                Repositorio.WebService.IntegradoraIntegracao repositoriointegradoraIntegracao = new Repositorio.WebService.IntegradoraIntegracao(unitOfWork, cancellationToken);

                Integradora integradora = await repositorioIntegradora.BuscarPorCodigoAsync(codigo, auditavel: false);

                await unitOfWork.StartAsync(cancellationToken);

                foreach (PermissaoWebservice permissao in integradora.PermissoesWebservice)
                    await repositorioPermissaoWebservice.DeletarAsync(permissao);

                List<IntegradoraIntegracao> integracoes = await repositoriointegradoraIntegracao.BuscarPorIntegradoraAsync(integradora.Codigo);

                foreach (IntegradoraIntegracao integracao in integracoes)
                    await repositoriointegradoraIntegracao.DeletarAsync(integracao);

                unitOfWork.Flush();
                integradora.Clientes.Clear();

                await repositorioIntegradora.DeletarAsync(integradora, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarIntegracoes()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracaos = await repositorioTipoIntegracao.BuscarTodosAtivosAsync();

                var retornoIntegracoes = (
                        from obj in tipoIntegracaos
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();

                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private async Task SetarPermissoesWebServiceAsync(Integradora integradora, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.WebService.PermissaoWebservice repPermissaoWebservice = new Repositorio.WebService.PermissaoWebservice(unitOfWork, cancellationToken);
            List<PermissaoWebservice> TodasPermissoes = await repPermissaoWebservice.BuscarPorIntegradoraAsync(integradora.Codigo);

            List<int> Codigos = new List<int>();

            dynamic dynPermissoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaPermissoesWebService"));

            if (dynPermissoes.Count > 0)
            {
                foreach (var p in dynPermissoes)
                {
                    int.TryParse((string)p.Codigo, out int codigo);
                    string nomeMetodo = p.NomeMetodo;
                    int.TryParse((string)p.QtdRequisicoes, out int qtdRequisicoes);
                    int.TryParse((string)p.RequisicoesMinuto, out int requisicoesMinuto);
                    DateTime.TryParse((string)p.UltimoReset, out DateTime ultimoReset);

                    if (codigo > 0)
                    {
                        Codigos.Add(codigo);

                        PermissaoWebservice per = TodasPermissoes.Find(x => x.Codigo == codigo);

                        per.Integradora = integradora;
                        per.NomeMetodo = nomeMetodo;
                        per.QtdRequisicoes = qtdRequisicoes;
                        per.RequisicoesMinuto = requisicoesMinuto;
                        per.UltimoReset = ultimoReset;

                        await repPermissaoWebservice.AtualizarAsync(per);

                        continue;
                    }

                    PermissaoWebservice permissao = new PermissaoWebservice()
                    {
                        Integradora = integradora,
                        Codigo = codigo,
                        NomeMetodo = nomeMetodo,
                        QtdRequisicoes = qtdRequisicoes,
                        RequisicoesMinuto = requisicoesMinuto,
                        UltimoReset = ultimoReset,
                    };

                    await repPermissaoWebservice.InserirAsync(permissao);
                }
            }

            foreach (PermissaoWebservice permissao in TodasPermissoes)
                if (!Codigos.Exists(x => x == permissao.Codigo))
                    await repPermissaoWebservice.DeletarAsync(permissao);
        }

        private async Task SetarClientesAsync(Integradora integradora, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Cliente repCliente = new Cliente(unitOfWork, cancellationToken);

            dynamic clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Clientes"));

            if (integradora.Clientes == null)
                integradora.Clientes = new List<Dominio.Entidades.Cliente>();
            else
                integradora.Clientes.Clear();

            foreach (dynamic cliente in clientes)
                integradora.Clientes.Add(await repCliente.BuscarPorCPFCNPJAsync((double)cliente.Codigo));
        }

        private dynamic ObterListaPermissoesWebServices(Integradora Integradora)
        {
            return (from obj in Integradora.PermissoesWebservice
                    where obj.Integradora.Codigo == Integradora.Codigo
                    select new
                    {
                        Codigo = obj?.Codigo ?? 0,
                        NomeMetodo = obj?.NomeMetodo ?? string.Empty,
                        RequisicoesMinuto = obj?.RequisicoesMinuto ?? 0,
                        QtdRequisicoes = obj?.QtdRequisicoes ?? 0,
                        UltimoReset = obj?.UltimoReset.ToString("dd/MM/yyyy") ?? string.Empty,
                    }).ToList();
        }

        private async Task<List<dynamic>> ObterListaIntegracoesAsync(Integradora integradora, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.WebService.IntegradoraIntegracao repositorioIntegradoraIntegracao = new Repositorio.WebService.IntegradoraIntegracao(unidadeDeTrabalho, cancellationToken);

            List<IntegradoraIntegracao> integracoes = await repositorioIntegradoraIntegracao.BuscarPorIntegradoraAsync(integradora.Codigo);

            if (integracoes == null || !integracoes.Any())
                return new List<dynamic>();

            return integracoes.Select(o => new
            {
                o.Codigo,
                o.Tipo,
                Descricao = o.Tipo.ObterDescricao()
            }).ToDynamicList();
        }

        private async Task PreencherIntegradoraAsync(Integradora integradora, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
            Cliente repositorioCliente = new Cliente(unitOfWork, cancellationToken);

            TipoAutenticacao tipoAutenticacao = Request.GetEnumParam("TipoAutenticacao", TipoAutenticacao.Token);
            string token = Request.GetStringParam("Token");
            string usuario = Request.GetStringParam("Usuario");
            string senha = Request.GetStringParam("Senha");
            int tempoExpiracao = Request.GetIntParam("TempoExpiracao");
            int codigoEmpresa = Request.GetIntParam("Empresa");
            int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");
            double codigoCliente = Request.GetDoubleParam("Cliente");
            bool autenticacaoUsuarioSenha = tipoAutenticacao == TipoAutenticacao.UsuarioESenha;

            if (autenticacaoUsuarioSenha && (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha)))
                throw new ControllerException("Necessário informar o usuário e senha.");

            if (autenticacaoUsuarioSenha && tempoExpiracao == 0)
                throw new ControllerException("Necessário informar o tempo de expiração.");

            integradora.TodosWebServicesLiberados = Request.GetBoolParam("TodosWebServicesLiberados");
            integradora.Ativo = Request.GetBoolParam("Ativo");
            integradora.Descricao = Request.GetStringParam("Descricao");
            integradora.TipoAutenticacao = tipoAutenticacao;
            integradora.Token = token;
            integradora.Usuario = autenticacaoUsuarioSenha ? usuario : string.Empty;
            integradora.Senha = autenticacaoUsuarioSenha ? senha : string.Empty;
            integradora.TempoExpiracao = autenticacaoUsuarioSenha ? tempoExpiracao : 0;
            integradora.Empresa = codigoEmpresa > 0 ? await repositorioEmpresa.BuscarPorCodigoAsync(codigoEmpresa) : null;
            integradora.GrupoPessoas = codigoGrupoPessoas > 0 ? await repositorioGrupoPessoas.BuscarPorCodigoAsync(codigoGrupoPessoas) : null;
            integradora.Cliente = codigoCliente > 0 ? await repositorioCliente.BuscarPorCPFCNPJAsync(codigoCliente) : null;

            if (!autenticacaoUsuarioSenha && string.IsNullOrWhiteSpace(integradora.Token))
                integradora.Token = Guid.NewGuid().ToString().Replace("-", "");
        }

        private async Task SetarIntegracoesAsync(Integradora integradora, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            dynamic integracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Integracoes"));

            await ExcluirIntegracoesRemovidasAsync(integradora, integracoes, unidadeDeTrabalho, cancellationToken);
            await SalvarIntegracoesAdicionadasAsync(integradora, integracoes, unidadeDeTrabalho, cancellationToken);
        }

        private async Task ExcluirIntegracoesRemovidasAsync(Integradora integradora, IEnumerable<dynamic> integracoes, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.WebService.IntegradoraIntegracao repositorioIntegradoraIntegracao = new Repositorio.WebService.IntegradoraIntegracao(unidadeDeTrabalho, cancellationToken);

            List<IntegradoraIntegracao> integracoesExistentes = await repositorioIntegradoraIntegracao.BuscarPorIntegradoraAsync(integradora.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (integracoesExistentes == null || !integracoesExistentes.Any())
                return;

            List<int> codigosAtualizados = integracoes
                .Select(i => ((string)i.Codigo).ToNullableInt())
                .Where(codigo => codigo.HasValue)
                .Select(codigo => codigo.Value)
                .ToList();

            List<IntegradoraIntegracao> integracoesParaRemover = integracoesExistentes
                .Where(integracao => !codigosAtualizados.Contains(integracao.Codigo))
                .ToList();

            if (!integracoesParaRemover.Any())
                return;

            foreach (IntegradoraIntegracao integracao in integracoesParaRemover)
            {
                await repositorioIntegradoraIntegracao.DeletarAsync(integracao);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "IntegradoraIntegracao",
                    De = $"{(int)integracao.Tipo} - {integracao.Tipo.ObterDescricao()}",
                    Para = ""
                });
            }

            string descricaoAcao = integracoesParaRemover.Count == 1
                ? Localization.Resources.Integracoes.Integradora.IntegracaoRemovida
                : Localization.Resources.Integracoes.Integradora.MultiplasIntegracaesRemovidas;

            integradora.SetExternalChanges(alteracoes);
            integradora.SetChanges();

            await Servicos.Auditoria.Auditoria.AuditarAsync(
                Auditado,
                integradora,
                integradora.GetChanges(),
                descricaoAcao,
                unidadeDeTrabalho,
                Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro,
                cancellationToken
            );
        }

        private async Task SalvarIntegracoesAdicionadasAsync(Integradora integradora, IEnumerable<dynamic> integracoes, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!integracoes.Any())
                return;

            Repositorio.WebService.IntegradoraIntegracao repositorioIntegradoraIntegracao = new Repositorio.WebService.IntegradoraIntegracao(unidadeDeTrabalho, cancellationToken);

            List<int> listaCodigosAdicionar = new List<int>();
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            foreach (dynamic integracao in integracoes)
            {
                int? codigoIntegracao = ((string)integracao.Codigo).ToNullableInt();

                if (!codigoIntegracao.HasValue)
                {
                    IntegradoraIntegracao integradoraIntegracao = new IntegradoraIntegracao()
                    {
                        Tipo = ((string)integracao.Tipo).ToEnum<TipoIntegracao>(),
                        Integradora = integradora
                    };

                    await repositorioIntegradoraIntegracao.InserirAsync(integradoraIntegracao);
                    listaCodigosAdicionar.Add(integradoraIntegracao.Codigo);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "IntegradoraIntegracao",
                        De = "",
                        Para = $"{(int)integradoraIntegracao.Tipo} - {integradoraIntegracao.Tipo.ObterDescricao()}"
                    });
                }
            }

            if (integradora.IsInitialized() && (listaCodigosAdicionar.Count > 0))
            {
                string descricaoAcao = listaCodigosAdicionar.Count == 1 ? Localization.Resources.Integracoes.Integradora.IntegracaoAdicionada : Localization.Resources.Integracoes.Integradora.MultiplasIntegracoesAdicionadas;

                integradora.SetExternalChanges(alteracoes);
                integradora.SetChanges();

                await Servicos.Auditoria.Auditoria.AuditarAsync(
                    Auditado,
                    integradora,
                    integradora.GetChanges(),
                    descricaoAcao,
                    unidadeDeTrabalho,
                    Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro,
                    cancellationToken
                );
            }
        }

        private Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        private Grid ObterGrid(Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora filtrosPesquisa)
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            bool ocultarColunaToken = Request.GetBoolParam("OcultarColunaToken");

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Align.left, true);

            if (!ocultarColunaToken)
                grid.AdicionarCabecalho(Localization.Resources.Enumeradores.TipoAutenticacao.Token, "Token", 40, Align.left, false);

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Align.center, false);

            return grid;
        }

        #endregion
    }
}
