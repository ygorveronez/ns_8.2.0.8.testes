using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Servicos.Embarcador.Canhotos
{
    public class LeitorOCR : ServicoBase
    {
        #region Atributos Privados

        private int tempoThread = 1000;
        private string caminhoArquivos = Servicos.FS.GetPath(@"C:\Arquivos\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"C:\Arquivos");
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private string AdminStringConexao = "";
        private string CaminhoBatReiniciar = "";
        private string APILink = "";
        private string APIKey = "";

        #endregion

        #region Construtores       
        public LeitorOCR() : base() { }
        public LeitorOCR(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto AdicionarControleLeituraImagemCanhotoAguardandoImagem(Repositorio.UnitOfWork unitOfWork, string nomeCompletoArquivo, string nomeArquivoSalvar, Dominio.Entidades.Usuario usuario)
        {
            return AdicionarOuAtualizarControleLeituraImagemCanhoto(unitOfWork, nomeCompletoArquivo, nomeArquivoSalvar, usuario, SituacaoleituraImagemCanhoto.AgImagem, buscarControleLeituraImagemCanhotoAguardandoImagem: false);
        }

        private Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto AdicionarControleLeituraImagemCanhotoAguardandoProcessamento(Repositorio.UnitOfWork unitOfWork, string nomeCompletoArquivo, string nomeArquivoSalvar, Dominio.Entidades.Usuario usuario)
        {
            return AdicionarOuAtualizarControleLeituraImagemCanhoto(unitOfWork, nomeCompletoArquivo, nomeArquivoSalvar, usuario, SituacaoleituraImagemCanhoto.AgProcessamento, buscarControleLeituraImagemCanhotoAguardandoImagem: false);
        }

        private Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto AdicionarOuAtualizarControleLeituraImagemCanhoto(Repositorio.UnitOfWork unitOfWork, string nomeCompletoArquivo, string nomeArquivoSalvar, Dominio.Entidades.Usuario usuario, SituacaoleituraImagemCanhoto situacaoleituraImagemCanhoto, bool buscarControleLeituraImagemCanhotoAguardandoImagem)
        {
            string nomeArquivo = Path.GetFileName(nomeCompletoArquivo);
            string nomeArquivoSemIdentificadorImagemCanhoto = ObterNomeArquivoSemIdentificadorImagemCanhoto(nomeCompletoArquivo);
            string identificadorImagemCanhoto = ObterIdentificadorImagemCanhoto(nomeCompletoArquivo);
            string parametrosImagemCanhoto = ObterParametrosNomeArquivoImagemCanhoto(nomeCompletoArquivo);
            string guidArquivo = $"{identificadorImagemCanhoto}{nomeArquivoSemIdentificadorImagemCanhoto}{parametrosImagemCanhoto}{Path.GetExtension(nomeCompletoArquivo)}";
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = null;

            if ((situacaoleituraImagemCanhoto == SituacaoleituraImagemCanhoto.AgProcessamento) && identificadorImagemCanhoto.Contains("_A_") && buscarControleLeituraImagemCanhotoAguardandoImagem)
                controleLeituraImagemCanhoto = repositorioControleLeituraImagemCanhoto.BuscarAguardandoImagemPorGuidArquivo(nomeArquivo);

            if (controleLeituraImagemCanhoto == null)
            {
                controleLeituraImagemCanhoto = new Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto
                {
                    Data = DateTime.Now,
                    GuidArquivo = guidArquivo,
                    NumeroDocumento = "",
                    MensagemRetorno = "",
                    NomeArquivo = nomeArquivoSalvar,
                    SituacaoleituraImagemCanhoto = situacaoleituraImagemCanhoto,
                    Extensao = ExtensaoArquivoHelper.ObterExtensaoArquivo(nomeCompletoArquivo),
                    Usuario = usuario
                };

                repositorioControleLeituraImagemCanhoto.Inserir(controleLeituraImagemCanhoto);
            }
            else
            {
                controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.AgProcessamento;

                repositorioControleLeituraImagemCanhoto.Atualizar(controleLeituraImagemCanhoto);
            }

            return controleLeituraImagemCanhoto;
        }

        private void BuscarImagens(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                string pastaFTP = "imagem";
                string caminho = caminhoRaiz + @"\FTP\Enviados\Canhotos\";

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pastaFTP.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true, false, true);

                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);

                        if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                        {
                            Servicos.Log.TratarErro("Solicitou bat reinicia");
                            System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                        }

                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho);

                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);

                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    try
                    {
                        Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = ObterControleLeituraImagemCanhotoAguardandoProcessamento(unitOfWork, arquivo);
                        MoverParaPastaProcessados(controleLeituraImagemCanhoto, arquivo);
                        repositorioControleLeituraImagemCanhoto.Atualizar(controleLeituraImagemCanhoto);
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro($"Não foi possível interpretar o arquivo. {fileName} de contingencia da Natura", "LeitorOCR");
                        Log.TratarErro(excecao, "LeitorOCR");
                    }
                }
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        private void ExecutarThread()
        {
            Servicos.Log.TratarErro("Iniciou Task");
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(tempoThread);

                    using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        IntegrarCanhotoPendente(unidadeDeTrabalho);
                        BuscarImagens(unidadeDeTrabalho);
                    }
                }
                catch (System.ServiceModel.CommunicationException com)
                {
                    Servicos.Log.TratarErro("Comunication: " + com);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (TimeoutException ti)
                {
                    Servicos.Log.TratarErro("Time out: " + ti);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    System.Threading.Thread.Sleep(tempoThread);
                }
            }
        }

        private void IntegrarCanhoto(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string caminhoRaizCanhotosProcessados, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, bool integracaoAutomatica)
        {
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            bool canhotoUnilever = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
            string caminho = caminhoRaizCanhotosProcessados + @"\Canhotos\Processados\";
            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);
            string mensagemErro;
            Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr = ObterTextoOCR(caminhoCompleto, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.FalhaProcessamento;
                controleLeituraImagemCanhoto.MensagemRetorno = mensagemErro;
                MoverParaPastaCanhotosNaoReconhecidos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);
                return;
            }

            bool canhotoAvulso = false;
            int numeroNotaFiscal = ObterNumeroNotaFiscal(retornoOcr, canhotoUnilever, out canhotoAvulso, unitOfWork);

            Log.TratarErro($"Leitor OCR NotaFiscal identificada: {numeroNotaFiscal}", "LeitorOCR");

            List<int> numerosNotasFiscais = ObterNumerosNotasFiscais(retornoOcr.RetornoParse, canhotoAvulso);

            if ((numeroNotaFiscal <= 0) && (numerosNotasFiscais.Count == 0))
            {
                controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.ImagemNaoReconhecida;
                controleLeituraImagemCanhoto.MensagemRetorno = "Não foi possível interpretar o conteúdo da imagem como um canhoto válido.";
                MoverParaPastaCanhotosNaoReconhecidos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);
                return;
            }

            string[] splitArquivo = Path.GetFileNameWithoutExtension(controleLeituraImagemCanhoto.GuidArquivo).Split('_');
            string cnpjEmpresa = "";
            double emitente = 0;
            int grupoCliente = 0;
            DateTime dataEntrega = DateTime.MinValue;
            TipoLeituraImagemCanhoto tipo = TipoLeituraImagemCanhoto.Automatico;
            Dominio.Entidades.Empresa empresa = null;
            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;

            if (splitArquivo.Length > 2)
                tipo = TipoLeituraImagemCanhotoHelper.ObterTipo(splitArquivo[1]);

            //Manipula os demais parâmetros que vierem no nome do arquivo
            if (splitArquivo.Length >= 4)
            {
                for (int i = 3; i < splitArquivo.Length; i++)
                {
                    if (Regex.IsMatch(splitArquivo[i], @"DATA[0-9]{14}"))
                    {
                        string parametroData = splitArquivo[i].Replace("DATA", "");
                        if (DateTime.TryParseExact(parametroData, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                            dataEntrega = data;
                        else
                            dataEntrega = DateTime.MinValue;
                    }
                }
            }

            if ((tipo == TipoLeituraImagemCanhoto.Automatico) || (tipo == TipoLeituraImagemCanhoto.Mobile) || (tipo == TipoLeituraImagemCanhoto.CnpjEmitenteInformadoViaMobile))
                double.TryParse(splitArquivo[0], out emitente);
            else if (tipo == TipoLeituraImagemCanhoto.GrupoCliente)
                int.TryParse(splitArquivo[0], out grupoCliente);
            else
            {
                cnpjEmpresa = splitArquivo[0];
                empresa = repositorioEmpresa.BuscarPorCNPJ(cnpjEmpresa);
            }

            if (integracaoAutomatica || (emitente > 0) || (empresa != null) || (grupoCliente > 0) || canhotoAvulso)
            {
                if (numeroNotaFiscal > 0)
                    canhoto = repositorioCanhoto.BuscarCanhotoNFePorNumero(numeroNotaFiscal, emitente, empresa?.Codigo ?? 0, canhotoAvulso, grupoCliente);

                if (canhoto == null && numerosNotasFiscais.Count > 0)
                    canhoto = repositorioCanhoto.BuscarCanhotoNFePorNumeros(numerosNotasFiscais, emitente, empresa?.Codigo ?? 0, canhotoAvulso, grupoCliente);
            }

            if (canhoto != null)
            {
                if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                {
                    controleLeituraImagemCanhoto.NumeroDocumento = canhoto.Numero.ToString();
                    controleLeituraImagemCanhoto.Canhoto = canhoto;
                    controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.Descartada;
                    controleLeituraImagemCanhoto.MensagemRetorno = "Canhoto já vinculado anteriormente.";
                    MoverParaPastaCanhotos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);
                    return;
                }

                if (dataEntrega != DateTime.MinValue && dataEntrega < canhoto.DataEmissao)
                {
                    controleLeituraImagemCanhoto.NumeroDocumento = canhoto.Numero.ToString();
                    controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.SemCanhoto;
                    controleLeituraImagemCanhoto.MensagemRetorno = "A data de entrega não pode ser inferior a data de emissão da nota fiscal.";
                    MoverParaPastaCanhotosNaoReconhecidos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);

                    return;
                }

                if ((canhoto.Carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false) && servicoCanhoto.CanhotoPossuiCTeNaoAutorizado(canhoto))
                {
                    controleLeituraImagemCanhoto.NumeroDocumento = canhoto.Numero.ToString();
                    controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.Descartada;
                    controleLeituraImagemCanhoto.MensagemRetorno = "O CT-e do canhoto não está autorizado.";
                    MoverParaPastaCanhotosNaoReconhecidos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);

                    return;
                }
                unitOfWork.Start();

                if (dataEntrega != DateTime.MinValue)
                    canhoto.DataEntregaNotaCliente = dataEntrega;

                if (!integracaoAutomatica)
                    repositorioControleLeituraImagemCanhoto.Inserir(controleLeituraImagemCanhoto);

                canhoto.ValidacaoViaOCR = true;

                VincularCanhotoDaDigitalizacao(canhoto, caminhoCompleto, controleLeituraImagemCanhoto, tipo, unitOfWork, controleLeituraImagemCanhoto.Usuario, tipoServicoMultisoftware);

                unitOfWork.CommitChanges();
            }
            else
            {
                string numeroCanhotos = numeroNotaFiscal.ToString() + ", " + string.Join(", ", (from obj in numerosNotasFiscais select obj.ToString()).ToList());

                if (numeroCanhotos.Length > 150)
                    numeroCanhotos = numeroCanhotos.Substring(0, 149);

                controleLeituraImagemCanhoto.NumeroDocumento = numeroCanhotos;
                controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.SemCanhoto;
                controleLeituraImagemCanhoto.MensagemRetorno = "Nenhum canhoto localizado com essa numeração na base Multisoftware ";

                if (empresa != null)
                    controleLeituraImagemCanhoto.MensagemRetorno += $" para o transportador {empresa.Descricao}.";
                else if (grupoCliente > 0)
                    controleLeituraImagemCanhoto.MensagemRetorno += $" para o cliente {repositorioGrupoPessoas.BuscarPorCodigo(grupoCliente)?.Descricao ?? ""}.";
                else
                    controleLeituraImagemCanhoto.MensagemRetorno += $" para o emissor {emitente}.";

                MoverParaPastaCanhotosNaoReconhecidos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);
            }
        }

        public void IntegrarCanhotoPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> controlesLeituraImagemCanhoto = repositorioControleLeituraImagemCanhoto.BuscarAgProcessamento(0, 5);

            for (int i = 0; i < controlesLeituraImagemCanhoto.Count; i++)
            {

                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = controlesLeituraImagemCanhoto[i];

                try
                {
                    IntegrarCanhoto(controleLeituraImagemCanhoto, caminhoRaiz, unitOfWork, tipoServicoMultisoftware: null, integracaoAutomatica: true);

                    repositorioControleLeituraImagemCanhoto.Atualizar(controleLeituraImagemCanhoto);

                }
                catch (Exception excecao)
                {
                    if (unitOfWork.IsActiveTransaction())
                        unitOfWork.Rollback();

                    controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.FalhaProcessamento;
                    controleLeituraImagemCanhoto.MensagemRetorno = "Ocorreu um erro ao processar canhoto";
                    Log.TratarErro(excecao, "LeitorOCR");
                }
            }
        }

        private void MoverParaOutraPasta(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string nomeCompletoArquivo, string caminho)
        {
            string nomeCompletoArquivoDestino = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);

            try
            {
                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                    Utilidades.IO.FileStorageService.Storage.Move(nomeCompletoArquivo, nomeCompletoArquivoDestino);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivoDestino))
                {
                    Servicos.Log.TratarErro($"Erro04 - Falha ao mover imagem do canhoto, de {nomeCompletoArquivo} para {nomeCompletoArquivoDestino}.", "EnviarCanhoto");
                    throw new IOException("Erro04 - Falha ao mover imagem do canhoto.");
                }

            }
            catch (IOException ex)
            {
                controleLeituraImagemCanhoto.GuidArquivo = $"{ObterIdentificadorImagemCanhoto(controleLeituraImagemCanhoto.GuidArquivo)}{Guid.NewGuid().ToString().Replace("_", "")}.{Path.GetExtension(controleLeituraImagemCanhoto.GuidArquivo)}";

                string nomeCompletoArquivoDestinoRenomeado = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);

                Utilidades.IO.FileStorageService.Storage.Move(nomeCompletoArquivo, nomeCompletoArquivoDestinoRenomeado);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivoDestino))
                {
                    Servicos.Log.TratarErro($"Erro04 - Falha ao mover imagem do canhoto, de {nomeCompletoArquivo} para {nomeCompletoArquivoDestino}.", "EnviarCanhoto");
                    throw new Exception("Erro04 - Falha ao mover imagem do canhoto.");
                }

            }
        }

        private void MoverParaPastaCanhotos(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string nomeCompletoArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoCanhoto(controleLeituraImagemCanhoto, unitOfWork);

            MoverParaOutraPasta(controleLeituraImagemCanhoto, nomeCompletoArquivo, caminho);
        }

        private void MoverParaPastaCanhotosNaoReconhecidos(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string nomeCompletoArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoCanhotoNaoReconhecido(unitOfWork);

            MoverParaOutraPasta(controleLeituraImagemCanhoto, nomeCompletoArquivo, caminho);
        }

        private void MoverParaPastaProcessados(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string nomeCompletoArquivo)
        {
            string caminho = caminhoRaiz + @"\Canhotos\Processados\";

            MoverParaOutraPasta(controleLeituraImagemCanhoto, nomeCompletoArquivo, caminho);
        }

        private string ObterCaminhoCanhoto(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            if ((controleLeituraImagemCanhoto.Canhoto == null) || (controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto == SituacaoleituraImagemCanhoto.Descartada))
                return Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoCanhotos, "Descartados");

            if (controleLeituraImagemCanhoto.Canhoto.TipoCanhoto == TipoCanhoto.NFe)
                return Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoCanhotos, controleLeituraImagemCanhoto.Canhoto.Emitente.CPF_CNPJ_SemFormato);

            if (controleLeituraImagemCanhoto.Canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                return Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoCanhotos, "CanhotosAvulsos");

            throw new NotImplementedException("Não foram implementados outros modelos de canhotos");
        }

        private string ObterCaminhoCanhotoEnviados(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            return configuracaoArquivo.CaminhoCanhotos + @"\Integracao\FTP\Enviados\Canhotos\";
        }

        private string ObterCaminhoCanhotoNaoReconhecido(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            return configuracaoArquivo.CaminhoCanhotos + @"\NaoReconhecidos\";
        }

        private string ObterCaminhoCanhotoProcessados(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            return configuracaoArquivo.CaminhoRaiz + @"\Canhotos\Processados\";
        }

        private Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto ObterControleLeituraImagemCanhotoAguardandoProcessamento(Repositorio.UnitOfWork unitOfWork, string nomeCompletoArquivo)
        {
            string nomeArquivoSalvar = $"{ObterNomeArquivoSemIdentificadorImagemCanhoto(nomeCompletoArquivo)}{Path.GetExtension(nomeCompletoArquivo)}";

            return AdicionarOuAtualizarControleLeituraImagemCanhoto(unitOfWork, nomeCompletoArquivo, nomeArquivoSalvar, usuario: null, situacaoleituraImagemCanhoto: SituacaoleituraImagemCanhoto.AgProcessamento, buscarControleLeituraImagemCanhotoAguardandoImagem: true);
        }

        private string ObterIdentificadorImagemCanhoto(string nomeCompletoArquivo)
        {
            string nomeArquivo = Path.GetFileNameWithoutExtension(nomeCompletoArquivo);
            string[] splitNomeArquivo = nomeArquivo.Split('_');

            if (splitNomeArquivo.Length > 2)
                return $"{splitNomeArquivo[0]}_{splitNomeArquivo[1]}_";

            if (splitNomeArquivo.Length == 2)
                return $"{splitNomeArquivo[0]}_A_";

            return "";
        }

        private string ObterNomeArquivoSemIdentificadorImagemCanhoto(string nomeCompletoArquivo)
        {
            string nomeArquivo = Path.GetFileNameWithoutExtension(nomeCompletoArquivo);
            string[] splitNomeArquivo = nomeArquivo.Split('_');

            if (splitNomeArquivo.Length > 2)
            {
                StringBuilder nomeArquivoSalvar = new StringBuilder();

                for (int i = 2; i < splitNomeArquivo.Length; i++)
                {
                    if (!IsParametroNomeArquivoImagemCanhoto(splitNomeArquivo[i]))
                        nomeArquivoSalvar.Append(splitNomeArquivo[i]);
                }

                return nomeArquivoSalvar.ToString();
            }

            if (splitNomeArquivo.Length == 2)
                return splitNomeArquivo[1];

            return nomeArquivo;
        }

        private string ObterParametrosNomeArquivoImagemCanhoto(string nomeCompletoArquivo)
        {
            string nomeArquivo = Path.GetFileNameWithoutExtension(nomeCompletoArquivo);
            string[] splitNomeArquivo = nomeArquivo.Split('_');

            StringBuilder parametros = new StringBuilder();

            for (int i = 0; i < splitNomeArquivo.Length; i++)
            {
                if (IsParametroNomeArquivoImagemCanhoto(splitNomeArquivo[i]))
                    parametros.Append($"_{splitNomeArquivo[i]}");
            }

            return parametros.ToString();
        }

        private bool IsParametroNomeArquivoImagemCanhoto(string parametro)
        {
            if (Regex.IsMatch(parametro, "DATA[0-9]{14}"))
                return true;

            return false;
        }

        private int ObterNumeroNotaFiscal(Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr, bool canhotoUnilever, out bool canhotoAvulso, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork).BuscarConfiguracaoPadrao();

            if (configuracaoCanhoto.ObterNumeroNotaFiscalPorObjetoOcr ?? false)
            {
                return ObterNumeroNotaFiscalPorObjeto(retornoOcr, out canhotoAvulso);
            }

            return ObterNumeroNotaFiscalPorTexto(retornoOcr.RetornoParse, canhotoUnilever, out canhotoAvulso);
        }

        private int ObterNumeroNotaFiscalPorObjeto(Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr, out bool canhotoAvulso)
        {
            canhotoAvulso = false;
            bool linhaEncontrada = false;
            int numero = 0;
            string linhaProcuradaCanhotoComum = "NF-e";
            string linhaProcuradaCanhotoAvulso = "Canhoto Avulso";

            for (int i = 0; i < retornoOcr.ParsedResults.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Canhoto.ResultadoProcessado resultado = retornoOcr.ParsedResults[i];

                for (int j = 0; j < resultado.TextOverlay.Lines.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Canhoto.Linha linha = resultado.TextOverlay.Lines[j];

                    if (linha.LineText.Contains(linhaProcuradaCanhotoComum))
                    {
                        linhaEncontrada = true;
                        numero = resultado.TextOverlay.Lines[j + 1].LineText.ToInt();
                    }

                    if (linha.LineText.Contains(linhaProcuradaCanhotoAvulso))
                    {
                        canhotoAvulso = true;
                        linhaEncontrada = true;
                        numero = resultado.TextOverlay.Lines[j].LineText.ObterSomenteNumeros().ToInt();
                    }

                    if (linhaEncontrada && numero == 0 && !canhotoAvulso && resultado.TextOverlay.Lines.Count > j + 2)
                        numero = resultado.TextOverlay.Lines[j + 2].LineText.ToInt(); //Eventualmente o LeitorOCR Muda a Ordem em que o número da NF-e vem posicionado entre as LineText

                    if (linhaEncontrada)
                        break;
                }
            }

            return numero;
        }

        private int ObterNumeroNotaFiscalPorTexto(string texto, bool canhotoUnilever, out bool canhotoAvulso)
        {

            canhotoAvulso = false;
            //todo: aqui será necessário tratar cada modelo de canhoto, por hora está apenas o modelo da danone.
            string textoTratado = texto.ToLower();

            int indexCanhotoAvulso = textoTratado.IndexOf("broker");
            if (indexCanhotoAvulso != -1)
                canhotoAvulso = true;

            if (!canhotoAvulso)
                indexCanhotoAvulso = textoTratado.IndexOf("protocolo");

            if (indexCanhotoAvulso != -1)
                canhotoAvulso = true;

            if (indexCanhotoAvulso != -1)
                canhotoAvulso = true;

            int indexnumeroNF = 0;

            if (canhotoAvulso)
            {
                indexnumeroNF = textoTratado.IndexOf("viagem");
                string textoViagem = textoTratado.Substring(indexnumeroNF + 6);
                int indexViagem = textoViagem.IndexOf("viagem");
                if (indexViagem != -1)
                {
                    textoTratado = textoViagem;
                    indexnumeroNF = indexViagem;
                }
            }
            else
            {
                indexnumeroNF = textoTratado.IndexOf("nf-e");

                int indexSerie = textoTratado.IndexOf("série: ");
                int indexNFe = textoTratado.LastIndexOf("nf-e");
                if (canhotoUnilever && indexSerie != -1 && indexNFe != -1)
                {
                    string[] listaSplit = textoTratado.Substring(indexSerie, indexNFe - indexSerie).Split('\t');
                    string numeroCanhoto = listaSplit.Where(x => int.TryParse(x, out int result)).OrderByDescending(x => x).FirstOrDefault();
                    indexnumeroNF = textoTratado.IndexOf(numeroCanhoto);
                }

                if (indexnumeroNF == -1)
                    indexnumeroNF = textoTratado.IndexOf("nfe");

                if (indexnumeroNF == -1)
                    indexnumeroNF = textoTratado.IndexOf("-e");

                if (indexnumeroNF == -1)
                {
                    indexnumeroNF = textoTratado.IndexOf("no.");
                    int indexdanone = textoTratado.IndexOf("none");
                    if (indexdanone != -1)
                        textoTratado = textoTratado.Substring(indexdanone + 4);

                    int indexnota = textoTratado.IndexOf("nota");
                    if (indexnota != -1)
                        textoTratado = textoTratado.Substring(indexnota + 4);

                    indexnumeroNF = textoTratado.IndexOf("no");
                    if (indexnumeroNF == -1)
                        indexnumeroNF = textoTratado.IndexOf("nªº");

                }
            }

            string strNumero = "";
            if (indexnumeroNF >= 0)
            {
                textoTratado = textoTratado.Substring(indexnumeroNF);
                bool isNumero = false;

                for (int j = 0; j < textoTratado.Length; j++)
                {
                    char cha = textoTratado[j];

                    if (Char.IsNumber(cha))
                    {
                        isNumero = true;
                        strNumero += cha;
                    }
                    else
                    {
                        if (isNumero)
                        {
                            if (cha == ' ' && Char.IsNumber(textoTratado[j + 1]))
                                continue;

                            if ((cha != ',' && cha != '.') || ((textoTratado.Length == j + 1) || (!Char.IsNumber(texto[j + 1]) && texto[j + 1] != ' ')))
                                break;
                        }

                    }
                }
            }
            else
            {
                if (!canhotoAvulso)
                {
                    textoTratado = Utilidades.String.RemoveDiacritics(textoTratado);
                    indexnumeroNF = textoTratado.IndexOf("serie");

                    if (indexnumeroNF == -1)
                        indexnumeroNF = textoTratado.IndexOf("rie");

                    if (indexnumeroNF >= 0)
                    {
                        textoTratado = textoTratado.Substring(0, indexnumeroNF);

                        bool isNumero = false;
                        for (int j = textoTratado.Length - 1; j >= 0; j--)
                        {
                            char cha = textoTratado[j];
                            if (Char.IsNumber(cha))
                            {
                                isNumero = true;
                                strNumero = cha + strNumero;
                            }
                            else
                            {
                                if (isNumero)
                                    break;
                            }
                        }
                    }
                }
            }

            int numero = 0;
            int.TryParse(strNumero, out numero);
            return numero;
        }

        private List<int> ObterNumerosNotasFiscais(string texto, bool canhotoAvulso)
        {
            texto = texto.ToLower();
            if (canhotoAvulso)
            {
                int indexLimite = texto.IndexOf("route");
                if (indexLimite != -1)
                    texto = texto.Substring(0, indexLimite);
                else
                {
                    indexLimite = texto.IndexOf("cliente");
                    if (indexLimite != -1)
                        texto = texto.Substring(0, indexLimite);
                }
            }

            List<int> numeros = new List<int>();
            string strNumero = "";
            bool isNumero = false;
            for (int j = 0; j < texto.Length; j++)
            {
                char cha = texto[j];
                if (Char.IsNumber(cha))
                {
                    isNumero = true;
                    strNumero += cha;
                }
                else
                {
                    if (isNumero)
                    {
                        if (cha == ' ' && ((j + 1) < texto.Length) && Char.IsNumber(texto[j + 1]))
                            continue;

                        if ((cha != ',' && cha != '.') || ((texto.Length == j + 1) || (!Char.IsNumber(texto[j + 1]) && texto[j + 1] != ' ')))
                        {
                            int numero = 0;
                            int.TryParse(strNumero, out numero);
                            if (canhotoAvulso)
                            {
                                if (numero > 1000)
                                    numeros.Add(numero);
                            }
                            else
                            {
                                if (numero > 10)
                                    numeros.Add(numero);
                            }


                            strNumero = "";
                            isNumero = false;
                        }
                    }
                }
            }
            return numeros;
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr ObterTextoOCR(string caminhoImagem, out string mensagemErro)
        {
            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoImagem))
            {
                mensagemErro = "Não foi possível encontrar a imagem.";
                return new Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr();
            }

            byte[] imageData = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoImagem);
            bool arquivoPDF = Path.GetExtension(caminhoImagem).ToLower() == ".pdf";

            return ObterTextoOCR(imageData, arquivoPDF, out mensagemErro);
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr ObterTextoOCR(byte[] imageData, bool arquivoPDF, out string mensagemErro)
        {
            try
            {
                mensagemErro = "";

                HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(LeitorOCR)); 

                requisicao.Timeout = new TimeSpan(1, 1, 1);

                MultipartFormDataContent conteudoRequisicao = new MultipartFormDataContent
                {
                    { new StringContent(APIKey), "apikey" },
                    { new StringContent("por"), "language" },
                    { new StringContent("true"), "detectOrientation" },
                    { new StringContent("true"), "scale" },
                    { new StringContent("true"), "isTable" },
                    { new StringContent("5"), "OCREngine" },
                    //{ new StringContent("true"), "isOverlayRequired" },
                    { new ByteArrayContent(imageData, 0, imageData.Length), "image", $"image.{(arquivoPDF ? "pdf" : "jpg")}" }
                };

                if (arquivoPDF)
                    conteudoRequisicao.Add(new StringContent("PDF"), "filetype");

                Log.TratarErro($"Leitor OCR Url: {APILink} Key: {APIKey}", "LeitorOCR");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(APILink, conteudoRequisicao).Result;
                byte[] byteRetornoRequisicao = retornoRequisicao.Content.ReadAsByteArrayAsync().Result;
                string conteudoRetornoRequisicao = Encoding.UTF8.GetString(byteRetornoRequisicao);
                Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOCR = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr>(conteudoRetornoRequisicao);

                if (retornoOCR.OCRExitCode != 1)
                {
                    mensagemErro = retornoOCR.ParsedResults[0] != null && !string.IsNullOrEmpty(retornoOCR.ParsedResults[0].ErrorMessage) ? retornoOCR.ParsedResults[0].ErrorMessage : "Ocorreu um erro ao tentar fazer a leitura do OCR";
                    return new Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr();
                }

                StringBuilder retorno = new StringBuilder();

                for (int i = 0; i < retornoOCR.ParsedResults.Count; i++)
                    retorno.Append(retornoOCR.ParsedResults[i].ParsedText);

                retornoOCR.RetornoParse = retorno.ToString();

                Log.TratarErro($"Retorno OCR: {retornoOCR.RetornoParse}", "LeitorOCR");

                return retornoOCR;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "LeitorOCR");
                mensagemErro = "Ocorreu uma falha ao processar a imagem";
                return new Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr();
            }
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarArquivoNaPastaEnviados(Repositorio.UnitOfWork unitOfWork, CustomFile arquivo, string caminhoRaiz, string filial, Dominio.Entidades.Usuario usuario, string dataEntrega)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "FTP", "Enviados", "Canhotos");

            string key = "A";
            string nomeArquivo = $"{filial}_{key}_{Guid.NewGuid().ToString().Replace("_", "")}{(!string.IsNullOrWhiteSpace(dataEntrega) ? $"_DATA{dataEntrega}" : "")}{Path.GetExtension(arquivo.FileName)}";
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                throw new ServicoException($"Já existe um arquivo dentro da pasta \"{caminhoArquivos}\" com o mesmo nome");

            Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = AdicionarControleLeituraImagemCanhotoAguardandoImagem(unitOfWork, nomeCompletoArquivo, arquivo.FileName, usuario);

            try
            {
                arquivo.SaveAs(nomeCompletoArquivo);
            }
            catch (Exception)
            {
                new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork).Deletar(controleLeituraImagemCanhoto);
                throw;
            }
        }

        public void AdicionarArquivoNaPastaProcessados(Repositorio.UnitOfWork unitOfWork, CustomFile arquivo, string caminhoRaiz, string filial, Dominio.Entidades.Usuario usuario, string dataEntrega)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Canhotos", "Processados");

            string key = "A";
            string nomeArquivo = $"{filial}_{key}_{Guid.NewGuid().ToString().Replace("_", "")}{(!string.IsNullOrWhiteSpace(dataEntrega) ? $"_DATA{dataEntrega}" : "")}{Path.GetExtension(arquivo.FileName)}";
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                throw new ServicoException($"Já existe um arquivo dentro da pasta \"{caminhoArquivos}\" com o mesmo nome");

            arquivo.SaveAs(nomeCompletoArquivo);

            try
            {
                AdicionarControleLeituraImagemCanhotoAguardandoProcessamento(unitOfWork, nomeCompletoArquivo, arquivo.FileName, usuario);
            }
            catch (Exception)
            {
                Utilidades.IO.FileStorageService.Storage.Delete(nomeCompletoArquivo);
                throw;
            }
        }

        public void DefinirAPI(string apiLink, string apiKey)
        {
            this.APIKey = apiKey;
            this.APILink = apiLink;
        }

        public void DescartarDigitalizacao(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string caminhoCompleto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);

            controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.Descartada;
            controleLeituraImagemCanhoto.MensagemRetorno = "Canhoto Descartado.";

            MoverParaPastaCanhotos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);

            repControleLeituraImagemCanhoto.Atualizar(controleLeituraImagemCanhoto);
        }

        public void Iniciar(string caminhoRaizArquivos, string caminhoCanhotos, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, bool ftpPassivo, string portaFTP, bool utilizaSFTP, string adminMultisoftware, string caminhoBatReiniciar, string apiLink, string apiKey)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread))
            {
                CurrentUICulture = new System.Globalization.CultureInfo("pt-BR"),
                CurrentCulture = new System.Globalization.CultureInfo("pt-BR"),
                IsBackground = true
            };

            caminhoRaiz = caminhoRaizArquivos;
            caminhoArquivos = caminhoRaizArquivos + @"\FTP";
            TipoArmazenamento = tipoArmazenamento;
            EnderecoFTP = enderecoFTP;
            UsuarioFTP = usuarioFTP;
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            CaminhoBatReiniciar = caminhoBatReiniciar;
            PortaFTP = portaFTP;
            UtilizaSFTP = utilizaSFTP;
            AdminStringConexao = adminMultisoftware;
            APILink = apiLink;
            APIKey = apiKey;
            thread.Start();
        }

        public void InterpretarCanhotoPendente(ref Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, string caminhoRaizCanhotosProcessados, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            IntegrarCanhoto(controleLeituraImagemCanhoto, caminhoRaizCanhotosProcessados, unitOfWork, tipoServicoMultisoftware, integracaoAutomatica: false);
        }

        public string ObterBase64DaImagem(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = (controleLeituraImagemCanhoto.Canhoto == null) ? ObterCaminhoCanhotoNaoReconhecido(unitOfWork) : ObterCaminhoCanhoto(controleLeituraImagemCanhoto, unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);

            // Alguns arquivos possuem extensão TIF, porém não são de natureza TIF
            if (controleLeituraImagemCanhoto.Extensao == ExtensaoArquivo.TIF)
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeCompletoArquivo)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        string tmp = Path.GetTempFileName();

                        Utilidades.IO.FileStorageService.Storage.SaveImage(tmp, image, System.Drawing.Imaging.ImageFormat.Png);

                        nomeCompletoArquivo = tmp;
                    }
                }
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                return null;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            string base64 = Convert.ToBase64String(imageArray);

            return base64;
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto ObterCanhoto(Dominio.Entidades.Embarcador.Cargas.Carga carga, string caminhoCompleto, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagemErro;
            Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr = ObterTextoOCR(caminhoCompleto, out mensagemErro);

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool canhotoUnilever = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return null;

            bool canhotoAvulso;
            int numeroNotaFiscal = ObterNumeroNotaFiscal(retornoOcr, canhotoUnilever, out canhotoAvulso, unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;

            if (numeroNotaFiscal > 0)
                canhoto = repositorioCanhoto.BuscarCanhotoPorNumeroECarga(numeroNotaFiscal, carga.Codigo, canhotoAvulso);

            if (canhoto != null)
                return canhoto;

            List<int> numerosNotasFiscais = ObterNumerosNotasFiscais(retornoOcr.RetornoParse, canhotoAvulso);

            if (numerosNotasFiscais.Count > 0)
                canhoto = repositorioCanhoto.BuscarCanhotoPorNumerosECarga(numerosNotasFiscais, carga.Codigo, canhotoAvulso);

            return canhoto;
        }

        public string ObterChaveNFe(Stream imagem)
        {
            string mensagemErro;
            byte[] imageData;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                imagem.CopyTo(memoryStream);
                imageData = memoryStream.ToArray();
            }

            Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr = ObterTextoOCR(imageData, arquivoPDF: false, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                Log.TratarErro(mensagemErro, "LeitorOCR");
                throw new ServicoException("Não foi possivel identificar o conteudo da imagem.");
            }

            Match correspondenciaChaveNFe = Regex.Match(retornoOcr.RetornoParse, "[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4} [0-9]{4}[ .]?[0-9]{4}[ .]?[0-9]{4}");

            if (!correspondenciaChaveNFe.Success)
                throw new ServicoException("Não foi possível identificar a chave da NF-e.");

            return correspondenciaChaveNFe.Value.ObterSomenteNumeros();
        }

        public string ObterNumeroDocumento(string caminhoCompleto, bool canhotoUnilever, out bool canhotoAvulso, Repositorio.UnitOfWork unitOfWork, byte[] arquivo = null)
        {
            canhotoAvulso = false;

            try
            {
                string mensagemErro;
                Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoOcr retornoOcr = arquivo != null && arquivo.Length > 0 ? ObterTextoOCR(arquivo, false, out mensagemErro) : ObterTextoOCR(caminhoCompleto, out mensagemErro);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    return string.Empty;

                int numeroNotaFiscal = ObterNumeroNotaFiscal(retornoOcr, canhotoUnilever, out canhotoAvulso, unitOfWork);

                Log.TratarErro($"Leitor OCR NotaFiscal identificada: {numeroNotaFiscal}", "LeitorOCR");

                List<int> numerosNotasFiscais = ObterNumerosNotasFiscais(retornoOcr.RetornoParse, canhotoAvulso);

                if ((numeroNotaFiscal <= 0) && (numerosNotasFiscais.Count == 0))
                    return string.Empty;

                if (numeroNotaFiscal > 0)
                    numerosNotasFiscais.Prepend(numeroNotaFiscal);

                string numeroDocumento = string.Join(", ", numerosNotasFiscais).Left(tamanho: 150);

                return numeroDocumento;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return string.Empty;
            }
        }

        public MemoryStream ObterStremingPDF(Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = (controleLeituraImagemCanhoto.Canhoto == null) ? ObterCaminhoCanhotoNaoReconhecido(unitOfWork) : ObterCaminhoCanhoto(controleLeituraImagemCanhoto, unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
            {
                if (controleLeituraImagemCanhoto.Canhoto == null)
                {
                    caminho = ObterCaminhoCanhotoEnviados(unitOfWork);
                    nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);
                }

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                {
                    caminho = ObterCaminhoCanhotoProcessados(unitOfWork);
                    nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleLeituraImagemCanhoto.GuidArquivo);
                }

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                    return null;
            }

            byte[] pdfArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            MemoryStream output = new MemoryStream();

            output.Write(pdfArray, 0, pdfArray.Length);
            output.Position = 0;

            return output;
        }

        public void VincularCanhotoDaDigitalizacao(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string caminhoCompleto, Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto, TipoLeituraImagemCanhoto tipo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            string nomeArquivo = System.IO.Path.GetFileNameWithoutExtension(controleLeituraImagemCanhoto.GuidArquivo);

            canhoto.GuidNomeArquivo = nomeArquivo;
            canhoto.NomeArquivo = controleLeituraImagemCanhoto.NomeArquivo;
            canhoto.DataEnvioCanhoto = DateTime.Now;

            if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto)
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.UsuarioDigitalizacao = usuario;

                Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, tipoServicoMultisoftware ?? AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, tipoServicoMultisoftware ?? AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, tipoServicoMultisoftware);
            }
            else
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
            }

            canhoto.DataDigitalizacao = DateTime.Now;
            canhoto.MotivoRejeicaoDigitalizacao = string.Empty;
            canhoto.DataUltimaModificacao = DateTime.Now;

            if (usuario == null)
            {
                if (tipo == TipoLeituraImagemCanhoto.Automatico)
                {
                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Portal;
                    serCanhoto.GerarHistoricoCanhoto(canhoto, null, "Imagem do Canhoto digitalizada automaticamente via Portal.", unitOfWork);
                }
                else
                {
                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Mobile;
                    serCanhoto.GerarHistoricoCanhoto(canhoto, null, "Imagem do Canhoto digitalizada automaticamente via Mobile.", unitOfWork);
                }
            }
            else
                serCanhoto.GerarHistoricoCanhoto(canhoto, usuario, "Imagem do Canhoto vinculada.", unitOfWork);

            controleLeituraImagemCanhoto.NumeroDocumento = canhoto.Numero.ToString();
            controleLeituraImagemCanhoto.Canhoto = canhoto;
            controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto = SituacaoleituraImagemCanhoto.CanhotoVinculado;
            controleLeituraImagemCanhoto.MensagemRetorno = "Canhoto Vinculado.";

            MoverParaPastaCanhotos(controleLeituraImagemCanhoto, caminhoCompleto, unitOfWork);

            repControleLeituraImagemCanhoto.Atualizar(controleLeituraImagemCanhoto);
            repCanhoto.Atualizar(canhoto);
        }

        #endregion
    }
}
