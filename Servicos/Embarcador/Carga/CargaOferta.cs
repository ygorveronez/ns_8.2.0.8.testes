using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Cargas
{
    public class CargaOferta : ServicoBase
    {

        #region Construtores

        public CargaOferta(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaOferta(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork, tipoServicoMultisoftware) { }

        #endregion Construtores

        #region Métodos Públicos

        public async Task<(int quantidadeCriada, List<string> mensagemErro)> AtualizarSituacaoAsync(List<long> codigosCargasOfertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaOferta novaSituacao, CancellationToken cancellationToken, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (codigosCargasOfertas.Count > 2000)
                throw new ServicoException("Não é possível processar mais de 2000 registros por vez.");

            // Initialize services and repositories
            Servicos.Embarcador.Carga.Carga servicoCarga = new(_unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            List<string> mensagensErro = new List<string>();
            int quantidadeCriada = 0;

            // Load all necessary data up-front
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao repositorioParametrosOfertasTipoIntegracao = new(_unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracoes = await repositorioParametrosOfertasTipoIntegracao.BuscarPorOfertaAsync(codigosCargasOfertas, cancellationToken);

            Repositorio.Embarcador.Cargas.CargaOferta repCargaOferta = new(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaOferta> cargasOfertas = await repCargaOferta.BuscarPorCodigosAsync(codigosCargasOfertas);

            List<int> codigosParametrosOfertas = cargasOfertas.Select(oferta => oferta.ParametrosOfertas.Codigo).ToList();

            Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repositorioCargaOfertaIntegracao = new(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao> integracoesDeOferta = await repositorioCargaOfertaIntegracao.BuscarPorCodigoCargaOfertaEPorTipo(codigosCargasOfertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Ofertar, cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao> parametrosOfertasTipoIntegracoes = await repositorioParametrosOfertasTipoIntegracao.BuscarPorParametrosOfertasAsync(codigosParametrosOfertas, cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta in cargasOfertas)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga tipoIntegracaoOfertaCarga = TipoIntegracaoOfertaCarga.NaoDefinido;
                DateTime? dataFimOferta = null;

                if (novaSituacao == SituacaoCargaOferta.EmOferta)
                {
                    (string erroVerificarHorario, dataFimOferta) = await VerificarParametrosOfertasDadosOfertaDiaSemanaAsync(cargaOferta, cancellationToken);
                    if (!string.IsNullOrEmpty(erroVerificarHorario))
                    {
                        mensagensErro.Add(erroVerificarHorario);
                        continue;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao integracaoEncontrada = integracoesDeOferta.Find(integracao => integracao.CargaOferta.Codigo == cargaOferta.Codigo);

                string validacaoOferta = ValidarSituacoesOferta(cargaOferta, integracaoEncontrada, novaSituacao);
                if (!string.IsNullOrEmpty(validacaoOferta))
                {
                    mensagensErro.Add(validacaoOferta);
                    continue;
                }
                if (novaSituacao == SituacaoCargaOferta.EmOferta)
                {
                    if (cargaOferta.Carga.SituacaoCarga == SituacaoCarga.Nova ||
                       (cargaOferta.Carga.SituacaoCarga == SituacaoCarga.AgTransportador && !cargaOferta.Carga.ExigeNotaFiscalParaCalcularFrete))
                    {
                        if ((cargaOferta.Situacao == SituacaoCargaOferta.Cancelada || cargaOferta.Situacao == SituacaoCargaOferta.PrazoExpirado) && cargaOferta.CodigoIntegracao != null)
                        {
                            tipoIntegracaoOfertaCarga = TipoIntegracaoOfertaCarga.Ativar;
                            cargaOferta.Descricao = "Reofertou a carga.";
                        }
                        else
                        {
                            tipoIntegracaoOfertaCarga = TipoIntegracaoOfertaCarga.Ofertar;
                            cargaOferta.Descricao = "Ofertou a carga";
                        }
                    }
                    else
                    {
                        mensagensErro.Add($"A situação da carga está como: {cargaOferta.Carga.SituacaoCarga}.");
                        continue;
                    }
                }
                else if (novaSituacao == SituacaoCargaOferta.Cancelada)
                {
                    tipoIntegracaoOfertaCarga = TipoIntegracaoOfertaCarga.Cancelar;

                    if (cargaOferta.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                        cargaOferta.Descricao = "Cancelou a oferta ao receber cancelamento da Carga.";
                    else
                        cargaOferta.Descricao = "Cancelou a oferta.";
                }

                List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao> parametrosOfertasTipoIntegracoesOfertaAtual = parametrosOfertasTipoIntegracoes.Where(pti => pti.ParametrosOfertas.Codigo == cargaOferta.ParametrosOfertas.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracoesOfertaAtual = tipoIntegracoes.Where(t => parametrosOfertasTipoIntegracoesOfertaAtual.Any(p => p.TipoIntegracao == t.Tipo)).ToList();

                if (tipoIntegracoesOfertaAtual.Count == 0)
                {
                    mensagensErro.Add("Não possui tipo de integração disponível nos parâmetros de oferta.");
                    continue;
                }

                await _unitOfWork.StartAsync();

                try
                {
                    if (novaSituacao == SituacaoCargaOferta.EmOferta)
                    {
                        cargaOferta.DataFimOferta = dataFimOferta;
                        await repCargaOferta.AtualizarAsync(cargaOferta);
                    }

                    await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, cargaOferta, null, "Atualizado", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);

                    if (novaSituacao == SituacaoCargaOferta.Cancelada)
                        await servicoCarga.ExcluirDadosTransporteParaOfertaCanceladaAsync(cargaOferta, auditado, cancellationToken);

                    var servico = new Servicos.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork);

                    foreach (var tipoIntegracao in tipoIntegracoesOfertaAtual)
                        await servico.CriarRegistroIntegracao(cargaOferta, tipoIntegracao, tipoIntegracaoOfertaCarga);

                    await _unitOfWork.CommitChangesAsync(cancellationToken);
                    quantidadeCriada++;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();

                    if (ex is BaseException)
                        mensagensErro.Add(ex.Message);
                    else
                    {
                        Servicos.Log.TratarErro(ex);
                        mensagensErro.Add("Ocorreu uma falha ao ofertar.");
                    }
                }
            }

            return (quantidadeCriada, mensagensErro);
        }

        public void CancelarOfertaAposCancelamentoCarga(int codigoCargaCancelamentoCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaOferta repositorioCargaOferta = new(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta = repositorioCargaOferta.BuscarPorCargaAsync(codigoCargaCancelamentoCarga).GetAwaiter().GetResult();

            if (cargaOferta != null)
            {
                Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao repositorioParametrosOfertasTipoIntegracao = new(_unitOfWork, default);
                repositorioParametrosOfertasTipoIntegracao.PossuiTipoIntegracaoAsync(TipoIntegracao.TrizyOfertarCarga, cargaOferta.ParametrosOfertas.Codigo).GetAwaiter().GetResult();

                if (cargaOferta.Situacao == SituacaoCargaOferta.PrazoExpirado || cargaOferta.Situacao == SituacaoCargaOferta.PendenteDeOferta || cargaOferta.CodigoIntegracao == null)
                {
                    cargaOferta.Situacao = SituacaoCargaOferta.Cancelada;

                    cargaOferta.Descricao = "Cancelou a oferta ao receber cancelamento da Carga.";
                    Servicos.Auditoria.Auditoria.AuditarAsync(auditado, cargaOferta, null, "Atualizado", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, default).GetAwaiter().GetResult();
                }
                else
                {
                    List<long> codigoCargaOferta = new List<long>() { cargaOferta.Codigo };
                    Servicos.Embarcador.Cargas.CargaOferta servicoCargaOferta = new(_unitOfWork);
                    servicoCargaOferta.AtualizarSituacaoAsync(codigoCargaOferta, SituacaoCargaOferta.Cancelada, default, auditado).GetAwaiter().GetResult();
                }
            }
        }


        public async Task VerificarCargaOfertaExpiradas(CancellationToken cancellationToken)
        {
            _unitOfWork.FlushAndClear();

            try
            {
                Servicos.Log.TratarErro("Inicio Buscando Cargas Expiradas", "VerificarCargaOfertaExpiradas");

                Repositorio.Embarcador.Cargas.CargaOferta repCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaOferta> cargasOfertasExpiradas = await repCargaOferta.BuscarCargasOfertasExpiradasAsync(DateTime.Now.AddMinutes(1), cancellationToken);

                foreach (var item in cargasOfertasExpiradas)
                {
                    item.Situacao = SituacaoCargaOferta.PrazoExpirado;
                    await repCargaOferta.AtualizarAsync(item);
                }

                _unitOfWork.Flush();

                Servicos.Log.TratarErro("Fim Cargas Expiradas", "VerificarCargaOfertaExpiradas");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private async Task<(string, DateTime?)> VerificarParametrosOfertasDadosOfertaDiaSemanaAsync(Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana repositorioDiaSemana = new Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana(_unitOfWork, cancellationToken);

            var diasDaSemana = await repositorioDiaSemana.BuscarPorCodigoParametrosOfertaDadosOfertaAsync(cargaOferta.ParametrosOfertas.Codigo, cancellationToken);


            if (diasDaSemana == null || diasDaSemana.Count == 0)
                return ($"Carga {cargaOferta.Carga.CodigoCargaEmbarcador} Sem Horario Configurado", null);

            var horariosDoDia = diasDaSemana.Where(d => d.DiaSemana == DiaSemanaHelper.ObterDiaSemana(DateTime.Now)).ToList();

            if (horariosDoDia?.Count == 0)
                return ($"Carga {cargaOferta.Carga.CodigoCargaEmbarcador} Sem Horario Configurado para o dia de hoje", null);


            if (!horariosDoDia.Any(h => h.ParametrosOfertasDadosOferta.HoraInicio <= DateTime.Now.TimeOfDay && h.ParametrosOfertasDadosOferta.HoraTermino >= DateTime.Now.TimeOfDay))
            {
                string horarios = string.Join(" e ", horariosDoDia.Select(h => $"{h.ParametrosOfertasDadosOferta.HoraInicio:hh\\:mm} às {h.ParametrosOfertasDadosOferta.HoraTermino:hh\\:mm}"));
                return ($"Carga {cargaOferta.Carga.CodigoCargaEmbarcador} Fora do Horario de Oferta. Horarios Permitidos Hoje: {horarios}", null);
            }

            DateTime DataFimOferta = DateTime.Now.Date.Add(horariosDoDia.First(h => h.ParametrosOfertasDadosOferta.HoraInicio <= DateTime.Now.TimeOfDay && h.ParametrosOfertasDadosOferta.HoraTermino >= DateTime.Now.TimeOfDay).ParametrosOfertasDadosOferta.HoraTermino);

            return (string.Empty, DataFimOferta);
        }

        private string ValidarSituacoesOferta(Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta, Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao integracaoEncontrada, SituacaoCargaOferta novaSituacao)
        {
            List<SituacaoCargaOferta> situacoesValidas = new List<SituacaoCargaOferta>()
            {
                SituacaoCargaOferta.PendenteDeOferta,
                SituacaoCargaOferta.Cancelada,
                SituacaoCargaOferta.PrazoExpirado
            };

            if (!situacoesValidas.Contains(cargaOferta.Situacao) && (integracaoEncontrada == null || (integracaoEncontrada != null && integracaoEncontrada.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)) && novaSituacao == SituacaoCargaOferta.EmOferta)
                return $"A situação da oferta da Carga: {cargaOferta.Carga.Descricao} não está: {SituacaoCargaOfertaHelper.ObterDescricao(SituacaoCargaOferta.PendenteDeOferta)}, {SituacaoCargaOfertaHelper.ObterDescricao(SituacaoCargaOferta.Cancelada)} ou {SituacaoCargaOfertaHelper.ObterDescricao(SituacaoCargaOferta.PrazoExpirado)}";

            if (string.IsNullOrEmpty(cargaOferta.CodigoIntegracao) && novaSituacao == SituacaoCargaOferta.Cancelada)
                return "A carga precisa ter sido ofertada com sucesso para que seja cancelada.";

            return string.Empty;
        }

        #endregion Métodos Privados
    }
}
