using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IndicadorIntegracaoNFe")]
    public class IndicadorIntegracaoNFeController : BaseController
    {
		#region Construtores

		public IndicadorIntegracaoNFeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaGraficoIntegracaoAutomatica()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaGraficoIntegracaoAutomatica();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaGraficoIntegracaoPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaGraficoIntegracaoPorEmail();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaIntegracaoRejeitada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaIntegracaoRejeitada();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaGraficoIntegracaoAutomatica()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Tipo = TipoIndicadorIntegracaoNFe.Automatico;
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork);
                dynamic dadosGrafico = repositorioIndicadorIntegracaoNFe.ConsultaGrafico(filtrosPesquisa);
                
                if (dadosGrafico == null)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                return new JsonpResult(dadosGrafico);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do gráfico de integração automática de NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaGraficoIntegracaoPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Tipo = TipoIndicadorIntegracaoNFe.PorEmail;
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork);
                dynamic dadosGrafico = repositorioIndicadorIntegracaoNFe.ConsultaGrafico(filtrosPesquisa);

                if (dadosGrafico == null)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                return new JsonpResult(dadosGrafico);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do gráfico de integração por e-mail de NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoRejeitada()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaIntegracaoRejeitada());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
        }

        private Models.Grid.Grid ObterGridPesquisaGraficoIntegracaoAutomatica()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>(),
                    tituloExportacao = "Integração Automática de NF-e"
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Nota Fiscal", propriedade: "NumeroNota", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Data Integração", propriedade: "DataIntegracao", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Motivo Rejeição", propriedade: "MotivoRejeicao", tamanho: 30, alinhamento: Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Tipo = TipoIndicadorIntegracaoNFe.Automatico;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null;
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork);
                int totalRegistros = repositorioIndicadorIntegracaoNFe.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe> listaIntegracaoRejeitada = (totalRegistros > 0) ? repositorioIndicadorIntegracaoNFe.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe>();

                var listaIntegracaoRejeitadaRetornar = (
                    from o in listaIntegracaoRejeitada
                    select new
                    {
                        o.Codigo,
                        NumeroNota = o.XMLNotaFiscal?.Numero.ToString() ?? "",
                        CodigoCargaEmbarcador = o.NumeroCarga ?? "",
                        DataIntegracao = o.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        Transportador = o.PedidoXMLNotaFiscal?.CargaPedido?.Carga?.Empresa?.Descricao,
                        Filial = o.Filial?.Descricao,
                        o.MotivoRejeicao,
                        Situacao = o.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracaoRejeitadaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaGraficoIntegracaoPorEmail()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>(),
                    tituloExportacao = "Integração por E-mail de NF-e"
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Nota Fiscal", propriedade: "NumeroNota", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "E-mail", propriedade: "Email", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Data Integração", propriedade: "DataIntegracao", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Motivo Rejeição", propriedade: "MotivoRejeicao", tamanho: 30, alinhamento: Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Tipo = TipoIndicadorIntegracaoNFe.PorEmail;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null;
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork);
                int totalRegistros = repositorioIndicadorIntegracaoNFe.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe> listaIntegracaoRejeitada = (totalRegistros > 0) ? repositorioIndicadorIntegracaoNFe.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe>();

                var listaIntegracaoRejeitadaRetornar = (
                    from o in listaIntegracaoRejeitada
                    select new
                    {
                        o.Codigo,
                        NumeroNota = o.XMLNotaFiscal?.Numero.ToString() ?? "",
                        Email = o.EmailRemetente,
                        CodigoCargaEmbarcador = o.NumeroCarga ?? "",
                        DataIntegracao = o.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        Transportador = o.PedidoXMLNotaFiscal?.CargaPedido?.Carga?.Empresa?.Descricao,
                        Filial = o.Filial?.Descricao,
                        o.MotivoRejeicao,
                        Situacao = o.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracaoRejeitadaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaIntegracaoRejeitada()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Nota Fiscal", propriedade: "NumeroNota", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Tipo", propriedade: "Tipo", tamanho: 20, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Data Integração", propriedade: "DataIntegracao", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Motivo Rejeição", propriedade: "MotivoRejeicao", tamanho: 30, alinhamento: Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Situacao = SituacaoIndicadorIntegracaoNFe.Rejeitada;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork);
                int totalRegistros = repositorioIndicadorIntegracaoNFe.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe> listaIntegracaoRejeitada = (totalRegistros > 0) ? repositorioIndicadorIntegracaoNFe.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe>();

                var listaIntegracaoRejeitadaRetornar = (
                    from o in listaIntegracaoRejeitada
                    select new
                    {
                        o.Codigo,
                        NumeroNota = o.XMLNotaFiscal?.Numero.ToString() ?? "",
                        Tipo = (o.Tipo == TipoIndicadorIntegracaoNFe.PorEmail) ? $"{o.Tipo.ObterDescricao()}: {o.EmailRemetente}" : o.Tipo.ObterDescricao(),
                        CodigoCargaEmbarcador = o.NumeroCarga ?? "",
                        DataIntegracao = o.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        o.MotivoRejeicao,
                        Transportador = o.PedidoXMLNotaFiscal?.CargaPedido?.Carga?.Empresa?.Descricao,
                        Filial = o.Filial?.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracaoRejeitadaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion
    }
}
