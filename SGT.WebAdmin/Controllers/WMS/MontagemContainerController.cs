using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/MontagemContainer")]
    public class MontagemContainerController : BaseController
    {
		#region Construtores

		public MontagemContainerController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenacao(parametrosConsulta.PropriedadeOrdenar);

                int totalRegistros = 0;
                dynamic lista = ExecutaPesquisa(ref totalRegistros, parametrosConsulta, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer = repositorioMontagemContainer.BuscarPorCodigo(codigo, false);

                if (montagemContainer == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal repositorioMontagemContainerNotaFiscal = new Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal(unitOfWork);
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repositorioRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

                List<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> listaNotasFiscais = repositorioMontagemContainerNotaFiscal.BuscarPorMontagemContainer(codigo);
                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaRecebimentoMercadoria = new List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();

                List<int> codigosXmlNotasFiscais = listaNotasFiscais.Select(obj => obj.XMLNotaFiscal.Codigo).Distinct().ToList();
                
                if (codigosXmlNotasFiscais.Count > 0)
                    listaRecebimentoMercadoria = repositorioRecebimentoMercadoria.BuscarPorXMLNotasFiscais(codigosXmlNotasFiscais);
                
                var retorno = new
                {
                    DadosGerais = new
                    {
                        Codigo = montagemContainer.Codigo,
                        montagemContainer.Status,
                        montagemContainer.NumeroBooking,
                        TipoContainer = new { montagemContainer.TipoContainer.Codigo, montagemContainer.TipoContainer.Descricao },
                        Container = new { Codigo = montagemContainer.Container?.Codigo ?? 0, Descricao = montagemContainer.Container?.Descricao ?? "" },
                        PortoOrigem = new { montagemContainer.PortoOrigem.Codigo, montagemContainer.PortoOrigem.Descricao },
                        PortoDestino = new { montagemContainer.PortoDestino.Codigo, montagemContainer.PortoDestino.Descricao },
                    },
                    InformacoesContainer = new
                    {
                        IDMontagemContainer = montagemContainer.Id,
                        StatusMontagemContainer = montagemContainer.Status.ObterDescricao(),
                        PesoContainer = montagemContainer.TipoContainer.PesoMaximo.ToString("n2"),
                        TaraContainer = montagemContainer.TipoContainer.Tara.ToString("n2"),
                        MetroCubicoContainer = montagemContainer.TipoContainer.MetrosCubicos.ToString("n2")
                    },
                    NotasFiscais = (from notaFiscal in listaNotasFiscais
                                    select new
                                    {
                                        notaFiscal.XMLNotaFiscal.Codigo,
                                        notaFiscal.XMLNotaFiscal.Numero,
                                        notaFiscal.XMLNotaFiscal.Chave,
                                        Emissor = notaFiscal.XMLNotaFiscal.Emitente.Nome,
                                        CNPJ = notaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ_Formatado,
                                        Quantidade = notaFiscal.XMLNotaFiscal.Volumes.ToString(),
                                        Tipo = notaFiscal.XMLNotaFiscal.TipoDeCarga,
                                        ProdutoEmbarcador = notaFiscal.XMLNotaFiscal.Produto,
                                        MetroCubico = notaFiscal.XMLNotaFiscal.MetrosCubicos.ToString("n2"),
                                        PesoNota = notaFiscal.XMLNotaFiscal.Peso.ToString("n2"),
                                        DataRecebimentoWMS = (from r in listaRecebimentoMercadoria where r.XMLNotaFiscal.Codigo == notaFiscal.XMLNotaFiscal.Codigo && r.Recebimento != null select r.Recebimento.Data.ToString("dd/MM/yyyy HH:mm")).FirstOrDefault(),
                                        Serie = notaFiscal.XMLNotaFiscal.Serie,
                                        DataEmissao = notaFiscal.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                        ValorNota = notaFiscal.XMLNotaFiscal.Valor.ToString("n2"),
                                        ValorProdutos = notaFiscal.XMLNotaFiscal.ValorTotalProdutos.ToString("n2"),
                                        Medidas = $"{notaFiscal.XMLNotaFiscal.Comprimento.ToString("n2")} x {notaFiscal.XMLNotaFiscal.Largura.ToString("n2")} x {notaFiscal.XMLNotaFiscal.Altura.ToString("n2")}"

                                    }).ToList()
                };
                
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer = new Dominio.Entidades.Embarcador.WMS.MontagemContainer();
                
                PreencherEntidade(montagemContainer, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> AlterarStatus()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);

                StatusMontagemContainer statusMontagemContainer = Request.GetEnumParam<StatusMontagemContainer>("Status");
                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer = repositorioMontagemContainer.BuscarPorCodigo(Request.GetIntParam("Codigo"), false);

                if (montagemContainer == null)
                    return new JsonpResult(true, "O registro não foi encontrado.");
                
                montagemContainer.Status = statusMontagemContainer;

                repositorioMontagemContainer.Atualizar(montagemContainer);

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o registro.");
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

                Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer = repositorioMontagemContainer.BuscarPorCodigo(Request.GetIntParam("Codigo"), false);

                if (montagemContainer == null)
                    return new JsonpResult(false, "O registro não foi encontrado.");

                PreencherEntidade(montagemContainer, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repositorioContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repositorioPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);

            int codigoContainer = Request.GetIntParam("CodigoContainer");

            if (montagemContainer.Status == StatusMontagemContainer.Aberto || montagemContainer.Codigo == 0)
            {
                montagemContainer.TipoContainer = repositorioContainerTipo.BuscarPorCodigo(Request.GetIntParam("CodigoTipoContainer"));
                montagemContainer.PortoOrigem = repositorioPorto.BuscarPorCodigo(Request.GetIntParam("CodigoPortoOrigem"));
                montagemContainer.PortoDestino = repositorioPorto.BuscarPorCodigo(Request.GetIntParam("CodigoPortoDestino"));
            }
                                  
            montagemContainer.NumeroBooking = Request.GetStringParam("NumeroBooking");
            montagemContainer.Container = codigoContainer > 0 ? repositorioContainer.BuscarPorCodigo(codigoContainer) : null;
            
            if (montagemContainer.Codigo > 0)
                repositorioMontagemContainer.Atualizar(montagemContainer);
            else
            {
                montagemContainer.Id = repositorioMontagemContainer.BuscarProximoIdSequencial();
                montagemContainer.Status = StatusMontagemContainer.Aberto;
                repositorioMontagemContainer.Inserir(montagemContainer);
            }
            
            PreencherEntidadeNotasFiscais(montagemContainer, unitOfWork);
        }

        private void PreencherEntidadeNotasFiscais(Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal repositorioMontagemContainerNotaFiscal = new Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> notasFiscais = repositorioMontagemContainerNotaFiscal.BuscarPorMontagemContainer(montagemContainer.Codigo);

            foreach (Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal notaFiscal in notasFiscais)
                repositorioMontagemContainerNotaFiscal.Deletar(notaFiscal);

            dynamic dynNotasFiscais = JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasFiscais"));

            if (((Newtonsoft.Json.Linq.JContainer)dynNotasFiscais).Count == 0)
                throw new ControllerException("É necessário selecionar pelo menos uma nota fiscal.");

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            foreach (dynamic dynNotaFiscal in dynNotasFiscais)
            {
                Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal montagemNotaFiscal = new Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal()
                {
                    MontagemContainer = montagemContainer
                };

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarNotaDisponivelMontagemContainerPorCodigo(((string)dynNotaFiscal.Codigo).ToInt());

                if (xmlNotaFiscal == null)
                    throw new ControllerException($"A nota fiscal {((string)dynNotaFiscal.Numero).ToInt()} já está em outra montagem de container.");

                montagemNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;

                repositorioMontagemContainerNotaFiscal.Inserir(montagemNotaFiscal);
            }
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer filtrosPesquisa = ObterFiltrosPesquisa();

            totalRegistros = repositorioMontagemContainer.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.WMS.MontagemContainer> listaGrid = totalRegistros > 0 ? repositorioMontagemContainer.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.WMS.MontagemContainer>();

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            TipoContainer = obj.TipoContainer.Descricao,
                            obj.NumeroBooking,
                            IdMontagemContainer = obj.Id,
                            Status = obj.Status.ObterDescricao(),
                            Descricao = $"{obj.NumeroBooking} - {obj.TipoContainer.Descricao}",
                            ContainerCodigo = obj.Container?.Codigo ?? 0,
                            ContainerDescricao = obj.Container?.Descricao ?? "",
                        };
            
            return lista.ToList();
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("ContainerCodigo", false);
            grid.AdicionarCabecalho("Tipo Container", "TipoContainer", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Id Montagem Container", "IdMontagemContainer", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "Status", 30, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Container", "ContainerDescricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N° Booking", "NumeroBooking", 12, Models.Grid.Align.left, true);
            
            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaMontagemContainer()
            {
                Container = Request.GetIntParam("Container"),
                TipoContainer = Request.GetIntParam("TipoContainer"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                IdMontagemContainer = Request.GetIntParam("IDMontagemContainer"),
                Status = Request.GetEnumParam<StatusMontagemContainer>("Status")
            };
        }

        private string ObterPropriedadeOrdenacao(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
