using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ProgramacaoCarga
{
    [CustomAuthorize("Cargas/ConfiguracaoProgramacaoCarga")]
    public class ConfiguracaoProgramacaoCargaController : BaseController
    {
		#region Construtores

		public ConfiguracaoProgramacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga();

                PreencherEntidade(configuracaoProgramacaoCarga, unitOfWork);

                repositorioConfiguracaoProgramacaoCarga.Inserir(configuracaoProgramacaoCarga, Auditado);

                AtualizarDestinos(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarEstadosDestino(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarRegioesDestino(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarModelosVeicularesCarga(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarTiposCarga(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarTiposOperacao(configuracaoProgramacaoCarga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga = repositorioConfiguracaoProgramacaoCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o regsitro.");

                PreencherEntidade(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarDestinos(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarEstadosDestino(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarRegioesDestino(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarModelosVeicularesCarga(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarTiposCarga(configuracaoProgramacaoCarga, unitOfWork);
                AtualizarTiposOperacao(configuracaoProgramacaoCarga, unitOfWork);

                repositorioConfiguracaoProgramacaoCarga.Atualizar(configuracaoProgramacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o registro.");
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
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga = repositorioConfiguracaoProgramacaoCarga.BuscarPorCodigo(codigo, auditavel: false);

                if (configuracaoProgramacaoCarga == null)
                    return new JsonpResult(false, "Não foi possível encontrar o regsitro.");

                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino repositorioConfiguracaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino(unitOfWork);
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino repositorioConfiguracaoEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino(unitOfWork);
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino repositorioConfiguracaoRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino(unitOfWork);
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga repositorioConfiguracaoModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga repositorioConfiguracaoEstadoTipoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao repositorioConfiguracaoTipoOperacao = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino> listaConfiguracaoDestino = repositorioConfiguracaoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino> listaConfiguracaoEstadoDestino = repositorioConfiguracaoEstadoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino> listaConfiguracaoRegiaoDestino = repositorioConfiguracaoRegiaoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga> listaConfiguracaoModeloVeicularCarga = repositorioConfiguracaoModeloVeicularCarga.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga> listaConfiguracaoTipoCarga = repositorioConfiguracaoEstadoTipoCarga.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao> listaConfiguracaoTipoOperacao = repositorioConfiguracaoTipoOperacao.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

                return new JsonpResult(new
                {
                    configuracaoProgramacaoCarga.Codigo,
                    configuracaoProgramacaoCarga.Descricao,
                    configuracaoProgramacaoCarga.Ativo,
                    Filial = new { configuracaoProgramacaoCarga.Filial.Codigo, configuracaoProgramacaoCarga.Filial.Descricao },
                    Destino = (
                        from configuracaoDestino in listaConfiguracaoDestino
                        select new
                        {
                            configuracaoDestino.Localidade.Codigo,
                            configuracaoDestino.Localidade.Descricao
                        }
                    ).ToList(),
                    EstadoDestino = (
                        from configuracaoEstadoDestino in listaConfiguracaoEstadoDestino
                        select new
                        {
                            Codigo = configuracaoEstadoDestino.Estado.Sigla,
                            Descricao = configuracaoEstadoDestino.Estado.Nome
                        }
                    ).ToList(),
                    RegiaoDestino = (
                        from configuracaoRegiaoDestino in listaConfiguracaoRegiaoDestino
                        select new
                        {
                            configuracaoRegiaoDestino.Regiao.Codigo,
                            configuracaoRegiaoDestino.Regiao.Descricao
                        }
                    ).ToList(),
                    TipoCarga = (
                        from configuracaoTipoCarga in listaConfiguracaoTipoCarga
                        select new
                        {
                            Codigo = configuracaoTipoCarga.TipoCarga.Codigo,
                            Descricao = configuracaoTipoCarga.TipoCarga.Descricao
                        }
                    ).ToList(),
                    TipoOperacao = (
                        from configuracaoTipoOperacao in listaConfiguracaoTipoOperacao
                        select new
                        {
                            Codigo = configuracaoTipoOperacao.TipoOperacao.Codigo,
                            Descricao = configuracaoTipoOperacao.TipoOperacao.Descricao
                        }
                    ).ToList(),
                    ModeloVeicularCarga = (
                        from configuracaoModeloVeicularCarga in listaConfiguracaoModeloVeicularCarga
                        select new
                        {
                            Codigo = configuracaoModeloVeicularCarga.ModeloVeicularCarga.Codigo,
                            Descricao = configuracaoModeloVeicularCarga.ModeloVeicularCarga.Descricao
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga = repositorioConfiguracaoProgramacaoCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o regsitro.");

                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);
                new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao(unitOfWork).DeletarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

                repositorioConfiguracaoProgramacaoCarga.Deletar(configuracaoProgramacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

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

        #endregion

        #region Métodos Privados

        private void AtualizarDestinos(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaDestinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destino"));
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino repositorioConfiguracaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino> listaConfiguracaoDestino = repositorioConfiguracaoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoDestino.Count > 0)
            {
                List<int> listaCodigosDestinos = new List<int>();

                foreach (var destino in listaDestinos)
                    listaCodigosDestinos.Add(((string)destino.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino> listaConfiguracaoDestinoRemover = (from o in listaConfiguracaoDestino where !listaCodigosDestinos.Contains(o.Localidade.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino configuracaoDestino in listaConfiguracaoDestinoRemover)
                {
                    repositorioConfiguracaoDestino.Deletar(configuracaoDestino);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Destinos",
                        De = configuracaoDestino.Localidade.Descricao,
                        Para = ""
                    });
                }
            }

            foreach (var destino in listaDestinos)
            {
                int codigoDestino = ((string)destino.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino configuracaoDestinoSalvar = (from o in listaConfiguracaoDestino where o.Localidade.Codigo == codigoDestino select o).FirstOrDefault();

                if (configuracaoDestinoSalvar != null)
                    continue;

                configuracaoDestinoSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    Localidade = repositorioLocalidade.BuscarPorCodigo(codigoDestino)
                };

                repositorioConfiguracaoDestino.Inserir(configuracaoDestinoSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Destinos",
                    De = "",
                    Para = configuracaoDestinoSalvar.Localidade.Descricao
                });
            }
        }

        private void AtualizarEstadosDestino(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaEstadosDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadoDestino"));
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino repositorioConfiguracaoEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino> listaConfiguracaoEstadoDestino = repositorioConfiguracaoEstadoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoEstadoDestino.Count > 0)
            {
                List<string> listaUfsEstadosDestino = new List<string>();

                foreach (var estadoDestino in listaEstadosDestino)
                    listaUfsEstadosDestino.Add((string)estadoDestino.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino> listaConfiguracaoEstadoDestinoRemover = (from o in listaConfiguracaoEstadoDestino where !listaUfsEstadosDestino.Contains(o.Estado.Sigla) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino configuracaoEstadoDestino in listaConfiguracaoEstadoDestinoRemover)
                {
                    repositorioConfiguracaoEstadoDestino.Deletar(configuracaoEstadoDestino);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "EstadosDestino",
                        De = configuracaoEstadoDestino.Estado.Nome,
                        Para = ""
                    });
                }
            }

            foreach (var estadoDestino in listaEstadosDestino)
            {
                string ufEstadoDestino = (string)estadoDestino.Codigo;
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino configuracaoEstadoDestinoSalvar = (from o in listaConfiguracaoEstadoDestino where o.Estado.Sigla == ufEstadoDestino select o).FirstOrDefault();

                if (configuracaoEstadoDestinoSalvar != null)
                    continue;

                configuracaoEstadoDestinoSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    Estado = repositorioEstado.BuscarPorSigla(ufEstadoDestino)
                };

                repositorioConfiguracaoEstadoDestino.Inserir(configuracaoEstadoDestinoSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "EstadosDestino",
                    De = "",
                    Para = configuracaoEstadoDestinoSalvar.Estado.Nome
                });
            }
        }

        private void AtualizarRegioesDestino(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaRegioesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegiaoDestino"));
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino repositorioConfiguracaoRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino> listaConfiguracaoRegiaoDestino = repositorioConfiguracaoRegiaoDestino.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoRegiaoDestino.Count > 0)
            {
                List<int> listaCodigosRegioesDestino = new List<int>();

                foreach (var regiaoDestino in listaRegioesDestino)
                    listaCodigosRegioesDestino.Add(((string)regiaoDestino.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino> listaConfiguracaoRegiaoDestinoRemover = (from o in listaConfiguracaoRegiaoDestino where !listaCodigosRegioesDestino.Contains(o.Regiao.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino configuracaoRegiaoDestino in listaConfiguracaoRegiaoDestinoRemover)
                {
                    repositorioConfiguracaoRegiaoDestino.Deletar(configuracaoRegiaoDestino);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "RegioesDestino",
                        De = configuracaoRegiaoDestino.Regiao.Descricao,
                        Para = ""
                    });
                }
            }

            foreach (var regiaoDestino in listaRegioesDestino)
            {
                int codigoRegiaoDestino = ((string)regiaoDestino.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino configuracaoRegiaoDestinoSalvar = (from o in listaConfiguracaoRegiaoDestino where o.Regiao.Codigo == codigoRegiaoDestino select o).FirstOrDefault();

                if (configuracaoRegiaoDestinoSalvar != null)
                    continue;

                configuracaoRegiaoDestinoSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    Regiao = repositorioRegiao.BuscarPorCodigo(codigoRegiaoDestino)
                };

                repositorioConfiguracaoRegiaoDestino.Inserir(configuracaoRegiaoDestinoSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "RegioesDestino",
                    De = "",
                    Para = configuracaoRegiaoDestinoSalvar.Regiao.Descricao
                });
            }
        }

        private void AtualizarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaModelosVeicularesCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModeloVeicularCarga"));
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga repositorioConfiguracaoModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga> listaConfiguracaoModeloVeicularCarga = repositorioConfiguracaoModeloVeicularCarga.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoModeloVeicularCarga.Count > 0)
            {
                List<int> listaCodigosModelosVeicularesCarga = new List<int>();

                foreach (var modeloVeicularCarga in listaModelosVeicularesCarga)
                    listaCodigosModelosVeicularesCarga.Add(((string)modeloVeicularCarga.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga> listaConfiguracaoModeloVeicularCargaRemover = (from o in listaConfiguracaoModeloVeicularCarga where !listaCodigosModelosVeicularesCarga.Contains(o.ModeloVeicularCarga.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga configuracaoModeloVeicularCarga in listaConfiguracaoModeloVeicularCargaRemover)
                {
                    repositorioConfiguracaoModeloVeicularCarga.Deletar(configuracaoModeloVeicularCarga);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "ModelosVeicularesCarga",
                        De = configuracaoModeloVeicularCarga.ModeloVeicularCarga.Descricao,
                        Para = ""
                    });
                }
            }

            foreach (var modeloVeicularCarga in listaModelosVeicularesCarga)
            {
                int codigoModeloVeicularCarga = ((string)modeloVeicularCarga.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga configuracaoModeloVeicularCargaSalvar = (from o in listaConfiguracaoModeloVeicularCarga where o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga select o).FirstOrDefault();

                if (configuracaoModeloVeicularCargaSalvar != null)
                    continue;

                configuracaoModeloVeicularCargaSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga)
                };

                repositorioConfiguracaoModeloVeicularCarga.Inserir(configuracaoModeloVeicularCargaSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "ModelosVeicularesCarga",
                    De = "",
                    Para = configuracaoModeloVeicularCargaSalvar.ModeloVeicularCarga.Descricao
                });
            }
        }

        private void AtualizarTiposCarga(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoCarga"));
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga repositorioConfiguracaoTipoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga> listaConfiguracaoTipoCarga = repositorioConfiguracaoTipoCarga.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoTipoCarga.Count > 0)
            {
                List<int> listaCodigosTiposCarga = new List<int>();

                foreach (var tipoCarga in listaTiposCarga)
                    listaCodigosTiposCarga.Add(((string)tipoCarga.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga> listaConfiguracaoTipoCargaRemover = (from o in listaConfiguracaoTipoCarga where !listaCodigosTiposCarga.Contains(o.TipoCarga.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga configuracaoTipoCarga in listaConfiguracaoTipoCargaRemover)
                {
                    repositorioConfiguracaoTipoCarga.Deletar(configuracaoTipoCarga);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TiposCarga",
                        De = configuracaoTipoCarga.TipoCarga.Descricao,
                        Para = ""
                    });
                }
            }

            foreach (var tipoCarga in listaTiposCarga)
            {
                int codigoTipoCarga = ((string)tipoCarga.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga configuracaoTipoCargaSalvar = (from o in listaConfiguracaoTipoCarga where o.TipoCarga.Codigo == codigoTipoCarga select o).FirstOrDefault();

                if (configuracaoTipoCargaSalvar != null)
                    continue;

                configuracaoTipoCargaSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    TipoCarga = repositorioTipoCarga.BuscarPorCodigo(codigoTipoCarga)
                };

                repositorioConfiguracaoTipoCarga.Inserir(configuracaoTipoCargaSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "TiposCarga",
                    De = "",
                    Para = configuracaoTipoCargaSalvar.TipoCarga.Descricao
                });
            }
        }

        private void AtualizarTiposOperacao(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaTiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoOperacao"));
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao repositorioConfiguracaoTipoOperacao = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao> listaConfiguracaoTipoOperacao = repositorioConfiguracaoTipoOperacao.BuscarPorConfiguracao(configuracaoProgramacaoCarga.Codigo);

            if (listaConfiguracaoTipoOperacao.Count > 0)
            {
                List<int> listaCodigosTiposOperacao = new List<int>();

                foreach (var tipoOperacao in listaTiposOperacao)
                    listaCodigosTiposOperacao.Add(((string)tipoOperacao.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao> listaConfiguracaoTipoOperacaoRemover = (from o in listaConfiguracaoTipoOperacao where !listaCodigosTiposOperacao.Contains(o.TipoOperacao.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao configuracaoTipoOperacao in listaConfiguracaoTipoOperacaoRemover)
                {
                    repositorioConfiguracaoTipoOperacao.Deletar(configuracaoTipoOperacao);

                    configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TiposOperacao",
                        De = configuracaoTipoOperacao.TipoOperacao.Descricao,
                        Para = ""
                    });
                }
            }

            foreach (var tipoOperacao in listaTiposOperacao)
            {
                int codigoTipoOperacao = ((string)tipoOperacao.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao configuracaoTipoOperacaoSalvar = (from o in listaConfiguracaoTipoOperacao where o.TipoOperacao.Codigo == codigoTipoOperacao select o).FirstOrDefault();

                if (configuracaoTipoOperacaoSalvar != null)
                    continue;

                configuracaoTipoOperacaoSalvar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao()
                {
                    ConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga,
                    TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao)
                };

                repositorioConfiguracaoTipoOperacao.Inserir(configuracaoTipoOperacaoSalvar);

                configuracaoProgramacaoCarga.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "TiposOperacao",
                    De = "",
                    Para = configuracaoTipoOperacaoSalvar.TipoOperacao.Descricao
                });
            }
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");

            configuracaoProgramacaoCarga.Ativo = Request.GetBoolParam("Ativo");
            configuracaoProgramacaoCarga.Descricao = Request.GetStringParam("Descricao");
            configuracaoProgramacaoCarga.Filial = (codigoFilial > 0) ? new Repositorio.Embarcador.Filiais.Filial(unitOfWork).BuscarPorCodigo(codigoFilial) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Ativo", "DescricaoAtivo", 10, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
                int totalRegistros = repositorioConfiguracaoProgramacaoCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga> configuracoesProgramacaoCarga = (totalRegistros > 0) ? repositorioConfiguracaoProgramacaoCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga>();

                var configuracoesProgramacaoCargaRetornar = (
                    from o in configuracoesProgramacaoCarga
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.DescricaoAtivo,
                        Filial = o.Filial.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(configuracoesProgramacaoCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion
    }
}
