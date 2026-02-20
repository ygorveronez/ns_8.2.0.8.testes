using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Data.Common;

namespace SGT.WebAdmin.Controllers.Localidades
{
    [CustomAuthorize("Localidades/Localidade")]
    public class LocalidadeController : BaseController
    {
        #region Construtores

        public LocalidadeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {

                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPolos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string estado = Request.GetStringParam("Estado");
                int codigoIBGE = 0;

                if (!String.IsNullOrEmpty(Request.Params("CodigoIBGE")))
                {
                    codigoIBGE = int.Parse(Request.Params("CodigoIBGE"));
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Cidade, "Cidade", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.UF, "Estado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.CodigoIBGE, "CodigoIBGE", 15, Models.Grid.Align.left, false, false, true);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.ConsultaPolos(descricao, codigoIBGE, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, estado);
                int totalRegistros = repLocalidade.ContarConsultaPolos(descricao, codigoIBGE, estado);

                if (!string.IsNullOrEmpty(descricao) && totalRegistros <= 5)
                {
                    List<Dominio.Entidades.Localidade> listaExata = (from p in listaLocalidades where p.Descricao.ToUpper() == descricao.ToUpper() select p).ToList();
                    if (listaExata.Count == 1)
                    {
                        listaLocalidades = listaExata;
                        totalRegistros = 1;
                    }
                }

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaLocalidades
                                 select new
                                 {
                                     p.Codigo,
                                     Descricao = p.DescricaoCidadeEstado,
                                     Cidade = buscarDescricaoCidade(p),
                                     Estado = p.Estado != null ?
                                     p.Estado.Sigla : "",
                                     p.CodigoIBGE
                                 }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade();

                // Preenche entidade com dados
                PreencheEntidade(ref localidade, unitOfWork);

                if (repLocalidade.ContemLocalidadeDuplicada(0, localidade.Pais?.Codigo ?? 0, localidade.Descricao, localidade.Estado?.Sigla ?? ""))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Localidades.Localidade.ExisteLocalidadeComPaisEstadoDescricao);
                }
                // Persiste dados
                repLocalidade.Inserir(localidade, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                localidade.TipoEmissaoIntramunicipal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal)int.Parse(Request.Params("TipoEmissaoIntramunicipal"));

                string codigoAnterior = localidade.CodigoDocumento;
                localidade.CodigoCidade = Request.GetStringParam("CodigoCidade");
                localidade.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
                localidade.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                localidade.CodigoURF = Request.GetStringParam("CodigoURF");
                localidade.CodigoRA = Request.GetStringParam("CodigoRA");
                localidade.RKST = Request?.GetStringParam("RKST").Replace(" ", "");
                string novoCodigo = localidade.CodigoDocumento;
                localidade.CodigoZonaTarifaria = Request.GetStringParam("CodigoZonaTarifaria");
                dynamic outrasDescricoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("OutrasDescricoes"));
                localidade.OutrasDescricoes.Clear();
                localidade.OutrasDescricoes = new List<string>();
                foreach (var descricao in outrasDescricoes)
                {
                    localidade.OutrasDescricoes.Add((string)descricao.Descricao);
                }

                string lat = Request.GetStringParam("LatitudeEntrega");
                string lng = Request.GetStringParam("LongitudeEntrega");
                if (this.ConfiguracaoEmbarcador.PermiteCadastrarLatLngEntregaLocalidade && !string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                {
                    localidade.LatitudeEntrega = decimal.Parse(lat.Replace(".", ","));
                    localidade.LongitudeEntrega = decimal.Parse(lng.Replace(".", ","));
                }
                else
                {
                    localidade.LatitudeEntrega = null;
                    localidade.LongitudeEntrega = null;
                }

                string latitude = Request.GetStringParam("Latitude").Replace(".", ",");
                string longitude = Request.GetStringParam("Longitude").Replace(".", ",");
                if (!string.IsNullOrEmpty(longitude) && !string.IsNullOrEmpty(latitude))
                {
                    localidade.Latitude = decimal.Parse(latitude);
                    localidade.Longitude = decimal.Parse(longitude);
                }

                if (repLocalidade.ContemLocalidadeDuplicada(localidade.Codigo, localidade.Pais?.Codigo ?? 0, localidade.Descricao, localidade.Estado?.Sigla ?? ""))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Localidades.Localidade.ExisteLocalidadeComPaisEstadoDescricao);
                }

                repLocalidade.Atualizar(localidade, Auditado);
                unitOfWork.CommitChanges();

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && codigoAnterior != novoCodigo && !string.IsNullOrWhiteSpace(novoCodigo))
                {
                    Servicos.Log.TratarErro($"Atualização de localidade e CAR_CARGA_INTEGRADA_EMBARCADOR para false pelo LocalidadeController", "AtualizacaoCargaIntegradaEmbarcador");

                    bool atualizarTodos = string.IsNullOrWhiteSpace(codigoAnterior);
                    DbConnection connection = unitOfWork.GetConnection();
                    DbTransaction transaction = connection.BeginTransaction();
                    Servicos.Embarcador.Localidades.Localidade.VerificarCargasEmitidasAnteriormente(localidade.Codigo, atualizarTodos, connection, transaction);
                    transaction.Commit();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigo);

                var dynRegiao = new
                {
                    localidade.Codigo,
                    localidade.Descricao,
                    localidade.CodigoCidade,
                    localidade.CodigoDocumento,
                    UF = localidade.Estado.Sigla,
                    Estado = localidade.Estado.Nome,
                    localidade.TipoEmissaoIntramunicipal,
                    localidade.CodigoIBGE,
                    localidade.CodigoIntegracao,
                    localidade.LatitudeEntrega,
                    localidade.LongitudeEntrega,
                    localidade.CodigoURF,
                    localidade.CodigoZonaTarifaria,
                    localidade.CodigoRA,
                    localidade.Longitude,
                    localidade.Latitude,
                    localidade.RKST,
                    OutrasDescricoes = (from obj in localidade.OutrasDescricoes
                                        select new
                                        {
                                            Codigo = obj,
                                            Descricao = obj
                                        }).ToList()
                };
                return new JsonpResult(dynRegiao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaBrasil()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string estado = Request.GetStringParam("Estado");
                int codigoIBGE = 0;

                if (!String.IsNullOrEmpty(Request.Params("CodigoIBGE")))
                {
                    codigoIBGE = int.Parse(Request.Params("CodigoIBGE"));
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("DescricaoCidadePolo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Cidade, "Cidade", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.UF, "Estado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.CodigoIBGE, "CodigoIBGE", 15, Models.Grid.Align.left, false, false, true);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

                List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.ConsultaBrasil(descricao, codigoIBGE, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, estado);
                int totalRegistros = repLocalidade.ContarConsultaBrasil(descricao, codigoIBGE, estado);

                if (!string.IsNullOrEmpty(descricao) && totalRegistros <= 5)
                {
                    List<Dominio.Entidades.Localidade> listaExata = (from p in listaLocalidades where p.Descricao.ToUpper() == descricao.ToUpper() select p).ToList();
                    if (listaExata.Count == 1)
                    {
                        listaLocalidades = listaExata;
                        totalRegistros = 1;
                    }
                }

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaLocalidades
                                 select new
                                 {
                                     p.Codigo,
                                     Descricao = p.DescricaoCidadeEstado,
                                     Cidade = buscarDescricaoCidade(p),
                                     Estado = p.Estado.Sigla,
                                     p.CodigoIBGE,
                                     DescricaoCidadePolo = p.LocalidadePolo != null ? p.LocalidadePolo.DescricaoCidadeEstado : ""
                                 }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEnderecoPorCEP()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string logradouro = Request.Params("Logradouro");
                string bairro = Request.Params("Bairro");
                string cep = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
                string descricao = Request.Params("Descricao");
                int codLocalidade = 0;
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                int.TryParse(Request.Params("Localidade"), out codLocalidade);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.CEP, "CEP", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Logradouro, "Logradouro", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Complemento, "Complemento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Bairro, "Bairro", 20, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("CodigoIBGE", false);
                grid.AdicionarCabecalho("DescricaoCidade", false);
                grid.AdicionarCabecalho("TipoLogradouro", false);
                grid.AdicionarCabecalho("CodigoCidade", false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);

                string codigoIBGE = "";
                List<AdminMultisoftware.Dominio.Entidades.Localidades.Endereco> listaEndereco = null;
                int totalRegistros = 0;
                if ((codLocalidade > 0 && !string.IsNullOrWhiteSpace(logradouro)) || !string.IsNullOrEmpty(cep))
                {
                    string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                    if (ordenacao == "Descricao")
                        ordenacao = "Localidade";

                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(unitOfWorkAdmin);

                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codLocalidade);
                    codigoIBGE = localidade.CodigoIBGE.ToString();

                    listaEndereco = repEndereco.BuscarEnderecos(logradouro, bairro, cep, descricao, codigoIBGE, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                    totalRegistros = repEndereco.ContarBuscarEnderecos(logradouro, bairro, cep, descricao, codigoIBGE);
                }
                else
                {
                    listaEndereco = new List<AdminMultisoftware.Dominio.Entidades.Localidades.Endereco>();
                }


                Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);

                dynamic lista = (from p in listaEndereco
                                     //where p.Localidade != null && p.Localidade.CodigoIBGE != "0"
                                 select new
                                 {
                                     Descricao = p.Localidade != null ? p.Localidade.DescricaoCidadeEstado : string.Empty,
                                     p.Logradouro,
                                     Bairro = p.Bairro != null ? p.Bairro.Descricao : string.Empty,
                                     CEP = p.CEP_Formatado,
                                     p.Complemento,
                                     CodigoIBGE = p.Localidade != null ? p.Localidade.CodigoIBGE : string.Empty,
                                     p.TipoLogradouro,
                                     CodigoCidade = p.Localidade != null && p.Localidade.CodigoIBGE != null && !string.IsNullOrWhiteSpace(p.Localidade.CodigoIBGE) && repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)) != null ? repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)).Codigo : 0,
                                     Latitude = "",//p.Localidade != null && p.Localidade.Estado != null && p.Bairro != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao) != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao).latitude : string.Empty : string.Empty,
                                     Longitude = "",//p.Localidade != null && p.Localidade.Estado != null && p.Bairro != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao) != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao).longitude : string.Empty : string.Empty,
                                     DescricaoCidade = true ? p.Localidade.Descricao : repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)).Descricao
                                 }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarEnderecoPorCEP()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string cep = Utilidades.String.OnlyNumbers(Request.Params("CEP"));

                Servicos.Embarcador.Localidades.Localidade svcLocalidade = new Servicos.Embarcador.Localidades.Localidade(unitOfWork);
                dynamic endereco = svcLocalidade.BuscarEnderecoPorCEP(cep, unitOfWork, unitOfWorkAdmin);

                return new JsonpResult(endereco);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarEnderecosCorreio()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string logradouro = Request.Params("Logradouro");
                string bairro = Request.Params("Bairro");
                string cep = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
                string descricao = Request.Params("Descricao");
                string codigoIBGE = Request.Params("CodigoIBGE");
                if (!string.IsNullOrEmpty(descricao) && descricao.IndexOf('-') > 0)
                    descricao = descricao.Remove(descricao.IndexOf('-') - 1, 5);
                int codigoCidade = 0;
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                if (!string.IsNullOrEmpty(Request.Params("CodigoCidade")) && int.Parse(Request.Params("CodigoCidade")) > 0 && !string.IsNullOrEmpty(Request.Params("NomeCidade")))
                {
                    codigoCidade = int.Parse(Request.Params("CodigoCidade"));
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoCidade);
                    if (localidade != null)
                    {
                        codigoIBGE = Convert.ToString(localidade.CodigoIBGE);
                        descricao = "";
                    }
                }

                AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(unitOfWorkAdmin);
                List<AdminMultisoftware.Dominio.Entidades.Localidades.Endereco> listaEndereco = repEndereco.BuscarEnderecos(logradouro, bairro, cep, descricao, codigoIBGE);
                Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);

                dynamic retorno = new
                {
                    Enderecos = from p in listaEndereco
                                select new
                                {
                                    Descricao = p.Localidade != null ? p.Localidade.DescricaoCidadeEstado : string.Empty,
                                    p.Logradouro,
                                    Bairro = p.Bairro != null ? p.Bairro.Descricao : string.Empty,
                                    CEP = p.CEP_Formatado,
                                    p.Complemento,
                                    CodigoIBGE = p.Localidade != null ? p.Localidade.CodigoIBGE : string.Empty,
                                    p.TipoLogradouro,
                                    CodigoCidade = p.Localidade != null && p.Localidade.CodigoIBGE != null && !string.IsNullOrWhiteSpace(p.Localidade.CodigoIBGE) && repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)) != null ? repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)).Codigo : 0,
                                    Latitude = "",//p.Localidade != null && p.Localidade.Estado != null && p.Bairro != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao) != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao).latitude : string.Empty : string.Empty,
                                    Longitude = "",//p.Localidade != null && p.Localidade.Estado != null && p.Bairro != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao) != null ? serMapRequestAPI.BuscarCoordenadasEndereco(p.Localidade.CodigoIBGE, p.Localidade.Estado.UF, p.Logradouro, p.CEP, p.Bairro.Descricao).longitude : string.Empty : string.Empty,
                                    DescricaoCidade = true ? p.Localidade.Descricao : repLocalidade.BuscarPorCodigoIBGE(int.Parse(p.Localidade.CodigoIBGE)).Descricao
                                }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
        }

        #endregion

        #region Métodos Privados
        private Grid ObterGridPesquisa()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            string descricao = Request.Params("Descricao");
            int codigoIBGE = Request.GetIntParam("CodigoIBGE");
            string estado = Request.GetStringParam("Estado");
            List<string> estados = Request.GetListParam<string>("Estados");
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("DescricaoCidadePolo", false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.Cidade, "Cidade", 60, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.UF, "Estado", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.Localidade.CodigoIBGE, "CodigoIBGE", 15, Models.Grid.Align.left, false, false, true);
            grid.AdicionarCabecalho("Latitude", false).NumberFormat("n10");
            grid.AdicionarCabecalho("Longitude", false).NumberFormat("n10");

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int totalRegistros;
            List<Dominio.Entidades.Localidade> listaLocalidades;

            totalRegistros = repLocalidade.ContarConsulta(descricao, codigoIBGE, estado, false, 0, estados);

            if (totalRegistros == 0)
            {
                grid.setarQuantidadeTotal(0);
                grid.AdicionaRows(new List<object>());
                return grid;
            }

            listaLocalidades = repLocalidade.Consulta(descricao, codigoIBGE, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, estado, false, 0, estados);

            if (!string.IsNullOrEmpty(descricao) && totalRegistros <= 5)
            {
                List<Dominio.Entidades.Localidade> listaExata = (from p in listaLocalidades where p.Descricao.ToUpper() == descricao.ToUpper() select p).ToList();
                if (listaExata.Count == 1)
                {
                    listaLocalidades = listaExata;
                    totalRegistros = 1;
                }
            }

            grid.setarQuantidadeTotal(totalRegistros);

            dynamic lista = (from p in listaLocalidades
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.DescricaoCidadeEstado,
                                 Cidade = buscarDescricaoCidade(p),
                                 Estado = !string.IsNullOrWhiteSpace(p.Estado.Abreviacao) ? p.Estado.Abreviacao : p.Estado.Sigla,
                                 p.CodigoIBGE,
                                 DescricaoCidadePolo = p.LocalidadePolo != null ? p.LocalidadePolo.DescricaoCidadeEstado : "",
                                 Latitude = p.Latitude,
                                 Longitude = p.Longitude
                             }).ToList();
            grid.AdicionaRows(lista);
            return grid;
        }

        private string buscarDescricaoCidade(Dominio.Entidades.Localidade cidade)
        {
            if (cidade.CodigoIBGE != 9999999 || cidade.Pais == null)
                return cidade.Descricao;
            else
                return cidade.Descricao + " (" + cidade.Pais.Nome + ")";

        }

        private void PreencheEntidade(ref Dominio.Entidades.Localidade localidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            int localidadePolo = Request.GetIntParam("LocalidadePolo");
            int pais = Request.GetIntParam("Pais");
            string estado = Request.GetStringParam("Estado");
            int ultimoCodigo = repLocalidade.BuscarPorMaiorCodigo() + 1;

            // Vincula dados
            localidade.Codigo = ultimoCodigo;
            localidade.Descricao = Request.GetStringParam("Descricao");
            localidade.CodigoCidade = Request.GetStringParam("CodigoCidade");
            localidade.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
            localidade.CodigoIBGE = Request.GetIntParam("CodigoIBGE");
            localidade.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            localidade.LatitudeEntrega = Request.GetDecimalParam("LatitudeEntrega");
            localidade.LongitudeEntrega = Request.GetDecimalParam("LongitudeEntrega");
            localidade.TipoEmissaoIntramunicipal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal)int.Parse(Request.Params("TipoEmissaoIntramunicipal"));
            localidade.LocalidadePolo = repLocalidade.BuscarPorCodigo(localidadePolo);
            localidade.Pais = repPais.BuscarPorCodigo(pais);
            localidade.Estado = repEstado.BuscarPorSigla(estado);
            localidade.CodigoURF = Request.GetStringParam("CodigoURF");
            localidade.CodigoRA = Request.GetStringParam("CodigoRA");
            localidade.CodigoZonaTarifaria = Request.GetStringParam("CodigoZonaTarifaria");
            localidade.RKST = Request?.GetStringParam("RKST").Replace(" ", "");

            if (localidade.Estado?.Sigla == "EX")
                localidade.CEP = "99999999";
            else
                localidade.CEP = Request.GetStringParam("CEP");
        }

        #endregion
    }
}
