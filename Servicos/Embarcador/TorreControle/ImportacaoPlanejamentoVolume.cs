using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.TorreControle
{
    public sealed class ImportacaoPlanejamentoVolume
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores
        public ImportacaoPlanejamentoVolume(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dictionary<string, dynamic> dados)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Métodos Públicos 
        public Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume ObterPlanejamentoVolumeImportar()
        {
            Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = ObterPlanejamento();
            Dominio.Entidades.Localidade destino = ObterDestino();
            Dominio.Entidades.Localidade origem = ObterOrigem();
            Dominio.Entidades.Cliente destinatario = ObterDestinatario();
            Dominio.Entidades.Cliente remetente = ObterRemetente();

            planejamentoVolume.TipoDeCarga = ObterTipoDeCarga();
            planejamentoVolume.Transportador = ObterTransportador();
            planejamentoVolume.ModeloVeicular = ObterModeloVeicular();
            planejamentoVolume.TipoOperacao = ObterTipoOperacao();
            planejamentoVolume.DataProgramacaoCargaInicial = ObterDataPlanejamentoCargaInicial();
            planejamentoVolume.DataProgramacaoCargaFinal = ObterDataPlanejamentoCargaFinal();
            planejamentoVolume.TotalToneladasMes = ObterTotalToneladasMes();
            planejamentoVolume.DisponibilidadePlacas = ObterDisponibilidadePlacas();
            planejamentoVolume.NumeroContrato = ObterNumeroContrato();
            planejamentoVolume.TotalTransferenciaEntrePlantas = ObterTotalTransferenciaEntrePlantas();

            if (destino != null)
            {
                if (planejamentoVolume.Destinos == null)
                    planejamentoVolume.Destinos = new List<Dominio.Entidades.Localidade>();

                planejamentoVolume.Destinos.Add(destino);
            }

            if (origem != null)
            {
                if (planejamentoVolume.Origens == null)
                    planejamentoVolume.Origens = new List<Dominio.Entidades.Localidade>();

                planejamentoVolume.Origens.Add(origem);
            }

            if (destinatario != null)
            {
                if (planejamentoVolume.Destinatarios == null)
                    planejamentoVolume.Destinatarios = new List<Dominio.Entidades.Cliente>();

                planejamentoVolume.Destinatarios.Add(destinatario);
            }

            if (remetente != null)
            {
                if (planejamentoVolume.Remetentes == null)
                    planejamentoVolume.Remetentes = new List<Dominio.Entidades.Cliente>();

                planejamentoVolume.Remetentes.Add(remetente);
            }

            return planejamentoVolume;
        }
        #endregion

        #region Métodos Privados
        private Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume ObterPlanejamento()
        {
            var codigoPlanejamentoVolume = 0;

            if (_dados.TryGetValue("Codigo", out var codigo))
                codigoPlanejamentoVolume = ((string)codigo).ToInt();

            Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(_unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = repositorioPlanejamentoVolume.BuscarPorCodigo(codigoPlanejamentoVolume);

            if (planejamentoVolume == null)
            {
                planejamentoVolume = new Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume();
                planejamentoVolume.Codigo = codigoPlanejamentoVolume;
            }
            else
                planejamentoVolume.Initialize();

            return planejamentoVolume;
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoDeCarga()
        {
            string codigoIntegracao = string.Empty;

            if (_dados.TryGetValue("TipoDeCarga", out var tipoCarga))
                codigoIntegracao = (string)tipoCarga;

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                throw new ImportacaoException("Codigo de Integração do Tipo de Carga não foi informado.");

            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoCarga.BuscarPorCodigoEmbarcador(codigoIntegracao);

            return tipoDeCarga;
        }

        private Dominio.Entidades.Embarcador.Pedidos.TipoOperacao ObterTipoOperacao()
        {
            string codigoIntegracao = string.Empty;

            if (_dados.TryGetValue("TipoOperacao", out var tipoOperacao))
                codigoIntegracao = (string)tipoOperacao;

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                throw new ImportacaoException("Codigo de Integração do Tipo de Operação não foi informado.");

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoDeOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(codigoIntegracao);

            return tipoDeOperacao;
        }

        private Dominio.Entidades.Localidade ObterDestino()
        {
            string descricaoLocalidade = string.Empty;
            string ufCidade = string.Empty;

            if (_dados.TryGetValue("NomeCidadeDestino", out var descricao))
                descricaoLocalidade = (string)descricao;

            if (_dados.TryGetValue("UFCidadeDestino", out var uf))
                ufCidade = (string)uf;

            if (string.IsNullOrWhiteSpace(descricao) && string.IsNullOrEmpty(uf))
                return null;

            if ((!string.IsNullOrWhiteSpace(descricaoLocalidade)) && string.IsNullOrEmpty(ufCidade))
                throw new ImportacaoException("Ao informar o Nome da Cidade de Destino é obrigatório informar também o UF da cidade.");

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorCidadeUF(descricaoLocalidade, ufCidade);

            return localidade;
        }

        private Dominio.Entidades.Localidade ObterOrigem()
        {
            string descricaoLocalidade = string.Empty;
            string ufCidade = string.Empty;

            if (_dados.TryGetValue("NomeCidadeOrigem", out var descricao))
                descricaoLocalidade = (string)descricao;

            if (_dados.TryGetValue("UFCidadeOrigem", out var uf))
                ufCidade = (string)uf;

            if (string.IsNullOrWhiteSpace(descricaoLocalidade) && string.IsNullOrEmpty(ufCidade))
                return null;

            if ((!string.IsNullOrWhiteSpace(descricaoLocalidade)) && string.IsNullOrEmpty(ufCidade))
                throw new ImportacaoException("Ao informar o Nome da Cidade de Origem é obrigatório informar também o UF da cidade.");

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorCidadeUF(descricaoLocalidade, ufCidade);

            return localidade;
        }

        private Dominio.Entidades.Cliente ObterDestinatario()
        {
            double cpfCnpjDestinatario = 0;


            if (_dados.TryGetValue("Destinatario", out var cpfCnpj))
                cpfCnpjDestinatario = System.Convert.ToDouble(cpfCnpj);

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

            return destinatario;
        }

        private Dominio.Entidades.Cliente ObterRemetente()
        {
            double cpfCnpjRemetente = 0;


            if (_dados.TryGetValue("Remetente", out var cpfCnpj))
                cpfCnpjRemetente = System.Convert.ToDouble(cpfCnpj);

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente remetente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

            return remetente;
        }

        private DateTime ObterDataPlanejamentoCargaInicial()
        {
            DateTime dataInicial = DateTime.MinValue;

            if (_dados.TryGetValue("DataProgramacaoCargaInicial", out var dataProgramacaoIncial))
                dataInicial = ((string)dataProgramacaoIncial).ToDateTime();

            if (dataInicial == DateTime.MinValue)
                throw new ImportacaoException("Favor informar a data de planejamento inicial.");

            return dataInicial;
        }

        private DateTime ObterDataPlanejamentoCargaFinal()
        {
            DateTime dataFinal = DateTime.MinValue;

            if (_dados.TryGetValue("DataProgramacaoCargaFinal", out var dataProgramacaoFinal))
                dataFinal = ((string)dataProgramacaoFinal).ToDateTime();

            if (dataFinal == DateTime.MinValue)
                throw new ImportacaoException("Favor informar a data de planejamento final.");

            return dataFinal;
        }

        private decimal ObterTotalToneladasMes()
        {
            decimal totalTonMes = 0;

            if (_dados.TryGetValue("TotalToneladasMes", out var toneladaMes))
                totalTonMes = ((string)toneladaMes).ToDecimal();

            return totalTonMes;
        }

        private int ObterDisponibilidadePlacas()
        {
            int totalPlacas = 0;

            if (_dados.TryGetValue("DisponibilidadePlacas", out var disponibilidadePlacas))
                totalPlacas = ((string)disponibilidadePlacas).ToInt();

            return totalPlacas;
        }

        private string ObterNumeroContrato()
        {
            string totalNumeroContrato = string.Empty;

            if (_dados.TryGetValue("DisponibilidadePlacas", out var numeroContrato))
                totalNumeroContrato = ((string)numeroContrato);

            return totalNumeroContrato;
        }

        private int ObterTotalTransferenciaEntrePlantas()
        {
            int totalTransferencia = 0;

            if (_dados.TryGetValue("TotalTransferenciaEntrePlantas", out var total))
                totalTransferencia = ((string)total).ToInt();

            return totalTransferencia;
        }

        private Dominio.Entidades.Empresa ObterTransportador()
        {
            string codigoIntegracao = string.Empty;

            if (_dados.TryGetValue("Transportador", out var transportadorIntegracao))
                codigoIntegracao = (string)transportadorIntegracao;

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                return null;

            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigoIntegracao(codigoIntegracao);

            return transportador;
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicular()
        {
            string codigoIntegracao = string.Empty;

            if (_dados.TryGetValue("ModeloVeicular", out var modeloVeicularCarga))
                codigoIntegracao = (string)modeloVeicularCarga;

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                return null;

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repositorioModeloVeicular.buscarPorCodigoIntegracao(codigoIntegracao);

            return modeloVeicular;
        }

        #endregion
    }
}
