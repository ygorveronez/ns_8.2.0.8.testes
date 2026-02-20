using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Acerto
{
    [CustomAuthorize(new string[] { "BuscarProximosDados" }, "Acertos/AcertoViagem", "Cargas/Carga")]
    public class AcertoViagemController : BaseController
    {
		#region Construtores

		public AcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMotorista, codigoOperador, codigoCarga, numeroAcerto, codigoVeiculo;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("NumeroAcerto"), out numeroAcerto);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);

                DateTime dataInicial, dataFinal, dataAcerto;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                DateTime.TryParse(Request.Params("DataAcerto"), out dataAcerto);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapaAcerto;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacaoAcerto;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.TryParse(Request.Params("Etapa"), out etapaAcerto);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.TryParse(Request.Params("Situacao"), out situacaoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Acerto", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Período", "DescricaoPeriodo", 25, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Placa(s)", "PlacasVeiculos", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, true);

                string propOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoViagem> listaAcertoViagem = repAcertoViagem.Consulta(codigoVeiculo, unitOfWork, numeroAcerto, codigoMotorista, dataInicial, dataFinal, dataAcerto, codigoOperador, etapaAcerto, situacaoAcerto, codigoCarga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoViagem.ContaConsulta(codigoVeiculo, unitOfWork, numeroAcerto, codigoMotorista, dataInicial, dataFinal, dataAcerto, codigoOperador, etapaAcerto, situacaoAcerto, codigoCarga));

                var lista = (from p in listaAcertoViagem
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Numero,
                                 DescricaoPeriodo = p.DescricaoPeriodo,
                                 p.PlacasVeiculos,
                                 Motorista = p.Motorista != null ? p.Motorista.Nome : string.Empty,
                                 p.DescricaoSituacao
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

        public async Task<IActionResult> Iniciar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller Iniciar " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                bool aprovacaoAbastecimento, aprovacaoPedagio;
                bool.TryParse(Request.Params("AprovacaoAbastecimento"), out aprovacaoAbastecimento);
                bool.TryParse(Request.Params("AprovacaoPedagio"), out aprovacaoPedagio);

                int codigoMotorista, codigo, codigoSegmento;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("SegmentoVeiculo"), out codigoSegmento);
                int.TryParse(Request.Params("Codigo"), out codigo);

                string observacao = Request.Params("Observacao");
                string numeroFrota = Request.Params("NumeroFrota");

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                DateTime dataHoraInicial, dataHoraFinal;
                DateTime.TryParse(Request.Params("DataHoraInicial"), out dataHoraInicial);
                DateTime.TryParse(Request.Params("DataHoraFinal"), out dataHoraFinal);
                if (ConfiguracaoEmbarcador.AcertoDeViagemComDiaria)
                {
                    dataInicial = dataHoraInicial;
                    dataFinal = dataHoraFinal;
                }


                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                if (dataInicial == null || dataInicial == DateTime.MinValue)
                    return new JsonpResult(false, "Favor informe a data inicial.");

                if ((!ConfiguracaoEmbarcador.AcertoDeViagemComDiaria) && (dataFinal == null || dataFinal == DateTime.MinValue))
                    return new JsonpResult(false, "Favor informe a data inicial.");

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = null;

                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);
                else
                    acertoViagem = new Dominio.Entidades.Embarcador.Acerto.AcertoViagem();

                if (ConfiguracaoEmbarcador.AcertoDeViagemComDiaria && repAcertoViagem.ContemAcertoEmAberto(codigoMotorista, codigo))
                    return new JsonpResult(false, "Já existe um acerto de viagem em andamento para este mesmo motorista.");

                if (codigo > 0 && acertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
                    return new JsonpResult(false, "A situação do acerto não permite esta operação, favor atualize a sua tela.");

                acertoViagem.AprovacaoAbastecimento = aprovacaoAbastecimento;
                acertoViagem.AprovacaoPedagio = aprovacaoPedagio;
                acertoViagem.DataInicial = dataInicial;
                if (dataFinal > DateTime.MinValue)
                    acertoViagem.DataFinal = dataFinal;
                else
                    acertoViagem.DataFinal = null;
                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;

                if (codigoMotorista > 0)
                    acertoViagem.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                if (codigoSegmento > 0)
                    acertoViagem.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigoSegmento);
                else
                    acertoViagem.SegmentoVeiculo = null;

                acertoViagem.Observacao = observacao;
                acertoViagem.Operador = this.Usuario;
                acertoViagem.OperadorInicioAcerto = this.Usuario;
                acertoViagem.NumeroFrota = numeroFrota;
                acertoViagem.CentroResultado = acertoViagem.Motorista?.CentroResultado ?? null;

                if (ConfiguracaoEmbarcador.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto && acertoViagem.CentroResultado == null)
                    return new JsonpResult(false, "O motorista informado no Acerto de Viagem não possui Centro de Resultado informado.");

                if (ConfiguracaoEmbarcador.LancarFolgaAutomaticamenteNoAcerto && acertoViagem.DataFinal.HasValue)
                {
                    int qtdDiasTrabalhado = (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;
                    qtdDiasTrabalhado += acertoViagem.Motorista.DiasTrabalhado;

                    if (qtdDiasTrabalhado > 0)
                    {
                        int qtdDiasFolga = qtdDiasTrabalhado / 6;
                        qtdDiasFolga -= acertoViagem.Motorista.DiasFolgaRetirado;
                        if (qtdDiasFolga > 0)
                        {
                            acertoViagem.QuantidadeDiasFolga = qtdDiasFolga;
                            acertoViagem.DataInicioFolga = acertoViagem.DataFinal.Value.AddDays(1);
                            acertoViagem.DataFinalFolga = acertoViagem.DataInicioFolga.Value.AddDays(qtdDiasFolga - 1);
                        }
                    }
                }

                bool atualizarPedagio = false;
                if (codigo > 0)
                {
                    repAcertoViagem.Atualizar(acertoViagem);
                    servAcertoViagem.AtualizarCargasAcerto(acertoViagem, unitOfWork, Request.Params("ListaCargas"), Auditado);
                    servAcertoViagem.AtualizarVeiculoAcerto(acertoViagem, unitOfWork, Auditado);
                    atualizarPedagio = true;
                }
                else
                {
                    acertoViagem.Numero = repAcertoViagem.UltimoNumeracao() + 1;
                    acertoViagem.DataAcerto = DateTime.Now;
                    repAcertoViagem.Inserir(acertoViagem);
                }

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = servAcertoViagem.InserirCargaAcerto(acertoViagem, unitOfWork, ConfiguracaoEmbarcador.SituacaoCargaAcertoViagem);
                List<Dominio.Entidades.Veiculo> listaVeiculo = new List<Dominio.Entidades.Veiculo>();
                if (!ConfiguracaoEmbarcador.NaoObrigarInformarSegmentoNoAcertoDeViagem)
                {
                    listaVeiculo = servAcertoViagem.InserirVeiculoAcerto(acertoViagem, unitOfWork);
                    for (int i = 0; i < listaVeiculo.Count; i++)
                    {
                        if (listaVeiculo[i].SegmentoVeiculo == null && listaVeiculo[i].TipoVeiculo == "1")//somente carreta
                            return new JsonpResult(false, "A carreta " + listaVeiculo[i].Placa + " não possui segmento em seu cadastro.");
                    }
                }                
                servAcertoViagem.InserirAbastecimentoAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas, Auditado);
                servAcertoViagem.InserirOcorrenciaAcerto(acertoViagem, unitOfWork, listaVeiculo, acertoViagem.Motorista);
                if (ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem)
                    servAcertoViagem.InserirInfracoesAcerto(acertoViagem, unitOfWork, acertoViagem.Motorista);

                if (ConfiguracaoEmbarcador.AcertoDeViagemComDiaria)
                {
                    servAcertoViagem.InserirAdiantamentoAcerto(acertoViagem, unitOfWork, acertoViagem.Motorista);
                    servAcertoViagem.InserirDiariasAcerto(acertoViagem, unitOfWork, acertoViagem.Motorista);
                }

                acertoViagem.Motorista.DataFechamentoAcerto = acertoViagem.DataFinal;
                repUsuario.Atualizar(acertoViagem.Motorista);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem.Motorista, null, "Iniciou acerto de viagem.", unitOfWork);

                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Inicio, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem, null, "Inciou o acerto de viagem " + acertoViagem.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                if (atualizarPedagio)
                    servAcertoViagem.AtualizarPedagiosAcerto(acertoViagem, unitOfWork, Request.Params("ListaPedagios"), Request.Params("ListaPedagiosCredito"), Auditado);
                else
                    servAcertoViagem.InserirPegadioAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas);

                var dynRetorno = new { Codigo = acertoViagem.Codigo };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {                
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller Iniciar " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacaoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller SalvarObservacaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                acertoViagem.Observacao = Request.Params("Observacao");
                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a observação do acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller SalvarObservacaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller CancelarAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteCancelamento)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para cancelar o acerto de viagem.");

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                acertoViagem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado;
                acertoViagem.DataAlteracao = DateTime.Now;

                repAcertoViagem.Atualizar(acertoViagem);

                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Cancelamento, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem, null, "Cancelou o acerto de viagem " + acertoViagem.Descricao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller CancelarAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Servicos.Log.TratarErro(" Inicio Controller BuscarPorCodigo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas;

                var dynRetorno = servAcertoViagem.RetornaObjetoCompletoAcertoViagem(codigo, unitOfWork, etapa, ConfiguracaoEmbarcador, Auditado);

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller BuscarPorCodigo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> BuscarProximosDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller BuscarProximosDados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoMotorista;
                int.TryParse(Request.Params("CodigoMotorista"), out codigoMotorista);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarProximosDados(codigoMotorista);

                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculo = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarVeiculoTracaoPorMotorista(codigoMotorista);

                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = veiculo?.SegmentoVeiculo != null
                                                                                        ? repSegmentoVeiculo.BuscarPorCodigo(veiculo.SegmentoVeiculo.Codigo)
                                                                                        : repSegmentoVeiculo.BuscarPorCodigo(0);


                dynamic dynRetorno;
                if (acertoViagem != null)
                {
                    dynRetorno = new
                    {
                        Numero = acertoViagem.Numero + 1,
                        Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataHoraInicial = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value.ToString("dd/MM/yyyy HH:HH:ss") : string.Empty,
                        NumeroFrota = veiculo?.NumeroFrota,
                        SegmentoVeiculoCodigo = segmentoVeiculo?.Codigo,
                        SegmentoVeiculoDescricao = segmentoVeiculo?.Descricao

                    };
                }
                else
                {
                    dynRetorno = new
                    {
                        Numero = 1,
                        Data = string.Empty,
                        DataHoraInicial = string.Empty,
                        NumeroFrota = veiculo?.NumeroFrota,
                        SegmentoVeiculoCodigo = segmentoVeiculo?.Codigo,
                        SegmentoVeiculoDescricao = veiculo?.SegmentoVeiculo?.Descricao
                    };
                }
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(" Fim Controller BuscarProximosDados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar próximos dados");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ValidarNovoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller ValidarNovoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoMotorisca;
                int.TryParse(Request.Params("Motorista"), out codigoMotorisca);
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                bool contemAcertoEmAberto = repAcertoViagem.ContemAcertoEmAberto(codigoMotorisca, codigo);

                var dynRetorno = new
                {
                    ContemAcertoEmAberto = contemAcertoEmAberto
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar acertos em abertos.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller ValidarNovoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";
            if (propriedadeOrdenar == "DescricaoPeriodo")
                return "DataInicial";
            if (propriedadeOrdenar == "Motorista")
                return propriedadeOrdenar + ".Nome";

            return propriedadeOrdenar;
        }

        #endregion
    }
}


