using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Imposto.OutrasAliquotas;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Text.RegularExpressions;

namespace SGT.WebAdmin.Controllers.OutrasAliquotas
{
    [CustomAuthorize(new string[] { "PesquisaAliquotaImposto" }, "Imposto/OutrasAliquotas")]
    public class OutrasAliquotasController : BaseController
    {
        #region Construtores

        public OutrasAliquotasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas = new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas();

                await PreencherOutrasAliquotas(outrasAliquotas, unitOfWork, cancellationToken);
                await ValidarOutrasAliquotas(outrasAliquotas, unitOfWork, cancellationToken);

                await repositorioOutrasAliquotas.InserirAsync(outrasAliquotas, Auditado);

                await unitOfWork.CommitChangesAsync();

                Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = await Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstanceAsync(unitOfWork);
                await servicoOutraAliquotaInstance.AtualizarOutraAliquotaInstanceAsync(unitOfWork);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigoOutrasAliquotas = Request.GetIntParam("CodigoOutrasAliquotas");
                Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork);
                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas = await repositorioOutrasAliquotas.BuscarPorCodigoAsync(codigoOutrasAliquotas, auditavel: true);

                if (outrasAliquotas == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                await PreencherOutrasAliquotas(outrasAliquotas, unitOfWork, cancellationToken);
                await ValidarOutrasAliquotas(outrasAliquotas, unitOfWork, cancellationToken);

                await repositorioOutrasAliquotas.AtualizarAsync(outrasAliquotas, Auditado);

                await unitOfWork.CommitChangesAsync();

                Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = await Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstanceAsync(unitOfWork);
                await servicoOutraAliquotaInstance.AtualizarOutraAliquotaInstanceAsync(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(await ObterGridPesquisa(unitOfWork, cancellationToken));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar aliquotas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaAliquotaImposto(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(await ObterGridPesquisaImpostos(unitOfWork, cancellationToken));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar aliquotas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarOutrasAliquotasImposto(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork, cancellationToken);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotasImposto = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unitOfWork, cancellationToken);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);

            try
            {
                int codigoOutrasAliquotas = Request.GetIntParam("CodigoOutrasAliquotas");
                int codigoOutrasAliquotasImposto = Request.GetIntParam("CodigoOutrasAliquotasImposto");
                int codigoMunicipio = Request.GetIntParam("Municipio");
                string siglaUF = Request.GetStringParam("UF");
                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto outrasAliquotasImposto = new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto();
                if (codigoOutrasAliquotasImposto > 0)
                    outrasAliquotasImposto = await repositorioOutrasAliquotasImposto.BuscarPorCodigoAsync(codigoOutrasAliquotasImposto, true);

                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas = await repositorioOutrasAliquotas.BuscarPorCodigoAsync(codigoOutrasAliquotas, false);
                if (outrasAliquotas == null)
                    throw new ControllerException("Não foi encontrado outras Alíquotas");

                await unitOfWork.StartAsync(cancellationToken);

                outrasAliquotasImposto.DataVigenciaFinal = Request.GetDateTimeParam("DataVigenciaFinal");
                outrasAliquotasImposto.DataVigenciaInicio = Request.GetDateTimeParam("DataVigenciaInicio");
                outrasAliquotasImposto.Aliquota = Request.GetDecimalParam("Aliquota");
                outrasAliquotasImposto.AliquotaMunicipio = Request.GetDecimalParam("AliquotaMunicipio");
                outrasAliquotasImposto.AliquotaUf = Request.GetDecimalParam("AliquotaUf");
                outrasAliquotasImposto.UF = await repositorioEstado.BuscarPorSiglaAsync(siglaUF);
                outrasAliquotasImposto.Municipio = codigoMunicipio > 0 ? new Dominio.Entidades.Localidade { Codigo = codigoMunicipio } : null;
                outrasAliquotasImposto.OutrasAliquotas = outrasAliquotas;
                outrasAliquotasImposto.Reducao = Request.GetDecimalParam("Reducao");
                outrasAliquotasImposto.ReducaoMunicipio = Request.GetDecimalParam("ReducaoMunicipio");
                outrasAliquotasImposto.ReducaoUf = Request.GetDecimalParam("ReducaoUf");
                outrasAliquotasImposto.TipoImposto = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto>("TipoImposto") ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.CBS;

                if (outrasAliquotasImposto.TipoImposto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.CBS)
                {
                    if (outrasAliquotasImposto.Aliquota < 0)
                        throw new ControllerException("O campo Alíquota é obrigatório e deve ser maior ou igual a zero.");

                    if (outrasAliquotasImposto.DataVigenciaInicio == DateTime.MinValue)
                        throw new ControllerException("O campo Data de Vigência Inicial é obrigatório.");

                    if (outrasAliquotasImposto.DataVigenciaFinal == DateTime.MinValue)
                        throw new ControllerException("O campo Data de Vigência Final é obrigatório.");
                }
                else if (outrasAliquotasImposto.TipoImposto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.IBS)
                {
                    if (outrasAliquotasImposto.AliquotaUf < 0)
                        throw new ControllerException("O campo Alíquota UF é obrigatório e deve ser maior ou igual a zero.");

                    if (outrasAliquotasImposto.DataVigenciaInicio == DateTime.MinValue)
                        throw new ControllerException("O campo Data de Vigência Inicial é obrigatório.");

                    if (outrasAliquotasImposto.DataVigenciaFinal == DateTime.MinValue)
                        throw new ControllerException("O campo Data de Vigência Final é obrigatório.");

                    if (outrasAliquotasImposto.UF == null)
                        throw new ControllerException("O campo UF é obrigatório.");
                }

                if (codigoOutrasAliquotasImposto > 0)
                    await repositorioOutrasAliquotasImposto.AtualizarAsync(outrasAliquotasImposto, Auditado);
                else
                    await repositorioOutrasAliquotasImposto.InserirAsync(outrasAliquotasImposto, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = await Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstanceAsync(unitOfWork);
                await servicoOutraAliquotaInstance.AtualizarOutraAliquotaInstanceAsync(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar configuração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirOutrasAliquotas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork, cancellationToken);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotasImposto = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unitOfWork, cancellationToken);

            try
            {
                int codigoOutrasAliquotas = Request.GetIntParam("CodigoOutrasAliquotas");
                if (codigoOutrasAliquotas <= 0)
                    throw new ControllerException("Código inválido.");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outraAliquota = await repositorioOutrasAliquotas.BuscarOutraAliquotaPorCodigoAsync(codigoOutrasAliquotas);

                if (outraAliquota == null)
                    throw new ControllerException("Registro não encontrado.");

                if (repositorioOutrasAliquotas.ValidarExistenciaOutrasAliquotasEmDocumentos(codigoOutrasAliquotas))
                    throw new ControllerException("Imposto já vinculado a Documento Fiscal.");

                IList<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto> outrasAliquotasImpostos = await repositorioOutrasAliquotasImposto.BuscarOutraAliquotaImpostoPorCodigoDeOutraAliquotaAsync(outraAliquota.Codigo);

                if (outrasAliquotasImpostos != null && outrasAliquotasImpostos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto aliquotaImposto in outrasAliquotasImpostos)
                    {
                        await repositorioOutrasAliquotasImposto.DeletarAsync(aliquotaImposto, Auditado);
                    }
                }

                await repositorioOutrasAliquotas.DeletarAsync(outraAliquota, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = await Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstanceAsync(unitOfWork);
                await servicoOutraAliquotaInstance.AtualizarOutraAliquotaInstanceAsync(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o imposto.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirOutrasAliquotasImposto(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotasImposto = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unitOfWork, cancellationToken);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork, cancellationToken);

            try
            {
                int codigoOutrasAliquotasImposto = Request.GetIntParam("CodigoOutrasAliquotasImposto");

                if (codigoOutrasAliquotasImposto <= 0)
                    throw new ControllerException("Código inválido.");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto outrasAliquotasImposto = await repositorioOutrasAliquotasImposto.BuscarOutraAliquotaImpostoPorCodigoAsync(codigoOutrasAliquotasImposto);

                if (repositorioOutrasAliquotas.ValidarExistenciaOutrasAliquotasEmDocumentos(outrasAliquotasImposto.OutrasAliquotas.Codigo))
                    throw new ControllerException("Imposto já vinculado a Documento Fiscal.");

                if (outrasAliquotasImposto == null)
                    throw new ControllerException("Registro não encontrado.");

                await repositorioOutrasAliquotasImposto.DeletarAsync(outrasAliquotasImposto, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = await Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstanceAsync(unitOfWork);
                await servicoOutraAliquotaInstance.AtualizarOutraAliquotaInstanceAsync(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o imposto.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarOutrasAliquotasImpostoPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unitOfWork, cancellationToken);

            try
            {
                int codigoOutrasAliquotasImposto = Request.GetIntParam("CodigoOutrasAliquotasImposto");

                if (codigoOutrasAliquotasImposto <= 0)
                    throw new ControllerException("Código inválido.");

                Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto outrasAliquotasImposto = await repositorioOutrasAliquotas.BuscarOutraAliquotaImpostoPorCodigoAsync(codigoOutrasAliquotasImposto);

                if (outrasAliquotasImposto == null)
                    throw new ControllerException("Registro não encontrado.");

                var resultado = new
                {
                    TipoImposto = outrasAliquotasImposto.TipoImposto,
                    ExibirIBS = outrasAliquotasImposto.TipoImposto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.IBS,
                    ExibirCBS = outrasAliquotasImposto.TipoImposto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.CBS,
                    Aliquota = outrasAliquotasImposto.Aliquota,
                    Reducao = outrasAliquotasImposto.Reducao,
                    DataVigenciaFinal = outrasAliquotasImposto.DataVigenciaFinal.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataVigenciaInicio = outrasAliquotasImposto.DataVigenciaInicio.ToString("dd/MM/yyyy") ?? string.Empty,
                    InclusaoDocumento = outrasAliquotasImposto.InclusaoDocumento ? 1 : 0,
                    AliquotaUF = outrasAliquotasImposto.AliquotaUf,
                    AliquotaMunicipio = outrasAliquotasImposto.AliquotaMunicipio,
                    UF = new { Descricao = outrasAliquotasImposto.UF?.Nome, Codigo = outrasAliquotasImposto.UF?.Sigla },
                    Municipio = new { Descricao = outrasAliquotasImposto.Municipio?.DescricaoCidadeEstado, outrasAliquotasImposto.Municipio?.Codigo },
                    ReducaoUF = outrasAliquotasImposto.ReducaoUf,
                    ReducaoMunicipio = outrasAliquotasImposto.ReducaoMunicipio,
                    CodigoOutrasAliquotasImposto = outrasAliquotasImposto.Codigo,
                    CodigoOutrasAliquotas = outrasAliquotasImposto.OutrasAliquotas?.Codigo
                };

                return new JsonpResult(resultado);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados do imposto.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task PreencherOutrasAliquotas(Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            outrasAliquotas.CodigoClassificacaoTributaria = Request.GetStringParam("CodigoClassificacaoTributaria");
            outrasAliquotas.CodigoIndicadorOperacao = Request.GetStringParam("CodigoIndicadorOperacao");        
            outrasAliquotas.CST = Request.GetStringParam("CST");
            outrasAliquotas.Ativo = Request.GetBoolParam("AtivoInativo");
            outrasAliquotas.TipoOperacao = (codigoTipoOperacao > 0) ? await repositorioTipoOperacao.BuscarPorCodigoAsync(codigoTipoOperacao) : null;
            outrasAliquotas.Descricao = $"{outrasAliquotas.CST} - {outrasAliquotas.CodigoClassificacaoTributaria}";
            outrasAliquotas.ZerarBase = Request.GetBoolParam("ZerarBase");
            outrasAliquotas.Exportacao = Request.GetBoolParam("Exportacao");
            outrasAliquotas.CalcularImpostoDocumento = Request.GetBoolParam("CalcularImpostoDocumento");
        }

        private async Task ValidarOutrasAliquotas(Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(outrasAliquotas.CST))
                throw new ControllerException("O campo CST é obrigatório.");

            if (!Regex.IsMatch(outrasAliquotas.CST, "^\\d{3}$"))
                throw new ControllerException("O campo CST deve conter exatamente 3 dígitos numéricos.");

            if (string.IsNullOrWhiteSpace(outrasAliquotas.CodigoClassificacaoTributaria))
                throw new ControllerException("O campo Código de Classificação Tributária é obrigatório.");

            if (!Regex.IsMatch(outrasAliquotas.CodigoClassificacaoTributaria, "^\\d{6}$"))
                throw new ControllerException("O campo Código de Classificação Tributária deve conter exatamente 6 dígitos numéricos.");

            if (!outrasAliquotas.Ativo)
                return;

            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork, cancellationToken);
            bool existeDuplicadoAtivo = await repositorioOutrasAliquotas.ExisteRegistroDuplicado(
                outrasAliquotas.CST,
                outrasAliquotas.CodigoClassificacaoTributaria,
                outrasAliquotas.TipoOperacao?.Codigo ?? 0,
                outrasAliquotas.Codigo
            );

            if (existeDuplicadoAtivo)
                throw new ControllerException("Já existe um imposto ativo cadastrado com essa configuração.");
        }

        private async Task<Grid> ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unidadeTrabalho, cancellationToken);

            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Ativo", "StatusAtividadeFormatado", 15, Align.left, true);
            grid.AdicionarCabecalho("StatusAtividade", "StatusAtividade", 15, Align.left, false, false);
            grid.AdicionarCabecalho("CodigoTipoOperacao", "CodigoTipoOperacao", 15, Align.left, false, false);
            grid.AdicionarCabecalho("CST", "CST", 15, Align.center, true);
            grid.AdicionarCabecalho("Código da Class. Tributária", "CodigoClassificacaoTributaria", 15, Align.center, true);
            grid.AdicionarCabecalho("Código Indicador de Operação (NFS-e)", "CodigoIndicadorOperacao", 15, Align.center, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Align.center, false);
            grid.AdicionarCabecalho("ZerarBase", "ZerarBase", 15, Align.left, false, false);
            grid.AdicionarCabecalho("Exportacao", "Exportacao", 15, Align.left, false, false);
            grid.AdicionarCabecalho("Somar Impostos no Documento", "CalcularImpostoDocumentoFormatado", 15, Align.left, true);
            grid.AdicionarCabecalho("CalcularImpostoDocumento", false);

            int totalLinhas = await repositorioOutrasAliquotas.ContarConsultaOutrasAliquotas();

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            if (parametrosConsulta.PropriedadeOrdenar == "StatusAtividadeFormatado")
                parametrosConsulta.PropriedadeOrdenar = "StatusAtividade";

            if (parametrosConsulta.PropriedadeOrdenar == "CalcularImpostoDocumentoFormatado")
                parametrosConsulta.PropriedadeOrdenar = "CalcularImpostoDocumento";

            IList<DadosOutrasAliquotas> dados = await repositorioOutrasAliquotas.ConsultaOutrasAliquotas(parametrosConsulta);

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        private async Task<Grid> ObterGridPesquisaImpostos(Repositorio.UnitOfWork unidadeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unidadeTrabalho, cancellationToken);
            int codigoOutrasAliquotas = Request.GetIntParam("CodigoOutrasAliquotas");

            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto tipoImposto = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto>("TipoImposto") ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.CBS;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Imposto", "TipoImpostoFormatado", 15, Align.left, true);

            if (tipoImposto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto.CBS)
            {
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 15, Align.center, true);
                grid.AdicionarCabecalho("% Redução", "PercentualReducao", 15, Align.center, true);
            }
            else
            {
                grid.AdicionarCabecalho("UF", "UF", 15, Align.left, true);
                grid.AdicionarCabecalho("Alíquota UF", "AliquotaUF", 15, Align.center, true);
                grid.AdicionarCabecalho("% Redução UF", "PercentualReducaoUF", 15, Align.center, true);
                grid.AdicionarCabecalho("Município", "Municipio", 15, Align.center, false);
                grid.AdicionarCabecalho("Alíquota Município", "AliquotaMunicipio", 15, Align.center, false);
                grid.AdicionarCabecalho("% Redução Município", "PercentualReducaoMunicipio", 15, Align.center, false);
            }

            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Align.center, false);

            int totalLinhas = await repositorioOutrasAliquotas.ContarConsultaOutrasAliquotasImposto(codigoOutrasAliquotas, tipoImposto);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            IList<DadosOutrasAliquotasImposto> dados = await repositorioOutrasAliquotas.ConsultaOutrasAliquotasImposto(parametrosConsulta, codigoOutrasAliquotas, tipoImposto);

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        #endregion
    }
}
