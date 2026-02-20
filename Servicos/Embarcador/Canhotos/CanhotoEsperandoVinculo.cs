using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Canhotos
{
    public class CanhotoEsperandoVinculo
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CanhotoEsperandoVinculo(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CanhotoEsperandoVinculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Canhotos.Canhoto ObterCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            string[] numerosDocumentos = canhotoEsperandoVinculo.NumeroDocumento.Split(',');

            for (int i = 0; i < numerosDocumentos.Length; i++)
            {
                int numeroDocumento = numerosDocumentos[i].Trim().ToInt();
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarCanhotoPorNumeroECarga(numeroDocumento, canhotoEsperandoVinculo.CargaEntrega.Carga.Codigo, canhotoEsperandoVinculo.CanhotoAvulso);

                if (canhoto != null)
                    return canhoto;
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo ObterCanhotoEsperandoVinculo(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            if (canhoto.CargaPedido == null)
                return null;

            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> canhotosEsperandoVinculo = repositorioCanhotoEsperandoVinculo.BuscarAguardandoVinculoPorCargaEPedido(canhoto.CargaPedido.Carga.Codigo, canhoto.CargaPedido.Pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo in canhotosEsperandoVinculo)
            {
                string[] numerosDocumentos = canhotoEsperandoVinculo.NumeroDocumento.Split(',');

                for (int i = 0; i < numerosDocumentos.Length; i++)
                {
                    int numeroDocumento = numerosDocumentos[i].Trim().ToInt();

                    if (canhoto.Numero == numeroDocumento)
                        return canhotoEsperandoVinculo;
                }
            }

            return null;
        }

        private async Task<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> ObterCanhotoEsperandoVinculoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            if (canhoto.CargaPedido == null)
                return null;

            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> canhotosEsperandoVinculo = await repositorioCanhotoEsperandoVinculo.BuscarAguardandoVinculoPorCargaEPedidoAsync(canhoto.CargaPedido.Carga.Codigo, canhoto.CargaPedido.Pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo in canhotosEsperandoVinculo)
            {
                string[] numerosDocumentos = canhotoEsperandoVinculo.NumeroDocumento.Split(',');

                for (int i = 0; i < numerosDocumentos.Length; i++)
                {
                    int numeroDocumento = numerosDocumentos[i].Trim().ToInt();

                    if (canhoto.Numero == numeroDocumento)
                        return canhotoEsperandoVinculo;
                }
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void Adicionar(string imagemEmBase64, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            using (MemoryStream imagemEmMemoria = new MemoryStream(Convert.FromBase64String(imagemEmBase64)))
            using (Image imagem = Image.FromStream(imagemEmMemoria))
            using (Bitmap manipuladorImagem = new Bitmap(imagem))
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                string nomeArquivo = $"{guid}.jpg";
                string caminhoCanhotosEsperandoVinculo = ObterCaminhoCanhotosEsperandoVinculo();
                string caminhoCompletoCanhoto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosEsperandoVinculo, nomeArquivo);

                Utilidades.IO.FileStorageService.Storage.SaveImage(caminhoCompletoCanhoto, manipuladorImagem);

                Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo = new Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo
                {
                    CargaEntrega = cargaEntrega,
                    GuidNomeArquivo = guid,
                    NomeArquivo = nomeArquivo,
                    Situacao = SituacaoCanhotoEsperandoVinculo.AguardandoLeituraNumeroDocumento
                };

                repositorioCanhotoEsperandoVinculo.Inserir(canhotoEsperandoVinculo);
            }
        }

        public string ObterCaminhoCanhotosEsperandoVinculo()
        {
            var configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo();

            string caminhoCanhotosEsperandoVinculo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoCanhotos, "CanhotosEsperandoVinculo");

            return caminhoCanhotosEsperandoVinculo;
        }

        public void ProcessarAguardandoLeituraNumeroDocumento(string apiLink, string apiKey, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> canhotosEsperandoVinculo = repositorioCanhotoEsperandoVinculo.BuscarAguardandoLeituraNumeroDocumento(limiteRegistros: 5);

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            bool canhotoUnilever = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

            if (canhotosEsperandoVinculo.Count == 0)
                return;

            string caminhoCanhotosEsperandoVinculo = ObterCaminhoCanhotosEsperandoVinculo();
            LeitorOCR servicoLeitorOCR = new LeitorOCR(_unitOfWork);

            servicoLeitorOCR.DefinirAPI(apiLink, apiKey);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo in canhotosEsperandoVinculo)
            {
                try
                {
                    string caminhoCompletoCanhotoEsperandoVinculo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosEsperandoVinculo, canhotoEsperandoVinculo.NomeArquivo);

                    _unitOfWork.Start();

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompletoCanhotoEsperandoVinculo))
                        canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.ArquivoNaoEncontrado;
                    else
                    {
                        bool canhotoAvulso;
                        string numeroDocumento = servicoLeitorOCR.ObterNumeroDocumento(caminhoCompletoCanhotoEsperandoVinculo, canhotoUnilever, out canhotoAvulso, _unitOfWork);

                        if (string.IsNullOrWhiteSpace(numeroDocumento))
                            canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.NumeroDocumentoNaoEncontrado;
                        else
                        {
                            canhotoEsperandoVinculo.CanhotoAvulso = canhotoAvulso;
                            canhotoEsperandoVinculo.NumeroDocumento = numeroDocumento;
                            canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.AguardandoVinculo;
                        }
                    }

                    repositorioCanhotoEsperandoVinculo.Atualizar(canhotoEsperandoVinculo);

                    if (canhotoEsperandoVinculo.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo)
                    {
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = ObterCanhoto(canhotoEsperandoVinculo);

                        if ((canhoto != null) && (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado))
                            VincularCanhotos(canhotoEsperandoVinculo, canhoto, tipoServicoMultisoftware, clienteMultisoftware);
                    }

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void Vincular(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo = ObterCanhotoEsperandoVinculo(canhoto);

            if (canhotoEsperandoVinculo == null)
                return;

            VincularCanhotos(canhotoEsperandoVinculo, canhoto, tipoServicoMultisoftware, clienteMultisoftware);
        }

        public async Task VincularAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo = await ObterCanhotoEsperandoVinculoAsync(canhoto);

            if (canhotoEsperandoVinculo == null)
                return;

            await VincularCanhotosAsync(canhotoEsperandoVinculo, canhoto, tipoServicoMultisoftware, clienteMultisoftware);
        }

        public async Task VincularCanhotosAsync(Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
            string caminhoCanhotosEsperandoVinculo = ObterCaminhoCanhotosEsperandoVinculo();
            string caminhoCompletoCanhotoEsperandoVinculo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosEsperandoVinculo, canhotoEsperandoVinculo.NomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompletoCanhotoEsperandoVinculo))
            {
                canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.ArquivoNaoEncontrado;
                await repositorioCanhotoEsperandoVinculo.AtualizarAsync(canhotoEsperandoVinculo);
                return;
            }

            Canhoto servicoCanhoto = new Canhoto(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            canhoto.DataEnvioCanhoto = DateTime.Now;
            canhoto.GuidNomeArquivo = canhotoEsperandoVinculo.GuidNomeArquivo;
            canhoto.NomeArquivo = canhotoEsperandoVinculo.NomeArquivo;
            canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;

            if (!configuracaoEmbarcador.ExigeAprovacaoDigitalizacaoCanhoto)
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.DataDigitalizacao = DateTime.Now;

                Canhoto.CanhotoLiberado(canhoto, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
                CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, _unitOfWork);
                Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, _unitOfWork, tipoServicoMultisoftware);
            }
            else
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracaoEmbarcador, _unitOfWork);
            }

            canhoto.DataUltimaModificacao = DateTime.Now;
            canhoto.MotivoRejeicaoDigitalizacao = string.Empty;
            canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Portal;

            await repositorioCanhoto.AtualizarAsync(canhoto);
            servicoCanhoto.GerarHistoricoCanhoto(canhoto, null, "Imagem do Canhoto digitalizada via portal utilizando o canhoto vinculado pelo mobile.", _unitOfWork);

            string caminhoCanhoto = Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
            string caminhoCompletoCanhoto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhotoEsperandoVinculo.NomeArquivo);

            Utilidades.IO.FileStorageService.Storage.Copy(caminhoCompletoCanhotoEsperandoVinculo, caminhoCompletoCanhoto);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompletoCanhoto))
            {
                Servicos.Log.TratarErro($"Erro05 - Falha ao copiar canhoto de {caminhoCompletoCanhotoEsperandoVinculo} para {caminhoCompletoCanhoto}.", "EnviarCanhoto");
                throw new ServicoException("Erro05 - Falha ao copiar canhoto.");
            }

            canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.Vinculado;
            canhotoEsperandoVinculo.XMLNotaFiscal = canhoto.XMLNotaFiscal;

            await repositorioCanhotoEsperandoVinculo.AtualizarAsync(canhotoEsperandoVinculo);
        }

        public void VincularCanhotos(Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(_unitOfWork);
            string caminhoCanhotosEsperandoVinculo = ObterCaminhoCanhotosEsperandoVinculo();
            string caminhoCompletoCanhotoEsperandoVinculo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosEsperandoVinculo, canhotoEsperandoVinculo.NomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompletoCanhotoEsperandoVinculo))
            {
                canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.ArquivoNaoEncontrado;
                repositorioCanhotoEsperandoVinculo.Atualizar(canhotoEsperandoVinculo);
                return;
            }

            Canhoto servicoCanhoto = new Canhoto(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            canhoto.DataEnvioCanhoto = DateTime.Now;
            canhoto.GuidNomeArquivo = canhotoEsperandoVinculo.GuidNomeArquivo;
            canhoto.NomeArquivo = canhotoEsperandoVinculo.NomeArquivo;
            canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;

            if (!configuracaoEmbarcador.ExigeAprovacaoDigitalizacaoCanhoto)
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.DataDigitalizacao = DateTime.Now;

                Canhoto.CanhotoLiberado(canhoto, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
                CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, _unitOfWork);
                Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, _unitOfWork, tipoServicoMultisoftware);
            }
            else
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracaoEmbarcador, _unitOfWork);
            }

            canhoto.DataUltimaModificacao = DateTime.Now;
            canhoto.MotivoRejeicaoDigitalizacao = string.Empty;
            canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Portal;

            repositorioCanhoto.Atualizar(canhoto);
            servicoCanhoto.GerarHistoricoCanhoto(canhoto, null, "Imagem do Canhoto digitalizada via portal utilizando o canhoto vinculado pelo mobile.", _unitOfWork);

            string caminhoCanhoto = Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
            string caminhoCompletoCanhoto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhotoEsperandoVinculo.NomeArquivo);

            Utilidades.IO.FileStorageService.Storage.Copy(caminhoCompletoCanhotoEsperandoVinculo, caminhoCompletoCanhoto);
            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompletoCanhoto))
            {
                Servicos.Log.TratarErro($"Erro06 - Falha ao copiar canhoto de {caminhoCompletoCanhotoEsperandoVinculo} para {caminhoCompletoCanhoto}.", "EnviarCanhoto");
                throw new Exception("Erro06 - Falha ao copiar canhoto.");
            }


            canhotoEsperandoVinculo.Situacao = SituacaoCanhotoEsperandoVinculo.Vinculado;
            canhotoEsperandoVinculo.XMLNotaFiscal = canhoto.XMLNotaFiscal;

            repositorioCanhotoEsperandoVinculo.Atualizar(canhotoEsperandoVinculo);
        }

        #endregion Métodos Públicos
    }
}
