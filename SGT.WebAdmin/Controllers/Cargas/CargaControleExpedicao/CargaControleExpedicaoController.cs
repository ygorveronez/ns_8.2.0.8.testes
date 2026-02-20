using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.CargaControleExpedicao
{
    [CustomAuthorize("Cargas/CargaControleExpedicao", "GestaoPatio/FluxoPatio", "GestaoPatio/AnexosProdutor")]
    public class CargaControleExpedicaoController : BaseController
    {
		#region Construtores

		public CargaControleExpedicaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AlterarDados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                int codigoCargaControleExpedicao;
                int.TryParse(Request.Params("Codigo"), out codigoCargaControleExpedicao);
                string placa = Request.Params("Placa");
                string doca = Request.Params("Doca");
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoCargaControleExpedicao, true);

                if (cargaControleExpedicao.Carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada para atualizar os dados da expedição.");

                if (cargaControleExpedicao.Carga.SituacaoCarga == SituacaoCarga.AgTransportador)
                {
                    cargaControleExpedicao.Placa = Utilidades.String.RemoveDiacritics(placa).ToUpper();
                    cargaControleExpedicao.Doca = doca;
                    repCargaControleExpedicao.Atualizar(cargaControleExpedicao, Auditado);
                    var retorno = ObterDadosCargaControleExpedicao(cargaControleExpedicao, unidadeDeTrabalho);
                    return new JsonpResult(retorno);
                }
                else
                    return new JsonpResult(false, true, "A atual situação da carga (" + cargaControleExpedicao.Carga.DescricaoSituacaoCarga + ") não permite atualizar os dados da expedição.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaControleExpedicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(ObterDadosCargaControleExpedicao(cargaControleExpedicao, unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar por Carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarDadosExpedicao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigoCargaControleExpedicao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCodigo(codigoCargaControleExpedicao);

                if (cargaControleExpedicao.Carga == null)
                    throw new ControllerException("Carga não encontrada para confirmar os dados da expedição.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeDeTrabalho, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio ?? servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaControleExpedicao.Carga);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(fluxoGestaoPatio, unidadeDeTrabalho);
                bool exigeNumeroDoca = sequenciaGestaoPatio?.ExpedicaoInformarDoca ?? true;
                bool exigeConfirmarPlaca = sequenciaGestaoPatio?.ExpedicaoConfirmarPlaca ?? true;

                if (string.IsNullOrWhiteSpace(cargaControleExpedicao.Placa) && exigeConfirmarPlaca)
                    throw new ControllerException("É obrigatório informar a placa.");

                if (string.IsNullOrWhiteSpace(cargaControleExpedicao.Doca) || exigeNumeroDoca)
                    throw new ControllerException("É obrigatório informar a doca.");

                if (cargaControleExpedicao.Carga.Veiculo == null)
                    throw new ControllerException("A carga não possui um veículo informado.");

                if (cargaControleExpedicao.Carga.Motoristas.Count == 0)
                    throw new ControllerException("não é possível conformar a expedição pois a carga ainda não possui um motorista informado.");

                if (cargaControleExpedicao.Carga.SituacaoCarga != SituacaoCarga.AgTransportador)
                    throw new ControllerException($"A atual situação da carga ({cargaControleExpedicao.Carga.DescricaoSituacaoCarga}) não permite atualizar os dados da expedição.");

                if (
                    Utilidades.String.RemoveDiacritics(cargaControleExpedicao.Placa).ToUpper() == Utilidades.String.RemoveDiacritics(cargaControleExpedicao.Carga.Veiculo.Placa).ToUpper() ||
                    cargaControleExpedicao.Carga.VeiculosVinculados.Any(obj => Utilidades.String.RemoveDiacritics(obj.Placa).ToUpper() == Utilidades.String.RemoveDiacritics(cargaControleExpedicao.Placa).ToUpper() || !exigeConfirmarPlaca)
                )
                {
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.Liberada;
                    cargaControleExpedicao.Usuario = this.Usuario;
                    cargaControleExpedicao.DataConfirmacao = DateTime.Now;
                    cargaControleExpedicao.Carga.PossuiPendencia = false;
                    cargaControleExpedicao.Carga.MotivoPendencia = "";
                    cargaControleExpedicao.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                    cargaControleExpedicao.Carga.DataInicioGeracaoCTes = DateTime.Now;
                    repositorioCarga.Atualizar(cargaControleExpedicao.Carga);
                    repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
                    //todo: fazer controle encerramento MDF-e aberto e controle de percurso

                    if (cargaControleExpedicao.Carga.Carregamento != null)
                    {
                        Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeDeTrabalho);
                        cargaControleExpedicao.Carga.Carregamento.SituacaoCarregamento = SituacaoCarregamento.Fechado;
                        repCarregamento.Atualizar(cargaControleExpedicao.Carga.Carregamento);
                    }

                    Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeDeTrabalho);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFeEncerramento = serCargaMDFe.ObterCargasEncerramentoPorPlacaVeiculo(cargaControleExpedicao.Placa, unidadeDeTrabalho);

                    var retorno = new { DadosGrid = ObterDadosCargaControleExpedicao(cargaControleExpedicao, unidadeDeTrabalho), Mensagem = "", Autorizou = true };

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaControleExpedicao, null, "Confirmou os Dados da Expedição", unidadeDeTrabalho);

                    servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.Expedicao);
                    servicoFluxoGestaoPatio.Auditar(fluxoGestaoPatio, "Informou o Início do Carregamento");

                    if (cargasMDFeEncerramento.Count > 0 && !cargaControleExpedicao.Carga.NaoGerarMDFe && (cargaControleExpedicao.Filial == null || !cargaControleExpedicao.Filial.EmitirMDFeManualmente) && (cargaControleExpedicao.Carga.TipoOperacao == null || !cargaControleExpedicao.Carga.TipoOperacao.NaoEmitirMDFe))
                        SolicitarEncerramentoMDFeAutomaticamente(cargasMDFeEncerramento[0].Carga, unidadeDeTrabalho, TipoServicoMultisoftware, WebServiceConsultaCTe);
                    else
                        unidadeDeTrabalho.CommitChanges();

                    return new JsonpResult(retorno);
                }
                else
                {
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.PlacaDivergente;
                    cargaControleExpedicao.Usuario = this.Usuario;
                    cargaControleExpedicao.DataConfirmacao = DateTime.Now;

                    cargaControleExpedicao.Carga.PossuiPendencia = true;
                    cargaControleExpedicao.Carga.MotivoPendencia = "A placa informada pela expedição ( " + cargaControleExpedicao.Placa + " ) é diferente da placa informada na carga ";
                    repositorioCarga.Atualizar(cargaControleExpedicao.Carga);
                    repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);

                    //if (cargaControleExpedicao.EtapaExpedicaoLiberada == false)
                    //{
                    //    unidadeDeTrabalho.Rollback();
                    //    return new JsonpResult(false, true, "À carga ainda não está nesta etapa do fluxo por isso não é possível informar.");
                    //}

                    servicoFluxoGestaoPatio.RejeitarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.Expedicao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaControleExpedicao, null, "Confirmou os Dados da Expedição", unidadeDeTrabalho);

                    var retorno = new
                    {
                        DadosGrid = ObterDadosCargaControleExpedicao(cargaControleExpedicao, unidadeDeTrabalho),
                        Mensagem = "A placa informada ( " + cargaControleExpedicao.Placa + " ) é diferente da placa informada na carga. ",
                        Autorizou = false
                    };

                    unidadeDeTrabalho.CommitChanges();
                    return new JsonpResult(retorno);
                }
            }
            catch (BaseException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar os dados da expedição.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
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

        public async Task<IActionResult> InformarInicioCarregamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigoCargaControleExpedicao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCodigo(codigoCargaControleExpedicao);

                if (cargaControleExpedicao != null)
                {
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.AguardandoLiberacao;
                    cargaControleExpedicao.DataInicioCarregamento = DateTime.Now;

                    repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);

                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio ?? servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaControleExpedicao.CargaBase);

                    if (fluxoGestaoPatio != null)
                    {
                        if (cargaControleExpedicao.EtapaExpedicaoLiberada == false)
                        {
                            unidadeDeTrabalho.Rollback();
                            return new JsonpResult(false, true, "A liberação da informação do início do carregamento ainda não foi autorizada.");
                        }

                        Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeDeTrabalho);
                        DateTime dataAtual = DateTime.Now;

                        fluxoGestaoPatio.TempoAgInicioCarregamento = servicoFluxoGestaoPatio.ObterTempoEtapaAnterior(fluxoGestaoPatio, dataAtual);
                        fluxoGestaoPatio.DataInicioCarregamento = dataAtual;

                        repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
                    }

                    var retorno = new { DadosGrid = ObterDadosCargaControleExpedicao(cargaControleExpedicao, unidadeDeTrabalho), Mensagem = "", Autorizou = true };

                    unidadeDeTrabalho.CommitChanges();

                    return new JsonpResult(retorno);
                }
                else
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não foi localizada a expedição informada.");
                }
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o início do carregamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarTerminoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaControleExpedicao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCodigo(codigoCargaControleExpedicao);
                Servicos.Embarcador.GestaoPatio.Expedicao servicoExpedicao = new Servicos.Embarcador.GestaoPatio.Expedicao(unitOfWork, Auditado, Cliente);

                servicoExpedicao.Avancar(cargaControleExpedicao);

                unitOfWork.CommitChanges();

                var retorno = new { DadosGrid = ObterDadosCargaControleExpedicao(cargaControleExpedicao, unitOfWork), Mensagem = "", Autorizou = true };

                return new JsonpResult(retorno);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o término do carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> NotificarUsuarios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios e Serviços
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Converte valores
                string placa = Request.Params("Placa");
                if (string.IsNullOrWhiteSpace(placa)) placa = string.Empty;

                string viagem = Request.Params("Viagem");
                if (string.IsNullOrWhiteSpace(viagem)) viagem = string.Empty;

                // Valida dados
                if (string.IsNullOrWhiteSpace(placa) || placa.Length != 7)
                    return new JsonpResult(false, true, "Placa é obrigatório.");

                if (string.IsNullOrWhiteSpace(viagem))
                    return new JsonpResult(false, true, "Viagem é obrigatória.");

                // Verifica se ja não existe uma carga
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.PesquisaPorCodigoEmbarcador(viagem);
                if (carga != null)
                    return new JsonpResult(false, true, "A Viagem " + carga.CodigoCargaEmbarcador + " já existe.");

                // Coloca mascara na placa
                placa = placa.Substring(0, 3).ToUpper() + "-" + placa.Substring(3);

                // Monta mensagem
                string mensagem = Localization.Resources.Cargas.CargaControleExpedicao.ExpedicaoNaoEncontrouDocumentoViagemPlaca;

                // Configuração notificação
                string URLPagina = "Cargas/CargaControleExpedicao";

                // Busca todos os usuarios
                List<Dominio.Entidades.Usuario> usuarios = repCargaControleExpedicao.BuscarUsuariosNotificacao();

                // Itera os usuários para notificar cada um deles
                for (var i = 0; i < usuarios.Count; i++)
                    serNotificacao.GerarNotificacao(usuarios[i], 0, URLPagina, mensagem, IconesNotificacao.agConfirmacao, SmartAdminBgColor.blueDark, TipoNotificacao.todas, TipoServicoMultisoftware, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao notificar os usuários.");
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

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCodigo(codigo);

                if (cargaControleExpedicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio ?? servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaControleExpedicao.CargaBase);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.Expedicao, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.Expedicao servicoExpedicao = new Servicos.Embarcador.GestaoPatio.Expedicao(unitOfWork, Auditado, Cliente);

                servicoExpedicao.Avancar(codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarFechamentoPesagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo repositorioGuaritaPesagemAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio);

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(fluxoGestaoPatio, unitOfWork);


                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                decimal pesoLiquidoKg = cargaGuarita.PesagemInicial - cargaGuarita.PesagemFinal;
                decimal pesoLiquidoCaixas = pesoLiquidoKg / (decimal)40.80;
                decimal pesoLiquidoPosPerdas = (((decimal)1.0 - (cargaGuarita.PorcentagemPerda / 100)) * pesoLiquidoCaixas);
                decimal pesoLiquidoPosPerdasKgs = pesoLiquidoPosPerdas * (decimal)40.80;
                decimal resultadoFinalProcessoCaixas = pesoLiquidoPosPerdas - cargaGuarita.PesagemQuantidadeCaixas;
                decimal resultadoFinalProcessoKg = resultadoFinalProcessoCaixas * (decimal)40.80;

                return new JsonpResult(new
                {
                    CaixasNFEntrada = cargaGuarita.PesagemQuantidadeCaixas,
                    PesoInicial = cargaGuarita.PesagemInicial,
                    PesoFinal = cargaGuarita.PesagemFinal,
                    PesoLiquidoKG = pesoLiquidoKg,
                    PesoLiquidoCXS = pesoLiquidoCaixas.ToString("n4"),
                    ConversaoPesoCaixa = "40,80",
                    PorcentagemPerdas = cargaGuarita.PorcentagemPerda.ToString() + '%',
                    PesoLiquidoPosPerdas = pesoLiquidoPosPerdas.ToString("n4"),
                    PesoLiquidoPosPerdasKgs = pesoLiquidoPosPerdasKgs.ToString("n4"),
                    ResultadoFinalProcessoCaixas = resultadoFinalProcessoCaixas.ToString("n4"),
                    ResultadoFinalProcessoKgs = resultadoFinalProcessoKg.ToString("n4"),
                    QuantidadeLitros = cargaGuarita.QuantidadeLitros,
                    PermiteInformarQtdLitros = sequenciaGestaoPatio.DeslocamentoPatioPermiteInformarQuantidade
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar informações da pesagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacoesFechamentoPesagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                cargaGuarita.QuantidadeLitros = Request.GetDecimalParam("QuantidadeLitros");

                repositorioCargaGuarita.Atualizar(cargaGuarita);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações do fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        public dynamic ObterDadosCargaControleExpedicao(Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio ?? servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaControleExpedicao.CargaBase);

            if (cargaControleExpedicao.Carga != null)
                return ObterDadosCargaControleExpedicaoPorCarga(cargaControleExpedicao, fluxoGestaoPatio, unitOfWork);

            return ObterDadosCargaControleExpedicaoPorPreCarga(cargaControleExpedicao, fluxoGestaoPatio, unitOfWork);
        }

        public dynamic ObterDadosCargaControleExpedicaoPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(fluxoGestaoPatio, unitOfWork);
            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            bool exigeConfirmacaoPlaca = sequenciaGestaoPatio?.ExpedicaoConfirmarPlaca ?? true;
            bool informarDoca = sequenciaGestaoPatio?.ExpedicaoInformarDoca ?? false;

            return new
            {
                cargaControleExpedicao.Codigo,
                cargaControleExpedicao.SituacaoCargaControleExpedicao,
                CodigoFluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio?.Codigo ?? 0,
                CodigoCarga = cargaControleExpedicao.Carga.Codigo,
                CodigoPreCarga = cargaControleExpedicao.PreCarga?.Codigo,
                NumeroCarga = cargaControleExpedicao.Carga.CodigoCargaEmbarcador ?? "",
                NumeroPreCarga = cargaControleExpedicao.PreCarga?.NumeroPreCarga ?? "",
                cargaControleExpedicao.DescricaoSituacao,
                Transportador = cargaControleExpedicao.Carga.Empresa?.Descricao,
                DataCarregamento = cargaControleExpedicao.Carga.DataCarregamentoCarga.HasValue ? cargaControleExpedicao.Carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:" + (exibirDataCarregamentoExato ? "mm" : "00")) : cargaControleExpedicao.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:" + (exibirDataCarregamentoExato ? "mm" : "00")),
                DataCarregamentoPrevista = cargaControleExpedicao.FluxoGestaoPatio?.DataInicioCarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Motorista = cargaControleExpedicao.Carga.Motoristas.FirstOrDefault()?.Nome ?? string.Empty,
                Veiculo = cargaControleExpedicao.Carga.RetornarPlacas,
                ExigeConfirmacaoPlaca = exigeConfirmacaoPlaca,
                InformarDoca = informarDoca,
                Filial = cargaControleExpedicao.Filial?.Descricao,
                Placa = !exigeConfirmacaoPlaca ? cargaControleExpedicao.Carga.RetornarPlacas : (!string.IsNullOrWhiteSpace(cargaControleExpedicao.Placa) ? cargaControleExpedicao.Placa : ""),
                Doca = !informarDoca ? (!string.IsNullOrWhiteSpace(cargaControleExpedicao.Carga.NumeroDocaEncosta) ? cargaControleExpedicao.Carga.NumeroDocaEncosta : cargaControleExpedicao.Carga.NumeroDoca) : !string.IsNullOrWhiteSpace(cargaControleExpedicao.Doca) ? cargaControleExpedicao.Doca : "",
                PossuiSeparacao = (cargaControleExpedicao.Carga.PossuiSeparacao && !cargaControleExpedicao.Carga.PossuiSeparacaoVolume),
                PossuiSeparacaoVolume = (cargaControleExpedicao.Carga.PossuiSeparacao && cargaControleExpedicao.Carga.PossuiSeparacaoVolume),
                AptoConferir = !cargaControleExpedicao.Carga.SeparacaoConferida,
                DT_RowColor = cargaControleExpedicao.SituacaoCargaControleExpedicao.ObterCorLinha(),
                DT_FontColor = cargaControleExpedicao.SituacaoCargaControleExpedicao.ObterCorFonte()
            };
        }

        public dynamic ObterDadosCargaControleExpedicaoPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(fluxoGestaoPatio, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(cargaControleExpedicao.PreCarga.Codigo);
            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            bool exigeConfirmacaoPlaca = sequenciaGestaoPatio?.ExpedicaoConfirmarPlaca ?? true;
            bool informarDoca = sequenciaGestaoPatio?.ExpedicaoInformarDoca ?? false;

            return new
            {
                cargaControleExpedicao.Codigo,
                cargaControleExpedicao.SituacaoCargaControleExpedicao,
                CodigoFluxoGestaoPatio = cargaControleExpedicao.FluxoGestaoPatio?.Codigo ?? 0,
                CodigoCarga = 0,
                CodigoPreCarga = cargaControleExpedicao.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = cargaControleExpedicao.PreCarga.NumeroPreCarga ?? "",
                cargaControleExpedicao.DescricaoSituacao,
                Transportador = cargaControleExpedicao.PreCarga.Empresa?.Descricao ?? "",
                DataCarregamento = cargaJanelaCarregamento?.InicioCarregamento.ToString($"dd/MM/yyyy HH:{(exibirDataCarregamentoExato ? "mm" : "00")}") ?? "",
                DataCarregamentoPrevista = cargaControleExpedicao.FluxoGestaoPatio?.DataInicioCarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Motorista = cargaControleExpedicao.PreCarga.Motoristas?.FirstOrDefault()?.Nome ?? string.Empty,
                Veiculo = cargaControleExpedicao.PreCarga.RetornarPlacas,
                ExigeConfirmacaoPlaca = exigeConfirmacaoPlaca,
                InformarDoca = informarDoca,
                Filial = cargaControleExpedicao.Filial?.Descricao,
                Placa = !exigeConfirmacaoPlaca ? cargaControleExpedicao.PreCarga.RetornarPlacas : (!string.IsNullOrWhiteSpace(cargaControleExpedicao.Placa) ? cargaControleExpedicao.Placa : ""),
                Doca = !informarDoca ? "" : cargaControleExpedicao.Doca ?? "",
                PossuiSeparacao = false,
                PossuiSeparacaoVolume = false,
                AptoConferir = false,
                DT_RowColor = cargaControleExpedicao.SituacaoCargaControleExpedicao.ObterCorLinha(),
                DT_FontColor = cargaControleExpedicao.SituacaoCargaControleExpedicao.ObterCorFonte()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Situacao = Request.GetEnumParam("Situacao", SituacaoCargaControleExpedicao.Todas)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.EditableCell editablePlaca = new Models.Grid.EditableCell(TipoColunaGrid.aMask, "*******", 8);
                Models.Grid.EditableCell editableDoca = new Models.Grid.EditableCell(TipoColunaGrid.aInt, 2);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("CodigoPreCarga", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("AptoConferir", false);
                grid.AdicionarCabecalho("PossuiSeparacao", false);
                grid.AdicionarCabecalho("PossuiSeparacaoVolume", false);
                grid.AdicionarCabecalho("ExigeConfirmacaoPlaca", false);
                grid.AdicionarCabecalho("InformarDoca", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.DataCarregamento, "DataCarregamento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "NumeroCarga", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.PreCarga, "NumeroPreCarga", 12, Models.Grid.Align.left, true).Ocultar(true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Filial, "Filial", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", 18, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Placa, "Placa", 15, Models.Grid.Align.center, true, false, false, false, true, editablePlaca).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Doca, "Doca", 5, Models.Grid.Align.center, true, false, false, false, true, editableDoca).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("SituacaoCargaControleExpedicao", false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                int totalRegistros = repositorioCargaControleExpedicao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao> listaCargaControleExpedicao = totalRegistros > 0 ? repositorioCargaControleExpedicao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>();
                var listaCargaControleExpedicaoRetornar = (from cargaControleExpedicao in listaCargaControleExpedicao select ObterDadosCargaControleExpedicao(cargaControleExpedicao, unitOfWork)).ToList();

                grid.AdicionaRows(listaCargaControleExpedicaoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCarregamento")
                return "Carga.DataCriacaoCarga";

            if (propriedadeOrdenar == "NumeroCarga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Transportador")
                return "Carga.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenar == "NumeroCarga")
                return "Carga.Filial.Descricao";

            return propriedadeOrdenar;
        }

        private Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio ObterSequenciaGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

            return servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);
        }

        private void SolicitarEncerramentoMDFeAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarPorCarga(carga.Codigo);
            if (cargaMDFEs.Count > 0)
            {
                unitOfWork.CommitChanges();
                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                {
                    if (cargaMDFe.MDFe != null)
                    {
                        if (cargaMDFe.MDFe.Importado != true && cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramento = serCargaMDFe.ObterDadosEncerramento(cargaMDFe.Codigo, unitOfWork);
                            string retornoEncerramento = serCargaMDFe.EncerrarMDFe(dadosEncerramento.Codigo, carga.Codigo, dadosEncerramento.Localidades[0].Codigo, dadosEncerramento.DataEncerramento, webServiceConsultaCTe, this.Usuario, tipoServicoMultisoftware, unitOfWork, Auditado);
                            if (string.IsNullOrWhiteSpace(retornoEncerramento))
                            {
                                cargaMDFe.EmEncerramento = true;
                                repCargaMDFe.Atualizar(cargaMDFe);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe, null, "Solicitação de encerramento pela confirmação de dados da expedição.", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe.MDFe, null, "Solicitação de encerramento pela confirmação de dados da expedição.", unitOfWork);
                        }
                        else
                        {
                            if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                            {
                                cargaMDFe.EmEncerramento = true;
                                repCargaMDFe.Atualizar(cargaMDFe);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
