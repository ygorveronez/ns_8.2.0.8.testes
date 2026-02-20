using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.IO;

namespace Servicos.Embarcador.Arquivo
{
    public sealed class ControleGeracaoArquivo
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ControleGeracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Notificacoes.Notificacao AdicionarNotificacao(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, string nota, string urlPagina, IconesNotificacao IconeNotificacao)
        {
            Repositorio.Embarcador.Notificacoes.Notificacao repositorioNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = new Dominio.Entidades.Embarcador.Notificacoes.Notificacao()
            {
                CodigoObjetoNotificacao = controleGeracaoArquivo.Codigo,
                DataNotificacao = DateTime.Now,
                Icone = IconeNotificacao,
                IconeCorFundo = IconeNotificacao.ObterCorFundoPadrao(),
                Nota = Utilidades.String.Left(nota, 1000),
                SituacaoNotificacao = SituacaoNotificacao.Nova,
                TipoNotificacao = TipoNotificacao.arquivo,
                URLPagina = urlPagina,
                Usuario = controleGeracaoArquivo.Usuario
            };

            repositorioNotificacao.Inserir(notificacao);

            return notificacao;
        }

        private Dominio.Entidades.Embarcador.Notificacoes.Notificacao AdicionarNotificacaoGeracaoArquivoFinalizada(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, string nota, string urlPagina)
        {
            return AdicionarNotificacao(controleGeracaoArquivo, nota, urlPagina, controleGeracaoArquivo.TipoArquivo.ObterIconeNotificacao());
        }

        private Dominio.Entidades.Embarcador.Notificacoes.Notificacao AdicionarNotificacaoGeracaoArquivoFinalizadaComFalha(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, string nota, string urlPagina)
        {
            return AdicionarNotificacao(controleGeracaoArquivo, nota, urlPagina, IconesNotificacao.falha);
        }

        private void NotificarGeracaoArquivoFinalizada(Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao)
        {
            Hubs.Notificacao servicoNotificacao = new Hubs.Notificacao();

            servicoNotificacao.NotificarUsuario(notificacao);
        }

        private string ObterCaminhoArquivo(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            var configuracaoArquivos = Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo();
            string diretorioArquivo = configuracaoArquivos.CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

            return $"{Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivo, controleGeracaoArquivo.GuidArquivo)}.{controleGeracaoArquivo.TipoArquivo.ObterExtensao()}";
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo Adicionar(string descricao, Dominio.Entidades.Usuario usuario, TipoArquivo tipoArquivo)
        {
            Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorio = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = new Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo()
            {
                DataInicioGeracao = DateTime.Now,
                Descricao = descricao,
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Situacao = SituacaoGeracaoArquivo.Gerando,
                TipoArquivo = tipoArquivo,
                Usuario = usuario
            };

            repositorio.Inserir(controleGeracaoArquivo);

            return controleGeracaoArquivo;
        }

        public void Finalizar(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, string nota, string urlPagina)
        {
            Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorio = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(_unitOfWork);

            controleGeracaoArquivo = repositorio.BuscarPorCodigo(controleGeracaoArquivo.Codigo);
            controleGeracaoArquivo.DataFimGeracao = DateTime.Now;
            controleGeracaoArquivo.Situacao = SituacaoGeracaoArquivo.Gerado;

            try
            {
                _unitOfWork.Start();

                repositorio.Atualizar(controleGeracaoArquivo);

                Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = AdicionarNotificacaoGeracaoArquivoFinalizada(controleGeracaoArquivo, nota, urlPagina);

                _unitOfWork.CommitChanges();

                NotificarGeracaoArquivoFinalizada(notificacao);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void FinalizarComFalha(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, string nota, string urlPagina, Exception excecao)
        {
            Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorio = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(_unitOfWork);

            controleGeracaoArquivo = repositorio.BuscarPorCodigo(controleGeracaoArquivo.Codigo);
            controleGeracaoArquivo.DataFimGeracao = DateTime.Now;
            controleGeracaoArquivo.Situacao = SituacaoGeracaoArquivo.FalhaAoGerar;

            try
            {
                _unitOfWork.Start();

                repositorio.Atualizar(controleGeracaoArquivo);

                Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = AdicionarNotificacaoGeracaoArquivoFinalizadaComFalha(controleGeracaoArquivo, nota, urlPagina);

                Log.TratarErro($"Gerar Arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} GUID: {controleGeracaoArquivo.GuidArquivo} Falha: " + excecao);

                _unitOfWork.CommitChanges();

                NotificarGeracaoArquivoFinalizada(notificacao);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public byte[] ObterBinarioArquivo(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            var configuracaoArquivos = Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo();
            string caminhoArquivo = $"{Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivos.CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), controleGeracaoArquivo.GuidArquivo)}.{controleGeracaoArquivo.TipoArquivo.ObterExtensao()}";

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                throw new ServicoException($"O arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} não existe mais no servidor, por favor gere um novo");

            byte[] binarioArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo);

            if (binarioArquivo == null)
                throw new ServicoException($"Não foi possível obter os dados do arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()}");

            return binarioArquivo;
        }

        public void Remover(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorio = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(_unitOfWork);

            repositorio.Deletar(controleGeracaoArquivo);
        }

        public void SalvarArquivo(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, MemoryStream arquivo)
        {
            SalvarArquivo(controleGeracaoArquivo, arquivo.ToArray());
        }

        public void SalvarArquivo(Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo, byte[] arquivo)
        {
            string caminhoArquivo = ObterCaminhoArquivo(controleGeracaoArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoArquivo, arquivo);
        }

        #endregion
    }
}
