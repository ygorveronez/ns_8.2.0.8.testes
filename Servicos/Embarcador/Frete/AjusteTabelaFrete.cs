using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public class AjusteTabelaFrete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private SituacaoItemParametroBaseCalculoTabelaFrete? _situacaoItemAjustado;
        private bool? _manterAlteracaoVigenciaPendente;

        #endregion Atributos

        #region Contrutores

        public AjusteTabelaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void VerificarAjustesPendentesCriacao()
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            Hubs.AjusteTabela hubAjusteTabela = new Hubs.AjusteTabela();

            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustesTabelaFrete = repAjusteTabelaFrete.BuscarParaProcessamentoPorSituacao(SituacaoAjusteTabelaFrete.EmCriacao, 1);

            for (int i = 0, s = ajustesTabelaFrete.Count; i < s; i++)
            {
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = ajustesTabelaFrete[i];

                try
                {
                    GerarAjuste(ajuste);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    ajuste.Situacao = SituacaoAjusteTabelaFrete.ProblemaCriacao;
                }

                repAjusteTabelaFrete.Atualizar(ajuste);

                hubAjusteTabela.InformarAjusteTabelaAtualizado(ajuste.Codigo);
            }
        }

        public void VerificarAjustesPendentesAplicacaoAjuste()
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            Hubs.AjusteTabela hubAjusteTabela = new Hubs.AjusteTabela();

            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustesTabelaFrete = repAjusteTabelaFrete.BuscarParaProcessamentoPorSituacao(SituacaoAjusteTabelaFrete.EmAjuste, 1);

            for (int i = 0, s = ajustesTabelaFrete.Count; i < s; i++)
            {
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = ajustesTabelaFrete[i];

                try
                {
                    AplicarAjuste(ajuste);
                    ajuste = repAjusteTabelaFrete.BuscarPorCodigo(ajuste.Codigo);
                    ajuste.Situacao = SituacaoAjusteTabelaFrete.Pendente;
                    repAjusteTabelaFrete.Atualizar(ajuste);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    _unitOfWork.Rollback();

                    ajuste.Situacao = SituacaoAjusteTabelaFrete.ProblemaAjuste;
                    repAjusteTabelaFrete.Atualizar(ajuste);
                }

                hubAjusteTabela.InformarAjusteTabelaAplicado(ajuste.Codigo);
            }
        }

        public void VerificarAjustesPendentesProcessamento()
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            Hubs.AjusteTabela hubAjusteTabela = new Hubs.AjusteTabela();

            List<int> ajustesTabelaFrete = repAjusteTabelaFrete.BuscarCodigosParaProcessamentoPorSituacao(SituacaoAjusteTabelaFrete.EmProcessamento, 1);
            for (int i = 0, s = ajustesTabelaFrete.Count; i < s; i++)
            {
                try
                {
                    FinalizarAjuste(ajustesTabelaFrete[i]);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    _unitOfWork.Rollback();
                    
                    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo(ajustesTabelaFrete[i]);
                    ajuste.Situacao = SituacaoAjusteTabelaFrete.ProblemaProcessamento;
                    repAjusteTabelaFrete.Atualizar(ajuste);
                }

                hubAjusteTabela.InformarAjusteTabelaAtualizado(ajustesTabelaFrete[i]);
            }
        }

        public void SalvarLog(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFreteLog repLog = new Repositorio.Embarcador.Frete.AjusteTabelaFreteLog(_unitOfWork);

            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteLog log = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteLog();
            log.AjusteTabelaFrete = ajuste;
            log.Data = DateTime.Now;
            log.Situacao = ajuste.Situacao.Value;
            log.Usuario = usuario;

            repLog.Inserir(log);
        }

        public void AdicionarParamentrosPadroesTabela(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBaseCalculoTabelaFrete)
        {
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.TipoCarga)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in tabelaFrete.TiposCarga)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();
                    item.CodigoObjeto = tipoDeCarga.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.TipoCarga;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.ModeloTracao)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloDeTracao in tabelaFrete.ModelosTracao)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = modeloDeTracao.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.ModeloTracao;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.ModeloReboque)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloDeReboque in tabelaFrete.ModelosReboque)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = modeloDeReboque.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.ModeloReboque;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.ComponenteFrete)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFrete in tabelaFrete.Componentes)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = componenteFrete.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.ComponenteFrete;

                    if (componenteFrete.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
                        item.TipoValor = TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                    else
                        item.TipoValor = TipoCampoValorTabelaFrete.AumentoValor;

                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.NumeroEntrega)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete numeroEntrega in tabelaFrete.NumeroEntregas)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = numeroEntrega.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.NumeroEntrega;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Pallets)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete pallet in tabelaFrete.Pallets)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = pallet.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.Pallets;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Peso)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete peso in tabelaFrete.PesosTransportados)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = peso.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.Peso;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }


            if (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Distancia)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete distancia in tabelaFrete.Distancias)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();

                    item.CodigoObjeto = distancia.Codigo;
                    item.ParametroBaseCalculo = parametroBaseCalculoTabelaFrete;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFreteCliente : null;
                    item.TipoObjeto = TipoParametroBaseTabelaFrete.Distancia;
                    item.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    repItemParametroBaseCalculoTabelaFrete.Inserir(item);
                }
            }
        }

        public void VincularRotasSemParar()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(new List<TipoIntegracao> { TipoIntegracao.SemParar }, null);

            if (tiposIntegracao.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> listaTabelaFreteCliente = repositorioTabelaFreteCliente.BuscarNaoVinculados(10);

            List<Dominio.Entidades.Localidade> localidades = new List<Dominio.Entidades.Localidade>();

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente in listaTabelaFreteCliente)
            {
                if (tabelaFreteCliente.Origens != null)
                    tabelaFreteCliente.Origens.ToList().ForEach(x => localidades.Add(x));
                
                if (tabelaFreteCliente.Destinos != null)
                    tabelaFreteCliente.Destinos.ToList().ForEach(x => localidades.Add(x));
            }
            
            localidades = localidades.Distinct().ToList();

            List<Dominio.Entidades.RotaFrete> listaRotaFrete = repositorioRotaFrete.BuscarPorLocalidades(localidades.Select(x => x.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabela in listaTabelaFreteCliente)
            {
                if (tabela.Origens == null || tabela.Origens.Count == 0)
                    continue;

                if (tabela.Destinos == null || tabela.Destinos.Count == 0)
                    continue;

                List<int> codigosLocalidadesOrigem = tabela.Origens?.Select(x => x.Codigo).ToList();
                List<int> codigosLocalidadesDestinos = tabela.Destinos?.Select(x => x.Codigo).ToList();

                Dominio.Entidades.RotaFrete rotaFrete = listaRotaFrete.Where(obj => obj.Localidades.Any(x => codigosLocalidadesDestinos.Contains(x.Localidade.Codigo)) && obj.LocalidadesOrigem.Any(x => codigosLocalidadesOrigem.Contains(x.Codigo))).FirstOrDefault();

                if (rotaFrete == null)
                    continue;

                tabela.RotaFrete = rotaFrete;
                
            }

            listaTabelaFreteCliente.ForEach(x => x.VinculadoSemParar = true);
            listaTabelaFreteCliente.ForEach(x => repositorioTabelaFreteCliente.Atualizar(x));
        }

        public void FinalizarAlteracoesVigenciaPendente(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool alteracaoVigenciaAprovada)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteAlteracaoVigenciaPendente = repositorioTabelaFreteCliente.BuscarPorAlteracaoVigenciaPendente(tabelaFreteCliente.Codigo);

            if (tabelaFreteClienteAlteracaoVigenciaPendente == null)
                return;

            if (alteracaoVigenciaAprovada)
            {
                Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete novaVigencia = tabelaFreteClienteAlteracaoVigenciaPendente.Vigencia;

                tabelaFreteClienteAlteracaoVigenciaPendente.Tipo = TipoTabelaFreteCliente.Calculo;
                tabelaFreteClienteAlteracaoVigenciaPendente.Vigencia = tabelaFreteCliente.Vigencia;
                tabelaFreteCliente.Vigencia = novaVigencia;

                FinalizarVigencia(tabelaFreteClienteAlteracaoVigenciaPendente, novaVigencia);
            }
            else
                tabelaFreteClienteAlteracaoVigenciaPendente.Tipo = TipoTabelaFreteCliente.AlteracaoVigenciaReprovada;

            new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(_unitOfWork).Confirmar(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente);
            repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            repositorioTabelaFreteCliente.Atualizar(tabelaFreteClienteAlteracaoVigenciaPendente);
        }

        #endregion

        #region Métodos Privados

        private void GerarAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete)
        {
            int codigoTabelaFrete = ajusteTabelaFrete.TabelaFrete.Codigo;
            int codigoVigencia = ajusteTabelaFrete.Vigencia.Codigo;
            int codigoContratoTransporteFrete = ajusteTabelaFrete.ContratoTransporteFrete?.Codigo ?? 0;
            List<double> remetente = (from o in ajusteTabelaFrete.Remetentes select o.CPF_CNPJ).ToList();
            List<double> destinatario = (from o in ajusteTabelaFrete.Destinatarios select o.CPF_CNPJ).ToList();
            List<double> tomador = (from o in ajusteTabelaFrete.Tomadores select o.CPF_CNPJ).ToList();
            List<int> codigoLocalidadeOrigem = (from o in ajusteTabelaFrete.Origens select o.Codigo).ToList();
            List<int> codigoLocalidadeDestino = (from o in ajusteTabelaFrete.Destinos select o.Codigo).ToList();
            List<int> codigoRegiaoDestino = (from o in ajusteTabelaFrete.RegioesDestino select o.Codigo).ToList();
            List<int> codigoTipoCarga = (from o in ajusteTabelaFrete.TiposCarga select o.Codigo).ToList();
            List<int> codigoModeloTracao = (from o in ajusteTabelaFrete.ModelosTracao select o.Codigo).ToList();
            List<int> codigoModeloReboque = (from o in ajusteTabelaFrete.ModelosReboque select o.Codigo).ToList();
            List<int> codigoTipoOperacao = (from o in ajusteTabelaFrete.TiposOperacao select o.Codigo).ToList();
            List<int> codigoEmpresa = (from o in ajusteTabelaFrete.Empresas select o.Codigo).ToList();
            List<int> codigoRotaFreteDestino = (from o in ajusteTabelaFrete.RotasFreteDestino select o.Codigo).ToList();
            List<int> codigoRotaFreteOrigem = (from o in ajusteTabelaFrete.RotasFreteOrigem select o.Codigo).ToList();
            List<int> codigoCanalVenda = (from o in ajusteTabelaFrete.CanaisVenda select o.Codigo).ToList();
            List<int> codigoCanalEntrega = (from o in ajusteTabelaFrete.CanaisEntrega select o.Codigo).ToList();
            List<string> estadoDestino = (from o in ajusteTabelaFrete.UFsDestinos select o.Sigla).ToList();
            List<string> estadoOrigem = (from o in ajusteTabelaFrete.UFsOrigem select o.Sigla).ToList();
            TipoPagamentoEmissao? tipoPagamento = ajusteTabelaFrete.TipoPagamento;
            bool tabelaComCargaRealizada = ajusteTabelaFrete.ApenasTabelasComCargasRealizadas;
            bool apenasRegistrosComValor = ajusteTabelaFrete.SomenteRegistrosComValores;
            bool utilizarBuscaNasLocalidadesPorEstadoOrigem = ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoOrigem;
            bool utilizarBuscaNasLocalidadesPorEstadoDestino = ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoDestino;

            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente servicoMensagemAlertaTabelaFreteCliente = new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            List<int> codigos = repositorioTabelaFreteCliente.BuscarCodigoTabelasParaAjuste(0, ajusteTabelaFrete.Codigo, codigoTabelaFrete, codigoVigencia, remetente, destinatario, tomador, codigoLocalidadeOrigem, estadoOrigem, codigoLocalidadeDestino, estadoDestino, codigoRegiaoDestino, codigoTipoOperacao, codigoRotaFreteOrigem, codigoRotaFreteDestino, codigoEmpresa, codigoContratoTransporteFrete, tipoPagamento, tabelaComCargaRealizada, utilizarBuscaNasLocalidadesPorEstadoOrigem, utilizarBuscaNasLocalidadesPorEstadoDestino, apenasRegistrosComValor, codigoCanalVenda, codigoCanalEntrega);           
            int totalTabelasFreteClienteDuplicadasParaAjuste = 0;

            foreach (int codigo in codigos)
            {
                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = repositorioTabelaFreteCliente.BuscarPorCodigo(codigo);

                if (!servicoMensagemAlertaTabelaFreteCliente.IsMensagemSemConfirmacao(tabelaFrete, TipoMensagemAlerta.AjusteTabelaFreteCliente))
                {
                    servicoTabelaFreteCliente.DuplicarParaAjuste(tabelaFrete, ajusteTabelaFrete, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga, true);
                    totalTabelasFreteClienteDuplicadasParaAjuste++;
                }

                _unitOfWork.CommitChanges();
                _unitOfWork.FlushAndClear();
            }

            ajusteTabelaFrete.Situacao = (totalTabelasFreteClienteDuplicadasParaAjuste == 0) ? SituacaoAjusteTabelaFrete.ProblemaCriacao : SituacaoAjusteTabelaFrete.Pendente;
        }

        private void FinalizarAjuste(int codigoAjusteTabelaFrete)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete repTempoEtapaAjusteTabelaFrete = new Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete(_unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new TabelaFreteClienteIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigoAjusteTabelaFrete);

            // Quando a flag estive ativa e o ajuste possuir uma nova vigencia
            // O sistema ira busca todas tabelas cliente da configuração para replicar apenas a vigencia
            if (configuracao.ReplicarAjusteTabelaFreteTodasTabelas && (ajusteTabelaFrete.NovaVigencia != null || ajusteTabelaFrete.NovaVigenciaIndefinida.HasValue))
            {
                int codigoTabelaFrete = ajusteTabelaFrete.TabelaFrete.Codigo;
                int codigoVigencia = ajusteTabelaFrete.Vigencia.Codigo;
                TipoPagamentoEmissao? tipoPagamento = ajusteTabelaFrete.TipoPagamento;
                bool tabelaComCargaRealizada = ajusteTabelaFrete.ApenasTabelasComCargasRealizadas;
                bool utilizarBuscaNasLocalidadesPorEstadoOrigem = ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoOrigem;
                bool utilizarBuscaNasLocalidadesPorEstadoDestino = ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoDestino;
                List<int> codigoModeloTracao = (from o in ajusteTabelaFrete.ModelosTracao select o.Codigo).ToList();
                List<int> codigoModeloReboque = (from o in ajusteTabelaFrete.ModelosReboque select o.Codigo).ToList();
                List<int> codigoTipoCarga = (from o in ajusteTabelaFrete.TiposCarga select o.Codigo).ToList();

                ReplicarTodasTabelasDaVigencia(ajusteTabelaFrete.Codigo, codigoTabelaFrete, codigoVigencia, tipoPagamento, tabelaComCargaRealizada, utilizarBuscaNasLocalidadesPorEstadoOrigem, utilizarBuscaNasLocalidadesPorEstadoDestino, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga);
                ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigoAjusteTabelaFrete);
            }

            List<int> codigosTabelaAjuste = repTabelaFreteCliente.BuscarCodigosTabelasPorAjuste(ajusteTabelaFrete.Codigo);
            AplicarAlteracoesFinalizacao(ajusteTabelaFrete, codigosTabelaAjuste);

            ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigoAjusteTabelaFrete);
            ajusteTabelaFrete.DataAjuste = DateTime.Now;
            ajusteTabelaFrete.Situacao = ajusteTabelaFrete.SituacaoAposProcessamento.HasValue ? ajusteTabelaFrete.SituacaoAposProcessamento.Value : SituacaoAjusteTabelaFrete.Finalizado;

            repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);
            servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(ajusteTabelaFrete);

            // Cria da nova etapa 
            Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete tempoEtapa = new Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete
            {
                AjusteTabelaFrete = ajusteTabelaFrete,
                Etapa = ajusteTabelaFrete.Etapa,
                Entrada = DateTime.Now,
                Saida = null
            };
            repTempoEtapaAjusteTabelaFrete.Inserir(tempoEtapa);

            if (ajusteTabelaFrete.UsuarioAprovador != null)
                SalvarLog(ajusteTabelaFrete, ajusteTabelaFrete.UsuarioAprovador);
            else
                SalvarLog(ajusteTabelaFrete, ajusteTabelaFrete.Criador);
        }

        private void FinalizarVigencia(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete novaVigencia)
        {
            bool finalizarVigenciaOriginal = (
                tabelaFreteCliente.TabelaFrete.PermitirVigenciasSobrepostas &&
                (novaVigencia.DataInicial > tabelaFreteCliente.Vigencia.DataInicial) &&
                (!tabelaFreteCliente.Vigencia.DataFinal.HasValue || (tabelaFreteCliente.Vigencia.DataFinal.Value >= novaVigencia.DataInicial))
            );

            if (!finalizarVigenciaOriginal)
                return;
            
            DateTime dataFinalVigencia = novaVigencia.DataInicial.AddDays(-1);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigenciaFechada = repositorioVigenciaTabelaFrete.BuscarCompativel(tabelaFreteCliente.Vigencia.DataInicial, dataFinalVigencia, tabelaFreteCliente.TabelaFrete.Codigo, tabelaFreteCliente.Vigencia.Empresa?.Codigo ?? 0);

            if (vigenciaFechada == null)
            {
                vigenciaFechada = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete
                {
                    DataInicial = tabelaFreteCliente.Vigencia.DataInicial,
                    DataFinal = dataFinalVigencia,
                    Empresa = tabelaFreteCliente.Vigencia.Empresa,
                    TabelaFrete = tabelaFreteCliente.TabelaFrete
                };

                repositorioVigenciaTabelaFrete.Inserir(vigenciaFechada);
            }

            tabelaFreteCliente.Vigencia = vigenciaFechada;

            repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
        }

        private void AplicarAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores repAjusteTabelaFreteProcessamentoValores = new Repositorio.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores(_unitOfWork);

            // Obtem valores do processamento
            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores processamentoValores = repAjusteTabelaFreteProcessamentoValores.BuscarPorAjuste(ajusteTabelaFrete.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete>>(processamentoValores.ItensAjuste);

            // Busca apenas tabelas cujo o ultimo processamento de valores antecede ao processamento em questao
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasAjuste = repTabelaFreteCliente.BuscarTabelasPorAjusteAplicacaoAjuste(ajusteTabelaFrete.Codigo, processamentoValores.Data);
            List<int> codigos = (from obj in tabelasAjuste select obj.Codigo).ToList();
            foreach (int codigo in codigos)
            {
                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaAjuste = repTabelaFreteCliente.BuscarPorCodigo(codigo);

                if (ajusteTabelaFrete.NovaVigencia != null) tabelaAjuste.Vigencia = ajusteTabelaFrete.NovaVigencia;

                repTabelaFreteCliente.Atualizar(tabelaAjuste);
                AjustarItensTabelaFrete(ajusteTabelaFrete.TabelaFrete, tabelaAjuste, itensAjuste);
                tabelaAjuste.DataProcessamentoValores = processamentoValores.Data;

                _unitOfWork.CommitChanges();
                _unitOfWork.FlushAndClear();
            }

            SalvarLog(ajusteTabelaFrete, processamentoValores.Usuario);
        }

        private void AplicarAlteracoesFinalizacao(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete, List<int> codigosTabelaAjuste)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete novaVigenciaIndefinida = ObterNovaVigenciaIndefinida(ajusteTabelaFrete.Vigencia, ajusteTabelaFrete.NovaVigenciaIndefinida);

            for (int i = 0, s = codigosTabelaAjuste.Count; i < s; i++)
            {
                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaAjuste = repositorioTabelaFreteCliente.BuscarPorCodigo(codigosTabelaAjuste[i]);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal = tabelaAjuste.TabelaOriginaria;

                servicoTabelaFreteCliente.DuplicarParaHistoricoAlteracao(tabelaOriginal, ajusteTabelaFrete.Criador);

                if (novaVigenciaIndefinida != null)
                    tabelaAjuste.Vigencia = novaVigenciaIndefinida;

                if (tabelaAjuste.Vigencia.Codigo != tabelaOriginal.Vigencia.Codigo)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginalAlteracaoVigencia = servicoTabelaFreteCliente.DuplicarParaAlteracaoVigencia(tabelaOriginal);

                    if (ajusteTabelaFrete.NovaVigencia != null)
                    {
                        if (IsManterAlteracaoVigenciaPendente())
                        {
                            tabelaOriginalAlteracaoVigencia.Tipo = TipoTabelaFreteCliente.AlteracaoVigenciaPendente;
                            tabelaOriginalAlteracaoVigencia.Vigencia = tabelaAjuste.Vigencia;

                            new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(_unitOfWork).Adicionar(tabelaOriginal, TipoMensagemAlerta.AjusteTabelaFreteCliente, "Ajuste dos valores da tabela de frete aguardando retorno");
                        }
                        else
                        {
                            tabelaOriginal.Vigencia = tabelaAjuste.Vigencia;

                            FinalizarVigencia(tabelaOriginalAlteracaoVigencia, tabelaAjuste.Vigencia);
                        }
                    }
                    else
                        tabelaOriginal.Vigencia = tabelaAjuste.Vigencia;
                }

                AplicarAlteracoesValores(tabelaOriginal, tabelaAjuste);

                repositorioTabelaFreteCliente.Atualizar(tabelaOriginal);
                repositorioTabelaFreteCliente.Atualizar(tabelaAjuste);

                _unitOfWork.CommitChanges();
                _unitOfWork.FlushAndClear();
            }
        }

        private void AplicarAlteracoesValores(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteOriginal, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteAjuste)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(_unitOfWork);
            SituacaoItemParametroBaseCalculoTabelaFrete situacaoItemAjustado = ObterSituacaoItemAjustado();

            if (tabelaFreteClienteOriginal.TabelaFrete.ParametroBase.HasValue)
            {
                List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosBaseCalculo = repositorioParametroBaseCalculoTabelaFrete.BuscarPorTabelaFrete(tabelaFreteClienteAjuste.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBase in parametrosBaseCalculo)
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroAjustar = repositorioParametroBaseCalculoTabelaFrete.Buscar(tabelaFreteClienteOriginal.Codigo, parametroBase.CodigoObjeto);

                    if (parametroAjustar == null)
                        continue;

                    List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensBaseCalculo = repositorioItemParametroBaseCalculoTabelaFrete.BuscarPorParametro(parametroBase.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase in itensBaseCalculo)
                    {
                        if (itemParametroBase.Valor == itemParametroBase.ValorOriginal)
                            continue;

                        Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAjustar = repositorioItemParametroBaseCalculoTabelaFrete.BuscarPorCodigoObjetoETipoItem(0, parametroAjustar.Codigo, itemParametroBase.CodigoObjeto, itemParametroBase.TipoObjeto);
                        
                        if (itemAjustar == null)
                            continue;

                        itemAjustar.PendenteIntegracao = true;
                        itemAjustar.ValorOriginal = itemAjustar.Valor;
                        itemAjustar.Valor = itemParametroBase.Valor;
                        itemAjustar.Situacao = situacaoItemAjustado;

                        repositorioItemParametroBaseCalculoTabelaFrete.Atualizar(itemAjustar);
                    }

                    parametroAjustar.ValorMaximoOriginal = (parametroAjustar.ValorMaximo != parametroBase.ValorMaximo) ? parametroAjustar.ValorMaximo : parametroAjustar.ValorMaximoOriginal;
                    parametroAjustar.ValorMaximo = parametroBase.ValorMaximo;
                    parametroAjustar.ValorMinimoGarantidoOriginal = (parametroAjustar.ValorMinimoGarantido != parametroBase.ValorMinimoGarantido) ? parametroAjustar.ValorMinimoGarantido : parametroAjustar.ValorMinimoGarantidoOriginal;
                    parametroAjustar.ValorMinimoGarantido = parametroBase.ValorMinimoGarantido;
                    parametroAjustar.ValorBaseOriginal = (parametroAjustar.ValorBase != parametroBase.ValorBase) ? parametroAjustar.ValorBase : parametroAjustar.ValorBaseOriginal;
                    parametroAjustar.ValorBase = parametroBase.ValorBase;
                    parametroAjustar.ValorAjudanteExcedenteOriginal = (parametroAjustar.ValorAjudanteExcedente != parametroBase.ValorAjudanteExcedente) ? parametroAjustar.ValorAjudanteExcedente : parametroAjustar.ValorAjudanteExcedenteOriginal;
                    parametroAjustar.ValorAjudanteExcedente = parametroBase.ValorAjudanteExcedente;
                    parametroAjustar.ValorEntregaExcedenteOriginal = (parametroAjustar.ValorEntregaExcedente != parametroBase.ValorEntregaExcedente) ? parametroAjustar.ValorEntregaExcedente : parametroAjustar.ValorEntregaExcedenteOriginal;
                    parametroAjustar.ValorEntregaExcedente = parametroBase.ValorEntregaExcedente;
                    parametroAjustar.ValorPalletExcedenteOriginal = (parametroAjustar.ValorPalletExcedente != parametroBase.ValorPalletExcedente) ? parametroAjustar.ValorPalletExcedente : parametroAjustar.ValorPalletExcedenteOriginal;
                    parametroAjustar.ValorPalletExcedente = parametroBase.ValorPalletExcedente;
                    parametroAjustar.ValorPesoExcedenteOriginal = (parametroAjustar.ValorPesoExcedente != parametroBase.ValorPesoExcedente) ? parametroAjustar.ValorPesoExcedente : parametroAjustar.ValorPesoExcedenteOriginal;
                    parametroAjustar.ValorPesoExcedente = parametroBase.ValorPesoExcedente;
                    parametroAjustar.ValorQuilometragemExcedenteOriginal = (parametroAjustar.ValorQuilometragemExcedente != parametroBase.ValorQuilometragemExcedente) ? parametroAjustar.ValorQuilometragemExcedente : parametroAjustar.ValorQuilometragemExcedenteOriginal;
                    parametroAjustar.ValorQuilometragemExcedente = parametroBase.ValorQuilometragemExcedente;

                    repositorioParametroBaseCalculoTabelaFrete.Atualizar(parametroAjustar);
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itemParametroBaseCalculoTabelaFrete = repositorioItemParametroBaseCalculoTabelaFrete.BuscarPorTabelaFrete(tabelaFreteClienteAjuste.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase in itemParametroBaseCalculoTabelaFrete)
                {
                    if (itemParametroBase.Valor == itemParametroBase.ValorOriginal)
                        continue;

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAjustar = repositorioItemParametroBaseCalculoTabelaFrete.BuscarPorCodigoObjetoETipoItem(tabelaFreteClienteOriginal.Codigo, 0, itemParametroBase.CodigoObjeto, itemParametroBase.TipoObjeto);
                    
                    itemAjustar.PendenteIntegracao = true;
                    itemAjustar.ValorOriginal = itemAjustar.Valor;
                    itemAjustar.Valor = itemParametroBase.Valor;
                    itemAjustar.Situacao = situacaoItemAjustado;

                    repositorioItemParametroBaseCalculoTabelaFrete.Atualizar(itemAjustar);

                    if (itemParametroBase.TipoObjeto == TipoParametroBaseTabelaFrete.ComponenteFrete)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabela = repositorioComponenteFreteTabelaFrete.BuscarPorCodigo(itemParametroBase.CodigoObjeto);

                        if ((componenteTabela != null) && (componenteTabela.TipoCalculo == TipoCalculoComponenteTabelaFrete.ValorFixo))
                        {
                            componenteTabela.ValorFormula = itemParametroBase.Valor;
                            repositorioComponenteFreteTabelaFrete.Atualizar(componenteTabela);
                        }
                    }
                }

                tabelaFreteClienteOriginal.ValorMaximoOriginal = (tabelaFreteClienteOriginal.ValorMaximo != tabelaFreteClienteAjuste.ValorMaximo) ? tabelaFreteClienteOriginal.ValorMaximo : tabelaFreteClienteOriginal.ValorMaximoOriginal;
                tabelaFreteClienteOriginal.ValorMaximo = tabelaFreteClienteAjuste.ValorMaximo;
                tabelaFreteClienteOriginal.ValorMinimoGarantidoOriginal = (tabelaFreteClienteOriginal.ValorMinimoGarantido != tabelaFreteClienteAjuste.ValorMinimoGarantido) ? tabelaFreteClienteOriginal.ValorMinimoGarantido : tabelaFreteClienteOriginal.ValorMinimoGarantidoOriginal;
                tabelaFreteClienteOriginal.ValorMinimoGarantido = tabelaFreteClienteAjuste.ValorMinimoGarantido;
                tabelaFreteClienteOriginal.ValorBaseOriginal = (tabelaFreteClienteOriginal.ValorBase != tabelaFreteClienteAjuste.ValorBase) ? tabelaFreteClienteOriginal.ValorBase : tabelaFreteClienteOriginal.ValorBaseOriginal;
                tabelaFreteClienteOriginal.ValorBase = tabelaFreteClienteAjuste.ValorBase;
                tabelaFreteClienteOriginal.ValorAjudanteExcedenteOriginal = (tabelaFreteClienteOriginal.ValorAjudanteExcedente != tabelaFreteClienteAjuste.ValorAjudanteExcedente) ? tabelaFreteClienteOriginal.ValorAjudanteExcedente : tabelaFreteClienteOriginal.ValorAjudanteExcedenteOriginal;
                tabelaFreteClienteOriginal.ValorAjudanteExcedente = tabelaFreteClienteAjuste.ValorAjudanteExcedente;
                tabelaFreteClienteOriginal.ValorEntregaExcedenteOriginal = (tabelaFreteClienteOriginal.ValorEntregaExcedente != tabelaFreteClienteAjuste.ValorEntregaExcedente) ? tabelaFreteClienteOriginal.ValorEntregaExcedente : tabelaFreteClienteOriginal.ValorEntregaExcedenteOriginal;
                tabelaFreteClienteOriginal.ValorEntregaExcedente = tabelaFreteClienteAjuste.ValorEntregaExcedente;
                tabelaFreteClienteOriginal.ValorPalletExcedenteOriginal = (tabelaFreteClienteOriginal.ValorPalletExcedente != tabelaFreteClienteAjuste.ValorPalletExcedente) ? tabelaFreteClienteOriginal.ValorPalletExcedente : tabelaFreteClienteOriginal.ValorPalletExcedenteOriginal;
                tabelaFreteClienteOriginal.ValorPalletExcedente = tabelaFreteClienteAjuste.ValorPalletExcedente;
                tabelaFreteClienteOriginal.ValorPesoExcedenteOriginal = (tabelaFreteClienteOriginal.ValorPesoExcedente != tabelaFreteClienteAjuste.ValorPesoExcedente) ? tabelaFreteClienteOriginal.ValorPesoExcedente : tabelaFreteClienteOriginal.ValorPesoExcedenteOriginal;
                tabelaFreteClienteOriginal.ValorPesoExcedente = tabelaFreteClienteAjuste.ValorPesoExcedente;
                tabelaFreteClienteOriginal.ValorQuilometragemExcedenteOriginal = (tabelaFreteClienteOriginal.ValorQuilometragemExcedente != tabelaFreteClienteAjuste.ValorQuilometragemExcedente) ? tabelaFreteClienteOriginal.ValorQuilometragemExcedente : tabelaFreteClienteOriginal.ValorQuilometragemExcedenteOriginal;
                tabelaFreteClienteOriginal.ValorQuilometragemExcedente = tabelaFreteClienteAjuste.ValorQuilometragemExcedente;
            }
        }

        private void ReplicarTodasTabelasDaVigencia(int codigoAjusteTabelaFrete, int codigoTabelaFrete, int codigoVigencia, TipoPagamentoEmissao? tipoPagamento, bool tabelaComCargaRealizada, bool utilizarBuscaNasLocalidadesPorEstadoOrigem, bool utilizarBuscaNasLocalidadesPorEstadoDestino, List<int> codigoModeloTracao, List<int> codigoModeloReboque, List<int> codigoTipoCarga)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            List<int> codigoTabelas = repTabelaFreteCliente.BuscarCodigoTabelasParaAjusteSemFiltro(codigoAjusteTabelaFrete, codigoTabelaFrete, codigoVigencia, tipoPagamento, tabelaComCargaRealizada, utilizarBuscaNasLocalidadesPorEstadoOrigem, utilizarBuscaNasLocalidadesPorEstadoDestino);

            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            for (int i = 0, s = codigoTabelas.Count; i < s; i++)
            {
                _unitOfWork.Start();
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = repTabelaFreteCliente.BuscarPorCodigo(codigoTabelas[i]);
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigoAjusteTabelaFrete);

                servicoTabelaFreteCliente.DuplicarParaAjuste(tabelaFrete, ajusteTabelaFrete, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga, false);

                _unitOfWork.CommitChanges();
                _unitOfWork.FlushAndClear();
            }
        }

        private void AjustarItensTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste)
        {
            if (tabelaFrete.ParametroBase.HasValue)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBaseCalculo in tabelaFreteCliente.ParametrosBaseCalculo)
                {
                    AjustarItensParametroBaseCalculo(parametroBaseCalculo.ItensBaseCalculo.ToList(), itensAjuste);
                    AjustarValoresGeraisParametroTabelaFrete(itensAjuste, parametroBaseCalculo);
                }
            }
            else
            {
                AjustarItensParametroBaseCalculo(tabelaFreteCliente.ItensBaseCalculo.ToList(), itensAjuste);
                AjustarValoresGeraisTabelaFrete(itensAjuste, tabelaFreteCliente);
            }
        }

        private void AjustarItensParametroBaseCalculo(List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensParametroBaseCalculoTabela, List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste)
        {
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjuste in itensAjuste)
            {
                decimal valor = itemAjuste.Valor;

                if (valor <= 0)
                    continue;

                TipoParametroAjusteTabelaFrete tipoObjetoAjuste = itemAjuste.Tipo;
                TipoParametroBaseTabelaFrete? tipoParametroBaseTabelaFrete = ObterTipoParametroTabelaFrete(tipoObjetoAjuste);
                TipoCampoValorTabelaFrete tipoOperacao = itemAjuste.TipoOperacao;
                bool aumenta = itemAjuste.Aumenta;
                int codigo = itemAjuste.Codigo;

                if (tipoParametroBaseTabelaFrete.HasValue)
                {
                    List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensParametroBaseCalculoAjustar = null;

                    if (tipoParametroBaseTabelaFrete != TipoParametroBaseTabelaFrete.ComponenteFrete)
                        itensParametroBaseCalculoAjustar = (from obj in itensParametroBaseCalculoTabela where obj.TipoObjeto == tipoParametroBaseTabelaFrete select obj).ToList();
                    else
                        itensParametroBaseCalculoAjustar = (from obj in itensParametroBaseCalculoTabela where obj.TipoObjeto == tipoParametroBaseTabelaFrete && obj.CodigoObjeto == codigo select obj).ToList();

                    foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBaseCalculo in itensParametroBaseCalculoAjustar)
                    {
                        itemParametroBaseCalculo.Valor = ObterValorComAjusteAplicado(itemAjuste, itemParametroBaseCalculo.Valor, itemParametroBaseCalculo.TipoValor);

                        repItemParametroBaseCalculo.Atualizar(itemParametroBaseCalculo);
                    }
                }
            }
        }

        private void AjustarValoresGeraisParametroTabelaFrete(List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste, Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBaseCalculoTabelaFrete)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorMaximo = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorMaximo).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorMinimo = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorMinimo).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorBase = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorBase).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorAjudanteExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.AjudanteExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorDistanciaExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.DistanciaExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorNumeroEntregaExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.NumeroEntregaExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorPalletExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.PalletExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorPesoExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.PesoExcedente).FirstOrDefault();

            if (itemAjusteValorMaximo != null)
                parametroBaseCalculoTabelaFrete.ValorMaximo = ObterValorComAjusteAplicado(itemAjusteValorMaximo, parametroBaseCalculoTabelaFrete.ValorMaximo, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorMinimo != null)
                parametroBaseCalculoTabelaFrete.ValorMinimoGarantido = ObterValorComAjusteAplicado(itemAjusteValorMinimo, parametroBaseCalculoTabelaFrete.ValorMinimoGarantido, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorBase != null)
                parametroBaseCalculoTabelaFrete.ValorBase = ObterValorComAjusteAplicado(itemAjusteValorBase, parametroBaseCalculoTabelaFrete.ValorBase, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorAjudanteExcedente != null)
                parametroBaseCalculoTabelaFrete.ValorAjudanteExcedente = ObterValorComAjusteAplicado(itemAjusteValorAjudanteExcedente, parametroBaseCalculoTabelaFrete.ValorAjudanteExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorDistanciaExcedente != null)
                parametroBaseCalculoTabelaFrete.ValorQuilometragemExcedente = ObterValorComAjusteAplicado(itemAjusteValorDistanciaExcedente, parametroBaseCalculoTabelaFrete.ValorQuilometragemExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorNumeroEntregaExcedente != null)
                parametroBaseCalculoTabelaFrete.ValorEntregaExcedente = ObterValorComAjusteAplicado(itemAjusteValorNumeroEntregaExcedente, parametroBaseCalculoTabelaFrete.ValorEntregaExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorPalletExcedente != null)
                parametroBaseCalculoTabelaFrete.ValorPalletExcedente = ObterValorComAjusteAplicado(itemAjusteValorPalletExcedente, parametroBaseCalculoTabelaFrete.ValorPalletExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorPesoExcedente != null)
                parametroBaseCalculoTabelaFrete.ValorPesoExcedente = ObterValorComAjusteAplicado(itemAjusteValorPesoExcedente, parametroBaseCalculoTabelaFrete.ValorPesoExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            repParametroBaseCalculoTabelaFrete.Atualizar(parametroBaseCalculoTabelaFrete);
        }

        private void AjustarValoresGeraisTabelaFrete(List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorMaximo = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorMaximo).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorMinimo = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorMinimo).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorBase = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.ValorBase).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorAjudanteExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.AjudanteExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorDistanciaExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.DistanciaExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorNumeroEntregaExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.NumeroEntregaExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorPalletExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.PalletExcedente).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteValorPesoExcedente = itensAjuste.Where(o => o.Tipo == TipoParametroAjusteTabelaFrete.PesoExcedente).FirstOrDefault();

            if (itemAjusteValorMaximo != null && itemAjusteValorMaximo.Valor > 0m)
                tabelaFreteCliente.ValorMaximo = ObterValorComAjusteAplicado(itemAjusteValorMaximo, tabelaFreteCliente.ValorMaximo, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorMinimo != null && itemAjusteValorMinimo.Valor > 0m)
                tabelaFreteCliente.ValorMinimoGarantido = ObterValorComAjusteAplicado(itemAjusteValorMinimo, tabelaFreteCliente.ValorMinimoGarantido, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorBase != null && itemAjusteValorBase.Valor > 0m)
                tabelaFreteCliente.ValorBase = ObterValorComAjusteAplicado(itemAjusteValorBase, tabelaFreteCliente.ValorBase, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorAjudanteExcedente != null)
                tabelaFreteCliente.ValorAjudanteExcedente = ObterValorComAjusteAplicado(itemAjusteValorAjudanteExcedente, tabelaFreteCliente.ValorAjudanteExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorDistanciaExcedente != null)
                tabelaFreteCliente.ValorQuilometragemExcedente = ObterValorComAjusteAplicado(itemAjusteValorDistanciaExcedente, tabelaFreteCliente.ValorQuilometragemExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorNumeroEntregaExcedente != null)
                tabelaFreteCliente.ValorEntregaExcedente = ObterValorComAjusteAplicado(itemAjusteValorNumeroEntregaExcedente, tabelaFreteCliente.ValorEntregaExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorPalletExcedente != null)
                tabelaFreteCliente.ValorPalletExcedente = ObterValorComAjusteAplicado(itemAjusteValorPalletExcedente, tabelaFreteCliente.ValorPalletExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            if (itemAjusteValorPesoExcedente != null)
                tabelaFreteCliente.ValorPesoExcedente = ObterValorComAjusteAplicado(itemAjusteValorPesoExcedente, tabelaFreteCliente.ValorPesoExcedente, TipoCampoValorTabelaFrete.ValorFixo);

            repTabelaFreteCliente.Atualizar(tabelaFreteCliente);
        }

        private bool IsManterAlteracaoVigenciaPendente()
        {
            if (!_manterAlteracaoVigenciaPendente.HasValue)
                _manterAlteracaoVigenciaPendente = new TabelaFreteClienteIntegracao(_unitOfWork).PossuiIntegracaoControlaSituacaoItens();

            return _manterAlteracaoVigenciaPendente.Value;
        }

        private Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete ObterNovaVigenciaIndefinida(Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia, DateTime? novaVigenciaIndefinida)
        {
            if (!novaVigenciaIndefinida.HasValue)
                return null;

            _unitOfWork.Start();

            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(_unitOfWork);

            if (!vigencia.DataFinal.HasValue)
            {
                vigencia.DataFinal = novaVigenciaIndefinida.Value.AddDays(-1);
                repositorioVigenciaTabelaFrete.Atualizar(vigencia);
            }

            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete novaVigencia = repositorioVigenciaTabelaFrete.BuscarPorDataInicio(novaVigenciaIndefinida.Value, vigencia.TabelaFrete.Codigo);

            if (novaVigencia == null)
            {
                novaVigencia = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete
                {
                    DataInicial = novaVigenciaIndefinida.Value,
                    DataFinal = null,
                    TabelaFrete = vigencia.TabelaFrete
                };

                repositorioVigenciaTabelaFrete.Inserir(novaVigencia);
            }

            _unitOfWork.CommitChanges();

            return novaVigencia;
        }

        private decimal ObterValorComAjusteAplicado(Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete itemAjusteTabelaFrete, decimal valorParaAjuste, TipoCampoValorTabelaFrete tipoValorDoCampoExistente)
        {
            decimal valor = 0m;

            switch (itemAjusteTabelaFrete.TipoOperacao)
            {
                case TipoCampoValorTabelaFrete.AumentoPercentual:
                    if (tipoValorDoCampoExistente == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal ||
                        tipoValorDoCampoExistente == TipoCampoValorTabelaFrete.AumentoPercentual)
                    {
                        valor = itemAjusteTabelaFrete.Valor;
                    }
                    else
                    {
                        if (itemAjusteTabelaFrete.Aumenta)
                            valor = (valorParaAjuste + (valorParaAjuste * (itemAjusteTabelaFrete.Valor / 100)));
                        else
                            valor = (valorParaAjuste - (valorParaAjuste * (itemAjusteTabelaFrete.Valor / 100)));

                        if (itemAjusteTabelaFrete.Arredondar)
                            valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
                    }
                    break;
                case TipoCampoValorTabelaFrete.AumentoValor:
                    if (tipoValorDoCampoExistente == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal ||
                        tipoValorDoCampoExistente == TipoCampoValorTabelaFrete.AumentoPercentual)
                        valor = itemAjusteTabelaFrete.Valor;
                    else
                    {
                        if (itemAjusteTabelaFrete.Aumenta)
                            valor = valorParaAjuste + itemAjusteTabelaFrete.Valor;
                        else
                            valor = valorParaAjuste - itemAjusteTabelaFrete.Valor;

                        if (itemAjusteTabelaFrete.Arredondar)
                            valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
                    }
                    break;
                case TipoCampoValorTabelaFrete.ValorFixo:
                    valor = itemAjusteTabelaFrete.Valor;
                    break;
                default:
                    valor = valorParaAjuste;
                    break;
            }

            return valor;
        }

        private TipoParametroBaseTabelaFrete? ObterTipoParametroTabelaFrete(TipoParametroAjusteTabelaFrete tipoParametroAjusteTabelaFrete)
        {
            switch (tipoParametroAjusteTabelaFrete)
            {
                case TipoParametroAjusteTabelaFrete.TipoCarga:
                    return TipoParametroBaseTabelaFrete.TipoCarga;
                case TipoParametroAjusteTabelaFrete.ModeloReboque:
                    return TipoParametroBaseTabelaFrete.ModeloReboque;
                case TipoParametroAjusteTabelaFrete.ModeloTracao:
                    return TipoParametroBaseTabelaFrete.ModeloTracao;
                case TipoParametroAjusteTabelaFrete.ComponenteFrete:
                    return TipoParametroBaseTabelaFrete.ComponenteFrete;
                case TipoParametroAjusteTabelaFrete.NumeroEntrega:
                    return TipoParametroBaseTabelaFrete.NumeroEntrega;
                case TipoParametroAjusteTabelaFrete.TipoEmbalagem:
                    return TipoParametroBaseTabelaFrete.TipoEmbalagem;
                case TipoParametroAjusteTabelaFrete.Peso:
                    return TipoParametroBaseTabelaFrete.Peso;
                case TipoParametroAjusteTabelaFrete.Distancia:
                    return TipoParametroBaseTabelaFrete.Distancia;
                case TipoParametroAjusteTabelaFrete.Rota:
                    return TipoParametroBaseTabelaFrete.Rota;
                case TipoParametroAjusteTabelaFrete.ParametrosOcorrencia:
                    return TipoParametroBaseTabelaFrete.ParametrosOcorrencia;
                case TipoParametroAjusteTabelaFrete.Pallets:
                    return TipoParametroBaseTabelaFrete.Pallets;
                case TipoParametroAjusteTabelaFrete.Tempo:
                    return TipoParametroBaseTabelaFrete.Tempo;
                case TipoParametroAjusteTabelaFrete.Ajudante:
                    return TipoParametroBaseTabelaFrete.Ajudante;
                case TipoParametroAjusteTabelaFrete.ValorFreteLiquido:
                    return TipoParametroBaseTabelaFrete.ValorFreteLiquido;
                default:
                    return null;
            }
        }

        private SituacaoItemParametroBaseCalculoTabelaFrete ObterSituacaoItemAjustado()
        {
            if (!_situacaoItemAjustado.HasValue)
            {
                if (new TabelaFreteClienteIntegracao(_unitOfWork).PossuiIntegracaoControlaSituacaoItens())
                    _situacaoItemAjustado = SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoIntegracao;
                else
                    _situacaoItemAjustado = SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;
            }

            return _situacaoItemAjustado.Value;
        }

        #endregion
    }
}
