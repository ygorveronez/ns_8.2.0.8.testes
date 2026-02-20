using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using JsonFx.IO;
using System;
using System.IO;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class TravamentoChave : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaRetornada
    {
        #region Construtores

        public TravamentoChave(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public TravamentoChave(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.TravamentoChave, cliente) { }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
                return;

            travamentoChave = new Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                Situacao = SituacaoTravamentoChave.Liberada,
                EtapaTravaChaveLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                DataTravamento = DateTime.Now,
                DataLiberacao = DateTime.Now
            };

            repositorioTravamentoChave.Inserir(travamentoChave);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Carga = carga;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public void EtapaRetornada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Situacao = SituacaoTravamentoChave.Liberada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Carga = cargaNova;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public bool SalvarAssinaturaMotorista(Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave, string tokenImagem, DateTime data, out string mensagemErro)
        {
            mensagemErro = "";
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista repositorioTravamentoChaveAssinaturaMotorista = new Repositorio.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista(_unitOfWork);

            if (travamentoChave == null)
            {
                mensagemErro = "Registro não encontrado.";
                return false;
            }

            string extensao = ".jpg";
            string nomeArquivo = "ASS_MOTORISTA_" + data.ToString("ddMMyyyyHHmmss") + "_" + travamentoChave.Codigo.ToString() + extensao;

            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista assinaturaMotorista = new Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista()
            {
                GuidArquivo = tokenImagem,
                NomeArquivo = nomeArquivo,
                DataEnvioAssinatura = data
            };

            repositorioTravamentoChaveAssinaturaMotorista.Inserir(assinaturaMotorista, _auditado);

            travamentoChave.TravamentoChaveAssinaturaMotorista = assinaturaMotorista;

            repositorioTravamentoChave.Atualizar(travamentoChave, _auditado);

            return true;
        }

        public void ArmazenarAssinaturaMotorista(Stream imagem, Repositorio.UnitOfWork unitOfWork, out string guid)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "GestaoPatio", "TravamentoChave", "AssinaturaMotorista" });

            ArmazenarArquivoFisico(imagem, caminho, out guid);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataTravaChavePrevista.HasValue)
                fluxoGestaoPatio.DataTravaChavePrevista = preSetTempoEtapa.DataTravaChavePrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!travamentoChave.EtapaTravaChaveLiberada)
                throw new ServicoException("O travamento da chave ainda não foi autorizada.");

            travamentoChave.Initialize();
            travamentoChave.DataTravamento = DateTime.Now;
            travamentoChave.Situacao = SituacaoTravamentoChave.Travada;

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, travamentoChave, null, "Travou a Chave", _unitOfWork);

            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioTravamentoChave.Atualizar(travamentoChave, _auditado);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataTravaChavePrevista = preSetTempoEtapa.DataTravaChavePrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataTravaChave.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgTravaChave = tempoEtapaAnterior;
            fluxoGestaoPatio.DataTravaChave = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.EtapaTravaChaveLiberada = true;
                travamentoChave.Situacao = SituacaoTravamentoChave.Liberada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataTravaChave;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataTravaChavePrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.EtapaTravaChaveLiberada = false;
                travamentoChave.Situacao = SituacaoTravamentoChave.Travada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgTravaChave = 0;
            fluxoGestaoPatio.DataTravaChave = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataTravaChavePrevista.HasValue)
                fluxoGestaoPatio.DataTravaChaveReprogramada = fluxoGestaoPatio.DataTravaChavePrevista.Value.Add(tempoReprogramar);
        }

        #endregion

        #region Métodos Privados

        private void ArmazenarArquivoFisico(Stream imagem, string caminho, out string guid)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                string mensagemRetorno = "Local para armazenamento do arquivo não está configurado! Favor entrar em contato com o suporte.";
                Log.TratarErro(mensagemRetorno);
                throw new ServicoException(mensagemRetorno);
            }

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";
            string token = Guid.NewGuid().ToString().Replace("-", "");

            using System.Drawing.Image t = System.Drawing.Image.FromStream(ms);

            guid = token;
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guid + extensao);
            string fileLocationMiniatura = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guid}-miniatura{extensao}");

            Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
            Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocationMiniatura, t);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                string mensagemRetorno = "Arquivo enviado não foi armazenado! Favor entrar em contato com o suporte.";
                Log.TratarErro(mensagemRetorno);
                throw new ServicoException(mensagemRetorno);
            }
        }

        #endregion
    }
}
