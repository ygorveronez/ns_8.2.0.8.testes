using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Models.Grid;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using AdminMultisoftware.Dominio.Enumeradores;

namespace SGT.WebAdmin.Controllers.SolicitacaoTokenes
{
    [CustomAuthorize("Transportadores/SolicitacaoToken")]
    public class SolicitacaoTokenController : BaseController
    {
		#region Construtores

		public SolicitacaoTokenController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa = ObterFiltrosPesquisa();

                Grid grid = ObterGrid();

                Repositorio.Embarcador.Transportadores.SolicitacaoToken repSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = repSolicitacaoToken.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> listaSolicitacoes = totalRegistros > 0 ? repSolicitacaoToken.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>();

                dynamic listaSolicitacoesRetornar = (
                    from solicitacao in listaSolicitacoes
                    select new
                    {
                        solicitacao.Codigo,
                        solicitacao.Descricao,
                        solicitacao.NumeroProtocolo,
                        Situacao = solicitacao.Situacao.ObterDescricao(),
                        TipoAutenticacao = solicitacao.TipoAutenticacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaSolicitacoesRetornar);
                grid.setarQuantidadeTotal(totalRegistros);
                return new JsonpResult(grid);

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

        private Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número do Protocolo", "NumeroProtocolo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo Autenticação", "TipoAutenticacao", 10, Models.Grid.Align.center, false);
            return grid;
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Transportadores.SolicitacaoToken repSolicitacao = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);

                Servicos.Embarcador.Transportadores.SolicitacaoToken solicitacaoService = new Servicos.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);

                Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken = new Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken();

                unitOfWork.Start();

                PreencherSolicitacaoToken(solicitacaoToken, repSolicitacao);
                PreencherTransportadores(solicitacaoToken, unitOfWork);
                PreencherPermissoesWS(solicitacaoToken, unitOfWork);

                repSolicitacao.Inserir(solicitacaoToken);
                solicitacaoService.EnviarEmailsAsyncAguardandoAprovacao(solicitacaoToken, Cliente.Email);

                new Servicos.Embarcador.Transportadores.AutorizacaoToken(unitOfWork).CriarAprovacao(solicitacaoToken, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Solicitação Adicionada com Sucesso!");
            }

            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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

                Repositorio.Embarcador.Transportadores.SolicitacaoToken repSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken = repSolicitacaoToken.BuscarPorCodigo(codigo, false);


                var solicitacaoRetorno = new
                {
                    solicitacaoToken.Codigo,
                    solicitacaoToken.Descricao,
                    solicitacaoToken.NumeroProtocolo,
                    DataInicioVigencia = solicitacaoToken.DataInicioVigencia.ToDateString(),
                    DataFimVigencia = solicitacaoToken.DataFimVigencia.ToDateString(),
                    solicitacaoToken.TipoAutenticacao,
                    solicitacaoToken.Observacao,
                    solicitacaoToken.Situacao,
                    PossuiRegras = (repositorioRegraAutorizacao.BuscarPorAtiva()?.Count ?? 0) > 0,
                    DescricaoSituacao = solicitacaoToken.Situacao.ObterDescricao(),

                    Transportadores = (from tr in solicitacaoToken.Transportadores
                                       select new
                                       {
                                           tr.Codigo,
                                           tr.Descricao,
                                           CNPJ = tr.CNPJ_Formatado
                                       }).ToList(),
                    PermissoesWS = (from permissao in solicitacaoToken.PermissoesWS
                                    select new
                                    {
                                        permissao.Codigo,
                                        Metodo = permissao.NomeMetodo,
                                        CodigoMetodo = permissao.Codigo,
                                        permissao.RequisicoesMinuto,
                                        permissao.QuantidadeRequisicoes
                                    }).ToList(),

                    ResumoAprovacao = ObterResumoAprovacao(solicitacaoToken, unitOfWork),

                };

                return new JsonpResult(solicitacaoRetorno);
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

        public async Task<IActionResult> ObterTokensGeradosPelaSolicitacao()
        {
         Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSolicitacao = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Numero Protocolo", "NumeroProtocolo", 15, Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Align.left, false);
                grid.AdicionarCabecalho("CNPJ", "CNPJ", 15, Align.left, false);
                grid.AdicionarCabecalho("Token", "Token", 15, Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 15, Align.left, false);


                Repositorio.Embarcador.Transportadores.SolicitacaoTokenTransportador repositorioTokensTransportador = new Repositorio.Embarcador.Transportadores.SolicitacaoTokenTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador> tokensTransportador = repositorioTokensTransportador.Consultar(codigoSolicitacao, grid.ObterParametrosConsulta());
                var retorno = (from obj in tokensTransportador
                               select new
                               {

                                   obj.Codigo,
                                   NumeroProtocolo = obj.SolicitacaoToken.NumeroProtocolo,
                                   Transportador = obj?.Transportador?.RazaoSocial ?? string.Empty,
                                   CNPJ = obj.Transportador.CNPJ,
                                   Token = obj.Token != null ? obj.Token : $"{obj.Usuario.Login} - {obj.Usuario.Senha}",
                                   Status = obj.Situacao ? "Gerado com Sucesso" : "Falha ao gerar o Token",
                               }).ToList();


                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(repositorioTokensTransportador.ContarConsulta(codigoSolicitacao));
                return new JsonpResult(grid);
            }
            catch (BaseException exe)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, exe.Message);
            }
            catch (Exception exe)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repAprovacao = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(unitOfWork);

                int codAjuste = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken> listaAutorizacao = repAprovacao.ConsultarAutorizacoes(codAjuste, grid.ObterParametrosConsulta());
                int totalRegistros = repAprovacao.ContarAutorizacoes(codAjuste);

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario.Nome ?? "",
                                 Regra = obj.RegraAutorizacao?.Descricao ?? string.Empty,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
                             }).ToList();


                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repositorioAutorizacao = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken autorizacao = repositorioAutorizacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
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


        #endregion

        #region Métodos Privados

        private dynamic ObterResumoAprovacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken Solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repositorioAprovacao = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(Solicitacao.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(Solicitacao.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(Solicitacao.Codigo);

            return new
            {
                Codigo = Solicitacao.Codigo,
                DataSolicitacao = Solicitacao.DataInicioVigencia.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                PossuiRegras = (Solicitacao.Situacao != EtapaAutorizacaoToken.SemRegraAprovacao),
                Reprovacoes = reprovacoes
            };
        }

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private void PreencherSolicitacaoToken(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao, Repositorio.Embarcador.Transportadores.SolicitacaoToken repSolicitacao)
        {
            solicitacao.NumeroProtocolo = repSolicitacao.ObterUltimoNumeroProtocolo() + 1;
            solicitacao.Descricao = Request.GetStringParam("Descricao");
            solicitacao.DataInicioVigencia = Request.GetDateTimeParam("DataInicioVigencia");
            solicitacao.DataFimVigencia = Request.GetDateTimeParam("DataFimVigencia");
            solicitacao.TipoAutenticacao = Request.GetEnumParam<TipoAutenticacao>("TipoAutenticacao");
            solicitacao.TempoExpiracao = Request.GetIntParam("TempoExpiracao");

            if (solicitacao.TipoAutenticacao == TipoAutenticacao.UsuarioESenha  && solicitacao.TempoExpiracao == 0)
                throw new ControllerException("Necessário informar o tempo de expiração.");

            // se for criado um método de atualizar, por favor cuidar disso!!
            solicitacao.DataCriacao = DateTime.Now;
        }

        private void PreencherTransportadores(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            List<int> codigosTransportadores = Request.GetListParam<int>("Transportadores");

            solicitacao.Transportadores = repEmpresa.BuscarPorCodigos(codigosTransportadores);
        }

        private void PreencherPermissoesWS(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken repPermissaoWSSolicitacao = new Repositorio.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken> permissoesFront = new List<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken>();

            dynamic dynPermissoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PermissoesWebService"));

            foreach (dynamic dynPermissao in dynPermissoes)
            {
                var novaPermissao = new Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken
                {
                    NomeMetodo = (string)dynPermissao?.Metodo,
                    QuantidadeRequisicoes = int.Parse((string)dynPermissao?.QuantidadeRequisicoes),
                    RequisicoesMinuto = int.Parse((string)dynPermissao?.RequisicoesMinuto),
                };

                permissoesFront.Add(novaPermissao);
            }
                
            //DeletarPermissoesRemovidas(solicitacao, repPermissaoWSSolicitacao, permissoesFront);

            foreach (var permissaoAdicionar in permissoesFront)
            {
                repPermissaoWSSolicitacao.Inserir(permissaoAdicionar);
            }

            solicitacao.PermissoesWS = permissoesFront;
        }

        private void DeletarPermissoesRemovidas(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao, Repositorio.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken repPermissaoWSSolicitacao, List<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken> permissoesFront)
        {
            List<int> codigosPermissoesFront = permissoesFront.Select(x => x.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken> permissoesDeletar = solicitacao?.PermissoesWS?.Where(x => !codigosPermissoesFront.Contains(x.Codigo)).ToList() ?? new List<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken>();
            foreach (var permissaoDeletar in permissoesDeletar)
            {
                solicitacao?.PermissoesWS.Remove(permissaoDeletar);
                repPermissaoWSSolicitacao.Deletar(permissaoDeletar);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken()
            {
                NumeroProtocolo = Request.GetIntParam("NumeroProtocolo"),
                Descricao = Request.GetStringParam("Descricao"),
                Prioridade = 99
            };

            if(this.TipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe)
            {
                filtro.CodigoEmpresa = Usuario.Empresa.Codigo;
            }

            return filtro;
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }

        #endregion
    }
}
