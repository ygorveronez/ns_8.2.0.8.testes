using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ConfiguracaoDescargaCliente")]
    public class ConfiguracaoDescargaClienteController : BaseController
    {
        #region Construtores

        public ConfiguracaoDescargaClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

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

            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracao = repositorioConfiguracaoDescargaCliente.BuscarPorCodigo(codigo);

                if (configuracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    configuracao.Codigo,
                    configuracao.Ativo,
                    Filial = new { Codigo = configuracao.Filial?.Codigo ?? 0, Descricao = configuracao.Filial?.Descricao ?? "" },
                    ModeloVeicular = new { Codigo = configuracao.ModeloVeicular?.Codigo ?? 0, Descricao = configuracao.ModeloVeicular?.Descricao ?? "" },
                    TipoCarga = new { Codigo = configuracao.TipoCarga?.Codigo ?? 0, Descricao = configuracao.TipoCarga?.Descricao ?? "" },
                    ComponentePreDescarga = new { Codigo = configuracao.ComponenteFretePreDescarga?.Codigo ?? 0, Descricao = configuracao.ComponenteFretePreDescarga?.Descricao ?? "" },
                    Situacao = configuracao.Situacao,
                    configuracao.Valor,
                    configuracao.ValorTonelada,
                    configuracao.ValorUnidade,
                    configuracao.ValorPallet,
                    configuracao.ValorAjudante,
                    InicioVigencia = configuracao.InicioVigencia.HasValue ? configuracao.InicioVigencia.Value.ToString("dd/MM/yyyy") : "",
                    FimVigencia = configuracao.FimVigencia.HasValue ? configuracao.FimVigencia.Value.ToString("dd/MM/yyyy") : "",
                    configuracao.ValorDePreDescarga,
                    Clientes = (
                        from o in configuracao.Clientes
                        select new
                        {
                            o.Descricao,
                            Codigo = o.CPF_CNPJ
                        }
                    ).ToList(),
                    GrupoClientes = (
                        from o in configuracao.GrupoPessoas
                        select new
                        {
                            o.Descricao,
                            Codigo = o.Codigo
                        }
                    ).ToList(),
                    TiposOperacoes = (
                        from o in configuracao.TiposOperacoes
                        select new
                        {
                            o.Descricao,
                            Codigo = o.Codigo
                        }
                    ).ToList(),
                    Transportadores = (
                        from o in configuracao.Transportadores
                        select new
                        {
                            o.Descricao,
                            o.Codigo
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

                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente = new Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente();
                Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente servicoConfiguracaoDescargaCliente = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                configuracaoDescargaCliente.DataCriacao = DateTime.Now;
                PreencherConfiguracaoDescargaCliente(ref configuracaoDescargaCliente, unitOfWork);
                ValidarConfiguracaoDescargaCliente(configuracaoDescargaCliente, unitOfWork);

                repositorioConfiguracaoDescargaCliente.Inserir(configuracaoDescargaCliente, Auditado);
                servicoConfiguracaoDescargaCliente.CriarAprovacao(configuracaoDescargaCliente, TipoServicoMultisoftware);
                repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoDescargaCliente);

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente = repositorioConfiguracaoDescargaCliente.BuscarPorCodigo(codigo, true);

                if (configuracaoDescargaCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente servicoConfiguracaoDescargaCliente = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                PreencherConfiguracaoDescargaCliente(ref configuracaoDescargaCliente, unitOfWork);
                ValidarConfiguracaoDescargaCliente(configuracaoDescargaCliente, unitOfWork);

                servicoConfiguracaoDescargaCliente.CriarAprovacao(configuracaoDescargaCliente, TipoServicoMultisoftware);
                repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoDescargaCliente, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracao = repositorioConfiguracaoDescargaCliente.BuscarPorCodigo(codigo);

                if (configuracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioConfiguracaoDescargaCliente.Deletar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro, pois o mesmo já possui vínculo com outros recursos do sistema.");

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente servicoConfiguracaoDescargaCliente = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };
                int totalRegistrosImportados = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente = new Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente()
                        {
                            Ativo = true,
                            DataCriacao = DateTime.Now,
                            Usuario = this.Usuario,
                            DataUltimaAlteracao = DateTime.Now
                        };

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colStatus = (from obj in linha.Colunas where obj.NomeCampo == "Status" select obj).FirstOrDefault();

                        if (colStatus?.Valor != null)
                        {
                            string status = Utilidades.String.RemoveAccents((string)colStatus.Valor.Trim().ToUpper());

                            if (status != "ATIVO")
                            {
                                configuracaoDescargaCliente.Ativo = false;
                                configuracaoDescargaCliente.DataInativacao = DateTime.Now;
                            }
                            else
                                configuracaoDescargaCliente.Ativo = true;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoFilial" select obj).FirstOrDefault();
                        string codigoIntegracaoFilial = "";

                        if (colFilial?.Valor != null)
                        {
                            codigoIntegracaoFilial = colFilial.Valor;

                            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigoIntegracao(codigoIntegracaoFilial);

                            if (filial == null)
                                throw new ControllerException("Filial não encontrada por esse Código de Integração.");

                            configuracaoDescargaCliente.Filial = filial;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoModeloVeicular" select obj).FirstOrDefault();
                        string codigoIntegracaoModeloVeicular = "";
                        if (colModeloVeicular?.Valor != null)
                        {
                            codigoIntegracaoModeloVeicular = colModeloVeicular.Valor;

                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorCodigoIntegracao(codigoIntegracaoModeloVeicular);

                            if (modeloVeicularCarga == null)
                                throw new ControllerException("Modelo Veicular não encontrado por esse Código de Integração.");

                            configuracaoDescargaCliente.ModeloVeicular = modeloVeicularCarga;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoDeCarga = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTipoDeCarga" select obj).FirstOrDefault();
                        string codigoIntegracaoTipoDeCarga = "";
                        if (colTipoDeCarga?.Valor != null)
                        {
                            codigoIntegracaoTipoDeCarga = colTipoDeCarga.Valor;

                            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(codigoIntegracaoTipoDeCarga);

                            if (tipoDeCarga == null)
                                throw new ControllerException("Tipo de Carga não encontrado por esse Código de Integração.");

                            configuracaoDescargaCliente.TipoCarga = tipoDeCarga;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();
                        decimal valor = 0;
                        if (colValor?.Valor != null && !string.IsNullOrWhiteSpace((string)colValor.Valor))
                        {
                            string strValor = (string)colValor.Valor;
                            decimal.TryParse(strValor, out valor);
                        }

                        configuracaoDescargaCliente.Valor = valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorTonelada = (from obj in linha.Colunas where obj.NomeCampo == "ValorTonelada" select obj).FirstOrDefault();
                        decimal valorTonelada = 0;
                        if (colValorTonelada?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorTonelada.Valor))
                        {
                            string strValorTonelada = (string)colValorTonelada.Valor;
                            decimal.TryParse(strValorTonelada, out valorTonelada);
                        }

                        configuracaoDescargaCliente.ValorTonelada = valorTonelada;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorUnidade = (from obj in linha.Colunas where obj.NomeCampo == "ValorUnidade" select obj).FirstOrDefault();
                        decimal valorUnidade = 0;
                        if (colValorUnidade?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorUnidade.Valor))
                        {
                            string strValorUnidade = (string)colValorUnidade.Valor;
                            decimal.TryParse(strValorUnidade, out valorUnidade);
                        }

                        configuracaoDescargaCliente.ValorUnidade = valorUnidade;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorPallet = (from obj in linha.Colunas where obj.NomeCampo == "ValorPallet" select obj).FirstOrDefault();
                        decimal valorPallet = 0;
                        if (colValorPallet?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorPallet.Valor))
                        {
                            string strValorPallet = (string)colValorPallet.Valor;
                            decimal.TryParse(strValorPallet, out valorPallet);
                        }

                        configuracaoDescargaCliente.ValorPallet = valorPallet;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataInicioVigencia = (from obj in linha.Colunas where obj.NomeCampo == "DataInicioVigencia" select obj).FirstOrDefault();
                        string dataDescarga = "";
                        DateTime? dataInicioVigencia = null;

                        if (colDataInicioVigencia?.Valor != null)
                        {
                            dataDescarga = colDataInicioVigencia.Valor;
                            double.TryParse(dataDescarga, out double dataFormatoExcel);
                            if (dataFormatoExcel > 0)
                                dataInicioVigencia = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                            else if (!string.IsNullOrWhiteSpace(dataDescarga))
                            {
                                DateTime dataInicialTest;
                                DateTime.TryParse(dataDescarga, out dataInicialTest);
                                if (dataInicialTest > DateTime.MinValue)
                                    dataInicioVigencia = dataInicialTest;
                            }
                        }

                        configuracaoDescargaCliente.InicioVigencia = dataInicioVigencia;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataFimVigencia = (from obj in linha.Colunas where obj.NomeCampo == "DataFimVigencia" select obj).FirstOrDefault();
                        DateTime? dataFimVigencia = null;

                        if (colDataFimVigencia?.Valor != null)
                        {
                            dataDescarga = colDataFimVigencia.Valor;
                            double.TryParse(dataDescarga, out double dataFormatoExcel);
                            if (dataFormatoExcel > 0)
                                dataFimVigencia = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                            else if (!string.IsNullOrWhiteSpace(dataDescarga))
                            {
                                DateTime dataInicialTest;
                                DateTime.TryParse(dataDescarga, out dataInicialTest);
                                if (dataInicialTest > DateTime.MinValue)
                                    dataFimVigencia = dataInicialTest;
                            }
                        }

                        configuracaoDescargaCliente.FimVigencia = dataFimVigencia;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colClientes = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoClientes" select obj).FirstOrDefault();
                        string clientes = "";
                        if (colClientes?.Valor != null)
                        {
                            clientes = colClientes.Valor;
                            string[] listaClientes = clientes.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            List<Dominio.Entidades.Cliente> listaCliente = new List<Dominio.Entidades.Cliente>();

                            for (int j = 0; j < listaClientes.Length; j++)
                            {
                                Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCodigoIntegracao(listaClientes[j]);

                                if (cliente == null)
                                    throw new ControllerException("Cliente não encontrado pelo Código de Integração informado.");

                                listaCliente.Add(cliente);
                            }

                            configuracaoDescargaCliente.Clientes = listaCliente;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoClientes = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoGrupoClientes" select obj).FirstOrDefault();
                        string grupoClientes = "";
                        if (colGrupoClientes?.Valor != null)
                        {
                            grupoClientes = colGrupoClientes.Valor;
                            string[] listaGrupoClientes = grupoClientes.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> listaGrupoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

                            for (int j = 0; j < listaGrupoClientes.Length; j++)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoCliente = repositorioGrupoPessoas.BuscarPorCodigoIntegracao(listaGrupoClientes[j]);

                                if (grupoCliente == null)
                                    throw new ControllerException("Grupo de Cliente não encontrada por esse Código de Integração.");

                                listaGrupoCliente.Add(grupoCliente);
                            }

                            configuracaoDescargaCliente.GrupoPessoas = listaGrupoCliente;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTipoOperacao" select obj).FirstOrDefault();
                        string tiposOperacao = "";
                        if (colTipoOperacao?.Valor != null)
                        {
                            tiposOperacao = colTipoOperacao.Valor;
                            string[] listaTipoOperacoes = tiposOperacao.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                            for (int j = 0; j < listaTipoOperacoes.Length; j++)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(listaTipoOperacoes[j]);

                                if (tipoOperacao == null)
                                    throw new ControllerException("Tipo de Operação não encontrada por esse Código de Integração.");

                                listaTipoOperacao.Add(tipoOperacao);
                            }

                            configuracaoDescargaCliente.TiposOperacoes = listaTipoOperacao;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTransportadores = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTransportador" select obj).FirstOrDefault();
                        string transportadores = "";
                        if (colTransportadores?.Valor != null)
                        {
                            transportadores = colTransportadores.Valor;
                            string[] listaTransportadores = transportadores.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            List<Dominio.Entidades.Empresa> listaTransportador = new List<Dominio.Entidades.Empresa>();

                            for (int j = 0; j < listaTransportadores.Length; j++)
                            {
                                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarEmpresaPorCNPJ(Utilidades.String.OnlyNumbers(listaTransportadores[j]));

                                if (transportador == null)
                                    throw new ControllerException("CPF/CNPJ Transportadores inválido.");

                                listaTransportador.Add(transportador);
                            }

                            configuracaoDescargaCliente.Transportadores = listaTransportador;
                        }

                        if ((configuracaoDescargaCliente.Valor == 0m) && (configuracaoDescargaCliente.ValorTonelada == 0m) && (configuracaoDescargaCliente.ValorUnidade == 0m) && (configuracaoDescargaCliente.ValorPallet == 0m))
                            throw new ControllerException("Ao menos um dos campos de valor deve ser informado.");

                        if (configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente && !configuracaoDescargaCliente.InicioVigencia.HasValue && !configuracaoDescargaCliente.FimVigencia.HasValue)
                            throw new ControllerException("O início ou o fim da vigência deve ser informado");

                        List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeis = ObterConfiguracoesCompativeis(configuracaoDescargaCliente, configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente, unitOfWork);
                        bool permitirAdicionarConfiguracaoDescargaCliente = true;

                        if (!configuracaoDescargaCliente.Ativo)
                        {
                            permitirAdicionarConfiguracaoDescargaCliente = false;

                            if (configuracoesCompativeis.Count > 0)
                            {
                                foreach (var configuracaoCompativel in configuracoesCompativeis)
                                {
                                    configuracaoCompativel.Ativo = configuracaoDescargaCliente.Ativo;
                                    configuracaoCompativel.DataUltimaAlteracao = DateTime.Now;

                                    if (!configuracaoCompativel.Ativo)
                                        configuracaoCompativel.DataInativacao = DateTime.Now;

                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoDescargaCliente, "Inativado via importação de planilha.", unitOfWork);
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativel);
                                }
                            }
                        }
                        else if (configuracoesCompativeis.Count > 0)
                        {
                            if (!configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente)
                                throw new ControllerException("Já existe configuração compatível com os dados informados.");

                            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeisSemFimVigencia = configuracoesCompativeis.Where(configuracao => !configuracao.FimVigencia.HasValue).ToList();
                            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeisSemInicioVigencia = configuracoesCompativeis.Where(configuracao => !configuracao.InicioVigencia.HasValue).ToList();
                            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeisComVigenciaCompleta = configuracoesCompativeis.Where(configuracao => configuracao.InicioVigencia.HasValue && configuracao.FimVigencia.HasValue).ToList();

                            if (configuracoesCompativeisSemFimVigencia.Count > 0)
                            {
                                if (!configuracaoDescargaCliente.FimVigencia.HasValue)
                                    throw new ControllerException("Já existe configuração compatível com os dados informados sem o fim da vigência.");

                                foreach (Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoCompativelAjustar in configuracoesCompativeisSemFimVigencia)
                                {
                                    if (configuracaoDescargaCliente.InicioVigencia.HasValue && (configuracaoDescargaCliente.InicioVigencia > configuracaoCompativelAjustar.InicioVigencia))
                                        throw new ControllerException($"Já existe configuração compatível com os dados informados com vigência que não permite alteração ({configuracaoCompativelAjustar.DescricaoVigencia}).");

                                    configuracaoCompativelAjustar.InicioVigencia = configuracaoDescargaCliente.FimVigencia.Value.AddDays(1);
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o início da vigência via importação de planilha", unitOfWork);
                                }
                            }

                            if (configuracoesCompativeisSemInicioVigencia.Count > 0)
                            {
                                if (!configuracaoDescargaCliente.InicioVigencia.HasValue)
                                    throw new ControllerException("Já existe configuração compatível com os dados informados sem o início da vigência.");

                                foreach (Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoCompativelAjustar in configuracoesCompativeisSemInicioVigencia)
                                {
                                    if (configuracaoDescargaCliente.FimVigencia.HasValue && (configuracaoDescargaCliente.FimVigencia < configuracaoCompativelAjustar.FimVigencia))
                                        throw new ControllerException($"Já existe configuração compatível com os dados informados com vigência que não permite alteração ({configuracaoCompativelAjustar.DescricaoVigencia}).");

                                    configuracaoCompativelAjustar.FimVigencia = configuracaoDescargaCliente.InicioVigencia.Value.AddDays(-1);
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o fim da vigência via importação de planilha", unitOfWork);
                                }
                            }

                            foreach (Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoCompativelAjustar in configuracoesCompativeisComVigenciaCompleta)
                            {
                                if (!configuracaoDescargaCliente.InicioVigencia.HasValue)
                                {
                                    if (configuracaoDescargaCliente.FimVigencia >= configuracaoCompativelAjustar.FimVigencia)
                                        throw new ControllerException($"Já existe configuração compatível com os dados informados com vigência que não permite alteração ({configuracaoCompativelAjustar.DescricaoVigencia}).");

                                    configuracaoCompativelAjustar.InicioVigencia = configuracaoDescargaCliente.FimVigencia.Value.AddDays(1);
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o início da vigência via importação de planilha", unitOfWork);
                                }
                                else if (!configuracaoDescargaCliente.FimVigencia.HasValue)
                                {
                                    if (configuracaoDescargaCliente.InicioVigencia == configuracaoCompativelAjustar.InicioVigencia)
                                    {
                                        permitirAdicionarConfiguracaoDescargaCliente = false;
                                        configuracaoCompativelAjustar.FimVigencia = configuracaoDescargaCliente.FimVigencia;
                                        configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                        servicoConfiguracaoDescargaCliente.CriarAprovacao(configuracaoCompativelAjustar, TipoServicoMultisoftware);
                                        repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o fim da vigência via importação de planilha", unitOfWork);
                                    }
                                    else
                                    {
                                        if (configuracaoDescargaCliente.InicioVigencia < configuracaoCompativelAjustar.InicioVigencia)
                                            throw new ControllerException($"Já existe configuração compatível com os dados informados com vigência que não permite alteração ({configuracaoCompativelAjustar.DescricaoVigencia}).");

                                        configuracaoCompativelAjustar.FimVigencia = configuracaoDescargaCliente.InicioVigencia.Value.AddDays(-1);
                                        configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                        repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o fim da vigência via importação de planilha", unitOfWork);
                                    }
                                }
                                else if ((configuracaoDescargaCliente.InicioVigencia > configuracaoCompativelAjustar.InicioVigencia) && (configuracaoDescargaCliente.FimVigencia >= configuracaoCompativelAjustar.FimVigencia))
                                {
                                    configuracaoCompativelAjustar.FimVigencia = configuracaoDescargaCliente.InicioVigencia.Value.AddDays(-1);
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o fim da vigência via importação de planilha", unitOfWork);
                                }
                                else if ((configuracaoDescargaCliente.FimVigencia < configuracaoCompativelAjustar.FimVigencia) && (configuracaoDescargaCliente.InicioVigencia <= configuracaoCompativelAjustar.InicioVigencia))
                                {
                                    configuracaoCompativelAjustar.InicioVigencia = configuracaoDescargaCliente.FimVigencia.Value.AddDays(1);
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o início da vigência via importação de planilha", unitOfWork);
                                }
                                else if (configuracaoDescargaCliente.InicioVigencia == configuracaoCompativelAjustar.InicioVigencia)
                                {
                                    permitirAdicionarConfiguracaoDescargaCliente = false;
                                    configuracaoCompativelAjustar.FimVigencia = configuracaoDescargaCliente.FimVigencia;
                                    configuracaoCompativelAjustar.DataUltimaAlteracao = DateTime.Now;
                                    servicoConfiguracaoDescargaCliente.CriarAprovacao(configuracaoCompativelAjustar, TipoServicoMultisoftware);
                                    repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoCompativelAjustar);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCompativelAjustar, "Alterado o fim da vigência via importação de planilha", unitOfWork);
                                }
                                else
                                    throw new ControllerException($"Já existe configuração compatível com os dados informados com vigência que não permite alteração ({configuracaoCompativelAjustar.DescricaoVigencia}).");
                            }
                        }

                        if (permitirAdicionarConfiguracaoDescargaCliente)
                        {
                            repositorioConfiguracaoDescargaCliente.Inserir(configuracaoDescargaCliente);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoDescargaCliente, "Adicionado via importação de planilha.", unitOfWork);
                            servicoConfiguracaoDescargaCliente.CriarAprovacao(configuracaoDescargaCliente, TipoServicoMultisoftware);
                            repositorioConfiguracaoDescargaCliente.Atualizar(configuracaoDescargaCliente);
                        }

                        unitOfWork.CommitChanges();

                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i));
                        totalRegistrosImportados++;
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(excecao.Message, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = totalRegistrosImportados;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Filial", Propriedade = "CodigoIntegracaoFilial", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Modelo Veicular", Propriedade = "CodigoIntegracaoModeloVeicular", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Tipo de Carga", Propriedade = "CodigoIntegracaoTipoDeCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Valor", Propriedade = "Valor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Valor por Tonelada", Propriedade = "ValorTonelada", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Valor por Unidade", Propriedade = "ValorUnidade", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Valor por Pallet", Propriedade = "ValorPallet", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Início de Vigência", Propriedade = "DataInicioVigencia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Fim da Vigência", Propriedade = "DataFimVigencia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Clientes", Propriedade = "CodigoIntegracaoClientes", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Grupo de Clientes", Propriedade = "CodigoIntegracaoGrupoClientes", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Tipo de Operação", Propriedade = "CodigoIntegracaoTipoOperacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Status", Propriedade = "Status", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Transportadores", Propriedade = "CodigoIntegracaoTransportador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> ObterConfiguracoesCompativeis(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente, bool somenteComVigenciaInformada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaClienteCompativel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaClienteCompativel()
            {
                CodigoConfiguracaoDescargaClienteDesconsiderar = configuracaoDescargaCliente.Codigo,
                CodigoFilial = configuracaoDescargaCliente.Filial?.Codigo ?? 0,
                CodigoModeloVeicularCarga = configuracaoDescargaCliente.ModeloVeicular?.Codigo ?? 0,
                CodigoTipoCarga = configuracaoDescargaCliente.TipoCarga?.Codigo ?? 0,
                CodigosGruposClientes = configuracaoDescargaCliente.GrupoPessoas?.Select(grupoPessoas => grupoPessoas.Codigo).ToList(),
                CodigosTiposOperacao = configuracaoDescargaCliente.TiposOperacoes?.Select(tipoOperacao => tipoOperacao.Codigo).ToList(),
                CpfCnpjClientes = configuracaoDescargaCliente.Clientes?.Select(cliente => cliente.CPF_CNPJ).ToList(),
                SomenteComVigenciaInformada = somenteComVigenciaInformada,
                CodigosTransportadores = configuracaoDescargaCliente.Transportadores?.Select(t => t.Codigo).ToList()

            };

            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeis = repositorioConfiguracaoDescargaCliente.BuscarConfiguracoesAtivasCompativeis(filtrosPesquisa);

            if (!somenteComVigenciaInformada || (configuracoesCompativeis.Count == 0))
                return configuracoesCompativeis;

            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeisComVigenciaConflitante;

            if (configuracaoDescargaCliente.InicioVigencia.HasValue && configuracaoDescargaCliente.FimVigencia.HasValue)
                configuracoesCompativeisComVigenciaConflitante = configuracoesCompativeis.Where(configuracaoCompativel =>
                    (!configuracaoCompativel.InicioVigencia.HasValue && (configuracaoCompativel.FimVigencia >= configuracaoDescargaCliente.InicioVigencia)) ||
                    (!configuracaoCompativel.FimVigencia.HasValue && (configuracaoCompativel.InicioVigencia <= configuracaoDescargaCliente.FimVigencia)) ||
                    (
                        configuracaoCompativel.InicioVigencia.HasValue && configuracaoCompativel.FimVigencia.HasValue && (
                            ((configuracaoDescargaCliente.InicioVigencia >= configuracaoCompativel.InicioVigencia) && (configuracaoDescargaCliente.InicioVigencia <= configuracaoCompativel.FimVigencia)) ||
                            ((configuracaoDescargaCliente.FimVigencia >= configuracaoCompativel.InicioVigencia) && (configuracaoDescargaCliente.FimVigencia <= configuracaoCompativel.FimVigencia)) ||
                            ((configuracaoDescargaCliente.InicioVigencia < configuracaoCompativel.InicioVigencia) && (configuracaoDescargaCliente.FimVigencia > configuracaoCompativel.FimVigencia))
                        )
                    )
                ).ToList();
            else if (configuracaoDescargaCliente.InicioVigencia.HasValue)
                configuracoesCompativeisComVigenciaConflitante = configuracoesCompativeis.Where(configuracaoCompativel =>
                    (!configuracaoCompativel.FimVigencia.HasValue || (configuracaoCompativel.FimVigencia >= configuracaoDescargaCliente.InicioVigencia))
                ).ToList();
            else
                configuracoesCompativeisComVigenciaConflitante = configuracoesCompativeis.Where(configuracaoCompativel =>
                    (!configuracaoCompativel.InicioVigencia.HasValue || (configuracaoCompativel.InicioVigencia <= configuracaoDescargaCliente.FimVigencia))
                ).ToList();

            return configuracoesCompativeisComVigenciaConflitante;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("ModeloVeicular").Nome("Modelo Veicular").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("TipoCarga").Nome("Tipo de Carga").Tamanho(12).Align(Models.Grid.Align.left);
                grid.Prop("Valor").Nome("Valor").Tamanho(5).Align(Models.Grid.Align.right);
                grid.Prop("ValorTonelada").Nome("Valor por tonelada").Tamanho(5).Align(Models.Grid.Align.right);
                grid.AdicionarCabecalho("Valor por unidade", "ValorUnidade", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor por pallet", "ValorPallet", 5, Models.Grid.Align.right, false);
                grid.Prop("Ativo").Nome("Status").Tamanho(3).Align(Models.Grid.Align.right);
                grid.Prop("Vigencia").Nome("Vigência").Tamanho(10).Align(Models.Grid.Align.center);
                grid.Prop("Situacao").Nome("Situação").Tamanho(5).Align(Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaCliente()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                    Situacao = Request.GetNullableEnumParam<SituacaoAjusteConfiguracaoDescargaCliente>("Situacao"),
                    Status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                    SomenteVigentes = Request.GetBoolParam("SomenteVigentes"),
                    DataVigenciaInicial = Request.GetDateTimeParam("DataVigenciaInicial"),
                    DataVigenciaFinal = Request.GetDateTimeParam("DataVigenciaFinal"),
                    CodigosModelosVeiculares = Request.GetListParam<int>("ModelosVeiculares")
                };
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                int totalRegistros = repositorioConfiguracaoDescargaCliente.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesDescargaCliente = (totalRegistros > 0) ? repositorioConfiguracaoDescargaCliente.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();

                var configuracoesDescargaClienteRetornar = (
                    from configuracaoDescargaCliente in configuracoesDescargaCliente
                    select new
                    {
                        configuracaoDescargaCliente.Codigo,
                        Filial = configuracaoDescargaCliente.Filial?.Descricao ?? string.Empty,
                        ModeloVeicular = configuracaoDescargaCliente.ModeloVeicular?.Descricao ?? string.Empty,
                        TipoCarga = configuracaoDescargaCliente.TipoCarga?.Descricao ?? string.Empty,
                        Valor = configuracaoDescargaCliente.Valor != 0 ? configuracaoDescargaCliente.Valor.ToString("n2") : string.Empty,
                        ValorTonelada = configuracaoDescargaCliente.ValorTonelada != 0 ? configuracaoDescargaCliente.ValorTonelada.ToString("n2") : string.Empty,
                        ValorUnidade = configuracaoDescargaCliente.ValorUnidade != 0 ? configuracaoDescargaCliente.ValorUnidade.ToString("n2") : string.Empty,
                        ValorPallet = configuracaoDescargaCliente.ValorPallet != 0 ? configuracaoDescargaCliente.ValorPallet.ToString("n2") : string.Empty,
                        Situacao = configuracaoDescargaCliente.Situacao.ObterDescricao(),
                        Ativo = configuracaoDescargaCliente.Ativo.ObterDescricaoAtivo(),
                        Vigencia = configuracaoDescargaCliente.DescricaoVigencia,
                        DT_RowColor = configuracaoDescargaCliente.Situacao.ObterCor()
                    }
                ).ToList();

                grid.AdicionaRows(configuracoesDescargaClienteRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return $"{propriedadeOrdenar}.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return $"{propriedadeOrdenar}.Descricao";

            if (propriedadeOrdenar == "ModeloVeicular")
                return $"{propriedadeOrdenar}.Descricao";

            return propriedadeOrdenar;
        }

        private void PreencherConfiguracaoDescargaCliente(ref Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositoriocomponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            configuracaoDescargaCliente.Filial = repositorioFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            configuracaoDescargaCliente.TipoCarga = repositorioTipoCarga.BuscarPorCodigo(Request.GetIntParam("TipoCarga"));
            configuracaoDescargaCliente.ModeloVeicular = repositorioModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicular"));
            configuracaoDescargaCliente.Ativo = Request.GetBoolParam("Ativo");
            configuracaoDescargaCliente.Valor = Request.GetDecimalParam("Valor");
            configuracaoDescargaCliente.InicioVigencia = Request.GetNullableDateTimeParam("InicioVigencia");
            configuracaoDescargaCliente.FimVigencia = Request.GetNullableDateTimeParam("FimVigencia");
            configuracaoDescargaCliente.ComponenteFretePreDescarga = repositoriocomponenteFrete.BuscarPorCodigo(Request.GetIntParam("ComponentePreDescarga"));
            configuracaoDescargaCliente.ValorDePreDescarga = Request.GetBoolParam("ValorDePreDescarga");
            configuracaoDescargaCliente.ValorTonelada = Request.GetDecimalParam("ValorTonelada");
            configuracaoDescargaCliente.ValorUnidade = Request.GetDecimalParam("ValorUnidade");
            configuracaoDescargaCliente.ValorPallet = Request.GetDecimalParam("ValorPallet");
            configuracaoDescargaCliente.ValorAjudante = Request.GetDecimalParam("ValorAjudante");
            configuracaoDescargaCliente.DataUltimaAlteracao = DateTime.Now;
            configuracaoDescargaCliente.Usuario = this.Usuario;

            if (!configuracaoDescargaCliente.Ativo)
                configuracaoDescargaCliente.DataInativacao = DateTime.Now;

            List<double> cpfCnpjClientes = JsonConvert.DeserializeObject<List<double>>(Request.GetStringParam("Clientes"));
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Cliente> clientes = repositorioCliente.BuscarPorCPFCNPJs(cpfCnpjClientes);

            List<int> codigosGruposPessoas = JsonConvert.DeserializeObject<List<int>>(Request.GetStringParam("GrupoClientes"));
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = repositorioGrupoPessoas.BuscarPorCodigos(codigosGruposPessoas);

            List<dynamic> dynTiposOperacoes = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("TiposOperacoes"));
            List<int> codigosTiposOperacao = (from obj in dynTiposOperacoes select ((string)obj.Tipo.Codigo).ToInt()).ToList();
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = repositorioTipoOperacao.BuscarPorCodigos(codigosTiposOperacao);

            List<int> codigosTransportadores = Request.GetListParam<int>("Transportadores");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            List<Dominio.Entidades.Empresa> transportadores = repEmpresa.BuscarPorCodigos(codigosTransportadores);

            if (configuracaoDescargaCliente.Clientes == null)
                configuracaoDescargaCliente.Clientes = new List<Dominio.Entidades.Cliente>();

            if (configuracaoDescargaCliente.GrupoPessoas == null)
                configuracaoDescargaCliente.GrupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            if (configuracaoDescargaCliente.TiposOperacoes == null)
                configuracaoDescargaCliente.TiposOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (configuracaoDescargaCliente.Transportadores == null)
                configuracaoDescargaCliente.Transportadores = new List<Dominio.Entidades.Empresa>();

            configuracaoDescargaCliente.Clientes.Clear();
            configuracaoDescargaCliente.GrupoPessoas.Clear();
            configuracaoDescargaCliente.TiposOperacoes.Clear();
            configuracaoDescargaCliente.Transportadores.Clear();

            foreach (Dominio.Entidades.Cliente cliente in clientes)
                configuracaoDescargaCliente.Clientes.Add(cliente);

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa in gruposPessoas)
                configuracaoDescargaCliente.GrupoPessoas.Add(grupoPessoa);

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacoes)
                configuracaoDescargaCliente.TiposOperacoes.Add(tipoOperacao);

            foreach (Dominio.Entidades.Empresa transportador in transportadores)
                configuracaoDescargaCliente.Transportadores.Add(transportador);
        }

        private void ValidarConfiguracaoDescargaCliente(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente configuracaoDescargaCliente, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracaoDescargaCliente.Ativo)
                return;

            if ((configuracaoDescargaCliente.Valor == 0m) && (configuracaoDescargaCliente.ValorTonelada == 0m) && (configuracaoDescargaCliente.ValorUnidade == 0m) && (configuracaoDescargaCliente.ValorPallet == 0m))
                throw new ControllerException("Ao menos um dos campos de valor deve ser informado.");

            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            if (configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente && !configuracaoDescargaCliente.InicioVigencia.HasValue && !configuracaoDescargaCliente.FimVigencia.HasValue)
                throw new ControllerException("O início ou o fim da vigência deve ser informado");

            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> configuracoesCompativeis = ObterConfiguracoesCompativeis(configuracaoDescargaCliente, configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente, unitOfWork);

            if (configuracoesCompativeis.Count == 0)
                return;

            if (!configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente)
                throw new ControllerException($"Já existe configuração compatível com os dados informados.");

            throw new ControllerException($"Já existe configuração compatível com os dados informados com as seguintes vigências: {string.Join(" - ", configuracoesCompativeis.Select(configuracao => configuracao.DescricaoVigencia))}");
        }

        #endregion Métodos Privados
    }
}
