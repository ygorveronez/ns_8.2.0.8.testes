using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ImportacaoProgramacaoColeta")]
    public class ImportacaoProgramacaoColetaController : BaseController
    {
		#region Construtores

		public ImportacaoProgramacaoColetaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "NumeroImportacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "ClienteDestino", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Produto Padrão", "Produto", 20, Models.Grid.Align.left, true);
                if (filtrosPesquisa.Situacao == SituacaoImportacaoProgramacaoColeta.Todos)
                    grid.AdicionarCabecalho("Situação", "SituacaoImportacaoProgramacaoColeta", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta> listaImportacaoProgramacaoColeta = repImportacaoProgramacaoColeta.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repImportacaoProgramacaoColeta.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in listaImportacaoProgramacaoColeta
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.NumeroImportacao,
                                       ClienteDestino = obj.ClienteDestino?.Descricao ?? string.Empty,
                                       TipoOperacao = obj.TipoOperacao?.Descricao ?? string.Empty,
                                       Produto = obj.Produto?.Descricao ?? string.Empty,
                                       SituacaoImportacaoProgramacaoColeta = obj.SituacaoImportacaoProgramacaoColeta.ObterDescricao()
                                   }).ToList());

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                double cnpjCpfDestino = Request.GetDoubleParam("ClienteDestino");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoProduto = Request.GetIntParam("ProdutoPadrao");

                Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);


                Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta = new Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta()
                {
                    SituacaoImportacaoProgramacaoColeta = SituacaoImportacaoProgramacaoColeta.EmCriacao,
                    DataCriacao = DateTime.Now,
                    NumeroImportacao = repImportacaoProgramacaoColeta.BuscarProximoNumero(),
                    Usuario = Usuario,
                    NumeroRepeticoes = Request.GetIntParam("NumeroRepeticoes"),
                    IntervaloDiasGeracao = Request.GetIntParam("IntervaloDiasGeracao"),
                    TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao),
                    Produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto),
                    ClienteDestino = repCliente.BuscarPorCPFCNPJ(cnpjCpfDestino)
                };

                repImportacaoProgramacaoColeta.Inserir(importacaoProgramacaoColeta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterDetalhesImportacaoProgramacaoColeta(importacaoProgramacaoColeta));
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o Programação de Coletas.");
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

                Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta = repImportacaoProgramacaoColeta.BuscarPorCodigo(codigo, true);

                if (importacaoProgramacaoColeta == null)
                    return new JsonpResult(false, true, "Importação não encontrada.");

                return new JsonpResult(ObterDetalhesImportacaoProgramacaoColeta(importacaoProgramacaoColeta));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Agrupamento", "NumeroAgrupamento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem Importação", "Mensagem", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Nova Programação", "NumeroNovaProgramacao", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento repImportacaoProgramacaoColetaAgrupamento = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int count = repImportacaoProgramacaoColetaAgrupamento.ContarConsulta(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> lista = count > 0 ? repImportacaoProgramacaoColetaAgrupamento.Consultar(codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();
                grid.setarQuantidadeTotal(count);

                grid.AdicionaRows((from obj in lista
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.NumeroAgrupamento,
                                       Carga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                       obj.Mensagem,
                                       NumeroNovaProgramacao = obj.AgrupamentoNovaProgramacao?.ImportacaoProgramacaoColeta.NumeroImportacao.ToString() ?? string.Empty
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProgramacaoColeta();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork);

                Servicos.Embarcador.Logistica.ImportacaoProgramacaoColeta servicoImportacaoProgramacaoColeta = new Servicos.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta servicoImportacaoPedidoProgramacaoColeta = new Servicos.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta(unitOfWork);

                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                int codigo = ((string)parametro.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta = repImportacaoProgramacaoColeta.BuscarPorCodigoComFetch(codigo);

                if (importacaoProgramacaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta != SituacaoImportacaoProgramacaoColeta.EmCriacao &&
                    importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta != SituacaoImportacaoProgramacaoColeta.FalhaNaGeracao)
                    return new JsonpResult(false, true, "Não é possível importar novamente a planilha na atual situação.");

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProgramacaoColeta();
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta> pedidosImportados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref pedidosImportados, ref dadosLinhas, out string erro, ((dicionario) =>
                {
                    return servicoImportacaoPedidoProgramacaoColeta.ObterPedidoImportar(dicionario);
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo");

                int totalRegistrosImportados = servicoImportacaoProgramacaoColeta.ProcessarImportacaoProgramacao(importacaoProgramacaoColeta, pedidosImportados, this.Cliente, Auditado);

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesImportacaoProgramacaoColeta(Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta)
        {
            if (importacaoProgramacaoColeta == null)
                return null;

            var retorno = new
            {
                NumeroImportacaoProgramacaoColeta = importacaoProgramacaoColeta.NumeroImportacao,
                NumeroRepeticoes = importacaoProgramacaoColeta.NumeroRepeticoes,
                IntervaloDiasGeracao = importacaoProgramacaoColeta.IntervaloDiasGeracao,
                TipoOperacao = new { Codigo = importacaoProgramacaoColeta.TipoOperacao?.Codigo ?? 0, Descricao = importacaoProgramacaoColeta.TipoOperacao?.Descricao ?? "" },
                ProdutoPadrao = new { Codigo = importacaoProgramacaoColeta.Produto?.Codigo ?? 0, Descricao = importacaoProgramacaoColeta.Produto?.Descricao ?? "" },
                ClienteDestino = new { Codigo = importacaoProgramacaoColeta.ClienteDestino?.CPF_CNPJ ?? 0, Descricao = importacaoProgramacaoColeta.ClienteDestino?.Descricao ?? "" },
                Situacao = importacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta
            };

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta()
            {
                NumeroImportacao = Request.GetIntParam("NumeroImportacaoProgramacaoColeta"),
                CnpjCpfDestino = Request.GetDoubleParam("ClienteDestino"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoProduto = Request.GetIntParam("ProdutoPadrao"),
                Situacao = Request.GetEnumParam<SituacaoImportacaoProgramacaoColeta>("Situacao")
            };
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoProgramacaoColeta()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Agrupamento", Propriedade = "Agrupamento", Tamanho = tamanho, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Sequência", Propriedade = "Sequencia", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Código Integração Remetente", Propriedade = "CodigoIntegracaoRemetente", Tamanho = tamanho, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Distância (KM)", Propriedade = "Distancia", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Quantidade Planejada (Litros)", Propriedade = "QuantidadePlanejada", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Placa", Propriedade = "Placa", Tamanho = tamanho, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Data Carregamento", Propriedade = "DataCarregamento", Tamanho = tamanho, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Hora Carregamento", Propriedade = "HoraCarregamento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Código Integração Transportador", Propriedade = "CodigoIntegracaoTransportador", Tamanho = tamanho, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } });
            //Próximo Id será o 10

            return configuracoes;
        }

        #endregion
    }
}
