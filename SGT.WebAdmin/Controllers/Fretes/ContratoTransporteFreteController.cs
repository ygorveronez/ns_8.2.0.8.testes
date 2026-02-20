
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ContratoTransporteFrete", "Fretes/ContratoTransporteFreteAnexo", "Fretes/ContratoTransportadorFrete")]
    public class ContratoTransporteFreteController : BaseController
    {
		#region Construtores

		public ContratoTransporteFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfigContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = new Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configContratoFreteTerceiro = repConfigContratoFreteTerceiro.BuscarConfiguracaoPadrao();

                PreencherEntidade(contratoTransporteFrete, unitOfWork);

                if (configContratoFreteTerceiro?.GerarNumeroContratoTransportadorFreteSequencial ?? false)
                {
                    contratoTransporteFrete.NumeroContratoSequencial = repContratoTransporteFrete.BuscarProximoNumero();
                    contratoTransporteFrete.NumeroContrato = contratoTransporteFrete.NumeroContratoSequencial;
                }

                repContratoTransporteFrete.Inserir(contratoTransporteFrete, Auditado);
                ClonarContrato(contratoTransporteFrete, unitOfWork, repContratoTransporteFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(contratoTransporteFrete.Codigo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repContratoTransporteFrete.BuscarPorCodigo(codigo);

                if (contratoTransporteFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                ClonarContrato(contratoTransporteFrete, unitOfWork, repContratoTransporteFrete);
                PreencherEntidade(contratoTransporteFrete, unitOfWork);

                if (contratoTransporteFrete.Ativo)
                    contratoTransporteFrete.StatusAprovacaoTransportador = StatusAprovacaoTransportador.AguardandoAprovacao;

                repContratoTransporteFrete.Atualizar(contratoTransporteFrete, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(contratoTransporteFrete.Codigo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repContratoTransporteFrete.BuscarPorCodigo(codigo);

                if (contratoTransporteFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repContratoTransporteFrete.Deletar(contratoTransporteFrete, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repContratoTransporteFrete.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoIntegracao = repContratoTransporteFreteIntegracao.BuscarIntegracaoPorCodigoContrato(codigo);

                if (contratoTransporteFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bool permiteEdicao = (
                    (contratoTransporteFrete.StatusAprovacaoTransportador != StatusAprovacaoTransportador.AguardandoAprovacao) &&
                    (contratoIntegracao?.SituacaoIntegracao != SituacaoIntegracao.AgIntegracao) &&
                    (
                        (contratoTransporteFrete.StatusAssinaturaContrato == null) ||
                        (contratoTransporteFrete.StatusAssinaturaContrato.CodigoIntegracao == "A") ||
                        (contratoTransporteFrete.StatusAssinaturaContrato.CodigoIntegracao == "J")
                    )
                );

                dynamic dynContratoTransporteFrete = new
                {
                    contratoTransporteFrete.Codigo,
                    contratoTransporteFrete.NumeroContrato,
                    ContratoExternoId = contratoTransporteFrete.ContratoExternoID,
                    NomeDoContrato = contratoTransporteFrete.NomeContrato,
                    contratoTransporteFrete.AprovacaoAdicionalRequerida,
                    contratoTransporteFrete.Cluster,
                    Pais = new { contratoTransporteFrete.Pais?.Codigo, contratoTransporteFrete.Pais?.Descricao },
                    contratoTransporteFrete.Network,
                    contratoTransporteFrete.Equipe,
                    contratoTransporteFrete.Categoria,
                    contratoTransporteFrete.SubCategoria,
                    contratoTransporteFrete.ModoContrato,
                    Transportador = new { contratoTransporteFrete.Transportador?.Codigo, contratoTransporteFrete.Transportador?.Descricao },
                    contratoTransporteFrete.ConformidadeComRSP,
                    contratoTransporteFrete.PessoaJuridica,
                    contratoTransporteFrete.TipoContrato,
                    contratoTransporteFrete.HubNonHub,
                    contratoTransporteFrete.DominioOTM,
                    DataInicial = contratoTransporteFrete.DataInicio.ToString("dd/MM/yyyy"),
                    DataFinal = contratoTransporteFrete.DataFim.ToString("dd/MM/yyyy"),
                    contratoTransporteFrete.Moeda,
                    contratoTransporteFrete.ValorPrevistoContrato,
                    contratoTransporteFrete.Padrao,
                    TermosPagamento = new { contratoTransporteFrete.TermosPagamento?.Codigo, contratoTransporteFrete.TermosPagamento?.Descricao },
                    contratoTransporteFrete.ClausulaPenal,
                    contratoTransporteFrete.Observacao,
                    UsuarioContrato = new { contratoTransporteFrete.UsuarioContrato?.Codigo, contratoTransporteFrete.UsuarioContrato?.Descricao },
                    StatusAprovacaoTransportador = contratoTransporteFrete.StatusAprovacaoTransportador,
                    StatusAssinaturaContrato = new { contratoTransporteFrete.StatusAssinaturaContrato?.Codigo, contratoTransporteFrete.StatusAssinaturaContrato?.Descricao },
                    contratoTransporteFrete.ProcessoAprovacao,
                    Situacao = contratoTransporteFrete.Ativo,
                    PermiteEdicao = permiteEdicao,
                    Anexos = (
                        from anexo in contratoTransporteFrete.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynContratoTransporteFrete);
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
        public async Task<IActionResult> ReenviarIntegracaoContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContrato = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracaoContratoTransporteFrete = repContratoTransporteFreteIntegracao.BuscarIntegracaoPendentePorCodigoContrato(codigoContrato);

                if (integracaoContratoTransporteFrete == null)
                    return new JsonpResult(false, "Integração não encontrada");

                unitOfWork.Start();

                integracaoContratoTransporteFrete.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                repContratoTransporteFreteIntegracao.Atualizar(integracaoContratoTransporteFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContrato = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> integracoesContratoTransporteFrete = repContratoTransporteFreteIntegracao.BuscarIntegracoesPorCodigoContrato(codigoContrato);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 60, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Situacao", 20, Models.Grid.Align.left);

                List<dynamic> retorno = new List<dynamic>();

                foreach (var ContratoTransporteFrete in integracoesContratoTransporteFrete)
                {
                    string tipoIntegracao = ContratoTransporteFrete.IntegrarAnexos ? "Anexo" : "Contrato";
                    foreach (var arquivo in ContratoTransporteFrete.ArquivosTransacao)
                        retorno.Add(new
                        {
                            arquivo.Codigo,
                            Data = arquivo.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                            Retorno = arquivo.Descricao + $" ({tipoIntegracao})",
                            Situacao = arquivo.DescricaoTipo,
                        });
                }

                grid.setarQuantidadeTotal(retorno.Count());
                grid.AdicionaRows(retorno);

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

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracoesContratoTransporteFrete = repContratoTransporteFreteIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracoesContratoTransporteFrete == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracoesContratoTransporteFrete.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivo Integração Contrato Transporte Frete.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarIntegracoesFalha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> integracaoContratoTransporteFrete = repContratoTransporteFreteIntegracao.BuscarIntegracoesRejeitadas();

                if (integracaoContratoTransporteFrete.Count == 0)
                    return new JsonpResult(false, "Não há integrações pendentes de integração");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracaoPendente in integracaoContratoTransporteFrete)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    repContratoTransporteFreteIntegracao.Atualizar(integracaoPendente);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete()
            {
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                ContratoExternoId = Request.GetIntParam("ContratoExternoId"),
                Categoria = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaContratoTransporte>("Categoria"),
                SubCategoria = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaContratoTransporte>("SubCategoria"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                PessoaJuridica = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PessoaJuridicaContratoTransporte>("PessoaJuridica"),
                DataInicio = Request.GetDateTimeParam("DataInicial"),
                DataFim = Request.GetDateTimeParam("DataFinal"),
                StatusAprovacaoTransportador = Request.GetBoolParam("FiltrarSomentePorAtivos") == true ? StatusAprovacaoTransportador.Aprovado : Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAprovacaoTransportador>("StatusAprovacaoTransportador"),
                StatusAssinaturaContrato = Request.GetIntParam("StatusAssinaturaContrato"),
                Situacao = Request.GetBoolParam("SomenteContratosVigentes"),
                NomeContrato = Request.GetStringParam("NomeContrato"),
                CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                FiltrarPorTransportadorContrato = Request.GetBoolParam("FiltrarPorTransportadorContrato"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Frete.TermosPagamento repTermosPagamento = new Repositorio.Embarcador.Frete.TermosPagamento(unitOfWork);
            Repositorio.Embarcador.Frete.StatusAssinaturaContrato repStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(unitOfWork);

            int codigoPais = Request.GetIntParam("Pais");
            int codigoEmpresa = Request.GetIntParam("Transportador");
            int codigoUsuario = Request.GetIntParam("UsuarioContrato");
            int codigoTermosPagamento = Request.GetIntParam("TermosPagamento");
            int codigoStatusAssinaturaContrato = Request.GetIntParam("StatusAssinaturaContrato");

            if (contratoTransporteFrete.ValorPrevistoContrato > 0)
                contratoTransporteFrete.UltimoValorPrevistoContrato = contratoTransporteFrete.ValorPrevistoContrato;

            if (contratoTransporteFrete.DataFim != null && contratoTransporteFrete.DataFim != DateTime.MinValue)
                contratoTransporteFrete.UltimaData = contratoTransporteFrete.DataFim;

            contratoTransporteFrete.NumeroContrato = Request.GetIntParam("NumeroContrato");
            contratoTransporteFrete.ContratoExternoID = Request.GetIntParam("ContratoExternoId");
            contratoTransporteFrete.NomeContrato = Request.GetStringParam("NomeDoContrato");
            contratoTransporteFrete.AprovacaoAdicionalRequerida = Request.GetBoolParam("AprovacaoAdiconalRequerida");
            contratoTransporteFrete.Cluster = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cluster>("Cluster");
            contratoTransporteFrete.Pais = codigoPais > 0 ? repPais.BuscarPorCodigo(codigoPais) : null;
            contratoTransporteFrete.Network = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.NetworkContratoTransporte>("Network");
            contratoTransporteFrete.Equipe = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EquipeContratoTransporte>("Equipe");
            contratoTransporteFrete.Categoria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaContratoTransporte>("Categoria");
            contratoTransporteFrete.SubCategoria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaContratoTransporte>("SubCategoria");
            contratoTransporteFrete.ModoContrato = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoContratoTransporte>("ModoContrato");
            contratoTransporteFrete.Transportador = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            contratoTransporteFrete.ConformidadeComRSP = Request.GetBoolParam("ConformidadeComRSP");
            contratoTransporteFrete.PessoaJuridica = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PessoaJuridicaContratoTransporte>("PessoaJuridica");
            contratoTransporteFrete.TipoContrato = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoTransporte>("TipoContrato");
            contratoTransporteFrete.HubNonHub = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.HubNonHub>("HubNonHub");
            contratoTransporteFrete.DominioOTM = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DominioOTM>("DominioOTM");
            contratoTransporteFrete.DataInicio = Request.GetDateTimeParam("DataInicial");
            contratoTransporteFrete.DataFim = Request.GetDateTimeParam("DataFinal");
            contratoTransporteFrete.Moeda = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");
            contratoTransporteFrete.ValorPrevistoContrato = Request.GetDecimalParam("ValorPrevistoContrato");
            contratoTransporteFrete.Padrao = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("Padrao");
            contratoTransporteFrete.TermosPagamento = codigoTermosPagamento > 0 ? repTermosPagamento.BuscarPorCodigo(codigoTermosPagamento) : null;
            contratoTransporteFrete.ClausulaPenal = Request.GetBoolParam("ClausulaPenal");
            contratoTransporteFrete.Observacao = Request.GetStringParam("Observacao");
            contratoTransporteFrete.UsuarioContrato = codigoUsuario > 0 ? repUsuario.BuscarPorCodigo(codigoUsuario) : null;
            contratoTransporteFrete.StatusAprovacaoTransportador = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAprovacaoTransportador>("StatusAprovacaoTransportador");
            contratoTransporteFrete.StatusAssinaturaContrato = codigoStatusAssinaturaContrato > 0 ? repStatusAssinaturaContrato.BuscarPorCodigo(codigoStatusAssinaturaContrato) : null;
            contratoTransporteFrete.ProcessoAprovacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessoAprovacaoContratoTransporte>("ProcessoAprovacao");
            contratoTransporteFrete.Ativo = Request.GetBoolParam("Situacao");
            contratoTransporteFrete.Clonado = true;
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
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.NumeroContrato, "NumeroContrato", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.NomeContrato, "NomeContrato", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.ContratoExternoID, "ContratoExternoID", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Categoria, "Categoria", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.SubCategoria, "SubCategoria", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Transportador, "Transportador", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.PessoaJuridica, "PessoaJuridica", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.DataInicial, "DataInicio", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.DataFinal, "DataFim", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.StatusAprovacaoTransportador, "StatusAprovacaoTransportador", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.StatusAssinaturaContrato, "StatusAssinaturaContrato", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Situacao, "Situacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Integração", "SituacaoIntegracao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CodigoTransportador", false);

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> integracoesContratoTransporteFrete = repContratoTransporteFreteIntegracao.BuscarTodos();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repContratoTransporteFrete.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> listaContratoTransporteFrete = totalRegistros > 0 ? repContratoTransporteFrete.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

                var lista = (from p in listaContratoTransporteFrete
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NumeroContrato,
                                 p.ContratoExternoID,
                                 NomeContrato = p.NomeContrato,
                                 Categoria = p.Categoria.ObterDescricao(),
                                 SubCategoria = p.SubCategoria.ObterDescricao(),
                                 Transportador = p.Transportador?.Descricao,
                                 CodigoTransportador = p.Transportador?.Codigo ?? 0,
                                 PessoaJuridica = p.PessoaJuridica.ObterDescricao(),
                                 DataInicio = p.DataInicio.ToString("dd/MM/yyyy"),
                                 DataFim = p.DataFim.ToString("dd/MM/yyyy"),
                                 StatusAprovacaoTransportador = p.StatusAprovacaoTransportador.ObterDescricao(),
                                 StatusAssinaturaContrato = p.StatusAssinaturaContrato?.Descricao,
                                 Situacao = p.Ativo ? "Ativo" : "Inativo",
                                 SituacaoIntegracao = integracoesContratoTransporteFrete.Where(o => o.ContratoTransporteFrete.Codigo == p.Codigo).Select(i => i.SituacaoIntegracao.ObterDescricao()).FirstOrDefault(),
                             }).ToList();

                grid.AdicionaRows(lista);
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

        private void ClonarContrato(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete, Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete novoContratoTransporteFrete = new Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete();

            novoContratoTransporteFrete = contratoTransporteFrete.Clonar();
            novoContratoTransporteFrete.CodigoContratoClonado = contratoTransporteFrete.Codigo;
            novoContratoTransporteFrete.Anexos = null;
            novoContratoTransporteFrete.Clonado = false;

            repContratoTransporteFrete.Inserir(novoContratoTransporteFrete);
        }

        #endregion
    }
}
