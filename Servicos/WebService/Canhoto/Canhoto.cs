
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.Canhoto
{
    public class Canhoto : ServicoWebServiceBase
    {
        #region Variaveis Privadas

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        private readonly protected string _adminStringConexao;

        #endregion

        #region Constructores

        public Canhoto(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        public Canhoto(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao) : base(unitOfWork)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Metodos Publicos

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(RequestPaginacao dadosRequisicao, Dominio.Entidades.WebService.Integradora integradora)
        {
            Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>(),
                NumeroTotalDeRegistro = 0
            };

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfigCanhoto = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfigCanhoto.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoEntregaQualidade = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = repositorioConfiguracaoEntregaQualidade.BuscarConfiguracaoPadraoAsync().Result;

            if (dadosRequisicao.Limite >= 100)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100");

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(configuracao.UtilizaPgtoCanhoto, configuracao.RetornarCanhotosViaIntegracaoEmQualquerSituacao, dadosRequisicao.Inicio, dadosRequisicao.Limite, integradora.Empresa?.Codigo ?? 0, configuracaoQualidadeEntrega, configuracaoCanhoto.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                byte[] bufferCanhoto = null;
                if (!configuracaoWebService.NaoRetornarImagemCanhoto && Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                //if(bufferCanhoto == null || canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao)
                //    return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoDadosInvalidos("Canhoto ainda não digitalizado!");

                Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoNF = new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto()
                {
                    Protocolo = canhoto.Codigo,
                    ChaveAcesso = canhoto.XMLNotaFiscal?.Chave ?? "",
                    DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss"),
                    SituacaoCanhoto = canhoto.SituacaoCanhoto,
                    SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                    DataDigitalizacao = canhoto.DataDigitalizacao.HasValue ? canhoto.DataDigitalizacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    Observacao = canhoto.Observacao,
                    ImagemCanhotoBase64 = bufferCanhoto != null ? Convert.ToBase64String(bufferCanhoto) : "",
                    NomeImagemCanhoto = canhoto?.NomeArquivo ?? "",
                    TipoCanhoto = canhoto.TipoCanhoto,
                    Latitude = !string.IsNullOrEmpty(canhoto.Latitude) ? canhoto.Latitude : "",
                    Longitude = !string.IsNullOrEmpty(canhoto.Longitude) ? canhoto.Longitude : "",
                    Transportador = canhoto.Empresa?.CNPJ ?? "",
                    ChaveAcessoCte = canhoto.ChaveCTe?.Split(',')?.FirstOrDefault() ?? "",
                    NumeroCanhotoAvulso = canhoto.CanhotoAvulso?.Numero ?? 0,
                    NumeroNotaFiscal = canhoto.XMLNotaFiscal?.Numero.ToString() ?? "",
                    SituacaoNotaFiscal = canhoto.XMLNotaFiscal?.SituacaoEntregaNotaFiscal.ObterDescricao(),
                };
                retorno.Itens.Add(canhotoNF);
            }
            retorno.NumeroTotalDeRegistro = repCanhoto.ContarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(configuracao.UtilizaPgtoCanhoto, configuracao.RetornarCanhotosViaIntegracaoEmQualquerSituacao, integradora.Empresa?.Codigo ?? 0, configuracaoQualidadeEntrega, configuracaoCanhoto.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados);
            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou canhotos das notas pendentes de integração da digitalizacao", _unitOfWork);

            return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosDigitalizadoseAgAprovacao(RequestPaginacao dadosRequisicao, Dominio.Entidades.WebService.Integradora integradora)
        {
            Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>(),
                NumeroTotalDeRegistro = 0
            };

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (dadosRequisicao.Limite > 100)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100");

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarCanhotosDigitalizadoseAgAprovacao(configuracao.UtilizaPgtoCanhoto, dadosRequisicao.Inicio, dadosRequisicao.Limite, integradora.Empresa?.Codigo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.BuscarExistePorCarga(canhoto.Carga, _unitOfWork) ||
                    !Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NaoPossuiEntregasPendentes(canhoto.Carga, _unitOfWork))
                    continue;

                string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                byte[] bufferCanhoto = null;
                if (!configuracaoWebService.NaoRetornarImagemCanhoto && Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoNF = new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto()
                {
                    Protocolo = canhoto.Codigo,
                    ChaveAcesso = canhoto.XMLNotaFiscal.Chave,
                    DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss"),
                    SituacaoCanhoto = canhoto.SituacaoCanhoto,
                    SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                    Observacao = canhoto.Observacao,
                    ImagemCanhotoBase64 = bufferCanhoto != null ? Convert.ToBase64String(bufferCanhoto) : "",
                    NomeImagemCanhoto = canhoto.NomeArquivo,
                    TipoCanhoto = canhoto.TipoCanhoto,
                    Latitude = !string.IsNullOrEmpty(canhoto.Latitude) ? canhoto.Latitude : "",
                    Longitude = !string.IsNullOrEmpty(canhoto.Longitude) ? canhoto.Longitude : "",
                    NumeroCanhoto = canhoto.Numero
                };
                retorno.Itens.Add(canhotoNF);
            }
            retorno.NumeroTotalDeRegistro = repCanhoto.ContarCanhotosDigitalizadoseAgAprovacao(configuracao.UtilizaPgtoCanhoto, integradora.Empresa?.Codigo ?? 0);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou canhotos das notas pendentes de integração da digitalizacao (Ag.Digitalização e Digitalizados)", _unitOfWork);

            return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(List<int> protocolos)
        {
            return ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(protocolos, out _);
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>> ConfirmarIntegracoesDigitalizacaoCanhotosNotasFiscais(List<int> protocolos)
        {
            ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(protocolos, out List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto> retorno);
            return Retorno<List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<bool> RetornoCanhotoEntrega(Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoCanhotoEntrega retornoCanhotoEntrega)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).BuscarPorCodigoCargaEmbarcador(retornoCanhotoEntrega.numeroCarga);
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in carga.CargaCTes)
            {
                Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracaoCTe = repCTeCanhotoIntegracao.BuscarIntegracaoAguardandoRetornoPorCTe(cargaCTe.CTe.Codigo);
                if (integracaoCTe != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Stage stageCTe = Servicos.Embarcador.Pedido.Stage.BuscarStagePorCargaCte(cargaCTe.Codigo, _unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoCanhotoEntregaStages retornoStage = retornoCanhotoEntrega.stage.Find(o => o.numeroStage == stageCTe.NumeroStage);
                    if (retornoStage != null)
                    {
                        if (retornoStage.CodigoMensagem == "001")
                            integracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        else
                            integracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        integracaoCTe.ProblemaIntegracao = retornoStage?.descricaoMensagem ?? string.Empty;

                        servicoArquivoTransacao.Adicionar(integracaoCTe, JsonConvert.SerializeObject(retornoCanhotoEntrega), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");
                        repCTeCanhotoIntegracao.Atualizar(integracaoCTe);
                    }
                }
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public async Task<Retorno<int>> EnviarDigitalizacaoCanhotoAsync(Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoIntegracao, Dominio.Entidades.WebService.Integradora integradora, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, cancellationToken);
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork, cancellationToken);
            Servicos.Embarcador.Canhotos.CanhotoIntegracao servicoCanhotoIntegracao = new Servicos.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork, cancellationToken);

            try
            {
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;

                if (canhotoIntegracao.Protocolo > 0)
                    canhoto = await repositorioCanhoto.BuscarPorCodigoAsync(canhotoIntegracao.Protocolo);

                if (canhoto == null && !string.IsNullOrWhiteSpace(canhotoIntegracao.ChaveAcesso))
                    canhoto = await repositorioCanhoto.BuscarPorChaveAsync(canhotoIntegracao.ChaveAcesso);

                if (canhoto == null && !string.IsNullOrWhiteSpace(canhotoIntegracao.NumeroNotaFiscal) && !string.IsNullOrWhiteSpace(canhotoIntegracao.SerieNotaFiscal) && !string.IsNullOrWhiteSpace(canhotoIntegracao.CnpjEmitenteNotaFiscal))
                    canhoto = await repositorioCanhoto.BuscarPorNumeroSerieEmitenteAsync(canhotoIntegracao.NumeroNotaFiscal.ToInt(), canhotoIntegracao.SerieNotaFiscal, canhotoIntegracao.CnpjEmitenteNotaFiscal.ToDouble());

                if (canhoto == null)
                    throw new ServicoException("Não foi localizado um canhoto compativel com os dados informados.");

                canhoto.Initialize();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                await _unitOfWork.StartAsync(cancellationToken);

                if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto)
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                    canhoto.DigitalizacaoIntegrada = false;
                    canhoto.DataDigitalizacao = DateTime.Now;
                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Integracao;

                    await servicoCanhoto.CanhotoLiberadoAsync(canhoto, configuracao, _tipoServicoMultisoftware, _clienteMultisoftware);
                    await servicoCanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhotoAsync(canhoto, configuracao, _tipoServicoMultisoftware, _clienteMultisoftware);
                    await servicoCanhoto.FinalizarDigitalizacaoCanhotoAsync(canhoto, _tipoServicoMultisoftware);
                }
                else
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                    await servicoCanhoto.CanhotoAgAprovacaoAsync(canhoto, configuracao);
                }

                DateTime dataEnvioCanhoto = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(canhotoIntegracao.DataEnvioCanhoto) && !DateTime.TryParseExact(canhotoIntegracao.DataEnvioCanhoto, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEnvioCanhoto))
                    throw new ServicoException("A data de ultima verificação não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                canhoto.DataUltimaModificacao = dataEnvioCanhoto;

                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;

                if (!string.IsNullOrWhiteSpace(canhotoIntegracao.ImagemCanhotoBase64))
                {
                    canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    canhoto.NomeArquivo = canhotoIntegracao.NomeImagemCanhoto;
                    canhoto.MotivoRejeicaoDigitalizacao = string.Empty;
                    canhoto.Latitude = canhotoIntegracao.Latitude;
                    canhoto.Longitude = canhotoIntegracao.Longitude;

                    string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
                    string extensao = System.IO.Path.GetExtension(canhotoIntegracao.NomeImagemCanhoto).ToLower();
                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);

                    await SalvarImagemCanhotoAsync(fileLocation, canhotoIntegracao, canhoto);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        Servicos.Log.TratarErro($"Erro01 - Falha ao salvar imagem do Canhoto {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}.", "EnviarCanhoto");
                        throw new ServicoException("Erro01 - Falha ao salvar imagem do Canhoto.");
                    }
                }

                await repositorioCanhoto.AtualizarAsync(canhoto, _auditado);
                await servicoCanhoto.GerarHistoricoCanhotoAsync(canhoto, null, "Imagem do canhoto digitalizada integração por " + integradora?.Descricao ?? "Integração Assíncrona" + ".");

                await _unitOfWork.CommitChangesAsync(cancellationToken);

                return Retorno<int>.CriarRetornoSucesso(canhoto.Codigo);
            }
            catch (ServicoException ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Retorno<int>.CriarRetornoDadosInvalidos(ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a digitalização do canhoto.");
            }
        }

        #endregion

        #region Metodos Privados

        private Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(List<int> protocolos, out List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto> retornoLista)
        {
            retornoLista = new List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>();
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            foreach (int protocolosCanhoto in protocolos)
            {
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(protocolosCanhoto, true);

                if (canhoto == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi encontrado um canhoto para o protocolo informado");

                if (canhoto.DigitalizacaoIntegrada)
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    if (!configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                        return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("A confirmação da integração já foi realizada anteriormente.");

                    continue;
                }

                canhoto.DigitalizacaoIntegrada = true;
                repCanhoto.Atualizar(canhoto);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, canhoto, "Confirmou integração da digitalização.", _unitOfWork);

                retornoLista.Add(new Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto()
                {
                    Protocolo = canhoto.Codigo,
                    SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto
                });
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        private async Task SalvarImagemCanhotoAsync(string fileLocation, Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoIntegracao, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {

            byte[] data = System.Convert.FromBase64String(canhotoIntegracao.ImagemCanhotoBase64);

            using MemoryStream ms = new MemoryStream(data);
            using (Image canhotofile = Image.FromStream(ms))
            using (Bitmap canhotoImagem = new Bitmap(canhotofile))
                await Utilidades.IO.FileStorageService.Storage.SaveImageAsync(fileLocation, canhotoImagem);
        }

        #endregion
    }
}
