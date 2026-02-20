using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/SelecaoWMS")]
    public class SelecaoWMSController : BaseController
    {
		#region Construtores

		public SelecaoWMSController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                int intSituacao, codigoFuncionario;
                int.TryParse(Request.Params("SituacaoCarregamento"), out intSituacao);
                int.TryParse(Request.Params("Funcionario"), out codigoFuncionario);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao situacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao)intSituacao;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoSelecaoSeparacao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 25, Models.Grid.Align.center, true);

                List<Dominio.Entidades.Embarcador.WMS.Selecao> selecoes = repSelecao.Consultar(dataInicio, dataFim, codigoFuncionario, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSelecao.ContarConsulta(dataInicio, dataFim, codigoFuncionario, situacao));
                var lista = (from p in selecoes
                             select new
                             {
                                 p.Codigo,
                                 p.DescricaoSituacaoSelecaoSeparacao,
                                 Data = p.Data.ToString("dd/MM/yyyy")
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);

                int codigoEmpresa, codigoFilial, codigoOrigem, codigoDestino, inicio, limite, codigoSelecao;
                double cnpjRemetente, cnpjDestinatario;

                int.TryParse(Request.Params("Codigo"), out codigoSelecao);
                int.TryParse(Request.Params("Inicio"), out inicio);
                int.TryParse(Request.Params("Limite"), out limite);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);

                double.TryParse(Request.Params("Remetente"), out cnpjRemetente);
                double.TryParse(Request.Params("Destinatario"), out cnpjDestinatario);

                string numeroPedidoEmbarcaodor = Request.Params("CodigoPedidoEmbarcador");
                string numeroCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repSelecao.ConsultarCarga(codigoSelecao, dataInicio, dataFim, codigoEmpresa, codigoFilial, codigoOrigem, codigoDestino, cnpjRemetente, cnpjDestinatario, numeroPedidoEmbarcaodor, numeroCargaEmbarcador, "Codigo", "desc", inicio, limite, TipoServicoMultisoftware);
                int quantidade = repSelecao.ContarConsultaCarga(codigoSelecao, dataInicio, dataFim, codigoEmpresa, codigoFilial, codigoOrigem, codigoDestino, cnpjRemetente, cnpjDestinatario, numeroPedidoEmbarcaodor, numeroCargaEmbarcador, TipoServicoMultisoftware);

                var retorno = new
                {
                    Quantidade = quantidade,
                    Registros = (from obj in carga select serCarga.ObterDetalhesCargaParaCarregamento(obj, unitOfWork)).ToList()
                };
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> BuscarPorCodigoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);
                return new JsonpResult(serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> SalvarSelecao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);
                Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                dynamic dynSelecao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Selecao"));

                int codigo = (int)dynSelecao.Codigo;

                bool inserir = false;
                Dominio.Entidades.Embarcador.WMS.Selecao selecao = null;
                if (codigo > 0)
                {
                    selecao = repSelecao.BuscarPorCodigo(codigo, true);
                    if (selecao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A seleção informada não foi localizado");
                    }
                }
                else
                {
                    inserir = true;
                    selecao = new Dominio.Entidades.Embarcador.WMS.Selecao();

                    selecao.Data = DateTime.Now.Date;
                    selecao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente;
                    selecao.Usuario = this.Usuario;
                }

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;
                if (inserir)
                    repSelecao.Inserir(selecao, Auditado);
                else
                    historicoObjeto = repSelecao.Atualizar(selecao, Auditado);

                if (!SalvarListaFuncionarios(selecao, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique os funcionários selecionados.");
                }
                if (!SalvarCargas(selecao, dynSelecao.Cargas, historicoObjeto, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique as cargas selecionadas.");
                }

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    selecao.Codigo,
                    Situacao = selecao.SituacaoSelecaoSeparacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.Selecao selecao = repSelecao.BuscarPorCodigo(codigo);
                return new JsonpResult(ObterSelecao(selecao, unitOfWork));
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

        public async Task<IActionResult> EnviarSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.Separacao repSeparacao = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);
                Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                dynamic dynSelecao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Selecao"));

                int codigo = (int)dynSelecao.Codigo;

                Dominio.Entidades.Embarcador.WMS.Selecao selecao = null;
                if (codigo > 0)
                {
                    selecao = repSelecao.BuscarPorCodigo(codigo, true);
                    if (selecao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A seleção informada não foi localizado");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A seleção informada não foi localizado");
                }

                selecao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Enviada;
                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;
                historicoObjeto = repSelecao.Atualizar(selecao, Auditado);

                if (!SalvarListaFuncionarios(selecao, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique os funcionários selecionados.");
                }
                if (!SalvarCargas(selecao, dynSelecao.Cargas, historicoObjeto, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique as cargas selecionadas.");
                }


                Dominio.Entidades.Embarcador.WMS.Separacao separacao = new Dominio.Entidades.Embarcador.WMS.Separacao();
                separacao.Selecao = selecao;
                separacao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente;

                repSeparacao.Inserir(separacao);

                if (!SalvarFuncionariosSeparacao(separacao, selecao, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique os funcionários selecionados.");
                }
                string retornoProduto = "";
                if (!SalvarProdutosEmbarcador(separacao, selecao, unitOfWork, ref retornoProduto))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não foi possível enviar a separação dos produtos: " + retornoProduto);
                }
                if (!SalvarCargas(separacao, selecao, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Falha ao salvar as cargas.");
                }

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    selecao.Codigo,
                    Situacao = selecao.SituacaoSelecaoSeparacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> CancelarSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.Separacao repSeparacao = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);
                Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                dynamic dynSelecao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Selecao"));

                int codigo = (int)dynSelecao.Codigo;

                Dominio.Entidades.Embarcador.WMS.Selecao selecao = null;
                if (codigo > 0)
                {
                    selecao = repSelecao.BuscarPorCodigo(codigo, true);
                    if (selecao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A seleção informada não foi localizado");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A seleção informada não foi localizado");
                }

                selecao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Cancelada;
                repSelecao.Atualizar(selecao, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    selecao.Codigo,
                    Situacao = selecao.SituacaoSelecaoSeparacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Privados
        private dynamic ObterSelecao(Dominio.Entidades.Embarcador.WMS.Selecao selecao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> cargasSelecao = repSelecaoCarga.BuscarPorSelecao(selecao.Codigo);

            var retorno = new
            {
                SelecaoWMS = new
                {
                    Codigo = selecao.Codigo,
                    Situacao = selecao.SituacaoSelecaoSeparacao,
                    ListaFuncionarios = selecao.Funcionarios != null ? (from obj in selecao.Funcionarios
                                                                        orderby obj.Usuario.Nome
                                                                        select new
                                                                        {
                                                                            obj.Usuario.Codigo,
                                                                            CPF = obj.Usuario.CPF_Formatado,
                                                                            obj.Usuario.Nome
                                                                        }).ToList() : null,
                    Cargas = (from obj in cargasSelecao select serCarga.ObterDetalhesCargaParaCarregamento(obj.Carga, unitOfWork)).ToList()
                }
            };

            return retorno;
        }

        private bool SalvarCargas(Dominio.Entidades.Embarcador.WMS.Selecao selecao, dynamic dynCargas, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);
            Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> selecaoCargaExiste = selecao.Cargas != null ? selecao.Cargas.ToList() : new List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();
            bool contemRegistro = false;
            List<int> codigos = new List<int>();

            foreach (dynamic dynCarga in dynCargas)
                codigos.Add((int)dynCarga.Codigo);

            List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> selecaoCargaOutros = repSelecaoCarga.BuscarPorCargas(codigos);
            for (int i = 0; i < selecaoCargaOutros.Count; i++)
            {
                Dominio.Entidades.Embarcador.WMS.SelecaoCarga selecaoCarga = selecaoCargaOutros[i];
                if (selecaoCarga.Selecao.Codigo != selecao.Codigo)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, selecaoCarga.Selecao, null, "A carga " + selecaoCarga.Carga.CodigoCargaEmbarcador + " foi removido desta seleção e foi vinculado a seleção " + selecao.Codigo.ToString() + ".", unitOfWork);
                    repSelecaoCarga.Deletar(selecaoCarga);
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAuditar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            for (int i = 0; i < codigos.Count; i++)
            {
                int codigoCarga = codigos[i];

                Dominio.Entidades.Embarcador.WMS.SelecaoCarga selecaoCarga = (from obj in selecaoCargaExiste where obj.Carga.Codigo == codigoCarga select obj).FirstOrDefault();

                if (selecaoCarga == null)
                {
                    selecaoCarga = new Dominio.Entidades.Embarcador.WMS.SelecaoCarga();
                    selecaoCarga.Selecao = selecao;
                    selecaoCarga.Carga = repCarga.BuscarPorCodigo(codigoCarga);

                    cargasAuditar.Add(selecaoCarga.Carga);
                    repSelecaoCarga.Inserir(selecaoCarga, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
                contemRegistro = true;
            }

            for (int i = 0; i < selecaoCargaExiste.Count; i++)
            {
                Dominio.Entidades.Embarcador.WMS.SelecaoCarga cargaExiste = selecaoCargaExiste[i];
                if (!codigos.Contains(cargaExiste.Carga.Codigo))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaExiste.Carga, null, "Removida da seleção " + selecao.Codigo.ToString() + ".", unitOfWork);

                    repSelecaoCarga.Deletar(cargaExiste, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAutitar in cargasAuditar)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAutitar, null, "Incluída na selação " + selecao.Codigo.ToString() + ".", unitOfWork);

            return contemRegistro;
        }

        private bool SalvarListaFuncionarios(Dominio.Entidades.Embarcador.WMS.Selecao selecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Embarcador.WMS.SelecaoFuncionario repSelecaoFuncionario = new Repositorio.Embarcador.WMS.SelecaoFuncionario(unidadeDeTrabalho);

            if (selecao.Codigo > 0)
                repSelecaoFuncionario.DeletarPorSelecao(selecao.Codigo);
            bool contemRegistro = false;
            dynamic listaFuncionario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFuncionario"));
            if (listaFuncionario != null)
            {
                foreach (var funcionario in listaFuncionario)
                {
                    int codigo = 0;
                    int.TryParse((string)funcionario.Funcionario.Codigo, out codigo);
                    if (codigo > 0)
                    {
                        Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario selecaoFuncionario = new Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario();
                        selecaoFuncionario.Selecao = selecao;
                        selecaoFuncionario.Usuario = repMotorista.BuscarPorCodigo(codigo);

                        repSelecaoFuncionario.Inserir(selecaoFuncionario);
                        contemRegistro = true;
                    }
                }
            }

            return contemRegistro;
        }

        private bool SalvarFuncionariosSeparacao(Dominio.Entidades.Embarcador.WMS.Separacao separacao, Dominio.Entidades.Embarcador.WMS.Selecao selecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.WMS.SeparacaoFuncionario repSeparacaoFuncionario = new Repositorio.Embarcador.WMS.SeparacaoFuncionario(unidadeDeTrabalho);
            Repositorio.Embarcador.WMS.SelecaoFuncionario repSelecaoFuncionario = new Repositorio.Embarcador.WMS.SelecaoFuncionario(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario> funcionarios = repSelecaoFuncionario.BuscarPorSelecao(selecao.Codigo);

            bool contemRegistro = false;
            for (int i = 0; i < funcionarios.Count; i++)
            {
                Dominio.Entidades.Embarcador.WMS.SeparacaoFuncionario separacaoFuncionario = new Dominio.Entidades.Embarcador.WMS.SeparacaoFuncionario();
                separacaoFuncionario.Separacao = separacao;
                separacaoFuncionario.Usuario = funcionarios[i].Usuario;

                repSeparacaoFuncionario.Inserir(separacaoFuncionario);
                contemRegistro = true;
            }

            return contemRegistro;
        }

        private bool SalvarCargas(Dominio.Entidades.Embarcador.WMS.Separacao separacao, Dominio.Entidades.Embarcador.WMS.Selecao selecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> cargas = repSelecaoCarga.BuscarPorSelecao(selecao.Codigo);

            for (int i = 0; i < cargas.Count; i++)
            {
                cargas[i].Carga.PossuiSeparacao = true;
                cargas[i].Carga.SeparacaoConferida = false;

                repCarga.Atualizar(cargas[i].Carga);
            }
            return true;
        }

        private bool SalvarProdutosEmbarcador(Dominio.Entidades.Embarcador.WMS.Separacao separacao, Dominio.Entidades.Embarcador.WMS.Selecao selecao, Repositorio.UnitOfWork unidadeDeTrabalho, ref string retorno)
        {
            retorno = "";
            Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unidadeDeTrabalho);
            Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> cargas = repSelecaoCarga.BuscarPorSelecao(selecao.Codigo);

            bool contemRegistro = false;
            for (int i = 0; i < cargas.Count; i++)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoCarga = repSelecaoCarga.BuscarPrimeiroProdutoCarga(cargas[i].Carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotas = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorCarga(cargas[i].Carga.Codigo);

                    if (listaNotas == null || listaNotas.Count == 0)
                    {
                        retorno = "Carga " + cargas[i].Carga.CodigoCargaEmbarcador + " sem nota fiscal.";
                        return false;
                    }


                    for (int k = 0; k < listaNotas.Count; k++)
                    {
                        int quantidade = listaNotas[k].Volumes;                        
                        List<int> codigosSelecionados = new List<int>();
                        while (quantidade > 0)
                        {
                            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote = repProdutoEmbarcadorLote.BuscarPorLote(produtoCarga.Produto.Codigo, listaNotas[k].Numero.ToString("D"), quantidade, listaNotas[k].Destinatario?.CPF_CNPJ ?? 0, listaNotas[k].Emitente?.CPF_CNPJ ?? 0, codigosSelecionados);
                            if (lote == null)
                            {
                                retorno = "Lote do produto " + produtoCarga.Produto.Descricao + " não encontrado ou não possui estoque suficiente";
                                return false;
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador separacaoProduto = new Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador();
                                if (lote.QuantidadeAtual > 0)
                                {
                                    if (lote.QuantidadeAtual >= quantidade)
                                    {
                                        if (lote.QuantidadeAtual == quantidade)
                                            codigosSelecionados.Add(lote.Codigo);
                                        separacaoProduto.Quantidade = quantidade;
                                        quantidade = 0;
                                    }
                                    else if (lote.QuantidadeAtual < quantidade)
                                    {
                                        codigosSelecionados.Add(lote.Codigo);
                                        separacaoProduto.Quantidade = lote.QuantidadeAtual;
                                        quantidade -= (int)lote.QuantidadeAtual;
                                    }

                                    separacaoProduto.ProdutoEmbarcadorLote = lote;
                                    separacaoProduto.QuantidadeSeparada = 0;
                                    separacaoProduto.Separacao = separacao;

                                    repSeparacaoProdutoEmbarcador.Inserir(separacaoProduto);
                                    contemRegistro = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = repSelecaoCarga.BuscarProdutosCarga(cargas[i].Carga.Codigo);

                    for (int k = 0; k < listaProdutos.Count; k++)
                    {
                        decimal quantidade = listaProdutos[k].Quantidade;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        {
                            quantidade = (decimal)repPedidoXMLNotaFiscal.BuscarVolumesPorCarga(cargas[i].Carga.Codigo);//buscar os volumes das ntoas
                            if (quantidade == 0)
                                quantidade = 1m;
                        }
                        List<int> codigosSelecionados = new List<int>();
                        while (quantidade > 0)
                        {
                            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote = repProdutoEmbarcadorLote.BuscarPorLote(listaProdutos[k].Produto.Codigo, codigosSelecionados);
                            if (lote == null)
                            {
                                retorno = "Lote do produto " + listaProdutos[k].Produto.Descricao + " não encontrado ou não possui estoque suficiente";
                                return false;
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador separacaoProduto = new Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador();
                                if (lote.QuantidadeAtual > 0)
                                {
                                    if (lote.QuantidadeAtual >= quantidade)
                                    {
                                        if (lote.QuantidadeAtual == quantidade)
                                            codigosSelecionados.Add(lote.Codigo);
                                        separacaoProduto.Quantidade = quantidade;
                                        quantidade = 0;
                                    }
                                    else if (lote.QuantidadeAtual < quantidade)
                                    {
                                        codigosSelecionados.Add(lote.Codigo);
                                        separacaoProduto.Quantidade = lote.QuantidadeAtual;
                                        quantidade -= lote.QuantidadeAtual;
                                    }

                                    separacaoProduto.ProdutoEmbarcadorLote = lote;
                                    separacaoProduto.QuantidadeSeparada = 0;
                                    separacaoProduto.Separacao = separacao;

                                    repSeparacaoProdutoEmbarcador.Inserir(separacaoProduto);
                                    contemRegistro = true;
                                }
                            }
                        }

                    }
                }
            }

            return contemRegistro;
        }

        #endregion
    }
}
