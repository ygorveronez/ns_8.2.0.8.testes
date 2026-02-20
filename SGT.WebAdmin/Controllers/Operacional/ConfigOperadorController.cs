using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Operacional
{
    [CustomAuthorize("Operacional/ConfigOperador", "Logistica/JanelaCarregamento")]
    public class ConfigOperadorController : BaseController
    {
		#region Construtores

		public ConfigOperadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorOperador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = Request.GetIntParam("Codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorFilial repOperadorFilial = new Repositorio.Embarcador.Operacional.OperadorFilial(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorTipoCarga repOperadorTipoCarga = new Repositorio.Embarcador.Operacional.OperadorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorTipoCargaModeloVeicular repOperadorTipoCargaModeloVeicular = new Repositorio.Embarcador.Operacional.OperadorTipoCargaModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorCliente repOperadorCliente = new Repositorio.Embarcador.Operacional.OperadorCliente(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);
                if (operadorLogistica == null)
                    return new JsonpResult(false, true, Localization.Resources.Operacional.ConfigOperador.UsuarioInformadoNaoOperadorLogistica);

                List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> operadorFiliais = repOperadorFilial.BuscarPorOperador(operadorLogistica.Codigo);
                List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga> operadorTiposCarga = repOperadorTipoCarga.BuscarPorOperador(operadorLogistica.Codigo);
                List<Dominio.Entidades.Embarcador.Operacional.OperadorCliente> operadorClientes = repOperadorCliente.BuscarPorOperador(operadorLogistica.Codigo);

                List<dynamic> dynOperadorTiposCarga = new List<dynamic>();
                foreach (Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga operadorTipoCarga in operadorTiposCarga)
                {
                    List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular> operadorTipoCargaModelosVeicular = repOperadorTipoCargaModeloVeicular.BuscarPorOperadorTipoCarga(operadorTipoCarga.Codigo);

                    var dynOperadorTipoCarga = new
                    {
                        Codigo = operadorTipoCarga.Codigo,
                        TipoCarga = new { operadorTipoCarga.TipoDeCarga.Codigo, operadorTipoCarga.TipoDeCarga.Descricao },
                        OperadorTipoCargaModelosVeicular = (from obj in operadorTipoCargaModelosVeicular
                                                            select new
                                                            {
                                                                obj.Codigo,
                                                                ModeloVeicularCarga = new { obj.ModeloVeicularCarga.Codigo, obj.ModeloVeicularCarga.Descricao }
                                                            }).ToList()
                    };
                    dynOperadorTiposCarga.Add(dynOperadorTipoCarga);
                }

                var dynOperadorLogistica = new
                {
                    CodigoOperadorLogistica = operadorLogistica.Codigo,
                    operadorLogistica.Usuario.Codigo,
                    operadorLogistica.SupervisorLogistica,
                    operadorLogistica.PossuiFiltroGrupoPessoas,
                    operadorLogistica.VisualizaCargasSemGrupoPessoas,
                    operadorLogistica.PossuiFiltroTipoOperacao,
                    operadorLogistica.PermiteSelecionarHorarioEncaixe,
                    operadorLogistica.TelaPedidosResumido,
                    operadorLogistica.VisualizaCargasSemTipoOperacao,
                    operadorLogistica.RegraRecebedorSeraSobrepostaNasDemais,
                    OperadorLogistica = new { operadorLogistica.Usuario.Codigo, Descricao = operadorLogistica.Usuario.Nome },
                    OperadorFiliais = (from obj in operadorFiliais
                                       select new
                                       {
                                           obj.Codigo,
                                           Filial = new { obj.Filial.Codigo, obj.Filial.Descricao }
                                       }).ToList(),
                    TiposOperacao = (from obj in operadorLogistica.TipoOperacoes
                                     select new
                                     {
                                         Operacao = new { obj.Codigo, obj.Descricao }
                                     }).ToList(),
                    OperadorTiposCarga = dynOperadorTiposCarga,
                    GrupoPessoas = (from obj in operadorLogistica.GrupoPessoas
                                    orderby obj.Descricao
                                    select new
                                    {
                                        Grupo = new { obj.Codigo, obj.Descricao }
                                    }).ToList(),
                    CentrosCarregamento = (from obj in operadorLogistica.CentrosCarregamento
                                           select new
                                           {
                                               obj.Codigo,
                                               obj.Descricao
                                           }).ToList(),
                    CentrosDescarregamento = (from obj in operadorLogistica.CentrosDescarregamento
                                              select new
                                              {
                                                  obj.Codigo,
                                                  obj.Descricao
                                              }).ToList(),
                    OperadorClientes = (from obj in operadorClientes
                                        select new
                                        {
                                            obj.Codigo,
                                            Cliente = new { obj.Cliente.Codigo, obj.Cliente.Descricao }
                                        }).ToList(),
                    OperadorTomadores = (from obj in operadorLogistica.Tomadores
                                         select new
                                         {
                                             Codigo = obj.CPF_CNPJ,
                                             Tomador = new { Codigo = obj.CPF_CNPJ, obj.Descricao }
                                         }).ToList(),
                    FiliaisVenda = (from obj in operadorLogistica.FiliaisVenda
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Descricao
                                    }).ToList(),
                    Transportadores = (from obj in operadorLogistica.Transportadores
                                       select new
                                       {
                                           obj.Codigo,
                                           obj.Descricao
                                       }).ToList(),
                    Recebedores = (from obj in operadorLogistica.Recebedores
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao
                                   }).ToList(),
                    Expedidores = (from obj in operadorLogistica.Expedidores
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao
                                   }).ToList(),
                    TipoTransportador = (from obj in operadorLogistica.TiposTransportadorCentroCarregamento
                                         select new
                                         {
                                             text = obj.ObterDescricao(),
                                             value = obj
                                         }).ToList(),
                    Vendedores = (from obj in operadorLogistica.Vendedores
                                  select new
                                       {
                                           obj.Codigo,
                                           obj.Descricao
                                       }).ToList(),
                };

                return new JsonpResult(dynOperadorLogistica);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Operacional.ConfigOperador.OcorreuFalhaBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTipoTransportadorConfigurado()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorFilial repOperadorFilial = new Repositorio.Embarcador.Operacional.OperadorFilial(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorTipoCarga repOperadorTipoCarga = new Repositorio.Embarcador.Operacional.OperadorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorTipoCargaModeloVeicular repOperadorTipoCargaModeloVeicular = new Repositorio.Embarcador.Operacional.OperadorTipoCargaModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorCliente repOperadorCliente = new Repositorio.Embarcador.Operacional.OperadorCliente(unitOfWork);

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
                if (operadorLogistica == null)
                    return new JsonpResult(false, true, Localization.Resources.Operacional.ConfigOperador.UsuarioInformadoNaoOperadorLogistica);

                var retorno = (from obj in operadorLogistica.TiposTransportadorCentroCarregamento
                               select new
                               {
                                   value = obj,
                                   text = obj.ObterDescricao()
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Operacional.ConfigOperador.OcorreuFalhaBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(Request.GetIntParam("Codigo"));

                if (operadorLogistica == null)
                    throw new ControllerException(Localization.Resources.Operacional.ConfigOperador.UsuarioInformadoNaoOperadorLogistica);

                operadorLogistica.Initialize();

                operadorLogistica.VisualizaCargasSemGrupoPessoas = Request.GetBoolParam("VisualizaCargasSemGrupoPessoas");
                operadorLogistica.PossuiFiltroGrupoPessoas = Request.GetBoolParam("PossuiFiltroGrupoPessoas");

                operadorLogistica.VisualizaCargasSemTipoOperacao = Request.GetBoolParam("VisualizaCargasSemTipoOperacao");
                operadorLogistica.PossuiFiltroTipoOperacao = Request.GetBoolParam("PossuiFiltroTipoOperacao");

                operadorLogistica.PermiteSelecionarHorarioEncaixe = Request.GetBoolParam("PermiteSelecionarHorarioEncaixe");
                operadorLogistica.TelaPedidosResumido = Request.GetBoolParam("TelaPedidosResumido");
                operadorLogistica.RegraRecebedorSeraSobrepostaNasDemais = Request.GetBoolParam("RegraRecebedorSeraSobrepostaNasDemais");

                //Lista de Set
                SalvarGrupoDePessoas(operadorLogistica, unitOfWork);
                SalvarTiposOperacao(operadorLogistica, unitOfWork);
                SalvarCentrosCarregamento(operadorLogistica, unitOfWork);
                SalvarCentrosDescarregamento(operadorLogistica, unitOfWork);
                SalvarFiliaisVenda(operadorLogistica, unitOfWork);
                SalvarTomadores(operadorLogistica, unitOfWork);
                SalvarTransportadores(operadorLogistica, unitOfWork);
                SalvarRecebedores(operadorLogistica, unitOfWork);
                SalvarExpedidores(operadorLogistica, unitOfWork);
                SalvarTipoTransportador(operadorLogistica);
                SalvarVendedores(operadorLogistica, unitOfWork);

                //Lista de Bag - Não utilizar dessa forma
                SalvarFilial(operadorLogistica, unitOfWork);
                SalvarTipoCarga(operadorLogistica, unitOfWork);
                SalvarClientes(operadorLogistica, unitOfWork);

                repOperadorLogistica.Atualizar(operadorLogistica);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, operadorLogistica, operadorLogistica.GetChanges(), Localization.Resources.Operacional.ConfigOperador.AtualizouOperador, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Operacional.ConfigOperador.OcorreuFalhaAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarFilial(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Operacional.OperadorFilial repOperadorFilial = new Repositorio.Embarcador.Operacional.OperadorFilial(unidadeTrabalho);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeTrabalho);

            dynamic dynFiliaisVenda = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OperadorFiliais"));

            List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> operadoresFiliais = repOperadorFilial.BuscarPorOperador(operadorLogistica.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            if (operadoresFiliais.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynFilialVenda in dynFiliaisVenda)
                    if (dynFilialVenda.Codigo != null)
                        codigos.Add((int)dynFilialVenda.Codigo);

                List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> operadoresFiliaisDeletar = (from obj in operadoresFiliais where !codigos.Contains(obj.OperadorLogistica.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilialDeletar in operadoresFiliaisDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Filial",
                        De = operadorFilialDeletar.Filial.Descricao,
                        Para = ""
                    });

                    repOperadorFilial.Deletar(operadorFilialDeletar);
                }
            }

            foreach (var dynFilialVenda in dynFiliaisVenda)
            {
                int codigoFilial = ((string)dynFilialVenda.Filial.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilial = codigoFilial > 0 ? repOperadorFilial.BuscarPorOperadorEFilial(operadorLogistica.Codigo, codigoFilial) : null;

                if (operadorFilial == null)
                {
                    operadorFilial = new Dominio.Entidades.Embarcador.Operacional.OperadorFilial()
                    {
                        Filial = repFilial.BuscarPorCodigo(codigoFilial),
                        OperadorLogistica = operadorLogistica
                    };
                    repOperadorFilial.Inserir(operadorFilial);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Filial",
                        De = "",
                        Para = operadorFilial.Filial.Descricao
                    });
                }
            }

            operadorLogistica.SetExternalChanges(alteracoes);
        }

        private void SalvarTipoCarga(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Operacional.OperadorTipoCarga repOperadorTipoCarga = new Repositorio.Embarcador.Operacional.OperadorTipoCarga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);

            dynamic dynTiposDeCargaVenda = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OperadorTiposCarga"));

            List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga> operadoresTipoDeCargas = repOperadorTipoCarga.BuscarPorOperador(operadorLogistica.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            if (operadoresTipoDeCargas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynTipoDeCarga in dynTiposDeCargaVenda)
                    if (dynTipoDeCarga.Codigo != null)
                        codigos.Add((int)dynTipoDeCarga.Codigo);

                List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga> operadoresTiposDeCargasDeletar = (from obj in operadoresTipoDeCargas where !codigos.Contains(obj.OperadorLogistica.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga operadorTipoDeCargaDeletar in operadoresTiposDeCargasDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoCarga",
                        De = operadorTipoDeCargaDeletar.TipoDeCarga.Descricao,
                        Para = ""
                    });

                    repOperadorTipoCarga.Deletar(operadorTipoDeCargaDeletar);
                }
            }

            foreach (var dynTipoDeCarga in dynTiposDeCargaVenda)
            {
                int codigoTipoDeCarga = ((string)dynTipoDeCarga.TipoCarga.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga operadorTipoCarga = codigoTipoDeCarga > 0 ? repOperadorTipoCarga.BuscarPorOperadorETipoDeCarga(operadorLogistica.Codigo, codigoTipoDeCarga) : null;

                if (operadorTipoCarga == null)
                {
                    operadorTipoCarga = new Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga()
                    {
                        TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga),
                        OperadorLogistica = operadorLogistica
                    };
                    repOperadorTipoCarga.Inserir(operadorTipoCarga);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoCarga",
                        De = "",
                        Para = operadorTipoCarga.TipoDeCarga.Descricao
                    });
                }
            }

            operadorLogistica.SetExternalChanges(alteracoes);
        }

        private void SalvarClientes(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Operacional.OperadorCliente repOperadorCliente = new Repositorio.Embarcador.Operacional.OperadorCliente(unidadeTrabalho);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unidadeTrabalho);

            dynamic dynClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("OperadorClientes"));

            List<Dominio.Entidades.Embarcador.Operacional.OperadorCliente> operadoresClientes = repOperadorCliente.BuscarPorOperador(operadorLogistica.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            if (operadoresClientes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynCliente in dynClientes)
                    if (dynCliente.Codigo != null)
                        codigos.Add((int)dynCliente.Codigo);

                List<Dominio.Entidades.Embarcador.Operacional.OperadorCliente> operadoresClientesDeletar = (from obj in operadoresClientes where !codigos.Contains(obj.OperadorLogistica.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Operacional.OperadorCliente operadorClienteDeletar in operadoresClientesDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Cliente",
                        De = operadorClienteDeletar.Cliente.Descricao,
                        Para = ""
                    });

                    repOperadorCliente.Deletar(operadorClienteDeletar);
                }
            }

            foreach (var dynCliente in dynClientes)
            {
                double cpfcnpjCliente = ((double)dynCliente.Cliente.Codigo);

                Dominio.Entidades.Embarcador.Operacional.OperadorCliente operadorCliente = cpfcnpjCliente > 0 ? repOperadorCliente.BuscarPorOperadorECliente(operadorLogistica.Codigo, cpfcnpjCliente) : null;

                if (operadorCliente == null)
                {
                    operadorCliente = new Dominio.Entidades.Embarcador.Operacional.OperadorCliente()
                    {
                        Cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfcnpjCliente),
                        OperadorLogistica = operadorLogistica
                    };
                    repOperadorCliente.Inserir(operadorCliente);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Cliente",
                        De = "",
                        Para = operadorCliente.Cliente.Descricao
                    });
                }
            }

            operadorLogistica.SetExternalChanges(alteracoes);
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            if (operadorLogistica.TipoOperacoes == null)
                operadorLogistica.TipoOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
                operadorLogistica.TipoOperacoes.Clear();

            dynamic dynTiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            foreach (dynamic dynTipoOperacao in dynTiposOperacao)
                operadorLogistica.TipoOperacoes.Add(repTipoOperacao.BuscarPorCodigo((int)dynTipoOperacao.Codigo));
        }

        private void SalvarGrupoDePessoas(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

            if (operadorLogistica.GrupoPessoas == null)
                operadorLogistica.GrupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            else
                operadorLogistica.GrupoPessoas.Clear();

            dynamic dynGrupoPessoas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GrupoPessoas"));

            foreach (dynamic dynGrupoPessoa in dynGrupoPessoas)
                operadorLogistica.GrupoPessoas.Add(repGrupoPessoas.BuscarPorCodigo((int)dynGrupoPessoa.Codigo));
        }

        private void SalvarCentrosCarregamento(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);

            dynamic centrosCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosCarregamento"));

            if (operadorLogistica.CentrosCarregamento == null)
            {
                operadorLogistica.CentrosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroCarregamento in centrosCarregamento)
                    codigos.Add((int)centroCarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosDeletar = operadorLogistica.CentrosCarregamento.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroDeletar in centrosDeletar)
                    operadorLogistica.CentrosCarregamento.Remove(centroDeletar);
            }

            foreach (var centroCarregamento in centrosCarregamento)
                if (!operadorLogistica.CentrosCarregamento.Any(o => o.Codigo == (int)centroCarregamento.Codigo))
                    operadorLogistica.CentrosCarregamento.Add(repCentroCarregamento.BuscarPorCodigo((int)centroCarregamento.Codigo));
        }

        private void SalvarTipoTransportador(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica)
        {
            dynamic tipoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoTransportador"));

            if (operadorLogistica.TiposTransportadorCentroCarregamento != null && operadorLogistica.TiposTransportadorCentroCarregamento.Count > 0)
            {
                List<TipoTransportadorCentroCarregamento> enumeradoresNaoRemover = new List<TipoTransportadorCentroCarregamento>();

                foreach (var item in tipoTransportador)
                    enumeradoresNaoRemover.Add((TipoTransportadorCentroCarregamento)item);

                List<TipoTransportadorCentroCarregamento> enumeradoresRemover = operadorLogistica.TiposTransportadorCentroCarregamento.Where(x => !enumeradoresNaoRemover.Contains(x)).ToList();

                foreach (var itemRemover in enumeradoresRemover)
                    operadorLogistica.TiposTransportadorCentroCarregamento.Remove(itemRemover);
            }
            else
                operadorLogistica.TiposTransportadorCentroCarregamento = new List<TipoTransportadorCentroCarregamento>();

            foreach (var tipo in tipoTransportador)
            {
                if (operadorLogistica.TiposTransportadorCentroCarregamento.Contains((TipoTransportadorCentroCarregamento)tipo))
                    continue;

                operadorLogistica.TiposTransportadorCentroCarregamento.Add((TipoTransportadorCentroCarregamento)tipo);
            }
        }
        
        private void SalvarCentrosDescarregamento(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);

            dynamic CentrosDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosDescarregamento"));

            if (operadorLogistica.CentrosDescarregamento == null)
            {
                operadorLogistica.CentrosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroDescarregamento in CentrosDescarregamento)
                    codigos.Add((int)centroDescarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDeletar = operadorLogistica.CentrosDescarregamento.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDeletar in centrosDeletar)
                    operadorLogistica.CentrosDescarregamento.Remove(centroDeletar);
            }

            foreach (var centroDescarregamento in CentrosDescarregamento)
                if (!operadorLogistica.CentrosDescarregamento.Any(o => o.Codigo == (int)centroDescarregamento.Codigo))
                    operadorLogistica.CentrosDescarregamento.Add(repCentroDescarregamento.BuscarPorCodigo((int)centroDescarregamento.Codigo));
        }

        private void SalvarFiliaisVenda(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

            if (operadorLogistica.FiliaisVenda == null)
                operadorLogistica.FiliaisVenda = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            else
                operadorLogistica.FiliaisVenda.Clear();

            dynamic dynFiliaisVenda = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FiliaisVenda"));

            foreach (var dynFilialVenda in dynFiliaisVenda)
                operadorLogistica.FiliaisVenda.Add(repFilial.BuscarPorCodigo((int)dynFilialVenda.Codigo));
        }

        private void SalvarTomadores(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            dynamic jOperadorTomadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("OperadorTomadores"));

            if (operadorLogistica.Tomadores == null)
                operadorLogistica.Tomadores = new List<Dominio.Entidades.Cliente>();
            else
                operadorLogistica.Tomadores.Clear();

            foreach (dynamic jOperadorTomador in jOperadorTomadores)
            {
                Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ((double)jOperadorTomador.Tomador.Codigo);
                operadorLogistica.Tomadores.Add(tomador);
            }
        }

        private void SalvarTransportadores(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            if (operadorLogistica.Transportadores == null)
                operadorLogistica.Transportadores = new List<Dominio.Entidades.Empresa>();
            else
                operadorLogistica.Transportadores.Clear();

            dynamic dynTransportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            foreach (dynamic dynTransportador in dynTransportadores)
                operadorLogistica.Transportadores.Add(repEmpresa.BuscarPorCodigo((int)dynTransportador.Codigo));
        }

        private void SalvarRecebedores(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (operadorLogistica.Recebedores == null)
                operadorLogistica.Recebedores = new List<Dominio.Entidades.Cliente>();
            else
                operadorLogistica.Recebedores.Clear();

            dynamic dynRecebedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Recebedores"));

            foreach (dynamic dynRecebedor in dynRecebedores)
                operadorLogistica.Recebedores.Add(repCliente.BuscarPorCPFCNPJ((double)dynRecebedor.Codigo));
        }

        private void SalvarExpedidores(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (operadorLogistica.Expedidores == null)
                operadorLogistica.Expedidores = new List<Dominio.Entidades.Cliente>();
            else
                operadorLogistica.Expedidores.Clear();

            dynamic dynExpedidores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Expedidores"));

            foreach (dynamic dynExpedidor in dynExpedidores)
                operadorLogistica.Expedidores.Add(repCliente.BuscarPorCPFCNPJ((double)dynExpedidor.Codigo));
        }

        private void SalvarVendedores(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            if (operadorLogistica.Vendedores == null)
                operadorLogistica.Vendedores = new List<Dominio.Entidades.Usuario>();
            else
                operadorLogistica.Vendedores.Clear();

            dynamic dynVendedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Vendedores"));

            foreach (dynamic dynVendedor in dynVendedores)
                operadorLogistica.Vendedores.Add(repositorioUsuario.BuscarPorCodigo((int)dynVendedor.Codigo));
        }

        #endregion
    }
}
