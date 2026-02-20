using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao.KuehneNagel
{
    public class IntegracaoKuehneNagel
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoKuehneNagel(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Kuehne+Nagel.");

            return configuracaoIntegracao;
        }

        private bool PossuiIntegracaoKuehneNagel()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.KuehneNagel);
        }

        #endregion Métodos Privados

        #region Métodos Privados - Integrar Carregamento

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporte ObterOrdemTransporte(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporte ordemTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporte()
            {
                Dados = ObterOrdemTransporteDados(cargaPedidoReferencia, dadosTransporteMaritimo),
                Identificacao = ObterOrdemTransporteIdentificacao(carregamentoIntegracao, cargaPedidoReferencia)
            };

            return ordemTransporte;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDados ObterOrdemTransporteDados(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDados ordemTransporteDados = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDados()
            {
                Controladoria = "BRIPV03",
                Endereco = ObterOrdemTransporteDadosEndereco(cargaPedidoReferencia),
                Envio = ObterOrdemTransporteDadosEnvio(cargaPedidoReferencia, dadosTransporteMaritimo)
            };

            return ordemTransporteDados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEndereco ObterOrdemTransporteDadosEndereco(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEndereco ordemTransporteDadosEndereco = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEndereco()
            {
                Cliente = ObterOrdemTransporteDadosEnderecoCliente(cargaPedidoReferencia),
                Consignatario = ObterOrdemTransporteDadosEnderecoConsignatario(cargaPedidoReferencia),
                Remetente = ObterOrdemTransporteDadosEnderecoRemetente(cargaPedidoReferencia)
            };

            return ordemTransporteDadosEndereco;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante ObterOrdemTransporteDadosEnderecoCliente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.Entidades.Cliente cliente = cargaPedidoReferencia.Pedido.Remetente;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante dadosPartida = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante()
            {
                Cidade = cliente.Localidade?.Descricao ?? "",
                CodigoCompactacao = "",
                CodigoPais = cliente.Localidade?.Estado?.Pais?.Abreviacao ?? "",
                Nome = cliente.Nome,
                POBox = "",
                Rua = cliente.Endereco.Left(35),
                SegundaRua = "",
                SegundoNome = ""
            };

            return dadosPartida;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoConsignatario ObterOrdemTransporteDadosEnderecoConsignatario(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.Entidades.Cliente consignatario = cargaPedidoReferencia.Pedido.ClienteAdicional;
            Dominio.Entidades.Pais pais = (consignatario != null) && (consignatario.Tipo == "E") ? consignatario?.Pais : consignatario?.Localidade?.Estado?.Pais;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante dadosPartida = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante()
            {
                Cidade = consignatario?.Localidade?.Descricao ?? "",
                CodigoCompactacao = "",
                CodigoPais = pais?.Abreviacao ?? "",
                Nome = consignatario?.Nome ?? "",
                POBox = "",
                Rua = consignatario?.Endereco.Left(35) ?? "",
                SegundaRua = "",
                SegundoNome = ""
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoConsignatario ordemTransporteDadosEnderecoConsignatario = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoConsignatario()
            {
                DadosPartida = dadosPartida
            };

            return ordemTransporteDadosEnderecoConsignatario;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoRemetente ObterOrdemTransporteDadosEnderecoRemetente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.Entidades.Cliente remetente = cargaPedidoReferencia.Pedido.Remetente;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante dadosPartida = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DadosParticipante()
            {
                Cidade = remetente.Localidade?.Descricao ?? "",
                CodigoCompactacao = "",
                CodigoPais = remetente.Localidade?.Estado?.Pais?.Abreviacao ?? "",
                Nome = remetente.Nome,
                POBox = "",
                Numero = "",
                Rua = remetente.Endereco.Left(35),
                SegundaRua = "",
                SegundoNome = ""
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoRemetente ordemTransporteDadosEnderecoRemetente = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnderecoRemetente()
            {
                DadosPartida = dadosPartida
            };

            return ordemTransporteDadosEnderecoRemetente;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvio ObterOrdemTransporteDadosEnvio(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvio ordemTransporteDadosEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvio()
            {
                DataETS = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DataHora(DateTime.Now),
                DetalhesCarga = ObterOrdemTransporteDadosEnvioDetalhesCarga(cargaPedidoReferencia, dadosTransporteMaritimo),
                ModoTransporte = cargaPedidoReferencia.Pedido.ViaTransporte?.CodigoIntegracaoEnvio ?? "",
                PaisDestino = cargaPedidoReferencia.Pedido.SiglaPaisPortoDestino ?? "",
                PaisOrigem = cargaPedidoReferencia.Pedido.SiglaPaisPortoOrigem ?? "",
                PortoDestino = cargaPedidoReferencia.Pedido.CodigoPortoDestino.Left(5) ?? "",
                PortoOrigem = cargaPedidoReferencia.Pedido.CodigoPortoOrigem.Left(5) ?? "",
                Referencias = ObterOrdemTransporteDadosEnvioReferencias(cargaPedidoReferencia),
                TermoEntrega = dadosTransporteMaritimo?.Incoterm.Left(3) ?? "",
                TermoEntregaLocalizacao = cargaPedidoReferencia.Pedido.PagamentoMaritimo?.ObterDescricao() ?? "",
                TipoEnvio = (int)(dadosTransporteMaritimo?.TipoEnvio ?? TipoEnvioTransporteMaritimo.TON)
            };

            return ordemTransporteDadosEnvio;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCarga ObterOrdemTransporteDadosEnvioDetalhesCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = cargaPedidoReferencia.Carga.Carregamento?.ModeloVeicularCarga ?? cargaPedidoReferencia.Carga.ModeloVeicularCarga;
            string tipo = modeloVeicularCarga?.CodigosIntegracao?.FirstOrDefault() ?? modeloVeicularCarga?.CodigoIntegracao ?? "";

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCarga ordemTransporteDadosEnvioDetalhesCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCarga()
            {
                Conteudos = ObterOrdemTransporteDadosEnvioDetalhesCargaConteudos(cargaPedidoReferencia, dadosTransporteMaritimo),
                NumeroContainer = "",
                NumeroFilhotes = "1",
                NumeroLacre = "",
                PesoBruto = Math.Round(cargaPedidoReferencia.Peso, decimals: 2),
                Tipo = tipo.Left(35),
                Volume = cargaPedidoReferencia.QtVolumes.ToString("n0", CultureInfo.CreateSpecificCulture("pt-BR"))
            };

            return ordemTransporteDadosEnvioDetalhesCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCargaConteudo[] ObterOrdemTransporteDadosEnvioDetalhesCargaConteudos(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCargaConteudo> conteudos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCargaConteudo>();
            string descricaoCarga = $"{(cargaPedidoReferencia.Carga.TipoDeCarga?.Descricao ?? "")}{(string.IsNullOrWhiteSpace(cargaPedidoReferencia.Pedido.Temperatura) ? "" : $" {cargaPedidoReferencia.Pedido.Temperatura}C")}";
            List<string> listaDescricao = descricaoCarga.SplitByLength(tamanho: 26).ToList();

            listaDescricao.Add($"NCM: {dadosTransporteMaritimo?.CodigoNCM ?? ""}");

            foreach (string descricao in listaDescricao)
                conteudos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioDetalhesCargaConteudo()
                {
                    Descricao = descricao,
                    MarcasENumeros = ""
                });

            return conteudos.ToArray();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia ObterOrdemTransporteDadosEnvioReferenciaCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia()
            {
                Referencia = cargaPedidoReferencia.Carga.Protocolo.ToString(),
                TipoEndereco = "SH",
                TipoReferencia = "CIN"
            };

            return referenciaCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia ObterOrdemTransporteDadosEnvioReferenciaDespachante(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            if (string.IsNullOrWhiteSpace(cargaPedidoReferencia.Pedido.DescricaoDespachante))
                return null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaDespachante = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia()
            {
                Referencia = cargaPedidoReferencia.Pedido.DescricaoDespachante.Left(35) ?? "",
                TipoEndereco = "SH",
                TipoReferencia = "AGE"
            };

            return referenciaDespachante;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia ObterOrdemTransporteDadosEnvioReferenciaEstufagem(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            if (cargaPedidoReferencia.Carga.DataCarregamentoCarga == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaEstufagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia()
            {
                Referencia = cargaPedidoReferencia.Carga.DataCarregamentoCarga?.ToString("yyyy-MM-dd") ?? "",
                TipoEndereco = "SH",
                TipoReferencia = "CUS"
            };

            return referenciaEstufagem;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia ObterOrdemTransporteDadosEnvioReferenciaEXP(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaEXP = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia()
            {
                Referencia = cargaPedidoReferencia.Pedido.NumeroEXP ?? "",
                TipoEndereco = "SH",
                TipoReferencia = "ON"
            };

            return referenciaEXP;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia[] ObterOrdemTransporteDadosEnvioReferencias(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia> referencias = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaDespachante = ObterOrdemTransporteDadosEnvioReferenciaDespachante(cargaPedidoReferencia);
            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteDadosEnvioReferencia referenciaEstufagem = ObterOrdemTransporteDadosEnvioReferenciaEstufagem(cargaPedidoReferencia);

            referencias.Add(ObterOrdemTransporteDadosEnvioReferenciaEXP(cargaPedidoReferencia));
            referencias.Add(ObterOrdemTransporteDadosEnvioReferenciaCarga(cargaPedidoReferencia));

            if (referenciaEstufagem != null)
                referencias.Add(referenciaEstufagem);

            if (referenciaDespachante != null)
                referencias.Add(referenciaDespachante);

            return referencias.ToArray();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteIdentificacao ObterOrdemTransporteIdentificacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa empresaPai = repositorioEmpresa.BuscarEmpresaPai();
            bool ambienteProducao = empresaPai?.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao;

            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteIdentificacao ordemTransporteIdentificacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteIdentificacao()
            {
                DataTransmissao = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.DataHora(carregamentoIntegracao.DataIntegracao),
                Identificacao = cargaPedidoReferencia.Carga.Protocolo,
                IdentificacaoDestinatario = ambienteProducao ? "KNPROD" : "KNTEST",
                IdentificacaoRemetente = "BRIPV03",
                TipoIdentificacao = "SHPFIX",
                Versao = "01.10"
            };

            return ordemTransporteIdentificacao;
        }

        #endregion Métodos Privados - Integrar de Carregamento

        #region Métodos Privados - Processar Arquivos de Recebimento

        private int ObterProtocoloCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacao statusConsignacao)
        {
            int protocoloCarga = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacaoDadosReferencia referencia in statusConsignacao.Dados.Referencias)
            {
                if (referencia.CodigoReferencia == "CIN")
                {
                    protocoloCarga = referencia.Referencia.ToInt();
                    break;
                }
            }

            if (protocoloCarga <= 0)
                throw new ServicoException("O protocolo da carga não foi encontrado nas referências do arquivo.");

            return protocoloCarga;
        }

        #endregion Métodos Privados - Processar Arquivos de Recebimento

        #region Métodos Publicos

        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositoriocarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel configuracaoIntegracao = ObterConfiguracaoIntegracao();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

                if (carga == null)
                    throw new ServicoException("A carga ainda não foi gerada");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorPedido(cargaPedidoReferencia.Pedido.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporte ordemTransporte = ObterOrdemTransporte(carregamentoIntegracao, cargaPedidoReferencia, dadosTransporteMaritimo);

                xmlRequisicao = Utilidades.XML.Serializar(ordemTransporte, indentar: true);

                MemoryStream arquivoEnviar = Utilidades.String.ToStream(xmlRequisicao);
                string numeroEXP = cargaPedidoReferencia.Pedido.NumeroEXP?.Replace("/", "") ?? "";
                string nomeArquivo = $"{numeroEXP}_{carregamentoIntegracao.DataIntegracao.ToString("ddMMyyyy_HHmmss")}_{cargaPedidoReferencia.Carga.Protocolo}.xml";
                string mensagemErro = string.Empty;
                string schemaUri = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), @"Content\Schemas\TransportOrderExtLight_0210.xsd");
                List<string> errosValidacaoSchema = Utilidades.XML.ObterErrosValidacaoSchema(xmlRequisicao, schemaUri);

                if (errosValidacaoSchema.Count > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteValidacao ordemTransporteValidacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.OrdemTransporteValidacao()
                    {
                        MensagensErro = errosValidacaoSchema.ToArray()
                    };

                    xmlRetorno = Utilidades.XML.Serializar(ordemTransporteValidacao, indentar: true);
                    mensagemErro = "Erro ao validar o schema do arquivo para integração com a Kuehne+Nagel";
                }
                else
                {
                    string diretorio = $"{configuracaoIntegracao.Diretorio}/inbound";

                    Servicos.FTP.EnviarArquivo(arquivoEnviar, nomeArquivo, configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);
                }

                if (string.IsNullOrWhiteSpace(mensagemErro))
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoIntegracao.ProblemaIntegracao = string.Empty;
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                }

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Kuehne+Nagel";

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositoriocarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void ProcessarArquivosRecebimento()
        {
            if (!PossuiIntegracaoKuehneNagel())
                return;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string mensagemErro = string.Empty;
                string diretorio = $"{configuracaoIntegracao.Diretorio}/outbound";
                string diretorioImportados = $"{configuracaoIntegracao.Diretorio}/outbound/imported/";
                string diretorioImportadoComErro = $"{configuracaoIntegracao.Diretorio}/outbound/error/";
                string diretorioDownload = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "IntegracaoKuehneNagel", "ArquivosRecebimento");

                List<string> nomesArquivosDisponiveisDownload = Servicos.FTP.ObterListagemArquivos(configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    throw new ServicoException(mensagemErro);

                if ((nomesArquivosDisponiveisDownload == null) || (nomesArquivosDisponiveisDownload.Count == 0))
                    return;

                int quantidadeArquivosProcessados = 0;
                int quantidadeLimiteArquivosProcessar = 10;
                Marfrig.IntegracaoExportacaoMarfrig servicoIntegracaoExportacaoMarfrig = new Marfrig.IntegracaoExportacaoMarfrig(_unitOfWork);

                foreach (string nomeArquivo in nomesArquivosDisponiveisDownload)
                {
                    if (Path.GetExtension(nomeArquivo).ToLower() != ".xml")
                        continue;

                    quantidadeArquivosProcessados++;

                    try
                    {
                        Stream arquivo = Servicos.FTP.DownloadArquivo(configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, nomeArquivo, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);

                        if (!string.IsNullOrWhiteSpace(mensagemErro))
                            throw new ServicoException(mensagemErro, CodigoExcecao.FalhaAoRealizarDownloadArquivo);

                        using (StreamReader reader = new StreamReader(arquivo))
                        {
                            string arquivoXml = reader.ReadToEnd();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacao statusConsignacao = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacao>(arquivoXml);

                            if ((statusConsignacao == null) || (statusConsignacao.Dados == null) || (statusConsignacao.Identificacao == null))
                                throw new ServicoException("Não foi possível realizar a leitura do arquivo.");

                            if ((statusConsignacao.Dados.Referencias == null) || (statusConsignacao.Dados.Referencias.Length == 0))
                                throw new ServicoException("O arquivo não possui referências.");

                            int protocoloCarga = ObterProtocoloCarga(statusConsignacao);
                            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia = repositorioCargaPedido.BuscarPrimeiroPedidoPorProtocoloCarga(protocoloCarga);

                            if (cargaPedidoReferencia == null)
                                throw new ServicoException("Não foi possível encontrar a carga com o protocolo das referências do arquivo.");

                            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
                            Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorPedido(cargaPedidoReferencia.Pedido.Codigo);

                            if (dadosTransporteMaritimo == null)
                                dadosTransporteMaritimo = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo()
                                {
                                    Pedido = cargaPedidoReferencia.Pedido
                                };

                            dadosTransporteMaritimo.ModoTransporte = statusConsignacao.Dados.ModoTransporte;
                            //dadosTransporteMaritimo.TipoTransporte = statusConsignacao.Dados.TipoTransporte;
                            dadosTransporteMaritimo.Status = StatusControleMaritimo.Ativo;

                            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacaoDadosReferencia referencia in statusConsignacao.Dados.Referencias)
                            {
                                if ((referencia.CodigoTipoEndereco == "CA") && (referencia.CodigoReferencia == "BN"))
                                    cargaPedidoReferencia.Pedido.NumeroBooking = referencia.Referencia;
                            }

                            if (statusConsignacao.Dados.Status != null)
                            {
                                if (statusConsignacao.Dados.Status.Codigo == "1250")
                                {
                                    dadosTransporteMaritimo.DataDeadLineCarga = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                    //dadosTransporteMaritimo.TerminalContainer = statusConsignacao.Dados.Status.Localizacao;
                                    //dadosTransporteMaritimo.TerminalOrigem = statusConsignacao.Dados.Status.Observacao;

                                    if (statusConsignacao.Dados.Status.Historicos?.Length > 0)
                                    {
                                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacaoDadosStatus historico in statusConsignacao.Dados.Status.Historicos)
                                        {
                                            if (historico.Codigo == "1211")
                                            {
                                                dadosTransporteMaritimo.DataDeadLineDraf = historico.Data?.ObterDataHora();
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (statusConsignacao.Dados.Status.Codigo == "0200")
                                    dadosTransporteMaritimo.DataRetiradaVazio = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1065")
                                    dadosTransporteMaritimo.DataDepositoContainer = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1075")
                                    dadosTransporteMaritimo.DataBooking = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1200")
                                    dadosTransporteMaritimo.DataETAOrigem = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1210")
                                    dadosTransporteMaritimo.DataETASegundaOrigem = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1300")
                                    dadosTransporteMaritimo.DataETAOrigemFinal = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "1400")
                                    dadosTransporteMaritimo.DataETADestinoFinal = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "2320")
                                    dadosTransporteMaritimo.DataRetiradaContainerDestino = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                                else if (statusConsignacao.Dados.Status.Codigo == "3400")
                                    dadosTransporteMaritimo.DataRetornoVazio = statusConsignacao.Dados.Status.Data?.ObterDataHora();
                            }

                            if (statusConsignacao.Dados.FreteMaritimo != null)
                            {
                                dadosTransporteMaritimo.CodigoArmador = statusConsignacao.Dados.FreteMaritimo.CodigoArmador;
                                dadosTransporteMaritimo.CodigoPortoCarregamento = statusConsignacao.Dados.FreteMaritimo.PortoCarga;
                                dadosTransporteMaritimo.DataETADestino = statusConsignacao.Dados.FreteMaritimo.PortoDescargaETA?.ObterDataHora();
                                dadosTransporteMaritimo.DataETS = statusConsignacao.Dados.FreteMaritimo.PortoCargaETS?.ObterDataHora();
                                dadosTransporteMaritimo.NomeNavio = statusConsignacao.Dados.FreteMaritimo.NomeNavio;
                                dadosTransporteMaritimo.NumeroViagem = statusConsignacao.Dados.FreteMaritimo.NumeroViagem;

                                if (statusConsignacao.Dados.FreteMaritimo.Tipo == "0")
                                    dadosTransporteMaritimo.NumeroBL = statusConsignacao.Dados.FreteMaritimo.NumeroBL;
                                else if (statusConsignacao.Dados.FreteMaritimo.Tipo == "1")
                                    dadosTransporteMaritimo.NumeroBL = statusConsignacao.Dados.ReferenciaKuehneNagel;
                            }

                            if (statusConsignacao.Dados.DetalhesCarga?.Container != null)
                            {
                                // dadosTransporteMaritimo.NumeroContainer = statusConsignacao.Dados.DetalhesCarga.Container.NumeroContainer;
                                dadosTransporteMaritimo.NumeroLacre = statusConsignacao.Dados.DetalhesCarga.Container.NumeroLacre;
                            }

                            if (dadosTransporteMaritimo.Codigo > 0)
                                repositorioPedidoDadosTransporteMaritimo.Atualizar(dadosTransporteMaritimo);
                            else
                                repositorioPedidoDadosTransporteMaritimo.Inserir(dadosTransporteMaritimo);

                            if (statusConsignacao.Dados.FreteMaritimo?.Roteamentos?.Length > 0)
                            {
                                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento repositorioPedidoDadosTransporteMaritimoRoteamento = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento(_unitOfWork);
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> dadosTransporteMaritimoRoteamentos = repositorioPedidoDadosTransporteMaritimoRoteamento.BuscarPorDadosTransporteMaritimo(dadosTransporteMaritimo.Codigo);

                                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel.StatusConsignacaoDadosFreteMaritimoRoteamento roteamento in statusConsignacao.Dados.FreteMaritimo.Roteamentos)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento pedidoDadosTransporteMaritimoRoteamento = (from o in dadosTransporteMaritimoRoteamentos where o.CodigoRoteamento == roteamento.CodigoRoteamento select o).FirstOrDefault();

                                    if (pedidoDadosTransporteMaritimoRoteamento != null)
                                        continue;

                                    pedidoDadosTransporteMaritimoRoteamento = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento()
                                    {
                                        CodigoRoteamento = roteamento.CodigoRoteamento,
                                        CodigoSCAC = roteamento.CodigoSCAC,
                                        DadosTransporteMaritimo = dadosTransporteMaritimo,
                                        FlagNavio = roteamento.FlagNavio,
                                        NomeNavio = roteamento.NomeNavio,
                                        NumeroViagem = roteamento.NumeroViagem,
                                        PortoCargaData = roteamento.PortoCarga?.Data?.ObterDataHora(),
                                        PortoCargaLocalizacao = roteamento.PortoCarga?.Localizacao,
                                        PortoDescargaData = roteamento.PortoDescarga?.Data?.ObterDataHora(),
                                        PortoDescargaLocalizacao = roteamento.PortoDescarga?.Localizacao,
                                        TipoRemessa = roteamento.TipoRemessa
                                    };

                                    repositorioPedidoDadosTransporteMaritimoRoteamento.Inserir(pedidoDadosTransporteMaritimoRoteamento);
                                }
                            }

                            repositorioPedido.Atualizar(cargaPedidoReferencia.Pedido);

                            servicoIntegracaoExportacaoMarfrig.AdicionarCargaIntegracaoExportacao(cargaPedidoReferencia.Carga);
                        }

                        Servicos.FTP.MoverArquivo(configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, nomeArquivo, $"{diretorioImportados}{nomeArquivo}", out mensagemErro, configuracaoIntegracao.UtilizarSFTP);
                    }
                    catch (ServicoException excecao)
                    {
                        Log.TratarErro($"Falhar ao processar o arquivo de recebimento {nomeArquivo} da integração com a Kuehne+Nagel: {excecao.Message}");

                        if (excecao.ErrorCode != CodigoExcecao.FalhaAoRealizarDownloadArquivo)
                            Servicos.FTP.MoverArquivo(configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, nomeArquivo, $"{diretorioImportadoComErro}{nomeArquivo}", out mensagemErro, configuracaoIntegracao.UtilizarSFTP);
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro($"Falhar ao processar o arquivo de recebimento {nomeArquivo} da integração com a Kuehne+Nagel:");
                        Log.TratarErro(excecao);

                        Servicos.FTP.MoverArquivo(configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, nomeArquivo, $"{diretorioImportadoComErro}{nomeArquivo}", out mensagemErro, configuracaoIntegracao.UtilizarSFTP);
                    }

                    if (quantidadeArquivosProcessados > quantidadeLimiteArquivosProcessar)
                        break;
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro($"Falha ao processar os arquivos de recebimento da integração com a Kuehne+Nagel: {excecao.Message}");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Publicos
    }
}
