using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Seguros
{
    [CustomAuthorize("Seguros/ApoliceSeguro")]
    public class ApoliceSeguroController : BaseController
    {
		#region Construtores

		public ApoliceSeguroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                string numeroAverbacao = Request.Params("NumeroAverbacao");
                string numeroApolice = Request.Params("NumeroApolice");
                string descricaoApolice = Request.GetStringParam("Descricao") ?? "";

                DateTime inicioVigencia, fimVigencia;
                DateTime.TryParseExact(Request.Params("InicioVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out inicioVigencia);
                DateTime.TryParseExact(Request.Params("FimVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fimVigencia);

                int.TryParse(Request.Params("Seguradora"), out int codigoSeguradora);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);


                List<int> codigosFilialMatriz = new List<int>();
                Dominio.Entidades.Empresa transportador = new Dominio.Entidades.Empresa();
                bool usuarioMultiTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;

                if (usuarioMultiTransportador)
                {
                    codigoEmpresa = Empresa.Codigo;
                    transportador = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    codigosFilialMatriz = repEmpresa.BuscarCodigoMatrizEFiliaisPorRaizCNPJ(transportador.RaizCnpj);
                }

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                bool validar;
                bool.TryParse(Request.Params("Validar"), out validar);

                bool somenteNaoVencidos = Request.GetBoolParam("SomenteNaoVencidos");
                bool exibirEmbarcador = Request.GetBoolParam("ExibirEmbarcador");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa ativa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa>("Ativa");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavelAux = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro>("Responsavel");
                if (responsavelAux != 0)
                    responsavel = responsavelAux;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ValorLimiteApolice", false);
                grid.AdicionarCabecalho("EnumResponsavel", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Seguradora, "Seguradora", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Responsavel, "Responsavel", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Apolice, "NumeroApolice", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Averbacao, "NumeroAverbacao", 20, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Transportador, "Empresa", 20, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Inicio, "InicioVigencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Fim, "FimVigencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Ativa, "Ativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ApoliceSeguro.Descricao, "DescricaoApolice", 20, Models.Grid.Align.left, true, (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS));

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApolice = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);

                Dominio.Entidades.Cliente pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> listaApolices = repApolice.Consultar(validar, numeroApolice, numeroAverbacao, codigoSeguradora, codigoGrupoPessoas, pessoa, inicioVigencia, fimVigencia, responsavel, codigoEmpresa, ativa, descricaoApolice, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, somenteNaoVencidos, exibirEmbarcador, usuarioMultiTransportador, codigosFilialMatriz);

                grid.setarQuantidadeTotal(repApolice.ContarConsulta(validar, numeroApolice, numeroAverbacao, codigoSeguradora, codigoGrupoPessoas, pessoa, inicioVigencia, fimVigencia, responsavel, codigoEmpresa, ativa, descricaoApolice, somenteNaoVencidos, exibirEmbarcador, usuarioMultiTransportador, codigosFilialMatriz));

                grid.AdicionaRows((from obj in listaApolices
                                   select new
                                   {
                                       obj.Codigo,
                                       Seguradora = obj.Seguradora.Nome,
                                       EnumResponsavel = obj.Responsavel,
                                       Responsavel = obj.DescricaoResponsavel,
                                       Empresa = obj.Empresa?.Descricao ?? "",
                                       obj.NumeroApolice,
                                       obj.NumeroAverbacao,
                                       obj.ValorLimiteApolice,
                                       InicioVigencia = obj.InicioVigencia.ToString("dd/MM/yyyy"),
                                       FimVigencia = obj.FimVigencia.ToString("dd/MM/yyyy"),
                                       Ativa = obj.DescricaoAtiva,
                                       DescricaoApolice = obj.DescricaoApolice ?? "",
                                       Descricao = obj.DescricaoComSeguradora
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string numeroAverbacao = Request.Params("NumeroAverbacao");
                string numeroApolice = Request.Params("NumeroApolice");
                string observacao = Request.Params("Observacao");
                string descricaoApolice = Request.GetStringParam("Descricao") ?? "";

                DateTime inicioVigencia, fimVigencia;
                DateTime.TryParseExact(Request.Params("InicioVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out inicioVigencia);
                DateTime.TryParseExact(Request.Params("FimVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fimVigencia);

                int.TryParse(Request.Params("Seguradora"), out int codigoSeguradora);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                decimal valorLimiteSeguro, valorFixoAverbacao = 0;
                decimal.TryParse(Request.Params("ValorLimiteApolice"), out valorLimiteSeguro);
                decimal.TryParse(Request.Params("ValorFixoAverbacao"), out valorFixoAverbacao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro responsavel;
                Enum.TryParse(Request.Params("Responsavel"), out responsavel);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao seguradoraAverbacao;
                Enum.TryParse(Request.Params("SeguradoraAverbacao"), out seguradoraAverbacao);

                bool ativa = Request.GetBoolParam("Ativa");

                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = Empresa;
                else if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro();

                apoliceSeguro.DescricaoApolice = descricaoApolice;
                apoliceSeguro.FimVigencia = fimVigencia;
                apoliceSeguro.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
                apoliceSeguro.InicioVigencia = inicioVigencia;
                apoliceSeguro.NumeroApolice = numeroApolice;
                apoliceSeguro.NumeroAverbacao = numeroAverbacao;
                apoliceSeguro.Pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                apoliceSeguro.Empresa = empresa;
                apoliceSeguro.Responsavel = responsavel;
                apoliceSeguro.Seguradora = repSeguradora.BuscarPorCodigo(codigoSeguradora);
                apoliceSeguro.ValorLimiteApolice = valorLimiteSeguro;
                apoliceSeguro.ValorFixoAverbacao = valorFixoAverbacao;
                apoliceSeguro.SeguradoraAverbacao = seguradoraAverbacao;
                apoliceSeguro.Observacao = observacao;
                apoliceSeguro.Ativa = ativa;
                repApoliceSeguro.Inserir(apoliceSeguro, Auditado);

                if (apoliceSeguro.Pessoa != null)
                {
                    apoliceSeguro.Pessoa.Initialize();
                    apoliceSeguro.Pessoa.ApolicesSeguro.Add(apoliceSeguro);

                    repPessoa.Atualizar(apoliceSeguro.Pessoa, Auditado);
                }
                else if (apoliceSeguro.GrupoPessoas != null)
                {
                    apoliceSeguro.GrupoPessoas.Initialize();
                    apoliceSeguro.GrupoPessoas.ApolicesSeguro.Add(apoliceSeguro);

                    repGrupoPessoas.Atualizar(apoliceSeguro.GrupoPessoas, Auditado);
                }

                SalvarConfiguracaoAverbacao(apoliceSeguro, unidadeDeTrabalho);

                AtualizarDescontos(apoliceSeguro, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(apoliceSeguro.Codigo);
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string numeroAverbacao = Request.Params("NumeroAverbacao");
                string numeroApolice = Request.Params("NumeroApolice");
                string observacao = Request.Params("Observacao");
                string descricaoApolice = Request.GetStringParam("Descricao") ?? "";


                DateTime inicioVigencia, fimVigencia;
                DateTime.TryParseExact(Request.Params("InicioVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out inicioVigencia);
                DateTime.TryParseExact(Request.Params("FimVigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fimVigencia);

                int.TryParse(Request.Params("Seguradora"), out int codigoSeguradora);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                decimal valorLimiteSeguro, valorFixoAverbacao = 0;
                decimal.TryParse(Request.Params("ValorLimiteApolice"), out valorLimiteSeguro);
                decimal.TryParse(Request.Params("ValorFixoAverbacao"), out valorFixoAverbacao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro responsavel;
                Enum.TryParse(Request.Params("Responsavel"), out responsavel);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao seguradoraAverbacao;
                Enum.TryParse(Request.Params("SeguradoraAverbacao"), out seguradoraAverbacao);

                bool ativa = Request.GetBoolParam("Ativa");

                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigo, true);

                if (apoliceSeguro == null)
                    return new JsonpResult(false, true, "Apólice de seguro não encontrada.");

                Dominio.Entidades.Empresa empresa = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = Empresa;
                else if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                unidadeDeTrabalho.Start();

                if (apoliceSeguro.Pessoa != null)
                {
                    apoliceSeguro.Pessoa.Initialize();
                    if (apoliceSeguro.Pessoa.ApolicesSeguro.Remove(apoliceSeguro))
                        repPessoa.Atualizar(apoliceSeguro.Pessoa, Auditado);
                }
                else if (apoliceSeguro.GrupoPessoas != null)
                {
                    apoliceSeguro.GrupoPessoas.Initialize();
                    if (apoliceSeguro.GrupoPessoas.ApolicesSeguro.Remove(apoliceSeguro))
                        repGrupoPessoas.Atualizar(apoliceSeguro.GrupoPessoas, Auditado);
                }

                apoliceSeguro.DescricaoApolice = descricaoApolice;
                apoliceSeguro.FimVigencia = fimVigencia;
                apoliceSeguro.GrupoPessoas = responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador && codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
                apoliceSeguro.InicioVigencia = inicioVigencia;
                apoliceSeguro.NumeroApolice = numeroApolice;
                apoliceSeguro.NumeroAverbacao = numeroAverbacao;
                apoliceSeguro.Pessoa = responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador && codigoGrupoPessoas <= 0 && cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                apoliceSeguro.Empresa = empresa;
                apoliceSeguro.Responsavel = responsavel;
                apoliceSeguro.Observacao = observacao;
                apoliceSeguro.Seguradora = repSeguradora.BuscarPorCodigo(codigoSeguradora);
                apoliceSeguro.ValorLimiteApolice = valorLimiteSeguro;
                apoliceSeguro.ValorFixoAverbacao = valorFixoAverbacao;
                apoliceSeguro.SeguradoraAverbacao = seguradoraAverbacao;
                apoliceSeguro.Ativa = ativa;
                repApoliceSeguro.Atualizar(apoliceSeguro, Auditado);

                if (apoliceSeguro.Pessoa != null && !apoliceSeguro.Pessoa.ApolicesSeguro.Where(o => o.Codigo == apoliceSeguro.Codigo).Any())
                {
                    apoliceSeguro.Pessoa.Initialize();
                    apoliceSeguro.Pessoa.ApolicesSeguro.Add(apoliceSeguro);

                    repPessoa.Atualizar(apoliceSeguro.Pessoa, Auditado);
                }
                else if (apoliceSeguro.GrupoPessoas != null && !apoliceSeguro.GrupoPessoas.ApolicesSeguro.Where(o => o.Codigo == apoliceSeguro.Codigo).Any())
                {
                    apoliceSeguro.GrupoPessoas.Initialize();
                    apoliceSeguro.GrupoPessoas.ApolicesSeguro.Add(apoliceSeguro);

                    repGrupoPessoas.Atualizar(apoliceSeguro.GrupoPessoas, Auditado);
                }

                SalvarConfiguracaoAverbacao(apoliceSeguro, unidadeDeTrabalho);

                AtualizarDescontos(apoliceSeguro, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigo);

                if (apoliceSeguro == null)
                    return new JsonpResult(false, true, "Apólice de seguro não encontrada.");

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> apoliceSeguroAnexos = repositorioApoliceSeguroAnexo.BuscarPorApolice(apoliceSeguro.Codigo);

                var retorno = new
                {
                    apoliceSeguro.Codigo,
                    FimVigencia = apoliceSeguro.FimVigencia.ToString("dd/MM/yyyy"),
                    GrupoPessoas = new
                    {
                        Codigo = apoliceSeguro.GrupoPessoas?.Codigo ?? 0,
                        Descricao = apoliceSeguro.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    Empresa = apoliceSeguro.Empresa != null ? new
                    {
                        Codigo = apoliceSeguro.Empresa.Codigo,
                        Descricao = apoliceSeguro.Empresa.RazaoSocial
                    } : null,
                    InicioVigencia = apoliceSeguro.InicioVigencia.ToString("dd/MM/yyyy"),
                    apoliceSeguro.NumeroApolice,
                    apoliceSeguro.NumeroAverbacao,
                    apoliceSeguro.ValorLimiteApolice,
                    apoliceSeguro.ValorFixoAverbacao,
                    apoliceSeguro.Observacao,
                    Descricao = apoliceSeguro.DescricaoApolice,
                    LimitirValorApolice = (apoliceSeguro.ValorLimiteApolice > 0),
                    ValorFixoAverbacaoEnable = (apoliceSeguro.ValorFixoAverbacao > 0),
                    Pessoa = new
                    {
                        Codigo = apoliceSeguro.Pessoa?.CPF_CNPJ_SemFormato ?? "0",
                        Descricao = apoliceSeguro.Pessoa?.Nome ?? string.Empty
                    },
                    apoliceSeguro.Responsavel,
                    apoliceSeguro.SeguradoraAverbacao,
                    Seguradora = new
                    {
                        apoliceSeguro.Seguradora.Codigo,
                        Descricao = apoliceSeguro.Seguradora.Nome
                    },
                    ConfiguracaoAverbacao = RetornaDynConfiguracaoAverbacao(apoliceSeguro, unidadeDeTrabalho),
                    Descontos = (
                        from desconto in apoliceSeguro.Descontos
                        select new
                        {
                            desconto.Codigo,
                            CodigoModeloVeicular = desconto.ModeloVeicularCarga?.Codigo,
                            DescricaoModeloVeicular = desconto.ModeloVeicularCarga?.Descricao,
                            ValorDesconto = desconto.ValorDesconto.ToString("n2"),
                            PercentualDesconto = desconto.PercentualDesconto.ToString("n2"),
                            CodigoFilial = desconto.Filial?.Codigo,
                            DescricaoFilial = desconto.Filial?.Descricao,
                            CodigoTipoOperacao = desconto.TipoOperacao?.Codigo,
                            DescricaoTipoOperacao = desconto.TipoOperacao?.Descricao
                        }

                    ).ToList(),
                    apoliceSeguro.Ativa,
                    Anexos = (from anexo in apoliceSeguroAnexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo
                              }
                    ).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Inicia transacao
                unidadeDeTrabalho.Start();

                // Instancia Repositorios
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.AverbacaoSenig repAverbacaoSenig = new Repositorio.Embarcador.Seguros.AverbacaoSenig(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unidadeDeTrabalho);
                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unidadeDeTrabalho);

                // Converte Parametros
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca dados
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigo);

                // Valida
                if (apoliceSeguro == null)
                    return new JsonpResult(false, true, "A apólice não foi encontrada");

                // Busca as configuracoes de averbacao
                Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apoliceSeguro.Codigo);
                Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacaoSenig = repAverbacaoSenig.BuscarPorApolice(apoliceSeguro.Codigo);
                Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacaoBradesco = repAverbacaoBradesco.BuscarPorApolice(apoliceSeguro.Codigo);
                Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacaoPortoSeguro = repAverbacaoPortoSeguro.BuscarPorApolice(apoliceSeguro.Codigo);
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> apoliceSeguroAnexos = repositorioApoliceSeguroAnexo.BuscarPorApolice(apoliceSeguro.Codigo);

                // Remove informacoes
                if (averbacaoATM != null) repAverbacaoATM.Deletar(averbacaoATM, Auditado);
                if (averbacaoSenig != null) repAverbacaoSenig.Deletar(averbacaoSenig, Auditado);
                if (averbacaoBradesco != null) repAverbacaoBradesco.Deletar(averbacaoBradesco, Auditado);
                if (averbacaoPortoSeguro != null) repAverbacaoPortoSeguro.Deletar(averbacaoPortoSeguro, Auditado);

                if (apoliceSeguroAnexos.Count > 0)
                    foreach (var anexo in apoliceSeguroAnexos)
                        repositorioApoliceSeguroAnexo.Deletar(anexo);

                repApoliceSeguro.Deletar(apoliceSeguro, Auditado);

                // Commita alterações
                unidadeDeTrabalho.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImportarDescontos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");
                dynamic parametro = JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                int codigoApolice = ((string)parametro.Codigo).ToInt();
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Servicos.Embarcador.Seguro.Seguro servicoSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoSeguro.Importar(linhas, unitOfWork, codigoApolice, Auditado);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicular", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Tipo Operação", Propriedade = "TipoOperacao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Filial", Propriedade = "Filial", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Valor Desconto", Propriedade = "ValorDesconto", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Percentual Desconto", Propriedade = "PercentualDesconto", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return new JsonpResult(configuracoes.ToList());

        }

        public async Task<IActionResult> ExportarDescontos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Modelo veicular", "ModeloVeicular", 25, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 25, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Filial", "Filial", 25, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Valor Desconto", "ValorDesconto", 25, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Percentual Desconto", "PercenteualDesconto", 25, Models.Grid.Align.left);

                Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto repositorioDescontos = new Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> listDesconto = repositorioDescontos.BuscaPorCodigoApoliceSeguro(codigo);


                int count = listDesconto.Count();
                if (count == 0)
                    return new JsonpResult(false, true, "Nenhum registro salvo para exportação");

                var retorno = (from obj in listDesconto
                               select new
                               {
                                   ModeloVeicular = obj.ModeloVeicularCarga.Descricao,
                                   Filial = obj.Filial.Descricao,
                                   ValorDesconto = obj.ValorDesconto,
                                   TipoOperacao = obj.TipoOperacao.Descricao,
                                   PercenteualDesconto = obj.PercentualDesconto
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "DescontosdoApoliceSeguro." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao baixao o arquivo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar dados salvos");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarPortalMultiTransportador()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                var retorno = new
                {
                    CodigoEmpresa = Usuario.Empresa?.Codigo ?? 0
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic RetornaDynConfiguracaoAverbacao(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                return RetornaDynConfiguracaoAverbacaoATM(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
                return RetornaDynConfiguracaoAverbacaoBradesco(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro)
                return RetornaDynConfiguracaoAverbacaoPortoSeguro(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Senig)
                return RetornaDynConfiguracaoAverbacaoSenig(apoliceSeguro, unitOfWork);

            return null;
        }

        private dynamic RetornaDynConfiguracaoAverbacaoATM(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacao = repAverbacaoATM.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) return null;

            // Retorna configuração
            return new
            {
                ATMCodigo = averbacao.CodigoATM,
                ATMUsuario = averbacao.Usuario,
                ATMSenha = averbacao.Senha,
                ATMAverbaComoEmbarcador = averbacao.AverbaComoEmbarcador,
                ATMAverbarNFeQuandoCargaPossuirNFSManual = averbacao.AverbarNFeQuandoCargaPossuirNFSManual,
                VersaoLayoutATMOutrosDocumentos = averbacao.VersaoLayoutATMOutrosDocumentos ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM.Versao200
            };
        }

        private dynamic RetornaDynConfiguracaoAverbacaoSenig(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoSenig repAverbacaoSenig = new Repositorio.Embarcador.Seguros.AverbacaoSenig(unitOfWork);

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacao = repAverbacaoSenig.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) return null;

            // Retorna configuração
            return new
            {
                AverbaComoEmbarcador = averbacao.AverbaComoEmbarcador
            };
        }

        private dynamic RetornaDynConfiguracaoAverbacaoBradesco(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unitOfWork);

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacao = repAverbacaoBradesco.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) return null;

            // Retorna configuração
            return new
            {
                BradescoToken = averbacao.Token,
                BradescoWSDLQuorum = averbacao.WSDLQuorum
            };
        }

        private dynamic RetornaDynConfiguracaoAverbacaoPortoSeguro(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacao = repAverbacaoPortoSeguro.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) return null;

            // Retorna configuração
            return new
            {
                PortoSeguroUsuario = averbacao.Usuario,
                PortoSeguroSenha = averbacao.Senha,
            };
        }

        private void SalvarConfiguracaoAverbacao(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                SalvarConfiguracaoAverbacaoATM(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
                SalvarConfiguracaoAverbacaoBradesco(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro)
                SalvarConfiguracaoAverbacaoPortoSeguro(apoliceSeguro, unitOfWork);
            else if (apoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Senig)
                SalvarConfiguracaoAverbacaoSenig(apoliceSeguro, unitOfWork);
        }

        private void SalvarConfiguracaoAverbacaoATM(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);

            // Converte valores
            string codigoATM = Request.Params("ATMCodigo") ?? string.Empty;
            string usuario = Request.Params("ATMUsuario") ?? string.Empty;
            string senha = Request.Params("ATMSenha") ?? string.Empty;
            bool.TryParse(Request.Params("ATMAverbaComoEmbarcador"), out bool averbaComoEmbarcador);
            bool averbarNFeQuandoCargaPossuirNFSManual = Request.GetBoolParam("ATMAverbarNFeQuandoCargaPossuirNFSManual");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM versaoLayoutATM;
            Enum.TryParse(Request.Params("VersaoLayoutATMOutrosDocumentos"), out versaoLayoutATM);

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacao = repAverbacaoATM.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoATM();
            else averbacao.Initialize();

            // Vincula os valores
            averbacao.ApoliceSeguro = apoliceSeguro;
            averbacao.CodigoATM = codigoATM;
            averbacao.Usuario = usuario;
            averbacao.Senha = senha;
            averbacao.AverbaComoEmbarcador = averbaComoEmbarcador;
            averbacao.AverbarNFeQuandoCargaPossuirNFSManual = averbarNFeQuandoCargaPossuirNFSManual;
            averbacao.VersaoLayoutATMOutrosDocumentos = versaoLayoutATM;

            // Atualiza banco
            if (averbacao.Codigo > 0)
                repAverbacaoATM.Atualizar(averbacao, Auditado);
            else
                repAverbacaoATM.Inserir(averbacao, Auditado);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, "Averbação ATM " + (averbacao.Codigo > 0 ? "atualizada" : "inserida") + " na Apólice.", unitOfWork);
        }

        private void SalvarConfiguracaoAverbacaoSenig(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.AverbacaoSenig repAverbacaoSenig = new Repositorio.Embarcador.Seguros.AverbacaoSenig(unitOfWork);

            bool senigAverbaComoEmbarcador = Request.GetBoolParam("AverbaComoEmbarcador");

            Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacao = repAverbacaoSenig.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig();
            else averbacao.Initialize();

            averbacao.ApoliceSeguro = apoliceSeguro;
            averbacao.AverbaComoEmbarcador = senigAverbaComoEmbarcador;

            if (averbacao.Codigo > 0)
                repAverbacaoSenig.Atualizar(averbacao, Auditado);
            else
                repAverbacaoSenig.Inserir(averbacao, Auditado);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, "Averbação Senig " + (averbacao.Codigo > 0 ? "atualizada" : "inserida") + " na Apólice.", unitOfWork);
        }

        private void SalvarConfiguracaoAverbacaoBradesco(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unitOfWork);

            // Converte valores
            string token = Request.Params("BradescoToken") ?? string.Empty;
            string WSDLQuorum = Request.Params("BradescoWSDLQuorum") ?? string.Empty;

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacao = repAverbacaoBradesco.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco();
            else averbacao.Initialize();

            // Vincula os valores
            averbacao.ApoliceSeguro = apoliceSeguro;
            averbacao.Token = token;
            averbacao.WSDLQuorum = WSDLQuorum;

            // Atualiza banco
            if (averbacao.Codigo > 0)
                repAverbacaoBradesco.Atualizar(averbacao, Auditado);
            else
                repAverbacaoBradesco.Inserir(averbacao, Auditado);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, "Averbação Bradesco " + (averbacao.Codigo > 0 ? "atualizada" : "inserida") + " na Apólice.", unitOfWork);
        }

        private void SalvarConfiguracaoAverbacaoPortoSeguro(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);

            // Converte valores
            string usuario = Request.Params("PortoSeguroUsuario") ?? string.Empty;
            string senha = Request.Params("PortoSeguroSenha") ?? string.Empty;

            // Busca se ja existe
            Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacao = repAverbacaoPortoSeguro.BuscarPorApolice(apoliceSeguro.Codigo);
            if (averbacao == null) averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro();
            else averbacao.Initialize();

            // Vincula os valores
            averbacao.ApoliceSeguro = apoliceSeguro;
            averbacao.Usuario = usuario;
            averbacao.Senha = senha;

            // Atualiza banco
            if (averbacao.Codigo > 0)
                repAverbacaoPortoSeguro.Atualizar(averbacao, Auditado);
            else
                repAverbacaoPortoSeguro.Inserir(averbacao, Auditado);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, "Averbação Porto Seguro " + (averbacao.Codigo > 0 ? "atualizada" : "inserida") + " na Apólice.", unitOfWork);
        }

        private void ExcluirDescontosRemovidas(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, dynamic descontos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (apoliceSeguro.Descontos != null)
            {
                Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto repositorioDescontos = new Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var desconto in descontos)
                {
                    int? codigo = ((string)desconto.Codigo).ToNullableInt();

                    if (codigo.HasValue && codigo > 0)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> listaDescontosRemover = (from desc in apoliceSeguro.Descontos where !listaCodigosAtualizados.Contains(desc.Codigo) select desc).ToList();

                foreach (var desconto in listaDescontosRemover)
                {
                    repositorioDescontos.Deletar(desconto);
                }

                if (listaDescontosRemover.Count > 0)
                {
                    string descricaoAcao = listaDescontosRemover.Count == 1 ? "Desconto removido" : "Múltiplas descontos removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosDesconto(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto apoliceSeguroDesconto, dynamic desconto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)desconto.CodigoModeloVeicular).ToInt()) ?? null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(((string)desconto.CodigoFilial).ToInt()) ?? null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(((string)desconto.CodigoTipoOperacao).ToInt()) ?? null;

            apoliceSeguroDesconto.ValorDesconto = ((string)desconto.ValorDesconto).ToDecimal();
            apoliceSeguroDesconto.PercentualDesconto = ((string)desconto.PercentualDesconto).ToDecimal();

            if (apoliceSeguroDesconto.ValorDesconto <= 0 && apoliceSeguroDesconto.PercentualDesconto <= 0)
                throw new ControllerException($"É obrigatório informar o valor do desconto do seguro.");

            if (apoliceSeguroDesconto.ValorDesconto > 0 && apoliceSeguroDesconto.PercentualDesconto > 0)
                throw new ControllerException($"É permitido informar somente o percentual ou o valor do desconto do seguro, nunca os dois.");

            apoliceSeguroDesconto.ModeloVeicularCarga = modeloVeicularCarga;
            apoliceSeguroDesconto.Filial = filial;
            apoliceSeguroDesconto.TipoOperacao = tipoOperacao;
        }

        private void SalvarDescontosAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, dynamic descontos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto repositorioDescontos = new Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto(unidadeDeTrabalho);
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var desconto in descontos)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto apoliceSeguroDesconto;
                int codigo = desconto.Codigo;

                if (codigo > 0)
                    apoliceSeguroDesconto = repositorioDescontos.BuscarPorCodigo(codigo, auditavel: true) ?? throw new Dominio.Excecoes.Embarcador.ControllerException("Desconto não encontrada");
                else
                    apoliceSeguroDesconto = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto { ApoliceSeguro = apoliceSeguro };

                PreencherDadosDesconto(apoliceSeguroDesconto, desconto, unidadeDeTrabalho);

                if (codigo > 0)
                {
                    totalRegistrosAtualizados += apoliceSeguroDesconto.GetChanges().Count > 0 ? 1 : 0;
                    repositorioDescontos.Atualizar(apoliceSeguroDesconto);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioDescontos.Inserir(apoliceSeguroDesconto);
                }
            }

            if (apoliceSeguro.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? "Desconto atualizado" : "Múltiplas descontos atualizadaoa";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? "Desconto adicionado" : "Múltiplas descontos adicionadas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void AtualizarDescontos(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic descontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Descontos"));

            ExcluirDescontosRemovidas(apoliceSeguro, descontos, unidadeDeTrabalho);
            SalvarDescontosAdicionadasOuAtualizadas(apoliceSeguro, descontos, unidadeDeTrabalho);
        }

        #endregion
    }
}
