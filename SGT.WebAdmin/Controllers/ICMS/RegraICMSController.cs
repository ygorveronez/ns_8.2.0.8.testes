using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.ICMS
{
    [CustomAuthorize("ICMS/RegraICMS")]
    public class RegraICMSController : BaseController
    {
        #region Construtores

        public RegraICMSController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

                SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : SituacaoAtivoPesquisa.Ativo;

                double remetente, destinatario, tomador;
                int grupoRemetente, grupoDestinatario, grupoTomador, empresa;
                double.TryParse(Request.Params("Remetente"), out remetente);
                double.TryParse(Request.Params("Destinatario"), out destinatario);
                double.TryParse(Request.Params("Tomador"), out tomador);
                int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
                int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
                int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
                int.TryParse(Request.Params("Empresa"), out empresa);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa.Codigo;

                string ufEmitente = Request.Params("UFEmitente");
                string ufEmitenteDiferente = Request.Params("UFEmitenteDiferente");
                string ufOrigem = Request.Params("UFOrigem");
                string ufDestino = Request.Params("UFDestino");
                string ufTomador = Request.Params("UFTomador");
                string descricao = Request.GetStringParam("Descricao");

                DateTime? dataInicio = null;
                DateTime? dataFim = null;
                if (DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicioAux))
                    dataInicio = dataInicioAux.Date;
                if (DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFimAux))
                    dataFim = dataFimAux.Date;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Emitente", "UFEmitente", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", "DescricaoRegra", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vig. Início", "VigenciaInicio", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vig. Fim", "VigenciaFim", 7, Models.Grid.Align.center, true);

                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "UFEmitente")
                    propOrdena += ".Sigla";
                if (propOrdena == "Origem" || propOrdena == "Destino" || propOrdena == "Tomador")
                    propOrdena = "UF" + propOrdena + ".Sigla";

                int totalRegistros = repRegraICMS.ContarConsulta(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, ufEmitente, ufEmitenteDiferente, ufOrigem, ufDestino, ufTomador, ativo, dataInicio, dataFim, descricao);
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> listaRegraICMS = totalRegistros > 0 ? repRegraICMS.Consultar(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, ufEmitente, ufEmitenteDiferente, ufOrigem, ufDestino, ufTomador, ativo, dataInicio, dataFim, descricao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

                var retorno = (
                    from obj in listaRegraICMS
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao,
                        UFEmitente = obj.UFEmitente != null ? obj.UFEmitente.Sigla : "",
                        Destino = obj.UFDestino != null ? obj.UFDestino.Sigla + (obj.Destinatario != null ? " - " + obj.Destinatario.Descricao : (obj.GrupoDestinatario != null ? " - " + obj.GrupoDestinatario.Descricao : "")) : "" + (obj.Destinatario != null ? obj.Destinatario.Descricao : (obj.GrupoDestinatario != null ? obj.GrupoDestinatario.Descricao : "")),
                        Origem = obj.UFOrigem != null ? obj.UFOrigem.Sigla + (obj.Remetente != null ? " - " + obj.Remetente.Descricao : (obj.GrupoRemetente != null ? " - " + obj.GrupoRemetente.Descricao : "")) : "" + (obj.Remetente != null ? obj.Remetente.Descricao : (obj.GrupoRemetente != null ? obj.GrupoRemetente.Descricao : "")),
                        Tomador = obj.UFTomador != null ? obj.UFTomador.Sigla + (obj.Tomador != null ? " - " + obj.Tomador.Descricao : (obj.GrupoTomador != null ? " - " + obj.GrupoTomador.Descricao : "")) : "" + (obj.Tomador != null ? obj.Tomador.Descricao : (obj.GrupoTomador != null ? obj.GrupoTomador.Descricao : "")),
                        Transportador = obj.Empresa != null ? obj.Empresa?.RazaoSocial + " (" + obj.Empresa?.CNPJ_Formatado + ")" : "",
                        obj.DescricaoRegra,
                        VigenciaInicio = obj.VigenciaInicio?.ToString("dd/MM/yyyy") ?? " - ",
                        VigenciaFim = obj.VigenciaFim?.ToString("dd/MM/yyyy") ?? " - ",
                        obj.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

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

                Servicos.Embarcador.ICMS.RegraICMSAprovacao servicoRegraICMSAprovacao = new Servicos.Embarcador.ICMS.RegraICMSAprovacao(unitOfWork);
                Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.Entidades.Embarcador.ICMS.RegraICMS();

                preecherRegraICMS(regraICMS, unitOfWork);

                regraICMS.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido  por ", this.Usuario.CPF, " - ", this.Usuario.Nome);

                repositorioRegraICMS.Inserir(regraICMS, Auditado);

                if (servicoRegraICMSAprovacao.UtilizarAlcadaAprovacaoAlteracaoRegraICMS())
                {
                    regraICMS.DataAlteracao = DateTime.Now;
                    regraICMS.Tipo = TipoRegraICMS.Alteracao;
                    servicoRegraICMSAprovacao.CriarAprovacao(regraICMS, TipoServicoMultisoftware);
                    repositorioRegraICMS.Atualizar(regraICMS);
                }

                unitOfWork.CommitChanges();

                if (regraICMS.Tipo == TipoRegraICMS.Ativa)
                    Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (ServicoException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (Exception exececao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exececao);
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
                Servicos.Embarcador.ICMS.RegraICMSAprovacao servicoRegraICMSAprovacao = new Servicos.Embarcador.ICMS.RegraICMSAprovacao(unitOfWork);
                Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repositorioRegraICMS.BuscarPorCodigo(codigo, true);
                bool utilizarAlcadaAprovacaoAlteracaoRegraICMS = servicoRegraICMSAprovacao.UtilizarAlcadaAprovacaoAlteracaoRegraICMS();

                if (utilizarAlcadaAprovacaoAlteracaoRegraICMS)
                {
                    Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSAprovacaoPendente = repositorioRegraICMS.BuscarPorAprovacaoPendente(codigo);

                    if (regraICMSAprovacaoPendente != null)
                        throw new ControllerException($"A alteração realizada em {regraICMSAprovacaoPendente.DataAlteracao.Value.ToString("dd/MM/yyyy HH:mm")} ainda está pendente de aprovação. A aprovação deve ser finalizada antes de realizar uma nova alteração.");

                    Servicos.Embarcador.Carga.ICMS servicoRegraICMS = new Servicos.Embarcador.Carga.ICMS();
                    Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSClonada = servicoRegraICMS.DuplicarParaAlteracao(regraICMS, unitOfWork);
                    regraICMS = regraICMSClonada;
                }

                preecherRegraICMS(regraICMS, unitOfWork);

                regraICMS.Log += string.Concat("\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome);
                repositorioRegraICMS.Atualizar(regraICMS, Auditado);

                if (utilizarAlcadaAprovacaoAlteracaoRegraICMS)
                {
                    servicoRegraICMSAprovacao.CriarAprovacao(regraICMS, TipoServicoMultisoftware);
                    repositorioRegraICMS.Atualizar(regraICMS);
                }

                unitOfWork.CommitChanges();

                if (regraICMS.Tipo == TipoRegraICMS.Ativa || regraICMS.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.Aprovada)
                    Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (ServicoException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (Exception exececao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exececao);
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
                Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).ObterRegrasICMS();

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repRegraICMS.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    regraICMS.Descricao,
                    regraICMS.AtividadeTomadorDiferente,
                    regraICMS.TipoModal,
                    regraICMS.TipoServico,
                    regraICMS.TipoPagamento,
                    regraICMS.UFOrigemIgualUFTomador,
                    regraICMS.RegimeTributarioTomador,
                    regraICMS.RegimeTributarioTomadorDiferente,
                    regraICMS.EstadoOrigemDiferente,
                    regraICMS.EstadoDestinoDiferente,
                    regraICMS.Ativo,
                    regraICMS.SomenteOptanteSimplesNacional,
                    regraICMS.Codigo,
                    regraICMS.Aliquota,
                    regraICMS.PercentualReducaoBC,
                    ComAliquota = regraICMS.Aliquota.HasValue,
                    VigenciaInicio = regraICMS.VigenciaInicio.HasValue ? regraICMS.VigenciaInicio.Value.ToString("dd/MM/yyyy") : "",
                    VigenciaFim = regraICMS.VigenciaFim.HasValue ? regraICMS.VigenciaFim.Value.ToString("dd/MM/yyyy") : "",
                    AtividadeDestinatario = new { Codigo = regraICMS.AtividadeDestinatario != null ? regraICMS.AtividadeDestinatario.Codigo : 0, Descricao = regraICMS.AtividadeDestinatario != null ? regraICMS.AtividadeDestinatario.Descricao : "" },
                    AtividadeRemetente = new { Codigo = regraICMS.AtividadeRemetente != null ? regraICMS.AtividadeRemetente.Codigo : 0, Descricao = regraICMS.AtividadeRemetente != null ? regraICMS.AtividadeRemetente.Descricao : "" },
                    AtividadeTomador = new { Codigo = regraICMS.AtividadeTomador != null ? regraICMS.AtividadeTomador.Codigo : 0, Descricao = regraICMS.AtividadeTomador != null ? regraICMS.AtividadeTomador.Descricao : "" },
                    CFOP = new { Codigo = regraICMS.CFOP != null ? regraICMS.CFOP.Codigo : 0, Descricao = regraICMS.CFOP != null ? regraICMS.CFOP.CodigoCFOP.ToString() : "" },
                    regraICMS.CST,
                    regraICMS.DescricaoRegra,
                    Destinatario = new { Codigo = regraICMS.Destinatario != null ? regraICMS.Destinatario.CPF_CNPJ : 0, Descricao = regraICMS.Destinatario != null ? regraICMS.Destinatario.Descricao : "" },
                    Remetente = new { Codigo = regraICMS.Remetente != null ? regraICMS.Remetente.CPF_CNPJ : 0, Descricao = regraICMS.Remetente != null ? regraICMS.Remetente.Descricao : "" },
                    Tomador = new { Codigo = regraICMS.Tomador != null ? regraICMS.Tomador.CPF_CNPJ : 0, Descricao = regraICMS.Tomador != null ? regraICMS.Tomador.Descricao : "" },
                    GrupoDestinatario = new { Codigo = regraICMS.GrupoDestinatario != null ? regraICMS.GrupoDestinatario.Codigo : 0, Descricao = regraICMS.GrupoDestinatario != null ? regraICMS.GrupoDestinatario.Descricao : "" },
                    GrupoRemetente = new { Codigo = regraICMS.GrupoRemetente != null ? regraICMS.GrupoRemetente.Codigo : 0, Descricao = regraICMS.GrupoRemetente != null ? regraICMS.GrupoRemetente.Descricao : "" },
                    GrupoTomador = new { Codigo = regraICMS.GrupoTomador != null ? regraICMS.GrupoTomador.Codigo : 0, Descricao = regraICMS.GrupoTomador != null ? regraICMS.GrupoTomador.Descricao : "" },
                    Empresa = new { Codigo = regraICMS.Empresa != null ? regraICMS.Empresa.Codigo : 0, Descricao = regraICMS.Empresa != null ? regraICMS.Empresa.RazaoSocial + " (" + regraICMS.Empresa.CNPJ_Formatado + ")" : "" },
                    regraICMS.ImprimeLeiNoCTe,
                    regraICMS.SetorEmpresa,
                    regraICMS.NumeroProposta,
                    UFEmitente = regraICMS.UFEmitente != null ? regraICMS.UFEmitente.Sigla : "",
                    UFDestino = regraICMS.UFDestino != null ? regraICMS.UFDestino.Sigla : "",
                    UFOrigem = regraICMS.UFOrigem != null ? regraICMS.UFOrigem.Sigla : "",
                    UFEmitenteDiferente = regraICMS.UFEmitenteDiferente != null ? regraICMS.UFEmitenteDiferente.Sigla : "",
                    UFTomador = regraICMS.UFTomador != null ? regraICMS.UFTomador.Sigla : "",
                    regraICMS.EstadoTomadorDiferente,
                    ProdutoEmbarcador = new { Codigo = regraICMS.ProdutoEmbarcador != null ? regraICMS.ProdutoEmbarcador.Codigo : 0, Descricao = regraICMS.ProdutoEmbarcador != null ? regraICMS.ProdutoEmbarcador.Descricao : "" },
                    TipoOperacao = new { Codigo = regraICMS.TipoOperacao?.Codigo ?? 0, Descricao = regraICMS.TipoOperacao?.Descricao ?? "" },
                    regraICMS.ZerarValorICMS,
                    regraICMS.IncluirPisConfisNaBC,
                    regraICMS.NaoIncluirPisConfisNaBCParaComplementos,
                    regraICMS.PercentualCreditoPresumido,
                    regraICMS.DescontarICMSDoValorAReceber,
                    regraICMS.NaoImprimirImpostosDACTE,
                    regraICMS.NaoEnviarImpostoICMSNaEmissaoCte,
                    regraICMS.NaoIncluirICMSValorFrete,
                    regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao,
                    regraICMS.NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber,
                    TiposOperacao = (from obj in regraICMS.TiposOperacao
                                     select new
                                     {
                                         obj.Codigo,
                                         Descricao = obj.Descricao
                                     }).ToList(),
                    TiposDeCarga = (from obj in regraICMS.TiposDeCarga
                                    select new
                                    {
                                        obj.Codigo,
                                        Descricao = obj.Descricao
                                    }).ToList(),
                    ProdutosEmbarcador = (from obj in regraICMS.ProdutosEmbarcador
                                          select new
                                          {
                                              obj.Codigo,
                                              Descricao = obj.Descricao
                                          }).ToList()
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
                Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repRegraICMS.BuscarPorCodigo(codigo);
                repRegraICMS.Deletar(regraICMS, Auditado);
                Servicos.Embarcador.ICMS.RegrasCalculoImpostos regrasCalculoImpostos = Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork);
                regrasCalculoImpostos.AtualizarRegrasICMS(unitOfWork);
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

        #endregion

        #region Métodos Privados

        private void preecherRegraICMS(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            double remetente, destinatario, tomador;
            double.TryParse(Request.Params("Remetente"), out remetente);
            double.TryParse(Request.Params("Destinatario"), out destinatario);
            double.TryParse(Request.Params("Tomador"), out tomador);

            int grupoRemetente, grupoDestinatario, grupoTomador, atividadeRemetente, atividadeDestinatario, atividadeTomador, cfop, produtoEmbarcador, empresa;
            int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
            int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
            int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
            int.TryParse(Request.Params("AtividadeRemetente"), out atividadeRemetente);
            int.TryParse(Request.Params("AtividadeDestinatario"), out atividadeDestinatario);
            int.TryParse(Request.Params("AtividadeTomador"), out atividadeTomador);
            int.TryParse(Request.Params("ProdutoEmbarcador"), out produtoEmbarcador);
            int.TryParse(Request.Params("Empresa"), out empresa);
            int.TryParse(Request.Params("CFOP"), out cfop);
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            DateTime? vigenciaInicio = null;
            if (DateTime.TryParse(Request.Params("VigenciaInicio"), out DateTime vigenciaInicioAux))
                vigenciaInicio = vigenciaInicioAux;
            DateTime? vigenciaFim = null;
            if (DateTime.TryParse(Request.Params("VigenciaFim"), out DateTime vigenciaFimAux))
                vigenciaFim = vigenciaFimAux;

            string ufOrigem = Request.Params("UFOrigem");
            string ufDestino = Request.Params("UFDestino");
            string ufTomador = Request.Params("UFTomador");
            string ufEmitente = Request.Params("UFEmitente");
            string UFEmitenteDiferente = Request.Params("UFEmitenteDiferente");
            string setorEmpresa = Request.Params("SetorEmpresa");
            string numeroProposta = Request.GetStringParam("NumeroProposta");

            string percentualReducaoBC = Request.Params("PercentualReducaoBC") ?? string.Empty;
            string cst = Request.Params("CST");
            string descricaoRegra = Request.Params("DescricaoRegra");

            bool incluirPisConfisNaBC, naoIncluirPisConfisNaBCParaComplementos, zerarValorICMS, ativo, imprimeLeiNoCTe, somenteOptanteSimplesNacional, descontarICMSDoValorAReceber, naoReduzirRetencaoICMSDoValorDaPrestacao;
            bool.TryParse(Request.Params("ZerarValorICMS"), out zerarValorICMS);
            bool.TryParse(Request.Params("IncluirPisConfisNaBC"), out incluirPisConfisNaBC);
            bool.TryParse(Request.Params("NaoIncluirPisConfisNaBCParaComplementos"), out naoIncluirPisConfisNaBCParaComplementos);
            bool.TryParse(Request.Params("ImprimeLeiNoCTe"), out imprimeLeiNoCTe);
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("SomenteOptanteSimplesNacional"), out somenteOptanteSimplesNacional);
            bool.TryParse(Request.Params("DescontarICMSDoValorAReceber"), out descontarICMSDoValorAReceber);
            bool.TryParse(Request.Params("NaoReduzirRetencaoICMSDoValorDaPrestacao"), out naoReduzirRetencaoICMSDoValorDaPrestacao);

            bool.TryParse(Request.Params("EstadoOrigemDiferente"), out bool estadoOrigemDiferente);
            bool.TryParse(Request.Params("EstadoDestinoDiferente"), out bool estadoDestinoDiferente);
            bool atividadeTomadorDiferente = Request.GetBoolParam("AtividadeTomadorDiferente");
            bool regimeTributarioTomadorDiferente = Request.GetBoolParam("RegimeTributarioTomadorDiferente");

            bool comAliquota = Request.GetBoolParam("ComAliquota");

            RegimeTributario? regimeTributarioTomador = Request.GetNullableEnumParam<RegimeTributario>("RegimeTributarioTomador");
            TipoModal? tipoModal = Request.GetNullableEnumParam<TipoModal>("TipoModal");
            Dominio.Enumeradores.TipoServico? tipoServico = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoServico>("TipoServico");
            Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoPagamentoRegraICMS>("TipoPagamento");

            if (comAliquota)
            {
                regraICMS.Aliquota = Request.GetDecimalParam("Aliquota");
            }
            else
                regraICMS.Aliquota = null;

            if (percentualReducaoBC.Length > 0)
                regraICMS.PercentualReducaoBC = decimal.Parse(percentualReducaoBC.Replace("%", ""));
            else
                regraICMS.PercentualReducaoBC = null;

            regraICMS.Descricao = Request.GetStringParam("Descricao");
            regraICMS.NaoImprimirImpostosDACTE = Request.GetBoolParam("NaoImprimirImpostosDACTE");
            regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = Request.GetBoolParam("NaoEnviarImpostoICMSNaEmissaoCte");
            regraICMS.NaoIncluirICMSValorFrete = Request.GetNullableBoolParam("NaoIncluirICMSValorFrete");
            regraICMS.EstadoOrigemDiferente = estadoOrigemDiferente;
            regraICMS.EstadoDestinoDiferente = estadoDestinoDiferente;
            regraICMS.VigenciaInicio = vigenciaInicio;
            regraICMS.VigenciaFim = vigenciaFim;
            regraICMS.CST = cst;
            regraICMS.DescricaoRegra = descricaoRegra;
            regraICMS.Ativo = ativo;
            regraICMS.ImprimeLeiNoCTe = imprimeLeiNoCTe;
            regraICMS.SetorEmpresa = setorEmpresa;
            regraICMS.NumeroProposta = numeroProposta;
            regraICMS.ZerarValorICMS = zerarValorICMS;
            regraICMS.IncluirPisConfisNaBC = incluirPisConfisNaBC;
            regraICMS.NaoIncluirPisConfisNaBCParaComplementos = naoIncluirPisConfisNaBCParaComplementos;
            regraICMS.SomenteOptanteSimplesNacional = somenteOptanteSimplesNacional;
            regraICMS.DescontarICMSDoValorAReceber = descontarICMSDoValorAReceber;
            regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = naoReduzirRetencaoICMSDoValorDaPrestacao;
            regraICMS.NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber = Request.GetBoolParam("NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber");

            regraICMS.Remetente = remetente > 0 ? repCliente.BuscarPorCPFCNPJ(remetente) : null;
            regraICMS.Tomador = tomador > 0 ? repCliente.BuscarPorCPFCNPJ(tomador) : null;

            regraICMS.TipoServicoMultisoftware = TipoServicoMultisoftware;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                regraICMS.Empresa = this.Empresa;
            else
                regraICMS.Empresa = repEmpresa.BuscarPorCodigo(empresa);

            regraICMS.Destinatario = destinatario > 0 ? repCliente.BuscarPorCPFCNPJ(destinatario) : null;
            regraICMS.GrupoDestinatario = repGrupoPessoas.BuscarPorCodigo(grupoDestinatario);
            regraICMS.GrupoRemetente = repGrupoPessoas.BuscarPorCodigo(grupoRemetente);
            regraICMS.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
            regraICMS.AtividadeRemetente = repAtividade.BuscarPorCodigo(atividadeRemetente);
            regraICMS.AtividadeDestinatario = repAtividade.BuscarPorCodigo(atividadeDestinatario);
            regraICMS.AtividadeTomador = repAtividade.BuscarPorCodigo(atividadeTomador);
            regraICMS.AtividadeTomadorDiferente = atividadeTomadorDiferente;
            regraICMS.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(produtoEmbarcador);
            regraICMS.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            regraICMS.CFOP = repCFOP.BuscarPorCodigo(cfop);
            regraICMS.EstadoTomadorDiferente = Request.GetBoolParam("EstadoTomadorDiferente");
            regraICMS.UFOrigemIgualUFTomador = Request.GetBoolParam("UFOrigemIgualUFTomador");

            if (!string.IsNullOrWhiteSpace(ufEmitente))
                regraICMS.UFEmitente = repEstado.BuscarPorSigla(ufEmitente);
            else
                regraICMS.UFEmitente = null;

            if (!string.IsNullOrWhiteSpace(UFEmitenteDiferente))
                regraICMS.UFEmitenteDiferente = repEstado.BuscarPorSigla(UFEmitenteDiferente);
            else
                regraICMS.UFEmitenteDiferente = null;

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                regraICMS.UFOrigem = repEstado.BuscarPorSigla(ufOrigem);
            else
                regraICMS.UFOrigem = null;

            if (!string.IsNullOrWhiteSpace(ufDestino))
                regraICMS.UFDestino = repEstado.BuscarPorSigla(ufDestino);
            else
                regraICMS.UFDestino = null;

            if (!string.IsNullOrWhiteSpace(ufTomador))
                regraICMS.UFTomador = repEstado.BuscarPorSigla(ufTomador);
            else
                regraICMS.UFTomador = null;

            if (decimal.TryParse(Request.Params("PercentualCreditoPresumido"), out decimal percentualCreditoPresumido))
                regraICMS.PercentualCreditoPresumido = percentualCreditoPresumido;
            else
                regraICMS.PercentualCreditoPresumido = null;

            if (tipoModal.HasValue)
                regraICMS.TipoModal = tipoModal;
            else
                regraICMS.TipoModal = null;

            if (tipoServico.HasValue)
                regraICMS.TipoServico = tipoServico;
            else
                regraICMS.TipoServico = null;

            if (tipoPagamento.HasValue)
                regraICMS.TipoPagamento = tipoPagamento;
            else
                regraICMS.TipoPagamento = null;

            if (regimeTributarioTomador.HasValue)
            {
                regraICMS.RegimeTributarioTomador = regimeTributarioTomador;
                regraICMS.RegimeTributarioTomadorDiferente = regimeTributarioTomadorDiferente;
            }
            else
            {
                regraICMS.RegimeTributarioTomador = null;
                regraICMS.RegimeTributarioTomadorDiferente = false;
            }

            SetarTiposOperacao(regraICMS, unitOfWork);
            SetarTiposDeCarga(regraICMS, unitOfWork);
            SetarProdutosEmbarcador(regraICMS, unitOfWork);

            Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMSExistente = repRegraICMS.BuscarPorParametros(somenteOptanteSimplesNacional, remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, ufEmitente, UFEmitenteDiferente, ufOrigem, ufDestino, ufTomador, atividadeRemetente, atividadeDestinatario, atividadeTomador, produtoEmbarcador, vigenciaInicio, regimeTributarioTomador, regimeTributarioTomadorDiferente, atividadeTomadorDiferente, setorEmpresa, codigoTipoOperacao, tipoModal, tipoServico, tipoPagamento, numeroProposta, regraICMS.EstadoDestinoDiferente, regraICMS.EstadoOrigemDiferente, regraICMS.EstadoTomadorDiferente);

            if (regraICMSExistente != null && regraICMSExistente.Codigo != (regraICMS.RegraOriginaria ?? regraICMS).Codigo)
                throw new ControllerException($"Já existe uma regra cadastrada para essa configuração de ICMS. Regra: {regraICMSExistente.Descricao}");
        }

        private void SetarTiposOperacao(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            regraICMS.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            foreach (var tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOp = repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Codigo);

                regraICMS.TiposOperacao.Add(tipoOp);
            }
        }

        private void SetarTiposDeCarga(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);

            dynamic tiposDeCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDeCarga"));

            regraICMS.TiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var tipoDeCarga in tiposDeCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCG = repTipoDeCarga.BuscarPorCodigo((int)tipoDeCarga.Codigo);

                regraICMS.TiposDeCarga.Add(tipoDeCG);
            }
        }

        private void SetarProdutosEmbarcador(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeTrabalho);

            dynamic produtosEmbarcador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ProdutosEmbarcador"));

            regraICMS.ProdutosEmbarcador = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            foreach (var prod in produtosEmbarcador)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo((int)prod.Codigo);

                regraICMS.ProdutosEmbarcador.Add(produtoEmbarcador);
            }
        }

        #endregion
    }
}
