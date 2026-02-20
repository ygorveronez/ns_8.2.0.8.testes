using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MarcacaoFilaCarregamento")]
    public class MarcacaoFilaCarregamentoController : BaseController
    {
        #region Construtores

        public MarcacaoFilaCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarNaFila()
        {
            Repositorio.UnitOfWorkContainer unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(new Repositorio.UnitOfWork(_conexao.StringConexao));

            try
            {
                unitOfWorkContainer.StartContainer();

                int codigoMotorista = Request.GetIntParam("Motorista");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWorkContainer.UnitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWorkContainer.UnitOfWork);

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    throw new ControllerException("Motorista não encontrado");

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, auditavel: true);

                if (veiculo == null)
                    throw new ControllerException("Veículo não encontrado");

                if (veiculo.SituacaoCadastro != SituacaoCadastroVeiculo.Aprovado)
                    throw new ControllerException("Aguardando autorização de cadastro (Veiculo)");

                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

                if (centroCarregamento == null)
                    throw new ControllerException("Centro de carregamento não encontrado.");

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (veiculoMotorista != null && veiculoMotorista.Codigo != motorista.Codigo)
                {
                    if ((veiculo.Empresa != null) && (motorista.Empresa != null))
                    {
                        if ((motorista.Empresa.Codigo != veiculo.Empresa.Codigo) && !motorista.Empresas.Contains(veiculo.Empresa))
                            return new JsonpResult(false, true, "O motorista informado não pertence a mesma empresa do veículo.");
                    }

                    //veiculo.CPFMotorista = motorista.CPF;
                    //veiculo.NomeMotorista = motorista.Nome;
                    //veiculo.Motorista = motorista;

                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristasPrincipalVeiculo = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                    if (motoristasPrincipalVeiculo != null)
                        repVeiculoMotorista.Deletar(motoristasPrincipalVeiculo);

                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaVeiculoVeiculo = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                    {
                        CPF = motorista.CPF,
                        Motorista = motorista,
                        Nome = motorista.Nome,
                        Veiculo = veiculo,
                        Principal = true
                    };

                    repVeiculoMotorista.Inserir(motoristaVeiculoVeiculo);
                    repositorioVeiculo.Atualizar(veiculo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, veiculo.GetChanges(), "Alterado o motorista para adicionar na fila de carregamento manualmente.", unitOfWorkContainer.UnitOfWork);
                }

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = AdicionarNaFila(veiculo, centroCarregamento, motorista, unitOfWorkContainer);

                unitOfWorkContainer.CommitChangesContainer();

                return new JsonpResult(new
                {
                    CentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento.Descricao,
                    PlacaTracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                    PlacasReboques = string.Join(", ", (from reboque in filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                    DataEntrada = filaCarregamentoVeiculo.DataEntrada.ToString("dd/MM/yyyy HH:mm"),
                    filaCarregamentoVeiculo.Posicao
                });
            }
            catch (BaseException excecao)
            {
                unitOfWorkContainer.RollbackContainer();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWorkContainer.RollbackContainer();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro na fila de carregamento.");
            }
            finally
            {
                unitOfWorkContainer.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarNaFilaPorVeiculo()
        {
            Repositorio.UnitOfWorkContainer unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(new Repositorio.UnitOfWork(_conexao.StringConexao));

            try
            {
                unitOfWorkContainer.StartContainer();

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, auditavel: true);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWorkContainer.UnitOfWork);

                if (veiculo == null)
                    throw new ControllerException("Veículo não encontrado");

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                if (veiculoMotorista == null)
                    throw new ControllerException("O veículo informado não possui motorista.");

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = servicoCentroCarregamento.ObterCentroCarregamentoPorVeiculo(veiculo);

                if (centroCarregamento == null)
                    throw new ControllerException("Centro de carregamento não encontrado.");

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = AdicionarNaFila(veiculo, centroCarregamento, veiculoMotorista, unitOfWorkContainer);

                unitOfWorkContainer.CommitChangesContainer();

                return new JsonpResult(new
                {
                    CentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento.Descricao,
                    PlacaTracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                    PlacasReboques = string.Join(", ", (from reboque in filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                    DataEntrada = filaCarregamentoVeiculo.DataEntrada.ToString("dd/MM/yyyy HH:mm"),
                    filaCarregamentoVeiculo.Posicao
                });
            }
            catch (BaseException excecao)
            {
                unitOfWorkContainer.RollbackContainer();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWorkContainer.RollbackContainer();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro na fila de carregamento.");
            }
            finally
            {
                unitOfWorkContainer.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMotorista = Request.GetIntParam("Motorista");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    return new JsonpResult(false, true, "Motorista não encontrado");

                return new JsonpResult(new
                {
                    Codigo = motorista.CodigoIntegracao,
                    motorista.Nome,
                    ValidadeCnh = motorista.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy"),
                    ValidadeGr = motorista.DataValidadeLiberacaoSeguradora?.ToString("dd/MM/yyyy"),
                    CodigoTransportador = motorista.Empresa?.CodigoIntegracao?.ToString() ?? "",
                    DescricaoTransportador = motorista.Empresa?.Descricao ?? "",
                    FotoMotorista = ObterFotoBase64(motorista.Codigo, unitOfWork),
                    StatusValidadeCnh = ObterStatusValidade(motorista.DataVencimentoHabilitacao),
                    StatusValidadeGr = ObterStatusValidade(motorista.DataValidadeLiberacaoSeguradora),
                    ClasseCorStatusValidadeCnh = ObterClasseCorStatusValidade(motorista.DataVencimentoHabilitacao),
                    ClasseCorStatusValidadeGr = ObterClasseCorStatusValidade(motorista.DataValidadeLiberacaoSeguradora)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, auditavel: true);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Veículo não encontrado");

                if (veiculo.IsTipoVeiculoTracao())
                {
                    return new JsonpResult(new
                    {
                        PlacaTracao = veiculo.Placa_Formatada,
                        PlacasReboques = string.Join(", ", (from veiculoVinculado in veiculo.VeiculosVinculados select veiculoVinculado.Placa_Formatada).ToList()),
                        ModeloVeicular = veiculo.ModeloVeicularCarga?.Descricao
                    });
                }

                Dominio.Entidades.Veiculo tracao = repositorioVeiculo.BuscarPorReboque(veiculo.Codigo);

                return new JsonpResult(new
                {
                    PlacaTracao = tracao?.Placa_Formatada,
                    PlacasReboques = veiculo.Placa_Formatada,
                    ModeloVeicular = veiculo.ModeloVeicularCarga?.Descricao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCentroCarregamentoPorVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, auditavel: true);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Veículo não encontrado");

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = servicoCentroCarregamento.ObterCentroCarregamentoPorVeiculo(veiculo);

                return new JsonpResult(new
                {
                    Codigo = centroCarregamento?.Codigo ?? 0,
                    Descricao = centroCarregamento?.Descricao ?? ""
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter o centro de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo AdicionarNaFila(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWorkContainer unitOfWorkContainer)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWorkContainer, Auditado.Usuario, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(TipoServicoMultisoftware));
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo, utilizarModeloVeicularCargaPorReboque: false);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculoEmViagem = servicoFilaCarregamentoVeiculo.ObterListaFilaCarregamentoVeiculoEmViagem(conjuntoVeiculo, centroCarregamento.Codigo);

            if (filasCarregamentoVeiculoEmViagem.Count > 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWorkContainer.UnitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWorkContainer.UnitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWorkContainer.UnitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculoEmViagem in filasCarregamentoVeiculoEmViagem)
                {
                    if (filaCarregamentoVeiculoEmViagem.Carga.CargaFechada)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(filaCarregamentoVeiculoEmViagem.Carga);
                        bool existeFluxoGestaoPatioComEtapaFimViagem = fluxoGestaoPatio?.Etapas.Any(o => o.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.FimViagem) ?? false;

                        if (existeFluxoGestaoPatioComEtapaFimViagem && !fluxoGestaoPatio.DataFimViagem.HasValue)
                        {
                            if ((fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != EtapaFluxoGestaoPatio.FimViagem) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Rejeitado))
                                throw new ControllerException($"A atual situação do fluxo de pátio da carga {fluxoGestaoPatio.Carga.CodigoCargaEmbarcador} não permite");

                            servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.FimViagem);
                        }
                    }

                    string descricaoAuditoria = "Fila de carregamento encerrada ao realizar marcação na fila de carregamento";
                    servicoFilaCarregamentoVeiculo.Finalizar(filaCarregamentoVeiculoEmViagem, descricaoHistorico: descricaoAuditoria);
                    Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(filaCarregamentoVeiculoEmViagem.Carga, DateTime.Now, configuracaoEmbarcador, base.Auditado, descricaoAuditoria, unitOfWorkContainer.UnitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoMarcarFilaCarregamento);

                    if (servicoCarga.VerificarFinalizarCargaAoFinalizarGestaoPatio(filaCarregamentoVeiculoEmViagem.Carga, configuracaoEmbarcador))
                        servicoCarga.LiberarSituacaoDeCargaFinalizada(filaCarregamentoVeiculoEmViagem.Carga, unitOfWorkContainer.UnitOfWork, TipoServicoMultisoftware, Auditado, this.Usuario);
                }
            }

            return servicoFilaCarregamentoVeiculo.Adicionar(conjuntoVeiculo, centroCarregamento, motorista, TipoServicoMultisoftware);
        }

        private string ObterCaminhoArquivoFoto(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });
        }

        private string ObterClasseCorStatusValidade(DateTime? data)
        {
            if (!data.HasValue)
                return "status-validade-azul";

            int diasParaVencer = data.Value.Date.Subtract(DateTime.Now.Date).Days;

            if (diasParaVencer < 0)
                return "status-validade-vermelho ";

            if (diasParaVencer == 0)
                return "status-validade-laranja";

            if (diasParaVencer < 31)
                return "status-validade-amarelo";

            return "status-validade-verde";
        }

        private string ObterFotoBase64(int codigoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivoFoto(unitOfWork);
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigoMotorista}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterStatusValidade(DateTime? data)
        {
            if (!data.HasValue)
                return "Validade não informada";

            int diasParaVencer = data.Value.Date.Subtract(DateTime.Now.Date).Days;

            if (diasParaVencer < 0)
                return "Vencida";

            if (diasParaVencer == 0)
                return "Vence hoje";

            if (diasParaVencer == 1)
                return "Vence em 1 dia";

            if (diasParaVencer < 31)
                return $"Vence em {diasParaVencer} dias";

            return "Válida";
        }

        #endregion
    }
}
