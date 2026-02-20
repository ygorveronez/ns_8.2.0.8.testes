using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/Malote")]
    public class MaloteCanhotoController : BaseController
    {
        #region Construtores

        public MaloteCanhotoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.Malote repositorioMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioMalote.BuscarCanhotos(codigo);

                return new JsonpResult(new
                {
                    Imagens = (
                        from canhoto in canhotos
                        select new
                        {
                            canhoto.Codigo,
                            canhoto.Numero,
                            Miniatura = !canhoto.IsPDF() ? srvCanhoto.ObterMiniatura(canhoto, unitOfWork) : null,
                            ArquivoPDF = canhoto.IsPDF(),
                            Rejeitado = canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada
                        }
                    ).ToList()
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
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> PesquisaCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisaCanhotos();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = 0;

                dynamic lista = ExecutaPesquisaCanhotos(ref totalRegistros, parametrosConsulta, unitOfWork);

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = repMalote.BuscarPorCodigo(codigo);

                // Valida
                if (malote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (malote.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Cancelado)
                    return new JsonpResult(false, true, "Não é possível imrpimir protocolo de um malote nessa situação.");


                byte[] pdf = null;
                if (!configuracaoGeral.PermitirCriacaoDiretaMalotes)
                    pdf = Servicos.Embarcador.Canhotos.Malote.GerarImpressaoProtocolo(malote, unitOfWork);
                else
                    pdf = Servicos.Embarcador.Canhotos.Malote.GerarImpressaoProtocoloEntrega(malote, unitOfWork);

                if (pdf == null)
                    return new JsonpResult(true, false, "Falha ao gerar PDF.");

                return Arquivo(pdf, "application/pdf", "Malote - " + malote.Protocolo.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar PDF.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = repMalote.BuscarPorCodigo(codigo);

                // Valida
                if (malote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    malote.Codigo,
                    malote.Situacao,
                    Quantidade = malote.QuantidadeCanhotos,
                    malote.Protocolo,
                    Data = malote.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                    Operador = malote.NomeOperador,
                    Origem = malote.Empresa.Descricao,
                    Destino = malote.Destino.Descricao,
                    Filial = malote.Filial.Descricao,
                    TipoOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                    tpOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                    Motivo = malote.MotivoInconsistencia ?? string.Empty
                };

                // Retorna informacoes
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

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = repMalote.BuscarPorCodigo(codigo, true);

                // Valida
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                if (malote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (malote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Gerado)
                    return new JsonpResult(false, true, "Não é possível cancelar o malote na atual situação do malote.");

                malote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Cancelado;

                // Persiste dados
                unitOfWork.Start();
                repCanhoto.RemoverMaloteCanhotos(malote.Codigo);
                repMalote.Atualizar(malote, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarMalote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = repMalote.BuscarPorCodigo(codigo, true);

                // Valida
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                if (malote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (malote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Gerado)
                    return new JsonpResult(false, true, "Não é possível cancelar o malote na atual situação do malote.");

                malote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Confirmado;

                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = null;

                if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial)
                    localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtualFilial(malote.Filial.Codigo);
                else
                    localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();

                if (localArmazenamentoCanhoto == null)
                    return new JsonpResult(false, true, $"Local de armazenamento {(ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial ? "da filial " : "")}não encontrado.");

                if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial && localArmazenamentoCanhoto.CapacidadeArmazenagem <= (localArmazenamentoCanhoto.QuantidadeArmazenada + malote.Canhotos.Count()))
                    return new JsonpResult(false, true, "Não há espaço suficiente no local de armazenamento da filial.");

                // Persiste dados
                unitOfWork.Start();

                localArmazenamentoCanhoto.QuantidadeArmazenada += malote.Canhotos.Count();
                repMalote.SetarCanhotosRecebidos(malote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente, localArmazenamentoCanhoto.Codigo, DateTime.Now);

                if (ConfiguracaoEmbarcador.GerarPagamentoBloqueado)
                {
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPendetesLiberacaoFisicaPorMalote(malote.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Cliente);
                }

                repMalote.Atualizar(malote, Auditado);
                repLocalArmazenamentoCanhoto.Atualizar(localArmazenamentoCanhoto);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InconsistenciaMalote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = repMalote.BuscarPorCodigo(codigo, true);

                // Valida
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                if (malote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (malote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Gerado)
                    return new JsonpResult(false, true, "Não é possível cancelar o malote na atual situação do malote.");

                malote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Inconsistente;
                malote.MotivoInconsistencia = Request.GetStringParam("Motivo");

                // Persiste dados
                unitOfWork.Start();
                repMalote.Atualizar(malote, Auditado);
                repCanhoto.RemoverCanhotosMalote(malote.Codigo);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarMaloteDireto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = new Dominio.Entidades.Embarcador.Canhotos.Malote();


                int transportador = Request.GetIntParam("Transportador");
                int filial = Request.GetIntParam("FilialCadastro");
                int tipoOperacao = Request.GetIntParam("TipoOperacao");

                if (transportador > 0 && filial > 0)
                    malote = repMalote.BuscarMalotePorTransportadorFilialOuTipoOperacao(transportador, filial, tipoOperacao);

                if (malote != null)
                    return new JsonpResult(true, "Já existe um malote compatível com os dados informados no status Gerado (aberto) com o protocolo: " + malote.Protocolo);
                else
                    malote = new Dominio.Entidades.Embarcador.Canhotos.Malote();

                // Preenche entidade com dados
                PreencheEntidade(ref malote, unitOfWork);

                // Persiste dados
                unitOfWork.Start();
                repMalote.Inserir(malote, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    malote.Codigo,
                    malote.Situacao,
                    Quantidade = malote.QuantidadeCanhotos,
                    malote.Protocolo,
                    Data = malote.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                    Operador = malote.Operador?.Descricao ?? malote.NomeOperador,
                    Origem = malote.Empresa.Descricao,
                    Destino = malote.Destino.Descricao,
                    Filial = malote.Filial.Descricao,
                    TipoOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                    tpOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                    Motivo = malote.MotivoInconsistencia ?? string.Empty
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BiparCanhotoMaloteDireto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                string dadosBaixa = (Request.Params("DadosCanhoto") ?? string.Empty).Replace(" ", "");
                int codMalote = Request.GetIntParam("Codigo");
                int transportador = Request.GetIntParam("Transportador");
                int filial = Request.GetIntParam("FilialCadastro");
                int tipoOperacao = Request.GetIntParam("TipoOperacao");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = null;

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

                Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

                string[] splitDados = dadosBaixa.Split('_');

                if (splitDados.Length > 1 && splitDados[0] == "AV") //QR Code do canhoto avulso
                {
                    canhoto = repCargaCanhoto.BuscarPorQRCodeAvulso(dadosBaixa);
                }
                else
                {
                    string chaveDocumento = Utilidades.String.OnlyNumbers(dadosBaixa);

                    bool chave = chaveDocumento.Length == 44;

                    if ((chave && !Utilidades.Validate.ValidarChave(chaveDocumento)) || (!chave && chaveDocumento.Length != 20))
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ChaveDoDocumentoInvalido);

                    if (chave)
                    {
                        string modelo = chaveDocumento.Substring(20, 2);

                        if (modelo == "57")
                        {
                            canhoto = repCargaCanhoto.BuscarPorChaveCTeSubcontratacao(dadosBaixa);

                            if (canhoto == null)
                                canhoto = repCargaCanhoto.BuscarPorChaveCTe(dadosBaixa);
                        }
                        else if (modelo == "55")
                        {
                            canhoto = repCargaCanhoto.BuscarPorChave(dadosBaixa);
                        }
                    }
                    else
                    {
                        int.TryParse(chaveDocumento.Substring(0, 10), out int codigoCarga);
                        int.TryParse(chaveDocumento.Substring(10, 10), out int codigoCTe);

                        canhoto = repCargaCanhoto.BuscarPorCTeCargaCTe(codigoCTe);
                    }
                }

                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.CanhotoInformadoNaoFoiLocalizado);

                if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.EsteCanhotoJaFoiBaixado);

                if (configuracaoCanhoto.NaoPermitirReceberCanhotosNaoDigitalizados && canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                    return new JsonpResult(false, true, "Imagem do canhoto precisa estar digitalizada ");

                unitOfWork.Start();

                if (codMalote > 0)
                    malote = repMalote.BuscarPorCodigo(codMalote);

                if (malote == null)
                {
                    //buscar malote em aberto para transportador, filial
                    if (transportador <= 0)
                        transportador = canhoto.Empresa?.Codigo ?? 0;

                    if (filial <= 0)
                        filial = canhoto.Filial?.Codigo ?? 0;

                    malote = repMalote.BuscarMalotePorTransportadorFilialOuTipoOperacao(transportador, filial, tipoOperacao);

                    if (malote == null)
                    {
                        malote = new Dominio.Entidades.Embarcador.Canhotos.Malote(); //criar novo malote..
                        // Preenche entidade com dados
                        PreencheEntidade(ref malote, unitOfWork, canhoto);
                        repMalote.Inserir(malote, Auditado);
                    }
                }

                if (SalvarValidarCanhotosMalote(malote, canhoto, unitOfWork))
                {
                    unitOfWork.CommitChanges();

                    var retorno = new
                    {
                        malote.Codigo,
                        malote.Situacao,
                        Quantidade = malote.QuantidadeCanhotos,
                        malote.Protocolo,
                        Data = malote.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                        Operador = malote.Operador?.Descricao ?? malote.NomeOperador,
                        Origem = malote.Empresa.Descricao,
                        Destino = malote.Destino.Descricao,
                        Filial = malote.Filial.Descricao,
                        TipoOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                        tpOperacao = malote.TipoOperacao?.Descricao ?? string.Empty,
                        Motivo = malote.MotivoInconsistencia ?? string.Empty
                    };

                    // Retorna informacoes
                    return new JsonpResult(retorno);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção, o Transportador, Filial ou Tipo Operação do canhoto não são iguais ao Malote");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao");
            grid.Prop("Situacao");
            grid.Prop("Protocolo").Nome("Protocolo").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("DataMalote").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("QuantidadeCanhotos").Nome("Qtd").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("Operador").Nome("Operador").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Filial").Nome("Filial").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Origem").Nome("Origem").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Destino").Nome("Destino").Tamanho(20).Align(Models.Grid.Align.left);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.Prop("Empresa").Nome("Transportador").Tamanho(20).Align(Models.Grid.Align.left);
            else
                grid.Prop("Empresa");
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaCanhotos()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data de Emissao", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Chave da NF-e", "Chave", 20, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Viagem", "NumeroCarga", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Valor NF-e", "Valor", 10, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 18, Models.Grid.Align.left, false);
            }
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Nota", "DataNotaFiscal", 15, Models.Grid.Align.left, false);
            }
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Digitalização", "DescricaoDigitalizacao", 10, Models.Grid.Align.center, true);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Data Baixa", "DataEnvioCanhoto", 10, Models.Grid.Align.center, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Dados do filtro
            int protocolo = Request.GetIntParam("Protocolo");
            int numeroCanhoto = Request.GetIntParam("NumeroCanhoto");
            int destino = Request.GetIntParam("Destino");
            int empresa = Request.GetIntParam("Transportador");
            int filial = Request.GetIntParam("Filial");
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                empresa = this.Empresa.Codigo;

            double emitente = Request.GetDoubleParam("Emitente");

            int codigoOperador = Request.GetIntParam("Operador");
            Dominio.Entidades.Usuario operador = repUsuario.BuscarPorCodigo(codigoOperador);
            string nomeOperador = operador?.Nome ?? string.Empty;

            DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
            DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto>("Situacao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Canhotos.Malote> listaGrid = repMalote.Consultar(empresa, protocolo, numeroCanhoto, emitente, destino, codigoOperador, nomeOperador, filial, dataInicial, dataFinal, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMalote.ContarConsulta(empresa, protocolo, numeroCanhoto, emitente, destino, codigoOperador, nomeOperador, filial, dataInicial, dataFinal, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Descricao = obj.Protocolo,
                            obj.Protocolo,
                            obj.QuantidadeCanhotos,
                            Empresa = obj.Empresa.Descricao,
                            DataMalote = obj.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                            Operador = obj.Operador?.Descricao ?? obj.NomeOperador,
                            Filial = obj.Filial.Descricao,
                            Origem = obj.Empresa.Descricao,
                            Destino = obj.Destino.Descricao,
                            obj.DescricaoSituacao,
                            obj.Situacao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaCanhotos(ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoEntregaQualidade = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = repositorioConfiguracaoEntregaQualidade.BuscarConfiguracaoPadraoAsync().Result;

            int malote = Request.GetIntParam("Codigo");

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                Malote = malote,
                BaixarCanhotoAposAprovacaoDigitalizacao = this.ConfiguracaoEmbarcador?.BaixarCanhotoAposAprovacaoDigitalizacao ?? false
            };

            totalRegistros = repCanhoto.ContarConsultaCanhotoSQLDynamic(filtrosPesquisa, configuracaoQualidadeEntrega);
            return totalRegistros > 0 ? repCanhoto.ConsultarCanhotoSQLDynamic(filtrosPesquisa, parametrosConsulta, false, configuracaoQualidadeEntrega) : new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos>();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Operador") propOrdenar = "Operador.Nome";
            else if (propOrdenar == "Origem") propOrdenar = "Empresa.RazaoSocial";
            else if (propOrdenar == "Destino") propOrdenar = "Destino.Descricao";
            else if (propOrdenar == "Empresa") propOrdenar = "Empresa.RazaoSocial";
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Canhotos.Malote malote, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null)
        {
            // Instancia Repositorios
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

            // Vincula dados
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                malote.Empresa = this.Empresa;
            else
                malote.Empresa = canhoto == null ? repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportador")) : canhoto.Empresa;
            malote.Protocolo = repMalote.BuscarProximoProtocolo();
            malote.Data = DateTime.Now;
            malote.DataEnvio = DateTime.Now;
            malote.Operador = this.Usuario;
            malote.NomeOperador = this.Usuario.Nome;
            malote.Filial = canhoto == null ? repFilial.BuscarPorCodigo(Request.GetIntParam("FilialCadastro")) : canhoto.Filial;
            malote.Destino = canhoto == null ? repFilial.BuscarPorCodigo(Request.GetIntParam("FilialCadastro")) : canhoto.Filial;
            malote.TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
            malote.Situacao = SituacaoMaloteCanhoto.Gerado;
            malote.QuantidadeCanhotos = 0;
        }

        private bool SalvarValidarCanhotosMalote(Dominio.Entidades.Embarcador.Canhotos.Malote malote, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.MaloteCanhoto repMaloteCanhoto = new Repositorio.Embarcador.Canhotos.MaloteCanhoto(unitOfWork);

            if (malote.Empresa.Codigo != canhoto.Empresa.Codigo)
                return false;

            if (malote.Filial.Codigo != canhoto.Filial.Codigo)
                return false;


            var count = malote.Canhotos?.Count() ?? 0;

            Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto maloteCanhoto = new Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto()
            {
                Canhoto = canhoto,
                Malote = malote
            };

            repMaloteCanhoto.Inserir(maloteCanhoto);

            maloteCanhoto.Canhoto.Malote = malote;
            repCanhoto.Atualizar(maloteCanhoto.Canhoto);

            malote.QuantidadeCanhotos = count + 1;
            repMalote.Atualizar(malote);

            return true;
        }

        #endregion

    }
}
