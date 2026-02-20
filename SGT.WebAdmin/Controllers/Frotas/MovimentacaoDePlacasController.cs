using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize(new string[] { "PesquisaVeiculos", "ImprimirMovimentacaoDePlacas" }, "Frotas/MovimentacaoDePlacas")]
    public class MovimentacaoDePlacasController : BaseController
    {
		#region Construtores

		public MovimentacaoDePlacasController(Conexao conexao) : base(conexao) { }

        #endregion


        #region Metodos Publicos

        public async Task<IActionResult> InformarMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);

            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codMotorista = int.Parse(Request.Params("Motorista"));
                int.TryParse(Request.Params("SegmentoVeiculo"), out int codSegmentoVeiculo);
                bool substituir = Request.GetBoolParam("Substituir");

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();



                if (veiculo != null)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codMotorista, true);
                    Dominio.Entidades.Veiculo veiculoAntigo = repVeiculo.BuscarPorMotorista(codMotorista);

                    if (veiculoAntigo != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculoAntigo.Codigo);
                        repVeiculo.Atualizar(veiculoAntigo);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculoAntigo);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoAntigo, null, "Motorista " + (motorista?.Descricao ?? "") + " removido da placa " + veiculoAntigo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        if (motorista != null)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, "Motorista " + (motorista?.Descricao ?? "") + " removido da placa " + veiculoAntigo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                    }

                    if (motorista != null)
                    {
                        if (!configuracaoVeiculo.BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca)
                            motorista.CentroResultado = veiculo.CentroResultado;

                        repUsuario.Atualizar(motorista, Auditado);
                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotoristaPrincipal = null;
                        bool contemMotoristaPrincipal = false;

                        if (substituir)
                        {
                            veiculoMotoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                            contemMotoristaPrincipal = true;
                        }
                        else
                            contemMotoristaPrincipal = repVeiculoMotorista.ContemMotoristaPrincipal(veiculo.Codigo);

                        if (veiculoMotoristaPrincipal == null)
                            veiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();

                        bool trocouMotorista = false;
                        if (veiculoMotoristaPrincipal.Motorista?.Codigo != motorista?.Codigo && substituir)
                            trocouMotorista = true;

                        veiculoMotoristaPrincipal.CPF = motorista.CPF;
                        veiculoMotoristaPrincipal.Motorista = motorista;
                        veiculoMotoristaPrincipal.Nome = motorista.Nome;
                        veiculoMotoristaPrincipal.Veiculo = veiculo;
                        veiculoMotoristaPrincipal.Principal = !contemMotoristaPrincipal || substituir ? true : false;

                        if (veiculoMotoristaPrincipal.Codigo == 0)
                            repVeiculoMotorista.Inserir(veiculoMotoristaPrincipal);
                        else
                            repVeiculoMotorista.Atualizar(veiculoMotoristaPrincipal);

                        if (codSegmentoVeiculo > 0)
                            veiculo.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codSegmentoVeiculo);

                        var dynMotorista = BuscarDynMotorista(motorista, substituir || !contemMotoristaPrincipal);

                        repVeiculo.Atualizar(veiculo, Auditado);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);
                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);

                        if (trocouMotorista)
                            Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoesTrocaMotorista(veiculo, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Motorista " + motorista.Descricao + " vinculado a placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, "Motorista " + motorista.Descricao + " vinculado a placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);

                        if (substituir || !contemMotoristaPrincipal)
                        {
                            if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                            {
                                foreach (var reboque in veiculo.VeiculosVinculados)
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, $"Removido motorista principal.", unitOfWork);
                                    repVeiculoMotorista.DeletarMotoristaPrincipal(reboque.Codigo);

                                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista reboqueMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                                    {
                                        CPF = motorista.CPF,
                                        Motorista = motorista,
                                        Nome = motorista.Nome,
                                        Veiculo = reboque,
                                        Principal = true
                                    };

                                    repVeiculoMotorista.Inserir(reboqueMotoristaPrincipal, Auditado);

                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Motorista " + motorista.Descricao + " vinculado a placa " + reboque.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                                }
                            }
                        }

                        servicoFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                        unitOfWork.CommitChanges();

                        return new JsonpResult(dynMotorista);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Motorista não encontrado");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Veículo não encontrado");
                }
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> InformarReboque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);

            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codReboque = int.Parse(Request.Params("Reboque"));
                int.TryParse(Request.Params("SegmentoVeiculo"), out int codSegmentoVeiculo);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

                if (veiculo != null)
                {
                    Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(codReboque, true);

                    if (ConfiguracaoEmbarcador.ValidarProprietarioVeiculoMovimentacaoPlaca)
                    {
                        if (reboque.Tipo == "T" && veiculo.Tipo == "T" && reboque.Proprietario != null && veiculo.Proprietario != null && reboque.Proprietario.CPF_CNPJ != veiculo.Proprietario.CPF_CNPJ)
                        {
                            return new JsonpResult(true, false, "O proprietário do reboque e da tração não são os mesmos.");
                        }
                    }

                    Dominio.Entidades.Veiculo veiculoAntigo = repVeiculo.BuscarPorReboque(codReboque);
                    if (veiculoAntigo != null)
                    {
                        veiculoAntigo.VeiculosVinculados.Remove(reboque);
                        repVeiculo.Atualizar(veiculoAntigo);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculoAntigo);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoAntigo, null, "Reboque " + reboque.Placa_Formatada + " removido da placa " + veiculoAntigo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Reboque " + reboque.Placa_Formatada + " removido da placa " + veiculoAntigo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                    }

                    if (reboque != null && !veiculo.VeiculosVinculados.Contains(reboque))
                    {
                        //reboque.Motorista = veiculo.Motorista;
                        //reboque.NomeMotorista = veiculo.Motorista?.Nome ?? string.Empty;
                        //reboque.CPFMotorista = veiculo.Motorista?.CPF ?? string.Empty;

                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                        if (veiculoMotorista != null)
                        {
                            //deletar e criar motorista principal para reboque (antes estava setando motorista do reboque de acordo com do veiculo)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, $"Removido motorista principal.", unitOfWork);
                            repVeiculoMotorista.DeletarMotoristaPrincipal(reboque.Codigo);
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaVeiculoReboque = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                            {
                                CPF = veiculoMotorista.CPF,
                                Motorista = veiculoMotorista.Motorista,
                                Nome = veiculoMotorista.Nome,
                                Veiculo = reboque,
                                Principal = true
                            };

                            repVeiculoMotorista.Inserir(motoristaVeiculoReboque);
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = veiculo.CentroResultado;

                        if (!configuracaoVeiculo.BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca)
                            reboque.CentroResultado = veiculo.CentroResultado;

                        reboque.FuncionarioResponsavel = veiculo.FuncionarioResponsavel;
                        repVeiculo.Atualizar(reboque, Auditado);

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                            historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                            if (historicoVeiculoVinculo == null)
                            {
                                historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                {
                                    Veiculo = veiculo,
                                    DataHora = DateTime.Now,
                                    Usuario = Usuario,
                                    KmRodado = veiculo.KilometragemAtual,
                                    KmAtualModificacao = 0,
                                    DiasVinculado = 0
                                };
                                repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                            }

                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                            {
                                HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                CentroResultado = veiculo.CentroResultado,
                                DataHora = DateTime.Now,
                            };

                            repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                        }

                        veiculo.VeiculosVinculados.Add(reboque);
                        if (codSegmentoVeiculo > 0)
                            veiculo.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codSegmentoVeiculo);
                        var dynVeiculo = BuscarDynVeiculos(reboque);

                        repVeiculo.Atualizar(veiculo, Auditado);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);
                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);
                        servicoFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Reboque " + reboque.Placa_Formatada + " vinculado da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Reboque " + reboque.Placa_Formatada + " vinculado da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);

                        if (veiculo.FuncionarioResponsavel != null)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Alterado funcionario Responsavel na Movimentação de placas", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(dynVeiculo);
                    }
                    else if (reboque != null && veiculo.VeiculosVinculados.Contains(reboque))
                    {
                        //reboque.Motorista = veiculo.Motorista;
                        //reboque.NomeMotorista = veiculo.Motorista?.Nome ?? string.Empty;
                        //reboque.CPFMotorista = veiculo.Motorista?.CPF ?? string.Empty;

                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                        if (veiculoMotorista != null)
                        {
                            //deletar e criar motorista principal para reboque (antes estava setando motorista do reboque de acordo com do veiculo)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, $"Removido motorista principal.", unitOfWork);
                            repVeiculoMotorista.DeletarMotoristaPrincipal(reboque.Codigo);
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaVeiculoReboque = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                            {
                                CPF = veiculoMotorista.CPF,
                                Motorista = veiculoMotorista.Motorista,
                                Nome = veiculoMotorista.Nome,
                                Veiculo = reboque,
                                Principal = true
                            };

                            repVeiculoMotorista.Inserir(motoristaVeiculoReboque);
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = veiculo.CentroResultado;

                        reboque.CentroResultado = veiculo.CentroResultado;
                        repVeiculo.Atualizar(reboque, Auditado);

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                            historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                            if (historicoVeiculoVinculo == null)
                            {
                                historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                {
                                    Veiculo = veiculo,
                                    DataHora = DateTime.Now,
                                    Usuario = Usuario,
                                    KmRodado = veiculo.KilometragemAtual,
                                    KmAtualModificacao = 0,
                                    DiasVinculado = 0
                                };
                                repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                            }

                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                            {
                                HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                CentroResultado = veiculo.CentroResultado,
                                DataHora = DateTime.Now,
                            };

                            repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                        }

                        var dynVeiculo = BuscarDynVeiculos(reboque);
                        return new JsonpResult(dynVeiculo);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Reboque não encontrado");
                    }

                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Veículo não encontrado");
                }
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> AlterarCentroResultado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);

            try
            {
                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codCentroResultado = int.Parse(Request.Params("CentroResultado"));
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorCodigo(codCentroResultado);

                if (veiculo != null && centroResultado != null)
                {
                    unitOfWork.Start();
                    Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = veiculo.CentroResultado;

                    veiculo.CentroResultado = centroResultado;
                    if (veiculoMotorista != null)
                    {
                        veiculoMotorista.CentroResultado = centroResultado;
                        repUsuario.Atualizar(veiculoMotorista);

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                            historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                            if (historicoVeiculoVinculo == null)
                            {
                                historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                {
                                    Veiculo = veiculo,
                                    DataHora = DateTime.Now,
                                    Usuario = Usuario,
                                    KmRodado = veiculo.KilometragemAtual,
                                    KmAtualModificacao = 0,
                                    DiasVinculado = 0
                                };
                                repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                            }

                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                            {
                                HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                CentroResultado = veiculo.CentroResultado,
                                DataHora = DateTime.Now,
                            };

                            repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                        }
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoMotorista, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                    }
                    if (veiculo.Equipamentos != null)
                    {
                        foreach (var equipamento in veiculo.Equipamentos)
                        {
                            equipamento.CentroResultado = centroResultado;
                            repEquipamento.Atualizar(equipamento);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, equipamento, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                        }
                    }
                    if (veiculo.VeiculosVinculados != null)
                    {
                        foreach (var reboque in veiculo.VeiculosVinculados)
                        {
                            if (reboque.Equipamentos != null)
                            {
                                foreach (var equipamento in reboque.Equipamentos)
                                {
                                    equipamento.CentroResultado = centroResultado;
                                    repEquipamento.Atualizar(equipamento);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, equipamento, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                                }
                            }
                            reboque.CentroResultado = centroResultado;
                            repVeiculo.Atualizar(reboque);

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                                historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                                if (historicoVeiculoVinculo == null)
                                {
                                    historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                    {
                                        Veiculo = veiculo,
                                        DataHora = DateTime.Now,
                                        Usuario = Usuario,
                                        KmRodado = veiculo.KilometragemAtual,
                                        KmAtualModificacao = 0,
                                        DiasVinculado = 0
                                    };
                                    repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                                }

                                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                                {
                                    HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                    CentroResultado = veiculo.CentroResultado,
                                    DataHora = DateTime.Now,
                                };

                                repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                        }
                    }

                    repVeiculo.Atualizar(veiculo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, true, "Sucesso");
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Veículo não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o centro de resultado.");
            }
        }

        public async Task<IActionResult> AlterarGestor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            #region Repositorios
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            //Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);
            #endregion

            try
            {
                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codGestor = int.Parse(Request.Params("GestorSelecionado"));

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);

                if (veiculo != null && codGestor>0)
                {
                    unitOfWork.Start();

                    var gestor = repUsuario.BuscarPorCodigo(codGestor);

                    
                    

                    if (gestor != null)
                    {
                        veiculo.FuncionarioResponsavel = gestor;

                        repVeiculo.Atualizar(veiculo);

                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                        if (veiculoMotorista != null)
                        {
                            veiculoMotorista.Gestor = gestor;

                            repUsuario.Atualizar(veiculoMotorista);
                        }

                        /*
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();

                            historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                            if (historicoVeiculoVinculo == null)
                            {
                                historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                {
                                    Veiculo = veiculo,
                                    DataHora = DateTime.Now,
                                    Usuario = Usuario,
                                    KmRodado = veiculo.KilometragemAtual,
                                    KmAtualModificacao = 0,
                                    DiasVinculado = 0
                                };
                                repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                            }

                            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                            {
                                HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                CentroResultado = veiculo.CentroResultado,
                                DataHora = DateTime.Now,
                            };

                            repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                        }
                        */

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Alterou o gestor do veiculo para [" + gestor.Descricao + "] pela Movimentação de Placas", unitOfWork);
                    }
                    
                    /*
                    if (veiculo.Equipamentos != null)
                    {
                        foreach (var equipamento in veiculo.Equipamentos)
                        {
                            equipamento.CentroResultado = centroResultado;

                            repEquipamento.Atualizar(equipamento);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, equipamento, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                        }
                    }
                    */

                    if (veiculo.VeiculosVinculados != null)
                    {
                        foreach (var reboque in veiculo.VeiculosVinculados)
                        {
                            /*
                            if (reboque.Equipamentos != null)
                            {
                                foreach (var equipamento in reboque.Equipamentos)
                                {
                                    equipamento.CentroResultado = centroResultado;

                                    repEquipamento.Atualizar(equipamento);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, equipamento, null, "Alterou centro de resultado para " + centroResultado.Descricao + " pela Movimentação de Placas", unitOfWork);
                                }
                            }
                            */

                            //reboque.CentroResultado = centroResultado;
                            reboque.FuncionarioResponsavel = gestor;

                            repVeiculo.Atualizar(reboque);

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                                historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                                if (historicoVeiculoVinculo == null)
                                {
                                    historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                    {
                                        Veiculo = veiculo,
                                        DataHora = DateTime.Now,
                                        Usuario = Usuario,
                                        KmRodado = veiculo.KilometragemAtual,
                                        KmAtualModificacao = 0,
                                        DiasVinculado = 0
                                    };
                                    repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                                }

                                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                                {
                                    HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                    CentroResultado = veiculo.CentroResultado,
                                    DataHora = DateTime.Now,
                                };

                                repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Alterou Funcionário responsavel para [" + gestor.Descricao + "] pela Movimentação de Placas", unitOfWork);
                        }
                    }

                    repVeiculo.Atualizar(veiculo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Alterou funcionário responavel  para [" + gestor.Descricao + "] pela Movimentação de Placas", unitOfWork);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, true, "Sucesso");
                }
                else
                {
                    unitOfWork.Rollback();
                }

                return new JsonpResult(false, true, "Gestor não encontrado");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o Gestor da composição.");
            }
        }

        public async Task<IActionResult> RemoverMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codMotorista = int.Parse(Request.Params("Motorista"));
                int.TryParse(Request.Params("SegmentoVeiculo"), out int codSegmentoVeiculo);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);

                if (veiculo != null)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codMotorista);
                    Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    if (motorista != null)
                    {
                        if (veiculoMotorista != null)
                        {
                            Dominio.Entidades.Usuario motoristaAntigo = veiculoMotorista;
                            motoristaAntigo.DataRemocaoVinculo = DateTime.Now;
                            repUsuario.Atualizar(motoristaAntigo);
                        }

                        bool motoristaPrincipal = repVeiculoMotorista.EMotoristaPrincipal(codVeiculo, codMotorista);

                        //veiculo.Motorista = null;
                        //veiculo.NomeMotorista = null;
                        //veiculo.CPFMotorista = null;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaVeiculo(veiculo.Codigo, motorista.Codigo);
                        if (codSegmentoVeiculo > 0)
                            veiculo.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codSegmentoVeiculo);

                        var dynMotorista = BuscarDynMotorista(motorista, motoristaPrincipal);

                        repVeiculo.Atualizar(veiculo, Auditado);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);
                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);
                        servicoFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, "Motorista " + (motorista?.Descricao ?? "") + " removido da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Motorista " + (motorista?.Descricao ?? "") + " removido da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);


                        if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                        {
                            foreach (var reboque in veiculo.VeiculosVinculados)
                            {
                                //reboque.Initialize();
                                //reboque.Motorista = null;
                                //reboque.NomeMotorista = null;
                                //reboque.CPFMotorista = null;
                                //repVeiculo.Atualizar(reboque, Auditado);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, $"Removido motorista principal.", unitOfWork);
                                repVeiculoMotorista.DeletarMotoristaVeiculo(reboque.Codigo, motorista.Codigo);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Motorista " + motorista.Descricao + " removido a placa " + reboque.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                            }
                        }

                        unitOfWork.CommitChanges();
                        return new JsonpResult(dynMotorista);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Motorista não encontrado");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Veículo não encontrado");
                }
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> RemoverReboque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codVeiculo = int.Parse(Request.Params("Veiculo"));
                int codReboque = int.Parse(Request.Params("Reboque"));
                int.TryParse(Request.Params("SegmentoVeiculo"), out int codSegmentoVeiculo);
                int kmAtual = 0;
                int.TryParse(Request.Params("KMAtual"), out kmAtual);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo, true);

                if (veiculo != null)
                {
                    Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(codReboque);
                    if (reboque != null && veiculo.VeiculosVinculados.Contains(reboque))
                    {
                        reboque.DataRemocaoVinculo = DateTime.Now;
                        //reboque.Motorista = null;
                        //reboque.NomeMotorista = null;
                        //reboque.CPFMotorista = null;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(reboque.Codigo);
                        repVeiculo.Atualizar(reboque);

                        veiculo.VeiculosVinculados.Remove(reboque);
                        var dynVeiculo = BuscarDynVeiculos(reboque);
                        if (codSegmentoVeiculo > 0)
                            veiculo.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codSegmentoVeiculo);
                        repVeiculo.Atualizar(veiculo, Auditado);

                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);
                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);
                        servicoFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Reboque " + reboque.Placa_Formatada + " removido da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, reboque, null, "Reboque " + reboque.Placa_Formatada + " removido da placa " + veiculo.Placa_Formatada + " na Movimentação de Placas", unitOfWork);

                        decimal qtdKMRodado = 0;
                        //atualiza km do veículo e seus pneus

                        if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        {
                            if (veiculo != null && veiculo.KilometragemAtual < kmAtual)
                            {
                                qtdKMRodado = (decimal)kmAtual - (decimal)veiculo.KilometragemAtual;

                                if (veiculo.Pneus != null && veiculo.Pneus.Count > 0 && qtdKMRodado > 0)
                                {
                                    foreach (var eixo in veiculo.Pneus)
                                    {
                                        Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                        if (pneu != null)
                                        {
                                            pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                            if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                                pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                            repPneu.Atualizar(pneu);
                                        }
                                    }
                                }
                                if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                                    veiculo.KilometragemAtual = (int)kmAtual;
                                repVeiculo.Atualizar(veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Movimentação de Placas");
                            }

                            //atualiza km dos reboques e seus pneus
                            if (reboque != null && qtdKMRodado > 0)
                            {
                                if (reboque.Pneus != null && reboque.Pneus.Count > 0)
                                {
                                    foreach (var eixo in reboque.Pneus)
                                    {
                                        Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                        if (pneu != null)
                                        {
                                            pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                            if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                                pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                            repPneu.Atualizar(pneu);
                                        }
                                    }
                                }
                                reboque.KilometragemAtual += (int)qtdKMRodado;
                                repVeiculo.Atualizar(reboque, Auditado, null, "Atualizada a Quilometragem Atual do Reboque via Movimentação de Placas");
                            }

                        }
                        unitOfWork.CommitChanges();
                        return new JsonpResult(dynVeiculo);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Reboque não encontrado");
                    }

                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Veículo não encontrado");
                }
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisaVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //int codigoPlaca = int.Parse(Request.Params("Placa"));
                int codigoPlaca = 0;
                string placaVeiculo = Request.Params("PlacaVeiculo");
                if (!string.IsNullOrWhiteSpace(placaVeiculo))
                    placaVeiculo = placaVeiculo.Replace("_", "");
                string numeroFrota = Request.Params("NumeroFrota");
                int motorista = 0;
                int.TryParse(Request.Params("Motorista"), out motorista);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);


                if (!string.IsNullOrWhiteSpace(numeroFrota) || codigoPlaca > 0 || motorista > 0 || !string.IsNullOrWhiteSpace(placaVeiculo))
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(codigoPlaca, numeroFrota, motorista, "0", placaVeiculo);
                    if (veiculo != null)
                    {
                        List<dynamic> dynRetorno = new List<dynamic>();

                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                        List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristas = repVeiculoMotorista.BuscarMotoristaPorVeiculo(veiculo.Codigo);

                        var dynVeiculo = new
                        {
                            TipoVeiculo = "Tração",
                            veiculo.Codigo,
                            DescricaoTipoVeiculo = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Descricao : string.Empty,
                            DescricaoRenavan = veiculo.Renavam,
                            DescricaoSegmentoVeiculo = veiculo.SegmentoVeiculo?.Descricao ?? "",
                            DescricaoModelo = veiculo.Modelo != null ? veiculo.Modelo.Descricao : string.Empty,
                            DescricaoMarca = veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty,
                            NumeroFrota = veiculo.NumeroFrota,
                            Placa = veiculo.Placa,
                            Equipamentos = veiculo.Equipamentos != null ? string.Join(", ", veiculo.Equipamentos.Select(c => c.Descricao).ToList()) : string.Empty,
                            CentroResultado = veiculo.CentroResultado?.Descricao,
                            SegmentoVeiculo = veiculo.SegmentoVeiculo != null ? new { Codigo = veiculo.SegmentoVeiculo.Codigo, Descricao = veiculo.SegmentoVeiculo.Descricao } : null,
                            KMAtual = veiculo.KilometragemAtual,
                            FuncionarioResponsavel = veiculo.FuncionarioResponsavel?.Nome,
                            Reboques = veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count() > 0 && veiculo.VeiculosVinculados != null ? (from obj in veiculo.VeiculosVinculados
                                                                                                                                                             select new
                                                                                                                                                             {
                                                                                                                                                                 obj.Codigo,
                                                                                                                                                                 DescricaoTipoVeiculo = obj.ModeloVeicularCarga != null ? obj.ModeloVeicularCarga.Descricao : string.Empty,
                                                                                                                                                                 DescricaoRenavan = obj.Renavam,
                                                                                                                                                                 DescricaoSegmentoVeiculo = obj.SegmentoVeiculo?.Descricao ?? "",
                                                                                                                                                                 DescricaoModelo = obj.Modelo != null ? obj.Modelo.Descricao : string.Empty,
                                                                                                                                                                 DescricaoMarca = obj.Marca != null ? obj.Marca.Descricao : string.Empty,
                                                                                                                                                                 NumeroFrota = obj.NumeroFrota,
                                                                                                                                                                 Placa = obj.Placa,
                                                                                                                                                                 Equipamentos = obj.Equipamentos != null ? string.Join(", ", obj.Equipamentos.Select(c => c.Descricao).ToList()) : string.Empty,
                                                                                                                                                                 CentroResultado = obj.CentroResultado?.Descricao,
                                                                                                                                                                 QuantidadePaletes = veiculo.ModeloVeicularCarga?.NumeroPaletes ?? 0,
                                                                                                                                                                 FuncionarioResponsavel = veiculo.FuncionarioResponsavel?.Nome
                                                                                                                                                             }).ToList() : null,
                            Motoristas = motoristas != null && motoristas.Count > 0 ? (from obj in motoristas
                                                                                       select new
                                                                                       {
                                                                                           Codigo = obj.Motorista != null ? obj.Motorista.Codigo : obj.Codigo,
                                                                                           CPF = obj.Motorista != null ? obj.Motorista.CPF_Formatado: obj.CPF,
                                                                                           obj.Nome,
                                                                                           DataVencimentoHabilitacao = obj.Motorista != null ? obj.Motorista.DataVencimentoHabilitacao != null ? obj.Motorista.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty :string.Empty,
                                                                                           Telefone = obj.Motorista != null ? obj.Motorista.Telefone : string.Empty,
                                                                                           Celular = obj.Motorista != null ? obj.Motorista.Celular : string.Empty,
                                                                                           NumeroHabilitacao = obj.Motorista != null ?  obj.Motorista.NumeroHabilitacao : string.Empty,
                                                                                           DescricaoCidadeEstado = obj.Motorista != null ?  obj.Motorista.Localidade != null ? obj.Motorista.Localidade.DescricaoCidadeEstado : string.Empty : string.Empty,
                                                                                           CentroResultado = obj.Motorista != null ? obj.Motorista.CentroResultado?.Descricao ?? string.Empty : string.Empty,
                                                                                           Principal = obj.Principal,
                                                                                           DescricaoPrincipal = obj.Principal ? "Sim" : "Não",
                                                                                           Gestor = obj.Motorista?.Gestor?.Nome
                                                                                       }).ToList() : null,

                            Motorista = veiculoMotorista != null ? new
                            {
                                Codigo = veiculoMotorista.Codigo,
                                CPF = veiculoMotorista.CPF_Formatado,
                                veiculoMotorista.Nome,
                                DataVencimentoHabilitacao = veiculoMotorista.DataVencimentoHabilitacao != null ? veiculoMotorista.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                veiculoMotorista.Telefone,
                                veiculoMotorista.Celular,
                                veiculoMotorista.NumeroHabilitacao,
                                DescricaoCidadeEstado = veiculoMotorista.Localidade != null ? veiculoMotorista.Localidade.DescricaoCidadeEstado : string.Empty,
                                CentroResultado = veiculoMotorista.CentroResultado?.Descricao ?? string.Empty,
                                Principal = true,
                                DescricaoPrincipal = "Sim",
                                Gestor = veiculoMotorista.Gestor?.Nome
                            } : null
                        };
                        dynRetorno.Add(dynVeiculo);
                        return new JsonpResult(dynRetorno);
                    }
                    else
                    {
                        veiculo = repVeiculo.BuscarPorPlacaETipoVeiculo(0, numeroFrota, placaVeiculo, "1");
                        if (veiculo != null)
                        {
                            List<dynamic> dynRetorno = new List<dynamic>();

                            var dynVeiculo = new
                            {
                                TipoVeiculo = "Reboque",
                                veiculo.Codigo,
                                DescricaoTipoVeiculo = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Descricao : string.Empty,
                                DescricaoRenavan = veiculo.Renavam,
                                DescricaoSegmentoVeiculo = veiculo.SegmentoVeiculo?.Descricao ?? "",
                                DescricaoModelo = veiculo.Modelo != null ? veiculo.Modelo.Descricao : string.Empty,
                                DescricaoMarca = veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty,
                                NumeroFrota = veiculo.NumeroFrota,
                                Placa = veiculo.Placa,
                                Equipamentos = veiculo.Equipamentos != null ? string.Join(", ", veiculo.Equipamentos.Select(c => c.Descricao).ToList()) : string.Empty,
                                CentroResultado = veiculo.CentroResultado?.Descricao,
                                QuantidadePaletes = veiculo.ModeloVeicularCarga?.NumeroPaletes ?? 0,
                                FuncionarioResponsavel = veiculo.FuncionarioResponsavel?.Nome
                            };
                            dynRetorno.Add(dynVeiculo);
                            return new JsonpResult(dynRetorno);
                        }
                        return new JsonpResult(null);
                    }
                }
                else
                {
                    return new JsonpResult(null);
                }

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

        public async Task<IActionResult> InformarEquipamentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);

                List<int> codigosEquipamentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Equipamentos"));

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Veículo não encontrado.");

                unidadeTrabalho.Start();

                if (veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0)
                {
                    int qtd = veiculo.Equipamentos.Count;
                    for (var i = 0; i < qtd; i++)
                        veiculo.Equipamentos.Remove(veiculo.Equipamentos[0]);
                }
                else
                    veiculo.Equipamentos = new List<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
                foreach (int codigoEquipamento in codigosEquipamentos)
                {
                    Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);
                    veiculo.Equipamentos.Add(equipamento);
                }

                repVeiculo.Atualizar(veiculo);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao informar os novos equipamentos");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirMovimentacaoDePlacas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                int codigoPlaca = Request.GetIntParam("Placa");
                string placaVeiculo = Request.GetStringParam("PlacaVeiculo");
                string numeroFrota = Request.GetStringParam("NumeroFrota");
                int motorista = Request.GetIntParam("Motorista");
                int tipoArquivo = Request.GetIntParam("tipoArquivo");

                if (!string.IsNullOrWhiteSpace(placaVeiculo))
                    placaVeiculo = placaVeiculo.Replace("_", "");

                Dominio.Entidades.Veiculo veiculo = null;
                if (!string.IsNullOrWhiteSpace(numeroFrota) || motorista > 0 || !string.IsNullOrWhiteSpace(placaVeiculo))
                {
                    veiculo = repositorioVeiculo.BuscarPorPlaca(0, numeroFrota, motorista, "0", placaVeiculo);

                    if (veiculo == null)
                        veiculo = repositorioVeiculo.BuscarPorPlacaETipoVeiculo(0, numeroFrota, placaVeiculo, "1");
                }

                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Frotas.MovimentacaoDePlacas servicoMovimentacaoDePlacas = new Servicos.Embarcador.Frotas.MovimentacaoDePlacas(unitOfWork);

                byte[] arquivo = servicoMovimentacaoDePlacas.RelatorioMovimentacaoDePlacas(veiculo, TipoServicoMultisoftware, tipoArquivo);

                string extensaoArquivo = tipoArquivo == 1 ? ".pdf" : ".xls";
                string tipoApplication = tipoArquivo == 1 ? "pdf" : "xls";

                return Arquivo(arquivo, $"application/{tipoApplication}", $"Movimentacao_De_Placas_{veiculo.Placa}{extensaoArquivo}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao imprimir a movimentação de placas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarKmAtualVeiculoTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int reb = Request.GetIntParam("Reboque");
                int vei = Request.GetIntParam("Veiculo");
                int tipo = Request.GetIntParam("TipoMovimento");
                int km = Request.GetIntParam("KmAtual");
                string msgErro = "";

                unitOfWork.Start();

                Servicos.Embarcador.Veiculo.CalculoKmReboque calculoKmReboque = new Servicos.Embarcador.Veiculo.CalculoKmReboque(unitOfWork);
                
                Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(vei);
                Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(reb);
                
                calculoKmReboque.CalcularKmReboque(veiculo, reboque, (TipoMovimentoKmReboque)tipo, km, false, null, out msgErro, unitOfWork, Auditado);

                if (msgErro == "")
                {
                    Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque historicoVinculoKmReboque = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque
                    {
                        Veiculo = veiculo,
                        Reboque = reboque,
                        KMAtual = km,
                        TipoMovimento = (TipoMovimentoKmReboque)tipo,
                        DataCriacao = DateTime.Now
                    };

                    repHistoricoVinculoKmReboque.Inserir(historicoVinculoKmReboque);

                    unitOfWork.CommitChanges();
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, msgErro);
                }
                return new JsonpResult(true);                
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a KM Atual da Tração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private List<dynamic> BuscarDynMotorista(Dominio.Entidades.Usuario usuario, bool motoristaPrincipal)
        {
            List<dynamic> dynVeiculos = new List<dynamic>();

            var dynVeiculo = new
            {
                Codigo = usuario.Codigo,
                CPF = usuario.CPF_Formatado,
                Nome = usuario.Nome,
                DataVencimentoHabilitacao = usuario.DataVencimentoHabilitacao != null ? usuario.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                usuario.Telefone,
                usuario.Celular,
                usuario.NumeroHabilitacao,
                DescricaoCidadeEstado = usuario.Localidade != null ? usuario.Localidade.DescricaoCidadeEstado : string.Empty,
                CentroResultado = usuario.CentroResultado?.Descricao ?? string.Empty,
                Principal = motoristaPrincipal,
                DescricaoPrincipal = motoristaPrincipal ? "Sim" : "Não",
                Gestor = usuario.Gestor?.Nome
            };

            dynVeiculos.Add(dynVeiculo);

            return dynVeiculos;
        }

        private List<dynamic> BuscarDynVeiculos(Dominio.Entidades.Veiculo veiculo)
        {
            List<dynamic> dynVeiculos = new List<dynamic>();

            var dynVeiculo = new
            {
                Codigo = veiculo.Codigo,
                DescricaoTipoVeiculo = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Descricao : string.Empty,
                DescricaoRenavan = veiculo.Renavam,
                DescricaoSegmentoVeiculo = veiculo.SegmentoVeiculo?.Descricao ?? "",
                DescricaoModelo = veiculo.Modelo != null ? veiculo.Modelo.Descricao : string.Empty,
                DescricaoMarca = veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty,
                NumeroFrota = veiculo.NumeroFrota,
                Placa = veiculo.Placa,
                Equipamentos = veiculo.Equipamentos != null ? string.Join(", ", veiculo.Equipamentos.Select(c => c.Descricao).ToList()) : string.Empty,
                CentroResultado = veiculo.CentroResultado?.Descricao,
                QuantidadePaletes = veiculo.ModeloVeicularCarga?.NumeroPaletes ?? 0,
                FuncionarioResponsavel = veiculo.FuncionarioResponsavel?.Nome
            };

            dynVeiculos.Add(dynVeiculo);

            return dynVeiculos;
        }
        #endregion

    }
}
