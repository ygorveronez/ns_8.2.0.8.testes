using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize(new string[] { "ConsultarCargaSubContratacaoPreCTe", "DownloadXMLPreCTe", "DownloadXMLCTeTerceiro", "PesquisaAutorizacoes", "DetalhesAutorizacao", "ConsultarCIOT", "ConsultarValor", "BuscarValorPorCodigo", "ConsultarContratoFreteCTe" }, "Terceiros/ContratoFrete")]
    public class ContratoFreteController : BaseController
    {
        #region Construtores

        public ContratoFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region ContratoFrete

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Contrato", "NumeroContrato", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Terceiro", "TransportadorTerceiro", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Val. Contrato", "ValorFreteSubcontratacao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Val. Outros Ad.", "ValorOutrosAdiantamento", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Val. Adiantamento", "ValorAdiantamento", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Emissão do Contrato", "DataEmissaoContrato", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Bloqueado", "DescricaoBloqueado", 7, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoContratoFrete", 11, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Número CIOT", "NumeroCIOT", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome Motorista", "NomeMotorista", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Bloqueado", false);
                grid.AdicionarCabecalho("Descricao", false);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> listaContratoFrete = repContratoFrete.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repContratoFrete.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaContratoFrete
                             select new
                             {
                                 p.Codigo,
                                 CodigoCarga = p.Carga.Codigo,
                                 DataEmissaoContrato = p.DataEmissaoContrato,
                                 Situacao = p.SituacaoContratoFrete,
                                 p.Bloqueado,
                                 DescricaoBloqueado = p.Bloqueado ? "Sim" : "Não",
                                 TransportadorTerceiro = p.TransportadorTerceiro.Nome,
                                 p.NumeroContrato,
                                 Descricao = p.NumeroContrato,
                                 Carga = p.Carga.CodigoCargaEmbarcador,
                                 p.DescricaoSituacaoContratoFrete,
                                 ValorFreteSubcontratacao = p.ValorFreteSubcontratacao.ToString("n2"),
                                 ValorAdiantamento = p.ValorAdiantamento.ToString("n2"),
                                 ValorOutrosAdiantamento = p.ValorOutrosAdiantamento.ToString("n2"),
                                 NumeroCIOT = ObterCIOTContratoFrete(p, unitOfWork),
                                 NomeMotorista = p.Carga.NomeMotoristas,
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Justificativa", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Busca
                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> listaAutorizacao = repAprovacaoAlcadaContratoFrete.ConsultarAutorizacoesPorContrato(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaContratoFrete.ContarConsultaAutorizacoesPorContrato(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = obj.RegraContratoFreteTerceiro.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 Justificativa = obj.Motivo ?? string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
                             }).ToList();
                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete autorizacao = repAprovacaoAlcadaContratoFrete.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.RegraContratoFreteTerceiro.Descricao,
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> AlterarDadosContratoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int contrato);

                decimal.TryParse(Request.Params("PercentualAdiantamento"), out decimal percentualAdiantamentoFretesTerceiro);
                decimal.TryParse(Request.Params("PercentualAbastecimento"), out decimal percentualAbastecimentoFretesTerceiro);
                decimal.TryParse(Request.Params("ValorFreteSubcontratacao"), out decimal valorFreteSubcontratacao);

                string observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato, true);

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato pois o mesmo está vinculado à um CIOT.");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contratoFrete.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível alterar as informações do contrato de frete na situação atual.");

                if (contratoFrete.ValorFreteSubcontratacao != valorFreteSubcontratacao)
                {
                    contratoFrete.ValorFreteSubcontratacao = valorFreteSubcontratacao;
                    contratoFrete.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                    contratoFrete.Usuario = this.Usuario;
                }

                contratoFrete.PercentualAdiantamento = percentualAdiantamentoFretesTerceiro;
                contratoFrete.PercentualAbastecimento = percentualAbastecimentoFretesTerceiro;
                contratoFrete.Observacao = observacao;

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unitOfWork, TipoServicoMultisoftware);

                //decimal valorTotalDescontoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
                //decimal valorTotalAcrescimoAdiantamento = repContratoFreteValor.BuscarValorPorContratoFrete(contratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento);
                //decimal valorTotal = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio;

                //contratoFrete.ValorAdiantamento = ((valorTotal * contratoFrete.PercentualAdiantamento) / 100) + valorTotalAcrescimoAdiantamento - valorTotalDescontoAdiantamento;
                //contratoFrete.ValorAbastecimento = (valorTotal * (contratoFrete.PercentualAbastecimento / 100));

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                return new JsonpResult(serContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contrato.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contrato.Carga.DescricaoSituacaoCarga + ").");

                if (contrato.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Busca as regras
                unitOfWork.Start();

                bool possuiRegras = VerificarRegrasAutorizacao(ref contrato, TipoServicoMultisoftware, unitOfWork);

                // Persiste dados
                repContratoFrete.Atualizar(contrato);

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.ProcessarContratoAprovado(contrato, TipoServicoMultisoftware, this.Usuario.Empresa.TipoAmbiente, Auditado, unitOfWork, _conexao.StringConexao, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    contrato.Codigo,
                    CodigoCarga = contrato.Carga.Codigo,
                    PossuiRegra = possuiRegras
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int contrato = 0;
                int.TryParse(Request.Params("Codigo"), out contrato);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);
                return new JsonpResult(serContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do contrato o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReabrirContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ReAbrir))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                int contrato = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);

                string erro = string.Empty;
                unitOfWork.Start();

                if (!serContratoFrete.ReabrirContratoFrete(contratoFrete, unitOfWork, TipoServicoMultisoftware, Usuario, Auditado, out erro)) 
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(serContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unitOfWork));

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //public async Task<IActionResult> RejeitarContrato()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

        //        if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato))
        //            return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

        //        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

        //        int.TryParse(Request.Params("Codigo"), out int codigo);

        //        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);
        //        if (contrato.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao)
        //        {
        //            contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado;
        //            repContratoFrete.Atualizar(contrato);

        //            Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Rejeitou o Contrato.", unitOfWork);

        //            return new JsonpResult(true);
        //        }
        //        else
        //        {
        //            return new JsonpResult(false, true, "Não é possível rejeitar o contrato em sua atual situação");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        //public async Task<IActionResult> AprovarContrato()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

        //        if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato))
        //            return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

        //        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);


        //        Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

        //        int.TryParse(Request.Params("Codigo"), out int codigo);




        //        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);



        //        unitOfWork.CommitChanges();


        //        return new JsonpResult(serContratoFrete.ObterDetalhescontratoFrete(contrato, unitOfWork));
        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o contrato.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        public async Task<IActionResult> FinalizarContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Finalizar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoa = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadePessoaTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                //Repositorio.Embarcador.Terceiros.ContratoFreteCTe repCargaFreteSubContratacaoPreCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Codigo"), out int contrato);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoa = repModalidadePessoa.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, contratoFrete.TransportadorTerceiro.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = null;

                if (modalidadePessoa != null)
                    modalidadeTransportador = repModalidadePessoaTransportador.BuscarPorModalidade(modalidadePessoa.Codigo);

                if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                {
                    bool validarCanhotosPendentes = true;

                    if (modalidadeTransportador != null && !modalidadeTransportador.ExigeCanhotoFechamentoContratoFrete)
                        validarCanhotosPendentes = false;

                    int countCanhotosPendentes = repCanhoto.ContarCanhotosNaoEnviadosPorCarga(contratoFrete.Carga.Codigo);
                    //int countCTesPendentes = repCargaFreteSubContratacaoPreCTe.ContarPorCargaFreteSubContratacaoPendentesEnvio(contratoFrete.Codigo);

                    //removida validação pois não é utilizado por ninguem
                    //if (countCTesPendentes > 0)
                    //    return new JsonpResult(false, true, "Não é possível finalizar o contrato pois existem pré CT-es pendentes de envio.");

                    if (validarCanhotosPendentes && countCanhotosPendentes > 0)
                        return new JsonpResult(false, true, "Não é possível finalizar o contrato pois existem canhotos pendentes de envio para a carga.");

                    if (contratoFrete.ConfiguracaoCIOT != null)
                    {
                        string mensagem = "";

                        Dominio.Entidades.Embarcador.Documentos.CIOT ciotEncerrado = serContratoFrete.SolicitarFinalizacaoPorCIOT(contratoFrete, TipoServicoMultisoftware, unitOfWork, out mensagem);
                        if (ciotEncerrado != null)
                        {
                            unitOfWork.Start();

                            if (contratoFrete.SituacaoContratoFrete == SituacaoContratoFrete.Aprovado)
                                serContratoFrete.EncerramentoContratoViaCIOT(contratoFrete, ciotEncerrado, TipoServicoMultisoftware, unitOfWork, DateTime.Now, false);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Finalizou o Contrato.", unitOfWork);
                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            return new JsonpResult(false, true, mensagem);
                        }
                    }
                    else
                    {
                        unitOfWork.Start();
                        serContratoFrete.SolicitarFinalizacaoContratoFrete(contratoFrete, TipoServicoMultisoftware, unitOfWork, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Finalizou o Contrato.", unitOfWork);
                        unitOfWork.CommitChanges();
                    }



                    serHubCarga.InformarCargaAtualizada(contratoFrete.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é possível finalizar o contrato em sua atual situação");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int contrato = 0;
                int.TryParse(Request.Params("Codigo"), out contrato);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);

                if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                    return new JsonpResult(false, true, "Não é possível cancelar o contrato em sua atual situação");

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível cancelar o contrato pois o mesmo está vinculado à um CIOT.");

                unitOfWork.Start();

                string mensagemErro = "";
                Servicos.Embarcador.Terceiros.ContratoFrete.RealizarCancelamentoTotvs(contratoFrete, unitOfWork, out mensagemErro);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(true, mensagemErro);
                }

                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado;
                repContratoFrete.Atualizar(contratoFrete);

                string erro = "";

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
                if (!modalidadeTransportadoraPessoas.GerarPagamentoTerceiro)
                {
                    if (!serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(true, erro);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Cancelou o Contrato.", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(contratoFrete.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BloquearContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int contrato = 0;
                int.TryParse(Request.Params("Codigo"), out contrato);

                string justificativa = Request.Params("Justificativa");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);

                if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada || contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado)
                    return new JsonpResult(false, true, "Não é possível bloquear o contrato em sua atual situação");

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível bloquear o contrato pois o mesmo está vinculado à um CIOT.");

                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Length <= 20)
                    return new JsonpResult(false, true, "A justificativa deve ter mais de 20 caracteres.");

                contratoFrete.Bloqueado = true;
                contratoFrete.JustificativaBloqueio = justificativa;

                repContratoFrete.Atualizar(contratoFrete);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Bloqueou o Contrato.", unitOfWork);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(contratoFrete.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(new { Bloqueado = contratoFrete.Bloqueado, Justificativa = contratoFrete.JustificativaBloqueio });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesbloquearContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int contrato = 0;
                int.TryParse(Request.Params("Codigo"), out contrato);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contrato);

                contratoFrete.Bloqueado = false;

                repContratoFrete.Atualizar(contratoFrete);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete, null, "Desbloqueou o Contrato.", unitOfWork);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(contratoFrete.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o contrato.");
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
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo, true);

                if (contrato.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível atualizar o contrato pois o mesmo está vinculado à um CIOT.");

                if (contrato.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, $"Não é possível atualizar o contrato de frete na situação atual ({contrato.SituacaoContratoFrete.ObterDescricao()}).");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contrato.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contrato.Carga.DescricaoSituacaoCarga + ").");

                if (repCargaCIOT.ExistePorContratoFrete(contrato.Codigo))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato de frete pois ele está vinculado à um CIOT.");

                unitOfWork.Start();

                // Busca as regras
                bool possuiRegras = VerificarRegrasAutorizacao(ref contrato, TipoServicoMultisoftware, unitOfWork);

                // Persiste dados
                repContratoFrete.Atualizar(contrato, Auditado);

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.ProcessarContratoAprovado(contrato, TipoServicoMultisoftware, this.Usuario.Empresa.TipoAmbiente, Auditado, unitOfWork, _conexao.StringConexao, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    contrato.Codigo,
                    CodigoCarga = contrato.Carga.Codigo,
                    PossuiRegra = possuiRegras
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao atualizar os dados do contrato de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarValor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Aplicação", "DescricaoAplicacaoValor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Justificativa")
                    propOrdena += ".Descricao";
                else if (propOrdena == "DescricaoTipoJustificativa")
                    propOrdena = "TipoJustificativa";
                else if (propOrdena == "DescricaoAplicacaoValor")
                    propOrdena = "AplicacaoValor";

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> listaContratoFreteValor = repContratoFreteValor.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContratoFreteValor.ContarConsulta(codigo));

                var lista = (from p in listaContratoFreteValor
                             select new
                             {
                                 p.Codigo,
                                 Justificativa = p.Justificativa.Descricao,
                                 p.DescricaoTipoJustificativa,
                                 p.DescricaoAplicacaoValor,
                                 Valor = p.Valor.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> AdicionarValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoContratoFrete = Request.GetIntParam("ContratoFrete");
                int codigoJustificativa = Request.GetIntParam("Justificativa");
                int codigoTaxaTerceiro = Request.GetIntParam("TaxaTerceiro");

                decimal valor = Request.GetDecimalParam("Valor");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete, true);

                Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho, TipoServicoMultisoftware);

                if (contratoFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto ao contrato pois o mesmo está vinculado à um CIOT.");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contratoFrete.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual do contrato de frete.");

                unidadeTrabalho.Start();

                if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, valor, codigoJustificativa, observacao, codigoTaxaTerceiro, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                unidadeTrabalho.CommitChanges();

                Servicos.Embarcador.Terceiros.ContratoFrete svcContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                return new JsonpResult(svcContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoContratoFrete = Request.GetIntParam("ContratoFrete");
                int codigoJustificativa = Request.GetIntParam("Justificativa");
                int codigo = Request.GetIntParam("Codigo");
                int codigoTaxaTerceiro = Request.GetIntParam("TaxaTerceiro");

                decimal valor = Request.GetDecimalParam("Valor");

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete, true);

                if (contratoFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato pois o mesmo está vinculado à um CIOT.");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contratoFrete.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível atualizar um acréscimo/desconto na situação atual do contrato de frete.");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo, true);

                if (contratoFreteValor.PendenciaContratoFrete != null)
                    return new JsonpResult(false, true, "Não é possível atualizar um acréscimo/desconto gerado automaticamente por pendência de um contrato de frete anterior.");

                unidadeTrabalho.Start();

                if (!servicoContratoFrete.AtualizarValorDoContrato(out string erro, ref contratoFrete, ref contratoFreteValor, valor, codigoJustificativa, codigoTaxaTerceiro, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                unidadeTrabalho.CommitChanges();

                Servicos.Embarcador.Terceiros.ContratoFrete svcContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                return new JsonpResult(svcContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Terceiros/ContratoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo);
                if (contratoFreteValor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = contratoFreteValor.ContratoFrete;

                if (contratoFrete.ConfiguracaoCIOT != null)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato pois o mesmo está vinculado à um CIOT.");

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CargaPermiteAlterarContratoFrete(contratoFrete.Carga))
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível excluir um acréscimo/desconto na situação atual do contrato de frete.");

                unidadeTrabalho.Start();

                contratoFrete.Initialize();

                Servicos.Embarcador.Terceiros.ContratoFrete.RemoverVinculoPendenciaContratoFrete(contratoFreteValor, unidadeTrabalho);

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.RemoverValorDoContrato(out string erro, ref contratoFrete, ref contratoFreteValor, unidadeTrabalho, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                unidadeTrabalho.CommitChanges();

                Servicos.Embarcador.Terceiros.ContratoFrete svcContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                return new JsonpResult(svcContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarValorPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo);

                if (contratoFreteValor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    contratoFreteValor.Codigo,
                    ContratoFrete = contratoFreteValor.ContratoFrete.Codigo,
                    Valor = contratoFreteValor.Valor.ToString("n2"),
                    contratoFreteValor.Observacao,
                    Justificativa = new
                    {
                        Codigo = contratoFreteValor.Justificativa.Codigo,
                        Descricao = contratoFreteValor.Justificativa.Descricao
                    },
                    TaxaTerceiro = new
                    {
                        Codigo = contratoFreteValor.TaxaTerceiro?.Codigo ?? 0,
                        Descricao = contratoFreteValor.TaxaTerceiro?.Descricao ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadRomaneioEntrega()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoContratoFrete);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete);

                byte[] romaneioEntrega = Servicos.Embarcador.Terceiros.ContratoFrete.GerarRomaneioEntrega(contratoFrete);

                return Arquivo(romaneioEntrega, "application/pdf", "Romaneio de Entrega " + contratoFrete.NumeroContrato.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o romaneio de entrega.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContratoFrete = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> acrescimosDescontos = repContratoFreteAcrescimoDesconto.ConsultarParaContratoFrete(codigoContratoFrete, parametrosConsulta);
                int totalRegistros = repContratoFreteAcrescimoDesconto.ContarConsultaParaContratoFrete(codigoContratoFrete);

                var retorno = (from obj in acrescimosDescontos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToDateTimeString(),
                                   Justificativa = obj.Justificativa.Descricao,
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   Valor = obj.Valor.ToString("n2"),
                                   obj.Observacao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os acréscimos/desconto do contrato.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarIntegracaoQuitacaoAX()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato de Terceiro não encontrado.");

                string mensagemErro = "";
                Servicos.Embarcador.Terceiros.ContratoFrete.RealizarCompensacaoAX(contrato, unidadeTrabalho, out mensagemErro);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(false, false, mensagemErro);
                else
                    return new JsonpResult(true, true, "Sucesso");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar integração com AX do Contrato de Terceiro.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigo);

                grid.setarQuantidadeTotal(contratoFrete.ArquivosTransacao.Count());

                var retorno = (from obj in contratoFrete.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null || (integracao.ArquivoRequisicao == null && integracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Contrato Frete.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AjusteValoresContratoFreteCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasSemSaldoContratoRateado = repositorioCarga.BuscarCargasSemSaldoNoContratoSaldo(1000);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                foreach (var carga in cargasSemSaldoContratoRateado)
                {
                    Servicos.Log.TratarErro($"Gerando Contrato Saldo Mes para Carga: {carga.Codigo}", "GeracaoContratoSaldoMes");
                    int distanciaCarga = (int)servicoCarga.ObterDistancia(carga, configuracao, unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoSaldoMes = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes()
                    {
                        Carga = carga,
                        ContratoFreteTransportador = carga.ContratoFreteTransportador,
                        DataRegistro = carga.DataValorContrato ?? (carga.DataInicioEmissaoDocumentos.HasValue ? carga.DataInicioEmissaoDocumentos.Value : carga.DataCriacaoCarga),
                        Distancia = distanciaCarga,
                        ValorPagar = configuracao.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm ? carga.ValorFreteContratoFrete : carga.ValorFreteAPagar
                    };

                    repositorioContratoSaldoMes.Inserir(contratoSaldoMes);
                }
                return new JsonpResult(true, "1000 Cargas Processas com sucesso");
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(true, "Falha ao tentar processar");
            }
        }
        #endregion

        #region CTesTerceiros

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarContratoFreteCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransbordo = 0, codigoCarga = 0;

                int.TryParse(Request.Params("Transbordo"), out codigoTransbordo);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteCTe repCargaFreteSubContratacaoPreCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiros;

                if (codigoTransbordo > 0)
                    contratoFreteTerceiros = repContratoFreteTerceiro.BuscarPorTransbordo(codigoTransbordo);
                else
                    contratoFreteTerceiros = repContratoFreteTerceiro.BuscarPorCarga(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPreCTE", false);
                grid.AdicionarCabecalho("CteEnviado", false);
                grid.AdicionarCabecalho("CT-e referênciado", "CTEReferenciado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CT-e Terceiro", "CteTerceiro", 10, Models.Grid.Align.center, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "CTEReferenciado")
                    propOrdenacao = "CargaCTe.CTe.Numero";
                else
                    propOrdenacao = "PreCTe." + propOrdenacao;


                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> contratoFreteCTes = repCargaFreteSubContratacaoPreCTe.BuscarPorCargaFreteSubContratacao(contratoFreteTerceiros.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCargaFreteSubContratacaoPreCTe.ContarPorCargaFreteSubContratacao(contratoFreteTerceiros.Codigo));
                var lista = (from obj in contratoFreteCTes
                             select new
                             {
                                 obj.Codigo,
                                 CTEReferenciado = obj.CargaCTe.CTe.Numero,
                                 CodigoPreCTE = obj.PreCTe?.Codigo,
                                 CteTerceiro = obj.CTeTerceiro != null ? obj.CTeTerceiro.Numero.ToString() : "",
                                 CteEnviado = obj.CTeTerceiro == null ? false : true,
                                 Remetente = obj.PreCTe?.Remetente?.Descricao,
                                 Destinatario = obj.PreCTe?.Destinatario?.Descricao,
                                 Destino = obj.PreCTe?.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado,
                                 ValorFrete = obj.PreCTe?.ValorAReceber.ToString("n2"),
                                 DT_RowColor = obj.CTeTerceiro != null ? "#dff0d8" : "#fcf8e3",
                             }).ToList();
                grid.AdicionaRows(lista);
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLCTeTerceiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoContratoFreteCTe = int.Parse(Request.Params("ContratoFreteCTe"));
                if (codigoContratoFreteCTe > 0)
                {
                    Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe contratoFreteCte = repContratoFreteCTe.BuscarPorCodigo(codigoContratoFreteCTe);
                    Repositorio.Embarcador.CTe.CTeTerceiroXML repCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(unitOfWork);

                    if (contratoFreteCte.CTeTerceiro != null)
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = repCTeTerceiroXML.BuscarPorCodigoCTeTerceiro(contratoFreteCte.CTeTerceiro.Codigo);

                        byte[] data = System.Text.Encoding.Unicode.GetBytes(cteTerceiroXML.XML);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(contratoFreteCte.CTeTerceiro.ChaveAcesso, ".xml"));
                        }
                    }
                }
                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarCTe()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoPreCTe = int.Parse(Request.Params("CodigoPreCTe"));
                int codigoContratoFreteCTe = int.Parse(Request.Params("ContratoFreteCTe"));

                Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
                Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe contratoFreteCte = repContratoFreteCTe.BuscarPorCodigo(codigoContratoFreteCTe);
                if (contratoFreteCte.CTeTerceiro == null)
                {
                    Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = repPreCTe.BuscarPorCodigo(codigoPreCTe);

                    List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                    if (files.Count > 0)
                    {
                        Servicos.DTO.CustomFile file = files[0];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                        if (extensao.Equals(".xml"))
                        {
                            Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = null;
                            string retorno = serCargaPreCTe.ValidarPreCte(file.InputStream, preCte, ref cteIntegracao, ConfiguracaoEmbarcador, unitOfWork);
                            file.InputStream.Dispose();
                            if (retorno.Length == 0)
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCte, null, "Enviou XML do CT-e.", unitOfWork);

                                contratoFreteCte.CTeTerceiro = serCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cteIntegracao);
                                repContratoFreteCTe.Atualizar(contratoFreteCte);
                                //if (repContratoFreteCTe.ContarPorCargaFreteSubContratacaoPendentesEnvio(contratoFreteCte.ContratoFrete.Codigo) <= 0)
                                //{
                                //    contratoFreteCte.ContratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.LiberadoParaPagamento;
                                //    repContratoFrete.Atualizar(contratoFreteCte.ContratoFrete);
                                //    unitOfWork.CommitChanges();

                                //    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                                //    serHubCarga.InformarCargaAtualizada(contratoFreteCte.ContratoFrete.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
                                //}
                                //else
                                //{
                                //    unitOfWork.CommitChanges();
                                //}

                                unitOfWork.CommitChanges();

                                return new JsonpResult(true);
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, retorno);
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "A extensão do arquivo é inválida.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não cte já foi enviado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region CIOT

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransbordo = 0, codigoCarga = 0;

                int.TryParse(Request.Params("Transbordo"), out codigoTransbordo);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteCTe repCargaFreteSubContratacaoPreCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiros;

                if (codigoTransbordo > 0)
                    contratoFreteTerceiros = repContratoFreteTerceiro.BuscarPorTransbordo(codigoTransbordo);
                else
                    contratoFreteTerceiros = repContratoFreteTerceiro.BuscarPorCarga(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número do CIOT", "Numero", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 25, Models.Grid.Align.left, true);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = contratoFreteTerceiros.Carga.CargaCTes.Where(o => o.CIOTs.Count() > 0).First().CIOTs.First().CIOT;

                grid.setarQuantidadeTotal(1);

                var lista = new List<dynamic>()
                {
                    new
                    {
                        ciot.Codigo,
                        ciot.Numero,
                        ciot.CodigoVerificador,
                        Transportador = ciot.Transportador.Nome,
                        ciot.DescricaoSituacao,
                        ciot.Mensagem
                    }
                };

                grid.AdicionaRows(lista);

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

        #endregion

        #region Métodos Privados

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private bool GerarMovimentacaoFinanceiraJustificativas(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeTrabalho, out string erro)
        {
            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> justificativas = repContratoFreteValor.BuscarPorContratoFrete(contratoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor justificativa in justificativas)
            {
                if (!svcMovimentoFinanceiro.GerarMovimentacao(out erro, justificativa.TipoMovimentoUso, contratoFrete.DataEmissaoContrato, justificativa.Valor, contratoFrete.NumeroContrato.ToString(), "Referente ao valor justificado no contrato de frete nº " + contratoFrete.NumeroContrato + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, TipoServicoMultisoftware))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete()
            {
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                Carga = Request.GetStringParam("Carga"),
                TransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro"),
                SituacaoContrato = Request.GetListEnumParam<SituacaoContratoFrete>("SituacaoContratoFrete"),
                Bloqueado = Request.GetNullableBoolParam("SituacaoBloqueio"),
                NumeroCIOT = Request.GetStringParam("NumeroCIOT"),
                DataInicialContratoFrete = Request.GetDateTimeParam("DataInicialContratoFrete"),
                DataFinalContratoFrete = Request.GetDateTimeParam("DataFinalContratoFrete")
            };
        }

        private bool VerificarRegrasAutorizacao(ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = Servicos.Embarcador.Terceiros.ContratoFrete.VerificarRegrasAutorizacao(contrato, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CriarRegrasAutorizacao(listaFiltrada, contrato, contrato.Carga.Operador, TipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
                    contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
                else
                    contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;

                return true;
            }
            else
                contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra;

            return false;
        }

        private string ObterCIOTContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            return cargaCIOT?.CIOT?.Numero ?? "";
        }

        #endregion
    }
}
