using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public class MontagemCargaRoteirizacao
    {
        #region Metodos Publicos

        public static void SetarPontosPassagem(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, string pontos, bool validaCodigoClientePonto, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(pontos))
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repositorioRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);

            repCarregamentoRoteirizacaoPontosPassagem.DeletarPorCarregamentoRoteirizado(carregamentoRoteirizacao.Codigo);

            bool calcularDistanciaDireta = repositorioRateioFormula.ExisteFormulaRateioConfiguradaPorFatorPonderacao();
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointPontoPartida = null;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontos);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem>();
            for (int i = 0; i < pontosDaRota.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = pontosDaRota[i];
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem carregamentoRoteirizacaoPontoPassagem = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem();
                carregamentoRoteirizacaoPontoPassagem.CarregamentoRoteirizacao = carregamentoRoteirizacao;
                if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Apoio || pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balanca)
                {
                    carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem;
                    if (pontoRota.codigo > 0)
                        carregamentoRoteirizacaoPontoPassagem.PontoDeApoio = new Dominio.Entidades.Embarcador.Logistica.Locais() { Codigo = (int)pontoRota.codigo };
                }
                else if (pontoRota.pontopassagem)
                {
                    carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem;
                }
                else if (pontoRota.pedagio)
                {
                    carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio;
                    carregamentoRoteirizacaoPontoPassagem.PracaPedagio = new Dominio.Entidades.Embarcador.Logistica.PracaPedagio() { Codigo = (int)pontoRota.codigo };
                }
                else if (pontoRota.fronteira)
                {
                    carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira;
                    if (pontoRota.codigo > 0)
                        carregamentoRoteirizacaoPontoPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);

                    if (carregamentoRoteirizacaoPontoPassagem.Cliente == null && pontoRota.codigo_cliente > 0)
                        carregamentoRoteirizacaoPontoPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo_cliente);
                }
                else
                {
                    //TODO: rever regra para incluir mais que uma coleta.
                    if (i == 0 || pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                        carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta;
                    else if (i == pontosDaRota.Count - 1)
                    {
                        if (carregamentoRoteirizacao.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                            carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;
                        else
                            carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno;
                    }
                    else
                        carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;

                    //Foi adicionado o codigo_cliente e o em algum processo está passando o ponto.Codigo um valor que não é o código do cliente
                    // gerando erro ao tentar incluir.. erro de FK.
                    if (validaCodigoClientePonto)
                    {
                        double cli_cgccpf = repCliente.ValidaCPFCNPJCliente(pontoRota.codigo, pontoRota.codigo_cliente);
                        if (cli_cgccpf > 0) // && !pontoRota.usarOutroEndereco)
                            carregamentoRoteirizacaoPontoPassagem.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = cli_cgccpf };
                        //else
                        //{
                        if (pontoRota.codigo > 0 && pontoRota.usarOutroEndereco)
                            carregamentoRoteirizacaoPontoPassagem.ClienteOutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco() { Codigo = (int)pontoRota.codigo };
                        else if (pontoRota.codigoOutroEndereco > 0)
                            carregamentoRoteirizacaoPontoPassagem.ClienteOutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco() { Codigo = pontoRota.codigoOutroEndereco };
                        //}
                    }
                    else
                    {
                        if (pontoRota.codigo_cliente > 0)
                            carregamentoRoteirizacaoPontoPassagem.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = pontoRota.codigo_cliente };
                        else if (pontoRota.codigo > 0)
                            carregamentoRoteirizacaoPontoPassagem.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = pontoRota.codigo };
                    }
                }

                carregamentoRoteirizacaoPontoPassagem.Distancia = pontoRota.distancia;
                carregamentoRoteirizacaoPontoPassagem.Tempo = pontoRota.tempo;
                carregamentoRoteirizacaoPontoPassagem.Ordem = i;
                carregamentoRoteirizacaoPontoPassagem.Latitude = (decimal)pontoRota.lat;
                carregamentoRoteirizacaoPontoPassagem.Longitude = (decimal)pontoRota.lng;

                if (calcularDistanciaDireta && !CalcularDistanciaDireta(ref carregamentoRoteirizacaoPontoPassagem, pontoRota, ref wayPointPontoPartida, unitOfWork))
                    throw new ServicoException($"Não foi possível obter uma distância maior que zero. Pontos:{pontos} - CarregamentoRoteirizacaoPontosPassagem: {carregamentoRoteirizacao.Codigo}");

                lista.Add(carregamentoRoteirizacaoPontoPassagem);
            }
            repCarregamentoRoteirizacaoPontosPassagem.InserirSQL(lista);
        }

        public static string ObterPontosPassagemCarregamentoRoteirizacaoSerializada(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> pontosPassagem = repCarregamentoRoteirizacaoPontosPassagem.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
            for (int i = 0; i < pontosPassagem.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem pontoPassagem = pontosPassagem[i];
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();

                pontoRota.descricao = pontoPassagem.Descricao;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                {
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;
                    pontoRota.fronteira = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontoRota.codigoOutroEndereco = pontoPassagem?.ClienteOutroEndereco?.Codigo ?? 0;
                pontosDaRota.Add(pontoRota);
            }
            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }

        public static string ObterDadosRotaFrete(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> pontosPassagem = repCarregamentoRoteirizacaoPontosPassagem.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
            for (int i = 0; i < pontosPassagem.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem pontoPassagem = pontosPassagem[i];
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();

                pontoRota.descricao = pontoPassagem.Descricao;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente.CPF_CNPJ;

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontosDaRota.Add(pontoRota);
            }
            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }

        #endregion

        #region Metodos Publico

        private static bool CalcularDistanciaDireta(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem carregamentoRoteirizacaoPontoPassagem, Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota, ref Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointPontoPartida, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta && wayPointPontoPartida == null)
                wayPointPontoPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(pontoRota.lat, pontoRota.lng);

            if (wayPointPontoPartida != null && carregamentoRoteirizacaoPontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
            {
                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
                rota.Add(wayPointPontoPartida);
                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(pontoRota.lat, pontoRota.lng));

                Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                {
                    AteOrigem = false,
                    Ordenar = false,
                    PontosNaRota = false
                };

                int tentativas = 3;
                int distancia = 0;

                while (tentativas > 0 && distancia <= 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = rota.Roteirizar(opcoes);
                    carregamentoRoteirizacaoPontoPassagem.DistanciaDireta = (int)(respostaRoteirizacao.Distancia * 1000);
                    distancia = carregamentoRoteirizacaoPontoPassagem.DistanciaDireta;

                    tentativas--;
                }

                if (distancia <= 0)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
