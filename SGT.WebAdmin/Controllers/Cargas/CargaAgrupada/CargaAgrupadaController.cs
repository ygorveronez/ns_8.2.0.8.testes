using Dominio.Excecoes.Embarcador;
using Servicos.Embarcador.Notificacao;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Cargas.CargaAgrupada
{
    [CustomAuthorize("Cargas/CargaAgrupada", "Cargas/Carga")]
    
    public class CargaAgrupadaController : BaseController
    {
		#region Construtores

		public CargaAgrupadaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AgruparCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("Iniciou consultas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");
                dynamic dynCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CargasAgrupadas"));

                List<int> codigosCarga = new List<int>();
                for (int i = 0; i < dynCargas.Count; i++)
                    codigosCarga.Add((int)dynCargas[i].Codigo);

                if (codigosCarga.Count > 350)
                    return new JsonpResult(false, true, "Não é permitido agrupar mais que 350 cargas.");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarPorCodigos(codigosCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);

                    if (carga.TipoOperacao.ObrigatorioVincularContainerCarga && coletaContainer == null)
                        throw new ControllerException($"Tipo de Operação da Carga exige Container e a Carga {carga.CodigoCargaEmbarcador} não tem número de Container");
                }

                if (cargas.Exists(o => o.PendenteGerarCargaDistribuidor))
                    throw new ControllerException("Não é possível gerar o agrupamento das cargas enquanto o segundo trecho da carga " + string.Join(", ", from obj in cargas where obj.PendenteGerarCargaDistribuidor select obj.CodigoCargaEmbarcador) + " não for gerado.");

                if (cargas.Exists(o => o.CalculandoFrete && (o.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete || o.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador)))
                    throw new ControllerException("Não é possível gerar o agrupamento das cargas enquanto a carga " + string.Join(", ", from obj in cargas where obj.PendenteGerarCargaDistribuidor select obj.CodigoCargaEmbarcador) + " estiver em processo de cálculo de frete, por favor aguarde um momento para tentar novamente.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (configuracaoGeralCarga.PermitirAlterarInformacoesAgrupamentoCarga)
                    cargas.ForEach(carga => AlterarDadosCarga(carga, unitOfWork));

                int codigoCargaAgrupadaAlterar = Request.GetIntParam("CargaAgrupadaAlterar");

                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = codigoCargaAgrupadaAlterar > 0 ? repositorioCarga.BuscarPorCodigo(codigoCargaAgrupadaAlterar) : null; //Aqui é necessário passar a relação da carga agrupada que está sendo editada, se estiver sendo no caso.

                if (cargaAgrupada == null && codigosCarga.Count == 0)
                    throw new ControllerException("É obrigatório informar uma carga.");

                int codigoFilialCargaAgrupada = Request.GetIntParam("FilialCargaAgrupada");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = codigoFilialCargaAgrupada > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilialCargaAgrupada) : null;
                if (codigoFilialCargaAgrupada > 0 && (filial == null || !cargas.Exists(o => o.Filial.Codigo == filial.Codigo)))
                    throw new ControllerException("Filial selecionada para ser a filial da carga agrupada não foi encontrada.");

                cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga).AgruparCargas(cargaAgrupada, cargas, TipoServicoMultisoftware, Cliente, filial);

                if (configuracaoGeralCarga.PermitirAlterarInformacoesAgrupamentoCarga)
                {
                    cargaAgrupada.Rota = cargas.FirstOrDefault().Rota;
                    cargaAgrupada.Empresa = cargas.FirstOrDefault().Empresa;
                    cargaAgrupada.TipoOperacao = cargas.FirstOrDefault().TipoOperacao;

                    new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(cargaAgrupada.Rota);

                    repositorioCarga.Atualizar(cargaAgrupada);
                }

                if (configuracaoGeralCarga.PermitirAgrupamentoDeCargasOrdenavel)
                {
                    for (int i = 0; i < dynCargas.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.Where(x => x.Codigo == (int)dynCargas[i].Codigo).FirstOrDefault();
                        if (carga != null)
                        {
                            int CodDivisaoCapacidade = ((string)dynCargas[i].DivisaoModeloVeicular).ToInt();
                            int Ordem = ((string)dynCargas[i].Ordem).ToInt();

                            if (CodDivisaoCapacidade > 0)
                            {
                                carga.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade { Codigo = CodDivisaoCapacidade };
                                carga.DadosSumarizados.OrdemAgrupamentoDivisao = Ordem;

                                repositorioCarga.Atualizar(carga);
                            }
                        }
                    }
                }

                // Notificar o app da unificação de cargas
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                List<Dominio.Entidades.Usuario> listaMotoristas = repCargaMotorista.BuscarMotoristasPorCarga(cargaAgrupada.Codigo);
                Servicos.Embarcador.Chamado.NotificacaoMobile serNotificacaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                serNotificacaoMobile.NotificarMotoristasUnificacaoCarga(listaMotoristas, new
                {
                    NovaCargaCodigo = cargaAgrupada.Codigo
                });

                // Envia para o novo app MTrack
                NotificacaoMTrack serNotificacoMTrack = new NotificacaoMTrack(unitOfWork);
                serNotificacoMTrack.NotificarMudancaCarga(cargaAgrupada, listaMotoristas, AdminMultisoftware.Dominio.Enumeradores.MobileHubs.UnificacaoCarga);

                if (cargaAgrupada.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false)
                {
                    bool possuiColetaContainer = false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);

                        if (coletaContainer == null)
                        {
                            possuiColetaContainer = false;
                            break;
                        }
                        else
                            possuiColetaContainer = true;
                    }

                    if (possuiColetaContainer)
                    {
                        cargaAgrupada.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                        repositorioCarga.Atualizar(cargaAgrupada);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAgrupada, null, "Criada pelo agrupamento das cargas " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao agrupar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarModeloVeicular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                carga.ModeloVeicularOrigem = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular);

                repositorioCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    ModeloVeicularOrigem = carga.ModeloVeicularOrigem?.Descricao ?? ""
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCargasAgrupadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridCargasAgrupadas();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
                {
                    SomenteAgrupadas = true,
                    CodigoCargaEmbarcador = Request.GetStringParam("NumeroCarga"),
                    SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica,
                    CpfCnpjRecebedoresOuSemRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                    CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork)
                };

                int totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = totalRegistros > 0 ? repositorioCarga.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta()) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaResultado = (from o in cargasAgrupadas
                                      select new
                                      {
                                          o.Codigo,
                                          Descricao = o.CodigoCargaEmbarcador,
                                          Carga = o.CodigoCargaEmbarcador,
                                          Veiculo = o.Veiculo?.Placa ?? "",
                                          Empresa = o.Empresa?.Descricao ?? ""
                                      }).ToList();

                grid.AdicionaRows(listaResultado);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar cargas agrupadas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCargasDoAgrupamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDoAgrupamento = repositorioCarga.BuscarCargasOriginais(Request.GetIntParam("CodigoCarga"));

                return new JsonpResult((from cargaAgrupamento in cargasDoAgrupamento
                                        select new
                                        {
                                            cargaAgrupamento.Codigo,
                                            cargaAgrupamento.CodigoCargaEmbarcador,
                                            OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(cargaAgrupamento, false, TipoServicoMultisoftware),
                                            ModeloVeicularOrigem = cargaAgrupamento.ModeloVeicularOrigem?.Descricao ?? cargaAgrupamento.ModeloVeicularCarga?.Descricao ?? "",
                                            Veiculo = !string.IsNullOrWhiteSpace(cargaAgrupamento.PlacaDeAgrupamento) ? cargaAgrupamento.PlacaDeAgrupamento : (cargaAgrupamento.Veiculo != null ? cargaAgrupamento.Veiculo.Placa : ""),
                                            EmpresaCodigo = cargaAgrupamento.Empresa?.Codigo ?? 0,
                                            RaizCNPJEmpresa = cargaAgrupamento.Empresa?.RaizCnpj
                                        }).ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar cargas do agrupamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDivisoesCapacidadeVeicular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicular.BuscarPorCodigo(Request.GetIntParam("codigoVeiculo"));

                return new JsonpResult(new
                {
                    DivisaoCapacidadeVeicular = ConverterObjetoListaDivisaoCapacidade(modeloVeicularCarga)
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar cargas do agrupamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> RelatorioControleDivisoesCapacidade()
        {
            var result = ReportRequest.WithType(ReportType.CargaAgrupadaDivisao)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("Codigo", Request.GetIntParam("Codigo").ToString())
                .CallReport();
            var pdfBytes = result.GetContentFile();
            return Arquivo(pdfBytes, "application/pdf", result.FileName);
        }

        #endregion

        #region Métodos Privados
        
        private void AlterarDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            carga.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoCarga"));
            carga.Rota = repositorioRotaFrete.BuscarPorCodigo(Request.GetIntParam("RotaCarga"));
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                carga.Empresa = repositorioEmpresa.BuscarPorCodigo(Request.GetIntParam("TransportadorCarga"));
            carga.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCargaCarga"));

            repositorioCarga.Atualizar(carga);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Alterou dados ao agrupar a carga", unitOfWork);
        }

        private Models.Grid.Grid ObterGridCargasAgrupadas()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular> ConverterObjetoListaDivisaoCapacidade(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular> divisoesCapacidade = new List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular>();

            if (modeloVeicularCarga == null)
                return divisoesCapacidade;

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade in modeloVeicularCarga.DivisoesCapacidade.ToList())
            {

                Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular divisaoCapacidadeModelo = new Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular();
                divisaoCapacidadeModelo.Capacidade = modeloVeicularCargaDivisaoCapacidade.Quantidade;
                divisaoCapacidadeModelo.Codigo = modeloVeicularCargaDivisaoCapacidade.Codigo;
                divisaoCapacidadeModelo.Descricao = modeloVeicularCargaDivisaoCapacidade.Descricao;
                divisaoCapacidadeModelo.Coluna = modeloVeicularCargaDivisaoCapacidade.Coluna;
                divisaoCapacidadeModelo.Piso = modeloVeicularCargaDivisaoCapacidade.Piso;
                divisaoCapacidadeModelo.UnidadeDeMedida = new Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida()
                {
                    Codigo = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Codigo ?? 0,
                    UnidadeMedida = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN,
                    Descricao = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Descricao ?? ""
                };

                divisoesCapacidade.Add(divisaoCapacidadeModelo);

            }

            return divisoesCapacidade;
        }

        #endregion
    }
}
