using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/BonificacaoTransportador")]
    public class BonificacaoTransportadorController : BaseController
    {
        #region Construtores

        public BonificacaoTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador = new Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador();

                PreencherBonificacaoTransportador(bonificacaoTransportador, unitOfWork);
                ValidarBonificacaoTransportadorDuplicada(bonificacaoTransportador, unitOfWork);
                repositorioBonificacaoTransportador.Inserir(bonificacaoTransportador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador = repositorioBonificacaoTransportador.BuscarPorCodigo(codigo, auditavel: true) ?? throw new ControllerException("Não foi possível encontrar o registro");

                PreencherBonificacaoTransportador(bonificacaoTransportador, unitOfWork);
                ValidarBonificacaoTransportadorDuplicada(bonificacaoTransportador, unitOfWork);
                repositorioBonificacaoTransportador.Atualizar(bonificacaoTransportador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador = repositorioBonificacaoTransportador.BuscarPorCodigo(codigo, auditavel: false) ?? throw new ControllerException("Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    bonificacaoTransportador.Codigo,
                    Empresa = new { bonificacaoTransportador.Empresa.Codigo, Descricao = bonificacaoTransportador.Empresa.RazaoSocial },
                    DataInicial = bonificacaoTransportador.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = bonificacaoTransportador.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    bonificacaoTransportador.Ativo,
                    bonificacaoTransportador.IncluirBaseCalculoICMS,
                    bonificacaoTransportador.NaoIncluirComponentesFreteCalculoBonificacao,
                    Percentual = bonificacaoTransportador.Percentual.ToString("n2"),
                    Tipo = bonificacaoTransportador.Tipo,
                    ComponenteFrete = new { Codigo = bonificacaoTransportador.ComponenteFrete?.Codigo ?? 0, Descricao = bonificacaoTransportador.ComponenteFrete?.Descricao ?? string.Empty },
                    TipoOcorrencia = new { Codigo = bonificacaoTransportador.TipoOcorrencia?.Codigo ?? 0, Descricao = bonificacaoTransportador.TipoOcorrencia?.Descricao ?? string.Empty },
                    TiposDeCarga = bonificacaoTransportador.TiposDeCarga != null ? (
                        from o in bonificacaoTransportador.TiposDeCarga
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList() : null,
                    Filiais = bonificacaoTransportador.Filiais != null ? (
                        from o in bonificacaoTransportador.Filiais
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList() : null,
                    TiposDeOcorrencia = bonificacaoTransportador.TiposOcorrencia != null ? (
                        from o in bonificacaoTransportador.TiposOcorrencia
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList() : null,
                    FiliaisTransportador = bonificacaoTransportador.FiliasTransportador != null ? (
                        from o in bonificacaoTransportador.FiliasTransportador
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList() : null,
                });
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador = repositorioBonificacaoTransportador.BuscarPorCodigo(codigo, auditavel: true) ?? throw new ControllerException("Não foi possível encontrar o registro");

                repositorioBonificacaoTransportador.Deletar(bonificacaoTransportador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador()
            {
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoCarga = Request.GetIntParam("TipoDeCarga"),
                CodigoTransportador = Request.GetIntParam("Empresa"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial")
            };
        }

        private void PreencherBonificacaoTransportador(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            int codigoComponenteFrete = Request.GetIntParam("ComponenteFrete");

            if (!configuracaoOcorrencia.UtilizarBonificacaoParaTransportadoresViaOcorrencia && codigoComponenteFrete == 0)
                throw new ControllerException("Não foi possível encontrar o componente de frete");

            int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

            bonificacaoTransportador.ComponenteFrete = codigoComponenteFrete > 0 ? repositorioComponenteFrete.BuscarPorCodigo(codigoComponenteFrete) : null;
            bonificacaoTransportador.Empresa = repositorioEmpresa.BuscarPorCodigo(Request.GetIntParam("Empresa")) ?? throw new ControllerException("Não foi possível encontrar o transportador");
            bonificacaoTransportador.Ativo = Request.GetBoolParam("Ativo", true);
            bonificacaoTransportador.DataInicial = Request.GetNullableDateTimeParam("DataInicial");
            bonificacaoTransportador.DataFinal = Request.GetNullableDateTimeParam("DataFinal");
            bonificacaoTransportador.IncluirBaseCalculoICMS = Request.GetBoolParam("IncluirBaseCalculoICMS");
            bonificacaoTransportador.NaoIncluirComponentesFreteCalculoBonificacao = Request.GetBoolParam("NaoIncluirComponentesFreteCalculoBonificacao");
            bonificacaoTransportador.Percentual = Request.GetDecimalParam("Percentual");
            bonificacaoTransportador.Tipo = Request.GetEnumParam("Tipo", TipoAjusteValor.Acrescimo);
            bonificacaoTransportador.TipoOcorrencia = codigoTipoOcorrencia > 0 ? repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia) : null;

            SalvarFiliais(bonificacaoTransportador, unitOfWork);
            SalvarTiposCarga(bonificacaoTransportador, unitOfWork);
            SalvarTiposOcorrencia(bonificacaoTransportador, unitOfWork);
            SalvarFiliaisTransportador(bonificacaoTransportador, unitOfWork);
        }

        private void SalvarFiliais(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (bonificacaoTransportador.Filiais == null)
                bonificacaoTransportador.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            else
                bonificacaoTransportador.Filiais.Clear();

            if (filiais != null)
            {
                foreach (var filial in filiais)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filialSalvar = repositorioFilial.BuscarPorCodigo((int)filial.Codigo);
                    bonificacaoTransportador.Filiais.Add(filialSalvar);
                }
            }
        }

        private void SalvarTiposCarga(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDeCarga"));

            if (bonificacaoTransportador.TiposDeCarga == null)
                bonificacaoTransportador.TiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            else
                bonificacaoTransportador.TiposDeCarga.Clear();

            if (tiposCarga != null)
            {
                foreach (var tipoCarga in tiposCarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaSalvar = repositorioTipoCarga.BuscarPorCodigo((int)tipoCarga.Codigo);
                    bonificacaoTransportador.TiposDeCarga.Add(tipoCargaSalvar);
                }
            }
        }

        private void ValidarBonificacaoTransportadorDuplicada(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
            bool bonificacaoTransportadorDuplicada = repositorioBonificacaoTransportador.ExisteBonificacaoTransportador(bonificacaoTransportador.Codigo, bonificacaoTransportador.Empresa.Codigo, bonificacaoTransportador.TiposDeCarga.Select(o => o.Codigo).ToList(), bonificacaoTransportador.Filiais.Select(o => o.Codigo).ToList(), bonificacaoTransportador.DataInicial, bonificacaoTransportador.DataFinal);

            if (bonificacaoTransportadorDuplicada)
                throw new ControllerException("Já existe uma bonificação para o Transportador que conflita com essa configuração");
        }

        private void SalvarTiposOcorrencia(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);

            dynamic tiposOcorrencia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDeOcorrencia"));

            if (bonificacaoTransportador.TiposOcorrencia != null && bonificacaoTransportador.TiposOcorrencia.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOcorrencia in tiposOcorrencia)
                    codigos.Add((int)tipoOcorrencia.Codigo);

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeletar = bonificacaoTransportador.TiposOcorrencia.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaDeletar in tiposDeletar)
                    bonificacaoTransportador.TiposOcorrencia.Remove(tipoOcorrenciaDeletar);
            }
            else
                bonificacaoTransportador.TiposOcorrencia = new List<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            foreach (var tipoOcorrencia in tiposOcorrencia)
            {
                int.TryParse((string)tipoOcorrencia.Codigo, out int codigoTipoOcorrencia);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe existeTipoOcorrencia = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia);

                if (existeTipoOcorrencia == null)
                    continue;

                bool existeTipoOcorrenciaCadastrada = bonificacaoTransportador.TiposOcorrencia.Any(o => o.Codigo == existeTipoOcorrencia.Codigo);

                if (!existeTipoOcorrenciaCadastrada)
                    bonificacaoTransportador.TiposOcorrencia.Add(existeTipoOcorrencia);
            }
        }

        private void SalvarFiliaisTransportador(Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);

            dynamic filiaisTransportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FiliaisTransportador"));

            if (bonificacaoTransportador.FiliasTransportador != null && bonificacaoTransportador.FiliasTransportador.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic filialTransportador in filiaisTransportadores)
                    codigos.Add((int)filialTransportador.Codigo);

                List<Dominio.Entidades.Empresa> tiposDeletar = bonificacaoTransportador.FiliasTransportador.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Empresa filialTransportadorDeletar in tiposDeletar)
                    bonificacaoTransportador.FiliasTransportador.Remove(filialTransportadorDeletar);
            }
            else
                bonificacaoTransportador.FiliasTransportador = new List<Dominio.Entidades.Empresa>();

            foreach (var filialTransportador in filiaisTransportadores)
            {

                int.TryParse((string)filialTransportador.Codigo, out int codigoFilialTransportador);
                Dominio.Entidades.Empresa existeFilialTransportador = repositorioEmpresa.BuscarPorCodigo(codigoFilialTransportador);

                if (existeFilialTransportador == null)
                    continue;

                bool existeFilialTransportadorCadastrada = bonificacaoTransportador.FiliasTransportador.Any(o => o.Codigo == existeFilialTransportador.Codigo);

                if (!existeFilialTransportadorCadastrada)
                    bonificacaoTransportador.FiliasTransportador.Add(existeFilialTransportador);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CNPJ", "CNPJ", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Empresa", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipos de Carga", "TipoDeCargas", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Filiais", "Filiais", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Percentual", "Percentual", 10, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.right, false);

                grid.AdicionarCabecalho("Descricao", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                int totalRegistros = repositorioBonificacaoTransportador.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador> listaBonificacaoTransportador = (totalRegistros > 0) ? repositorioBonificacaoTransportador.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>();

                var listaBonificacaoTransportadorRetornar = (
                    from bonificacaoTransportador in listaBonificacaoTransportador
                    select new
                    {
                        bonificacaoTransportador.Codigo,
                        Empresa = bonificacaoTransportador.Empresa.RazaoSocial + " (" + bonificacaoTransportador.Empresa.Localidade.DescricaoCidadeEstado + " )",
                        Filiais = string.Join(", ", bonificacaoTransportador.Filiais.Select(o => o.Descricao)),
                        TipoDeCargas = string.Join(", ", bonificacaoTransportador.TiposDeCarga.Select(o => o.Descricao)),
                        Vigencia = bonificacaoTransportador.DescricaoVigencia,
                        Percentual = bonificacaoTransportador.Percentual.ToString("n2"),
                        Tipo = bonificacaoTransportador.Tipo.ObterDescricao(),
                        bonificacaoTransportador.DescricaoAtivo,
                        CNPJ = bonificacaoTransportador.Empresa.CNPJ_Formatado,
                        bonificacaoTransportador.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaBonificacaoTransportadorRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Privados
    }
}
