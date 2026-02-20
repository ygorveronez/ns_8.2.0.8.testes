using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize(new string[] { "PesquisaAutorizacao", "DetalhesSolicitacao", "DetalhesAutorizacao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaAprovacaoFreteController : BaseController
    {
        #region Construtores

        public CargaAprovacaoFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> DetalhesAutorizacaoAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga autorizacao = await repositorioAprovacao.BuscarPorCodigoAsync(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                    string.Empty,
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DetalhesSolicitacaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.NaoInformada)
                    return new JsonpResult(null);

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaSolicitacaoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.Carga> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaSolicitacaoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaSolicitacaoFreteAnexo> anexos = await repositorioAnexo.BuscarPorEntidadeAsync(codigo);

                return new JsonpResult(new
                {
                    carga.Codigo,
                    Motivo = carga.MotivoSolicitacaoFrete?.Descricao ?? string.Empty,
                    Observacao = carga.ObservacaoSolicitacaoFrete ?? string.Empty,
                    ListaAnexo = (
                        from anexo in anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a solicitação de frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaAutorizacaoAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellation);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a carga.");

                if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.NaoInformada)
                    return new JsonpResult(null);

                if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.SemRegraAprovacao)
                {
                    return new JsonpResult(new
                    {
                        AprovacoesNecessarias = "",
                        Aprovacoes = "",
                        CorIconeAba = carga.SituacaoAlteracaoFreteCarga.ObterCorIcone(),
                        PossuiRegras = false,
                        Reprovacoes = "",
                        carga.SituacaoAlteracaoFreteCarga,
                        Autorizacoes = new List<dynamic>()
                    });
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null;
                    Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork, cancellation);
                    List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> listaAutorizacao = await repositorioAprovacao.ConsultaAutorizacoesAsync(codigoCarga, parametrosConsulta);
                    int aprovacoes = await repositorioAprovacao.ContarAprovacoesAsync(codigoCarga);
                    int aprovacoesNecessarias = await repositorioAprovacao.ContarAprovacoesNecessariasAsync(codigoCarga);
                    int reprovacoes = await repositorioAprovacao.ContarReprovacoesAsync(codigoCarga);

                    return new JsonpResult(new
                    {
                        AprovacoesNecessarias = aprovacoesNecessarias,
                        Aprovacoes = aprovacoes,
                        CorIconeAba = carga.SituacaoAlteracaoFreteCarga.ObterCorIcone(),
                        PossuiRegras = true,
                        Reprovacoes = reprovacoes,
                        carga.SituacaoAlteracaoFreteCarga,
                        Autorizacoes = (
                            from autorizacao in listaAutorizacao
                            select new
                            {
                                autorizacao.Codigo,
                                PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                                Situacao = autorizacao.Situacao.ObterDescricao(),
                                Usuario = autorizacao.Usuario?.Nome,
                                Regra = autorizacao.Descricao,
                                Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                                Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                                DT_RowColor = autorizacao.ObterCorGrid()
                            }
                        ).ToList()
                    });
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as autorizações.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReprocessarRegrasAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorio.BuscarPorCodigoAsync(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a carga.");

                if (carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                AtualizarAprovacao(unitOfWork, carga);

                await repositorio.AtualizarAsync(carga);

                await unitOfWork.CommitChangesAsync();

                if (carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.SemRegraAprovacao)
                    return new JsonpResult(true);

                return new JsonpResult(false, true, "Sem regra de aprovação.");
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, ConfiguracaoEmbarcador);

            servicoCargaAprovacaoFrete.CriarAprovacao(carga, TipoRegraAutorizacaoCarga.InformadoManualmente, TipoServicoMultisoftware);
        }

        #endregion
    }
}
