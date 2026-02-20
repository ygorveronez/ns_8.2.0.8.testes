using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using OfficeOpenXml;

namespace Servicos.Embarcador.Importacao
{
    public sealed class ImportacaoTabelaFrete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete _configuracaoTabelaFrete;
        private Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade _destino;
        private Dominio.Entidades.Empresa _empresa;
        private Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade _origem;
        private Dominio.Entidades.Embarcador.Frete.TabelaFrete _tabelaFrete;
        private Dominio.Entidades.Embarcador.Pedidos.TipoOperacao _tipoOperacao;
        private Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete _vigencia;
        private List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete> _listaParametrosBaseTabelaFrete;
        private bool? _permitirVincularFilialCliente;
        private bool? _validarPermissaoSolicitarAprovacao;

        #endregion Atributos

        #region Construtores

        public ImportacaoTabelaFrete(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public ImportacaoTabelaFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void CarregarParametrosBase(Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro()
            {
                CodigoDestino = importacaoTabelaFrete.Destino?.Codigo ?? 0,
                CodigoOrigem = importacaoTabelaFrete.Origem?.Codigo ?? 0,
                CodigoTabelaFrete = importacaoTabelaFrete.TabelaFrete?.Codigo ?? 0,
                CodigoTipoOperacao = importacaoTabelaFrete.TipoOperacao?.Codigo ?? 0,
                CodigoVigencia = importacaoTabelaFrete.Vigencia?.Codigo ?? 0,
                IndiceColunaParametroBase = importacaoTabelaFrete.ColunaParametroBase ?? 0
            };

            CarregarParametrosBase(parametrosImportacao);
        }

        private void CarregarParametrosBase(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);

            _tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(parametrosImportacao.CodigoTabelaFrete) ?? throw new ServicoException("Tabela de frete não encontrada.");
            _vigencia = repositorioVigencia.BuscarPorCodigo(parametrosImportacao.CodigoVigencia) ?? throw new ServicoException("Vigência não encontrada.");
            _tipoOperacao = (parametrosImportacao.CodigoTipoOperacao > 0) ? repositorioTipoOperacao.BuscarPorCodigo(parametrosImportacao.CodigoTipoOperacao) : null;
            _origem = (parametrosImportacao.CodigoOrigem > 0) ? repositorioLocalidade.BuscarPorCodigoParaImportacaoTabelaFrete(parametrosImportacao.CodigoOrigem) : null;
            _destino = (parametrosImportacao.CodigoDestino > 0) ? repositorioLocalidade.BuscarPorCodigoParaImportacaoTabelaFrete(parametrosImportacao.CodigoDestino) : null;

            if (parametrosImportacao.IndiceColunaParametroBase > 0)
            {
                if (_tabelaFrete.ParametroBase != TipoParametroBaseTabelaFrete.ModeloReboque)
                    throw new ServicoException("Informação da Coluna para parametro base disponível apenas para o Modelos de Reboque.");

                if ((_tabelaFrete.ModelosReboque == null) || (_tabelaFrete.ModelosReboque.Count == 0))
                    throw new ServicoException("Tabela de frete sem modelos de reboques configurado.");

                _listaParametrosBaseTabelaFrete = _tabelaFrete.ModelosReboque.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete()
                {
                    Codigo = o.Codigo,
                    Descricao = o.Descricao,
                    CodigoIntegracao = o.CodigoIntegracao
                }).ToList();
            }
            else
                _listaParametrosBaseTabelaFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete>();
        }

        private int Importar(int indiceLinha, Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao, ExcelWorksheet planilha, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repTabelaFreteClienteFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(_unitOfWork);

            Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(_unitOfWork, ObterConfiguracaoTabelaFrete());
            Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);

            List<Dominio.Entidades.Cliente> fronteiras = null;
            List<Dominio.Entidades.Cliente> clientesDestino = null;
            List<Dominio.Entidades.Cliente> clientesOrigem = null;
            List<Dominio.Entidades.RotaFrete> rotasDestino = null;
            List<Dominio.Entidades.RotaFrete> rotasOrigem = null;
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = null;
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem = null;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixasCepOrigem = null;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixasCepDestino = null;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> localidadesOrigem = null;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> localidadesDestino = null;
            List<Dominio.Entidades.Estado> estadosOrigem = new List<Dominio.Entidades.Estado>();
            List<Dominio.Entidades.Estado> estadosDestino = null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = null;
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = null;
            Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = null;
            Dominio.Entidades.Empresa empresaPorTabelaFreteCliente = null;
            Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransportador = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga grupoCarga = TipoGrupoCarga.Nenhum;
            bool gerenciarCapacidade = false;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela? estruturaTabela = null;
            Dominio.Entidades.Cliente tomador = null;
            int codigoTabelaFreteClienteAdicionada = 0;

            if (configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
            {
                List<IList> origens = new List<IList>();
                List<IList> destinos = new List<IList>();

                if (_origem != null)
                {
                    localidadesOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _origem };
                }
                if (parametrosImportacao.IndiceColunaOrigem > 0)
                {
                    string descricaoOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaOrigem].Text;
                    string descricaoEstadoOrigem = (parametrosImportacao.IndiceColunaEstadoOrigem > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoOrigem].Text : string.Empty;

                    localidadesOrigem = ObterLocalidades(descricaoOrigem, descricaoEstadoOrigem, configuracaoTabelaFrete);

                    origens.Add(localidadesOrigem);
                }
                if (localidadesOrigem.Count <= 0 && parametrosImportacao.IndiceColunaEstadoOrigem > 0)
                {
                    string descricaoEstadoOrigem = (parametrosImportacao.IndiceColunaEstadoOrigem > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoOrigem].Text : string.Empty;

                    estadosOrigem = ObterEstados(descricaoEstadoOrigem, configuracaoTabelaFrete);

                    origens.Add(estadosOrigem);
                }
                if (parametrosImportacao.IndiceColunaClienteOrigem > 0)
                {
                    string cpfCnpjClientesOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteOrigem].Text;

                    clientesOrigem = ObterClientes(cpfCnpjClientesOrigem, configuracaoTabelaFrete);

                    origens.Add(clientesOrigem);
                }
                if (parametrosImportacao.IndiceColunaCepOrigem > 0)
                {
                    string faixaCepOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepOrigem].Text;

                    faixasCepOrigem = ObterFaixasCep(faixaCepOrigem, configuracaoTabelaFrete);

                    origens.Add(faixasCepOrigem);
                }
                if (parametrosImportacao.IndiceColunaRotaOrigem > 0)
                {
                    string codigoIntegracaoRotaOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaOrigem].Text;

                    rotasOrigem = ObterRotas(codigoIntegracaoRotaOrigem, configuracaoTabelaFrete);

                    origens.Add(rotasOrigem);
                }
                if (parametrosImportacao.IndiceColunaRegiaoOrigem > 0)
                {
                    string codigoIntegracaoRegiaoOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoOrigem].Text;

                    regioesOrigem = ObterRegioes(codigoIntegracaoRegiaoOrigem, configuracaoTabelaFrete);

                    origens.Add(regioesOrigem);
                }

                ValidarOrigensOuDestinos(origens, "Origem");

                if (_destino != null)
                {
                    localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _destino };
                }
                if (parametrosImportacao.IndiceColunaDestino > 0)
                {
                    string descricaoDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaDestino].Text;
                    string descricaoEstadoDestino = (parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                    localidadesDestino = ObterLocalidades(descricaoDestino, descricaoEstadoDestino, configuracaoTabelaFrete);

                    destinos.Add(localidadesDestino);
                }
                if (localidadesDestino.Count <= 0 && parametrosImportacao.IndiceColunaEstadoDestino > 0)
                {
                    string descricaoEstadoDestino = (parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                    estadosDestino = ObterEstados(descricaoEstadoDestino, configuracaoTabelaFrete);

                    destinos.Add(estadosDestino);
                }
                if (parametrosImportacao.IndiceColunaClienteDestino > 0)
                {
                    string cpfCnpjClientesDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteDestino].Text;

                    clientesDestino = ObterClientes(cpfCnpjClientesDestino, configuracaoTabelaFrete);

                    destinos.Add(clientesDestino);
                }
                if (parametrosImportacao.IndiceColunaCepDestino > 0)
                {
                    string faixaCepDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestino].Text;
                    string diasUteis = (parametrosImportacao.IndiceColunaCepDestinoDiasUteis > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestinoDiasUteis].Text : string.Empty;

                    faixasCepDestino = ObterFaixasCep(faixaCepDestino, diasUteis, configuracaoTabelaFrete);

                    destinos.Add(faixasCepDestino);
                }
                if (parametrosImportacao.IndiceColunaRotaDestino > 0)
                {
                    string codigoIntegracaoRotaDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaDestino].Text;

                    rotasDestino = ObterRotas(codigoIntegracaoRotaDestino, configuracaoTabelaFrete);

                    destinos.Add(rotasDestino);
                }
                if (parametrosImportacao.IndiceColunaRegiaoDestino > 0)
                {
                    string codigoIntegracaoRegiaoDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoDestino].Text;

                    regioesDestino = ObterRegioes(codigoIntegracaoRegiaoDestino, configuracaoTabelaFrete);

                    destinos.Add(regioesDestino);
                }

                ValidarOrigensOuDestinos(destinos, "Destino");
            }
            else
            {

                if (_origem != null)
                {
                    localidadesOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _origem };
                }
                else if (parametrosImportacao.IndiceColunaOrigem > 0)
                {
                    string descricaoOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaOrigem].Text;
                    string descricaoEstadoOrigem = (parametrosImportacao.IndiceColunaEstadoOrigem > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoOrigem].Text : string.Empty;

                    localidadesOrigem = ObterLocalidades(descricaoOrigem, descricaoEstadoOrigem, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaOrigem <= 0 && parametrosImportacao.IndiceColunaEstadoOrigem > 0)
                {
                    string descricaoEstadoOrigem = (parametrosImportacao.IndiceColunaEstadoOrigem > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoOrigem].Text : string.Empty;

                    estadosOrigem = ObterEstados(descricaoEstadoOrigem, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaClienteOrigem > 0)
                {
                    string cpfCnpjClientesOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteOrigem].Text;

                    clientesOrigem = ObterClientes(cpfCnpjClientesOrigem, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaCepOrigem > 0)
                {
                    string faixaCepOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepOrigem].Text;

                    faixasCepOrigem = ObterFaixasCep(faixaCepOrigem, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaRotaOrigem > 0)
                {
                    string codigoIntegracaoRotaOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaOrigem].Text;

                    rotasOrigem = ObterRotas(codigoIntegracaoRotaOrigem, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaRegiaoOrigem > 0)
                {
                    string codigoIntegracaoRegiaoOrigem = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoOrigem].Text;

                    regioesOrigem = ObterRegioes(codigoIntegracaoRegiaoOrigem, configuracaoTabelaFrete);
                }

                if (_destino != null)
                {
                    localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _destino };
                }
                else if (parametrosImportacao.IndiceColunaDestino > 0)
                {
                    string descricaoDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaDestino].Text;
                    string descricaoEstadoDestino = (parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                    localidadesDestino = ObterLocalidades(descricaoDestino, descricaoEstadoDestino, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaEstadoDestino > 0)
                {
                    string descricaoEstadoDestino = (parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                    estadosDestino = ObterEstados(descricaoEstadoDestino, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaClienteDestino > 0)
                {
                    string cpfCnpjClientesDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteDestino].Text;

                    clientesDestino = ObterClientes(cpfCnpjClientesDestino, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaCepDestino > 0)
                {
                    string faixaCepDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestino].Text;
                    string diasUteis = (parametrosImportacao.IndiceColunaCepDestinoDiasUteis > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestinoDiasUteis].Text : string.Empty;

                    faixasCepDestino = ObterFaixasCep(faixaCepDestino, diasUteis, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaRotaDestino > 0)
                {
                    string codigoIntegracaoRotaDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaDestino].Text;

                    rotasDestino = ObterRotas(codigoIntegracaoRotaDestino, configuracaoTabelaFrete);
                }
                else if (parametrosImportacao.IndiceColunaRegiaoDestino > 0)
                {
                    string codigoIntegracaoRegiaoDestino = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoDestino].Text;

                    regioesDestino = ObterRegioes(codigoIntegracaoRegiaoDestino, configuracaoTabelaFrete);
                }
            }

            if (parametrosImportacao.IndiceColunaTransportador > 0)
            {
                string cnpjEmpresa = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaTransportador].Text;

                empresaPorTabelaFreteCliente = ObterEmpresa(cnpjEmpresa);
            }

            if (parametrosImportacao.IndiceColunaFronteira > 0)
            {
                string cpfCnpjFronteiras = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaFronteira].Text;

                fronteiras = ObterClientes(cpfCnpjFronteiras, configuracaoTabelaFrete, true);
            }

            if (parametrosImportacao.IndiceColunaCanalEntrega > 0)
            {
                string codigoIntegracaoCanalEntrega = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCanalEntrega].Text;

                canalEntrega = ObterCanalEntrega(codigoIntegracaoCanalEntrega);
            }

            if (parametrosImportacao.IndiceColunaTipoOperacao > 0)
            {
                string codigoIntegracaoTipoOperacao = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaTipoOperacao].Text;

                tiposOperacao = ObterTipoOperacao(codigoIntegracaoTipoOperacao);
            }

            if (parametrosImportacao.IndiceColunaTipoDeCarga > 0)
            {
                string codigoIntegracaoTipoDeCarga = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaTipoDeCarga].Text;

                tiposDeCarga = ObterTipoDeCarga(codigoIntegracaoTipoDeCarga);
            }

            if (parametrosImportacao.IndiceColunaContratoTransportador > 0)
            {
                string codigoContratoTransportador = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaContratoTransportador].Text;

                contratoTransportador = ObterContratoTransporteFrete(codigoContratoTransportador);

                if (contratoTransportador.Codigo > 0 && (!_vigencia.DataFinal.HasValue || contratoTransportador.DataInicio > _vigencia.DataInicial || contratoTransportador.DataInicio > _vigencia.DataFinal || contratoTransportador.DataFim < _vigencia.DataInicial || contratoTransportador.DataFim < _vigencia.DataFinal))
                    throw new ServicoException($"A vigência informada não pode estar fora do período do contrato do transportador (De {contratoTransportador.DataInicio.ToString("dd/MM/yyyy")} até {contratoTransportador.DataFim.ToString("dd/MM/yyyy")}).");

            }

            if (parametrosImportacao.IndiceColunaGrupoCarga > 0)
                grupoCarga = (planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaGrupoCarga].Text).ToEnum<TipoGrupoCarga>();

            if (parametrosImportacao.IndiceColunaGerenciarCapacidade > 0)
                gerenciarCapacidade = ((planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaGrupoCarga].Text).ToEnum<SimNao>()) == SimNao.Sim;

            if (parametrosImportacao.IndiceColunaEstruturaTabela > 0)
                estruturaTabela = (planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaGrupoCarga].Text).ToEnum<EstruturaTabela>();

            int leadtime = 0;
            if (parametrosImportacao.IndiceColunaLeadTime > 0)
                int.TryParse(Utilidades.String.OnlyNumbers(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaLeadTime].Text), out leadtime);

            int leadTimeDias = 0;
            if (parametrosImportacao.IndiceColunaLeadTimeDias > 0)
                int.TryParse(Utilidades.String.OnlyNumbers(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaLeadTimeDias].Text), out leadTimeDias);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasEntrega = new List<DiaSemana>();
            if (parametrosImportacao.IndiceColunaSeg > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaSeg].Text))
                diasEntrega.Add(DiaSemana.Segunda);
            if (parametrosImportacao.IndiceColunaTer > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaTer].Text))
                diasEntrega.Add(DiaSemana.Terca);
            if (parametrosImportacao.IndiceColunaQua > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaQua].Text))
                diasEntrega.Add(DiaSemana.Quarta);
            if (parametrosImportacao.IndiceColunaQui > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaQui].Text))
                diasEntrega.Add(DiaSemana.Quinta);
            if (parametrosImportacao.IndiceColunaSex > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaSex].Text))
                diasEntrega.Add(DiaSemana.Sexta);
            if (parametrosImportacao.IndiceColunaSab > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaSab].Text))
                diasEntrega.Add(DiaSemana.Sabado);
            if (parametrosImportacao.IndiceColunaDom > 0 && !string.IsNullOrWhiteSpace(planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaDom].Text))
                diasEntrega.Add(DiaSemana.Domingo);


            if (_tipoOperacao != null)
            {
                if (tiposOperacao == null)
                    tiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                tiposOperacao.Add(_tipoOperacao);
            }

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarTabelaIgualParaImportacao(_tabelaFrete.Codigo, localidadesOrigem, localidadesDestino, estadosOrigem, estadosDestino, clientesOrigem, clientesDestino, faixasCepOrigem, faixasCepDestino, rotasOrigem, rotasDestino, fronteiras, parametrosImportacao.CodigoVigencia, tiposOperacao, parametrosImportacao.CodigoEmpresa, canalEntrega?.Codigo ?? 0, tiposDeCarga);

            _unitOfWork.Start();

            bool adicionarTabelaFreteCliente = (tabelaFreteCliente == null);

            if (adicionarTabelaFreteCliente)
            {
                tabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente()
                {
                    Ativo = true,
                    CodigoIntegracao = (parametrosImportacao.IndiceColunaCodigoIntegracao > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCodigoIntegracao].Text : "",
                    Empresa = empresaPorTabelaFreteCliente ?? _empresa,
                    FreteValidoParaQualquerDestino = parametrosImportacao.FreteValidoParaQualquerDestino,
                    FreteValidoParaQualquerOrigem = parametrosImportacao.FreteValidoParaQualquerOrigem,
                    HerdarInclusaoICMSTabelaFrete = true,
                    IncluirICMSValorFrete = _tabelaFrete.IncluirICMSValorFrete,
                    PercentualICMSIncluir = _tabelaFrete.PercentualICMSIncluir,
                    TabelaFrete = _tabelaFrete,
                    TipoPagamento = parametrosImportacao.TipoPagamento.HasValue ? parametrosImportacao.TipoPagamento.Value : tomador != null ? TipoPagamentoEmissao.Outros : TipoPagamentoEmissao.UsarDaNotaFiscal,
                    Vigencia = _vigencia,
                    Moeda = parametrosImportacao.Moeda ?? MoedaCotacaoBancoCentral.Real,
                    CanalEntrega = canalEntrega,
                    TipoGrupoCarga = grupoCarga,
                    GerenciarCapacidade = gerenciarCapacidade,
                    EstruturaTabela = estruturaTabela,
                };

                if (tiposOperacao?.Count > 0)
                    tabelaFreteCliente.TiposOperacao = tiposOperacao;

                if (tiposDeCarga?.Count > 0)
                    tabelaFreteCliente.TiposCarga = tiposDeCarga;

                if (leadTimeDias > 0)
                    tabelaFreteCliente.LeadTime = leadTimeDias;

                if (contratoTransportador?.Codigo > 0)
                    tabelaFreteCliente.ContratoTransporteFrete = contratoTransportador;

                if (parametrosImportacao.IndiceColunaPrioridadeUso > 0)
                    tabelaFreteCliente.PrioridadeUso = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaPrioridadeUso].Text?.ToNullableInt();

                repositorioTabelaFreteCliente.Inserir(tabelaFreteCliente);

                SalvarClientesOrigem(tabelaFreteCliente, clientesOrigem);
                SalvarLocalidadesOrigem(tabelaFreteCliente, localidadesOrigem);
                SalvarRegioesOrigem(tabelaFreteCliente, regioesOrigem);
                SalvarRotasOrigem(tabelaFreteCliente, rotasOrigem);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = SalvarFaixasCepOrigem(tabelaFreteCliente, faixasCepOrigem);

                SalvarClientesDestino(tabelaFreteCliente, clientesDestino);
                SalvarLocalidadesDestino(tabelaFreteCliente, localidadesDestino);
                SalvarEstadosDestino(tabelaFreteCliente, estadosDestino);
                SalvarRegioesDestino(tabelaFreteCliente, regioesDestino);
                SalvarRotasDestino(tabelaFreteCliente, rotasDestino);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = SalvarFaixasCepDestino(tabelaFreteCliente, faixasCepDestino);

                SalvarFronteiras(tabelaFreteCliente, fronteiras);

                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteExiste = repositorioTabelaFreteCliente.BuscarTabelaComMesmaIncidencia(tabelaFreteCliente, cepsOrigem, cepsDestino);

                if (tabelaFreteClienteExiste != null)
                    throw new ServicoException($"Já existe uma tabela com a mesma incidência (diferente desta tabela) cadastrada: {tabelaFreteClienteExiste.DescricaoOrigem} até {tabelaFreteClienteExiste.DescricaoDestino}");

                servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
                servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);
            }
            else
            {
                if (new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(_unitOfWork).IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente))
                    throw new ServicoException("Não é possível alterar valores da tabela de frete com ajuste aguardando retorno");

                if (configuracaoTabelaFrete?.PermitirInformarLeadTimeTabelaFreteCliente ?? false)
                    repTabelaFreteClienteFrequenciaEntrega.DeletarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
            }

            if (tabelaFreteCliente.ContratoTransporteFrete != null && tabelaFreteCliente.Empresa != null && tabelaFreteCliente.ContratoTransporteFrete.Transportador?.Codigo != tabelaFreteCliente.Empresa?.Codigo)
                throw new ServicoException(Localization.Resources.Fretes.TabelaFreteCliente.NaoEPossivelCadastrarUmaTabelaParaUmClienteComUmTransportadorDiferenteDoContrato);


            if (diasEntrega.Count > 0 && diasEntrega.Count < 7)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana in diasEntrega)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega tabelaFreteClienteFrequenciaEntrega = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega();
                    tabelaFreteClienteFrequenciaEntrega.TabelaFreteCliente = tabelaFreteCliente;
                    tabelaFreteClienteFrequenciaEntrega.DiaSemana = diaSemana;
                    repTabelaFreteClienteFrequenciaEntrega.Inserir(tabelaFreteClienteFrequenciaEntrega);
                }
            }

            if (parametrosImportacao.IndiceColunaLeadTime > 0)
                tabelaFreteCliente.LeadTime = leadtime;

            if (parametrosImportacao.IndiceColunaLeadTimeDias > 0)
                tabelaFreteCliente.LeadTime = leadTimeDias;

            ImportarParametros(indiceLinha, parametrosImportacao, planilha, tabelaFreteCliente);
            ImportarParametrosSaida(indiceLinha, parametrosImportacao, planilha, tabelaFreteCliente);

            bool permitirSolicitarAprovacao = adicionarTabelaFreteCliente || PermitirSolicitarAprovacao(tabelaFreteCliente);

            _unitOfWork.CommitChanges();

            return permitirSolicitarAprovacao ? tabelaFreteCliente.Codigo : 0;
        }

        private void ImportarParametros(int indiceLinha, Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao, ExcelWorksheet planilha, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            string descricaoParametro = (parametrosImportacao.IndiceColunaParametroBase > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaParametroBase].Text : "";

            foreach (dynamic parametro in parametrosImportacao.Parametros)
            {
                string itemCompleto = (string)parametro.ItemParametroBase;
                string[] itemSeparado = itemCompleto.Split('_');
                int codigoObjeto = itemSeparado[1].ToInt();
                string tipoObjeto = itemSeparado[0];
                TipoCampoValorTabelaFrete tipoValor = (TipoCampoValorTabelaFrete)parametro.TipoValor;
                int codigoParametroBase = 0;

                if (!string.IsNullOrWhiteSpace(descricaoParametro))
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete parametrosBaseTabelaFrete = _listaParametrosBaseTabelaFrete.Where(o => o.Descricao == descricaoParametro || o.CodigoIntegracao == descricaoParametro).FirstOrDefault();

                    if (parametrosBaseTabelaFrete == null)
                        throw new ControllerException($"Não foi possível encontrar o parâmetro base na tabela de frete ({descricaoParametro})");

                    codigoParametroBase = parametrosBaseTabelaFrete.Codigo;
                }
                else
                    codigoParametroBase = _tabelaFrete.ParametroBase.HasValue ? (int)parametro.ParametroBase : 0;

                string valor = planilha.Cells[indiceLinha, (int)parametro.Coluna].Text.ObterSomenteCaracteresPermitidos("01234567890.,");

                if (string.IsNullOrWhiteSpace(valor))
                    valor = "0,00";

                decimal valorConvertido = Utilidades.Decimal.Converter(valor);

                if ((valorConvertido <= 0m) && parametrosImportacao.NaoAtualizarValoresZerados)
                    continue;

                if (codigoParametroBase > 0)
                    ImportarParametrosComParametroBase(tabelaFreteCliente, codigoParametroBase, codigoObjeto, tipoObjeto, tipoValor, valorConvertido);
                else
                    ImportarParametrosSemParametroBase(tabelaFreteCliente, codigoObjeto, tipoObjeto, tipoValor, valorConvertido);
            }
        }

        private void ImportarParametros(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoPendentetabelaFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro> ParametrosImportacao)
        {
            string descricaoParametro = ImportacaoPendentetabelaFrete.ColunaParametroBase > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaParametroBase - 1].Valor : ""; // (parametrosImportacao.IndiceColunaParametroBase > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaParametroBase].Text : "";

            foreach (Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametro in ParametrosImportacao)
            {
                if (linha.Colunas.Count < (int)parametro.Coluna)
                    continue;

                string itemCompleto = (string)parametro.ItemParametroBase;
                string[] itemSeparado = itemCompleto.Split('_');
                int codigoObjeto = itemSeparado[1].ToInt();
                string tipoObjeto = itemSeparado[0];
                TipoCampoValorTabelaFrete tipoValor = (TipoCampoValorTabelaFrete)parametro.TipoValor;
                int codigoParametroBase = 0;

                if (!string.IsNullOrWhiteSpace(descricaoParametro))
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosBaseTabelaFrete parametrosBaseTabelaFrete = _listaParametrosBaseTabelaFrete.Where(o => o.Descricao == descricaoParametro || o.CodigoIntegracao == descricaoParametro).FirstOrDefault();

                    if (parametrosBaseTabelaFrete == null)
                        throw new ControllerException($"Não foi possível encontrar o parâmetro base na tabela de frete ({descricaoParametro})");

                    codigoParametroBase = parametrosBaseTabelaFrete.Codigo;
                }
                else
                    codigoParametroBase = _tabelaFrete.ParametroBase.HasValue ? (int)parametro.ParametroBase : 0;

                string valor = linha.Colunas[(int)parametro.Coluna - 1].Valor; //planilha.Cells[indiceLinha, (int)parametro.Coluna].Text.ObterSomenteCaracteresPermitidos("01234567890.,");
                valor = valor.ObterSomenteCaracteresPermitidos("01234567890.,");

                if (string.IsNullOrWhiteSpace(valor))
                    valor = "0,00";

                decimal valorConvertido = Utilidades.Decimal.Converter(valor);

                if ((valorConvertido <= 0m) && ImportacaoPendentetabelaFrete.NaoAtualizarValoresZerados)
                    continue;

                if (codigoParametroBase > 0)
                    ImportarParametrosComParametroBase(tabelaFreteCliente, codigoParametroBase, codigoObjeto, tipoObjeto, tipoValor, valorConvertido);
                else
                    ImportarParametrosSemParametroBase(tabelaFreteCliente, codigoObjeto, tipoObjeto, tipoValor, valorConvertido);
            }
        }

        private void ImportarParametrosComParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, int codigoParametroBase, int codigoObjeto, string tipoObjeto, TipoCampoValorTabelaFrete tipoValor, decimal valor)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBase = ObterParametroBase(tabelaFreteCliente, codigoParametroBase);
            TipoParametroBaseTabelaFrete? tipo = tipoObjeto.ToNullableEnum<TipoParametroBaseTabelaFrete>();

            if (tipo.HasValue)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase = repositorioItemParametroBase.BuscarPorCodigoObjetoETipoItem(tabelaFreteCliente.Codigo, parametroBase.Codigo, codigoObjeto, tipo.Value);

                if (itemParametroBase == null)
                    itemParametroBase = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
                    {
                        CodigoObjeto = codigoObjeto,
                        ParametroBaseCalculo = parametroBase,
                        TipoObjeto = tipo.Value
                    };
                else
                    itemParametroBase.Initialize();

                itemParametroBase.TipoValor = tipoValor;

                servicoTabelaFreteCliente.DefinirValorItem(itemParametroBase, valor);

                if (itemParametroBase.Codigo > 0)
                    repositorioItemParametroBase.Atualizar(itemParametroBase);
                else
                    repositorioItemParametroBase.Inserir(itemParametroBase);
            }
            else if (tipoObjeto == "ValorBase")
            {
                parametroBase.ValorBaseOriginal = (parametroBase.ValorBase != valor) ? parametroBase.ValorBase : parametroBase.ValorBaseOriginal;
                parametroBase.ValorBase = valor;
                repositorioParametro.Atualizar(parametroBase);
            }
            else if (tipoObjeto == "ValorMinimoGarantido")
            {
                parametroBase.ValorMinimoGarantidoOriginal = (parametroBase.ValorMinimoGarantido != valor) ? parametroBase.ValorMinimoGarantido : parametroBase.ValorMinimoGarantidoOriginal;
                parametroBase.ValorMinimoGarantido = valor;
                repositorioParametro.Atualizar(parametroBase);
            }
            else if (tipoObjeto == "EntregaExcedente")
            {
                parametroBase.ValorEntregaExcedenteOriginal = (parametroBase.ValorEntregaExcedente != valor) ? parametroBase.ValorEntregaExcedente : parametroBase.ValorEntregaExcedenteOriginal;
                parametroBase.ValorEntregaExcedente = valor;
                repositorioParametro.Atualizar(parametroBase);
            }
            else if (tipoObjeto == "PesoExcedente")
            {
                parametroBase.ValorPesoExcedenteOriginal = (parametroBase.ValorPesoExcedente != valor) ? parametroBase.ValorPesoExcedente : parametroBase.ValorPesoExcedenteOriginal;
                parametroBase.ValorPesoExcedente = valor;
                repositorioParametro.Atualizar(parametroBase);
            }
        }

        private void ImportarParametrosSemParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, int codigoObjeto, string tipoObjeto, TipoCampoValorTabelaFrete tipoValor, decimal valor)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            TipoParametroBaseTabelaFrete? tipo = tipoObjeto.ToNullableEnum<TipoParametroBaseTabelaFrete>();

            if (tipo.HasValue)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBase = repositorioItemParametroBase.BuscarPorCodigoObjetoETipoItem(tabelaFreteCliente.Codigo, 0, codigoObjeto, tipo.Value);

                if (itemParametroBase == null)
                    itemParametroBase = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete()
                    {
                        CodigoObjeto = codigoObjeto,
                        TabelaFrete = tabelaFreteCliente,
                        TipoObjeto = tipo.Value
                    };
                else
                    itemParametroBase.Initialize();

                itemParametroBase.TipoValor = tipoValor;

                servicoTabelaFreteCliente.DefinirValorItem(itemParametroBase, valor);

                if (itemParametroBase.Codigo > 0)
                    repositorioItemParametroBase.Atualizar(itemParametroBase);
                else
                    repositorioItemParametroBase.Inserir(itemParametroBase);
            }
            else if (tipoObjeto == "ValorBase")
            {
                tabelaFreteCliente.ValorBaseOriginal = (tabelaFreteCliente.ValorBase != valor) ? tabelaFreteCliente.ValorBase : tabelaFreteCliente.ValorBaseOriginal;
                tabelaFreteCliente.ValorBase = valor;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            }
            else if (tipoObjeto == "ValorMinimoGarantido")
            {
                tabelaFreteCliente.ValorMinimoGarantidoOriginal = (tabelaFreteCliente.ValorMinimoGarantido != valor) ? tabelaFreteCliente.ValorMinimoGarantido : tabelaFreteCliente.ValorMinimoGarantidoOriginal;
                tabelaFreteCliente.ValorMinimoGarantido = valor;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            }
            else if (tipoObjeto == "EntregaExcedente")
            {
                tabelaFreteCliente.ValorEntregaExcedenteOriginal = (tabelaFreteCliente.ValorEntregaExcedente != valor) ? tabelaFreteCliente.ValorEntregaExcedente : tabelaFreteCliente.ValorEntregaExcedenteOriginal;
                tabelaFreteCliente.ValorEntregaExcedente = valor;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            }
            else if (tipoObjeto == "PesoExcedente")
            {
                tabelaFreteCliente.ValorPesoExcedenteOriginal = (tabelaFreteCliente.ValorPesoExcedente != valor) ? tabelaFreteCliente.ValorPesoExcedente : tabelaFreteCliente.ValorPesoExcedenteOriginal;
                tabelaFreteCliente.ValorPesoExcedente = valor;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            }
        }

        private void ImportarParametrosSaida(int indiceLinha, Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao, ExcelWorksheet planilha, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            string descricaoParametro = (parametrosImportacao.IndiceColunaParametroBase > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaParametroBase].Text : "";

            if (!string.IsNullOrEmpty(descricaoParametro))
            {
                string percentualRota = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaPercentualRota].Text;
                string quantidadadeEntregas = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaQuantidadeEntregas].Text;
                string capacidadeOTM = planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCapacidadeOTM].Text;

                SalvarModeloVeicularCarga(tabelaFreteCliente, descricaoParametro, percentualRota, quantidadadeEntregas, capacidadeOTM);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete ObterConfiguracaoTabelaFrete()
        {
            if (_configuracaoTabelaFrete == null)
                _configuracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoTabelaFrete;
        }

        private Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete ObterParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, int codigoParametro)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repositorioParametro.Buscar(tabelaFrete.Codigo, codigoParametro);

            if (parametro != null)
                return parametro;

            parametro = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete
            {
                CodigoObjeto = codigoParametro,
                TabelaFrete = tabelaFrete,
            };

            repositorioParametro.Inserir(parametro);

            return parametro;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha Importar(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, int contadorLinha, Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoPendentetabelaFrete, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref int numeroTentativas, List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro> ParametrosImportacao)
        {
            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(_unitOfWork, ObterConfiguracaoTabelaFrete());
                Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);

                List<Dominio.Entidades.Cliente> fronteiras = null;
                List<Dominio.Entidades.Cliente> clientesDestino = null;
                List<Dominio.Entidades.Cliente> clientesOrigem = null;
                List<Dominio.Entidades.RotaFrete> rotasDestino = null;
                List<Dominio.Entidades.RotaFrete> rotasOrigem = null;
                List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = null;
                List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem = null;
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixasCepOrigem = null;
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixasCepDestino = null;
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> localidadesOrigem = null;
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> localidadesDestino = null;
                List<Dominio.Entidades.Estado> estadosDestino = null;
                List<Dominio.Entidades.Estado> estadosOrigem = null;
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = null;
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = null;
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = null;
                Dominio.Entidades.Empresa empresaPorTabelaFreteCliente = null;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = configuracaoEmbarcador.ConfiguracaoTabelaFrete;

                if (configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                {
                    List<IList> origens = new List<IList>();
                    List<IList> destinos = new List<IList>();

                    if (_origem != null)
                    {
                        localidadesOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _origem };
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaOrigem > 0)
                    {
                        string descricaoOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaOrigem - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaOrigem].Text;
                        string descricaoEstadoOrigem = (ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0) ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoOrigem - 1].Valor : string.Empty;

                        localidadesOrigem = ObterLocalidades(descricaoOrigem, descricaoEstadoOrigem, configuracaoTabelaFrete);

                        origens.Add(localidadesOrigem);
                    }
                    if (localidadesOrigem.Count <= 0 && ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0)
                    {
                        string descricaoEstadoOrigem = ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoOrigem - 1].Valor : string.Empty;

                        estadosOrigem = ObterEstados(descricaoEstadoOrigem, configuracaoTabelaFrete);

                        origens.Add(estadosOrigem);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaRemetente > 0)
                    {
                        string cpfCnpjClientesOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRemetente - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteOrigem].Text;

                        clientesOrigem = ObterClientes(cpfCnpjClientesOrigem, configuracaoTabelaFrete);

                        origens.Add(clientesOrigem);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaCEPOrigem > 0)
                    {
                        string faixaCepOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCEPOrigem - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepOrigem].Text;

                        faixasCepOrigem = ObterFaixasCep(faixaCepOrigem, configuracaoTabelaFrete);

                        origens.Add(faixasCepOrigem);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaRotaOrigem > 0)
                    {
                        string codigoIntegracaoRotaOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRotaOrigem - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaOrigem].Text;

                        rotasOrigem = ObterRotas(codigoIntegracaoRotaOrigem, configuracaoTabelaFrete);

                        origens.Add(rotasOrigem);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaRegiaoOrigem > 0)
                    {
                        string codigoIntegracaoRegiaoOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRegiaoOrigem - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoOrigem].Text;

                        regioesOrigem = ObterRegioes(codigoIntegracaoRegiaoOrigem, configuracaoTabelaFrete);

                        origens.Add(regioesOrigem);
                    }

                    ValidarOrigensOuDestinos(origens, "Origem");

                    if (_destino != null)
                    {
                        localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _destino };
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaDestino > 0)
                    {
                        string descricaoDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaDestino - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaDestino].Text;
                        string descricaoEstadoDestino = ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                        localidadesDestino = ObterLocalidades(descricaoDestino, descricaoEstadoDestino, configuracaoTabelaFrete);

                        destinos.Add(localidadesDestino);
                    }
                    if (localidadesDestino.Count <= 0 && ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0)
                    {
                        string descricaoEstadoDestino = ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                        estadosDestino = ObterEstados(descricaoEstadoDestino, configuracaoTabelaFrete);

                        destinos.Add(estadosDestino);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaDestinatario > 0)
                    {
                        string cpfCnpjClientesDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaDestinatario - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteDestino].Text;

                        clientesDestino = ObterClientes(cpfCnpjClientesDestino, configuracaoTabelaFrete);

                        destinos.Add(clientesDestino);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaCEPDestino > 0)
                    {
                        string faixaCepDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCEPDestino - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestino].Text;
                        string diasUteis = ImportacaoPendentetabelaFrete.ColunaPrazoCEPDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaPrazoCEPDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaCepDestinoDiasUteis > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestinoDiasUteis].Text : string.Empty;

                        faixasCepDestino = ObterFaixasCep(faixaCepDestino, diasUteis, configuracaoTabelaFrete);

                        destinos.Add(faixasCepDestino);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaRotaDestino > 0)
                    {
                        string codigoIntegracaoRotaDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRotaDestino - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaDestino].Text;

                        rotasDestino = ObterRotas(codigoIntegracaoRotaDestino, configuracaoTabelaFrete);

                        destinos.Add(rotasDestino);
                    }
                    if (ImportacaoPendentetabelaFrete.ColunaRegiaoDestino > 0)
                    {
                        string codigoIntegracaoRegiaoDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRegiaoDestino - 1].Valor;//planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoDestino].Text;

                        regioesDestino = ObterRegioes(codigoIntegracaoRegiaoDestino, configuracaoTabelaFrete);

                        destinos.Add(regioesOrigem);
                    }

                    ValidarOrigensOuDestinos(destinos, "Destino");
                }
                else
                {
                    if (_origem != null)
                    {
                        localidadesOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _origem };
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaOrigem > 0)
                    {
                        string descricaoOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaOrigem - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaOrigem].Text;
                        string descricaoEstadoOrigem = (ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0) ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoOrigem - 1].Valor : string.Empty;

                        localidadesOrigem = ObterLocalidades(descricaoOrigem, descricaoEstadoOrigem, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaOrigem <= 0 && ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0)
                    {
                        string descricaoEstadoOrigem = ImportacaoPendentetabelaFrete.ColunaEstadoOrigem > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoOrigem - 1].Valor : string.Empty;

                        estadosOrigem = ObterEstados(descricaoEstadoOrigem, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaRemetente > 0)
                    {
                        string cpfCnpjClientesOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRemetente - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteOrigem].Text;

                        clientesOrigem = ObterClientes(cpfCnpjClientesOrigem, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaCEPOrigem > 0)
                    {
                        string faixaCepOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCEPOrigem - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepOrigem].Text;

                        faixasCepOrigem = ObterFaixasCep(faixaCepOrigem, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaRotaOrigem > 0)
                    {
                        string codigoIntegracaoRotaOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRotaOrigem - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaOrigem].Text;

                        rotasOrigem = ObterRotas(codigoIntegracaoRotaOrigem, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaRegiaoOrigem > 0)
                    {
                        string codigoIntegracaoRegiaoOrigem = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRegiaoOrigem - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoOrigem].Text;

                        regioesOrigem = ObterRegioes(codigoIntegracaoRegiaoOrigem, configuracaoTabelaFrete);
                    }

                    if (_destino != null)
                    {
                        localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>() { _destino };
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaDestino > 0)
                    {
                        string descricaoDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaDestino - 1].Valor;  //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaDestino].Text;
                        string descricaoEstadoDestino = ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                        localidadesDestino = ObterLocalidades(descricaoDestino, descricaoEstadoDestino, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaDestino <= 0 && ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0)
                    {
                        string descricaoEstadoDestino = ImportacaoPendentetabelaFrete.ColunaEstadoDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaEstadoDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaEstadoDestino > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaEstadoDestino].Text : string.Empty;

                        estadosDestino = ObterEstados(descricaoEstadoDestino, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaDestinatario > 0)
                    {
                        string cpfCnpjClientesDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaDestinatario - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaClienteDestino].Text;

                        clientesDestino = ObterClientes(cpfCnpjClientesDestino, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaCEPDestino > 0)
                    {
                        string faixaCepDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCEPDestino - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestino].Text;
                        string diasUteis = ImportacaoPendentetabelaFrete.ColunaPrazoCEPDestino > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaPrazoCEPDestino - 1].Valor : string.Empty; //(parametrosImportacao.IndiceColunaCepDestinoDiasUteis > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCepDestinoDiasUteis].Text : string.Empty;

                        faixasCepDestino = ObterFaixasCep(faixaCepDestino, diasUteis, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaRotaDestino > 0)
                    {
                        string codigoIntegracaoRotaDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRotaDestino - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRotaDestino].Text;

                        rotasDestino = ObterRotas(codigoIntegracaoRotaDestino, configuracaoTabelaFrete);
                    }
                    else if (ImportacaoPendentetabelaFrete.ColunaRegiaoDestino > 0)
                    {
                        string codigoIntegracaoRegiaoDestino = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaRegiaoDestino - 1].Valor;//planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaRegiaoDestino].Text;

                        regioesDestino = ObterRegioes(codigoIntegracaoRegiaoDestino, configuracaoTabelaFrete);
                    }
                }

                if (ImportacaoPendentetabelaFrete.ColunaTransportador > 0)
                {
                    string cnpjEmpresa = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaTransportador - 1].Valor; //planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaTransportador].Text;

                    empresaPorTabelaFreteCliente = ObterEmpresa(cnpjEmpresa);
                }

                if (ImportacaoPendentetabelaFrete.ColunaFronteira > 0)
                {
                    string cpfCnpjFronteiras = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaFronteira - 1].Valor;

                    fronteiras = ObterClientes(cpfCnpjFronteiras, configuracaoTabelaFrete, true);
                }

                if (ImportacaoPendentetabelaFrete.ColunaCanalEntrega > 0)
                {
                    string codigoIntegracaoCanalEntrega = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCanalEntrega - 1].Valor;

                    canalEntrega = ObterCanalEntrega(codigoIntegracaoCanalEntrega);
                }

                if (ImportacaoPendentetabelaFrete.ColunaTipoOperacao > 0)
                {
                    string codigoIntegracaoTipoOperacao = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaTipoOperacao - 1].Valor;

                    tiposOperacao = ObterTipoOperacao(codigoIntegracaoTipoOperacao);
                }

                if (ImportacaoPendentetabelaFrete.ColunaTipoDeCarga > 0)
                {
                    string codigoIntegracaoTipoDeCarga = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaTipoDeCarga - 1].Valor;

                    tiposDeCarga = ObterTipoDeCarga(codigoIntegracaoTipoDeCarga);
                }

                int leadTimeDias = 0;
                if (ImportacaoPendentetabelaFrete.ColunaLeadTimeDias > 0)
                    leadTimeDias = linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaLeadTimeDias - 1].Valor;

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = null;

                if (_tipoOperacao != null)
                {
                    if (tiposOperacao == null)
                        tiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                    tiposOperacao.Add(_tipoOperacao);
                }

                if (!ImportacaoPendentetabelaFrete.NaoValidarTabelasExistentes)
                {
                    if (faixasCepOrigem != null && faixasCepOrigem.Count > 1 && faixasCepDestino.Count == 1)
                    {
                        IList<int> tabelasFreteCliente = repositorioTabelaFreteCliente.BuscarTabelaClientePorCEP(_tabelaFrete.Codigo, (ImportacaoPendentetabelaFrete.Vigencia != null ? (int)ImportacaoPendentetabelaFrete.Vigencia?.Codigo : 0), (ImportacaoPendentetabelaFrete.TipoOperacao != null ? (int)ImportacaoPendentetabelaFrete.TipoOperacao?.Codigo : 0), (ImportacaoPendentetabelaFrete.Empresa != null ? (int)ImportacaoPendentetabelaFrete.Empresa?.Codigo : 0), faixasCepOrigem, faixasCepDestino);

                        if (tabelasFreteCliente.Count > 1)
                        {
                            foreach (int codigoTabelaFretecliente in tabelasFreteCliente)
                            {
                                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteInativar = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoTabelaFretecliente);
                                if (tabelaFreteClienteInativar != null)
                                {
                                    tabelaFreteClienteInativar.Ativo = false;
                                    repositorioTabelaFreteCliente.Atualizar(tabelaFreteClienteInativar);
                                }
                            }
                        }
                        else if (tabelasFreteCliente.Count == 1)
                        {
                            tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(tabelasFreteCliente.FirstOrDefault());

                            DeletarFaixaCepOrigem(tabelaFreteCliente.Codigo);
                            DeletarFaixaCepDestino(tabelaFreteCliente.Codigo);

                            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = SalvarFaixasCepOrigem(tabelaFreteCliente, faixasCepOrigem);
                            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = SalvarFaixasCepDestino(tabelaFreteCliente, faixasCepDestino);
                        }
                    }
                    else
                        tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarTabelaIgualParaImportacao(_tabelaFrete.Codigo, localidadesOrigem, localidadesDestino, estadosOrigem, estadosDestino, clientesOrigem, clientesDestino, faixasCepOrigem, faixasCepDestino, rotasOrigem, rotasDestino, fronteiras, (ImportacaoPendentetabelaFrete.Vigencia != null ? (int)ImportacaoPendentetabelaFrete.Vigencia?.Codigo : 0), tiposOperacao/*(ImportacaoPendentetabelaFrete.TipoOperacao != null ? (int)ImportacaoPendentetabelaFrete.TipoOperacao?.Codigo : 0)*/, (ImportacaoPendentetabelaFrete.Empresa != null ? (int)ImportacaoPendentetabelaFrete.Empresa?.Codigo : 0), canalEntrega?.Codigo ?? 0, tiposDeCarga);
                }

                bool adicionarTabelaFreteCliente = (tabelaFreteCliente == null);

                if (adicionarTabelaFreteCliente)
                {
                    tabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente()
                    {
                        Ativo = true,
                        CodigoIntegracao = ImportacaoPendentetabelaFrete.ColunaCodigoIntegracao > 0 ? linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaCodigoIntegracao - 1].Valor : "", //(parametrosImportacao.IndiceColunaCodigoIntegracao > 0) ? planilha.Cells[indiceLinha, parametrosImportacao.IndiceColunaCodigoIntegracao].Text : "",
                        Empresa = empresaPorTabelaFreteCliente ?? _empresa,
                        FreteValidoParaQualquerDestino = ImportacaoPendentetabelaFrete.ValidoParaQualquerDestino,
                        FreteValidoParaQualquerOrigem = ImportacaoPendentetabelaFrete.ValidoParaQualquerOrigem,
                        HerdarInclusaoICMSTabelaFrete = true,
                        IncluirICMSValorFrete = _tabelaFrete.IncluirICMSValorFrete,
                        PercentualICMSIncluir = _tabelaFrete.PercentualICMSIncluir,
                        TabelaFrete = _tabelaFrete,
                        TipoPagamento = ImportacaoPendentetabelaFrete.TipoPagamento ?? TipoPagamentoEmissao.UsarDaNotaFiscal,
                        Vigencia = _vigencia,
                        Moeda = ImportacaoPendentetabelaFrete.Moeda ?? MoedaCotacaoBancoCentral.Real,
                        CanalEntrega = canalEntrega,
                        Quilometragem = ImportacaoPendentetabelaFrete.ColunaKMSistema != null ? ImportacaoPendentetabelaFrete.ColunaKMSistema.Value : 0
                    };

                    if (tiposOperacao?.Count > 0)
                        tabelaFreteCliente.TiposOperacao = tiposOperacao;

                    if (tiposDeCarga?.Count > 0)
                        tabelaFreteCliente.TiposCarga = tiposDeCarga;

                    if (leadTimeDias > 0)
                        tabelaFreteCliente.LeadTime = leadTimeDias;

                    if (ImportacaoPendentetabelaFrete.ColunaPrioridadeUso > 0)
                        tabelaFreteCliente.PrioridadeUso = ((string)linha.Colunas[(int)ImportacaoPendentetabelaFrete.ColunaPrioridadeUso - 1].Valor)?.ToNullableInt();

                    repositorioTabelaFreteCliente.Inserir(tabelaFreteCliente);

                    SalvarClientesOrigem(tabelaFreteCliente, clientesOrigem);
                    SalvarLocalidadesOrigem(tabelaFreteCliente, localidadesOrigem);
                    SalvarRegioesOrigem(tabelaFreteCliente, regioesOrigem);
                    SalvarRotasOrigem(tabelaFreteCliente, rotasOrigem);
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = SalvarFaixasCepOrigem(tabelaFreteCliente, faixasCepOrigem);

                    SalvarClientesDestino(tabelaFreteCliente, clientesDestino);
                    SalvarLocalidadesDestino(tabelaFreteCliente, localidadesDestino);
                    SalvarEstadosDestino(tabelaFreteCliente, estadosDestino);
                    SalvarEstadosOrigem(tabelaFreteCliente, estadosOrigem);
                    SalvarRegioesDestino(tabelaFreteCliente, regioesDestino);
                    SalvarRotasDestino(tabelaFreteCliente, rotasDestino);
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = SalvarFaixasCepDestino(tabelaFreteCliente, faixasCepDestino);

                    SalvarFronteiras(tabelaFreteCliente, fronteiras);

                    repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);

                    //aqui vamos validar, se faz essa validacao ou nao;
                    if (!configuracaoEmbarcador.NaoValidarTabelaFreteMesmaIncidenciaImportacao || !ImportacaoPendentetabelaFrete.NaoValidarTabelasExistentes)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteExiste = repositorioTabelaFreteCliente.BuscarTabelaComMesmaIncidencia(tabelaFreteCliente, cepsOrigem, cepsDestino);

                        if (tabelaFreteClienteExiste != null)
                            return Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha($"Já existe uma tabela com a mesma incidência (diferente desta tabela) cadastrada: {tabelaFreteClienteExiste.DescricaoOrigem} até {tabelaFreteClienteExiste.DescricaoDestino}", contadorLinha);
                    }

                    servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
                    servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);
                }
                else
                {
                    if (new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(_unitOfWork).IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente))
                        throw new ServicoException("Não é possível alterar valores da tabela de frete com ajuste aguardando retorno");
                }

                ImportarParametros(linha, ImportacaoPendentetabelaFrete, tabelaFreteCliente, ParametrosImportacao);

                bool permitirSolicitarAprovacao = adicionarTabelaFreteCliente || PermitirSolicitarAprovacao(tabelaFreteCliente);

                return Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(contadorLinha, tabelaFreteCliente.Codigo, isAlterado: permitirSolicitarAprovacao);
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha($"{excecao.Message} Em Linha: {contadorLinha}", contadorLinha);
            }
            catch (TimeoutException excecao)
            {
                numeroTentativas++;
                Servicos.Log.TratarErro("Time out, ex: " + excecao);
                return Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha($"Time out Em Linha: {contadorLinha}", contadorLinha, istimeOut: (numeroTentativas < 10));
            }
            catch (Exception excecao)
            {
                numeroTentativas++;
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha($"Erro ao ler Linha: {contadorLinha}", contadorLinha, istimeOut: (numeroTentativas < 10));
            }
        }

        private void ValidarOrigensOuDestinos(List<IList> lists, string parametroMsgException)
        {
            int quantidadeColunasPreenchidas = 0;
            foreach (IList list in lists)
            {
                if (list.Count > 0)
                    quantidadeColunasPreenchidas++;
            }

            if (quantidadeColunasPreenchidas == 0)
            {
                throw new ServicoException($"Nenhum tipo de {parametroMsgException} preenchido");
            }
            else if (quantidadeColunasPreenchidas > 1)
            {
                throw new ServicoException($"Não é permitido informar mais de um tipo de {parametroMsgException}");
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> ConverterArquivoemDadosLinha(string token, int indiceInicioLinha, Repositorio.UnitOfWork unitOfWork)
        {
            string arq = ObterArquivoTemporario(token, unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> ListaLinhasArquivo = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>();

            try
            {

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arq)))
                using (ExcelPackage arquivoExcel = new ExcelPackage())
                {
                    arquivoExcel.Load(ms);

                    ExcelWorksheet planilha = arquivoExcel.Workbook.Worksheets.First();
                    for (int indiceLinha = indiceInicioLinha; indiceLinha <= planilha.Dimension.End.Row; indiceLinha++)
                    {

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha novaLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha();
                        List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna> listaColunasLinha = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna>();

                        for (int indiceColuna = 1; indiceColuna <= planilha.Dimension.End.Column; indiceColuna++)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna novaColuna = new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna
                            {
                                Indice = indiceColuna,
                                Valor = planilha.Cells[indiceLinha, indiceColuna].Text
                            };
                            listaColunasLinha.Add(novaColuna);
                        }

                        novaLinha.Colunas = listaColunasLinha;
                        ListaLinhasArquivo.Add(novaLinha);
                    }

                    planilha.Dispose();
                    arquivoExcel.Dispose();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro("Erro processar Arquivo Tabela Frete: " + ex.Message);
            }

            return ListaLinhasArquivo;
        }

        private bool PermitirSolicitarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            if (!ValidarPermissaoSolicitarAprovacao())
                return true;

            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            if (
                (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repositorioItemParametroBaseCalculo.ExistePendenteAprovacaoPorParametrosTabelaFrete(tabelaFreteCliente.Codigo)) ||
                (!tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repositorioItemParametroBaseCalculo.ExistePendenteAprovacaoPorTabelaFrete(tabelaFreteCliente.Codigo))
            )
            {
                if (tabelaFreteCliente.Tipo != TipoTabelaFreteCliente.Alteracao)
                {
                    tabelaFreteCliente.PermitirCalcularFreteEmAlteracao = true;

                    new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork).Atualizar(tabelaFreteCliente);
                }

                return true;
            }

            return false;
        }

        private bool PermitirVincularFilialCliente()
        {
            if (!_permitirVincularFilialCliente.HasValue)
                _permitirVincularFilialCliente = new Repositorio.Cliente(_unitOfWork).ExisteClienteComFiliaisClientesRelacionadas();

            return _permitirVincularFilialCliente.Value;
        }

        private bool ValidarPermissaoSolicitarAprovacao()
        {
            if (!_validarPermissaoSolicitarAprovacao.HasValue)
                _validarPermissaoSolicitarAprovacao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork).ExistePorTipo(TipoIntegracao.LBC);

            return _validarPermissaoSolicitarAprovacao.Value;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Cliente

        private List<Dominio.Entidades.Cliente> ObterClientes(string cpfCnpjClientes, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete, bool fronteira = false)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();
            string[] listaCpfCnpjCliente = cpfCnpjClientes.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < listaCpfCnpjCliente.Length; i++)
            {
                string cpfCnpjCliente = listaCpfCnpjCliente[i].Trim();

                if (string.IsNullOrWhiteSpace(cpfCnpjCliente))
                    continue;

                double cpfCnpj = cpfCnpjCliente.ObterSomenteNumeros().ToDouble();
                Dominio.Entidades.Cliente cliente = null;

                cliente = repositorioCliente.BuscarPorCodigoIntegracao(cpfCnpjCliente); //RIACHUELO primeiro verificar codigo integracao

                if (cliente == null && cpfCnpj > 0d)
                    cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                    throw new ServicoException($"Pessoa não encontrada: {cpfCnpjCliente}");

                if (fronteira && !cliente.FronteiraAlfandega)
                    throw new ServicoException($"Este CPF/CNPJ não é uma fronteira: {cpfCnpjCliente}");

                clientes.Add(cliente);
            }

            if (clientes.Count == 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                throw new ServicoException($"Nenhuma pessoa encontrada com o CPF/CNPJ fornecido: {cpfCnpjClientes}");

            return clientes;
        }

        private void SalvarClientesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Cliente> clientesDestino)
        {
            if ((clientesDestino == null) || (clientesDestino.Count == 0))
                return;

            if (PermitirVincularFilialCliente())
            {
                List<Dominio.Entidades.Cliente> clientesComFiliaisCliente = new Servicos.Cliente().ObterFiliaisClientesRelacionadas(clientesDestino, _unitOfWork);
                List<Dominio.Entidades.Cliente> clientesPai = clientesComFiliaisCliente.Where(cliente => cliente.PossuiFilialCliente).ToList();

                tabelaFreteCliente.ClientesDestino = clientesComFiliaisCliente;
                tabelaFreteCliente.DescricaoDestino = (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", clientesComFiliaisCliente.Select(cliente => cliente.Descricao));
            }
            else
            {
                tabelaFreteCliente.ClientesDestino = clientesDestino;
                tabelaFreteCliente.DescricaoDestino = string.Join(" / ", clientesDestino.Select(cliente => cliente.Descricao));
            }
        }

        private void SalvarClientesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Cliente> clientesOrigem)
        {
            if ((clientesOrigem == null) || (clientesOrigem.Count == 0))
                return;

            if (PermitirVincularFilialCliente())
            {
                List<Dominio.Entidades.Cliente> clientesComFiliaisCliente = new Servicos.Cliente().ObterFiliaisClientesRelacionadas(clientesOrigem, _unitOfWork);
                List<Dominio.Entidades.Cliente> clientesPai = clientesComFiliaisCliente.Where(cliente => cliente.PossuiFilialCliente).ToList();

                tabelaFreteCliente.ClientesOrigem = clientesComFiliaisCliente;
                tabelaFreteCliente.DescricaoOrigem = (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", clientesComFiliaisCliente.Select(cliente => cliente.Descricao));
            }
            else
            {
                tabelaFreteCliente.ClientesOrigem = clientesOrigem;
                tabelaFreteCliente.DescricaoOrigem = string.Join(" / ", clientesOrigem.Select(cliente => cliente.Descricao));
            }
        }

        #endregion Métodos Privados - Cliente

        #region Métodos Privados - Empresa

        private Dominio.Entidades.Empresa ObterEmpresa(string cnpjEmpresa)
        {
            if (string.IsNullOrWhiteSpace(cnpjEmpresa))
                return null;

            string cnpjEmpresaFormatado = cnpjEmpresa.Trim().ObterSomenteNumeros().ToLong().ToString("d14");
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cnpjEmpresaFormatado) ?? throw new ServicoException($"Transportador não encontrado: {cnpjEmpresa.Trim()}.");

            return empresa;
        }

        #endregion Métodos Privados - Empresa

        #region Métodos Privados - Faixas de CEP

        private void ValidarFaixaCepDuplicada(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCepImportar, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> listaFaixaCepImportar)
        {
            for (var i = 0; i < listaFaixaCepImportar.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCepExistente = listaFaixaCepImportar[i];

                if (
                    (faixaCepImportar.CEPInicial >= faixaCepExistente.CEPInicial && faixaCepImportar.CEPInicial <= faixaCepExistente.CEPFinal) ||
                    (faixaCepImportar.CEPFinal >= faixaCepExistente.CEPInicial && faixaCepImportar.CEPFinal <= faixaCepExistente.CEPFinal) ||
                    (faixaCepExistente.CEPInicial >= faixaCepImportar.CEPInicial && faixaCepExistente.CEPInicial <= faixaCepImportar.CEPFinal) ||
                    (faixaCepExistente.CEPFinal >= faixaCepImportar.CEPInicial && faixaCepExistente.CEPFinal <= faixaCepImportar.CEPFinal)
                )
                    throw new ServicoException($"Existem faixas de CEP conflitantes ({faixaCepImportar.Descricao} e {faixaCepExistente.Descricao}).");
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> ObterFaixasCep(string faixaCep, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            return ObterFaixasCep(faixaCep, diasUteis: string.Empty, configuracaoTabelaFrete);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> ObterFaixasCep(string faixaCep, string diasUteis, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> listaFaixaCepImportar = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP>();
            string[] listaFaixaCep = faixaCep.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            int dias = diasUteis.ObterSomenteNumeros().ToInt();

            for (var i = 0; i < listaFaixaCep.Length; i++)
            {
                string faixaCepCompleta = listaFaixaCep[i].Trim();

                if (string.IsNullOrWhiteSpace(faixaCepCompleta))
                    continue;

                string[] faixaCepSeparada = faixaCepCompleta.Split('-');
                Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCepImportar = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP()
                {
                    CEPInicial = faixaCepSeparada[0].Trim().ToInt(),
                    CEPFinal = faixaCepSeparada[1].Trim().ToInt(),
                    DiasUteis = dias
                };

                if (faixaCepImportar.CEPInicial > faixaCepImportar.CEPFinal)
                    throw new ServicoException($"O CEP inicial é maior que o CEP final ({faixaCepCompleta}). Formato de faixa de CEP deve ser 34800000-34800974/34740000-34740970.");

                ValidarFaixaCepDuplicada(faixaCepImportar, listaFaixaCepImportar);

                listaFaixaCepImportar.Add(faixaCepImportar);
            }

            if (listaFaixaCepImportar.Count == 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                throw new ServicoException($"Nenhuma faixa de CEP informada: {faixaCep}.");

            return listaFaixaCepImportar;
        }

        private List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> SalvarFaixasCepDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> listaFaixaCepDestino)
        {
            if ((listaFaixaCepDestino == null) || (listaFaixaCepDestino.Count == 0))
                return null;

            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repositorioTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCep in listaFaixaCepDestino)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino cepDestino = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino()
                {
                    CEPFinal = faixaCep.CEPFinal,
                    CEPInicial = faixaCep.CEPInicial,
                    DiasUteis = faixaCep.DiasUteis,
                    TabelaFreteCliente = tabelaFreteCliente
                };

                repositorioTabelaFreteClienteCEPDestino.Inserir(cepDestino);
                cepsDestino.Add(cepDestino);
            }

            tabelaFreteCliente.DescricaoDestino = string.Join(" / ", listaFaixaCepDestino.Select(o => o.Descricao));

            return cepsDestino;
        }

        private List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> SalvarFaixasCepOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> listaFaixaCepOrigem)
        {
            if ((listaFaixaCepOrigem == null) || (listaFaixaCepOrigem.Count == 0))
                return null;

            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repositorioTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCep in listaFaixaCepOrigem)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem cepOrigem = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem()
                {
                    CEPFinal = faixaCep.CEPFinal,
                    CEPInicial = faixaCep.CEPInicial,
                    TabelaFreteCliente = tabelaFreteCliente
                };

                repositorioTabelaFreteClienteCEPOrigem.Inserir(cepOrigem);
                cepsOrigem.Add(cepOrigem);
            }

            tabelaFreteCliente.DescricaoOrigem = string.Join(" / ", listaFaixaCepOrigem.Select(o => o.Descricao));

            return cepsOrigem;
        }

        private void DeletarFaixaCepOrigem(int codigoTabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repositorioTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(_unitOfWork);
            repositorioTabelaFreteClienteCEPOrigem.DeletarPorTabelaFreteCliente(codigoTabelaFreteCliente);
        }

        private void DeletarFaixaCepDestino(int codigoTabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repositorioTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(_unitOfWork);
            repositorioTabelaFreteClienteCEPDestino.DeletarPorTabelaFreteCliente(codigoTabelaFreteCliente);
        }

        #endregion Métodos Privados - Faixas de CEP

        #region Métodos Privados - Estados

        private List<Dominio.Entidades.Estado> ObterEstados(string estado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            Repositorio.Estado repositorioestado = new Repositorio.Estado(_unitOfWork);
            List<Dominio.Entidades.Estado> listaEstadosImportar = new List<Dominio.Entidades.Estado>();

            string siglaEstado = estado.Trim();

            if (!string.IsNullOrWhiteSpace(siglaEstado))
            {
                Dominio.Entidades.Estado EstadoImportar = repositorioestado.BuscarPorSigla(siglaEstado);
                if (EstadoImportar != null)
                    listaEstadosImportar.Add(EstadoImportar);
                else if (!configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                    throw new ServicoException($"Nenhum estado encontrado com a descrição fornecida: {estado}.");
            }

            return listaEstadosImportar;
        }

        private void SalvarEstadosDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Estado> listaEstadoDestino)
        {
            if ((listaEstadoDestino == null) || (listaEstadoDestino.Count == 0))
                return;

            tabelaFreteCliente.EstadosDestino = listaEstadoDestino;
        }

        private void SalvarEstadosOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Estado> listaEstadoOrigem)
        {
            if (listaEstadoOrigem == null || listaEstadoOrigem.Count == 0)
                return;

            tabelaFreteCliente.EstadosOrigem = listaEstadoOrigem;
            tabelaFreteCliente.DescricaoOrigem = listaEstadoOrigem.Count > 0 ? string.Join(" / ", listaEstadoOrigem.Select(o => o.Nome).ToList()) : string.Empty;
        }

        #endregion Métodos Privados - Estados

        #region Métodos Privados - Localidades

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> ObterLocalidades(string localidade, string estado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> listaLocalidadeImportar = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade>();
            string[] listaLocalidade = Utilidades.String.RemoveDiacritics(localidade).Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < listaLocalidade.Length; i++)
            {
                string descricaoLocalidade = listaLocalidade[i].Trim();

                if (string.IsNullOrWhiteSpace(descricaoLocalidade))
                    continue;

                string siglaEstado = estado.Trim();

                if (string.IsNullOrWhiteSpace(siglaEstado))
                {
                    string[] descricaoLocalidadeSeparada = descricaoLocalidade.Split('-');

                    if (descricaoLocalidadeSeparada.Length <= 1)
                        throw new ServicoException($"Localidade inválida: {descricaoLocalidade}.");

                    siglaEstado = descricaoLocalidadeSeparada[1].Trim();
                    descricaoLocalidade = Utilidades.String.RemoveDiacritics(descricaoLocalidadeSeparada[0]).Trim();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade localidadeImportar = repositorioLocalidade.BuscarPorDescricaoEUfParaImportacaoTabelaFrete(descricaoLocalidade, siglaEstado) ?? throw new ServicoException($"Localidade não encontrada: {descricaoLocalidade}-{siglaEstado}.");

                listaLocalidadeImportar.Add(localidadeImportar);
            }

            if (listaLocalidadeImportar.Count == 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                throw new ServicoException($"Nenhuma localidade encontrada com a descrição fornecida: {localidade}.");

            return listaLocalidadeImportar;
        }

        private void SalvarLocalidadesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> listaLocalidadeDestino)
        {
            if ((listaLocalidadeDestino == null) || (listaLocalidadeDestino.Count == 0))
                return;

            tabelaFreteCliente.Destinos = listaLocalidadeDestino.Select(o => new Dominio.Entidades.Localidade() { Codigo = o.Codigo }).ToList();
            tabelaFreteCliente.DescricaoDestino = string.Join(" / ", listaLocalidadeDestino.Select(o => o.DescricaoCidadeEstado));
        }

        private void SalvarLocalidadesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> listaLocalidadeOrigem)
        {
            if ((listaLocalidadeOrigem == null) || (listaLocalidadeOrigem.Count == 0))
                return;

            tabelaFreteCliente.Origens = listaLocalidadeOrigem.Select(o => new Dominio.Entidades.Localidade() { Codigo = o.Codigo }).ToList();
            tabelaFreteCliente.DescricaoOrigem = string.Join(" / ", listaLocalidadeOrigem.Select(o => o.DescricaoCidadeEstado));
        }

        #endregion Métodos Privados - Localidades

        #region Métodos Privados - Regiões

        private List<Dominio.Entidades.Embarcador.Localidades.Regiao> ObterRegioes(string codigosIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioes = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            string[] listaCodigoIntegracaoRegiao = codigosIntegracao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if (listaCodigoIntegracaoRegiao.Length == 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                throw new ServicoException("Nenhum código de integração de região fornecido");

            for (var i = 0; i < listaCodigoIntegracaoRegiao.Length; i++)
            {
                string codigoIntegracao = listaCodigoIntegracaoRegiao[i].Trim();

                if (string.IsNullOrWhiteSpace(codigoIntegracao))
                    continue;

                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repositorioRegiao.BuscarPorCodigoIntegracao(codigoIntegracao);

                if (regiao == null)
                    throw new ServicoException($"Região não encontrada: {codigoIntegracao}.");

                regioes.Add(regiao);
            }

            if (regioes.Count <= 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
            {
                if (listaCodigoIntegracaoRegiao.Length == 1)
                    throw new ServicoException($"Nenhuma região encontrada com o código de integração fornecido: {codigosIntegracao}");

                throw new ServicoException($"Nenhuma região encontrada com os códigos de integração fornecidos: {codigosIntegracao}");
            }

            return regioes;
        }

        private void SalvarRegioesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino)
        {
            if ((regioesDestino == null) || (regioesDestino.Count == 0))
                return;

            tabelaFreteCliente.RegioesDestino = regioesDestino;
            tabelaFreteCliente.DescricaoDestino = string.Join(" / ", regioesDestino.Select(o => o.Descricao));
        }

        private void SalvarRegioesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem)
        {
            if ((regioesOrigem == null) || (regioesOrigem.Count == 0))
                return;

            tabelaFreteCliente.RegioesOrigem = regioesOrigem;
            tabelaFreteCliente.DescricaoOrigem = string.Join(" / ", regioesOrigem.Select(o => o.Descricao));
        }

        #endregion Métodos Privados - Regiões

        #region Métodos Privados - Rotas

        private List<Dominio.Entidades.RotaFrete> ObterRotas(string codigosIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();
            string[] listaCodigoIntegracaoRotaFrete = codigosIntegracao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            if (listaCodigoIntegracaoRotaFrete.Length == 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
                throw new ServicoException("Nenhum código de integração de rota fornecido.");

            for (var i = 0; i < listaCodigoIntegracaoRotaFrete.Length; i++)
            {
                string codigoIntegracao = listaCodigoIntegracaoRotaFrete[i].Trim();

                if (string.IsNullOrWhiteSpace(codigoIntegracao))
                    continue;

                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigoIntegracao(codigoIntegracao);

                if (rotaFrete == null)
                    throw new ServicoException($"Rota não encontrada: {codigoIntegracao}.");

                rotas.Add(rotaFrete);
            }

            if (rotas.Count <= 0 && !configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo)
            {
                if (listaCodigoIntegracaoRotaFrete.Length == 1)
                    throw new ServicoException($"Nenhuma rota encontrada com o código de integração fornecido: {codigosIntegracao}");

                throw new ServicoException($"Nenhuma rota encontrada com os códigos de integração fornecidos: {codigosIntegracao}");
            }

            return rotas;
        }

        private void SalvarRotasDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.RotaFrete> rotasDestino)
        {
            if ((rotasDestino == null) || (rotasDestino.Count == 0))
                return;

            tabelaFreteCliente.RotasDestino = rotasDestino;
            tabelaFreteCliente.DescricaoDestino = string.Join(" / ", rotasDestino.Select(o => o.Descricao));
        }

        private void SalvarRotasOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.RotaFrete> rotasOrigem)
        {
            if ((rotasOrigem == null) || (rotasOrigem.Count == 0))
                return;

            tabelaFreteCliente.RotasOrigem = rotasOrigem;
            tabelaFreteCliente.DescricaoOrigem = string.Join(" / ", rotasOrigem.Select(o => o.Descricao));
        }

        #endregion Métodos Privados - Rotas

        #region Métodos Privados - Fronteiras

        private void SalvarFronteiras(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Cliente> fronteiras)
        {
            if ((fronteiras == null) || (fronteiras.Count == 0))
                return;

            tabelaFreteCliente.Fronteiras = fronteiras;
        }

        #endregion Métodos Privados - Fronteiras

        #region Métodos Privados - Canal de Entrega

        private Dominio.Entidades.Embarcador.Pedidos.CanalEntrega ObterCanalEntrega(string codigoIntegracaoCanalEntrega)
        {
            if (!string.IsNullOrEmpty(codigoIntegracaoCanalEntrega))
                return null;

            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repositorioCanalEntrega.BuscarPorCodigoIntegracao(codigoIntegracaoCanalEntrega);

            return canalEntrega;
        }

        #endregion Métodos Privados - Canal de Entrega

        #region Métodos Privados - Tipo de Operação

        private List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> ObterTipoOperacao(string codigosIntegracaoTipoOperacao)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (string.IsNullOrWhiteSpace(codigosIntegracaoTipoOperacao))
                return tiposOperacoes;

            string[] listaCodigoIntegracaoTipoOperacao = codigosIntegracaoTipoOperacao.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < listaCodigoIntegracaoTipoOperacao.Length; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(listaCodigoIntegracaoTipoOperacao[i].Trim());

                if (tipoOperacao != null)
                    tiposOperacoes.Add(tipoOperacao);
            }

            return tiposOperacoes;
        }

        #endregion Métodos Privados - Tipo de Operação

        #region Métodos Privados - Tipo de Carga

        private List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> ObterTipoDeCarga(string codigosIntegracaoTipoDeCarga)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoOperacao = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            if (string.IsNullOrWhiteSpace(codigosIntegracaoTipoDeCarga))
                return tiposDeCarga;

            string[] listaCodigoIntegracaoTipoDeCarga = codigosIntegracaoTipoDeCarga.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < listaCodigoIntegracaoTipoDeCarga.Length; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoOperacao.BuscarPorCodigoEmbarcador(listaCodigoIntegracaoTipoDeCarga[i].Trim());

                if (tipoDeCarga != null)
                    tiposDeCarga.Add(tipoDeCarga);
            }

            return tiposDeCarga;
        }

        #endregion Métodos Privados - Tipo de Carga

        #region Métodos Privados - Contrato Transportador

        private Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete ObterContratoTransporteFrete(string codigoContrato)
        {
            if (string.IsNullOrEmpty(codigoContrato.ObterSomenteNumeros()))
                return null;

            int numeroContrato = codigoContrato.ObterSomenteNumeros().ToInt();
            Repositorio.Embarcador.Frete.ContratoTransporteFrete repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repositorioContratoTransporteFrete.BuscarPorNumeroContrato(numeroContrato);

            if (contratoTransporteFrete == null)
                throw new ServicoException($"Nenhum Contrato Transportador encontrado: {codigoContrato.Trim()}.");

            return contratoTransporteFrete;
        }

        #endregion Métodos Privados - Contrato Transportador

        #region Métodos Privados - Modelo Veicular de Carga

        private void SalvarModeloVeicularCarga(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, string descricaoParametro, string percentualRota, string quantidadadeEntregas, string capacidadeOTM)
        {
            decimal? percentualRotaSalvar = percentualRota?.ToString().Replace(",", ".").ToNullableDecimal();
            int? quantidadadeEntregasSalvar = quantidadadeEntregas?.ToNullableInt();
            bool? capacidadeOTMSalvar = capacidadeOTM?.ToNullableBool();

            if ((percentualRotaSalvar == null) && (quantidadadeEntregasSalvar == null) && (capacidadeOTMSalvar == null))
                return;

            if (_tabelaFrete.ParametroBase != TipoParametroBaseTabelaFrete.ModeloReboque)
                throw new ControllerException($"Parâmetro base da tabela deve ser modelos de reboque para importar os parâmetros de saída.");

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorDescricao(descricaoParametro);

            if (modeloVeicularCarga == null)
                throw new ServicoException($"Modelo Veicular (Parametro base) não encontrado: {descricaoParametro.Trim()}");

            if (!_tabelaFrete.ModelosReboque.Any(modelo => modelo.Codigo == modeloVeicularCarga.Codigo))
                throw new ServicoException($"O modelo {modeloVeicularCarga.Descricao} não está cadastrado na tabela.");

            Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga modeloVeicularCargaSalvar = repositorioTabelaFreteClienteModeloVeicularCarga.BuscarPorTabelaFreteClienteEModeloVeicularCarga(tabelaFreteCliente.Codigo, modeloVeicularCarga.Codigo);

            if (modeloVeicularCargaSalvar == null)
                modeloVeicularCargaSalvar = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga()
                {
                    TabelaFreteCliente = tabelaFreteCliente,
                    ModeloVeicularCarga = modeloVeicularCarga,
                    PendenteIntegracao = true
                };
            else
                modeloVeicularCargaSalvar.Initialize();

            modeloVeicularCargaSalvar.PercentualRota = percentualRotaSalvar ?? 0;
            modeloVeicularCargaSalvar.QuantidadeEntregas = quantidadadeEntregasSalvar ?? 0;
            modeloVeicularCargaSalvar.CapacidadeOTM = capacidadeOTMSalvar ?? false;

            if (modeloVeicularCargaSalvar.Codigo > 0)
            {
                modeloVeicularCargaSalvar.PendenteIntegracao = modeloVeicularCargaSalvar.IsChanged();

                repositorioTabelaFreteClienteModeloVeicularCarga.Atualizar(modeloVeicularCargaSalvar);
            }
            else
                repositorioTabelaFreteClienteModeloVeicularCarga.Inserir(modeloVeicularCargaSalvar);
        }

        #endregion Métodos Privados - Modelo Veicular de Carga

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete> Importar(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao, ExcelWorksheet planilha, string nomeArquivo, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            if (parametrosImportacao.IndiceLinhaIniciarImportacao <= 0)
                throw new ServicoException("Linha de início dos dados inválida.");

            if ((parametrosImportacao.CodigoOrigem <= 0) && (parametrosImportacao.IndiceColunaOrigem <= 0) && (parametrosImportacao.IndiceColunaClienteOrigem <= 0) && (parametrosImportacao.IndiceColunaEstadoOrigem <= 0) && (parametrosImportacao.IndiceColunaCepOrigem <= 0) && (parametrosImportacao.IndiceColunaRotaOrigem <= 0) && (parametrosImportacao.IndiceColunaRegiaoOrigem <= 0))
                throw new ServicoException("É necessário selecionar a origem ou informar uma das colunas de origem (origem, estado, remetente, CEP, rota ou região).");

            if ((parametrosImportacao.CodigoDestino <= 0) && (parametrosImportacao.IndiceColunaDestino <= 0) && (parametrosImportacao.IndiceColunaClienteDestino <= 0) && (parametrosImportacao.IndiceColunaEstadoDestino <= 0) && (parametrosImportacao.IndiceColunaCepDestino <= 0) && (parametrosImportacao.IndiceColunaRotaDestino <= 0) && (parametrosImportacao.IndiceColunaRegiaoDestino <= 0))
                throw new ServicoException("É necessário selecionar o destino ou informar uma das colunas de destino (destino, estado, destinatário, CEP, rota ou região).");

            if ((parametrosImportacao.Parametros == null) || (parametrosImportacao.Parametros.Count <= 0))
                throw new ServicoException("É necessário adicionar os parâmetros para importação da tabela de frete.");

            CarregarParametrosBase(parametrosImportacao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete> erros = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete>();
            List<int> codigosTabelaFreteClienteAtualizarAprovacao = new List<int>();

            Servicos.Log.TratarErro("Inicio importação tabela", $"TabelaFrete_{nomeArquivo}");
            Servicos.Log.TratarErro("Linhas " + planilha.Dimension.End.Row.ToString(), $"TabelaFrete_{nomeArquivo}");

            for (int indiceLinha = parametrosImportacao.IndiceLinhaIniciarImportacao; indiceLinha <= planilha.Dimension.End.Row; indiceLinha++)
            {
                try
                {
                    _unitOfWork.FlushAndClear();

                    Servicos.Log.TratarErro("Processando linha " + indiceLinha.ToString(), $"TabelaFrete_{nomeArquivo}");

                    int codigoTabelaFreteClienteAtualizarAprovacao = Importar(indiceLinha, parametrosImportacao, planilha, usuario, tipoServicoMultisoftware, configuracaoTabelaFrete);

                    if (codigoTabelaFreteClienteAtualizarAprovacao > 0)
                        codigosTabelaFreteClienteAtualizarAprovacao.Add(codigoTabelaFreteClienteAtualizarAprovacao);
                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete linhaRetorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.RetornoImportacaoTabelaFrete
                    {
                        CodigoErro = indiceLinha,
                        LinhaErro = $"Linha {indiceLinha}",
                        DescricaoErro = excecao.Message
                    };
                    erros.Add(linhaRetorno);
                }
            }

            Servicos.Log.TratarErro("Fim importação tabela", $"TabelaFrete_{nomeArquivo}");

            if (codigosTabelaFreteClienteAtualizarAprovacao.Count > 0)
            {
                try
                {
                    _unitOfWork.Start();

                    Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoTabelaFrete = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(_unitOfWork, ObterConfiguracaoEmbarcador(), notificarUsuario: false);

                    servicoAprovacaoTabelaFrete.AtualizarAprovacao(_tabelaFrete.Codigo, codigosTabelaFreteClienteAtualizarAprovacao, tipoServicoMultisoftware);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro($"Falha ao atualizar a aprovação: {excecao.ToString()}", $"TabelaFrete_{nomeArquivo}");
                }
            }

            return erros;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao Importar(Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoPendentetabelaFrete, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha repImportacaoTabelaFreteLinha = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha(_unitOfWork);
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro repImportacaoTabelaFreteParametro = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro(_unitOfWork);

            if ((ImportacaoPendentetabelaFrete.Origem == null) && (ImportacaoPendentetabelaFrete.ColunaOrigem <= 0) && (ImportacaoPendentetabelaFrete.ColunaEstadoOrigem <= 0) && (ImportacaoPendentetabelaFrete.ColunaCEPOrigem <= 0) && (ImportacaoPendentetabelaFrete.ColunaRotaOrigem <= 0) && (ImportacaoPendentetabelaFrete.ColunaRegiaoOrigem <= 0) && (ImportacaoPendentetabelaFrete.ColunaRemetente <= 0))
                throw new ServicoException("É necessário selecionar a origem ou informar uma das colunas de origem (origem, estado, remetente, CEP, rota ou região).");

            if ((ImportacaoPendentetabelaFrete.Destino == null) && (ImportacaoPendentetabelaFrete.ColunaDestino <= 0) && (ImportacaoPendentetabelaFrete.ColunaEstadoDestino <= 0) && (ImportacaoPendentetabelaFrete.ColunaCEPDestino <= 0) && (ImportacaoPendentetabelaFrete.ColunaRotaDestino <= 0) && (ImportacaoPendentetabelaFrete.ColunaRegiaoDestino <= 0) && (ImportacaoPendentetabelaFrete.ColunaDestinatario <= 0))
                throw new ServicoException("É necessário selecionar o destino ou informar uma das colunas de destino (destino, estado, destinatário, CEP, rota ou região).");

            int contador = (int)ImportacaoPendentetabelaFrete.ColunaLinhaInicioDados;
            int indexLeitura = (int)ImportacaoPendentetabelaFrete.ColunaLinhaInicioDados;

            if (ImportacaoPendentetabelaFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Processando)
            {
                int LinhasImportadas = repImportacaoTabelaFreteLinha.contarTodasLinhasporImportacaoTabelaFrete(ImportacaoPendentetabelaFrete.Codigo);

                if (LinhasImportadas > indexLeitura)
                {
                    indexLeitura = LinhasImportadas;
                    contador = LinhasImportadas;
                }
            }

            int contadorVerificacaoCancelamento = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> listaDadosLinha = ConverterArquivoemDadosLinha(ImportacaoPendentetabelaFrete.TokenArquivo, indexLeitura, _unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro> parametrosImportacao = repImportacaoTabelaFreteParametro.BuscarPorImportacaoTabelaFrete(ImportacaoPendentetabelaFrete.Codigo);

            CarregarParametrosBase(ImportacaoPendentetabelaFrete);

            for (int i = 0; i < listaDadosLinha.Count; i++)
            {
                try
                {
                    int numeroTentativas = 0;
                    Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha linha = new Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = listaDadosLinha[i];

                    _unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha;

                    while (true)
                    {
                        retornoLinha = Importar(dadosLinha, contador, ImportacaoPendentetabelaFrete, usuario, tipoServicoMultisoftware, ref numeroTentativas, parametrosImportacao);
                        if (retornoLinha.contar || !retornoLinha.isTimeOut)
                            break;
                    }

                    linha.Numero = contador;
                    linha.ImportacaoTabelaFrete = ImportacaoPendentetabelaFrete;

                    if (retornoLinha.codigo > 0)
                        linha.TabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente { Codigo = retornoLinha.codigo };

                    if (retornoLinha.contar)
                    {
                        if (retornoLinha.codigo > 0)
                        {
                            linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Sucesso;
                            linha.Mensagem = "Linha Tabela frete Importada.";
                            linha.PermitirSolicitarAprovacao = retornoLinha.isAlterado;
                            repImportacaoTabelaFreteLinha.Inserir(linha);
                        }
                    }
                    else
                    {
                        linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Erro;
                        linha.Mensagem = retornoLinha.mensagemFalha;
                        repImportacaoTabelaFreteLinha.Inserir(linha);
                    }

                    contador++;
                    contadorVerificacaoCancelamento++;

                    if (contadorVerificacaoCancelamento == 10)
                    {
                        //vamos verificar se o usuario nao cancelou essa importacao;
                        contadorVerificacaoCancelamento = 0;
                        bool isCancelada = repImportacaoTabelaFrete.VerificarUsuarioCancelouImportacaoEmAndamento(ImportacaoPendentetabelaFrete.Codigo);
                        if (isCancelada)
                        {
                            retorno.MensagemAviso = "Importação cancelada pelo Usuário";
                            return retorno;
                        }
                    }

                    _unitOfWork.CommitChanges();
                    _unitOfWork.FlushAndClear();

                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                    Servicos.Log.TratarErro("Erro processar linha " + i + ":" + excecao.Message);

                    retorno.MensagemAviso = excecao.Message;
                    return retorno;
                }
            }

            retorno.Total = repImportacaoTabelaFreteLinha.contarTodasLinhasporImportacaoTabelaFrete(ImportacaoPendentetabelaFrete.Codigo);
            retorno.Importados = repImportacaoTabelaFreteLinha.contarLinhasImportadasporImportacaoTabelaFrete(ImportacaoPendentetabelaFrete.Codigo);
            retorno.MensagemAviso = "";

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao GerarEntidadesImportacaoTabelaFrete(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao, int quantidadeLinhas, dynamic parametros, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string nomeArquivo, string tokenArquivo)
        {
            if (quantidadeLinhas <= 0)
                throw new ServicoException("Nenhuma linha encontrada na planilha");

            if ((parametros == null) || (parametros.Count <= 0))
                throw new ServicoException("É necessário adicionar os parâmetros para importação da tabela de frete.");

            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(parametrosImportacao.CodigoTabelaFrete);

            if (tabelaFrete == null)
                throw new ServicoException("Tabela de frete não encontrada.");

            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigenciaTabela = repositorioVigencia.BuscarPorCodigo(parametrosImportacao.CodigoVigencia);

            if (vigenciaTabela == null)
                throw new ServicoException("Vigência não encontrada.");

            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repositorioImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro repositorioImportacaoTabelaFreteParametro = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (parametrosImportacao.CodigoTipoOperacao > 0) ? repositorioTipoOperacao.BuscarPorCodigo(parametrosImportacao.CodigoTipoOperacao) : null;
            Dominio.Entidades.Localidade origem = (parametrosImportacao.CodigoOrigem > 0) ? repositorioLocalidade.BuscarPorCodigo(parametrosImportacao.CodigoOrigem) : null;
            Dominio.Entidades.Localidade destino = (parametrosImportacao.CodigoDestino > 0) ? repositorioLocalidade.BuscarPorCodigo(parametrosImportacao.CodigoDestino) : null;
            Dominio.Entidades.Empresa empresa = (parametrosImportacao.CodigoEmpresa > 0) ? repositorioEmpresa.BuscarPorCodigo(parametrosImportacao.CodigoEmpresa) : null;

            Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete novaImportacaoTabelaFrete = new Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Pendente,
                QuantidadeLinhas = quantidadeLinhas,
                Usuario = usuario,
                DataImportacao = DateTime.Now,
                ColunaCEPDestino = parametrosImportacao.IndiceColunaCepDestino,
                ColunaCEPOrigem = parametrosImportacao.IndiceColunaCepOrigem,
                ColunaCodigoIntegracao = parametrosImportacao.IndiceColunaCodigoIntegracao,
                ColunaDestino = parametrosImportacao.IndiceColunaDestino,
                ColunaEstadoDestino = parametrosImportacao.IndiceColunaEstadoDestino,
                ColunaEstadoOrigem = parametrosImportacao.IndiceColunaEstadoOrigem,
                ColunaLinhaInicioDados = parametrosImportacao.IndiceLinhaIniciarImportacao,
                ColunaOrigem = parametrosImportacao.IndiceColunaOrigem,
                ColunaParametroBase = parametrosImportacao.IndiceColunaParametroBase,
                ColunaPrazoCEPDestino = parametrosImportacao.IndiceColunaCepDestinoDiasUteis,
                ColunaRegiaoDestino = parametrosImportacao.IndiceColunaRegiaoDestino,
                ColunaRegiaoOrigem = parametrosImportacao.IndiceColunaRegiaoOrigem,
                ColunaRemetente = parametrosImportacao.IndiceColunaClienteOrigem,
                ColunaDestinatario = parametrosImportacao.IndiceColunaClienteDestino,
                ColunaRotaDestino = parametrosImportacao.IndiceColunaRotaDestino,
                ColunaRotaOrigem = parametrosImportacao.IndiceColunaRotaOrigem,
                ColunaTransportador = parametrosImportacao.IndiceColunaTransportador,
                ColunaCanalEntrega = parametrosImportacao.IndiceColunaCanalEntrega,
                ColunaTipoOperacao = parametrosImportacao.IndiceColunaTipoOperacao,
                ColunaTipoDeCarga = parametrosImportacao.IndiceColunaTipoDeCarga,
                ColunaLeadTimeDias = parametrosImportacao.IndiceColunaLeadTimeDias,
                NomeArquivo = nomeArquivo,
                DataInicioProcessamento = DateTime.Now,
                TabelaFrete = tabelaFrete,
                Origem = origem,
                Destino = destino,
                TipoOperacao = tipoOperacao,
                Vigencia = vigenciaTabela,
                ValidoParaQualquerDestino = parametrosImportacao.FreteValidoParaQualquerDestino,
                ValidoParaQualquerOrigem = parametrosImportacao.FreteValidoParaQualquerOrigem,
                NaoAtualizarValoresZerados = parametrosImportacao.NaoAtualizarValoresZerados,
                NaoValidarTabelasExistentes = parametrosImportacao.NaoValidarTabelasExistentes,
                Empresa = empresa,
                TokenArquivo = tokenArquivo,
                Mensagem = "",
                Moeda = parametrosImportacao.Moeda,
                TipoPagamento = parametrosImportacao.TipoPagamento,
                ColunaFronteira = parametrosImportacao.IndiceColunaFronteira,
            };

            repositorioImportacaoTabelaFrete.Inserir(novaImportacaoTabelaFrete, auditado);

            foreach (dynamic parametro in parametros)
            {
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro ImportacaoTabelaFreteParametro = new Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro()
                {
                    ItemParametroBase = (string)parametro.ItemParametroBase,
                    ParametroBase = ((string)parametro.ParametroBase).ToInt(),
                    TipoValor = (TipoCampoValorTabelaFrete)parametro.TipoValor,
                    Coluna = parametro.Coluna != null ? (int)parametro.Coluna : 0,
                    ImportacaoTabelaFrete = novaImportacaoTabelaFrete
                };

                repositorioImportacaoTabelaFreteParametro.Inserir(ImportacaoTabelaFreteParametro);
            }

            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                MensagemAviso = "Planilha adicionada com sucesso à fila de processamento.",
                Total = quantidadeLinhas,
                Importados = quantidadeLinhas
            };
        }

        public string ObterArquivoTemporario(string tokenArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;

            return Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{tokenArquivo}.*", SearchOption.TopDirectoryOnly)?.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
