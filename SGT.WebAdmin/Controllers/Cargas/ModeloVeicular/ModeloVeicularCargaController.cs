using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ModeloVeicular
{
    [CustomAuthorize(new string[] { "ObterConfiguracoes" }, "Cargas/ModeloVeicularCarga")]
    public class ModeloVeicularCargaController : BaseController
    {
        #region Construtores

        public ModeloVeicularCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {

            try
            {
                return new JsonpResult(ObterGrid());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private Models.Grid.Grid ObterGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                int.TryParse(Request.Params("GrupoPessoa"), out int codGrupoPessoa);
                int.TryParse(Request.Params("TipoVeiculo"), out int tipoVeiculo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                double.TryParse(Request.Params("Destinatario"), out double destinatario);

                decimal? capacidade = null;
                decimal capacidadeAux;
                if (decimal.TryParse(Request.Params("CapacidadePesoTransporte"), System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"), out capacidadeAux))
                    capacidade = capacidadeAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> tipos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga>();

                if (!string.IsNullOrWhiteSpace(Request.Params("Tipos")) && Request.Params("Tipos") != "null")
                    tipos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga>>(Request.Params("Tipos"));

                int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);

                if (tipos.Count <= 0 && tipoVeiculo < 3)
                {
                    if (tipoVeiculo == 0)
                    {
                        tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral);
                        tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao);
                    }
                    else if (tipoVeiculo == 1)
                    {
                        tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral);
                        tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque);
                    }

                }

                List<int> codigosFornecedor = new List<int>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                    codigosFornecedor = modalidadeFornecedor?.ModelosVeicular.Select(o => o.Codigo).ToList() ?? null;
                }

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = GridModeloVeicular();

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listaModeloVeicularCarga = null;

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 8, Models.Grid.Align.center, false);

                if (codigoCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga != null && carga.GrupoPessoaPrincipal != null && carga.GrupoPessoaPrincipal.ModelosReboque != null && carga.GrupoPessoaPrincipal.ModelosReboque.Count() > 0)
                    {
                        listaModeloVeicularCarga = carga.GrupoPessoaPrincipal.ModelosReboque.ToList();
                        grid.setarQuantidadeTotal(carga.GrupoPessoaPrincipal.ModelosReboque.Count());
                    }
                }

                if (codGrupoPessoa > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigo(codGrupoPessoa);
                    if (grupoPessoa.ModelosReboque != null && grupoPessoa.ModelosReboque.Count() > 0)
                    {
                        listaModeloVeicularCarga = grupoPessoa.ModelosReboque.ToList();
                        grid.setarQuantidadeTotal(grupoPessoa.ModelosReboque.Count());
                    }
                }
                List<int> modelosLiberados = new List<int>();
                if (destinatario > 0)
                {
                    Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento descarga = repCentroDescarregamento.BuscarPorDestinatario(destinatario);
                    if (descarga != null)
                        modelosLiberados = (from obj in descarga.VeiculosPermitidos select obj.Codigo).ToList();
                }

                if (listaModeloVeicularCarga == null)
                {
                    grid.setarQuantidadeTotal(repModeloVeicularCarga.ContarConsulta(codigosFornecedor, descricao, ativo, tipos, capacidade, modelosLiberados, codigoTipoCarga, codigoEmpresa, codigoIntegracao));
                    listaModeloVeicularCarga = grid.recordsTotal > 0 ? repModeloVeicularCarga.Consultar(codigosFornecedor, descricao, ativo, tipos, capacidade, modelosLiberados, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite > 0 ? grid.limite : grid.recordsTotal, codigoTipoCarga, codigoEmpresa, codigoIntegracao) : new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
                }

                var retorno = (from obj in listaModeloVeicularCarga
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.CapacidadePesoTransporte,
                                   obj.UnidadeCapacidade,
                                   obj.ModeloVeicularAceitaLocalizador,
                                   obj.ToleranciaPesoExtra,
                                   obj.ToleranciaPesoMenor,
                                   obj.VeiculoPaletizado,
                                   obj.ModeloControlaCubagem,
                                   obj.ExigirDefinicaoReboquePedido,
                                   obj.NumeroPaletes,
                                   obj.ToleranciaMinimaPaletes,
                                   obj.OcupacaoCubicaPaletes,
                                   obj.Cubagem,
                                   obj.ToleranciaMinimaCubagem,
                                   obj.NumeroEixos,
                                   obj.NumeroEixosSuspensos,
                                   obj.DescricaoAtivo,
                                   ModeloCalculoFranquiaCodigo = obj.ModeloCalculoFranquia?.Codigo ?? 0,
                                   ModeloCalculoFranquiaDescricao = obj.ModeloCalculoFranquia?.Descricao ?? "",
                                   obj.NumeroReboques,
                                   GrupoModeloVeicularCodigo = obj.GrupoModeloVeicular?.Codigo ?? 0,
                                   GrupoModeloVeicularDescricao = obj.GrupoModeloVeicular?.Descricao ?? "",
                                   obj.CodigoIntegracao
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        private Grid GridModeloVeicular()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 37, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloVeicularCarga.CapacidadeCarga, "CapacidadePesoTransporte", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloVeicularCarga.ToleranciaExtra, "ToleranciaPesoExtra", 10, Models.Grid.Align.right, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloVeicularCarga.ToleranciaMinima, "ToleranciaPesoMenor", 10, Models.Grid.Align.right, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloVeicularCarga.Eixos, "NumeroEixos", 5, Models.Grid.Align.right);
            grid.AdicionarCabecalho("VeiculoPaletizado", false);
            grid.AdicionarCabecalho("NumeroPaletes", false);
            grid.AdicionarCabecalho("ToleranciaMinimaPaletes", false);
            grid.AdicionarCabecalho("OcupacaoCubicaPaletes", false);
            grid.AdicionarCabecalho("ModeloControlaCubagem", false);
            grid.AdicionarCabecalho("ExigirDefinicaoReboquePedido", false);
            grid.AdicionarCabecalho("Cubagem", false);
            grid.AdicionarCabecalho("ToleranciaMinimaCubagem", false);
            grid.AdicionarCabecalho("ModeloCalculoFranquiaCodigo", false);
            grid.AdicionarCabecalho("ModeloCalculoFranquiaDescricao", false);
            grid.AdicionarCabecalho("NumeroReboques", false);
            grid.AdicionarCabecalho("GrupoModeloVeicularCodigo", false);
            grid.AdicionarCabecalho("GrupoModeloVeicularDescricao", false);
            grid.AdicionarCabecalho("UnidadeCapacidade", false);
            grid.AdicionarCabecalho("ModeloVeicularAceitaLocalizador", false);

            return grid;
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();

                PreencheEntidade(ref modelo, unitOfWork);

                if (!ValidaEntidade(modelo, unitOfWork, out string erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.Start();

                repModeloVeicularCarga.Inserir(modelo, Auditado);

                if (!SalvarDivisoesCapacidade(modelo, unitOfWork, out string msgErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, msgErro);
                }
                SalvarCodigosIntegracao(modelo, unitOfWork);
                AdicionarOuAtualizarProdutos(modelo, unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                {
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarModeloVeicular(modelo, unitOfWork);
                }

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
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoAdicionarDados);
            }
        }

        public async Task<IActionResult> AdicionarEixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga");
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga, auditavel: true);

                if (modeloVeicularCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontrarModeloVeicularDeCarga);

                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo RepositorioEixo = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo();

                unitOfWork.Start();

                PreencherEixo(eixo, modeloVeicularCarga);

                RepositorioEixo.Inserir(eixo, Auditado);

                AdicionarEixoPneus(eixo, unitOfWork);

                if (modeloVeicularCarga.NumeroEixos < eixo.Numero)
                {
                    modeloVeicularCarga.NumeroEixos = eixo.Numero;

                    repositorioModeloVeicularCarga.Atualizar(modeloVeicularCarga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, modeloVeicularCarga.GetChanges(), Localization.Resources.Cargas.ModeloVeicularCarga.AdicionadoEixoNumero + eixo.Numero, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    eixo.Codigo,
                    NomeImagem = ObterNomeImagemEixo(eixo)
                });
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
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoAdicionarEixo);
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
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo = repModeloVeicularCarga.BuscarPorCodigo(codigo, true);

                if (modelo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontrarRegistro);

                int quantidadeEstepesAnterior = modelo.QuantidadeEstepes;

                PreencheEntidade(ref modelo, unitOfWork);

                if (!ValidaEntidade(modelo, unitOfWork, out string erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.Start();

                repModeloVeicularCarga.Atualizar(modelo, Auditado);

                if (!SalvarDivisoesCapacidade(modelo, unitOfWork, out string msgErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, msgErro);
                }
                SalvarCodigosIntegracao(modelo, unitOfWork);
                AtualizarEstepes(modelo, quantidadeEstepesAnterior, unitOfWork);
                AdicionarOuAtualizarProdutos(modelo, unitOfWork);
                AdicionarOuAtualizarGruposProdutos(modelo, unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                {
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarModeloVeicular(modelo, unitOfWork);
                }

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

                if ((excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException))))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;
                    if (excecaoSql.Number == 547)
                    {
                        return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.RegistroPossuiDependenciasNaoPodeSerExcluido);
                    }
                }

                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReplicarEixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga");
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);

                if (modeloVeicularCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontrarModeloVeicularDeCarga);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo RepositorioEixo = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo = RepositorioEixo.BuscarPorCodigo(codigo, auditavel: true);

                if (eixo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontraEixo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo quantidadePneuAnterior = eixo.QuantidadePneu;

                unitOfWork.Start();

                PreencherEixo(eixo, modeloVeicularCarga);

                RepositorioEixo.Atualizar(eixo, Auditado);

                AtualizarEixoPneus(eixo, quantidadePneuAnterior, unitOfWork);

                //adiciona novos eixos não configurados
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo novoEixo = null;
                for (int i = 1; i < modeloVeicularCarga.NumeroEixos + 1; i++)
                {
                    if (!RepositorioEixo.ContemEixoConfigurado(codigoModeloVeicularCarga, i))
                    {
                        novoEixo = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo();

                        PreencherEixo(novoEixo, modeloVeicularCarga);
                        novoEixo.Numero = i;

                        RepositorioEixo.Inserir(novoEixo, Auditado);

                        AdicionarEixoPneus(novoEixo, unitOfWork);
                    }
                }
                //atualiza eixos já configurados
                //List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo> eixos = RepositorioEixo.BuscarPorModeloVeicular(codigoModeloVeicularCarga);
                //foreach (var item in eixos)
                //{
                //    if (item.Numero != eixo.Numero)
                //    {
                //        quantidadePneuAnterior = item.QuantidadePneu;
                //        int numeroAnterior = item.Numero;
                //        PreencherEixo(item, modeloVeicularCarga);
                //        item.Numero = numeroAnterior;

                //        RepositorioEixo.Atualizar(item);

                //        AtualizarEixoPneus(item, quantidadePneuAnterior, unitOfWork);
                //    }
                //}

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    NomeImagem = ObterNomeImagemEixo(eixo)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoAdicionarEixo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarEixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga");
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);

                if (modeloVeicularCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontrarModeloVeicularDeCarga);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo RepositorioEixo = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo = RepositorioEixo.BuscarPorCodigo(codigo, auditavel: true);

                if (eixo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontraEixo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo quantidadePneuAnterior = eixo.QuantidadePneu;

                unitOfWork.Start();

                PreencherEixo(eixo, modeloVeicularCarga);

                RepositorioEixo.Atualizar(eixo, Auditado);

                AtualizarEixoPneus(eixo, quantidadePneuAnterior, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    NomeImagem = ObterNomeImagemEixo(eixo)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoAdicionarEixo);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.GrupoModeloVeicular repGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    modeloVeicularCarga.Ativo,
                    CapacidadePesoTransporte = modeloVeicularCarga.CapacidadePesoTransporte > 0m ? modeloVeicularCarga.CapacidadePesoTransporte.ToString("n2") : "",
                    modeloVeicularCarga.Codigo,
                    CodigoModeloVeicularDeCargaEmbarcador = modeloVeicularCarga.CodigoIntegracao,
                    modeloVeicularCarga.Descricao,
                    modeloVeicularCarga.DescricaoAtivo,
                    modeloVeicularCarga.NumeroEixos,
                    modeloVeicularCarga.PadraoEixos,
                    modeloVeicularCarga.NumeroEixosSuspensos,
                    modeloVeicularCarga.NumeroPaletes,
                    modeloVeicularCarga.ModeloControlaCubagem,
                    modeloVeicularCarga.ExigirDefinicaoReboquePedido,
                    modeloVeicularCarga.Tipo,
                    modeloVeicularCarga.UnidadeCapacidade,
                    Cubagem = modeloVeicularCarga.Cubagem > 0 ? modeloVeicularCarga.Cubagem.ToString("n2") : "",
                    Altura = modeloVeicularCarga.Altura > 0 ? modeloVeicularCarga.Altura.ToString("n2") : "",
                    Largura = modeloVeicularCarga.Largura > 0 ? modeloVeicularCarga.Largura.ToString("n2") : "",
                    Comprimento = modeloVeicularCarga.Comprimento > 0 ? modeloVeicularCarga.Comprimento.ToString("n2") : "",
                    ToleranciaMinimaCubagem = modeloVeicularCarga.ToleranciaMinimaCubagem > 0 ? modeloVeicularCarga.ToleranciaMinimaCubagem.ToString("n2") : "",
                    ToleranciaMinimaPaletes = modeloVeicularCarga.ToleranciaMinimaPaletes > 0 ? modeloVeicularCarga.ToleranciaMinimaPaletes.ToString() : "",
                    OcupacaoCubicaPaletes = modeloVeicularCarga.OcupacaoCubicaPaletes > 0 ? modeloVeicularCarga.OcupacaoCubicaPaletes.ToString("n2") : "",
                    FatorEmissaoCO2 = modeloVeicularCarga.FatorEmissaoCO2.ToString("n3"),
                    modeloVeicularCarga.CodigoTipoCargaANTT,
                    modeloVeicularCarga.VelocidadeMedia,
                    modeloVeicularCarga.ToleranciaPesoExtra,
                    modeloVeicularCarga.ToleranciaPesoMenor,
                    ModeloCalculoFranquia = new { Codigo = modeloVeicularCarga.ModeloCalculoFranquia?.Codigo ?? 0, Descricao = modeloVeicularCarga.ModeloCalculoFranquia?.Descricao ?? "" },
                    ContainerTipo = new { Codigo = modeloVeicularCarga.ContainerTipo?.Codigo ?? 0, Descricao = modeloVeicularCarga.ContainerTipo?.Descricao ?? "" },
                    Grupo = new { Codigo = modeloVeicularCarga.GrupoModeloVeicular?.Codigo ?? 0, Descricao = modeloVeicularCarga.GrupoModeloVeicular?.Descricao ?? "" },
                    modeloVeicularCarga.VeiculoPaletizado,
                    modeloVeicularCarga.CodigoIntegracaoGerenciadoraRisco,
                    modeloVeicularCarga.CodigoIntegracaoGoldenService,
                    modeloVeicularCarga.NumeroReboques,
                    modeloVeicularCarga.DiasRealizarProximoChecklist,
                    modeloVeicularCarga.ValidarLicencaVeiculo,
                    modeloVeicularCarga.AlertarOperadorPesoExcederCapacidade,
                    modeloVeicularCarga.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga,
                    modeloVeicularCarga.ModeloVeicularAceitaLocalizador,
                    modeloVeicularCarga.NaoSolicitarNoChecklist,
                    modeloVeicularCarga.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador,
                    modeloVeicularCarga.ValidarCapacidadeMaximaNoApp,
                    CodigosIntegracao = (
                        from obj in modeloVeicularCarga.CodigosIntegracao
                        select new
                        {
                            Codigo = obj,
                            CodigoIntegracao = obj
                        }
                    ).ToList(),
                    Eixo = ObterEixo(modeloVeicularCarga),
                    CustoPneuKM = modeloVeicularCarga.CustoPneuKM?.ToString("n6") ?? string.Empty,
                    modeloVeicularCarga.TipoVeiculoRepom,
                    modeloVeicularCarga.TipoVeiculoPamcard,
                    DivisoesCapacidade = (
                        from obj in modeloVeicularCarga.DivisoesCapacidade
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            UnidadeMedida = new
                            {
                                obj.UnidadeMedida.Codigo,
                                obj.UnidadeMedida.Descricao,
                                obj.UnidadeMedida.Sigla
                            },
                            Quantidade = new { val = obj.Quantidade, tipo = "decimal", configDecimal = new { precision = 2 } },
                            Coluna = obj.Coluna.HasValue ? obj.Coluna.Value.ToString() : "",
                            Piso = obj.Piso.HasValue ? obj.Piso.Value.ToString() : ""
                        }
                    ).ToList(),
                    Produtos = (
                        from produto in modeloVeicularCarga.Produtos
                        select new
                        {
                            produto.Codigo,
                            produto.Descricao
                        }
                    ).ToList(),
                    GruposProdutos = (
                        from grupo in modeloVeicularCarga.GruposProdutos
                        select new
                        {
                            grupo.Codigo,
                            grupo.Descricao
                        }
                    ).ToList(),
                    ArrobaMinima = modeloVeicularCarga.ArrobaMinima > 0 ? modeloVeicularCarga.ArrobaMinima.ToString("n2") : "",
                    ArrobaMaxima = modeloVeicularCarga.ArrobaMaxima > 0 ? modeloVeicularCarga.ArrobaMaxima.ToString("n2") : "",
                    modeloVeicularCarga.CabecaMinima,
                    modeloVeicularCarga.CabecaMaxima,
                    modeloVeicularCarga.CategoriaVeiculoTarget,
                    modeloVeicularCarga.TipoVeiculoA52,
                    modeloVeicularCarga.TipoVeiculoGadle,
                    modeloVeicularCarga.QuantidadePaletes,
                    modeloVeicularCarga.TempoEmissaoFluxoPatio,
                    modeloVeicularCarga.TipoSemirreboque,
                    modeloVeicularCarga.LayoutSuperAppId,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoBuscarPorCOdigo);
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
                int codigo = Request.GetIntParam("codigo");
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                modeloVeicularCarga.DivisoesCapacidade = null;

                repositorioModeloVeicularCarga.Deletar(modeloVeicularCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirEixoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int numeroEixosAnterior = Request.GetIntParam("NumeroEixosAnterior");
                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo RepositorioEixo = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo = RepositorioEixo.BuscarPorCodigo(codigo, auditavel: false);

                if (eixo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelEncontrarEixo);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = eixo.ModeloVeicularCarga;
                int maiorNumeroEixoCadastrado = (from eixoCadastrado in modeloVeicularCarga.Eixos where eixoCadastrado.Codigo != eixo.Codigo select (int?)eixoCadastrado.Numero).Max() ?? 0;

                unitOfWork.Start();

                RepositorioEixo.Deletar(eixo);

                if ((eixo.Numero > maiorNumeroEixoCadastrado) && (modeloVeicularCarga.NumeroEixos != numeroEixosAnterior))
                {
                    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                    modeloVeicularCarga.Initialize();
                    modeloVeicularCarga.NumeroEixos = Math.Max(numeroEixosAnterior, maiorNumeroEixoCadastrado);

                    repositorioModeloVeicularCarga.Atualizar(modeloVeicularCarga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, modeloVeicularCarga.GetChanges(), Localization.Resources.Cargas.ModeloVeicularCarga.RemovidoEixoNumero + eixo.Numero, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                }
                else
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, null, Localization.Resources.Cargas.ModeloVeicularCarga.RemovidoEixoNumero + eixo.Numero, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ModeloVeicularCarga.NaoFoiPossivelExcluirEixoPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistema);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoExcluirEixo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.ContainerTipo repositorioContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                bool possuiTipoContainer = repositorioContainerTipo.ContarTodos() > 0;

                return new JsonpResult(new
                {
                    PossuiTipoContainer = possuiTipoContainer
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ModeloVeicularCarga.OcorreuUmaFalhaAoObterAsConfiguracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Exportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGrid();

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarProdutos(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

            ExcluirProdutosRemovidos(modeloVeicularCarga, produtos, unitOfWork);
            InserirProdutosAdicionados(modeloVeicularCarga, produtos, unitOfWork);
        }

        private void ExcluirProdutosRemovidos(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga, dynamic produtos, Repositorio.UnitOfWork unitOfWork)
        {
            if (ModeloVeicularCarga.Produtos?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var produto in produtos)
                    listaCodigosAtualizados.Add(((string)produto.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutoRemover = (from produto in ModeloVeicularCarga.Produtos where !listaCodigosAtualizados.Contains(produto.Codigo) select produto).ToList();

                foreach (var produto in listaProdutoRemover)
                    ModeloVeicularCarga.Produtos.Remove(produto);

                if (listaProdutoRemover.Count > 0)
                {
                    string descricaoAcao = listaProdutoRemover.Count == 1 ? Localization.Resources.Cargas.ModeloVeicularCarga.ProdutoRemovido : Localization.Resources.Cargas.ModeloVeicularCarga.MultiplosProdutosRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ModeloVeicularCarga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirProdutosAdicionados(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga, dynamic produtos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            int totalProdutosAdicionados = 0;

            if (ModeloVeicularCarga.Produtos == null)
                ModeloVeicularCarga.Produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            foreach (var produto in produtos)
            {
                int codigo = ((string)produto.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoAdicionar = repositorioProduto.BuscarPorCodigo(codigo) ?? throw new ControllerException(Localization.Resources.Cargas.ModeloVeicularCarga.ProdutoNaoEncontrado);

                if (!ModeloVeicularCarga.Produtos.Contains(produtoAdicionar))
                {
                    ModeloVeicularCarga.Produtos.Add(produtoAdicionar);

                    totalProdutosAdicionados++;
                }
            }

            if (ModeloVeicularCarga.IsInitialized() && (totalProdutosAdicionados > 0))
            {
                string descricaoAcao = totalProdutosAdicionados == 1 ? Localization.Resources.Cargas.ModeloVeicularCarga.ProdutoAdicionado : Localization.Resources.Cargas.ModeloVeicularCarga.MultiplosProdutosAdicionados;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ModeloVeicularCarga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void AdicionarOuAtualizarGruposProdutos(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic gruposProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GruposProdutos"));

            ExcluirGruposProdutosRemovidos(modeloVeicularCarga, gruposProdutos, unitOfWork);
            InserirGruposProdutosAdicionados(modeloVeicularCarga, gruposProdutos, unitOfWork);
        }

        private void ExcluirGruposProdutosRemovidos(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga, dynamic gruposProdutos, Repositorio.UnitOfWork unitOfWork)
        {
            if (ModeloVeicularCarga.GruposProdutos?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var grupo in gruposProdutos)
                    listaCodigosAtualizados.Add(((string)grupo.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> listaGrupoProdutoRemover = (from grupo in ModeloVeicularCarga.GruposProdutos where !listaCodigosAtualizados.Contains(grupo.Codigo) select grupo).ToList();

                foreach (var grupo in listaGrupoProdutoRemover)
                    ModeloVeicularCarga.GruposProdutos.Remove(grupo);

                if (listaGrupoProdutoRemover.Count > 0)
                {
                    string descricaoAcao = listaGrupoProdutoRemover.Count == 1 ? Localization.Resources.Cargas.ModeloVeicularCarga.ProdutoRemovido : Localization.Resources.Cargas.ModeloVeicularCarga.MultiplosGruposProdutosRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ModeloVeicularCarga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirGruposProdutosAdicionados(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga, dynamic gruposProdutos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            int totalGruposProdutosAdicionados = 0;

            if (ModeloVeicularCarga.GruposProdutos == null)
                ModeloVeicularCarga.GruposProdutos = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

            foreach (var grupo in gruposProdutos)
            {
                int codigo = ((string)grupo.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProdutoAdicionar = repositorioGrupoProduto.BuscarPorCodigo(codigo) ?? throw new ControllerException(Localization.Resources.Cargas.ModeloVeicularCarga.GrupoProdutoNaoEncontrado);

                if (!ModeloVeicularCarga.GruposProdutos.Contains(grupoProdutoAdicionar))
                {
                    ModeloVeicularCarga.GruposProdutos.Add(grupoProdutoAdicionar);

                    totalGruposProdutosAdicionados++;
                }
            }

            if (ModeloVeicularCarga.IsInitialized() && (totalGruposProdutosAdicionados > 0))
            {
                string descricaoAcao = totalGruposProdutosAdicionados == 1 ? Localization.Resources.Cargas.ModeloVeicularCarga.GrupoProdutoAdicionado : Localization.Resources.Cargas.ModeloVeicularCarga.MultiplosGruposProdutosAdicionados;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ModeloVeicularCarga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.GrupoModeloVeicular repositorioGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repositorioContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            int codigoEmpresa = 0;
            int codigoContainerTipo = Request.GetIntParam("ContainerTipo");
            int codigoGrupoModeloVeicular = Request.GetIntParam("Grupo");
            int codigoModeloCalculoFranquia = Request.GetIntParam("ModeloCalculoFranquia");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            modeloVeicularCarga.TipoVeiculoA52 = Request.GetStringParam("TipoVeiculoA52");
            modeloVeicularCarga.TipoVeiculoGadle = Request.GetStringParam("TipoVeiculoGadle");
            modeloVeicularCarga.CategoriaVeiculoTarget = Request.GetIntParam("CategoriaVeiculoTarget");
            modeloVeicularCarga.TipoVeiculoPamcard = Request.GetStringParam("TipoVeiculoPamcard");
            modeloVeicularCarga.TipoVeiculoRepom = Request.GetStringParam("TipoVeiculoRepom");
            modeloVeicularCarga.CustoPneuKM = Request.GetDecimalParam("CustoPneuKM");
            modeloVeicularCarga.Descricao = Request.GetStringParam("Descricao");
            modeloVeicularCarga.CodigoIntegracaoGerenciadoraRisco = Request.GetStringParam("CodigoIntegracaoGerenciadoraRisco");
            modeloVeicularCarga.CodigoIntegracao = Request.GetStringParam("CodigoModeloVeicularDeCargaEmbarcador");
            modeloVeicularCarga.CodigoIntegracaoGoldenService = Request.GetStringParam("CodigoIntegracaoGoldenService");
            modeloVeicularCarga.Ativo = Request.GetBoolParam("Ativo");
            modeloVeicularCarga.ModeloControlaCubagem = Request.GetBoolParam("ModeloControlaCubagem");
            modeloVeicularCarga.VeiculoPaletizado = Request.GetBoolParam("VeiculoPaletizado");
            modeloVeicularCarga.ExigirDefinicaoReboquePedido = Request.GetBoolParam("ExigirDefinicaoReboquePedido");
            modeloVeicularCarga.CapacidadePesoTransporte = Request.GetDecimalParam("CapacidadePesoTransporte");
            modeloVeicularCarga.ToleranciaPesoExtra = Request.GetDecimalParam("ToleranciaPesoExtra");
            modeloVeicularCarga.ToleranciaPesoMenor = Request.GetDecimalParam("ToleranciaPesoMenor");
            modeloVeicularCarga.Cubagem = Request.GetDecimalParam("Cubagem");
            modeloVeicularCarga.Altura = Request.GetDecimalParam("Altura");
            modeloVeicularCarga.Largura = Request.GetDecimalParam("Largura");
            modeloVeicularCarga.Comprimento = Request.GetDecimalParam("Comprimento");
            modeloVeicularCarga.ToleranciaMinimaCubagem = Request.GetDecimalParam("ToleranciaMinimaCubagem");
            modeloVeicularCarga.ToleranciaPesoMenor = Request.GetDecimalParam("ToleranciaPesoMenor");
            modeloVeicularCarga.FatorEmissaoCO2 = Request.GetDecimalParam("FatorEmissaoCO2");
            modeloVeicularCarga.CodigoTipoCargaANTT = Request.GetStringParam("CodigoTipoCargaANTT");
            modeloVeicularCarga.VelocidadeMedia = Request.GetIntParam("VelocidadeMedia");
            modeloVeicularCarga.NumeroEixos = Request.GetIntParam("NumeroEixos");
            modeloVeicularCarga.PadraoEixos = modeloVeicularCarga.NumeroEixos < 3 ? Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo>("PadraoEixos") : null;
            modeloVeicularCarga.NumeroEixosSuspensos = Request.GetIntParam("NumeroEixosSuspensos");
            modeloVeicularCarga.NumeroPaletes = Request.GetIntParam("NumeroPaletes");
            modeloVeicularCarga.NumeroReboques = Request.GetIntParam("NumeroReboques");
            modeloVeicularCarga.ToleranciaMinimaPaletes = Request.GetIntParam("ToleranciaMinimaPaletes");
            modeloVeicularCarga.OcupacaoCubicaPaletes = Request.GetDecimalParam("OcupacaoCubicaPaletes");
            modeloVeicularCarga.DiasRealizarProximoChecklist = Request.GetIntParam("DiasRealizarProximoChecklist");
            modeloVeicularCarga.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga>("Tipo");
            modeloVeicularCarga.UnidadeCapacidade = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade>("UnidadeCapacidade");
            modeloVeicularCarga.ArrobaMinima = Request.GetDecimalParam("ArrobaMinima");
            modeloVeicularCarga.ArrobaMaxima = Request.GetDecimalParam("ArrobaMaxima");
            modeloVeicularCarga.CabecaMinima = Request.GetIntParam("CabecaMinima");
            modeloVeicularCarga.CabecaMaxima = Request.GetIntParam("CabecaMaxima");
            modeloVeicularCarga.ValidarLicencaVeiculo = Request.GetBoolParam("ValidarLicencaVeiculo");
            modeloVeicularCarga.AlertarOperadorPesoExcederCapacidade = Request.GetBoolParam("AlertarOperadorPesoExcederCapacidade");
            modeloVeicularCarga.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga = Request.GetBoolParam("IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga");
            modeloVeicularCarga.ModeloVeicularAceitaLocalizador = Request.GetBoolParam("ModeloVeicularAceitaLocalizador");
            modeloVeicularCarga.NaoSolicitarNoChecklist = Request.GetBoolParam("NaoSolicitarNoChecklist");
            modeloVeicularCarga.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador = Request.GetBoolParam("ExigirInformacaoLacreJanelaCarregamentoPortalTransportador");
            modeloVeicularCarga.ValidarCapacidadeMaximaNoApp = Request.GetBoolParam("ValidarCapacidadeMaximaNoApp");
            modeloVeicularCarga.QuantidadePaletes = Request.GetIntParam("QuantidadePaletes");
            modeloVeicularCarga.IntegradoERP = false;
            modeloVeicularCarga.TempoEmissaoFluxoPatio = Request.GetIntParam("TempoEmissaoFluxoPatio");

            modeloVeicularCarga.ContainerTipo = (codigoContainerTipo > 0) ? repositorioContainerTipo.BuscarPorCodigo(codigoContainerTipo) : null;
            modeloVeicularCarga.Empresa = (codigoEmpresa > 0) ? repositorioEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            modeloVeicularCarga.GrupoModeloVeicular = (codigoGrupoModeloVeicular > 0) ? repositorioGrupoModeloVeicular.BuscarPorCodigo(codigoGrupoModeloVeicular) : null;
            modeloVeicularCarga.ModeloCalculoFranquia = (codigoModeloCalculoFranquia > 0) ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloCalculoFranquia) : null;
            modeloVeicularCarga.TipoSemirreboque = Request.GetBoolParam("TipoSemirreboque");

            if (modeloVeicularCarga.Codigo > 0)
            {
                modeloVeicularCarga.CalibragemAposKm = Request.GetIntParam("CalibragemAposKm");
                modeloVeicularCarga.QuantidadeEstepes = Request.GetIntParam("QuantidadeEstepes");
                modeloVeicularCarga.GerarAlertaManutencao = Request.GetBoolParam("GerarAlertaManutencao");
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork, out string msgErro)
        {
            msgErro = "";

            //if (!string.IsNullOrWhiteSpace(modeloVeicularCarga.CodigoIntegracao))
            //{
            //    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            //    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(modeloVeicularCarga.CodigoIntegracao);
            //    if (modeloVeicular != null && modeloVeicularCarga.Codigo != modeloVeicular.Codigo)
            //    {
            //        msgErro = Localization.Resources.Cargas.ModeloVeicularCarga.CodigoIntegracaoExistente;
            //        return false;
            //    }
            //}

            if (modeloVeicularCarga.ToleranciaPesoMenor > modeloVeicularCarga.CapacidadePesoTransporte)
            {
                msgErro = Localization.Resources.Cargas.ModeloVeicularCarga.ToleranciaMinimaNaoPodeSerSuperiorCapacidadeDeCarregamentoDoModeloVeicular;
                return false;
            }

            if (modeloVeicularCarga.ModeloControlaCubagem && modeloVeicularCarga.ToleranciaMinimaCubagem > modeloVeicularCarga.Cubagem)
            {
                msgErro = Localization.Resources.Cargas.ModeloVeicularCarga.CubagemMinimaNaoPodeSerSuperiorCubagemDoModeloVeicula;
                return false;
            }

            if (modeloVeicularCarga.VeiculoPaletizado && modeloVeicularCarga.ToleranciaMinimaPaletes > modeloVeicularCarga.NumeroPaletes.Value)
            {
                msgErro = Localization.Resources.Cargas.ModeloVeicularCarga.NumeroMinimoDePalletsNaoPodeSerSuperiorCapacidadeDePalletsDoVeiculo;
                return false;
            }

            return true;
        }

        private void AdicionarEixoPneus(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu RepositorioEixoPneu = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu(unitOfWork);

            RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.DireitoExterno });
            RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.EsquerdoExterno });

            if (eixo.QuantidadePneu == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo.Duplo)
            {
                RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.DireitoInterno });
                RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.EsquerdoInterno });
            }
        }

        private void AtualizarEixoPneus(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo quantidadePneuAnterior, Repositorio.UnitOfWork unitOfWork)
        {
            if (quantidadePneuAnterior != eixo.QuantidadePneu)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu RepositorioEixoPneu = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEixoPneu(unitOfWork);

                if (eixo.QuantidadePneu == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo.Duplo)
                {
                    RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.DireitoInterno });
                    RepositorioEixoPneu.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu() { Eixo = eixo, Posicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.EsquerdoInterno });
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu> posicoesRemover = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu>()
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.DireitoInterno,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.PosicaoEixoPneu.EsquerdoInterno
                    };
                    List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu> pneusRemover = (from pneu in eixo.Pneus where posicoesRemover.Contains(pneu.Posicao) select pneu).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu pneu in pneusRemover)
                    {
                        RepositorioEixoPneu.Deletar(pneu);
                    }
                }
            }
        }

        private void AtualizarEstepes(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, int quantidadeEstepesAnterior, Repositorio.UnitOfWork unitOfWork)
        {
            if (quantidadeEstepesAnterior != modeloVeicularCarga.QuantidadeEstepes)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCargaEstepe RepositorioEstepe = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaEstepe(unitOfWork);

                if (quantidadeEstepesAnterior < modeloVeicularCarga.QuantidadeEstepes)
                {
                    for (int i = (quantidadeEstepesAnterior + 1); i <= modeloVeicularCarga.QuantidadeEstepes; i++)
                    {
                        RepositorioEstepe.Inserir(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe() { ModeloVeicularCarga = modeloVeicularCarga, Numero = i });
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe> estepesRemover = (from estepe in modeloVeicularCarga.Estepes where estepe.Numero > modeloVeicularCarga.QuantidadeEstepes select estepe).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe estepe in estepesRemover)
                    {
                        RepositorioEstepe.Deletar(estepe);
                    }
                }
            }
        }

        private dynamic ObterEixo(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            List<dynamic> eixos = new List<dynamic>();
            int totalEixos = modeloVeicularCarga.NumeroEixos ?? 0;

            if (totalEixos > 0)
            {
                for (var i = 1; i <= totalEixos; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo = modeloVeicularCarga.Eixos.Where(o => o.Numero == i).FirstOrDefault();

                    if (eixo == null)
                        eixo = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo()
                        {
                            Numero = i,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEixo.Direcional,
                            QuantidadePneu = Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo.Simples
                        };

                    eixos.Add(new
                    {
                        eixo.Codigo,
                        CodigoModeloVeicularCarga = modeloVeicularCarga.Codigo,
                        eixo.Descricao,
                        eixo.Numero,
                        eixo.PrevisaoPneuNovoKm,
                        eixo.PrevisaoReformaKm,
                        eixo.PrevisaoRodizioKm,
                        eixo.QuantidadePneu,
                        eixo.Tipo,
                        eixo.ToleranciaPneuNovoKm,
                        eixo.ToleranciaReformaKm,
                        eixo.ToleranciaRodizioKm,
                        eixo.PrevisaoPneuNovoDia,
                        eixo.PrevisaoReformaDia,
                        eixo.PrevisaoRodizioDia,
                        eixo.ToleranciaPneuNovoDia,
                        eixo.ToleranciaReformaDia,
                        eixo.ToleranciaRodizioDia,
                        NomeImagem = ObterNomeImagemEixo(eixo)
                    });
                }
            }

            return new
            {
                modeloVeicularCarga.CalibragemAposKm,
                modeloVeicularCarga.GerarAlertaManutencao,
                modeloVeicularCarga.QuantidadeEstepes,
                Eixos = eixos
            };
        }

        private string ObterNomeImagemEixo(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo)
        {
            if (eixo.Codigo > 0)
                return eixo.QuantidadePneu == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo.Duplo ? "img/Eixos/EixoDuplo.png" : "img/Eixos/EixoSimplesMenor.png";

            return "img/Eixos/EixoSimplesSemPneuMenor.png";
        }

        private void PreencherEixo(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            eixo.ModeloVeicularCarga = modeloVeicularCarga;
            eixo.Numero = Request.GetIntParam("Numero");
            eixo.PrevisaoPneuNovoKm = Request.GetIntParam("PrevisaoPneuNovoKm");
            eixo.PrevisaoReformaKm = Request.GetIntParam("PrevisaoReformaKm");
            eixo.PrevisaoRodizioKm = Request.GetIntParam("PrevisaoRodizioKm");
            eixo.QuantidadePneu = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo>("QuantidadePneu");
            eixo.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEixo>("Tipo");
            eixo.ToleranciaPneuNovoKm = Request.GetIntParam("ToleranciaPneuNovoKm");
            eixo.ToleranciaReformaKm = Request.GetIntParam("ToleranciaReformaKm");
            eixo.ToleranciaRodizioKm = Request.GetIntParam("ToleranciaRodizioKm");
            eixo.PrevisaoPneuNovoDia = Request.GetIntParam("PrevisaoPneuNovoDia");
            eixo.PrevisaoReformaDia = Request.GetIntParam("PrevisaoReformaDia");
            eixo.PrevisaoRodizioDia = Request.GetIntParam("PrevisaoRodizioDia");
            eixo.ToleranciaPneuNovoDia = Request.GetIntParam("ToleranciaPneuNovoDia");
            eixo.ToleranciaReformaDia = Request.GetIntParam("ToleranciaReformaDia");
            eixo.ToleranciaRodizioDia = Request.GetIntParam("ToleranciaRodizioDia");
        }

        private void SalvarCodigosIntegracao(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            dynamic codigosIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CodigosIntegracao"));

            if (modelo.CodigosIntegracao != null)
                modelo.CodigosIntegracao.Clear();

            modelo.CodigosIntegracao = new List<string>();

            if (codigosIntegracao.Count > 0)
            {
                foreach (var codigoIntegracao in codigosIntegracao)
                {
                    string codigo = (string)codigoIntegracao.CodigoIntegracao;
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloExiste = repModeloVeicularCarga.BuscarPorCodigoEmbarcadorDiferenteDe(codigo, modelo.Codigo);

                    if (modeloExiste != null)
                        throw new ControllerException(Localization.Resources.Cargas.ModeloVeicularCarga.JaExisteUmModeloVeicularComCodigoDeIntegracao + codigo + Localization.Resources.Cargas.ModeloVeicularCarga.Cadastrado);

                    modelo.CodigosIntegracao.Add(codigo);
                }
            }
        }

        private bool SalvarDivisoesCapacidade(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork, out string msg)
        {
            msg = "";
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade repDivisaoCapacidade = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade(unitOfWork);
            Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisoes = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();
            dynamic divisoesCapacidade = JsonConvert.DeserializeObject<dynamic>(Request.Params("DivisoesCapacidade"));

            if (modeloVeicularCarga.DivisoesCapacidade != null && modeloVeicularCarga.DivisoesCapacidade.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var divisaoCapacidade in divisoesCapacidade)
                {
                    int codigo = 0;
                    if (divisaoCapacidade.Codigo != null && int.TryParse((string)divisaoCapacidade.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisoesCapacidadeDeletar = (from obj in modeloVeicularCarga.DivisoesCapacidade where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < divisoesCapacidadeDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisaoCapacidadeDeletar = divisoesCapacidadeDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, null, Localization.Resources.Cargas.ModeloVeicularCarga.RemoveuDivisaoDeCapacidade + divisaoCapacidadeDeletar.Descricao + ".", unitOfWork);

                    repDivisaoCapacidade.Deletar(divisaoCapacidadeDeletar);
                }
            }

            foreach (var divisaoCapacidade in divisoesCapacidade)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisao = null;

                int codigo = 0;

                if (divisaoCapacidade.Codigo != null && int.TryParse((string)divisaoCapacidade.Codigo, out codigo))
                    divisao = repDivisaoCapacidade.BuscarPorCodigo(codigo, true);

                if (divisao == null)
                    divisao = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade();

                divisao.ModeloVeicularCarga = modeloVeicularCarga;
                divisao.Descricao = (string)divisaoCapacidade.Descricao;
                divisao.Quantidade = Utilidades.Decimal.Converter((string)divisaoCapacidade.Quantidade);
                divisao.Piso = null;
                divisao.Coluna = null;
                if (int.TryParse((string)divisaoCapacidade.Piso, out int piso))
                    divisao.Piso = piso;
                if (int.TryParse((string)divisaoCapacidade.Coluna, out int coluna))
                    divisao.Coluna = coluna;
                divisao.UnidadeMedida = repUnidadeMedida.BuscarPorCodigo((int)divisaoCapacidade.UnidadeMedida.Codigo);

                if (divisao.Codigo > 0)
                {
                    repDivisaoCapacidade.Atualizar(divisao);
                    divisoes.Add(divisao);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = divisao.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, alteracoes, Localization.Resources.Cargas.ModeloVeicularCarga.AlterouDivisaoDeCapacidade + divisao.Descricao + ".", unitOfWork);
                }
                else
                {
                    repDivisaoCapacidade.Inserir(divisao);
                    divisoes.Add(divisao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, modeloVeicularCarga, null, Localization.Resources.Cargas.ModeloVeicularCarga.AdicionouDivisaoDeCapacidade + divisao.Descricao + ".", unitOfWork);
                }
            }

            bool andarObrigatorio = (from o in divisoes where o.Piso.HasValue select o).Any();
            bool possuiAndarNulo = (from o in divisoes where !o.Piso.HasValue select o).Any();
            if (andarObrigatorio && possuiAndarNulo)
            {
                msg = Localization.Resources.Cargas.ModeloVeicularCarga.NecessarioInformarAndarEmTodasAsDivisoesDeCapacidade;
                return false;
            }

            bool colunaObrigatorio = (from o in divisoes where o.Coluna.HasValue select o).Any();
            bool possuiColunaNulo = (from o in divisoes where !o.Coluna.HasValue select o).Any();
            if (colunaObrigatorio && possuiColunaNulo)
            {
                msg = Localization.Resources.Cargas.ModeloVeicularCarga.NecessarioInformarColunaEmTodasAsDivisoesDeCapacidade;
                return false;
            }

            return true;
        }

        #endregion
    }
}

