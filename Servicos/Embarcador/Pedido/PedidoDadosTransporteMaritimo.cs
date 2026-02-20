using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class PedidoDadosTransporteMaritimo
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Privados


        #region Construtores

        public PedidoDadosTransporteMaritimo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Metodos Publicos

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoBooking()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 73, Descricao = "Nº Pedido", Propriedade = "NumeroPedido", Tamanho = 150, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 74, Descricao = "Nº EXP", Propriedade = "NumeroEXP", Tamanho = 150, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = "Nº Carga", Propriedade = "NumeroCarga", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Nº Booking", Propriedade = "NumeroBooking", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Armador (CPF/CNPJ)", Propriedade = "CNPJCPFArmador", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Tipo Container", Propriedade = "TipoContainer", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 69, Descricao = "Container", Propriedade = "Container", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Status EXP", Propriedade = "StatusEXP", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Despachante", Propriedade = "Despachante", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Transbordo", Propriedade = "Transbordo", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Mensagem Transbordo", Propriedade = "MensagemTransbordo", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 57, Descricao = "Rota", Propriedade = "CodigoRota", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 59, Descricao = "Data Booking", Propriedade = "DataBooking", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 60, Descricao = "Data Dead Line Carga", Propriedade = "DataDeadLineCarga", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 62, Descricao = "Data Dead Line Pedido", Propriedade = "DataDeadLinePedido", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Data Dead Line Draf", Propriedade = "DataDeadLineDraf", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Segunda Data Dead Line Carga", Propriedade = "SegundaDataDeadLineCarga", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Segunda Data Dead Line Draf", Propriedade = "SegundaDataDeadLineDraf", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Data ETA Destino", Propriedade = "DataETADestino", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Data ETA Destino Final", Propriedade = "DataETADestinoFinal", Tamanho = 400 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Data ETA Origem", Propriedade = "DataETAOrigem", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Data ETA Origem Final", Propriedade = "DataETAOrigemFinal", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Data ETA Segunda Origem", Propriedade = "DataETASegundaOrigem", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Data ETA Segundo Destino", Propriedade = "DataETASegundoDestino", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Data ETA Transbordo", Propriedade = "DataETATransbordo", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Data Retirada Container", Propriedade = "DataRetiradaContainer", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "Data Retirada Container Destino", Propriedade = "DataRetiradaContainerDestino", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = "Data Retirada Vazio", Propriedade = "DataRetiradaVazio", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = "Data ETS", Propriedade = "DataETS", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = "Data ETS Transbordo", Propriedade = "DataETSTransbordo", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = "Data Carregamento Pedido", Propriedade = "DataCarregamentoPedido", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = "Data Previsão Entrega", Propriedade = "DataPrevisaoEntrega", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = "Data Conhecimento", Propriedade = "DataConhecimento", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = "Moeda Capatazia", Propriedade = "MoedaCapatazia", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "Valor Capatazia", Propriedade = "ValorCapatazia", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = "Valor Frete", Propriedade = "ValorFrete", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = "Código Contrato FOB", Propriedade = "CodigoContratoFOB", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = "Porto Origem", Propriedade = "PortoOrigem", Tamanho = 50 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "Descrição Porto Carregamento", Propriedade = "DescricaoPortoOrigem", Tamanho = 150 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = "País Porto Porto Carregamento", Propriedade = "PaisPortoOrigem", Tamanho = 50 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = "Sigla Pais Porto Carregamento", Propriedade = "SiglaPaisPortoOrigem", Tamanho = 10 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 34, Descricao = "Código Porto Carregamento Transbordo", Propriedade = "CodigoPortoCarregamentoTransbordo", Tamanho = 50 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = "Descrição Porto Carregamento Transbordo", Propriedade = "DescricaoPortoCarregamentoTransbordo", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = "Porto Destino", Propriedade = "PortoDestino", Tamanho = 50 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = "Descrição Porto Destino Transbordo", Propriedade = "DescricaoPortoDestinoTransbordo", Tamanho = 200 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = "País Porto Destino Transbordo", Propriedade = "PaisPortoDestinoTransbordo", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = "Navio Transbordo", Propriedade = "NavioTransbordo", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = "Navio", Propriedade = "Navio", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 52, Descricao = "Via Transporte", Propriedade = "ViaTransporte", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 53, Descricao = "Modo Transporte", Propriedade = "ModoTransporte", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 54, Descricao = "Tipo Envio", Propriedade = "TipoEnvio", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 55, Descricao = "Tipo Probe", Propriedade = "TipoProbe", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 56, Descricao = "Nº BL", Propriedade = "NumeroBL", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 61, Descricao = "Nº Lacre", Propriedade = "NumeroLacre", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 71, Descricao = "Nº Viagem", Propriedade = "NumeroViagem", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 63, Descricao = "Terminal Origem", Propriedade = "TerminalOrigem", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 70, Descricao = "Terminal Container", Propriedade = "TerminalContainer", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 64, Descricao = "Nº Viagem Transbordo", Propriedade = "NumeroViagemTransbordo", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 65, Descricao = "Código Especie", Propriedade = "CodigoEspecie", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 66, Descricao = "Descrição Especie", Propriedade = "DescricaoEspecie", Tamanho = 150 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 72, Descricao = "Código InLand", Propriedade = "CodigoInLand", Tamanho = 150 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = "Descrição InLand", Propriedade = "DescricaoInLand", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = "Tipo InLand", Propriedade = "TipoInland", Tamanho = 50 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 68, Descricao = "Possui Genset (S/N)", Propriedade = "PossuiGenset", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 75, Descricao = "Código Integração Armador", Propriedade = "CodigoIntegracaoArmador", Tamanho = 150 });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarBooking(string dados, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao)
        {

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();

                    // Processa linha do arquivo como um booking isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarBookingLinha(linhas[i], arquivoGerador, usuario, operadorLogistica, clienteMultiSoftware, auditado, AdminStringConexao);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Deve contar como linha importada?
                    if (retornoLinha.contar) contador++;

                    // Processou com sucesso?
                    if (retornoLinha.processou)
                        _unitOfWork.CommitChanges();
                    else
                        _unitOfWork.Rollback();
                }
                catch (Exception ex2)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }


        #endregion

        #region Metodos Privados

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarBookingLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao)
        {
            Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(_unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repPedidoDadosTransporteIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ViaTransporte repViatransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(_unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(_unitOfWork);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                string NumeroPedido = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedido" select obj).FirstOrDefault();
                if (colNumeroPedido != null)
                    NumeroPedido = colNumeroPedido.Valor;

                string NumeroExp = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColNumeroExp = (from obj in linha.Colunas where obj.NomeCampo == "NumeroEXP" select obj).FirstOrDefault();
                if (ColNumeroExp != null)
                    NumeroExp = ColNumeroExp.Valor;


                if (!string.IsNullOrEmpty(NumeroPedido) && !string.IsNullOrEmpty(NumeroExp))
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo bookingOriginal = repPedidoDadosTransporteMaritimo.BuscarPorPedidoENumeroEXP(NumeroPedido, NumeroExp);

                    if (bookingOriginal == null)
                        return RetornarFalhaLinha("Booking não encontrado pelo NumeroPedido e NumeroEXP");

                    if (repPedidoDadosTransporteIntegracao.ExisteDadosTransporteMaritimoCodigoOriginalAguardandoRetorno(bookingOriginal.Codigo))
                        return RetornarFalhaLinha($"Para o Pedido { NumeroPedido } já existe Booking em edição aguardando integração");

                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo bookingEditar = bookingOriginal.Clonar();

                    bookingEditar.Initialize();

                    bookingEditar.NumeroEXP = NumeroExp;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColNumeroBooking = (from obj in linha.Colunas where obj.NomeCampo == "NumeroBooking" select obj).FirstOrDefault();
                    if (ColNumeroBooking != null)
                        bookingEditar.NumeroBooking = ColNumeroBooking.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColNumeroCarga = (from obj in linha.Colunas where obj.NomeCampo == "NumeroCarga" select obj).FirstOrDefault();
                    if (ColNumeroCarga != null)
                        bookingEditar.CodigoCargaEmbarcador = ColNumeroCarga.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colArmador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFArmador" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoArmador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoArmador" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente armador = null;
                    if(colCodigoIntegracaoArmador != null && colCodigoIntegracaoArmador.Valor != null)
                    {
                        armador = repCliente.BuscarPorCodigoIntegracao((string)colCodigoIntegracaoArmador.Valor);
                        if (armador != null)
                            bookingEditar.Armador = armador;
                    }

                    if (colArmador != null && armador == null)
                    {
                        string somenteNumeros = Utilidades.String.OnlyNumbers((string)colArmador.Valor);
                        if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || _configuracaoEmbarcador.Pais == TipoPais.Exterior))
                        {
                            double cpfCNPJArmador = double.Parse(somenteNumeros);
                            armador = repCliente.BuscarPorCPFCNPJ(cpfCNPJArmador);
                            if (armador != null)
                                bookingEditar.Armador = armador;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoContainer = (from obj in linha.Colunas where obj.NomeCampo == "TipoContainer" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = null;
                    if (colTipoContainer != null)
                    {
                        tipoContainer = repTipoContainer.BuscarPorCodigoIntegracao(colTipoContainer.Valor);
                        if (tipoContainer != null)
                            bookingEditar.TipoContainer = tipoContainer;

                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColNumeroContainer = (from obj in linha.Colunas where obj.NomeCampo == "Container" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                    if (ColNumeroContainer != null)
                    {
                        container = repContainer.BuscarPorCodigoIntegracao(ColNumeroContainer.Valor);
                        if (container != null)
                            bookingEditar.Container = container;

                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colStatusEXP = (from obj in linha.Colunas where obj.NomeCampo == "StatusEXP" select obj).FirstOrDefault();
                    if (colStatusEXP != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEXP StatusEXP = StatusEXP.NaoDefinido;
                        Enum.TryParse((string)colStatusEXP.Valor, out StatusEXP);

                        if (StatusEXP != StatusEXP.NaoDefinido)
                            bookingEditar.StatusEXP = StatusEXP;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDespachante = (from obj in linha.Colunas where obj.NomeCampo == "Despachante" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente despachante = null;
                    if (colDespachante != null)
                    {
                        despachante = repCliente.BuscarPorCodigoIntegracao(colDespachante.Valor);
                        if (despachante != null)
                            bookingEditar.Despachante = despachante;

                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "Transbordo" select obj).FirstOrDefault();
                    if (ColTransbordo != null)
                        bookingEditar.Transbordo = ColTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ColMensagemTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "MensagemTransbordo" select obj).FirstOrDefault();
                    if (ColMensagemTransbordo != null)
                        bookingEditar.MensagemTransbordo = ColMensagemTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRota = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRota" select obj).FirstOrDefault();
                    if (colCodigoRota != null)
                        bookingEditar.CodigoRota = colCodigoRota.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataBooking = (from obj in linha.Colunas where obj.NomeCampo == "DataBooking" select obj).FirstOrDefault();
                    DateTime DataBooking = ConverterDataTimeImportacao(colDataBooking);
                    if (DataBooking != DateTime.MinValue)
                        bookingEditar.DataBooking = DataBooking;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataDeadLineCarga = (from obj in linha.Colunas where obj.NomeCampo == "DataDeadLineCarga" select obj).FirstOrDefault();
                    DateTime DataDeadLineCarga = ConverterDataTimeImportacao(colDataDeadLineCarga);
                    if (DataDeadLineCarga != DateTime.MinValue)
                        bookingEditar.DataDeadLineCarga = DataDeadLineCarga;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataDeadLinePedido = (from obj in linha.Colunas where obj.NomeCampo == "DataDeadLinePedido" select obj).FirstOrDefault();
                    DateTime DataDeadLinePedido = ConverterDataTimeImportacao(colDataDeadLinePedido);
                    if (DataDeadLinePedido != DateTime.MinValue)
                        bookingEditar.DataDeadLinePedido = DataDeadLinePedido;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataDeadLineDraf = (from obj in linha.Colunas where obj.NomeCampo == "DataDeadLineDraf" select obj).FirstOrDefault();
                    DateTime DataDeadLineDraf = ConverterDataTimeImportacao(colDataDeadLineDraf);
                    if (DataDeadLineDraf != DateTime.MinValue)
                        bookingEditar.DataDeadLineDraf = DataDeadLineDraf;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSegundaDataDeadLineCarga = (from obj in linha.Colunas where obj.NomeCampo == "SegundaDataDeadLineCarga" select obj).FirstOrDefault();
                    DateTime SegundaDataDeadLineCarga = ConverterDataTimeImportacao(colSegundaDataDeadLineCarga);
                    if (SegundaDataDeadLineCarga != DateTime.MinValue)
                        bookingEditar.SegundaDataDeadLineCarga = SegundaDataDeadLineCarga;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSegundaDataDeadLineDraf = (from obj in linha.Colunas where obj.NomeCampo == "SegundaDataDeadLineDraf" select obj).FirstOrDefault();
                    DateTime SegundaDataDeadLineDraf = ConverterDataTimeImportacao(colSegundaDataDeadLineDraf);
                    if (SegundaDataDeadLineDraf != DateTime.MinValue)
                        bookingEditar.SegundaDataDeadLineDraf = SegundaDataDeadLineDraf;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETADestino = (from obj in linha.Colunas where obj.NomeCampo == "DataETADestino" select obj).FirstOrDefault();
                    DateTime DataETADestino = ConverterDataTimeImportacao(colDataETADestino);
                    if (DataETADestino != DateTime.MinValue)
                        bookingEditar.DataETADestino = DataETADestino;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETADestinoFinal = (from obj in linha.Colunas where obj.NomeCampo == "DataETADestinoFinal" select obj).FirstOrDefault();
                    DateTime DataETADestinoFinal = ConverterDataTimeImportacao(colDataETADestinoFinal);
                    if (DataETADestinoFinal != DateTime.MinValue)
                        bookingEditar.DataETADestinoFinal = DataETADestinoFinal;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETAOrigem = (from obj in linha.Colunas where obj.NomeCampo == "DataETAOrigem" select obj).FirstOrDefault();
                    DateTime DataETAOrigem = ConverterDataTimeImportacao(colDataETAOrigem);
                    if (DataETAOrigem != DateTime.MinValue)
                        bookingEditar.DataETAOrigem = DataETAOrigem;


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETAOrigemFinal = (from obj in linha.Colunas where obj.NomeCampo == "DataETAOrigemFinal" select obj).FirstOrDefault();
                    DateTime DataETAOrigemFinal = ConverterDataTimeImportacao(colDataETAOrigemFinal);
                    if (DataETAOrigemFinal != DateTime.MinValue)
                        bookingEditar.DataETAOrigemFinal = DataETAOrigemFinal;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETASegundaOrigem = (from obj in linha.Colunas where obj.NomeCampo == "DataETASegundaOrigem" select obj).FirstOrDefault();
                    DateTime DataETASegundaOrigem = ConverterDataTimeImportacao(colDataETASegundaOrigem);
                    if (DataETASegundaOrigem != DateTime.MinValue)
                        bookingEditar.DataETASegundaOrigem = DataETASegundaOrigem;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETASegundoDestino = (from obj in linha.Colunas where obj.NomeCampo == "DataETASegundoDestino" select obj).FirstOrDefault();
                    DateTime DataETASegundoDestino = ConverterDataTimeImportacao(colDataETASegundoDestino);
                    if (DataETASegundoDestino != DateTime.MinValue)
                        bookingEditar.DataETASegundoDestino = DataETASegundoDestino;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETATransbordo = (from obj in linha.Colunas where obj.NomeCampo == "DataETATransbordo" select obj).FirstOrDefault();
                    DateTime DataETATransbordo = ConverterDataTimeImportacao(colDataETATransbordo);
                    if (DataETATransbordo != DateTime.MinValue)
                        bookingEditar.DataETATransbordo = DataETATransbordo;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataRetiradaContainer = (from obj in linha.Colunas where obj.NomeCampo == "DataRetiradaContainer" select obj).FirstOrDefault();
                    DateTime DataRetiradaContainer = ConverterDataTimeImportacao(colDataRetiradaContainer);
                    if (DataRetiradaContainer != DateTime.MinValue)
                        bookingEditar.DataRetiradaContainer = DataRetiradaContainer;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataRetiradaContainerDestino = (from obj in linha.Colunas where obj.NomeCampo == "DataRetiradaContainerDestino" select obj).FirstOrDefault();
                    DateTime DataRetiradaContainerDestino = ConverterDataTimeImportacao(colDataRetiradaContainerDestino);
                    if (DataRetiradaContainerDestino != DateTime.MinValue)
                        bookingEditar.DataRetiradaContainerDestino = DataRetiradaContainerDestino;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataRetiradaVazio = (from obj in linha.Colunas where obj.NomeCampo == "DataRetiradaVazio" select obj).FirstOrDefault();
                    DateTime DataRetiradaVazio = ConverterDataTimeImportacao(colDataRetiradaVazio);
                    if (DataRetiradaVazio != DateTime.MinValue)
                        bookingEditar.DataRetiradaContainerDestino = DataRetiradaVazio;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETS = (from obj in linha.Colunas where obj.NomeCampo == "DataETS" select obj).FirstOrDefault();
                    DateTime DataETS = ConverterDataTimeImportacao(colDataETS);
                    if (DataETS != DateTime.MinValue)
                        bookingEditar.DataETS = DataETS;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataETSTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "DataETSTransbordo" select obj).FirstOrDefault();
                    DateTime DataETSTransbordo = ConverterDataTimeImportacao(colDataETSTransbordo);
                    if (DataETSTransbordo != DateTime.MinValue)
                        bookingEditar.DataETSTransbordo = DataETSTransbordo;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataCarregamentoPedido = (from obj in linha.Colunas where obj.NomeCampo == "DataCarregamentoPedido" select obj).FirstOrDefault();
                    DateTime DataCarregamentoPedido = ConverterDataTimeImportacao(colDataCarregamentoPedido);
                    if (DataCarregamentoPedido != DateTime.MinValue)
                        bookingEditar.DataETSTransbordo = DataCarregamentoPedido;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoEntrega = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevisaoEntrega" select obj).FirstOrDefault();
                    DateTime DataPrevisaoEntrega = ConverterDataTimeImportacao(colDataPrevisaoEntrega);
                    if (DataPrevisaoEntrega != DateTime.MinValue)
                        bookingEditar.DataPrevisaoEntrega = DataPrevisaoEntrega;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataConhecimento = (from obj in linha.Colunas where obj.NomeCampo == "DataConhecimento" select obj).FirstOrDefault();
                    DateTime DataConhecimento = ConverterDataTimeImportacao(colDataConhecimento);
                    if (DataConhecimento != DateTime.MinValue)
                        bookingEditar.DataConhecimento = DataConhecimento;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMoedaCapatazia = (from obj in linha.Colunas where obj.NomeCampo == "MoedaCapatazia" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Moedas.Moeda moeda = null;
                    if (colMoedaCapatazia != null)
                    {
                        moeda = repMoeda.BuscarPorSigla(colMoedaCapatazia.Valor);
                        if (moeda != null)
                            bookingEditar.MoedaCapatazia = moeda;
                    }


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorCapatazia = (from obj in linha.Colunas where obj.NomeCampo == "ValorCapatazia" select obj).FirstOrDefault();
                    if (colValorCapatazia != null)
                        bookingEditar.ValorCapatazia = Utilidades.Decimal.Converter((string)colValorCapatazia.Valor);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFrete = (from obj in linha.Colunas where obj.NomeCampo == "ValorFrete" select obj).FirstOrDefault();
                    if (colValorFrete != null)
                        bookingEditar.ValorFrete = Utilidades.Decimal.Converter((string)colValorFrete.Valor);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoContratoFOB = (from obj in linha.Colunas where obj.NomeCampo == "CodigoContratoFOB" select obj).FirstOrDefault();
                    if (colCodigoContratoFOB != null)
                        bookingEditar.CodigoContratoFOB = colCodigoContratoFOB.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoPortoCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "CodigoPortoCarregamento" select obj).FirstOrDefault();
                    //if (colCodigoPortoCarregamento != null)
                    //    bookingEditar.CodigoPortoCarregamento = colCodigoPortoCarregamento.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoPortoOrigem" select obj).FirstOrDefault();
                    //if (colDescricaoPortoOrigem != null)
                    //    bookingEditar.DescricaoPortoOrigem = colDescricaoPortoOrigem.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPaisPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "PaisPortoOrigem" select obj).FirstOrDefault();
                    //if (colPaisPortoOrigem != null)
                    //    bookingEditar.DescricaoPortoOrigem = colPaisPortoOrigem.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSiglaPaisPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "SiglaPaisPortoOrigem" select obj).FirstOrDefault();
                    //if (colSiglaPaisPortoOrigem != null)
                    //    bookingEditar.SiglaPaisPortoOrigem = colSiglaPaisPortoOrigem.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "PortoOrigem" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente portoOrigem = null;
                    if (colPortoOrigem != null)
                    {
                        portoOrigem = repCliente.BuscarPorCodigoIntegracao(colPortoOrigem.Valor);
                        if (portoOrigem != null)
                            bookingEditar.PortoOrigem = portoOrigem;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoPortoCarregamentoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "CodigoPortoCarregamentoTransbordo" select obj).FirstOrDefault();
                    if (colCodigoPortoCarregamentoTransbordo != null)
                        bookingEditar.CodigoPortoCarregamentoTransbordo = colCodigoPortoCarregamentoTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoPortoCarregamentoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoPortoCarregamentoTransbordo" select obj).FirstOrDefault();
                    if (colDescricaoPortoCarregamentoTransbordo != null)
                        bookingEditar.DescricaoPortoCarregamentoTransbordo = colDescricaoPortoCarregamentoTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoDestino = (from obj in linha.Colunas where obj.NomeCampo == "PortoDestino" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente PortoDestino = null;
                    if (colPortoDestino != null)
                    {
                        PortoDestino = repCliente.BuscarPorCodigoIntegracao(colPortoDestino.Valor);
                        if (PortoDestino != null)
                            bookingEditar.PortoDestino = PortoDestino;
                    }

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoPortoDestinoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "CodigoPortoDestinoTransbordo" select obj).FirstOrDefault();
                    //if (colCodigoPortoDestinoTransbordo != null)
                    //    bookingEditar.CodigoPortoDestinoTransbordo = colCodigoPortoDestinoTransbordo.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoPortoDestinoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoPortoDestinoTransbordo" select obj).FirstOrDefault();
                    //if (colDescricaoPortoDestinoTransbordo != null)
                    //    bookingEditar.DescricaoPortoDestinoTransbordo = colDescricaoPortoDestinoTransbordo.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPaisPortoDestinoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "PaisPortoDestinoTransbordo" select obj).FirstOrDefault();
                    //if (colPaisPortoDestinoTransbordo != null)
                    //    bookingEditar.PaisPortoDestinoTransbordo = colPaisPortoDestinoTransbordo.Valor;

                    //Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSiglaPaisPortoDestinoTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "SiglaPaisPortoDestinoTransbordo" select obj).FirstOrDefault();
                    //if (colSiglaPaisPortoDestinoTransbordo != null)
                    //    bookingEditar.SiglaPaisPortoDestinoTransbordo = colSiglaPaisPortoDestinoTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeNavioTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "NavioTransbordo" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo = null;
                    if (colNomeNavioTransbordo != null)
                    {
                        NavioTransbordo = repNavio.BuscarPorCodigoIntegracao(colNomeNavioTransbordo.Valor);
                        if (NavioTransbordo != null)
                            bookingEditar.NavioTransbordo = NavioTransbordo;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNavio = (from obj in linha.Colunas where obj.NomeCampo == "Navio" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.Navio Navio = null;
                    if (colNavio != null)
                    {
                        Navio = repNavio.BuscarPorCodigoIntegracao(colNavio.Valor);
                        if (Navio != null)
                            bookingEditar.Navio = Navio;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colViaTransporte = (from obj in linha.Colunas where obj.NomeCampo == "ViaTransporte" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = null;
                    if (colViaTransporte != null)
                    {
                        viaTransporte = repViatransporte.BuscarPorCodigoIntegracao(colViaTransporte.Valor);
                        if (viaTransporte != null)
                            bookingEditar.ViaTransporte = viaTransporte;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModoTransporte = (from obj in linha.Colunas where obj.NomeCampo == "ModoTransporte" select obj).FirstOrDefault();
                    if (colModoTransporte != null)
                        bookingEditar.ModoTransporte = colModoTransporte.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoEnvio = (from obj in linha.Colunas where obj.NomeCampo == "TipoEnvio" select obj).FirstOrDefault();
                    if (colTipoEnvio != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioTransporteMaritimo tipoEnvio = TipoEnvioTransporteMaritimo.FCL;
                        Enum.TryParse((string)colTipoEnvio.Valor, out tipoEnvio);

                        if (bookingEditar.TipoEnvio != tipoEnvio)
                            bookingEditar.TipoEnvio = tipoEnvio;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoProbe = (from obj in linha.Colunas where obj.NomeCampo == "TipoProbe" select obj).FirstOrDefault();
                    if (colTipoProbe != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProbe TipoProbe = TipoProbe.NaoDefinido;
                        Enum.TryParse((string)colTipoEnvio.Valor, out TipoProbe);

                        if (bookingEditar.TipoProbe != TipoProbe)
                            bookingEditar.TipoProbe = TipoProbe;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroBL = (from obj in linha.Colunas where obj.NomeCampo == "NumeroBL" select obj).FirstOrDefault();
                    if (colNumeroBL != null)
                        bookingEditar.NumeroBL = colNumeroBL.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroLacre = (from obj in linha.Colunas where obj.NomeCampo == "NumeroLacre" select obj).FirstOrDefault();
                    if (colNumeroLacre != null)
                        bookingEditar.NumeroLacre = colNumeroLacre.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroViagem = (from obj in linha.Colunas where obj.NomeCampo == "NumeroViagem" select obj).FirstOrDefault();
                    if (colNumeroViagem != null)
                        bookingEditar.NumeroViagem = colNumeroViagem.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminalOrigem = (from obj in linha.Colunas where obj.NomeCampo == "TerminalOrigem" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente terminalOrigem = null;
                    if (colTerminalOrigem != null)
                    {
                        terminalOrigem = repCliente.BuscarPorCodigoIntegracao(colTerminalOrigem.Valor);
                        if (terminalOrigem != null)
                            bookingEditar.LocalTerminalOrigem = terminalOrigem;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminalContainer = (from obj in linha.Colunas where obj.NomeCampo == "TerminalContainer" select obj).FirstOrDefault();
                    Dominio.Entidades.Cliente terminalContainer = null;
                    if (colTerminalContainer != null)
                    {
                        terminalContainer = repCliente.BuscarPorCodigoIntegracao(colTerminalContainer.Valor);
                        if (terminalContainer != null)
                            bookingEditar.LocalTerminalContainer = terminalContainer;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroViagemTransbordo = (from obj in linha.Colunas where obj.NomeCampo == "NumeroViagemTransbordo" select obj).FirstOrDefault();
                    if (colNumeroViagemTransbordo != null)
                        bookingEditar.NumeroViagemTransbordo = colNumeroViagemTransbordo.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoEspecie = (from obj in linha.Colunas where obj.NomeCampo == "CodigoEspecie" select obj).FirstOrDefault();
                    if (colCodigoEspecie != null)
                        bookingEditar.CodigoEspecie = colCodigoEspecie.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoEspecie = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoEspecie" select obj).FirstOrDefault();
                    if (colDescricaoEspecie != null)
                        bookingEditar.DescricaoEspecie = colDescricaoEspecie.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInLand = (from obj in linha.Colunas where obj.NomeCampo == "TipoInland" select obj).FirstOrDefault();
                    if (colInLand != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInland TipoInland = TipoInland.NaoDefinido;
                        Enum.TryParse((string)colInLand.Valor, out TipoInland);

                        if (bookingEditar.TipoInLand != TipoInland)
                            bookingEditar.TipoInLand = TipoInland;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPossuiGenset = (from obj in linha.Colunas where obj.NomeCampo == "PossuiGenset" select obj).FirstOrDefault();
                    if (colPossuiGenset != null)
                    {
                        if (colPossuiGenset.Valor == "S")
                            bookingEditar.PossuiGenset = true;
                        else if (colPossuiGenset.Valor == "N")
                            bookingEditar.PossuiGenset = false;
                    }

                    string retornoCamposObrigatorios = "";

                    ////if (string.IsNullOrEmpty(bookingEditar.CodigoCargaEmbarcador))
                    ////    retornoCamposObrigatorios += " Booking sem informação no campo Codigo Carga (campo obrigatório para integração) ";

                    if (!bookingEditar.DataPrevisaoEstufagem.HasValue)
                        retornoCamposObrigatorios += " Booking sem Data Previsão Estufagem (campo obrigatório para integração) ";

                    if (!string.IsNullOrEmpty(retornoCamposObrigatorios))
                        return RetornarFalhaLinha(retornoCamposObrigatorios);

                    if (bookingEditar.IsChanged())
                    {
                        bookingEditar.CodigoOriginal = bookingOriginal.Codigo;
                        bookingEditar.BookingTemporario = true;

                        repPedidoDadosTransporteMaritimo.Inserir(bookingEditar);
                        servIntegracaoPedidoDadosTransporteMaritimo.AdicionarPedidoDadosTransporteMaritimoIntegracao(bookingEditar);
                    }

                    return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
                }
                else
                    return RetornarFalhaLinha("Número Pedido e Número EXP são obrigatórios");

            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + ex2.Message + ").");
            }
        }


        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }


        private DateTime ConverterDataTimeImportacao(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna)
        {
            DateTime DataRetorno = DateTime.MinValue;
            string data;
            if (coluna != null)
            {
                data = coluna.Valor;
                double.TryParse(data, out double dataFormatoExcel);

                if (dataFormatoExcel > 0)
                    DataRetorno = DateTime.FromOADate(dataFormatoExcel);
                else
                    DataRetorno = data.ToDateTime();
            }

            return DataRetorno;
        }

        #endregion
    }
}
