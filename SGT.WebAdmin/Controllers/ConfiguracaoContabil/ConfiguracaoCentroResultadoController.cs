using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.ConfiguracaoContabil
{
    [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoCentroResultado")]
    public class ConfiguracaoCentroResultadoController : BaseController
    {
        #region Construtores

        public ConfiguracaoCentroResultadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Ocorrencia", "TipoOcorrencia", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro Contabilização", "CentroResultadoContabilizacao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro Escrituração", "CentroResultadoEscrituracao", 12, Models.Grid.Align.left, true);
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> listaConfiguracaoCentroResultado = repConfiguracaoCentroResultado.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConfiguracaoCentroResultado.ContarConsulta(filtrosPesquisa));

                var retorno = (from obj in listaConfiguracaoCentroResultado
                               select new
                               {
                                   obj.Codigo,
                                   Empresa = obj.Empresa?.Descricao ?? "",
                                   TipoOperacao = obj.TipoOperacao?.Descricao ?? "",
                                   TipoOcorrencia = obj.TipoOcorrencia?.Descricao ?? "",
                                   CentroResultadoContabilizacao = obj.CentroResultadoContabilizacao.BuscarDescricao,
                                   CentroResultadoEscrituracao = obj.CentroResultadoEscrituracao.BuscarDescricao,
                                   Tomador = obj.CategoriaTomador != null ? obj.CategoriaTomador.Descricao : (obj.GrupoTomador != null ? obj.GrupoTomador.Descricao : (obj.Tomador != null ? obj.Tomador.Descricao : "")),
                                   Remetente = obj.CategoriaRemetente != null ? obj.CategoriaRemetente.Descricao : (obj.GrupoRemetente != null ? obj.GrupoRemetente.Descricao : (obj.Remetente != null ? obj.Remetente.Descricao : "")),
                                   Destinatario = obj.CategoriaDestinatario != null ? obj.CategoriaDestinatario.Descricao : (obj.GrupoDestinatario != null ? obj.GrupoDestinatario.Descricao : (obj.Destinatario != null ? obj.Destinatario.Descricao : "")),
                                   obj.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
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
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado();
                string retorno = preecherConfiguracaoCentroResultado(ref configuracaoCentroResultado, unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    repConfiguracaoCentroResultado.Inserir(configuracaoCentroResultado, Auditado);
                    Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado regraConfiguracaoCentroResultado = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado.GetInstance(unitOfWork);
                    regraConfiguracaoCentroResultado.AtualizarConfiguracaoCentroResultado(unitOfWork);

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = repConfiguracaoCentroResultado.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                string retorno = preecherConfiguracaoCentroResultado(ref configuracaoCentroResultado, unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    repConfiguracaoCentroResultado.Atualizar(configuracaoCentroResultado, Auditado);
                    Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado regraConfiguracaoCentroResultado = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado.GetInstance(unitOfWork);
                    regraConfiguracaoCentroResultado.AtualizarConfiguracaoCentroResultado(unitOfWork);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = repConfiguracaoCentroResultado.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    configuracaoCentroResultado.Ativo,
                    configuracaoCentroResultado.Codigo,
                    Destinatario = new { Codigo = configuracaoCentroResultado.Destinatario != null ? configuracaoCentroResultado.Destinatario.CPF_CNPJ : 0, Descricao = configuracaoCentroResultado.Destinatario != null ? configuracaoCentroResultado.Destinatario.Descricao : "" },
                    Remetente = new { Codigo = configuracaoCentroResultado.Remetente != null ? configuracaoCentroResultado.Remetente.CPF_CNPJ : 0, Descricao = configuracaoCentroResultado.Remetente != null ? configuracaoCentroResultado.Remetente.Descricao : "" },
                    Tomador = new { Codigo = configuracaoCentroResultado.Tomador != null ? configuracaoCentroResultado.Tomador.CPF_CNPJ : 0, Descricao = configuracaoCentroResultado.Tomador != null ? configuracaoCentroResultado.Tomador.Descricao : "" },
                    Expedidor = new { Codigo = configuracaoCentroResultado.Expedidor != null ? configuracaoCentroResultado.Expedidor.CPF_CNPJ : 0, Descricao = configuracaoCentroResultado.Expedidor != null ? configuracaoCentroResultado.Expedidor.Descricao : "" },
                    Recebedor = new { Codigo = configuracaoCentroResultado.Recebedor != null ? configuracaoCentroResultado.Recebedor.CPF_CNPJ : 0, Descricao = configuracaoCentroResultado.Recebedor != null ? configuracaoCentroResultado.Recebedor.Descricao : "" },
                    CategoriaDestinatario = new { Codigo = configuracaoCentroResultado.CategoriaDestinatario?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CategoriaDestinatario?.Descricao ?? "" },
                    CategoriaRemetente = new { Codigo = configuracaoCentroResultado.CategoriaRemetente?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CategoriaRemetente?.Descricao ?? "" },
                    CategoriaTomador = new { Codigo = configuracaoCentroResultado.CategoriaTomador?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CategoriaTomador?.Descricao ?? "" },
                    GrupoDestinatario = new { Codigo = configuracaoCentroResultado.GrupoDestinatario != null ? configuracaoCentroResultado.GrupoDestinatario.Codigo : 0, Descricao = configuracaoCentroResultado.GrupoDestinatario != null ? configuracaoCentroResultado.GrupoDestinatario.Descricao : "" },
                    GrupoRemetente = new { Codigo = configuracaoCentroResultado.GrupoRemetente != null ? configuracaoCentroResultado.GrupoRemetente.Codigo : 0, Descricao = configuracaoCentroResultado.GrupoRemetente != null ? configuracaoCentroResultado.GrupoRemetente.Descricao : "" },
                    GrupoTomador = new { Codigo = configuracaoCentroResultado.GrupoTomador != null ? configuracaoCentroResultado.GrupoTomador.Codigo : 0, Descricao = configuracaoCentroResultado.GrupoTomador != null ? configuracaoCentroResultado.GrupoTomador.Descricao : "" },
                    TipoOperacao = new { Codigo = configuracaoCentroResultado.TipoOperacao != null ? configuracaoCentroResultado.TipoOperacao.Codigo : 0, Descricao = configuracaoCentroResultado.TipoOperacao != null ? configuracaoCentroResultado.TipoOperacao.Descricao : "" },
                    Filial = new { Codigo = configuracaoCentroResultado.Filial != null ? configuracaoCentroResultado.Filial.Codigo : 0, Descricao = configuracaoCentroResultado.Filial != null ? configuracaoCentroResultado.Filial.Descricao : "" },
                    Origem = new { Codigo = configuracaoCentroResultado.Origem != null ? configuracaoCentroResultado.Origem.Codigo : 0, Descricao = configuracaoCentroResultado.Origem != null ? configuracaoCentroResultado.Origem.DescricaoCidadeEstado : "" },
                    TipoOcorrencia = new { Codigo = configuracaoCentroResultado.TipoOcorrencia != null ? configuracaoCentroResultado.TipoOcorrencia.Codigo : 0, Descricao = configuracaoCentroResultado.TipoOcorrencia != null ? configuracaoCentroResultado.TipoOcorrencia.Descricao : "" },
                    Empresa = new { Codigo = configuracaoCentroResultado.Empresa != null ? configuracaoCentroResultado.Empresa.Codigo : 0, Descricao = configuracaoCentroResultado.Empresa != null ? configuracaoCentroResultado.Empresa.Descricao : "" },
                    GrupoProduto = new { Codigo = configuracaoCentroResultado.GrupoProduto != null ? configuracaoCentroResultado.GrupoProduto.Codigo : 0, Descricao = configuracaoCentroResultado.GrupoProduto != null ? configuracaoCentroResultado.GrupoProduto.Descricao : "" },
                    RotaFrete = new { Codigo = configuracaoCentroResultado.RotaFrete != null ? configuracaoCentroResultado.RotaFrete.Codigo : 0, Descricao = configuracaoCentroResultado.RotaFrete != null ? configuracaoCentroResultado.RotaFrete.Descricao : "" },
                    CentroResultadoContabilizacao = new { configuracaoCentroResultado.CentroResultadoContabilizacao.Codigo, Descricao = configuracaoCentroResultado.CentroResultadoContabilizacao.BuscarDescricao },
                    CentroResultadoEscrituracao = new { configuracaoCentroResultado.CentroResultadoEscrituracao.Codigo, Descricao = configuracaoCentroResultado.CentroResultadoEscrituracao.BuscarDescricao },
                    CentroResultadoICMS = new { Codigo = configuracaoCentroResultado.CentroResultadoICMS?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CentroResultadoICMS?.BuscarDescricao ?? "" },
                    CentroResultadoPIS = new { Codigo = configuracaoCentroResultado.CentroResultadoPIS?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CentroResultadoPIS?.BuscarDescricao ?? "" },
                    CentroResultadoCOFINS = new { Codigo = configuracaoCentroResultado.CentroResultadoCOFINS?.Codigo ?? 0, Descricao = configuracaoCentroResultado.CentroResultadoCOFINS?.BuscarDescricao ?? "" },
                    configuracaoCentroResultado.ItemServico,
                    configuracaoCentroResultado.ValorMaximoCentroContabilizacao
                };

                return new JsonpResult(entidade);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = repConfiguracaoCentroResultado.BuscarPorCodigo(codigo);
                repConfiguracaoCentroResultado.Deletar(configuracaoCentroResultado, Auditado);
                Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado regraConfiguracaoCentroResultado = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado.GetInstance(unitOfWork);
                regraConfiguracaoCentroResultado.AtualizarConfiguracaoCentroResultado(unitOfWork);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoConfiguracaoCentroCustoEmbarcador();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoConfiguracaoCentroCustoEmbarcador();
                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRemetente = linha.Colunas?.Where(o => o.NomeCampo == "Remetente").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinatario = linha.Colunas?.Where(o => o.NomeCampo == "Destinatario").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTomador = linha.Colunas?.Where(o => o.NomeCampo == "Tomador").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = linha.Colunas?.Where(o => o.NomeCampo == "TipoOperacao").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCRContabilizacao = linha.Colunas?.Where(o => o.NomeCampo == "CRContabilizacao").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCREscrituracao = linha.Colunas?.Where(o => o.NomeCampo == "CREscrituracao").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCRICMS = linha.Colunas?.Where(o => o.NomeCampo == "CRICMS").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCRPIS = linha.Colunas?.Where(o => o.NomeCampo == "CRPIS").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCRCOFINS = linha.Colunas?.Where(o => o.NomeCampo == "CRCOFINS").FirstOrDefault();

                        Dominio.Entidades.Cliente tomador = null;
                        if (colTomador != null)
                        {
                            double.TryParse(Utilidades.String.OnlyNumbers(colTomador.Valor), out double cpfcnpj);
                            if (cpfcnpj > 0)
                            {
                                tomador = repCliente.BuscarPorCPFCNPJ(cpfcnpj);
                                if (tomador == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tomador informado não existe na base multisoftware.", i));
                                    continue;
                                }
                            }
                        }

                        Dominio.Entidades.Cliente remetente = null;
                        if (colRemetente != null)
                        {
                            double.TryParse(Utilidades.String.OnlyNumbers(colRemetente.Valor), out double cpfcnpj);
                            if (cpfcnpj > 0)
                            {
                                remetente = repCliente.BuscarPorCPFCNPJ(cpfcnpj);
                                if (remetente == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O remetente informado não existe na base multisoftware.", i));
                                    continue;
                                }
                            }
                        }

                        Dominio.Entidades.Cliente destinatario = null;
                        if (colDestinatario != null)
                        {

                            double.TryParse(Utilidades.String.OnlyNumbers(colDestinatario.Valor), out double cpfcnpj);
                            if (cpfcnpj > 0)
                            {
                                destinatario = repCliente.BuscarPorCPFCNPJ(cpfcnpj);
                                if (destinatario == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário informado não existe na base multisoftware.", i));
                                    continue;
                                }
                            }
                        }

                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                        if (colTipoOperacao != null)
                        {
                            tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(colTipoOperacao.Valor);
                            if (tipoOperacao == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo de operação informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado CRContabilizacao = null;
                        if (colCRContabilizacao != null)
                        {
                            CRContabilizacao = repCentroResultado.BuscarPorPlano(colCRContabilizacao.Valor);
                            if (CRContabilizacao == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O centro de resultado de contabilização informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }


                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado CRICMS = null;
                        if (colCRICMS != null)
                        {
                            CRICMS = repCentroResultado.BuscarPorPlano(colCRICMS.Valor);
                            if (CRICMS == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O centro de resultado de ICMS informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado CRPIS = null;
                        if (colCRPIS != null)
                        {
                            CRPIS = repCentroResultado.BuscarPorPlano(colCRPIS.Valor);
                            if (CRPIS == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O centro de resultado de PIS informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado CRCOFINS = null;
                        if (colCRCOFINS != null)
                        {
                            CRCOFINS = repCentroResultado.BuscarPorPlano(colCRCOFINS.Valor);
                            if (CRCOFINS == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O centro de resultado de COFINS informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }

                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado CREscrituracao = null;
                        if (colCREscrituracao != null)
                        {
                            CREscrituracao = repCentroResultado.BuscarPorPlano(colCREscrituracao.Valor);
                            if (CREscrituracao == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O centro de resultado de escrituração informado não existe na base multisoftware.", i));
                                continue;
                            }
                        }


                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado();
                        configuracaoCentroResultado.Ativo = true;

                        configuracaoCentroResultado.Remetente = remetente;
                        configuracaoCentroResultado.Tomador = tomador;
                        configuracaoCentroResultado.TipoOperacao = tipoOperacao;
                        configuracaoCentroResultado.Destinatario = destinatario;
                        configuracaoCentroResultado.CentroResultadoEscrituracao = CREscrituracao;
                        configuracaoCentroResultado.CentroResultadoContabilizacao = CRContabilizacao;
                        configuracaoCentroResultado.CentroResultadoICMS = CRICMS;
                        configuracaoCentroResultado.CentroResultadoPIS = CRPIS;
                        configuracaoCentroResultado.CentroResultadoCOFINS = CRCOFINS;

                        Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoExistente = repConfiguracaoCentroResultado.BuscarPorParametros(configuracaoCentroResultado);

                        if (configuracaoCentroResultadoExistente != null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Já existe uma regra cadastrada para essa configuração de Centros de Resultado", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                        repConfiguracaoCentroResultado.Inserir(configuracaoCentroResultado, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoCentroResultado, null, "Adicionou o documento " + configuracaoCentroResultado.Descricao + " por importação.", unitOfWork);

                        contador++;

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                        unitOfWork.CommitChanges();

                        if ((i % 10) == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                        continue;
                    }
                }


                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

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

        #endregion

        #region MétodosPrivados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoConfiguracaoCentroCustoEmbarcador()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CPF/CNPJ Remetente", Propriedade = "Remetente", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CPF/CNPJ Destinatário", Propriedade = "Destinatario", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CPF/CNPJ Tomador", Propriedade = "Tomador", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Tipo de Operação", Propriedade = "TipoOperacao", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Centro Resultado Contabilização", Propriedade = "CRContabilizacao", Obrigatorio = true, Tamanho = 200, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Centro Resultado Escrituração", Propriedade = "CREscrituracao", Obrigatorio = true, Tamanho = 200, Regras = new List<string> { "required" } });

            if (ConfiguracaoEmbarcador.CentroResultadoPedidoObrigatorio)
            {
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Centro Resultado ICMS", Propriedade = "CRICMS", Obrigatorio = false, Tamanho = 200 });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Centro Resultado PIS", Propriedade = "CRPIS", Obrigatorio = false, Tamanho = 200 });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Centro Resultado COFINS", Propriedade = "CRCOFINS", Obrigatorio = false, Tamanho = 200 });
            }


            return configuracoes;
        }

        private string preecherConfiguracaoCentroResultado(ref Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorioCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

            string retorno = "";

            double remetente, destinatario, tomador, expedidor, recebedor;
            double.TryParse(Request.Params("Remetente"), out remetente);
            double.TryParse(Request.Params("Destinatario"), out destinatario);
            double.TryParse(Request.Params("Tomador"), out tomador);
            double.TryParse(Request.Params("Expedidor"), out expedidor);
            double.TryParse(Request.Params("Recebedor"), out recebedor);

            int filial, origem, grupoRemetente, grupoDestinatario, grupoTomador, rotaFrete, tipoOperacao, grupoProduto, centroResultadoEscrituracao, centroResultadoContabilizacao, centroResultadoICMS, centroResultadoPIS, centroResultadoCOFINS, empresa, tipoOcorrencia;
            int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
            int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
            int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
            int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
            int.TryParse(Request.Params("TipoOcorrencia"), out tipoOcorrencia);
            int.TryParse(Request.Params("GrupoProduto"), out grupoProduto);
            int.TryParse(Request.Params("Empresa"), out empresa);
            int.TryParse(Request.Params("Filial"), out filial);
            int.TryParse(Request.Params("Origem"), out origem);
            int.TryParse(Request.Params("RotaFrete"), out rotaFrete);
            int.TryParse(Request.Params("CentroResultadoEscrituracao"), out centroResultadoEscrituracao);
            int.TryParse(Request.Params("CentroResultadoContabilizacao"), out centroResultadoContabilizacao);
            int.TryParse(Request.Params("CentroResultadoICMS"), out centroResultadoICMS);
            int.TryParse(Request.Params("CentroResultadoPIS"), out centroResultadoPIS);
            int.TryParse(Request.Params("CentroResultadoCOFINS"), out centroResultadoCOFINS);
            int categoriaDestinatario = Request.GetIntParam("CategoriaDestinatario");
            int categoriaRemetente = Request.GetIntParam("CategoriaRemetente");
            int categoriaTomador = Request.GetIntParam("CategoriaTomador");

            bool ativo;
            bool.TryParse(Request.Params("Ativo"), out ativo);

            configuracaoCentroResultado.Ativo = ativo;

            configuracaoCentroResultado.Remetente = remetente > 0 ? repCliente.BuscarPorCPFCNPJ(remetente) : null;
            configuracaoCentroResultado.Tomador = tomador > 0 ? repCliente.BuscarPorCPFCNPJ(tomador) : null;
            configuracaoCentroResultado.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;
            configuracaoCentroResultado.Recebedor = recebedor > 0 ? repCliente.BuscarPorCPFCNPJ(recebedor) : null;
            configuracaoCentroResultado.Expedidor = expedidor > 0 ? repCliente.BuscarPorCPFCNPJ(expedidor) : null;
            configuracaoCentroResultado.Origem = origem > 0 ? repLocalidade.BuscarPorCodigo(origem) : null;
            configuracaoCentroResultado.Filial = filial > 0 ? repFilial.BuscarPorCodigo(filial) : null;
            configuracaoCentroResultado.TipoOperacao = tipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacao) : null;
            configuracaoCentroResultado.TipoOcorrencia = tipoOcorrencia > 0 ? repTipoOcorrencia.BuscarPorCodigo(tipoOcorrencia) : null;
            configuracaoCentroResultado.Destinatario = destinatario > 0 ? repCliente.BuscarPorCPFCNPJ(destinatario) : null;
            configuracaoCentroResultado.CategoriaDestinatario = categoriaDestinatario > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaDestinatario) : null;
            configuracaoCentroResultado.CategoriaRemetente = categoriaRemetente > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaRemetente) : null;
            configuracaoCentroResultado.CategoriaTomador = categoriaTomador > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaTomador) : null;
            configuracaoCentroResultado.GrupoDestinatario = repGrupoPessoas.BuscarPorCodigo(grupoDestinatario);
            configuracaoCentroResultado.GrupoRemetente = repGrupoPessoas.BuscarPorCodigo(grupoRemetente);
            configuracaoCentroResultado.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
            configuracaoCentroResultado.GrupoProduto = repGrupoProduto.BuscarPorCodigo(grupoProduto);
            configuracaoCentroResultado.RotaFrete = repRotaFrete.BuscarPorCodigo(rotaFrete);
            configuracaoCentroResultado.CentroResultadoEscrituracao = repCentroResultado.BuscarPorCodigo(centroResultadoEscrituracao);
            configuracaoCentroResultado.CentroResultadoContabilizacao = repCentroResultado.BuscarPorCodigo(centroResultadoContabilizacao);

            if (centroResultadoICMS > 0)
                configuracaoCentroResultado.CentroResultadoICMS = repCentroResultado.BuscarPorCodigo(centroResultadoICMS);
            else
                configuracaoCentroResultado.CentroResultadoICMS = null;

            if (centroResultadoPIS > 0)
                configuracaoCentroResultado.CentroResultadoPIS = repCentroResultado.BuscarPorCodigo(centroResultadoPIS);
            else
                configuracaoCentroResultado.CentroResultadoPIS = null;

            if (centroResultadoCOFINS > 0)
                configuracaoCentroResultado.CentroResultadoCOFINS = repCentroResultado.BuscarPorCodigo(centroResultadoCOFINS);
            else
                configuracaoCentroResultado.CentroResultadoCOFINS = null;

            configuracaoCentroResultado.ValorMaximoCentroContabilizacao = Request.GetDecimalParam("ValorMaximoCentroContabilizacao");
            configuracaoCentroResultado.ItemServico = Request.GetStringParam("ItemServico");

            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoExistente = repConfiguracaoCentroResultado.BuscarPorParametros(configuracaoCentroResultado);

            if (configuracaoCentroResultadoExistente != null && configuracaoCentroResultadoExistente.Codigo != configuracaoCentroResultado.Codigo)
                retorno = "Já existe uma regra cadastrada para essa configuração de Centros de Resultado";

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado()
            {
                Remetente = Request.GetDoubleParam("Remetente"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                Tomador = Request.GetDoubleParam("Tomador"),
                Empresas = Request.GetListParam<int>("Empresas"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                TipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                GrupoDestinatario = Request.GetIntParam("GrupoDestinatario"),
                GrupoRemetente = Request.GetIntParam("GrupoRemetente"),
                GrupoTomador = Request.GetIntParam("GrupoTomador"),
                CategoriaDestinatario = Request.GetIntParam("CategoriaDestinatario"),
                CategoriaRemetente = Request.GetIntParam("CategoriaRemetente"),
                CategoriaTomador = Request.GetIntParam("CategoriaTomador"),
                CentroResultado = Request.GetIntParam("CentroResultado"),
                Situacao = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
            };

            return filtrosPesquisa;
        }
        #endregion
    }
}
