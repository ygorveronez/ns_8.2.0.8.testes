using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PreCargas
{
    [CustomAuthorize(new string[] { "ConsultarPreCargas", "ObterDetalhesPreCarga" }, "PreCargas/PreCargaTransportador")]
    public class PreCargaTransportadorController : BaseController
    {
		#region Construtores

		public PreCargaTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConfirmarPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioOfertaTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (ofertaTransportador == null)
                    throw new ControllerException("Pré planejamento não encontrado.");

                if (ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.AguardandoAceite)
                    throw new ControllerException("A situação do pré planejamento não permite a confirmação.");

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador servicoPreCargaOfertaTransportador = new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ofertaTransportador.PreCargaOferta.PreCarga;

                if ((preCarga.Empresa != null) || ofertaTransportador.Bloqueada)
                    throw new ControllerException($"O pré planejamento não está mais disponível para confirmação.");

                ofertaTransportador.HorarioLimiteConfirmacao = null;
                ofertaTransportador.Situacao = SituacaoPreCargaOfertaTransportador.AguardandoConfirmacao;

                preCarga.Initialize();
                preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoDadosTransporte;

                repositorioPreCarga.Atualizar(preCarga);
                repositorioOfertaTransportador.Atualizar(ofertaTransportador);
                repositorioOfertaTransportador.BloquearTodas(ofertaTransportador.PreCargaOferta.Codigo, ofertaTransportador.Codigo, ofertaTransportador.Tipo);
                servicoPreCargaOfertaTransportador.SalvarHistoricoAlteracao(ofertaTransportador, "Confirmou o pré planejamento.", Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), $"O transportador {ofertaTransportador.Transportador.Descricao} confirmou o pré planejamento", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterOfertaTransportadorRetornar(ofertaTransportador, unitOfWork));
            }
            catch (ControllerException escecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, escecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarPreCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                };

                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino = new List<(int CodigoPreCarga, string Regiao)>();
                List<(int CodigoPreCarga, string Destino)> listaDestino = new List<(int CodigoConfiguracaoProgramacaoCarga, string Destino)>();
                List<(int CodigoPreCarga, string Estado)> listaEstadoDestino = new List<(int CodigoConfiguracaoProgramacaoCarga, string Estado)>();
                List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> listaOfertaTransportador = new List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>();
                int totalRegistros = repositorioOfertaTransportador.ContarConsulta(filtrosPesquisa);

                if (totalRegistros > 0)
                {
                    Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                    Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                    Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);

                    listaOfertaTransportador = repositorioOfertaTransportador.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosPreCarga = listaOfertaTransportador.Select(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.Codigo).ToList();
                    listaDestino = repositorioPreCargaDestino.BuscarPorPreCargas(codigosPreCarga);
                    listaEstadoDestino = repositorioPreCargaEstadoDestino.BuscarPorPreCargas(codigosPreCarga);
                    listaRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarPorPreCargas(codigosPreCarga);
                }

                return new JsonpResult(new
                {
                    PreCargas = (
                        from ofertaTransportador in listaOfertaTransportador
                        select ObterOfertaTransportadorRetornar(ofertaTransportador, listaRegiaoDestino, listaDestino, listaEstadoDestino)
                    ).ToList(),
                    Total = totalRegistros
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os pré planejamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInteressePreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioOfertaTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (ofertaTransportador == null)
                    throw new ControllerException("Pré planejamento não encontrado.");

                if (ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.Disponivel)
                    throw new ControllerException("A situação do pré planejamento não permite a confirmação.");

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador servicoPreCargaOfertaTransportador = new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ofertaTransportador.PreCargaOferta.PreCarga;

                if ((preCarga.Empresa != null) || ofertaTransportador.Bloqueada)
                    throw new ControllerException($"O pré planejamento não está mais disponível para marcar interesse.");

                ofertaTransportador.HorarioLimiteConfirmacao = null;
                ofertaTransportador.Situacao = SituacaoPreCargaOfertaTransportador.AguardandoConfirmacao;

                preCarga.Initialize();
                preCarga.Empresa = ofertaTransportador.Transportador;
                preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoDadosTransporte;

                repositorioPreCarga.Atualizar(preCarga);
                repositorioOfertaTransportador.Atualizar(ofertaTransportador);
                repositorioOfertaTransportador.BloquearTodas(ofertaTransportador.PreCargaOferta.Codigo, ofertaTransportador.Codigo, ofertaTransportador.Tipo);
                servicoPreCargaOfertaTransportador.SalvarHistoricoAlteracao(ofertaTransportador, "Marcou interesse no pré planejamento.", Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), $"O transportador {ofertaTransportador.Transportador.Descricao} marcou interesse no pré planejamento", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterOfertaTransportadorRetornar(ofertaTransportador, unitOfWork));
            }
            catch (ControllerException escecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, escecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao marcar interesse no pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioOfertaTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (ofertaTransportador == null)
                    return new JsonpResult(false, true, "Pré planejamento não encontrado.");

                Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ofertaTransportador.PreCargaOferta.PreCarga;
                Dominio.Entidades.Empresa transportador = preCarga.Empresa ?? this.Usuario.Empresa;
                Dominio.Entidades.Usuario motorista = preCarga.Motoristas?.FirstOrDefault();
                List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarPorPreCarga(preCarga.Codigo);
                List<(int CodigoPreCarga, string Destino)> listaDestino = repositorioPreCargaDestino.BuscarPorPreCarga(preCarga.Codigo);
                List<(int CodigoPreCarga, string Estado)> listaEstadoDestino = repositorioPreCargaEstadoDestino.BuscarPorPreCarga(preCarga.Codigo);
                Dominio.Entidades.Veiculo veiculo = preCarga.Veiculo;
                Dominio.Entidades.Veiculo reboque = preCarga.VeiculosVinculados?.ElementAtOrDefault(0);
                Dominio.Entidades.Veiculo segundoReboque = preCarga.VeiculosVinculados?.ElementAtOrDefault(1);

                var retorno = new
                {
                    ofertaTransportador.Codigo,
                    preCarga.NumeroPreCarga,
                    NumeroReboques = preCarga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                    Transportador = new { transportador.Codigo, transportador.Descricao },
                    DataPrevisaoEntrega = preCarga.DataPrevisaoEntrega?.ToDateTimeString() ?? "",
                    ModeloVeicularCarga = new { Codigo = preCarga.ModeloVeicularCarga?.Codigo ?? 0, Descricao = preCarga.ModeloVeicularCarga?.Descricao ?? string.Empty },
                    Filial = preCarga.Filial?.Descricao ?? "",
                    TipoCarga = preCarga.TipoDeCarga?.Descricao ?? "",
                    TipoOperacao = preCarga.TipoOperacao?.Descricao ?? "",
                    Motorista = new { Codigo = motorista?.Codigo ?? 0, Descricao = motorista?.Nome ?? string.Empty },
                    Veiculo = new { Codigo = veiculo?.Codigo ?? 0, Descricao = veiculo?.Descricao ?? "", Tipo = veiculo != null ? veiculo.Tipo : "P" },
                    Reboque = new { Codigo = reboque?.Codigo ?? 0, Descricao = reboque?.Descricao ?? "" },
                    SegundoReboque = new { Codigo = segundoReboque?.Codigo ?? 0, Descricao = segundoReboque?.Descricao ?? "" },
                    CidadesDestino = string.Join(", ", listaDestino.Where(destino => destino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Destino.Trim())),
                    EstadosDestino = string.Join(", ", listaEstadoDestino.Where(estadoDestino => estadoDestino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Estado.Trim())),
                    RegioesDestino = string.Join(", ", listaRegiaoDestino.Where(regiaoDestino => regiaoDestino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Regiao.Trim())),
                    PermitirEdicaoDadosTransporte = (preCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) && (preCarga.SituacaoPreCarga != SituacaoPreCarga.CargaGerada)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes do pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioOfertaTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (ofertaTransportador == null)
                    throw new ControllerException("Pré planejamento não encontrado.");

                if ((ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.AguardandoAceite) && (ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.AguardandoConfirmacao))
                    throw new ControllerException("A situação do pré planejamento não permite a confirmação.");

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador servicoPreCargaOfertaTransportador = new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ofertaTransportador.PreCargaOferta.PreCarga;

                ofertaTransportador.HorarioLimiteConfirmacao = null;
                ofertaTransportador.Situacao = SituacaoPreCargaOfertaTransportador.Rejeitada;

                preCarga.Initialize();
                preCarga.Empresa = null;
                preCarga.SituacaoPreCarga = SituacaoPreCarga.Nova;

                repositorioPreCarga.Atualizar(preCarga);
                repositorioOfertaTransportador.Atualizar(ofertaTransportador);
                servicoPreCargaOfertaTransportador.SalvarHistoricoAlteracao(ofertaTransportador, "Rejeitou o pré planejamento.", Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), $"O transportador {ofertaTransportador.Transportador.Descricao} rejeitou o pré planejamento", unitOfWork);
                servicoPreCargaOfertaTransportador.DisponibilizarParaTransportadorPorRota(preCarga);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterOfertaTransportadorRetornar(ofertaTransportador, unitOfWork));
            }
            catch (ControllerException escecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, escecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar o pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDadosTransportePreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioOfertaTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (ofertaTransportador == null)
                    throw new ControllerException("Pré planejamento não encontrado.");

                if ((ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.AguardandoAceite) && (ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.AguardandoConfirmacao) && (ofertaTransportador.Situacao != SituacaoPreCargaOfertaTransportador.Confirmada))
                    throw new ControllerException("A situação do pré planejamento não permite a confirmação.");

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaOferta repositorioPreCargaOferta = new Repositorio.Embarcador.PreCargas.PreCargaOferta(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador servicoPreCargaOfertaTransportador = new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ofertaTransportador.PreCargaOferta.PreCarga;

                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoReboque = Request.GetIntParam("Reboque");
                int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");

                List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();
                Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, true);
                Dominio.Entidades.Veiculo primeiroReboque = null;
                Dominio.Entidades.Veiculo segundoReboque = null;

                if (codigoReboque > 0)
                {
                    primeiroReboque = repositorioVeiculo.BuscarPorCodigo(codigoReboque);

                    if (primeiroReboque != null)
                        reboques.Add(primeiroReboque);
                }

                if (codigoSegundoReboque > 0)
                {
                    segundoReboque = repositorioVeiculo.BuscarPorCodigo(codigoSegundoReboque);

                    if (segundoReboque != null && !reboques.Contains(segundoReboque))
                        reboques.Add(segundoReboque);
                }

                if ((veiculo.ModeloVeicularCarga == null) && (preCarga.ModeloVeicularCarga != null))
                {
                    veiculo.ModeloVeicularCarga = preCarga.ModeloVeicularCarga;
                    repositorioVeiculo.Atualizar(veiculo, Auditado);

                    foreach (Dominio.Entidades.Veiculo reboque in reboques)
                    {
                        if (reboque.ModeloVeicularCarga == null)
                        {
                            reboque.ModeloVeicularCarga = preCarga.ModeloVeicularCarga;
                            repositorioVeiculo.Atualizar(reboque);
                        }
                    }
                }

                ofertaTransportador.HorarioLimiteConfirmacao = null;
                ofertaTransportador.Situacao = SituacaoPreCargaOfertaTransportador.Confirmada;
                ofertaTransportador.PreCargaOferta.Situacao = SituacaoPreCargaOferta.Finalizada;

                preCarga.Initialize();
                preCarga.SituacaoPreCarga = (preCarga.Carga == null) ? SituacaoPreCarga.AguardandoGeracaoCarga : SituacaoPreCarga.CargaGerada;
                preCarga.Veiculo = veiculo;
                preCarga.VeiculosVinculados?.Clear();
                preCarga.Motoristas?.Clear();

                if (preCarga.Motoristas == null)
                    preCarga.Motoristas = new List<Dominio.Entidades.Usuario>();
                else
                    preCarga.Motoristas.Clear();

                if (preCarga.VeiculosVinculados == null)
                    preCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                else
                    preCarga.VeiculosVinculados.Clear();

                if (reboques?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in reboques)
                        preCarga.VeiculosVinculados.Add(veiculoVinculado);
                }

                if (motorista != null)
                    preCarga.Motoristas.Add(motorista);

                repositorioPreCarga.Atualizar(preCarga);
                repositorioPreCargaOferta.Atualizar(ofertaTransportador.PreCargaOferta);
                repositorioOfertaTransportador.Atualizar(ofertaTransportador);
                servicoPreCargaOfertaTransportador.SalvarHistoricoAlteracao(ofertaTransportador, "Salvou os dados de transporte do pré planejamento.", Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), $"O transportador {ofertaTransportador.Transportador.Descricao} informou os dados de transporte do pré planejamento", unitOfWork);

                Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, OrigemAlteracaoFilaCarregamento.Transportador);

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo()
                {
                    Tracao = preCarga.Veiculo,
                    Reboques = preCarga.VeiculosVinculados.ToList(),
                    ModeloVeicularCarga = preCarga.ModeloVeicularCarga ?? preCarga.VeiculosVinculados.FirstOrDefault()?.ModeloVeicularCarga ?? preCarga.Veiculo?.ModeloVeicularCarga
                };

                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
                {
                    CodigoFilial = preCarga.Filial.Codigo,
                    CodigoMotorista = preCarga.Motoristas.FirstOrDefault()?.Codigo ?? 0,
                    CodigosDestino = repositorioPreCargaDestino.BuscarCodigosDestinosPorPreCarga(preCarga.Codigo),
                    CodigosRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarCodigosRegioesDestinoPorPreCarga(preCarga.Codigo),
                    ConjuntoVeiculo = conjuntoVeiculo,
                    DataProgramada = preCarga.DataPrevisaoEntrega,
                    SiglasEstadoDestino = repositorioPreCargaEstadoDestino.BuscarSiglasEstadosDestinoPorPreCarga(preCarga.Codigo)
                };

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamento.Adicionar(filaCarregamentoVeiculoAdicionar, TipoServicoMultisoftware);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoAlteradas = servicoFilaCarregamento.AlocarPreCargaManualmente(preCarga, filaCarregamentoVeiculo.Codigo, codigoFilaCarregamentoMotorista: 0, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                servicoFilaCarregamento.NotificarAlteracoes(filasCarregamentoAlteradas);

                return new JsonpResult(ObterOfertaTransportadorRetornar(ofertaTransportador, unitOfWork));
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
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados de transporte do pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador()
            {
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Usuario.Empresa?.Codigo ?? 0,
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                NumeroPreCarga = Request.GetStringParam("NumeroPreCarga")
            };
        }

        private dynamic ObterOfertaTransportadorRetornar(Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoPreCarga = ofertaTransportador.PreCargaOferta.PreCarga.Codigo;
            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);
            List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarPorPreCarga(codigoPreCarga);
            List<(int CodigoPreCarga, string Destino)> listaDestino = repositorioPreCargaDestino.BuscarPorPreCarga(codigoPreCarga);
            List<(int CodigoPreCarga, string Estado)> listaEstadoDestino = repositorioPreCargaEstadoDestino.BuscarPorPreCarga(codigoPreCarga);

            return ObterOfertaTransportadorRetornar(ofertaTransportador, listaRegiaoDestino, listaDestino, listaEstadoDestino);
        }

        private dynamic ObterOfertaTransportadorRetornar(Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador, List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino, List<(int CodigoPreCarga, string Destino)> listaDestino, List<(int CodigoPreCarga, string Estado)> listaEstadoDestino)
        {
            return new
            {
                ofertaTransportador.Codigo,
                ofertaTransportador.Situacao,
                HorarioLimiteConfirmacao = ofertaTransportador.HorarioLimiteConfirmacao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                ofertaTransportador.PreCargaOferta.PreCarga.NumeroPreCarga,
                DataPrevisaoEntrega = ofertaTransportador.PreCargaOferta.PreCarga.DataPrevisaoEntrega?.ToDateTimeString(),
                Filial = ofertaTransportador.PreCargaOferta.PreCarga.Filial?.Descricao,
                ModeloVeicularCarga = ofertaTransportador.PreCargaOferta.PreCarga.ModeloVeicularCarga?.Descricao ?? "",
                TipoCarga = ofertaTransportador.PreCargaOferta.PreCarga.TipoDeCarga?.Descricao ?? "",
                TipoOperacao = ofertaTransportador.PreCargaOferta.PreCarga.TipoOperacao?.Descricao ?? "",
                Motorista = ofertaTransportador.PreCargaOferta.PreCarga.RetornarMotoristas,
                Placas = ofertaTransportador.PreCargaOferta.PreCarga.RetornarPlacas,
                CidadesDestino = string.Join(", ", listaDestino.Where(destino => destino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Destino.Trim())),
                EstadosDestino = string.Join(", ", listaEstadoDestino.Where(estadoDestino => estadoDestino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Estado.Trim())),
                RegioesDestino = string.Join(", ", listaRegiaoDestino.Where(regiaoDestino => regiaoDestino.CodigoPreCarga == ofertaTransportador.PreCargaOferta.PreCarga.Codigo).Select(regiaoDestino => regiaoDestino.Regiao.Trim()))
            };
        }

        #endregion Métodos Privados
    }
}
