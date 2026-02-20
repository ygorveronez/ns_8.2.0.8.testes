using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class SeparacaoMercadoria : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public SeparacaoMercadoria(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public SeparacaoMercadoria(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.SeparacaoMercadoria, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaAvancar separacaoMercadoriaAvancar)
        {
            if (separacaoMercadoria == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!separacaoMercadoria.EtapaSeparacaoMercadoriaLiberada)
                throw new ServicoException("A liberação da separação de mercadoria ainda não foi autorizada.");

            separacaoMercadoria.Initialize();
            separacaoMercadoria.DataSeparacaoMercadoriaInformada = DateTime.Now;
            separacaoMercadoria.Situacao = SituacaoSeparacaoMercadoria.SeparacaoMercadoriaFinalizada;

            if (separacaoMercadoriaAvancar != null)
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

                separacaoMercadoria.NumeroCarregadores = separacaoMercadoriaAvancar.NumeroCarregadores;
                separacaoMercadoria.ResponsavelCarregamento = (separacaoMercadoriaAvancar.CodigoResponsavelCarregamento > 0) ? repositorioUsuario.BuscarPorCodigo(separacaoMercadoriaAvancar.CodigoResponsavelCarregamento) : null;

                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel repositorioSeparacaoMercadoriaResponsavel = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel(_unitOfWork);

                foreach(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavelSeparacao responsavel in separacaoMercadoriaAvancar.ResponsaveisSeparacao)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel responsavelAdicionar = new Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel()
                    {
                        CapacidadeSeparacao = responsavel.CapacidadeSeparacao,
                        Responsavel = repositorioUsuario.BuscarPorCodigo(responsavel.CodigoResponsavel),
                        SeparacaoMercadoria = separacaoMercadoria
                    };

                    repositorioSeparacaoMercadoriaResponsavel.Inserir(responsavelAdicionar);
                }
            }

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork).Atualizar(separacaoMercadoria, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (separacaoMercadoria != null)
                return;

            separacaoMercadoria = new Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaSeparacaoMercadoriaLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria
            };

            repositorioSeparacaoMercadoria.Inserir(separacaoMercadoria);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaAvancar separacaoMercadoriaAvancar)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorCodigo(separacaoMercadoriaAvancar.Codigo);

            AvancarEtapa(separacaoMercadoria?.FluxoGestaoPatio, separacaoMercadoria, separacaoMercadoriaAvancar);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (separacaoMercadoria != null)
            {
                separacaoMercadoria.Carga = carga;
                repositorioSeparacaoMercadoria.Atualizar(separacaoMercadoria);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (separacaoMercadoria != null)
            {
                separacaoMercadoria.Carga = cargaNova;
                repositorioSeparacaoMercadoria.Atualizar(separacaoMercadoria);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataSeparacaoMercadoriaPrevista.HasValue)
                fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista = preSetTempoEtapa.DataSeparacaoMercadoriaPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, separacaoMercadoria, separacaoMercadoriaAvancar: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista = preSetTempoEtapa.DataSeparacaoMercadoriaPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataSeparacaoMercadoria.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoSeparacaoMercadoria = tempoEtapaAnterior;
            fluxoGestaoPatio.DataSeparacaoMercadoria = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (separacaoMercadoria != null)
            {
                separacaoMercadoria.EtapaSeparacaoMercadoriaLiberada = true;
                repositorioSeparacaoMercadoria.Atualizar(separacaoMercadoria);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataSeparacaoMercadoria;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (separacaoMercadoria != null)
            {
                separacaoMercadoria.EtapaSeparacaoMercadoriaLiberada = false;
                repositorioSeparacaoMercadoria.Atualizar(separacaoMercadoria);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoSeparacaoMercadoria = 0;
            fluxoGestaoPatio.DataSeparacaoMercadoria = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista.HasValue)
                fluxoGestaoPatio.DataSeparacaoMercadoriaReprogramada = fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
