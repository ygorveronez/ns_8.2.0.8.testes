using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Data;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class MontagemCargaRoteirizacaoController : BaseController
    {
        #region Construtores

        public MontagemCargaRoteirizacaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> SalvarRotaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unidadeTrabalho);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repostorioClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unidadeTrabalho);
                int codigoCarregamento;
                decimal distancia;
                int.TryParse(Request.Params("Carregamento"), out codigoCarregamento);
                decimal.TryParse(Request.Params("Distancia").Replace("km", ""), out distancia);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao)int.Parse(Request.Params("TipoUltimoPontoRoteirizacao"));
                string tipoRota = Request.Params("TipoRota");

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);

                if (carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoEstaAguardandoAprovacaoParaGerarCargaNaoPodeSerAlterado);

                if (string.IsNullOrWhiteSpace(Request.GetStringParam("PolilinhaRota")))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioTerUmaRotaParaSalvar);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);
                bool inserir = false;
                if (carregamentoRoteirizacao == null)
                {
                    inserir = true;
                    carregamentoRoteirizacao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao();
                }

                carregamentoRoteirizacao.Carregamento = carregamento;
                carregamentoRoteirizacao.DistanciaKM = distancia;
                carregamentoRoteirizacao.TipoRota = tipoRota;
                carregamentoRoteirizacao.PolilinhaRota = Request.GetStringParam("PolilinhaRota");
                carregamentoRoteirizacao.TempoDeViagemEmMinutos = Request.GetIntParam("TempoDeViagemEmMinutos");
                string pontosDaRota = Request.GetStringParam("PontosDaRota");
                carregamentoRoteirizacao.TipoUltimoPontoRoteirizacao = tipoUltimoPontoRoteirizacao;


                if (inserir)
                    repCarregamentoRoteirizacao.Inserir(carregamentoRoteirizacao);
                else
                {
                    repCarregamentoRoteirizacao.Atualizar(carregamentoRoteirizacao);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> rotasClientesExiste = repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota rotasClienteExiste in rotasClientesExiste)
                        repCarregamentoRoteirizacaoClientesRota.Deletar(rotasClienteExiste);

                }

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCargaRoteirizacao.SetarPontosPassagem(carregamentoRoteirizacao, pontosDaRota, true, unidadeTrabalho);

                dynamic pessoas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Pessoas"));
                int ordem = 1;
                foreach (var pessoa in pessoas)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota rotasCliente = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota();
                    double codigoCliente = double.Parse(Utilidades.String.OnlyNumbers(string.IsNullOrEmpty(pessoa.CPFCNPJ.ToString()) ? "0" : pessoa.CPFCNPJ.ToString()));
                    rotasCliente.Cliente = repCliente.BuscarPorCPFCNPJ(codigoCliente);
                    rotasCliente.CarregamentoRoteirizacao = carregamentoRoteirizacao;
                    rotasCliente.Ordem = ordem;
                    rotasCliente.Coleta = pessoa.Coleta;
                    if (pessoa.ContainsKey("CodigoOutroEndereco"))
                    {
                        int.TryParse(pessoa.CodigoOutroEndereco.ToString(), out int codigoOutroEndereco);
                        if (codigoOutroEndereco > 0)
                            rotasCliente.OutroEndereco = repostorioClienteOutroEndereco.BuscarPorCodigo(codigoOutroEndereco);
                    }
                    if (rotasCliente.Cliente != null)
                    {
                        ordem++;
                        repCarregamentoRoteirizacaoClientesRota.Inserir(rotasCliente);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, null, Localization.Resources.Cargas.MontagemCargaMapa.SalvouRota, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSalvarRotaDaCarga);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSalvarRotaDaCarga);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosRoteirizacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unidadeTrabalho);

                Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unidadeTrabalho);

                int.TryParse(Request.Params("Carregamento"), out int codigoCarregamento);

                bool alterouPedidos = Request.GetBoolParam("AlterouPedidos");

                List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
                List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa>();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

                bool sempreConsiderarExpedidor = repositorioConfiguracaoMontagemCarga.ExisteExibirListagemNotasFiscais();

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(codigoCarregamento);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = (alterouPedidos ? null : repCarregamentoRoteirizacao.BuscarPorCarregamento(codigoCarregamento));

                bool pedidosDeColeta = false;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                bool roteirizarPorLocalidade = (configuracaoEmbarcador.RoteirizarPorCidade || (carregamento.TipoOperacao?.RoteirizarPorLocalidade ?? false));

                if (carregamentoRoteirizacao != null)
                {
                    OberCarregamentoRoteirizacaoComCarregamento(serPessoa, rotasInformacaoPessoa, carregamentoRoteirizacao, roteirizarPorLocalidade, unidadeTrabalho);
                    tipoUltimoPonto = carregamentoRoteirizacao.TipoUltimoPontoRoteirizacao;
                }

                if (rotasInformacaoPessoa == null || rotasInformacaoPessoa.Count == 0)
                {
                    pedidosDeColeta = ObterRemetenteDestinatarioCarregamentoRoteirizacao(remetentes, destinatarios, carregamento, pedidosDeColeta, sempreConsiderarExpedidor);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenados = carregamentoPedidos.OrderBy(o => o.Pedido.DataCarregamentoPedido).Select(o => o.Pedido).ToList();

                    foreach (Dominio.Entidades.Cliente remetente in remetentes)
                        ObterRotasInformacaoPessoa(serPessoa, rotasInformacaoPessoa, remetente, true, roteirizarPorLocalidade, pedidosOrdenados, configuracaoRoteirizacao);

                    foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
                        ObterRotasInformacaoPessoa(serPessoa, rotasInformacaoPessoa, destinatario, false, roteirizarPorLocalidade, pedidosOrdenados, configuracaoRoteirizacao);

                    Dominio.Entidades.Embarcador.Filiais.Filial filialPreCarga = carregamento.PreCarga?.Filial;
                    ObterRotasInformacaoFilial(repCliente, serPessoa, rotasInformacaoPessoa, filialPreCarga);

                    tipoUltimoPonto = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unidadeTrabalho, configuracaoEmbarcador).ObterTipoUltimoPontoRoteirizacao((from ped in carregamento.Pedidos select ped.Pedido).ToList());

                    //// Se o carregamento ainda não está roteirizado e os pedidos são todos do mesmo tipo de operação, vamos pegar o tipo de roteirização do tipo de operação,
                    //// Caso não for, vamos retornar o padrão das configurações gerais.
                    //if (tiposOperacao?.Count == 1)
                    //{
                    //    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Servicos.Embarcador.Pedido.TipoOperacao(unidadeTrabalho).ObterTipoUltimoPontoRoteirizacao(tiposOperacao[0], carregamento.Empresa);
                    //    tipoUltimoPonto = ultimoPontoPorTipoOperacao ?? tipoUltimoPonto;
                    //}
                    //else
                    //    tipoUltimoPonto = this.ConfiguracaoEmbarcador?.TipoUltimoPontoRoteirizacao ?? tipoUltimoPonto;
                }

                var retorno = new
                {
                    roteirizado = carregamentoRoteirizacao != null,
                    PedidosDeColeta = pedidosDeColeta,
                    rotasInformacaoPessoa,
                    PontosPassagem = ObterPontosPassagem(carregamento.Rota),
                    DistanciaKM = carregamentoRoteirizacao?.DistanciaKM ?? 0,
                    TipoRota = carregamentoRoteirizacao?.TipoRota ?? "",
                    TipoUltimoPontoRoteirizacao = carregamentoRoteirizacao?.TipoUltimoPontoRoteirizacao ?? tipoUltimoPonto,
                    PolilinhaRota = carregamentoRoteirizacao?.PolilinhaRota ?? "",
                    TempoDeViagemEmMinutos = carregamentoRoteirizacao?.TempoDeViagemEmMinutos ?? 0,
                    PontosDaRota = carregamentoRoteirizacao != null ? Servicos.Embarcador.Carga.MontagemCarga.MontagemCargaRoteirizacao.ObterPontosPassagemCarregamentoRoteirizacaoSerializada(carregamentoRoteirizacao, unidadeTrabalho) : ""
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarOsDadosDaRoteirizacao);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> CriarRotaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento;
                int.TryParse(Request.Params("Carregamento"), out codigoCarregamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao)int.Parse(Request.Params("TipoUltimoPontoRoteirizacao"));

                string tipoRota = Request.Params("TipoRota");

                Servicos.Embarcador.Maps.Google.RoteirizarCarga serGoogleMaps = new Servicos.Embarcador.Maps.Google.RoteirizarCarga(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeTrabalho);


                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotaInformacao = serGoogleMaps.CriarRotaCarregamentoViaGoogleMaps(carregamento, tipoUltimoPontoRoteirizacao, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, null, Localization.Resources.Cargas.MontagemCargaMapa.CriouRotasViaGoogleMaps, unidadeTrabalho);

                return new JsonpResult(rotaInformacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoObterMelhorRota);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterPontosPassagem(Dominio.Entidades.RotaFrete rota)
        {
            List<dynamic> listaPontos = new List<dynamic>();

            if (rota?.PontoPassagemPreDefinido == null)
                return listaPontos;

            foreach (Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido ponto in rota.PontoPassagemPreDefinido)
            {
                listaPontos.Add(new
                {
                    Descricao = ponto.ObterDescricao(),
                    Latitude = ponto.ObterLatitude(),
                    Longitude = ponto.ObterLongitude(),
                    TempoEstimadoPermanencia = ponto.TempoEstimadoPermanenciaFormatado,
                });
            }

            return listaPontos;
        }

        private void OberCarregamentoRoteirizacaoComCarregamento(Servicos.WebService.Pessoas.Pessoa serPessoa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, bool roteirizarPorLocalidade, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> carregamentoRoteirizacaoClientesRota = repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repCarregamentoRoteirizacaoPontos = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> carregamentoRoteirizacaoPontosPassagem = repCarregamentoRoteirizacaoPontos.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamentoRoteirizacao.Carregamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem pontosPassagem in carregamentoRoteirizacaoPontosPassagem)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota clienteRota = (
                    from cliente in carregamentoRoteirizacaoClientesRota
                    where cliente?.Cliente?.CPF_CNPJ == pontosPassagem?.Cliente?.CPF_CNPJ
                    select cliente
                ).FirstOrDefault();

                if ((clienteRota != null || pontosPassagem.ClienteOutroEndereco != null) && pontosPassagem.TipoPontoPassagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = serPessoa.ConverterObjetoPessoa(clienteRota?.Cliente ?? pontosPassagem.ClienteOutroEndereco?.Cliente);

                    pessoa.Codigo = clienteRota?.Cliente?.CPF_CNPJ.ToString() ?? pontosPassagem.ClienteOutroEndereco?.Codigo.ToString();

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (
                        from obj in carregamentoPedidos
                        where obj.Pedido.Destinatario?.CPF_CNPJ == (pontosPassagem?.Cliente ?? pontosPassagem.ClienteOutroEndereco?.Cliente)?.CPF_CNPJ
                        select obj
                    ).FirstOrDefault();

                    Dominio.Entidades.Cliente cliente = carregamentoPedido?.Pedido?.Recebedor ?? clienteRota?.Cliente ?? pontosPassagem.ClienteOutroEndereco?.Cliente;
                    string latitude = pontosPassagem?.ClienteOutroEndereco != null ? pontosPassagem.ClienteOutroEndereco.Latitude : cliente.Latitude;
                    string longitude = pontosPassagem?.ClienteOutroEndereco != null ? pontosPassagem.ClienteOutroEndereco.Longitude : cliente.Longitude;

                    //#66938-Vamos manter as coordenas dos clientes na montagem. By Ro.Ro..
                    //if (roteirizarPorLocalidade)
                    //{
                    //    latitude = (pontosPassagem?.ClienteOutroEndereco != null ? pontosPassagem.ClienteOutroEndereco.Cliente : cliente).Localidade.Latitude?.ToString().Replace(",", ".");
                    //    longitude = (pontosPassagem?.ClienteOutroEndereco != null ? pontosPassagem.ClienteOutroEndereco.Cliente : cliente).Localidade.Longitude?.ToString().Replace(",", ".");
                    //}

                    if (pontosPassagem?.ClienteOutroEndereco != null)
                    {
                        pessoa.Endereco.Cidade.Descricao = pontosPassagem.ClienteOutroEndereco.Localidade?.Descricao;
                        pessoa.Endereco.Cidade.SiglaUF = pontosPassagem.ClienteOutroEndereco.Localidade?.Estado?.Sigla;
                        pessoa.Endereco.Logradouro = pontosPassagem.ClienteOutroEndereco.Endereco;
                        pessoa.Endereco.Numero = pontosPassagem.ClienteOutroEndereco.Numero;
                        pessoa.Endereco.Bairro = pontosPassagem.ClienteOutroEndereco.Bairro;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa
                    {
                        pessoa = pessoa,
                        coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                        {
                            tipoLocalizacao = cliente.TipoLocalizacao,
                            latitude = latitude ?? "0",
                            longitude = longitude ?? "0",
                            RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>(),
                            PrimeiraEntrega = false,
                            CodigoOutroEndereco = (pontosPassagem?.ClienteOutroEndereco?.Codigo ?? 0)
                        },
                        coleta = clienteRota?.Coleta ?? false,
                        TipoPonto = ((clienteRota?.Coleta ?? false) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega),
                        DataAgendamento = carregamentoPedido?.Pedido?.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                    };

                    if (rotaInformacao.coleta)
                    {
                        rotaInformacao.pessoa.NomeFantasia = rotaInformacao.pessoa.NomeFantasia + $" ({Localization.Resources.Cargas.MontagemCargaMapa.Coleta})";
                        rotaInformacao.pessoa.RazaoSocial = rotaInformacao.pessoa.RazaoSocial + $" ({Localization.Resources.Cargas.MontagemCargaMapa.Coleta})";
                    }

                    List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricaoEntrega = (clienteRota?.Cliente ?? pontosPassagem?.ClienteOutroEndereco?.Cliente)?.ClienteDescargas?.FirstOrDefault()?.RestricoesDescarga.ToList();

                    if (restricaoEntrega?.Count > 0)
                    {
                        rotaInformacao.coordenadas.RestricoesEntregas = (
                            from restricao in restricaoEntrega
                            select new Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega()
                            {
                                Codigo = restricao.Codigo,
                                Descricao = restricao.Descricao,
                                Observacao = restricao.Observacao,
                                PrimeiraEntrega = restricao.PrimeiraEntrega,
                                CorVisualizacao = restricao.CorVisualizacao
                            }
                        ).ToList();
                    }

                    rotasInformacaoPessoa.Add(rotaInformacao);
                }
                else if (pontosPassagem.PontoDeApoio != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa
                    {
                        pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
                        {
                            Codigo = pontosPassagem.PontoDeApoio.Codigo.ToString(),
                            NomeFantasia = pontosPassagem.PontoDeApoio.Descricao,
                            CPFCNPJ = pontosPassagem.PontoDeApoio.Codigo.ToString(),
                            //CodigoIntegracao = pontosPassagem.PontoDeApoio.Codigo.ToString(),
                            RazaoSocial = pontosPassagem.PontoDeApoio.Descricao + " - " + pontosPassagem.PontoDeApoio.Observacao,
                            Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                            {
                                Cidade = new Dominio.ObjetosDeValor.Localidade()
                                {
                                    Descricao = Localization.Resources.Cargas.MontagemCargaMapa.PontoDeApoio,
                                    SiglaUF = string.Empty
                                }
                            }
                        },
                        coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                        {
                            tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto,
                            latitude = pontosPassagem.Latitude.ToString(),
                            longitude = pontosPassagem.Longitude.ToString(),
                            RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>(),
                            PrimeiraEntrega = false,
                            CodigoOutroEndereco = 0
                        },
                        coleta = false,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem
                    };
                    rotasInformacaoPessoa.Add(rotaInformacao);
                }
            }
        }

        private bool ObterRemetenteDestinatarioCarregamentoRoteirizacao(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.Entidades.Cliente> destinatarios, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool pedidosDeColeta, bool sempreConsiderarExpedidor)
        {
            List<Dominio.Entidades.Cliente> remetentesOrdenados = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatariosOrdenados = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> pontosColetaEquipamento = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> pontosPartida = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosOrdenados = carregamento.Pedidos.OrderBy(o => o.Pedido.DataCarregamentoPedido).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from o in pedidosOrdenados where o.Pedido.TipoOperacao != null select o.Pedido.TipoOperacao).Distinct().ToList();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (tiposOperacao.Count() == 1) ? tiposOperacao.FirstOrDefault() : null;

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedido in pedidosOrdenados)
            {
                if ((tipoOperacao?.UsarRecebedorComoPontoPartidaCarga ?? false) && (pedido.Pedido.Recebedor != null))
                    pontosColetaEquipamento.Add(pedido.Pedido.Recebedor);
                else if (pedido.Pedido.PontoPartida != null)
                    pontosPartida.Add(pedido.Pedido.PontoPartida);

                if (!carregamento.CarregamentoRedespacho)
                {
                    if ((ConfiguracaoEmbarcador.NaoGerarCarregamentoRedespacho || sempreConsiderarExpedidor) && (pedido.Pedido.Expedidor != null))
                        remetentesOrdenados.Add(pedido.Pedido.Expedidor);
                    else
                        remetentesOrdenados.Add(pedido.Pedido.Remetente.ClientePai ?? pedido.Pedido.Remetente);

                    if (pedido.Pedido.Recebedor != null)
                        destinatariosOrdenados.Add(pedido.Pedido.Recebedor);
                    else if (pedido.Pedido.Destinatario != null)
                        destinatariosOrdenados.Add(pedido.Pedido.Destinatario);

                    if ((carregamento.CarregamentoColeta) || (carregamento.Recebedor != null && (carregamento.TipoOperacao?.ConfiguracaoMontagemCarga?.MontagemComRecebedorNaoGerarCargaComoColeta ?? false)))
                    {
                        pedidosDeColeta = true;
                        destinatariosOrdenados.Add(carregamento.Recebedor);
                    }
                }
                else
                {
                    if (pedido.Pedido.Expedidor == null)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoUmCarregamentoDeRedespachoPedidoNaoPossuiUmExpedidor, carregamento.NumeroCarregamento, pedido.Pedido.NumeroPedidoEmbarcador));

                    remetentesOrdenados.Add(pedido.Pedido.Expedidor);

                    if (pedido.Pedido.Destinatario != null)
                        destinatariosOrdenados.Add(pedido.Pedido.Destinatario);
                }
            }

            remetentes.Clear();
            destinatarios.Clear();

            if (pontosPartida.Count > 0)
                remetentes.Add(pontosPartida.FirstOrDefault());
            else if (pontosColetaEquipamento.Count > 0)
                remetentes.Add(pontosColetaEquipamento.FirstOrDefault());

            remetentes.AddRange(remetentesOrdenados.Distinct().ToList());
            destinatarios.AddRange(destinatariosOrdenados.Distinct().ToList());

            return pedidosDeColeta;
        }

        private void ObterRotasInformacaoFilial(Repositorio.Cliente repCliente, Servicos.WebService.Pessoas.Pessoa serPessoa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa, Dominio.Entidades.Embarcador.Filiais.Filial filialPreCarga)
        {
            if (filialPreCarga != null)
            {
                double.TryParse(filialPreCarga.CNPJ, out double cnpjFilial);
                Dominio.Entidades.Cliente pessoaFilial = repCliente.BuscarPorCPFCNPJ(cnpjFilial);

                if (pessoaFilial != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa pontoFilial = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa()
                    {
                        pessoa = serPessoa.ConverterObjetoPessoa(pessoaFilial),
                        coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                        {
                            tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto,
                            latitude = pessoaFilial.Latitude,
                            longitude = pessoaFilial.Longitude,
                            PrimeiraEntrega = false,
                            CodigoOutroEndereco = 0,
                            RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>()
                        }
                    };

                    if (rotasInformacaoPessoa.Count > 0)
                        rotasInformacaoPessoa[0] = pontoFilial;
                    else
                        rotasInformacaoPessoa.Add(pontoFilial);
                }
            }
        }

        private void ObterRotasInformacaoPessoa(Servicos.WebService.Pessoas.Pessoa serPessoa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa, Dominio.Entidades.Cliente pessoa, bool coleta, bool roteirizarPorLocalidade, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenados, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao)
        {
            string latitude = pessoa.Latitude;
            string longitude = pessoa.Longitude;

            if (roteirizarPorLocalidade)
            {
                latitude = pessoa.Localidade.Latitude?.ToString().Replace(",", ".");
                longitude = pessoa.Localidade.Longitude?.ToString().Replace(",", ".");
            }

            //Destinatários...
            if (!coleta)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenadosPessoa = (
                    from obj in pedidosOrdenados
                    where (
                        (obj?.Recebedor?.Codigo ?? 0) == pessoa.Codigo ||
                        (obj?.Destinatario?.Codigo ?? 0) == pessoa.Codigo ||
                        (obj?.RecebedorColeta?.Codigo ?? 0) == pessoa.Codigo
                    )
                    select obj
                ).ToList();

                if (pedidosOrdenadosPessoa.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoOrdenado in pedidosOrdenadosPessoa)
                {
                    PreencherRotaInformacaoPessoa(serPessoa, rotasInformacaoPessoa, pessoa, pedidoOrdenado.EnderecoDestino, latitude, longitude, coleta, pedidoOrdenado.UsarOutroEnderecoDestino, roteirizarPorLocalidade, pedidoOrdenado.DataAgendamento, configuracaoRoteirizacao);
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenadosPessoa = (
                    from obj in pedidosOrdenados
                    where (
                        (obj?.Expedidor?.Codigo ?? 0) == pessoa.Codigo ||
                        (obj?.Remetente?.Codigo ?? 0) == pessoa.Codigo
                    )
                    select obj
                ).ToList();

                if (pedidosOrdenadosPessoa.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoOrdenado in pedidosOrdenadosPessoa)
                {
                    PreencherRotaInformacaoPessoa(serPessoa, rotasInformacaoPessoa, pessoa, pedidoOrdenado.EnderecoOrigem, latitude, longitude, coleta, pedidoOrdenado.UsarOutroEnderecoOrigem, roteirizarPorLocalidade, null, configuracaoRoteirizacao);
                }
            }
        }

        private void PreencherRotaInformacaoPessoa(Servicos.WebService.Pessoas.Pessoa serPessoa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco endereco, string latitude, string longitude, bool coleta, bool usaOutroEndereco, bool roteirizarPorLocalidade, DateTime? dataAgendamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao)
        {
            bool coletaOuIgnorarOutroEnderecoPedidoComRecebedor = coleta || !configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor;

            if (usaOutroEndereco && endereco?.ClienteOutroEndereco != null && coletaOuIgnorarOutroEnderecoPedidoComRecebedor)
            {
                latitude = endereco.ClienteOutroEndereco.Latitude;
                longitude = endereco.ClienteOutroEndereco.Longitude;
            }
            else if (roteirizarPorLocalidade && !coleta && usaOutroEndereco && endereco?.ClienteOutroEndereco != null && configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor)
            {
                latitude = pessoa.Latitude;
                longitude = pessoa.Longitude;
            }

            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacaoNaoEncontrado = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa
                {
                    pessoa = serPessoa.ConverterObjetoPessoa(pessoa),
                    coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                    {
                        tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado
                    },
                    DataAgendamento = dataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    TipoPonto = coleta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega,
                };

                rotaInformacaoNaoEncontrado.pessoa.CodigoIntegracao = pessoa.CodigoIntegracao ?? string.Empty;
                rotasInformacaoPessoa.Add(rotaInformacaoNaoEncontrado);

                return;
            }

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaObj = serPessoa.ConverterObjetoPessoa(pessoa);

            if (pessoa.Tipo == "E")
                pessoaObj.CPFCNPJ = pessoa.CPF_CNPJ.ToString();

            pessoaObj.Codigo = pessoa.CPF_CNPJ.ToString();
            if (coleta)
            {
                pessoaObj.NomeFantasia = pessoaObj.NomeFantasia + $" ({Localization.Resources.Cargas.MontagemCargaMapa.Coleta})";
                pessoaObj.RazaoSocial = pessoaObj.RazaoSocial + $" ({Localization.Resources.Cargas.MontagemCargaMapa.Coleta})";
            }

            int codigoOutroEndereco = 0;
            if (usaOutroEndereco && endereco?.ClienteOutroEndereco != null && coletaOuIgnorarOutroEnderecoPedidoComRecebedor)
            {
                codigoOutroEndereco = endereco.ClienteOutroEndereco.Codigo;
                pessoaObj.Endereco.Cidade.Descricao = endereco.ClienteOutroEndereco.Localidade?.Descricao;
                pessoaObj.Endereco.Cidade.SiglaUF = endereco.ClienteOutroEndereco.Localidade?.Estado?.Sigla;
                pessoaObj.Endereco.Logradouro = endereco.ClienteOutroEndereco.Endereco;
                pessoaObj.Endereco.Numero = endereco.ClienteOutroEndereco.Numero;
                pessoaObj.Endereco.Bairro = endereco.Bairro;
            }

            if (rotasInformacaoPessoa.Exists(x => x.pessoa.Codigo == pessoaObj.Codigo && x.coleta == coleta && x.CodigoOutroEndereco == codigoOutroEndereco))
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa
            {
                pessoa = pessoaObj,

                coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                {
                    tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto,
                    latitude = latitude,
                    longitude = longitude,
                    RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>(),
                    PrimeiraEntrega = false,
                    CodigoOutroEndereco = codigoOutroEndereco
                },
                coleta = coleta,
                DataAgendamento = dataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                TipoPonto = coleta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
            };

            List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricaoEntrega = pessoa.ClienteDescargas?.FirstOrDefault()?.RestricoesDescarga.ToList();
            if (restricaoEntrega != null && restricaoEntrega.Count > 0)
            {
                rotaInformacao.coordenadas.RestricoesEntregas = (from restricao in restricaoEntrega
                                                                 select new Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega()
                                                                 {
                                                                     Codigo = restricao.Codigo,
                                                                     Descricao = restricao.Descricao,
                                                                     Observacao = restricao.Observacao,
                                                                     PrimeiraEntrega = restricao.PrimeiraEntrega,
                                                                     CorVisualizacao = restricao.CorVisualizacao,
                                                                 }).ToList();
            }

            rotaInformacao.pessoa.CodigoIntegracao = pessoa.CodigoIntegracao ?? string.Empty;
            rotasInformacaoPessoa.Add(rotaInformacao);
        }

        #endregion
    }
}
