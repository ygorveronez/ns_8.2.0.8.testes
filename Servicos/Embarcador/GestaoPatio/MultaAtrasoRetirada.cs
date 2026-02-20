using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class MultaAtrasoRetirada
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public MultaAtrasoRetirada(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null)
        {
        }

        public MultaAtrasoRetirada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void GerarCargaOcorrencia(Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada multaAtrasoRetirada, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioCargaCTe.BuscarPorCarga(multaAtrasoRetirada.Carga.Codigo, false, false, false, false, false, 0, 0, true, false, 0);

            if (listaCargaCTe.Count == 0)
                return;

            Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada repositorioMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            string mensagemRetorno = null;
            decimal valorOcorrencia = multaAtrasoRetirada.Carga.ValorFreteAPagar * (multaAtrasoRetirada.RegrasMultaAtrasoRetirada.PercentualInclusao / 100);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia()
            {
                Carga = multaAtrasoRetirada.Carga,
                DataAlteracao = DateTime.Now,
                DataOcorrencia = DateTime.Now,
                DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork),
                Observacao = "",
                ObservacaoCTe = "",
                ObservacaoCTes = "",
                OrigemOcorrencia = multaAtrasoRetirada.RegrasMultaAtrasoRetirada.TipoOcorrencia.OrigemOcorrencia,
                TipoOcorrencia = multaAtrasoRetirada.RegrasMultaAtrasoRetirada.TipoOcorrencia,
                ComponenteFrete = multaAtrasoRetirada.RegrasMultaAtrasoRetirada.TipoOcorrencia.ComponenteFrete,
                ValorOcorrencia = valorOcorrencia,
                ValorOcorrenciaOriginal = valorOcorrencia
            };

            repositorioOcorrencia.Inserir(cargaOcorrencia);

            Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, listaCargaCTe, _unitOfWork);
            servicoOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, listaCargaCTe, null, ref mensagemRetorno, _unitOfWork, tipoServicoMultisoftware, null, configuracaoEmbarcador, clienteMultisoftware, "", false);

            multaAtrasoRetirada.CargaOcorrencia = cargaOcorrencia;
            multaAtrasoRetirada.GerarOcorrencia = false;

            repositorioMultaAtrasoRetirada.Atualizar(multaAtrasoRetirada);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento ObterPeriodoCarregamento(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regraMultaAtrasoRetirada, DateTime dataLiberacaoCargaParaTransportador)
        {
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataLiberacaoCargaParaTransportador);
            Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorRegrasMultaAtrasoRetiradaEDia(regraMultaAtrasoRetirada.Codigo, diaSemana);

            if (periodosCarregamento.Count == 0)
                return null;

            return periodosCarregamento
                .Where(o => o.HoraInicio <= dataLiberacaoCargaParaTransportador.TimeOfDay && o.HoraTermino >= dataLiberacaoCargaParaTransportador.TimeOfDay)
                .FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada ObterRegraMultaAtrasoRetirada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repositorioRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> regrasMultaAtrasoRetirada = repositorioRegrasMultaAtrasoRetirada.BuscarAtivasPorFilial(carga.Filial?.Codigo ?? 0);

            if (regrasMultaAtrasoRetirada.Count == 0)
                return null;

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            List<Dominio.Entidades.Cliente> destinatarios = servicoCargaDadosSumarizados.ObterDestinatarios(carga.Codigo, _unitOfWork);
            List<Dominio.Entidades.Localidade> cidades = destinatarios.Select(o => o.Localidade).Distinct().ToList();
            List<Dominio.Entidades.Estado> estados = cidades.Select(o => o.Estado).Distinct().ToList();
            List<int> ceps = cidades.Select(o => o.CEP.ObterSomenteNumeros().ToInt()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> regrasMultaAtrasoRetiradaFiltradas = new List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada>();

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regraMultaAtrasoRetirada in regrasMultaAtrasoRetirada)
            {
                if (!regraMultaAtrasoRetirada.Transportadores.Any(o => o.Codigo == carga.Empresa.Codigo))
                    continue;

                if ((regraMultaAtrasoRetirada.TipoOperacoes.Count > 0) && (!regraMultaAtrasoRetirada.TipoOperacoes.Any(o => o.Codigo == (carga.TipoOperacao?.Codigo ?? 0))))
                    continue;

                if ((regraMultaAtrasoRetirada.Clientes.Count > 0) && (!regraMultaAtrasoRetirada.Clientes.Any(o => destinatarios.Any(destinatario => destinatario.CPF_CNPJ == o.CPF_CNPJ))))
                    continue;

                if ((regraMultaAtrasoRetirada.Cidades.Count > 0) && (!regraMultaAtrasoRetirada.Cidades.Any(o => cidades.Any(cidade => cidade.Codigo == o.Codigo))))
                    continue;

                if ((regraMultaAtrasoRetirada.Estados.Count > 0) && (!regraMultaAtrasoRetirada.Estados.Any(o => estados.Any(estado => estado.Codigo == o.Codigo))))
                    continue;

                if ((regraMultaAtrasoRetirada.CEPs.Count > 0) && (!regraMultaAtrasoRetirada.CEPs.Any(o => ceps.Any(cep => o.CEPInicial <= cep && o.CEPInicial >= cep))))
                    continue;

                regrasMultaAtrasoRetiradaFiltradas.Add(regraMultaAtrasoRetirada);
            }

            return regrasMultaAtrasoRetiradaFiltradas
                .OrderByDescending(o => o.TipoOperacoes.Count > 0)
                .ThenByDescending(o => o.Clientes.Count > 0)
                .ThenByDescending(o => o.CEPs.Count > 0)
                .ThenByDescending(o => o.Cidades.Count > 0)
                .ThenByDescending(o => o.Estados.Count > 0)
                .FirstOrDefault();
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void GerarOcorrenciaPorFinalizacaoEtapaFluxoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapaFluxoPatio, DateTime dataFinalizacao, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (etapaFluxoPatio != EtapaFluxoGestaoPatio.Guarita)
                return;

            if (carga.Empresa == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador janelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(carga.Codigo, carga.Empresa.Codigo);

            if (janelaCarregamentoTransportador?.HorarioLiberacao == null)
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regraMultaAtrasoRetirada = ObterRegraMultaAtrasoRetirada(carga);

            if (regraMultaAtrasoRetirada == null)
                return;

            DateTime dataLiberacaoCarga = janelaCarregamentoTransportador.HorarioLiberacao.Value;
            Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento periodoCarregamento = ObterPeriodoCarregamento(regraMultaAtrasoRetirada, dataLiberacaoCarga);

            if (periodoCarregamento == null)
                return;

            Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada repositorioMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada multaAtrasoRetirada = new Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada()
            {
                Carga = carga,
                DataCriacao = DateTime.Now,
                DataLiberacaoCarga = dataLiberacaoCarga,
                DataRetiradaCarga = dataFinalizacao,
                HoraInicioPeriodo = periodoCarregamento.HoraInicio,
                HoraTerminoPeriodo = periodoCarregamento.HoraTermino,
                QuantidadeCargas = periodoCarregamento.QuantidadeCargas,
                QuantidadeHorasContrato = periodoCarregamento.QuantidadeHorasContrato,
                RegrasMultaAtrasoRetirada = regraMultaAtrasoRetirada,
                RetiradaNoPeriodo = (dataFinalizacao <= dataLiberacaoCarga.AddHours(periodoCarregamento.QuantidadeHorasContrato)),
                Transportador = carga.Empresa
            };

            repositorioMultaAtrasoRetirada.Inserir(multaAtrasoRetirada);

            if (multaAtrasoRetirada.RetiradaNoPeriodo)
                return;

            int totalCargasRetiradasNoPeriodo = repositorioMultaAtrasoRetirada.BuscarTotalCargasRetiradasNoPeriodo(carga.Codigo, carga.Empresa.Codigo, regraMultaAtrasoRetirada.Codigo, dataLiberacaoCarga);

            if (totalCargasRetiradasNoPeriodo >= periodoCarregamento.QuantidadeCargas)
                return;

            multaAtrasoRetirada.GerarOcorrencia = true;

            repositorioMultaAtrasoRetirada.Atualizar(multaAtrasoRetirada);
            GerarCargaOcorrencia(multaAtrasoRetirada, tipoServicoMultisoftware, clienteMultisoftware);
        }

        public void VerificarGeracaoOcorrencias(TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada repositorioMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.MultaAtrasoRetirada(_unitOfWork);
            List<int> codigosMultaAtrasoRetirada = repositorioMultaAtrasoRetirada.BuscarCodigosParaGeracaoOcorrencias();

            foreach (int codigoMultaAtrasoRetirada in codigosMultaAtrasoRetirada)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada multaAtrasoRetirada = repositorioMultaAtrasoRetirada.BuscarPorCodigo(codigoMultaAtrasoRetirada, auditavel: false);

                try
                {
                    _unitOfWork.Start();

                    GerarCargaOcorrencia(multaAtrasoRetirada, tipoServicoMultisoftware, clienteMultisoftware);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        #endregion Métodos Públicos
    }
}
