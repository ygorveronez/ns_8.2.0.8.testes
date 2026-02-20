using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Locais", "Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class LocaisController : BaseController
    {
        #region Construtores

        public LocaisController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.Locais reg = new Dominio.Entidades.Embarcador.Logistica.Locais();

                try
                {
                    PreencherDados(reg, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);

                repositorio.Inserir(reg, Auditado);

                if (reg.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.RaioProximidade)
                    SalvarRaiosProximidade(reg, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Locais reg = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (reg.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.RaioProximidade)
                    SalvarRaiosProximidade(reg, unitOfWork);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherDados(reg, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(reg, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Logistica.RaioProximidade repositorioRaiosProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Locais reg = repositorio.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raiosProximidade = repositorioRaiosProximidade.BuscarPorLocal(codigo);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<string> outrasAreas = new List<string>();
                if (reg.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.MicroRegiaoRoteirizacao)
                {
                    var outras = repositorio.BuscarPorTipoDeLocalEFiliais(reg.Tipo, new List<int>() { reg.Filial?.Codigo ?? 0 });
                    outras = outras.FindAll(x => x.Codigo != reg.Codigo).ToList();
                    foreach (var outra in outras)
                        outrasAreas.Add(outra.Area);
                }

                return new JsonpResult(new
                {
                    reg.Codigo,
                    reg.Descricao,
                    reg.Tipo,
                    reg.TipoArea,
                    reg.Area,
                    reg.Observacao,
                    Filial = new
                    {
                        reg.Filial?.Codigo,
                        reg.Filial?.Descricao
                    },
                    DemaisMicroRegioes = outrasAreas,
                    RaiosProximidade = raiosProximidade
                });
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorTipoEFiliais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var tipolocal = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal>("TipoLocal");
                List<int> filiais = Request.GetListParam<int>("CodigosFiliais");
                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Locais> itens = repositorio.BuscarPorTipoDeLocalEFiliais(tipolocal, filiais);

                if (itens == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var result = (
                   from reg in itens
                   select new
                   {
                       reg.Codigo,
                       reg.Descricao,
                       reg.Area
                   }
                ).ToList();

                return new JsonpResult(result);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorTiposEFiliais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var tiposlocal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal>("TiposLocal");
                List<int> filiais = Request.GetListParam<int>("CodigosFiliais");
                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Locais> itens = repositorio.BuscarPorTiposDeLocalEFiliais(tiposlocal, filiais);

                if (itens == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var result = (from tp in tiposlocal
                              select new
                              {
                                  Tipo = tp,
                                  Data = (from reg in itens
                                          where reg.Tipo == tp 
                                          select new
                                          {
                                              reg.Codigo,
                                              reg.Descricao,
                                              reg.Area
                                          }).ToList()
                              }).ToList();

                return new JsonpResult(result);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Logistica.RaioProximidade repositorioRaiosProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Locais reg = repositorio.BuscarPorCodigo(codigo, auditavel: true);
                List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raiosProximidade = repositorioRaiosProximidade.BuscarPorLocal(codigo);


                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                if (reg.Tipo == TipoLocal.RaioProximidade)
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.RaioProximidade raio in raiosProximidade)
                    {
                        repositorioRaiosProximidade.Deletar(raio);
                    }
                }

                repositorio.Deletar(reg, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaLocaisRaioProximidade()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaLocaisRaioProximidade());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.Locais servicoLocais = new Servicos.Embarcador.Logistica.Locais(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                var parametros = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Parametro"));

                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoLocais.ImportarLocais(dados, arquivoGerador, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Logistica.Locais servicoLocais = new Servicos.Embarcador.Logistica.Locais(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoLocais.ConfiguracaoImportacaoLocais(unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.Locais reg, Repositorio.UnitOfWork unitOfWork)
        {
            var descricao = Request.Params("Descricao");
            var tipolocal = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal>("TipoLocal");
            var tipoarea = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea>("TipoArea");
            var area = Request.Params("Area");
            var observacao = Request.Params("Observacao");
            var codigoFilial = Request.GetIntParam("Filial");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new Exception("Descrição é obrigatória.");

            if (descricao.Length > 100)
                throw new Exception("Descrição não pode passar de 100 caracteres.");

            if (string.IsNullOrWhiteSpace(area))
                throw new Exception("Área deve ser informadas.");

            reg.Descricao = descricao;
            reg.Tipo = tipolocal;
            reg.TipoArea = tipoarea;
            reg.Area = area;
            reg.Observacao = observacao;
            reg.Filial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork).BuscarPorCodigo(codigoFilial);
        }

        private void SalvarRaiosProximidade(Dominio.Entidades.Embarcador.Logistica.Locais local, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.RaioProximidade repositorioRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RaioProximidade> raiosProximidade = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.RaioProximidade>>(Request.Params("RaiosProximidade"));

            List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raiosSalvos = repositorioRaioProximidade.BuscarPorLocal(local.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RaioProximidade raioProximidade in raiosProximidade)
            {
                Dominio.Entidades.Embarcador.Logistica.RaioProximidade raio = repositorioRaioProximidade.BuscarPorCodigo(raioProximidade.Codigo);

                if (raio == null)
                {
                    Dominio.Entidades.Embarcador.Logistica.RaioProximidade raioSalvar = new Dominio.Entidades.Embarcador.Logistica.RaioProximidade();

                    raioSalvar.Raio = raioProximidade.Raio;
                    raioSalvar.Identificacao = raioProximidade.Identificacao ?? "";
                    raioSalvar.Cor = raioProximidade.Cor ?? "";
                    raioSalvar.GerarAlertaAutomaticoPorPermanencia = raioProximidade?.GerarAlertaAutomaticoPorPermanencia ?? false;
                    raioSalvar.Tempo = raioProximidade?.Tempo ?? 0;
                    raioSalvar.TipoAlerta = raioProximidade?.TipoAlerta ?? TipoAlerta.SemAlerta;
                    raioSalvar.Local = local;

                    repositorioRaioProximidade.Inserir(raioSalvar);
                }
                else
                {
                    raio.Raio = raioProximidade.Raio;
                    raio.Identificacao = raioProximidade.Identificacao ?? "";
                    raio.Cor = raioProximidade.Cor ?? "";
                    raio.GerarAlertaAutomaticoPorPermanencia = raioProximidade?.GerarAlertaAutomaticoPorPermanencia ?? false;
                    raio.Tempo = raioProximidade?.Tempo ?? 0;
                    raio.TipoAlerta = raioProximidade?.TipoAlerta ?? TipoAlerta.SemAlerta;
                    raio.Local = local;

                    repositorioRaioProximidade.Atualizar(raio);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Logistica.RaioProximidade raioSalvo in raiosSalvos)
            {
                if (!raiosProximidade.Any(r => r.Codigo == raioSalvo.Codigo))
                {
                    repositorioRaioProximidade.Deletar(raioSalvo);
                }
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Locais.Filial, "Filial", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Locais.TipoLocal, "Tipo", 50, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais filtroPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Logistica.Locais repositorio = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Locais> listaConsulta = repositorio.Consultar(filtroPesquisa, parametrosConsulta);

                int totalRegistros = repositorio.ContarConsulta(filtroPesquisa);

                var listaRetornar = (
                    from reg in listaConsulta
                    select new
                    {
                        reg.Codigo,
                        reg.Descricao,
                        Filial = reg.Filial?.Descricao,
                        Tipo = reg.Tipo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaLocaisRaioProximidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Local", "Local", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Identificação", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Raio (Km)", "Raio", 50, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocaisRaioProximidade filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocaisRaioProximidade();
                filtroPesquisa.Descricao = Request.GetStringParam("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Logistica.RaioProximidade repositorioRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> listaConsulta = repositorioRaioProximidade.Consultar(filtroPesquisa, parametrosConsulta);

                int totalRegistros = repositorioRaioProximidade.ContarConsulta(filtroPesquisa);

                var listaRetornar = (
                    from reg in listaConsulta
                    select new
                    {
                        reg.Codigo,
                        Descricao = reg.Identificacao,
                        reg.Raio,
                        Local = reg.Local.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais()
            {
                TipoLocal = Request.GetNullableEnumParam<TipoLocal>("TipoLocal"),
                Descricao = Request.GetStringParam("Descricao"),
                CodigosFiliais = Request.GetListParam<int>("Filial")
            };

            return filtroPesquisa;
        }

        #endregion
    }
}
