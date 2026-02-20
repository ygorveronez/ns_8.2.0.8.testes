using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao.Trizy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaRotaFrete
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaRotaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private static void AtualizarCoordenadasCidadesDoClientes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pessoas, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, /*string key,*/ bool atualizarSempre, bool atualizarLatitudePessoa = false)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            //Pessoas
            foreach (var pessoa in pessoas)
            {
                if (pessoa == null)
                    continue;

                AtualizarCoordenadasClientePorCidade(pessoa.Cliente, configuracaoIntegracao, unitOfWork, atualizarLatitudePessoa);
            }
        }

        private static void AtualizarCoordenadasClientePorCidade(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, /*string key,*/ Repositorio.UnitOfWork unitOfWork, bool atualizarLatitudePessoa = true)
        {
            if (pessoa?.Localidade == null)
                return;

            if (pessoa?.NaoAtualizarDados ?? false)
                return;

            if ((pessoa?.Localidade?.Latitude == null || pessoa?.Localidade?.Longitude == null) || (pessoa.Localidade.Latitude == 0 || pessoa.Localidade.Longitude == 0))
                AtualizarCoordenadasCidade(pessoa.Localidade, configuracaoIntegracao, unitOfWork);

            if ((pessoa?.Localidade?.Latitude != null && pessoa?.Localidade?.Longitude != null) && (pessoa.Localidade.Latitude != 0 && pessoa.Localidade.Longitude != 0) && (atualizarLatitudePessoa))
            {
                pessoa.Latitude = pessoa.Localidade.Latitude.ToString().Replace(",", ".");
                pessoa.Longitude = pessoa.Localidade.Longitude.ToString().Replace(",", ".");
                pessoa.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado;
                pessoa.GeoLocalizacaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo.Localidade;
                pessoa.DataUltimaAtualizacao = DateTime.Now;
                pessoa.Integrado = false;
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                repCliente.Atualizar(pessoa);
            }
        }

        /// <summary>
        /// Utilizando pela Frimesa, quando cadastrado um lat x lng na "Localidade" do destinatário vamos entregar nessa coordenada.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        private static PointF LocalEntregaCliente(Dominio.Entidades.Cliente cliente)
        {
            if ((cliente?.Localidade?.LatitudeEntrega ?? 0) != 0 && (cliente?.Localidade?.LongitudeEntrega ?? 0) != 0)
                return new PointF((float)cliente.Localidade.LongitudeEntrega, (float)cliente.Localidade.LatitudeEntrega);
            else
                return new PointF((float)ParseDouble(cliente.Longitude), (float)ParseDouble(cliente.Latitude));
        }

        private static Dominio.Entidades.RotaFrete ObterRotaPorOrigemDestino(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork)
        {

            if ((origem == null) || (destino == null))
                return null;

            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            return repRotaFrete.BuscarPorOrigemDestino(origem, destino, true);
        }

        public static Logistica.Nominatim.Service ObterServiceNominatim(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Logistica.Nominatim.Service nominatim = new Logistica.Nominatim.Service();
            if (!string.IsNullOrEmpty(configuracaoIntegracao?.ServidorNominatim ?? null))
                nominatim.url_servico = configuracaoIntegracao.ServidorNominatim;
            return nominatim;
        }

        public static Servicos.Embarcador.Logistica.GeoCode ObterServiceGeocodingGoogle(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new Servicos.Embarcador.Logistica.GeoCode(configuracaoIntegracao?.APIKeyGoogle);
        }

        /// <summary>
        /// Utilizado na atualização de coordenadas das Cidades...
        /// </summary>
        /// <param name="endereco">Endereço para ser pesquisado... se google formato Google, se Nominatim formato Nominatim...</param>
        /// <param name="enderecoGoogle">Formatação de endereço de pesquisa Google....</param>
        /// <param name="configuracaoIntegracao">Configuração Integração</param>
        /// <returns>Geolocalização do endereço pesquisado, em caso da configuração pesquisar pelo serviço Nominatim e der erro na pesquisa, irá pesquisar pelo enderecoGoogle na api do Google.</returns>
        private static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterWayPointEndereco(string endereco, string enderecoGoogle, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Localidade localidade)
        {
            GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? GeoServiceGeocoding.Google);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = null;

            if (geoServiceGeocoding == GeoServiceGeocoding.Nominatim)
            {
                Logistica.Nominatim.Service nominatim = ObterServiceNominatim(configuracaoIntegracao);
                Logistica.Nominatim.RootObject rootObject = nominatim.Geocoding(endereco);
                if (rootObject != null)
                    point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = rootObject.lat.ToDouble(), Lng = rootObject.lon.ToDouble() };

                //Se não encontrou no Nominatim, busca por CEP no cadastro MultisoftwareAdmin.
                if (point == null)
                {
                    string cep = ObtemCepEnderecoNominatim(endereco);
                    if (!String.IsNullOrEmpty(cep))
                        point = ObterCoordenadasPorCep(cep.ObterSomenteNumeros().ToInt(), new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin()));
                }

                //Se não encontrou no cadastro MultisoftwareAdmin, busca por Localidade.
                if (point == null && localidade != null && localidade.Latitude.HasValue && localidade.Longitude.HasValue)
                    point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = (double)localidade.Latitude, Lng = (double)localidade.Longitude };

            }

            // Se a configuração for Google
            if (geoServiceGeocoding == GeoServiceGeocoding.Google)
            {
                Servicos.Embarcador.Logistica.GeoCode geo = ObterServiceGeocodingGoogle(configuracaoIntegracao);
                point = geo.BuscarLatLng(enderecoGoogle);
            }

            return point;
        }

        private static bool ValidarCoordenadaDentroLocalidade(Dominio.Entidades.Localidade localidade, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint point, ref string poligono, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            if ((localidade?.CodigoIBGE ?? 0) == 0)
                return true;

            AdminMultisoftware.Repositorio.Localidades.LocalidadeGeo repositorioLocalidadeGeo = new AdminMultisoftware.Repositorio.Localidades.LocalidadeGeo(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.LocalidadeGeo localidadeGeo = repositorioLocalidadeGeo.BuscarLocalidade(localidade.CodigoIBGE);

            //#59106 - Validar se a coordenada está dentro da localidade...
            if (!string.IsNullOrWhiteSpace(localidadeGeo?.geom) && (point.Latitude != 0 && point.Longitude != 0))
            {
                poligono = localidadeGeo.geom;
                // POLYGON ((-35.798748 -6.042688, -35.805682 -6.043288, -35.822631 -6.07298, -35.924476 -6.010235, -35.834445 -5.9467, -35.77801 -6.015479, -35.798748 -6.042688))

                //MULTIPOLYGON (((-34.864957 -7.00112, -34.86792 -6.986892, -34.8654 -6.980439, -34.860523 -6.979999, -34.857198 -6.982419, -34.848775 -6.981759, -34.846989 -6.982363, -34.846834 -6.988959, -34.849307 -7.001383, -34.85742 -7.014541, -34.860589 -7.014574, -34.865071 -7.004911, -34.864957 -7.00112)),
                //              ((-34.842191 -7.055604, -34.84545 -7.056563, -34.849697 -7.061642, -34.846519 -7.075851, -34.847001 -7.084504, -34.846983 -7.093183, -34.844973 -7.099319, -34.848528 -7.099626, -34.851001 -7.097632, -34.851921 -7.097089, -34.854563 -7.0964, -34.854505 -7.095654, -34.855252 -7.095194, -34.85709 -7.095022, -34.858008 -7.095424, -34.859194 -7.095484, -34.859501 -7.093185, -34.858755 -7.092381, -34.857434 -7.090371, -34.857434 -7.088993, -34.858181 -7.088074, -34.859674 -7.087844, -34.862602 -7.087959, -34.863464 -7.086409, -34.863579 -7.08371, -34.864498 -7.082734, -34.865761 -7.081241, -34.865818 -7.079633, -34.8649 -7.077623, -34.863406 -7.076992, -34.859616 -7.075901, -34.858884 -7.075084, -34.859042 -7.073317, -34.859789 -7.07079, -34.8591 -7.068263, -34.85709 -7.065852, -34.856802 -7.063784, -34.858238 -7.062062, -34.860937 -7.061545, -34.86421 -7.05942, -34.861632 -7.055243, -34.856533 -7.048423, -34.854981 -7.039402, -34.854808 -7.02711, -34.853651 -7.022021, -34.851684 -7.016568, -34.848309 -7.011003, -34.840962 -7.003991, -34.83881 -7.001906, -34.836913 -7.000829, -34.835286 -6.998677, -34.834144 -6.99408, -34.834202 -6.987378, -34.835286 -6.983477, -34.835828 -6.978635, -34.842816 -6.966104, -34.842876 -6.963704, -34.841889 -6.963037, -34.83648 -6.964724, -34.831898 -6.965318, -34.829594 -6.966798, -34.828781 -6.968815, -34.827968 -6.972582, -34.827155 -6.988185, -34.826206 -6.995852, -34.824987 -7.002847, -34.825122 -7.005672, -34.828104 -7.015088, -34.829052 -7.021276, -34.829046 -7.030822, -34.832814 -7.033682, -34.840573 -7.044462, -34.842191 -7.055604)))

                //string poligonoLocalidade = localidadeGeo?.geom.Replace(")), ((", ";").Replace("MULTIPOLYGON (((", "").Replace(")))", "").Replace("POLYGON ((", "").Replace("))", "");

                //bool contem = false;
                //string[] poligonsLocalidade = poligonoLocalidade.Split(';');
                //foreach (string poligon in poligonsLocalidade)
                //{
                //    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPoligono = (from obj in poligon.Split(',')
                //                                                                                      select new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(obj.Trim().Split(' ')[1].Replace("(", "").Replace(")", ""), obj.Trim().Split(' ')[0].Replace("(", "").Replace(")", ""))).ToList();
                //    contem = Logistica.Distancia.ValidarPoligono(point, coordenadasPoligono.ToArray());
                //    if (contem)
                //        break;
                //}
                //return contem;
                return ValidarCoordenadaDentroGeom(localidadeGeo.geom, point);
            }
            return true;
        }

        private static bool ValidarCoordenadaDentroGeom(string geom, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint point)
        {
            string poligono = geom.Replace(")), ((", ";").Replace("MULTIPOLYGON (((", "").Replace(")))", "").Replace("POLYGON ((", "").Replace("))", "");
            bool contem = false;
            string[] poligons = poligono.Split(';');
            foreach (string poligon in poligons)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPoligono = (from obj in poligon.Split(',')
                                                                                                  select new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(obj.Trim().Split(' ')[1].Replace("(", "").Replace(")", ""), obj.Trim().Split(' ')[0].Replace("(", "").Replace(")", ""))).ToList();
                contem = Logistica.Distancia.ValidarPoligono(point, coordenadasPoligono.ToArray());
                if (contem)
                    break;
            }
            return contem;
        }

        /// <summary>
        /// Método para verificar se as coordenadas de uma pessoa estão fora de um raio cadastrado para a cidade para trocar pelo lat, lng da localidade.
        /// Troca somente se a configuraççao do raio for maior que "0" ou se a coordenada está fora do poligono da cidade... localidade.Poligono.
        /// </summary>
        /// <param name="pessoa">Pessoa com as coordenadas a serem validadas...</param>
        /// <param name="coordenadaForaPoligono">Se já foi validado, e a coordenada da pessoa está fora do poligono da localidade.</param>
        /// <param name="configuracao"></param>
        /// <returns></returns>
        private static bool CoordenadaPessoaForaRaioLocalidade(Dominio.Entidades.Cliente pessoa, bool coordenadaForaPoligono, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (pessoa?.NaoAtualizarDados ?? false)
                return false;

            //#20810 - Validar se a configuração em raio maximo estiver marcado..
            if (configuracao.RaioMaximoGeoLocalidadeGeoCliente > 0 || coordenadaForaPoligono)
            {
                if ((pessoa?.Localidade?.Latitude != null && pessoa?.Localidade?.Longitude != null) && (pessoa.Localidade.Latitude != 0 && pessoa.Localidade.Longitude != 0))
                {
                    double latitude_dest = ParseDouble(pessoa.Latitude);
                    double longitude_dest = ParseDouble(pessoa.Longitude);
                    if ((pessoa?.Localidade?.Latitude != null && pessoa?.Localidade?.Longitude != null) && (pessoa.Localidade.Latitude != 0 && pessoa.Localidade.Longitude != 0))
                    {
                        double latLocalidade = (double)(pessoa?.Localidade?.Latitude.Value ?? 0);
                        double lngLocalidade = (double)(pessoa?.Localidade?.Longitude.Value ?? 0);
                        double distancia = Logistica.Polilinha.CalcularDistancia(latLocalidade, lngLocalidade, latitude_dest, longitude_dest) / 1000;
                        if (distancia > configuracao.RaioMaximoGeoLocalidadeGeoCliente)
                        {
                            pessoa.Latitude = pessoa.Localidade.Latitude.ToString().Replace(",", ".");
                            pessoa.Longitude = pessoa.Localidade.Longitude.ToString().Replace(",", ".");
                            pessoa.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado;
                            pessoa.GeoLocalizacaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo.Localidade;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region Métodos Públicos

        public void GerarRoteirizacaoManual(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarCarregamentoRoteirizacao = false)
        {
            if (!carga.CargaRotaFreteInformadaViaIntegracao)
                return;

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> remetentes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();

            cargaPedidos = cargaPedidos.OrderBy(obj => obj.OrdemColeta).ThenBy(obj => obj.OrdemEntrega).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(o => o.Pedido).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.Entidades.Cliente remetente = pedido.Remetente;
                Dominio.Entidades.Cliente destinatario = pedido.Destinatario;

                if (!remetentes.Exists(o => o.Cliente.Codigo == remetente.Codigo))
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco remetenteOutroEndereco = pedido.UsarOutroEnderecoOrigem ? pedido.EnderecoOrigem?.ClienteOutroEndereco : null;

                    remetentes.Add(ObterClienteTipoPonto(remetente, TipoPontoPassagem.Coleta, coletaEquipamento: false, remetenteOutroEndereco, sequenciaPreDefinida: 0));
                }

                if (!destinatarios.Exists(o => o.Cliente.Codigo == destinatario.Codigo))
                {
                    if (destinatario?.CPF_CNPJ_SemFormato == carga.Empresa?.CNPJ_SemFormato && cargaPedidos.Where(obj => obj.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ).Any(obj => obj.PedidoPallet))
                        continue;

                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco destinatarioOutroEndereco = pedido.UsarOutroEnderecoDestino ? pedido.EnderecoDestino?.ClienteOutroEndereco : null;

                    destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, coletaEquipamento: false, destinatarioOutroEndereco, sequenciaPreDefinida: 0));
                }
            }

            //#62188
            if ((carga.Rota?.SituacaoDaRoteirizacao ?? SituacaoRoteirizacao.Erro) == SituacaoRoteirizacao.Concluido && (carga.Rota?.RotaRoteirizadaPorLocal ?? false) && !string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
            {
                if ((carga.Rota?.PontoPassagemPreDefinido?.Count ?? 0) > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosPreDefinidos = (from obj in carga.Rota.PontoPassagemPreDefinido
                                                                                                                          select new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                                                                                                          {
                                                                                                                              TipoPontoPassagem = TipoPontoPassagem.Passagem,
                                                                                                                              Codigo = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                              Cliente = new Dominio.Entidades.Cliente()
                                                                                                                              {
                                                                                                                                  CPF_CNPJ = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                                  CodigoIntegracao = obj.Codigo.ToString(),
                                                                                                                                  Latitude = obj.ObterLatitude(),
                                                                                                                                  Longitude = obj.ObterLongitude(),
                                                                                                                                  Nome = obj.ObterDescricao() ?? "Passagem",
                                                                                                                                  Tipo = "Passagem"
                                                                                                                              }
                                                                                                                          }).ToList();
                    if (destinatarios.Count > 0)
                        destinatarios.InsertRange(0, pontosPreDefinidos);
                    else
                        destinatarios.AddRange(pontosPreDefinidos);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();
            pontoPartida = remetentes.FirstOrDefault();
            remetentes.Remove(pontoPartida);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> rota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

            double lat;
            double lng;

            string infoRota = pontoPartida.Cliente.CodigoIntegracao != null ? pontoPartida.Cliente.CodigoIntegracao.ToString() : pontoPartida.Cliente.CPF_CNPJ > 0 ? pontoPartida.Cliente.CPF_CNPJ.ToString() : "";

            lat = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Latitude : pontoPartida.Cliente.Latitude);
            lng = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Longitude : pontoPartida.Cliente.Longitude);

            rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
            {
                Lat = lat,
                Lng = lng,
                Codigo = pontoPartida.Codigo,
                CodigoCliente = pontoPartida.Cliente.Codigo,
                Descricao = pontoPartida.Cliente.Nome,
                Pedagio = false,
                UsarOutroEndereco = pontoPartida.UsarOutroEndereco,
                Informacao = infoRota,
                TipoPonto = pontoPartida.TipoPontoPassagem,
                ColetaEquipamento = pontoPartida.ColetaEquipamento
            });

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto coleta in remetentes)
            {
                infoRota = (coleta.Cliente.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(coleta.Cliente.CodigoIntegracao)) ? coleta.Cliente.CodigoIntegracao.ToString() : coleta.Cliente.CPF_CNPJ > 0 ? coleta.Cliente.CPF_CNPJ.ToString() : "";

                PointF geolocalizacao = LocalEntregaCliente(coleta.Cliente);

                lat = ParseDouble(coleta.UsarOutroEndereco ? coleta.ClienteOutroEndereco.Latitude : geolocalizacao.Y.ToString());
                lng = ParseDouble(coleta.UsarOutroEndereco ? coleta.ClienteOutroEndereco.Longitude : geolocalizacao.X.ToString());

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Lat = lat,
                    Lng = lng,
                    Codigo = coleta.Codigo,
                    CodigoCliente = coleta.Cliente.Codigo,
                    Descricao = coleta.Cliente.Nome,
                    Pedagio = false,
                    UsarOutroEndereco = coleta.UsarOutroEndereco,
                    Informacao = infoRota,
                    TipoPonto = TipoPontoPassagem.Coleta
                });
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto destinatario in destinatarios)
            {
                infoRota = (destinatario.Cliente.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(destinatario.Cliente.CodigoIntegracao)) ? destinatario.Cliente.CodigoIntegracao.ToString() : destinatario.Cliente.CPF_CNPJ > 0 ? destinatario.Cliente.CPF_CNPJ.ToString() : "";

                PointF geolocalizacao = LocalEntregaCliente(destinatario.Cliente);

                lat = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Latitude : geolocalizacao.Y.ToString());
                lng = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Longitude : geolocalizacao.X.ToString());

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Lat = lat,
                    Lng = lng,
                    Codigo = destinatario.Codigo,
                    CodigoCliente = destinatario.Cliente.Codigo,
                    Descricao = destinatario.Cliente.Nome,
                    Pedagio = false,
                    Informacao = infoRota,
                    UsarOutroEndereco = destinatario.UsarOutroEndereco,
                    TipoPonto = (destinatario.Cliente.Tipo == "PontoApoio" ? TipoPontoPassagem.Apoio : (destinatario.Cliente.Tipo == "Passagem" ? TipoPontoPassagem.Passagem : TipoPontoPassagem.Entrega)),
                });
            }

            bool ateOrigem = cargaRotaFrete.TipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.AteOrigem || cargaRotaFrete.TipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.Retornando;
            if (ateOrigem)
            {
                lat = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Latitude : pontoPartida.Cliente.Latitude);
                lng = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Longitude : pontoPartida.Cliente.Longitude);

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Lat = lat,
                    Lng = lng,
                    Codigo = pontoPartida.Codigo,
                    CodigoCliente = pontoPartida.Cliente.Codigo,
                    Descricao = pontoPartida.Cliente.Nome,
                    Pedagio = false,
                    Informacao = infoRota,
                    UsarOutroEndereco = pontoPartida.UsarOutroEndereco,
                    TipoPonto = TipoPontoPassagem.Retorno,
                    ColetaEquipamento = pontoPartida.ColetaEquipamento
                });
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
            clientes.Add(pontoPartida);
            clientes.AddRange(remetentes);
            clientes.AddRange(destinatarios);

            Servicos.Embarcador.Logistica.Roteirizacao servicoRoteirizacao = new Servicos.Embarcador.Logistica.Roteirizacao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = Servicos.Embarcador.Carga.RotaFrete.SetarPontosPassagemCarga(carga, cargaPedidos, cargaRotaFrete, servicoRoteirizacao.GetListaPontos(rota), clientes, null, _unitOfWork, gerarCarregamentoRoteirizacao);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagem, true, configuracao, _unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, _unitOfWork, tipoServicoMultisoftware);
            //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagio(carga, _unitOfWork);

            carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
            repCarga.Atualizar(carga);
        }

        public int CalcularDistanciaPorOrigemEDestino(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            if (!origem.Latitude.HasValue || !origem.Longitude.HasValue || !destino.Latitude.HasValue || !destino.Longitude.HasValue)
                return 0;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

            string configuracaoIntegracaoServidorRouteOSM = repositorioConfiguracaoIntegracao.BuscarConfiguracaoIntegracaoServidorRouteOSM();

            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracaoServidorRouteOSM);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
            wayPoints.AddRange(new[]
            {
                rota.ObterWaypoint((double)origem.Latitude.Value, (double)origem.Longitude.Value, origem.Descricao, 0, TipoPontoPassagem.Coleta),
                rota.ObterWaypoint((double)destino.Latitude.Value, (double)destino.Longitude.Value, destino.Descricao, 1, TipoPontoPassagem.Entrega)
            });

            rota.Add(wayPoints);

            Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar();

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = rota.Roteirizar(opcoes);
            return (int)Math.Round(respostaRoteirizacao.Distancia, 0, MidpointRounding.AwayFromZero);
        }

        public (Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem) DuplicarCargaRotaFrete(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFreteAntigo = repCargaRotaFrete.BuscarPorCarga(cargaAntiga.Codigo);

            if (cargaRotaFreteAntigo == null)
                return (null, null);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFreteNovo = cargaRotaFreteAntigo.Clonar();

            Utilidades.Object.DefinirListasGenericasComoNulas(cargaRotaFreteNovo);
            cargaRotaFreteNovo.Carga = cargaNova;
            repCargaRotaFrete.Inserir(cargaRotaFreteNovo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> listaCargaRotaFretePontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFreteAntigo.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem = new();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontosPassagemAntigo in listaCargaRotaFretePontosPassagem)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontosPassagemNovo = cargaRotaFretePontosPassagemAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(cargaRotaFretePontosPassagemNovo);
                cargaRotaFretePontosPassagemNovo.CargaRotaFrete = cargaRotaFreteNovo;
                cargaRotaFretePontosPassagem.Add(cargaRotaFretePontosPassagemNovo);
                repCargaRotaFretePontosPassagem.Inserir(cargaRotaFretePontosPassagemNovo);
            }

            return (cargaRotaFreteNovo, cargaRotaFretePontosPassagem);
        }

        #endregion

        #region Métodos Públicos Estáticos

        private static string StringConexaoAdmin()
        {
            return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
        }

        private static string ObtemCepEnderecoNominatim(string endereco)
        {
            string cep = String.Empty;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\d{5}-\d{3}|\d{8}");
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(endereco))
            {
                cep = match.Value.ToString().Replace("-", "");
                if (cep.Length == 8) break;
            }
            if (cep.Length != 8) cep = String.Empty;
            return cep;
        }

        public static void AtualizarCoordenadas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pessoas, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao /*, string key*/, bool atualizarSempre, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (pessoas == null || pessoas.Count == 0)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Cliente> clientesAtualizar = new List<Dominio.Entidades.Cliente>();

            Servicos.Embarcador.Logistica.GeoCode geo = ObterServiceGeocodingGoogle(configuracaoIntegracao);

            Logistica.Nominatim.Service nominatim = ObterServiceNominatim(configuracaoIntegracao);
            GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? GeoServiceGeocoding.Google);

            string stringAdmin = StringConexaoAdmin();
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringAdmin);
            try
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto ponto in pessoas)
                {
                    Dominio.Entidades.Cliente pessoa = ponto?.Cliente;
                    if (pessoa == null)
                        continue;
                    else if (pessoa.NaoAtualizarDados)
                        continue;


                    if (!ValidarCoordenadas(pessoa?.Latitude) || !ValidarCoordenadas(pessoa?.Longitude) || atualizarSempre)
                    {
                        string enderecoGoogle = string.Empty;
                        string bairroNominatim = string.Empty;
                        string endereco = ObterEndereco(pessoa, configuracaoIntegracao, ref enderecoGoogle, ref bairroNominatim);

                        Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado;

                        bool atualizarPorCidade = false;
                        if (geoServiceGeocoding == GeoServiceGeocoding.Nominatim)
                        {
                            Logistica.Nominatim.RootObject rootObject = nominatim.Geocoding(endereco);
                            if (rootObject == null && !pessoa.TipoLogradouro.HasValue)
                            {
                                rootObject = nominatim.Geocoding("RUA " + endereco);
                                if (rootObject == null)
                                    rootObject = nominatim.Geocoding("AVENIDA " + endereco);
                            }

                            if (rootObject == null && !string.IsNullOrWhiteSpace(bairroNominatim))
                                rootObject = nominatim.Geocoding(endereco.Replace(bairroNominatim, ""));

                            if (rootObject != null)
                            {
                                point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = rootObject.lat.ToDouble(), Lng = rootObject.lon.ToDouble() };
                                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.GeradoNominatim;
                            }
                            else if (pessoa.Tipo == "F")
                                atualizarPorCidade = true;

                            //Se não encontrou no Nominatim, busca por CEP no cadastro MultisoftwareAdmin.
                            if (point == null)
                                point = ObterCoordenadasPorCep(pessoa.CEP.ObterSomenteNumeros().ToInt(), unitOfWorkAdmin);

                            //Se não encontrou no cadastro MultisoftwareAdmin, assume Latitude e Longitude do cliente.
                            if (point == null && !String.IsNullOrEmpty(pessoa.Latitude) && !String.IsNullOrEmpty(pessoa.Longitude))
                                point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = Convert.ToDouble(pessoa.Latitude), Lng = Convert.ToDouble(pessoa.Longitude) };
                        }

                        // Se a configuração for Google
                        if (geoServiceGeocoding == GeoServiceGeocoding.Google && !atualizarPorCidade)
                        {
                            Servicos.Log.TratarErro(pessoa.Codigo + " - " + enderecoGoogle, "GoogleGeocoding");
                            point = geo.BuscarLatLng(enderecoGoogle);
                        }

                        string poligono = string.Empty;

                        bool coordenadaForaCidade = false;
                        if (point != null && !ValidarCoordenadaDentroLocalidade(pessoa.Localidade, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(point?.Lat ?? 0, point?.Lng ?? 0), ref poligono, unitOfWorkAdmin))
                            coordenadaForaCidade = true;

                        if (point == null || coordenadaForaCidade)
                            AtualizarCoordenadasClientePorCidade(pessoa, configuracaoIntegracao, unitOfWork);
                        else
                        {
                            pessoa.Latitude = point.Lat.ToString().Replace(",", ".");
                            pessoa.Longitude = point.Lng.ToString().Replace(",", ".");

                            pessoa.GeoLocalizacaoStatus = status;
                            pessoa.GeoLocalizacaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo.Endereco;

                            if (pessoa.RaioEmMetros == null || pessoa.RaioEmMetros == 0)
                                pessoa.RaioEmMetros = 300;

                            //Validando se a coordenada estiver fora do poligono da cidade.. vamos chamar para ajustar pela localidade.... ou se não existe poligono e existe configuração de raio..
                            if (!ValidarCoordenadaDentroLocalidade(pessoa.Localidade, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pessoa.Latitude, pessoa.Longitude), ref poligono, unitOfWorkAdmin))
                                CoordenadaPessoaForaRaioLocalidade(pessoa, true, configuracao);
                            else if (string.IsNullOrEmpty(poligono))
                                CoordenadaPessoaForaRaioLocalidade(pessoa, false, configuracao);

                            clientesAtualizar.Add(pessoa);
                        }
                    }
                    else
                    {
                        string poligono = string.Empty;
                        if (!ValidarCoordenadaDentroLocalidade(pessoa.Localidade, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pessoa.Latitude, pessoa.Longitude), ref poligono, unitOfWorkAdmin))
                        {
                            if (CoordenadaPessoaForaRaioLocalidade(pessoa, true, configuracao))
                                clientesAtualizar.Add(pessoa);
                        }
                        else if (string.IsNullOrWhiteSpace(poligono))
                        {
                            if (CoordenadaPessoaForaRaioLocalidade(pessoa, false, configuracao))
                                clientesAtualizar.Add(pessoa);
                        }
                    }
                }

                if (clientesAtualizar.Count > 0)
                {
                    bool abriuTransacao = false;
                    if (!unitOfWork.IsActiveTransaction())
                    {
                        unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    foreach (Dominio.Entidades.Cliente clienteAtualizar in clientesAtualizar)
                    {
                        clienteAtualizar.DataUltimaAtualizacao = DateTime.Now;
                        clienteAtualizar.Integrado = false;
                        repCliente.Atualizar(clienteAtualizar);
                    }

                    if (abriuTransacao)
                        unitOfWork.CommitChanges();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public static void AtualizarCoordenadasCidade(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao /*, string key*/, Repositorio.UnitOfWork unitOfWork)
        {
            if (localidade == null)
                return;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            string enderecoGoogle = string.Empty;
            string enderecoCidade = ObterEnderecoLocalidade(localidade, configuracaoIntegracao, ref enderecoGoogle);

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = ObterWayPointEndereco(enderecoCidade, enderecoGoogle, configuracaoIntegracao, localidade);

            if (point == null)
            {
                string enderecoCEP = ObterEnderecoLocalidade(localidade, configuracaoIntegracao, ref enderecoGoogle, true);
                if (enderecoCEP != "")
                    point = ObterWayPointEndereco(enderecoCEP, enderecoGoogle, configuracaoIntegracao, localidade);
            }

            if (point != null)
            {
                localidade.Latitude = Convert.ToDecimal(point.Lat);
                localidade.Longitude = Convert.ToDecimal(point.Lng);
                localidade.DataAtualizacao = DateTime.Now;
                repLocalidade.Atualizar(localidade);
            }
        }

        public static void AtualizarCoordenadasOutroEndereco(List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> pessoasOutroEndereco, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao /*, string key*/, bool atualizarSempre, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> pessoasOutroEnderecoAtualizar = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();

            string stringAdmin = StringConexaoAdmin();
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringAdmin);

            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            Servicos.Embarcador.Logistica.GeoCode geo = ObterServiceGeocodingGoogle(configuracaoIntegracao);
            Logistica.Nominatim.Service nominatim = ObterServiceNominatim(configuracaoIntegracao);
            GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? GeoServiceGeocoding.Google);
            try
            {
                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco in pessoasOutroEndereco)
                {
                    if (outroEndereco == null)
                        continue;

                    if (outroEndereco.Cliente?.NaoAtualizarDados ?? false)
                        continue;

                    if (!ValidarCoordenadas(outroEndereco?.Latitude) || !ValidarCoordenadas(outroEndereco?.Longitude) || atualizarSempre)
                    {
                        string enderecoGoogle = string.Empty;
                        string bairroNominatim = string.Empty;
                        string endereco = ObterOutroEndereco(outroEndereco, ref enderecoGoogle, geoServiceGeocoding, ref bairroNominatim);

                        Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = null;
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado;

                        bool atualizarPorCidade = false;
                        if (geoServiceGeocoding == GeoServiceGeocoding.Nominatim)
                        {
                            Logistica.Nominatim.RootObject rootObject = nominatim.Geocoding(endereco);
                            if (rootObject == null && !string.IsNullOrWhiteSpace(bairroNominatim))
                                rootObject = nominatim.Geocoding(endereco.Replace(bairroNominatim, ""));

                            //Problema SAINTGOBAIN, Avenida ROMUALDO ULHOA TOMBA, 53, CENTRO, PARACATU - MINAS GERAIS, BR
                            if (rootObject == null && (endereco.StartsWith("Avenida ") || endereco.StartsWith("Rua ") || endereco.ToUpper().Contains("TREVO")))
                                rootObject = nominatim.Geocoding(endereco.Replace("Avenida ", "").Replace("Rua ", "").Replace(" TREVO,", "").Replace(", TREVO", ""));

                            if (rootObject != null)
                            {
                                point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = rootObject.lat.ToDouble(), Lng = rootObject.lon.ToDouble() };
                                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.GeradoNominatim;
                            }
                            else if (outroEndereco.Cliente.Tipo == "F")
                                atualizarPorCidade = true;

                            //Se não encontrou no Nominatim, busca por CEP no cadastro MultisoftwareAdmin.
                            if (point == null)
                                point = ObterCoordenadasPorCep(outroEndereco.CEP.ObterSomenteNumeros().ToInt(), unitOfWorkAdmin);

                            //Se não encontrou no cadastro MultisoftwareAdmin, assume Latitude e Longitude do outro Endereco.
                            if (point == null && !String.IsNullOrEmpty(outroEndereco.Latitude) && !String.IsNullOrEmpty(outroEndereco.Longitude))
                                point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = Convert.ToDouble(outroEndereco.Latitude), Lng = Convert.ToDouble(outroEndereco.Longitude) };
                        }

                        // Se a configuração for Google
                        if (geoServiceGeocoding == GeoServiceGeocoding.Google && !atualizarPorCidade)
                        {
                            Servicos.Log.TratarErro(outroEndereco.Codigo + " - Outro Endereco - " + enderecoGoogle, "GoogleGeocoding");
                            point = geo.BuscarLatLng(enderecoGoogle);
                        }

                        string poligono = string.Empty;

                        bool coordenadaForaCidade = false;
                        if (point != null && !ValidarCoordenadaDentroLocalidade(outroEndereco.Localidade, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(point?.Lat ?? 0, point?.Lng ?? 0), ref poligono, unitOfWorkAdmin))
                            coordenadaForaCidade = true;
                        else if (point != null && string.IsNullOrWhiteSpace(poligono))
                        {
                            if ((outroEndereco?.Localidade?.Latitude != null && outroEndereco?.Localidade?.Longitude != null) && (outroEndereco.Localidade.Latitude != 0 && outroEndereco.Localidade.Longitude != 0))
                            {
                                double latLocalidade = (double)(outroEndereco?.Localidade?.Latitude.Value ?? 0);
                                double lngLocalidade = (double)(outroEndereco?.Localidade?.Longitude.Value ?? 0);
                                double distancia = Logistica.Polilinha.CalcularDistancia(latLocalidade, lngLocalidade, point.Lat, point.Lng) / 1000;
                                if (distancia > configuracao.RaioMaximoGeoLocalidadeGeoCliente)
                                    atualizarPorCidade = false;
                            }
                        }

                        if ((point == null && atualizarPorCidade) || coordenadaForaCidade)
                            point = ObterCoordenadasPorCidade(outroEndereco.Localidade, configuracaoIntegracao, unitOfWork);

                        if (point != null)
                        {
                            outroEndereco.GeoLocalizacaoStatus = status;
                            outroEndereco.Latitude = point.Lat.ToString().Replace(",", ".");
                            outroEndereco.Longitude = point.Lng.ToString().Replace(",", ".");
                            pessoasOutroEnderecoAtualizar.Add(outroEndereco);
                        }
                        else
                        {
                            outroEndereco.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Erro;
                            pessoasOutroEnderecoAtualizar.Add(outroEndereco);
                        }
                    }
                    else if (outroEndereco.GeoLocalizacaoStatus == GeoLocalizacaoStatus.NaoGerado)
                    {
                        outroEndereco.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;
                        pessoasOutroEnderecoAtualizar.Add(outroEndereco);
                    }
                }

                if (pessoasOutroEnderecoAtualizar.Count > 0)
                {
                    bool abriuTransacao = false;
                    if (!unitOfWork.IsActiveTransaction())
                    {
                        unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco enderecoAtualizar in pessoasOutroEnderecoAtualizar)
                        repClienteOutroEndereco.Atualizar(enderecoAtualizar);

                    if (abriuTransacao)
                        unitOfWork.CommitChanges();
                }
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public static Servicos.Embarcador.Logistica.Nominatim.RootObject Geocodificar(Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosEndereco dadosEndereco, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Servicos.Embarcador.Logistica.GeoCode geo = ObterServiceGeocodingGoogle(configuracaoIntegracao);
            Logistica.Nominatim.Service nominatim = ObterServiceNominatim(configuracaoIntegracao);
            GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? GeoServiceGeocoding.Google);

            string enderecoGoogle = string.Empty;
            string bairroNominatim = string.Empty;
            string endereco = Servicos.Embarcador.Carga.CargaRotaFrete.ObterEndereco(dadosEndereco, configuracaoIntegracao, ref enderecoGoogle, ref bairroNominatim);

            Servicos.Embarcador.Logistica.Nominatim.RootObject rootObject = null;

            if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Nominatim)
            {
                rootObject = nominatim.Geocoding(endereco);
                if (rootObject == null && !string.IsNullOrWhiteSpace(bairroNominatim))
                    rootObject = nominatim.Geocoding(endereco.Replace(bairroNominatim, ""));
            }
            else
            {
                Servicos.Log.TratarErro("Cadastro pessoa - " + enderecoGoogle, "GoogleGeocoding");
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = geo.BuscarLatLng(enderecoGoogle);
                rootObject = new Logistica.Nominatim.RootObject() { lat = point.Lat.ToString(), lon = point.Lng.ToString() };
            }

            return rootObject;
        }

        public static void GerarIntegracoesRoteirizacaoCarga(bool controlePorLote, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unidadeTrabalho, IdentificadorControlePosicaoThread.BuscaCargasSemRoteirizacao);

            List<int> idsCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscaCargasSemRoteirizacao(controlePorLote, configuracao.NumeroTentativasConsultarCargasErroRoteirizacao, configuracao.TempoMinutosAguardarReconsultarCargasErroRoteirizacao, limiteRegistros, configuracao.CalcularFreteInicioCarga));

            foreach (int cargaId in idsCargas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(cargaId);

                    GerarIntegracoesRoteirizacaoCarga(carga, unidadeTrabalho, configuracao, tipoServicoMultisoftware);

                    unidadeTrabalho.FlushAndClear();
                    servicoOrquestradorFila.RegistroLiberadoComSucesso(cargaId);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "GerarIntegracoesRoteirizacaoCarga");
                    servicoOrquestradorFila.RegistroComFalha(cargaId, excecao.Message);
                }
            }
        }

        public static void GerarCargaEntregaPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPendentes = repCarga.BuscarCargasPendentesGeracaoControleColetaEntrega();

                if (cargasPendentes?.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCargas(cargasPendentes.Select(x => x.Codigo).ToList());

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasPendentes)
                    {
                        try
                        {
                            Servicos.Log.TratarErro($"Carga |{carga.Codigo}| 1 - GerarCargaEntregaPendentes", "GerarCargaEntregaPendentes");

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDaCarga = cargaPedidos.Where(x => x.Carga.Codigo == carga.Codigo).ToList();

                            unitOfWork.Start();

                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidosDaCarga, true, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                            carga.GerandoControleColetaEntrega = false;
                            repCarga.Atualizar(carga);

                            Servicos.Log.TratarErro($"Carga |{carga.Codigo}| 2 - GerarCargaEntregaPendentes", "GerarCargaEntregaPendentes");

                            unitOfWork.Flush();
                            unitOfWork.CommitChanges();

                            Servicos.Log.TratarErro($"Carga |{carga.Codigo}| 3 - GerarCargaEntregaPendentes", "GerarCargaEntregaPendentes");
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();

                            unitOfWork.Start();

                            carga.NumeroTentativaGeracaoEntregasControleQualidade += 1;
                            repCarga.Atualizar(carga);

                            unitOfWork.CommitChanges();

                            Servicos.Log.TratarErro(ex, "GerarCargaEntregaPendentes");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "GerarCargaEntregaPendentes");
                unitOfWork.Rollback();
            }
        }

        public static void GerarIntegracoesRoteirizacaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool refazerControleEntrega = true)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repositorioConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unidadeTrabalho);
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioPontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);
            Servicos.Embarcador.Carga.CTe svcCargaCTe = new Servicos.Embarcador.Carga.CTe(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repositorioConfiguracaoTipoOperacaoControleEntrega.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

            bool iniciarTransacao = !unidadeTrabalho.IsActiveTransaction();

            try
            {
                Log.TratarErro($"Iniciou Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosColetaEquipamento = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosPartida = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> remetentes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();

                cargaPedidos = cargaPedidos.OrderBy(obj => obj.OrdemColeta).ThenBy(obj => obj.OrdemEntrega).ToList();
                bool geraColeta = carga.TipoOperacao?.GerarControleColeta ?? false;

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

                if (carga.CargaRotaFreteInformadaViaIntegracao)
                {
                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                    repCarga.Atualizar(carga);

                    bool existeEntregaCarga = repositorioCargaEntrega.BuscarExistePorCarga(carga.Codigo);

                    if (!existeEntregaCarga)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagemCarga = repositorioPontosPassagem.BuscarPorCarga(carga.Codigo);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagemCarga, true, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                    }

                    return;
                }

                if ((retiradaContainer != null) && !carga.LiberadaSemRetiradaContainer)
                    pontosColetaEquipamento.Add(ObterClienteTipoPonto(retiradaContainer.Local, TipoPontoPassagem.Coleta, coletaEquipamento: true, clienteOutroEndereco: null, 0));

                Log.TratarErro($"__________| Roteirizacao - Iniciou ObterDestinatarios - {carga.Codigo} |__________", "GeracaoControleEntrega");

                StringBuilder problemasRoteirizacao = new StringBuilder();

                servicoMensagemAlerta.Remover(carga, TipoMensagemAlerta.ProblemaRoterizacao);

                Dictionary<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto, DateTime?> dicColetas = new Dictionary<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto, DateTime?>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if ((cargaPedido.PontoPartida != null) && cargaPedido.PossuiColetaEquipamentoPontoPartida)
                        pontosColetaEquipamento.Add(ObterClienteTipoPonto(cargaPedido.PontoPartida, TipoPontoPassagem.Coleta, coletaEquipamento: true, clienteOutroEndereco: null, geraColeta ? cargaPedido.OrdemColeta : 0));

                    if ((cargaPedido.PontoPartida != null) && !cargaPedido.PossuiColetaEquipamentoPontoPartida)
                        pontosPartida.Add(ObterClienteTipoPonto(cargaPedido.PontoPartida, TipoPontoPassagem.Passagem, coletaEquipamento: false, clienteOutroEndereco: null, geraColeta ? cargaPedido.OrdemColeta : 0));

                    bool obterRemetentesEDestinatariosPorNota = (
                        cargaPedido.TipoRateio != TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado &&
                        carga.ExigeNotaFiscalParaCalcularFrete &&
                        svcCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes, tipoServicoMultisoftware)
                    );

                    //Caso necessita da roteirizacao antes da nota, apenas com os pedidos e apos confirmacao dos documentos a carga é novamente roteirizada
                    if ((configuracao.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && configuracao.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoGerarCarga &&
                         carga.ExigeNotaFiscalParaCalcularFrete && cargaPedidosXMLs.Count <= 0 && (carga.TipoOperacao?.GerarControleColeta ?? false))
                    {
                        obterRemetentesEDestinatariosPorNota = false;
                    }

                    Log.TratarErro($"Roteirizacao |{carga.Codigo}| ObterDestinatarios - CargaPedido: '{cargaPedido.Codigo}' Recebedor: '{cargaPedido.Recebedor?.CPF_CNPJ}' UsarOutroEndereco: {cargaPedido.Pedido.UsarOutroEnderecoDestino}, OutroEndereco: {cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo}", "GeracaoControleEntrega");

                    if (!obterRemetentesEDestinatariosPorNota || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                    {
                        Dominio.Entidades.Cliente destinatario = null;

                        if (cargaPedido.Recebedor != null && (cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            destinatario = cargaPedido.Recebedor;
                        else if (cargaPedido.Pedido.Destinatario != null)
                            destinatario = cargaPedido.Pedido.Destinatario;

                        if (destinatario != null)
                        {
                            if (destinatario.CPF_CNPJ_SemFormato == carga.Empresa?.CNPJ_SemFormato && cargaPedido.PedidoPallet)
                                continue;

                            if (cargaPedido.Recebedor != null && configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor)
                                destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, null, cargaPedido.OrdemEntrega));
                            else
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoDestino = cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco;
                                if (cargaPedido.Pedido.UsarOutroEnderecoDestino && clienteOutroEnderecoDestino != null && (cargaPedido.Recebedor == null || cargaPedido.Recebedor.CPF_CNPJ == clienteOutroEnderecoDestino.Cliente.CPF_CNPJ))
                                {
                                    destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, clienteOutroEnderecoDestino, cargaPedido.OrdemEntrega));
                                    listaClienteOutroEndereco.Add(clienteOutroEnderecoDestino);
                                }
                                else
                                    destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, null, cargaPedido.OrdemEntrega));
                            }
                        }

                        if (cargaPedido.Pedido.ClienteDeslocamento != null)
                        {
                            var cliente = ObterClienteTipoPonto(cargaPedido.Pedido.ClienteDeslocamento, TipoPontoPassagem.Coleta, false, null, geraColeta ? cargaPedido.OrdemColeta : 0);
                            remetentes.Add(cliente);
                            dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                        }
                        else if (cargaPedido.Expedidor != null && (cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        {
                            var cliente = ObterClienteTipoPonto(cargaPedido.Expedidor, TipoPontoPassagem.Coleta, false, null, geraColeta ? cargaPedido.OrdemColeta : 0);
                            remetentes.Add(cliente);
                            dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                        }
                        else if (cargaPedido.Pedido.Remetente != null)
                        {
                            if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.ClienteOutroEndereco != null && cargaPedido.Expedidor == null)
                            {
                                var cliente = ObterClienteTipoPonto((cargaPedido.Pedido.Remetente.ClientePai != null ? cargaPedido.Pedido.Remetente.ClientePai : cargaPedido.Pedido.Remetente), TipoPontoPassagem.Coleta, false, cargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco, geraColeta ? cargaPedido.OrdemColeta : 0);
                                remetentes.Add(cliente);
                                dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                                listaClienteOutroEndereco.Add(cargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco);
                            }
                            else
                            {
                                var cliente = ObterClienteTipoPonto((cargaPedido.Pedido.Remetente.ClientePai != null ? cargaPedido.Pedido.Remetente.ClientePai : cargaPedido.Pedido.Remetente), TipoPontoPassagem.Coleta, false, cargaPedido.Pedido.UsarOutroEnderecoOrigem ? (cargaPedido.Pedido.EnderecoOrigem?.ClienteOutroEndereco) : null, geraColeta ? cargaPedido.OrdemColeta : 0);
                                remetentes.Add(cliente);
                                dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                            }

                        }
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = (from obj in cargaPedidosXMLs where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                        destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida select ObterClienteTipoPonto(obj.XMLNotaFiscal.Destinatario, TipoPontoPassagem.Entrega, false, ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoDestino ?? false) && obj.CargaPedido?.Pedido?.EnderecoDestino != null) ? obj.CargaPedido?.Pedido?.EnderecoDestino?.ClienteOutroEndereco : null, 0)).Distinct().ToList());

                        destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada select ObterClienteTipoPonto(obj.XMLNotaFiscal.Emitente, TipoPontoPassagem.Entrega, false, ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoDestino ?? false) && obj.CargaPedido?.Pedido?.EnderecoDestino != null) ? obj.CargaPedido?.Pedido?.EnderecoDestino?.ClienteOutroEndereco : null, 0)).Distinct().ToList());

                        listaClienteOutroEndereco.AddRange((from obj in pedidoXMLs
                                                            where (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ||
                                                            obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada) &&
                                                            obj.CargaPedido.Pedido.UsarOutroEnderecoDestino
                                                            select obj.CargaPedido.Pedido.EnderecoDestino.ClienteOutroEndereco).Distinct().ToList());




                        var listaClientesSaida = (from obj in pedidoXMLs
                                                  where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida
                                                  select new
                                                  {
                                                      Cliente = ObterClienteTipoPonto(
                                                      obj.XMLNotaFiscal.Emitente,
                                                      TipoPontoPassagem.Coleta,
                                                      false,
                                                      ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoOrigem ?? false) && obj.CargaPedido?.Pedido?.EnderecoOrigem != null)
                                                          ? obj.CargaPedido?.Pedido?.EnderecoOrigem?.ClienteOutroEndereco
                                                          : null,
                                                      0),

                                                      DataColeta = obj.CargaPedido?.Pedido?.DataInicialColeta
                                                  }).Distinct().ToList();

                        remetentes.AddRange(listaClientesSaida.Select(x => x.Cliente).Distinct().ToList());

                        foreach (var obj in listaClientesSaida)
                        {
                            if (!dicColetas.ContainsKey(obj.Cliente))
                            {
                                dicColetas[obj.Cliente] = obj.DataColeta;
                            }
                        }

                        var clientesEntradaEColetas = pedidoXMLs
                            .Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada)
                            .Select(obj => new
                            {
                                Cliente = ObterClienteTipoPonto(
                                    obj.XMLNotaFiscal.Destinatario,
                                    TipoPontoPassagem.Coleta,
                                    false,
                                    ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoOrigem ?? false) && obj.CargaPedido?.Pedido?.EnderecoOrigem != null)
                                        ? obj.CargaPedido?.Pedido?.EnderecoOrigem?.ClienteOutroEndereco
                                        : null,
                                    0),
                                DataColeta = obj.CargaPedido?.Pedido?.DataInicialColeta
                            })
                            .Distinct()
                            .ToList();

                        remetentes.AddRange(clientesEntradaEColetas.Select(x => x.Cliente));

                        // Preencher o dicColetas para os clientes de entrada
                        foreach (var item in clientesEntradaEColetas)
                        {
                            if (!dicColetas.ContainsKey(item.Cliente))
                            {
                                dicColetas[item.Cliente] = item.DataColeta;
                            }
                        }

                        listaClienteOutroEndereco.AddRange((from obj in pedidoXMLs
                                                            where (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ||
                                                            obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada) &&
                                                            obj.CargaPedido.Pedido.UsarOutroEnderecoOrigem
                                                            select obj.CargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco).Distinct().ToList());
                    }
                }

                // Adicionar fronteiras
                Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unidadeTrabalho);
                var fronteiras = repCargaFronteira.BuscarPorCarga(carga.Codigo);

                foreach (var fronteira in fronteiras)
                    destinatarios.Add(ObterClienteTipoPonto(fronteira.Fronteira, TipoPontoPassagem.Fronteira, coletaEquipamento: false, clienteOutroEndereco: null, 0));

                pontosColetaEquipamento = pontosColetaEquipamento.Where(obj => obj != null).Distinct().ToList();
                pontosPartida = pontosPartida.Where(obj => obj != null).Distinct().ToList();
                remetentes = remetentes.Where(obj => obj != null).Distinct().ToList();
                destinatarios = destinatarios.Where(obj => obj != null).Distinct().ToList();

                if (configuracaoTipoOperacaoControleEntrega?.OrdenarColetasPorDataCarregamento ?? false)
                {
                    remetentes = remetentes
                        .OrderBy(p => dicColetas.ContainsKey(p) ? dicColetas[p] : DateTime.MaxValue)
                        .ToList();

                }

                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - destinatarios.Cliente: ({(destinatarios.Count > 0 ? string.Join(", ", destinatarios.Select(x => x.Cliente?.Codigo)) : "Sem destinatarios")})", "GeracaoControleEntrega");
                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - destinatarios.ClienteOutroEndereco: ({(destinatarios.Count > 0 ? string.Join(", ", destinatarios.Select(x => x.ClienteOutroEndereco?.Codigo)) : "Sem destinatarios")})", "GeracaoControleEntrega");

                if (destinatarios.Count == 0)
                    problemasRoteirizacao.Append($"Nenhum destino encontrado ao roteirizar a carga {carga.CodigoCargaEmbarcador}");

                AtualizarCoordenadasOutroEndereco(listaClienteOutroEndereco, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                Log.TratarErro($"AtualizarCoordenadasOutroEndereco Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                AtualizarCoordenadas(pontosColetaEquipamento, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(pontosPartida, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(remetentes, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(destinatarios, unidadeTrabalho, configuracaoIntegracao, false, configuracao);

                Log.TratarErro($"AtualizarCoordenadas Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                var origemSemLatitude = (from obj in remetentes where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var destinoSemLatitude = (from obj in destinatarios where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var pontosPartidaSemLatitude = (from obj in pontosPartida where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var pontosColetaEquipamentoSemLatitude = (from obj in pontosColetaEquipamento where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();

                if (origemSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Origem sem coordenadas: {string.Join(", ", (from obj in origemSemLatitude select obj).ToList())} ");

                if (destinoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Destino sem coordenadas: {string.Join(", ", (from obj in destinoSemLatitude select obj).ToList())} ");

                if (pontosPartidaSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", pontosPartidaSemLatitude)} ");

                if (pontosColetaEquipamentoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de coleta de equipamento sem coordenadas: {string.Join(", ", pontosColetaEquipamentoSemLatitude)} ");

                if (pontosPartidaSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", (from obj in pontosPartidaSemLatitude select obj).ToList())} ");

                if (pontosColetaEquipamentoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", (from obj in pontosColetaEquipamentoSemLatitude select obj).ToList())} ");

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();

                if (pontosPartida.Count > 0)
                    pontoPartida = pontosPartida.FirstOrDefault();
                else if (pontosColetaEquipamento.Count > 0)
                    pontoPartida = pontosColetaEquipamento.FirstOrDefault();
                else
                {
                    pontoPartida = remetentes.FirstOrDefault();
                    remetentes.Remove(pontoPartida);
                }

                if (!string.IsNullOrWhiteSpace(problemasRoteirizacao.ToString()))
                    throw new ServicoException(problemasRoteirizacao.ToString());

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta;
                TipoUltimoPontoRoteirizacao ultimoPonto = configuracao?.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = carga.Rota?.TipoUltimoPontoRoteirizacaoPorEstado ?? new Pedido.TipoOperacao(unidadeTrabalho).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);

                if (ultimoPontoPorTipoOperacao.HasValue)
                    ultimoPonto = ultimoPontoPorTipoOperacao.Value;

                //#62188
                if ((carga.Rota?.SituacaoDaRoteirizacao ?? SituacaoRoteirizacao.Erro) == SituacaoRoteirizacao.Concluido && (carga.Rota?.RotaRoteirizadaPorLocal ?? false) && !string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
                {
                    if ((carga.Rota?.PontoPassagemPreDefinido?.Count ?? 0) > 0)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosPreDefinidos = (from obj in carga.Rota.PontoPassagemPreDefinido
                                                                                                                              select new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                                                                                                              {
                                                                                                                                  TipoPontoPassagem = TipoPontoPassagem.Passagem,
                                                                                                                                  Codigo = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                                  Cliente = new Dominio.Entidades.Cliente()
                                                                                                                                  {
                                                                                                                                      CPF_CNPJ = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                                      CodigoIntegracao = obj.Codigo.ToString(),
                                                                                                                                      Latitude = obj.ObterLatitude(),
                                                                                                                                      Longitude = obj.ObterLongitude(),
                                                                                                                                      Nome = obj.ObterDescricao() ?? "Passagem",
                                                                                                                                      Tipo = "Passagem"
                                                                                                                                  }
                                                                                                                              }).ToList();
                        if (destinatarios.Count > 0)
                            destinatarios.InsertRange(0, pontosPreDefinidos);
                        else
                            destinatarios.AddRange(pontosPreDefinidos);
                    }
                }


                if ((carga.Rota?.PostosFiscais?.Count ?? 0) > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> postosFiscais = (from obj in carga.Rota.PostosFiscais
                                                                                                                     select new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                                                                                                     {
                                                                                                                         TipoPontoPassagem = TipoPontoPassagem.PostoFiscal,
                                                                                                                         Codigo = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                         Cliente = new Dominio.Entidades.Cliente()
                                                                                                                         {
                                                                                                                             CPF_CNPJ = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                             CodigoIntegracao = obj.Codigo.ToString(),
                                                                                                                             Latitude = obj.ObterLatitude(),
                                                                                                                             Longitude = obj.ObterLongitude(),
                                                                                                                             Nome = obj.Descricao ?? "Posto Fiscal",
                                                                                                                             Tipo = "Posto Fiscal"
                                                                                                                         }
                                                                                                                     }).ToList();
                    if (destinatarios.Count > 0)
                        destinatarios.InsertRange(0, postosFiscais);
                    else
                        destinatarios.AddRange(postosFiscais);
                }


                Log.TratarErro($"GerarRoteirizacao Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                if (configuracao.RoteirizarPorCidade && (!configuracao.ExigirCargaRoteirizada || !(carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)))
                {
                    AtualizarCoordenadasClientePorCidade(pontoPartida.Cliente, configuracaoIntegracao, unidadeTrabalho, false);
                    AtualizarCoordenadasCidadesDoClientes(remetentes, unidadeTrabalho, configuracaoIntegracao, false);
                    AtualizarCoordenadasCidadesDoClientes(destinatarios, unidadeTrabalho, configuracaoIntegracao, false);

                    resposta = GerarRoteirizacaoPorCidade(pontoPartida, destinatarios, remetentes, ultimoPonto, unidadeTrabalho, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false, !carga.OrdemRoteirizacaoDefinida);
                }
                else
                    resposta = GerarRoteirizacao(pontoPartida, destinatarios, remetentes, ultimoPonto, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false, !carga.OrdemRoteirizacaoDefinida, unidadeTrabalho);

                Log.TratarErro($"RespostaRoteirizacao Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                clientes.Add(pontoPartida);
                clientes.AddRange(remetentes);
                clientes.AddRange(destinatarios);

                string pontoDaRota = resposta.PontoDaRota;
                if (resposta.Status == "OK" && cargaPedidos.Any(o => o.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.FatorPonderacaoDistanciaPeso))
                    pontoDaRota = GerarRoteirizacaoPontosEntrega(pontoPartida, clientes, pontoDaRota, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false);

                if (iniciarTransacao)
                    unidadeTrabalho.Start();

                if (resposta.Status != "OK")
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarControleEntregaSemRota(carga, cargaPedidos, cargaPedidosXMLs, true, configuracao, tipoServicoMultisoftware, unidadeTrabalho);
                    throw new ServicoException(resposta.Status);
                }

                cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
                bool inserir = false;

                if (cargaRotaFrete == null)
                {
                    cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();
                    cargaRotaFrete.Carga = carga;
                    inserir = true;
                }

                cargaRotaFrete.TipoUltimoPontoRoteirizacao = ultimoPonto;
                cargaRotaFrete.PolilinhaRota = resposta.Polilinha;
                cargaRotaFrete.TempoDeViagemEmMinutos = resposta.TempoMinutos;

                Log.TratarErro($"CargaRotaFrete Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                if (inserir)
                    repCargaRotaFrete.Inserir(cargaRotaFrete);
                else
                    repCargaRotaFrete.Atualizar(cargaRotaFrete);

                Log.TratarErro($"Pontos da rota da carga - {carga.CodigoCargaEmbarcador} - {pontoDaRota}", "PontosPassagem");

                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = RotaFrete.SetarPontosPassagemCarga(carga, cargaPedidos, cargaRotaFrete, pontoDaRota, clientes, listaClienteOutroEndereco, unidadeTrabalho);

                if (refazerControleEntrega)
                {
                    Log.TratarErro($"GerarCargaEntrega Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagem, true, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                }

                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagio(carga, unidadeTrabalho);

                if (carga.DadosSumarizados != null)
                {
                    Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
                    Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unidadeTrabalho);

                    if (carga.Distancia == 0)
                    {
                        decimal distanciaPedidos = cargaPedidos.Sum(o => o.Pedido.Distancia);

                        if (distanciaPedidos > 0m)
                            carga.DadosSumarizados.Distancia = distanciaPedidos;
                        else if (carga.Rota != null && carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido && (configuracao.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && (configuracao.RoteirizarPorCidade || (carga.TipoOperacao?.RoteirizarPorLocalidade ?? false)))
                            carga.DadosSumarizados.Distancia = Decimal.ToInt32(carga.Rota.Quilometros);
                        else
                            carga.DadosSumarizados.Distancia = Decimal.ToInt32(resposta.Distancia);
                    }
                    carga.DadosSumarizados.RoteirizarNaEtapaDeDadosTransporte = false;

                    servicoCargaDadosSumarizados.AtualizarOrigensEDestinos(carga.DadosSumarizados, carga, cargaPedidos, unidadeTrabalho, tipoServicoMultisoftware);
                    repositorioCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                }

                carga.NumeroTentativasRoteirizacaoCarga = 0;
                carga.DataUltimaTentativaRoteirizacaoCarga = null;
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                Log.TratarErro($"Atualizar Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                repCarga.Atualizar(carga);



                Log.TratarErro($"SetarPrevisaoEntrega Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                SetarPrevisaoEntrega(carga.Codigo, unidadeTrabalho);

                IntegracaoTrizy integracaoTrizy = new IntegracaoTrizy(unidadeTrabalho);
                integracaoTrizy.ReenviarIntegracaoTrizyDadosTransporte(carga.Codigo);

                if (iniciarTransacao)
                    unidadeTrabalho.CommitChanges();

                Log.TratarErro($"Finalizou Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
            }
            catch (ServicoException excecao)
            {
                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.Rollback();

                Log.TratarErro(excecao.Message, "CargaRoteirizacao");

                repCarga.AtualizarTentativaRoteirizacao(carga.Codigo, SituacaoRoteirizacao.Erro);

                servicoMensagemAlerta.AdicionarNaoDuplicado(carga, TipoMensagemAlerta.ProblemaRoterizacao, excecao.Message, true);

                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.CommitChanges();
            }
            catch (Exception excecao)
            {
                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.Rollback();

                Log.TratarErro(excecao, "CargaRoteirizacao");

                repCarga.AtualizarTentativaRoteirizacao(carga.Codigo, SituacaoRoteirizacao.Erro);

                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.CommitChanges();
            }
        }

        public async static Task GerarIntegracoesRoteirizacaoCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool refazerControleEntrega = true)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repositorioConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unidadeTrabalho);
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unidadeTrabalho);
            Servicos.Embarcador.Carga.CTe svcCargaCTe = new Servicos.Embarcador.Carga.CTe(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = await repConfiguracaoRoteirizacao.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repositorioConfiguracaoTipoOperacaoControleEntrega.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

            if (carga.CargaRotaFreteInformadaViaIntegracao)
            {
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                repCarga.Atualizar(carga);

                return;
            }

            bool iniciarTransacao = !unidadeTrabalho.IsActiveTransaction();

            try
            {
                Log.TratarErro($"Iniciou Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosColetaEquipamento = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosPartida = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> remetentes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();

                cargaPedidos = cargaPedidos.OrderBy(obj => obj.OrdemColeta).ThenBy(obj => obj.OrdemEntrega).ToList();
                bool geraColeta = carga.TipoOperacao?.GerarControleColeta ?? false;

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();

                if ((retiradaContainer != null) && !carga.LiberadaSemRetiradaContainer)
                    pontosColetaEquipamento.Add(ObterClienteTipoPonto(retiradaContainer.Local, TipoPontoPassagem.Coleta, coletaEquipamento: true, clienteOutroEndereco: null, 0));

                Log.TratarErro($"__________| Roteirizacao - Iniciou ObterDestinatarios - {carga.Codigo} |__________", "GeracaoControleEntrega");

                StringBuilder problemasRoteirizacao = new StringBuilder();

                servicoMensagemAlerta.Remover(carga, TipoMensagemAlerta.ProblemaRoterizacao);

                Dictionary<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto, DateTime?> dicColetas = new Dictionary<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto, DateTime?>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if ((cargaPedido.PontoPartida != null) && cargaPedido.PossuiColetaEquipamentoPontoPartida)
                        pontosColetaEquipamento.Add(ObterClienteTipoPonto(cargaPedido.PontoPartida, TipoPontoPassagem.Coleta, coletaEquipamento: true, clienteOutroEndereco: null, geraColeta ? cargaPedido.OrdemColeta : 0));

                    if ((cargaPedido.PontoPartida != null) && !cargaPedido.PossuiColetaEquipamentoPontoPartida)
                        pontosPartida.Add(ObterClienteTipoPonto(cargaPedido.PontoPartida, TipoPontoPassagem.Passagem, coletaEquipamento: false, clienteOutroEndereco: null, geraColeta ? cargaPedido.OrdemColeta : 0));

                    bool obterRemetentesEDestinatariosPorNota = (
                        cargaPedido.TipoRateio != TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado &&
                        carga.ExigeNotaFiscalParaCalcularFrete &&
                        svcCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes, tipoServicoMultisoftware)
                    );

                    //Caso necessita da roteirizacao antes da nota, apenas com os pedidos e apos confirmacao dos documentos a carga é novamente roteirizada
                    if ((configuracao.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && configuracao.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoGerarCarga &&
                         carga.ExigeNotaFiscalParaCalcularFrete && cargaPedidosXMLs.Count <= 0 && (carga.TipoOperacao?.GerarControleColeta ?? false))
                    {
                        obterRemetentesEDestinatariosPorNota = false;
                    }

                    Log.TratarErro($"Roteirizacao |{carga.Codigo}| ObterDestinatarios - CargaPedido: '{cargaPedido.Codigo}' Recebedor: '{cargaPedido.Recebedor?.CPF_CNPJ}' UsarOutroEndereco: {cargaPedido.Pedido.UsarOutroEnderecoDestino}, OutroEndereco: {cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo}", "GeracaoControleEntrega");

                    if (!obterRemetentesEDestinatariosPorNota || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                    {
                        Dominio.Entidades.Cliente destinatario = null;

                        if (cargaPedido.Recebedor != null && (cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            destinatario = cargaPedido.Recebedor;
                        else if (cargaPedido.Pedido.Destinatario != null)
                            destinatario = cargaPedido.Pedido.Destinatario;

                        if (destinatario != null)
                        {
                            if (destinatario.CPF_CNPJ_SemFormato == carga.Empresa?.CNPJ_SemFormato && cargaPedido.PedidoPallet)
                                continue;

                            if (cargaPedido.Recebedor != null && configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor)
                                destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, null, cargaPedido.OrdemEntrega));
                            else
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoDestino = cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco;
                                if (cargaPedido.Pedido.UsarOutroEnderecoDestino && clienteOutroEnderecoDestino != null && (cargaPedido.Recebedor == null || cargaPedido.Recebedor.CPF_CNPJ == clienteOutroEnderecoDestino.Cliente.CPF_CNPJ))
                                {
                                    destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, clienteOutroEnderecoDestino, cargaPedido.OrdemEntrega));
                                    listaClienteOutroEndereco.Add(clienteOutroEnderecoDestino);
                                }
                                else
                                    destinatarios.Add(ObterClienteTipoPonto(destinatario, TipoPontoPassagem.Entrega, false, null, cargaPedido.OrdemEntrega));
                            }
                        }

                        if (cargaPedido.Pedido.ClienteDeslocamento != null)
                        {
                            var cliente = ObterClienteTipoPonto(cargaPedido.Pedido.ClienteDeslocamento, TipoPontoPassagem.Coleta, false, null, geraColeta ? cargaPedido.OrdemColeta : 0);
                            remetentes.Add(cliente);
                            dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                        }
                        else if (cargaPedido.Expedidor != null && (cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        {
                            var cliente = ObterClienteTipoPonto(cargaPedido.Expedidor, TipoPontoPassagem.Coleta, false, null, geraColeta ? cargaPedido.OrdemColeta : 0);
                            remetentes.Add(cliente);
                            dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                        }
                        else if (cargaPedido.Pedido.Remetente != null)
                        {
                            if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.ClienteOutroEndereco != null && cargaPedido.Expedidor == null)
                            {
                                var cliente = ObterClienteTipoPonto((cargaPedido.Pedido.Remetente.ClientePai != null ? cargaPedido.Pedido.Remetente.ClientePai : cargaPedido.Pedido.Remetente), TipoPontoPassagem.Coleta, false, cargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco, geraColeta ? cargaPedido.OrdemColeta : 0);
                                remetentes.Add(cliente);
                                dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                                listaClienteOutroEndereco.Add(cargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco);
                            }
                            else
                            {
                                var cliente = ObterClienteTipoPonto((cargaPedido.Pedido.Remetente.ClientePai != null ? cargaPedido.Pedido.Remetente.ClientePai : cargaPedido.Pedido.Remetente), TipoPontoPassagem.Coleta, false, cargaPedido.Pedido.UsarOutroEnderecoOrigem ? (cargaPedido.Pedido.EnderecoOrigem?.ClienteOutroEndereco) : null, geraColeta ? cargaPedido.OrdemColeta : 0);
                                remetentes.Add(cliente);
                                dicColetas[cliente] = cargaPedido.Pedido?.DataInicialColeta;
                            }

                        }
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = (from obj in cargaPedidosXMLs where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                        destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida select ObterClienteTipoPonto(obj.XMLNotaFiscal.Destinatario, TipoPontoPassagem.Entrega, false, ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoDestino ?? false) && obj.CargaPedido?.Pedido?.EnderecoDestino != null) ? obj.CargaPedido?.Pedido?.EnderecoDestino?.ClienteOutroEndereco : null, 0)).Distinct().ToList());

                        destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada select ObterClienteTipoPonto(obj.XMLNotaFiscal.Emitente, TipoPontoPassagem.Entrega, false, ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoDestino ?? false) && obj.CargaPedido?.Pedido?.EnderecoDestino != null) ? obj.CargaPedido?.Pedido?.EnderecoDestino?.ClienteOutroEndereco : null, 0)).Distinct().ToList());

                        listaClienteOutroEndereco.AddRange((from obj in pedidoXMLs
                                                            where (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ||
                                                            obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada) &&
                                                            obj.CargaPedido.Pedido.UsarOutroEnderecoDestino
                                                            select obj.CargaPedido.Pedido.EnderecoDestino.ClienteOutroEndereco).Distinct().ToList());




                        var listaClientesSaida = (from obj in pedidoXMLs
                                                  where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida
                                                  select new
                                                  {
                                                      Cliente = ObterClienteTipoPonto(
                                                      obj.XMLNotaFiscal.Emitente,
                                                      TipoPontoPassagem.Coleta,
                                                      false,
                                                      ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoOrigem ?? false) && obj.CargaPedido?.Pedido?.EnderecoOrigem != null)
                                                          ? obj.CargaPedido?.Pedido?.EnderecoOrigem?.ClienteOutroEndereco
                                                          : null,
                                                      0),

                                                      DataColeta = obj.CargaPedido?.Pedido?.DataInicialColeta
                                                  }).Distinct().ToList();

                        remetentes.AddRange(listaClientesSaida.Select(x => x.Cliente).Distinct().ToList());

                        foreach (var obj in listaClientesSaida)
                        {
                            if (!dicColetas.ContainsKey(obj.Cliente))
                            {
                                dicColetas[obj.Cliente] = obj.DataColeta;
                            }
                        }

                        var clientesEntradaEColetas = pedidoXMLs
                            .Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada)
                            .Select(obj => new
                            {
                                Cliente = ObterClienteTipoPonto(
                                    obj.XMLNotaFiscal.Destinatario,
                                    TipoPontoPassagem.Coleta,
                                    false,
                                    ((obj.CargaPedido?.Pedido?.UsarOutroEnderecoOrigem ?? false) && obj.CargaPedido?.Pedido?.EnderecoOrigem != null)
                                        ? obj.CargaPedido?.Pedido?.EnderecoOrigem?.ClienteOutroEndereco
                                        : null,
                                    0),
                                DataColeta = obj.CargaPedido?.Pedido?.DataInicialColeta
                            })
                            .Distinct()
                            .ToList();

                        remetentes.AddRange(clientesEntradaEColetas.Select(x => x.Cliente));

                        // Preencher o dicColetas para os clientes de entrada
                        foreach (var item in clientesEntradaEColetas)
                        {
                            if (!dicColetas.ContainsKey(item.Cliente))
                            {
                                dicColetas[item.Cliente] = item.DataColeta;
                            }
                        }

                        listaClienteOutroEndereco.AddRange((from obj in pedidoXMLs
                                                            where (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ||
                                                            obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada) &&
                                                            obj.CargaPedido.Pedido.UsarOutroEnderecoOrigem
                                                            select obj.CargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco).Distinct().ToList());
                    }
                }

                // Adicionar fronteiras
                Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unidadeTrabalho);
                var fronteiras = repCargaFronteira.BuscarPorCarga(carga.Codigo);

                foreach (var fronteira in fronteiras)
                    destinatarios.Add(ObterClienteTipoPonto(fronteira.Fronteira, TipoPontoPassagem.Fronteira, coletaEquipamento: false, clienteOutroEndereco: null, 0));

                pontosColetaEquipamento = pontosColetaEquipamento.Where(obj => obj != null).Distinct().ToList();
                pontosPartida = pontosPartida.Where(obj => obj != null).Distinct().ToList();
                remetentes = remetentes.Where(obj => obj != null).Distinct().ToList();
                destinatarios = destinatarios.Where(obj => obj != null).Distinct().ToList();

                if (configuracaoTipoOperacaoControleEntrega?.OrdenarColetasPorDataCarregamento ?? false)
                {
                    remetentes = remetentes
                        .OrderBy(p => dicColetas.ContainsKey(p) ? dicColetas[p] : DateTime.MaxValue)
                        .ToList();

                }

                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - destinatarios.Cliente: ({(destinatarios.Count > 0 ? string.Join(", ", destinatarios.Select(x => x.Cliente?.Codigo)) : "Sem destinatarios")})", "GeracaoControleEntrega");
                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - destinatarios.ClienteOutroEndereco: ({(destinatarios.Count > 0 ? string.Join(", ", destinatarios.Select(x => x.ClienteOutroEndereco?.Codigo)) : "Sem destinatarios")})", "GeracaoControleEntrega");

                if (destinatarios.Count == 0)
                    problemasRoteirizacao.Append($"Nenhum destino encontrado ao roteirizar a carga {carga.CodigoCargaEmbarcador}");

                AtualizarCoordenadasOutroEndereco(listaClienteOutroEndereco, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                Log.TratarErro($"AtualizarCoordenadasOutroEndereco Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                AtualizarCoordenadas(pontosColetaEquipamento, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(pontosPartida, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(remetentes, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
                AtualizarCoordenadas(destinatarios, unidadeTrabalho, configuracaoIntegracao, false, configuracao);

                Log.TratarErro($"AtualizarCoordenadas Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                var origemSemLatitude = (from obj in remetentes where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var destinoSemLatitude = (from obj in destinatarios where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var pontosPartidaSemLatitude = (from obj in pontosPartida where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();
                var pontosColetaEquipamentoSemLatitude = (from obj in pontosColetaEquipamento where obj != null && (!obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.Cliente?.Latitude) || string.IsNullOrEmpty(obj.Cliente?.Longitude)) || (obj.UsarOutroEndereco && (string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(obj.ClienteOutroEndereco?.Longitude)))) select (obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente).Descricao).ToList();

                if (origemSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Origem sem coordenadas: {string.Join(", ", (from obj in origemSemLatitude select obj).ToList())} ");

                if (destinoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Destino sem coordenadas: {string.Join(", ", (from obj in destinoSemLatitude select obj).ToList())} ");

                if (pontosPartidaSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", pontosPartidaSemLatitude)} ");

                if (pontosColetaEquipamentoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de coleta de equipamento sem coordenadas: {string.Join(", ", pontosColetaEquipamentoSemLatitude)} ");

                if (pontosPartidaSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", (from obj in pontosPartidaSemLatitude select obj).ToList())} ");

                if (pontosColetaEquipamentoSemLatitude?.Count > 0)
                    problemasRoteirizacao.Append($" Pontos de partida sem coordenadas: {string.Join(", ", (from obj in pontosColetaEquipamentoSemLatitude select obj).ToList())} ");

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();

                if (pontosPartida.Count > 0)
                    pontoPartida = pontosPartida.FirstOrDefault();
                else if (pontosColetaEquipamento.Count > 0)
                    pontoPartida = pontosColetaEquipamento.FirstOrDefault();
                else
                {
                    pontoPartida = remetentes.FirstOrDefault();
                    remetentes.Remove(pontoPartida);
                }

                if (!string.IsNullOrWhiteSpace(problemasRoteirizacao.ToString()))
                    throw new ServicoException(problemasRoteirizacao.ToString());

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta;
                TipoUltimoPontoRoteirizacao ultimoPonto = configuracao?.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = carga.Rota?.TipoUltimoPontoRoteirizacaoPorEstado ?? new Pedido.TipoOperacao(unidadeTrabalho).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);

                if (ultimoPontoPorTipoOperacao.HasValue)
                    ultimoPonto = ultimoPontoPorTipoOperacao.Value;

                //#62188
                if ((carga.Rota?.SituacaoDaRoteirizacao ?? SituacaoRoteirizacao.Erro) == SituacaoRoteirizacao.Concluido && (carga.Rota?.RotaRoteirizadaPorLocal ?? false) && !string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
                {
                    if ((carga.Rota?.PontoPassagemPreDefinido?.Count ?? 0) > 0)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pontosPreDefinidos = (from obj in carga.Rota.PontoPassagemPreDefinido
                                                                                                                              select new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                                                                                                              {
                                                                                                                                  TipoPontoPassagem = TipoPontoPassagem.Passagem,
                                                                                                                                  Codigo = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                                  Cliente = new Dominio.Entidades.Cliente()
                                                                                                                                  {
                                                                                                                                      CPF_CNPJ = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                                      CodigoIntegracao = obj.Codigo.ToString(),
                                                                                                                                      Latitude = obj.ObterLatitude(),
                                                                                                                                      Longitude = obj.ObterLongitude(),
                                                                                                                                      Nome = obj.ObterDescricao() ?? "Passagem",
                                                                                                                                      Tipo = "Passagem"
                                                                                                                                  }
                                                                                                                              }).ToList();
                        if (destinatarios.Count > 0)
                            destinatarios.InsertRange(0, pontosPreDefinidos);
                        else
                            destinatarios.AddRange(pontosPreDefinidos);
                    }
                }


                if ((carga.Rota?.PostosFiscais?.Count ?? 0) > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> postosFiscais = (from obj in carga.Rota.PostosFiscais
                                                                                                                     select new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto()
                                                                                                                     {
                                                                                                                         TipoPontoPassagem = TipoPontoPassagem.PostoFiscal,
                                                                                                                         Codigo = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                         Cliente = new Dominio.Entidades.Cliente()
                                                                                                                         {
                                                                                                                             CPF_CNPJ = obj.Cliente?.CPF_CNPJ ?? obj.Codigo,
                                                                                                                             CodigoIntegracao = obj.Codigo.ToString(),
                                                                                                                             Latitude = obj.ObterLatitude(),
                                                                                                                             Longitude = obj.ObterLongitude(),
                                                                                                                             Nome = obj.Descricao ?? "Posto Fiscal",
                                                                                                                             Tipo = "Posto Fiscal"
                                                                                                                         }
                                                                                                                     }).ToList();
                    if (destinatarios.Count > 0)
                        destinatarios.InsertRange(0, postosFiscais);
                    else
                        destinatarios.AddRange(postosFiscais);
                }


                Log.TratarErro($"GerarRoteirizacao Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                if (configuracao.RoteirizarPorCidade && (!configuracao.ExigirCargaRoteirizada || !(carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)))
                {
                    AtualizarCoordenadasClientePorCidade(pontoPartida.Cliente, configuracaoIntegracao, unidadeTrabalho, false);
                    AtualizarCoordenadasCidadesDoClientes(remetentes, unidadeTrabalho, configuracaoIntegracao, false);
                    AtualizarCoordenadasCidadesDoClientes(destinatarios, unidadeTrabalho, configuracaoIntegracao, false);

                    resposta = GerarRoteirizacaoPorCidade(pontoPartida, destinatarios, remetentes, ultimoPonto, unidadeTrabalho, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false, !carga.OrdemRoteirizacaoDefinida);
                }
                else
                    resposta = GerarRoteirizacao(pontoPartida, destinatarios, remetentes, ultimoPonto, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false, !carga.OrdemRoteirizacaoDefinida, unidadeTrabalho);

                Log.TratarErro($"RespostaRoteirizacao Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
                clientes.Add(pontoPartida);
                clientes.AddRange(remetentes);
                clientes.AddRange(destinatarios);

                string pontoDaRota = resposta.PontoDaRota;
                if (resposta.Status == "OK" && cargaPedidos.Any(o => o.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.FatorPonderacaoDistanciaPeso))
                    pontoDaRota = GerarRoteirizacaoPontosEntrega(pontoPartida, clientes, pontoDaRota, configuracaoIntegracao?.ServidorRouteOSM ?? "", "", false, false);

                if (iniciarTransacao)
                    unidadeTrabalho.Start();

                if (resposta.Status != "OK")
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarControleEntregaSemRota(carga, cargaPedidos, cargaPedidosXMLs, true, configuracao, tipoServicoMultisoftware, unidadeTrabalho);
                    throw new ServicoException(resposta.Status);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
                bool inserir = false;

                if (cargaRotaFrete == null)
                {
                    cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();
                    cargaRotaFrete.Carga = carga;
                    inserir = true;
                }

                cargaRotaFrete.TipoUltimoPontoRoteirizacao = ultimoPonto;
                cargaRotaFrete.PolilinhaRota = resposta.Polilinha;
                cargaRotaFrete.TempoDeViagemEmMinutos = resposta.TempoMinutos;

                Log.TratarErro($"CargaRotaFrete Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");

                if (inserir)
                    repCargaRotaFrete.Inserir(cargaRotaFrete);
                else
                    repCargaRotaFrete.Atualizar(cargaRotaFrete);

                Log.TratarErro($"Pontos da rota da carga - {carga.CodigoCargaEmbarcador} - {pontoDaRota}", "PontosPassagem");

                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = RotaFrete.SetarPontosPassagemCarga(carga, cargaPedidos, cargaRotaFrete, pontoDaRota, clientes, listaClienteOutroEndereco, unidadeTrabalho);

                if (refazerControleEntrega)
                {
                    Log.TratarErro($"GerarCargaEntrega Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagem, true, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                }

                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagio(carga, unidadeTrabalho);

                if (carga.DadosSumarizados != null)
                {
                    Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
                    Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unidadeTrabalho);

                    if (carga.Distancia == 0)
                    {
                        decimal distanciaPedidos = cargaPedidos.Sum(o => o.Pedido.Distancia);

                        if (distanciaPedidos > 0m)
                            carga.DadosSumarizados.Distancia = distanciaPedidos;
                        else if (carga.Rota != null && carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido && (configuracao.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && (configuracao.RoteirizarPorCidade || (carga.TipoOperacao?.RoteirizarPorLocalidade ?? false)))
                            carga.DadosSumarizados.Distancia = Decimal.ToInt32(carga.Rota.Quilometros);
                        else
                            carga.DadosSumarizados.Distancia = Decimal.ToInt32(resposta.Distancia);
                    }
                    carga.DadosSumarizados.RoteirizarNaEtapaDeDadosTransporte = false;

                    servicoCargaDadosSumarizados.AtualizarOrigensEDestinos(carga.DadosSumarizados, carga, cargaPedidos, unidadeTrabalho, tipoServicoMultisoftware);
                    repositorioCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                }

                carga.NumeroTentativasRoteirizacaoCarga = 0;
                carga.DataUltimaTentativaRoteirizacaoCarga = null;
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;

                Log.TratarErro($"Atualizar Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                repCarga.Atualizar(carga);

                Log.TratarErro($"SetarPrevisaoEntrega Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
                SetarPrevisaoEntrega(carga.Codigo, unidadeTrabalho);

                IntegracaoTrizy integracaoTrizy = new IntegracaoTrizy(unidadeTrabalho);
                integracaoTrizy.ReenviarIntegracaoTrizyDadosTransporte(carga.Codigo);

                if (iniciarTransacao)
                    unidadeTrabalho.CommitChanges();

                Log.TratarErro($"Finalizou Roteirizacao carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CargaRoteirizacao");
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao.Message, "CargaRoteirizacao");

                carga.NumeroTentativasRoteirizacaoCarga++;
                carga.DataUltimaTentativaRoteirizacaoCarga = DateTime.Now;
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Erro;

                servicoMensagemAlerta.AdicionarNaoDuplicado(carga, TipoMensagemAlerta.ProblemaRoterizacao, excecao.Message, true);

                repCarga.Atualizar(carga);

                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.CommitChanges();
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "CargaRoteirizacao");

                carga.NumeroTentativasRoteirizacaoCarga++;
                carga.DataUltimaTentativaRoteirizacaoCarga = DateTime.Now;
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Erro;

                repCarga.Atualizar(carga);

                if (iniciarTransacao && unidadeTrabalho.IsActiveTransaction())
                    unidadeTrabalho.CommitChanges();
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao GerarRoteirizacao(Dominio.Entidades.Cliente remetente, ICollection<Dominio.Entidades.Localidade> destinos, ICollection<Dominio.Entidades.Cliente> coletas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUlitmoPonto, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota, bool ordenar, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                if ((remetente == null) || (destinos == null) || (destinos.Count == 0))
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);

                double lat;
                double lng;

                var infoRota = remetente.CodigoIntegracao != null ? remetente.CodigoIntegracao.ToString() : remetente.CPF_CNPJ > 0 ? remetente.CPF_CNPJ.ToString() : "";

                lat = double.Parse(remetente.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                lng = double.Parse(remetente.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = remetente.Nome, Pedagio = false, Informacao = infoRota, Codigo = remetente.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta });

                foreach (var destinatario in destinos)
                {
                    if (destinatario == null || !destinatario.Latitude.HasValue || !destinatario.Longitude.HasValue)
                        continue;

                    infoRota = destinatario.Descricao;

                    lat = (double)destinatario.Latitude.Value;
                    lng = (double)destinatario.Longitude.Value;

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Descricao = destinatario.Descricao,
                        Pedagio = false,
                        Informacao = infoRota,
                        Codigo = destinatario.Codigo,
                        TipoPonto = (destinatario.Codigo < 0 ? TipoPontoPassagem.Passagem : TipoPontoPassagem.Entrega),
                        UtilizaLocalidade = true
                    });
                }

                if (coletas != null)
                {
                    foreach (var coleta in coletas)
                    {
                        infoRota = (coleta.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(coleta.CodigoIntegracao)) ? coleta.CodigoIntegracao.ToString() : coleta.CPF_CNPJ > 0 ? coleta.CPF_CNPJ.ToString() : "";

                        lat = double.Parse(coleta.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        lng = double.Parse(coleta.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                        rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                        {
                            Lat = lat,
                            Lng = lng,
                            Descricao = coleta.Nome,
                            Pedagio = false,
                            Informacao = infoRota,
                            Codigo = coleta.Codigo,
                            CodigoCliente = coleta.Codigo,
                            TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
                        });
                    }
                }

                bool AteOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;

                if (AteOrigem)
                {
                    lat = double.Parse(remetente.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    lng = double.Parse(remetente.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Descricao = remetente.Nome,
                        Pedagio = false,
                        Informacao = infoRota,
                        Codigo = remetente.Codigo,
                        CodigoCliente = remetente.Codigo,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno
                    });
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unitOfWork);

                if (roteirizarGoogle)
                    respostaRoteirizacao = rota.RoteirizarGoogle(ordenar, AteOrigem, key);
                else
                {
                    bool ateOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = ordenar, PontosNaRota = pontoNaRota };
                    respostaRoteirizacao = rota.Roteirizar(opcoes);
                }

                respostaRoteirizacao = AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaRoteirizacao, trechoBalsa, wayPointDestinosRemovidos, wayPointRetorno, servidor, unitOfWork);

                return AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, key, roteirizarGoogle, unitOfWork);

            }
            catch (Exception e)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = e.Message };
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao GerarRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> coletas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUlitmoPonto, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota, bool ordenar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

                if ((pontoPartida == null) || (pontoPartida.Cliente == null) || (destinatarios == null) || (destinatarios.Count == 0))
                {
                    Servicos.Log.TratarErro("ponto de partida ou destinatarios não disponíveis.");
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };
                }


                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);

                double lat;
                double lng;

                var infoRota = pontoPartida.Cliente.CodigoIntegracao != null ? pontoPartida.Cliente.CodigoIntegracao.ToString() : pontoPartida.Cliente.CPF_CNPJ > 0 ? pontoPartida.Cliente.CPF_CNPJ.ToString() : "";

                lat = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Latitude : pontoPartida.Cliente.Latitude);
                lng = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Longitude : pontoPartida.Cliente.Longitude);

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Lat = lat,
                    Lng = lng,
                    Codigo = pontoPartida.Codigo,
                    CodigoCliente = pontoPartida.Cliente.Codigo,
                    Descricao = pontoPartida.Cliente.Nome,
                    Pedagio = false,
                    UsarOutroEndereco = pontoPartida.UsarOutroEndereco,
                    Informacao = infoRota,
                    SequenciaPredefinida = pontoPartida.SequenciaPreDefinida,
                    TipoPonto = pontoPartida.TipoPontoPassagem,
                    ColetaEquipamento = pontoPartida.ColetaEquipamento,
                    Fronteira = pontoPartida.TipoPontoPassagem == TipoPontoPassagem.Fronteira
                });

                if (coletas != null)
                {
                    foreach (var coleta in coletas)
                    {
                        infoRota = (coleta.Cliente.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(coleta.Cliente.CodigoIntegracao)) ? coleta.Cliente.CodigoIntegracao.ToString() : coleta.Cliente.CPF_CNPJ > 0 ? coleta.Cliente.CPF_CNPJ.ToString() : "";

                        PointF geolocalizacao = LocalEntregaCliente(coleta.Cliente);

                        lat = ParseDouble(coleta.UsarOutroEndereco ? coleta.ClienteOutroEndereco.Latitude : geolocalizacao.Y.ToString());
                        lng = ParseDouble(coleta.UsarOutroEndereco ? coleta.ClienteOutroEndereco.Longitude : geolocalizacao.X.ToString());

                        rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                        {
                            Lat = lat,
                            Lng = lng,
                            Codigo = coleta.Codigo,
                            CodigoCliente = coleta.Cliente.Codigo,
                            Descricao = coleta.Cliente.Nome,
                            Pedagio = false,
                            UsarOutroEndereco = coleta.UsarOutroEndereco,
                            SequenciaPredefinida = coleta.SequenciaPreDefinida,
                            Informacao = infoRota,
                            TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
                        });
                    }
                }

                //Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto primeiraEntrega = destinatarios.FirstOrDefault(x => x.PrimeiraEntrega);
                //if (primeiraEntrega != null)
                //{
                //    destinatarios.Remove(primeiraEntrega);
                //    destinatarios.Insert(0, primeiraEntrega);
                //}

                foreach (var destinatario in destinatarios)
                {
                    if (destinatario == null)
                        continue;

                    infoRota = (destinatario.Cliente.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(destinatario.Cliente.CodigoIntegracao)) ? destinatario.Cliente.CodigoIntegracao.ToString() : destinatario.Cliente.CPF_CNPJ > 0 ? destinatario.Cliente.CPF_CNPJ.ToString() : "";

                    PointF geolocalizacao = LocalEntregaCliente(destinatario.Cliente);

                    lat = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Latitude : geolocalizacao.Y.ToString());
                    lng = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Longitude : geolocalizacao.X.ToString());

                    TipoPontoPassagem tipoPonto;

                    if (destinatario.Cliente.Tipo == "PontoApoio")
                        tipoPonto = TipoPontoPassagem.Apoio;
                    else if (destinatario.Cliente.Tipo == "Passagem")
                        tipoPonto = TipoPontoPassagem.Passagem;
                    else if (destinatario.TipoPontoPassagem == TipoPontoPassagem.Coleta)
                        tipoPonto = TipoPontoPassagem.Coleta;
                    else if (destinatario.TipoPontoPassagem == TipoPontoPassagem.Fronteira)
                        tipoPonto = TipoPontoPassagem.Fronteira;
                    else if (destinatario.TipoPontoPassagem == TipoPontoPassagem.PostoFiscal)
                        tipoPonto = TipoPontoPassagem.PostoFiscal;
                    else
                        tipoPonto = TipoPontoPassagem.Entrega;

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Codigo = destinatario.Codigo,
                        CodigoCliente = destinatario.Cliente.Codigo,
                        Descricao = destinatario.Cliente.Nome,
                        Pedagio = false,
                        SequenciaPredefinida = destinatario.SequenciaPreDefinida,
                        Informacao = infoRota,
                        UsarOutroEndereco = destinatario.UsarOutroEndereco,
                        Fronteira = destinatario.TipoPontoPassagem == TipoPontoPassagem.Fronteira,
                        TipoPonto = tipoPonto,
                        PrimeiraEntrega = destinatario.PrimeiraEntrega,
                        CodigoOutroEndereco = (!destinatario.UsarOutroEndereco ? 0 : destinatario.ClienteOutroEndereco?.Codigo ?? 0)
                    });
                }

                bool ateOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;

                if (ateOrigem)
                {

                    lat = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Latitude : pontoPartida.Cliente.Latitude);
                    lng = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Longitude : pontoPartida.Cliente.Longitude);

                    int sequencia = rota.GetUltimaOrdemPredefinicao();
                    if (sequencia > 0)
                        sequencia++;

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Codigo = pontoPartida.Codigo,
                        CodigoCliente = pontoPartida.Cliente.Codigo,
                        Descricao = pontoPartida.Cliente.Nome,
                        Pedagio = false,
                        Informacao = infoRota,
                        SequenciaPredefinida = sequencia,
                        UsarOutroEndereco = pontoPartida.UsarOutroEndereco,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno, //pontoPartida.TipoPontoPassagem,
                        ColetaEquipamento = pontoPartida.ColetaEquipamento
                    });
                }

                if (!ordenar)
                    rota.OrdenarPredefinicao(configuracaoRoteirizacao.ColetasSempreInicioRotaOrdenadaCliente);

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unitOfWork);

                if (roteirizarGoogle)
                    respostaRoteirizacao = rota.RoteirizarGoogle(ordenar, ateOrigem, key);
                else
                {
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                    {
                        AteOrigem = ateOrigem,
                        Ordenar = ordenar,
                        PontosNaRota = pontoNaRota
                    };
                    respostaRoteirizacao = rota.Roteirizar(opcoes);
                }

                respostaRoteirizacao = AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaRoteirizacao, trechoBalsa, wayPointDestinosRemovidos, wayPointRetorno, servidor, unitOfWork);

                return AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, key, roteirizarGoogle, unitOfWork);

            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = e.Message };
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao GerarRoteirizacaoOrigem(Dominio.Entidades.Localidade origem, ICollection<Dominio.Entidades.Localidade> destinos, ICollection<Dominio.Entidades.Cliente> coletas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUlitmoPonto, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota, bool ordenar, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                if ((origem == null) || (destinos == null) || (destinos.Count == 0))
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);

                double lat;
                double lng;

                var infoRota = origem.Descricao;

                if (!origem.Latitude.HasValue || !origem.Longitude.HasValue)
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                lat = (double)origem.Latitude.Value;
                lng = (double)origem.Longitude.Value;

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = origem.Descricao, Pedagio = false, Informacao = infoRota, Codigo = origem.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta, UtilizaLocalidade = true });


                foreach (var destinatario in destinos)
                {
                    if (destinatario == null || !destinatario.Latitude.HasValue || !destinatario.Longitude.HasValue)
                        continue;

                    infoRota = destinatario.Descricao;

                    lat = (double)destinatario.Latitude.Value;
                    lng = (double)destinatario.Longitude.Value;

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Descricao = destinatario.Descricao,
                        Pedagio = false,
                        Informacao = infoRota,
                        Codigo = destinatario.Codigo,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega,
                        UtilizaLocalidade = true
                    });
                }

                if (coletas != null)
                {
                    foreach (var coleta in coletas)
                    {
                        infoRota = (coleta.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(coleta.CodigoIntegracao)) ? coleta.CodigoIntegracao.ToString() : coleta.CPF_CNPJ > 0 ? coleta.CPF_CNPJ.ToString() : "";


                        lat = double.Parse(coleta.Latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        lng = double.Parse(coleta.Longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);


                        rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = coleta.Nome, Pedagio = false, Informacao = infoRota, Codigo = coleta.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta });
                    }
                }

                bool AteOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;

                if (AteOrigem)
                {
                    lat = (double)origem.Latitude.Value;
                    lng = (double)origem.Longitude.Value;
                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = origem.Descricao, Pedagio = false, Informacao = infoRota, Codigo = origem.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta, UtilizaLocalidade = true });
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unitOfWork);

                if (roteirizarGoogle)
                    respostaRoteirizacao = rota.RoteirizarGoogle(ordenar, AteOrigem, key);
                else
                {
                    bool ateOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = ordenar, PontosNaRota = pontoNaRota };
                    respostaRoteirizacao = rota.Roteirizar(opcoes);
                }

                respostaRoteirizacao = AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaRoteirizacao, trechoBalsa, wayPointDestinosRemovidos, wayPointRetorno, servidor, unitOfWork);

                return AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, key, roteirizarGoogle, unitOfWork);
            }
            catch (Exception e)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = e.Message };
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao GerarRoteirizacaoWayPoint(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> destinos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUlitmoPonto, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota, List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                if ((origem == null) || (destinos == null) || (destinos.Count == 0))
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);

                if (origem.Lat == 0 || origem.Lng == 0)
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                rota.Add(origem);


                foreach (var destinatario in destinos)
                {
                    if (destinatario == null || destinatario.Lat == 0 || destinatario.Lng == 0)
                        continue;

                    rota.Add(destinatario);
                }

                bool AteOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;

                if (AteOrigem)
                    rota.Add(origem);

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unitOfWork);

                if (roteirizarGoogle)
                    respostaRoteirizacao = rota.RoteirizarGoogle(true, AteOrigem, key);
                else
                {
                    bool ateOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = true, PontosNaRota = pontoNaRota };
                    respostaRoteirizacao = rota.Roteirizar(opcoes);
                }

                return AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, key, roteirizarGoogle, unitOfWork);

            }
            catch (Exception e)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = e.Message };
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao GerarRoteirizacaoPorCidade(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto remetentePonto, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> coletas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUlitmoPonto, Repositorio.UnitOfWork unitOfWork, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota, bool ordenar)
        {
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

                Dominio.Entidades.Cliente remetente = remetentePonto?.Cliente;

                if ((remetente == null) || (destinatarios == null) || (destinatarios.Count == 0))
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Erro" };

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);

                double lat;
                double lng;

                string infoRota = remetente.CodigoIntegracao != null ? remetente.CodigoIntegracao.ToString() : remetente.CPF_CNPJ > 0 ? remetente.CPF_CNPJ.ToString() : "";

                lat = Convert.ToDouble(remetente.Localidade?.Latitude ?? 0);
                lng = Convert.ToDouble(remetente.Localidade?.Longitude ?? 0);

                if ((lat == 0) || (lng == 0))
                    return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Latitude inválida para o remetente" };

                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = remetente.Nome, Pedagio = false, Informacao = infoRota, Codigo = remetente.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta });

                if (coletas?.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto coletaPonto in coletas)
                    {
                        Dominio.Entidades.Cliente coleta = coletaPonto.Cliente;
                        infoRota = (coleta.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(coleta.CodigoIntegracao)) ? coleta.CodigoIntegracao.ToString() : coleta.CPF_CNPJ > 0 ? coleta.CPF_CNPJ.ToString() : "";

                        lat = Convert.ToDouble(coleta.Localidade?.Latitude ?? 0);
                        lng = Convert.ToDouble(coleta.Localidade?.Longitude ?? 0);

                        if ((lat == 0) || (lng == 0))
                            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Latitude inválida para a coleta" };

                        rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = coleta.Nome, Pedagio = false, Informacao = infoRota, Codigo = coleta.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta });
                    }
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto destinatarioPonto in destinatarios)
                {
                    Dominio.Entidades.Cliente destinatario = destinatarioPonto?.Cliente;
                    if (destinatario == null)
                        continue;

                    infoRota = (destinatario.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(destinatario.CodigoIntegracao)) ? destinatario.CodigoIntegracao.ToString() : destinatario.CPF_CNPJ > 0 ? destinatario.CPF_CNPJ.ToString() : "";

                    lat = Convert.ToDouble(destinatario.Localidade?.Latitude ?? 0);
                    lng = Convert.ToDouble(destinatario.Localidade?.Longitude ?? 0);

                    //#64802
                    if (destinatarioPonto.UsarOutroEndereco && destinatarioPonto.ClienteOutroEndereco != null)
                    {
                        if (destinatarioPonto.ClienteOutroEndereco.Localidade != null)
                        {
                            lat = (double)destinatarioPonto.ClienteOutroEndereco.Localidade.Latitude.Value;
                            lng = (double)destinatarioPonto.ClienteOutroEndereco.Localidade.Longitude.Value;
                        }
                        else
                        {
                            lat = destinatarioPonto.ClienteOutroEndereco.Latitude.ToDouble();
                            lng = destinatarioPonto.ClienteOutroEndereco.Longitude.ToDouble();
                        }
                    }

                    if ((lat == 0) || (lng == 0))
                        return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = "Latitude inválida para o destinatario" };

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = destinatario.Nome, Pedagio = false, Informacao = infoRota, Codigo = destinatario.Codigo, TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega, UsarOutroEndereco = destinatarioPonto.UsarOutroEndereco, CodigoOutroEndereco = destinatarioPonto.ClienteOutroEndereco?.Codigo ?? 0 });
                }

                bool AteOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;

                if (AteOrigem)
                {
                    lat = Convert.ToDouble(remetente.Localidade?.Latitude ?? 0);
                    lng = Convert.ToDouble(remetente.Localidade?.Longitude ?? 0);

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = lat, Lng = lng, Descricao = remetente.Nome, Pedagio = false, Informacao = infoRota, Codigo = remetente.Codigo, TipoPonto = TipoPontoPassagem.Retorno });
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unitOfWork);

                if (roteirizarGoogle)
                    respostaRoteirizacao = rota.RoteirizarGoogle(ordenar, AteOrigem, key);
                else
                {
                    bool ateOrigem = tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || tipoUlitmoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando;
                    Logistica.OpcoesRoteirizar opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = AteOrigem, Ordenar = ordenar, PontosNaRota = pontoNaRota };
                    respostaRoteirizacao = rota.Roteirizar(opcoes);
                }

                respostaRoteirizacao = AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaRoteirizacao, trechoBalsa, wayPointDestinosRemovidos, wayPointRetorno, servidor, unitOfWork);

                return AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, key, roteirizarGoogle, unitOfWork);
            }
            catch (Exception e)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao { Status = e.Message };
            }
        }

        public static string GerarRoteirizacaoPontosEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes, string pontos, string servidor, string key, bool roteirizarGoogle, bool pontoNaRota)
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontos);

                if (pontoPartida?.Cliente == null || pontosDaRota.Count == 0)
                {
                    Servicos.Log.TratarErro("Ponto de partida ou entregas não existentes para roteirização.");
                    return pontos;
                }

                string infoRota = pontoPartida.Cliente.CodigoIntegracao != null ? pontoPartida.Cliente.CodigoIntegracao.ToString() : pontoPartida.Cliente.CPF_CNPJ > 0 ? pontoPartida.Cliente.CPF_CNPJ.ToString() : "";

                double lat = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Latitude : pontoPartida.Cliente.Latitude);
                double lng = ParseDouble(pontoPartida.UsarOutroEndereco ? pontoPartida.ClienteOutroEndereco.Longitude : pontoPartida.Cliente.Longitude);

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint rotaPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Lat = lat,
                    Lng = lng,
                    Codigo = pontoPartida.Codigo,
                    CodigoCliente = pontoPartida.Cliente.Codigo,
                    Descricao = pontoPartida.Cliente.Nome,
                    Pedagio = false,
                    UsarOutroEndereco = pontoPartida.UsarOutroEndereco,
                    Informacao = infoRota,
                    SequenciaPredefinida = pontoPartida.SequenciaPreDefinida,
                    TipoPonto = pontoPartida.TipoPontoPassagem,
                    ColetaEquipamento = pontoPartida.ColetaEquipamento,
                    Fronteira = pontoPartida.TipoPontoPassagem == TipoPontoPassagem.Fronteira,
                };

                //Roteiriza do ponto de origem até cada entrega individualmente
                for (int i = 0; i < pontosDaRota.Count; i++)
                {
                    if (pontosDaRota[i].pontopassagem || pontosDaRota[i].pedagio || pontosDaRota[i].fronteira || pontosDaRota[i].tipoponto != TipoPontoPassagem.Entrega)//Utilizar apenas as entregas
                        continue;

                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto destinatario = clientes.Where(obj => obj.Cliente.CPF_CNPJ == pontosDaRota[i].codigo).FirstOrDefault();

                    if (destinatario == null && pontosDaRota[i].usarOutroEndereco)
                        destinatario = clientes.Where(obj => obj.Cliente.CPF_CNPJ == pontosDaRota[i].codigo_cliente).FirstOrDefault();

                    if (destinatario == null)
                        continue;

                    Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);
                    rota.Add(rotaPartida);

                    infoRota = (destinatario.Cliente.CodigoIntegracao != null) && (!string.IsNullOrWhiteSpace(destinatario.Cliente.CodigoIntegracao)) ? destinatario.Cliente.CodigoIntegracao.ToString() : destinatario.Cliente.CPF_CNPJ > 0 ? destinatario.Cliente.CPF_CNPJ.ToString() : "";

                    PointF geolocalizacao = LocalEntregaCliente(destinatario.Cliente);

                    lat = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Latitude : geolocalizacao.Y.ToString());
                    lng = ParseDouble(destinatario.UsarOutroEndereco ? destinatario.ClienteOutroEndereco.Longitude : geolocalizacao.X.ToString());

                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Lat = lat,
                        Lng = lng,
                        Codigo = destinatario.Codigo,
                        CodigoCliente = destinatario.Cliente.Codigo,
                        Descricao = destinatario.Cliente.Nome,
                        Pedagio = false,
                        SequenciaPredefinida = destinatario.SequenciaPreDefinida,
                        Informacao = infoRota,
                        UsarOutroEndereco = destinatario.UsarOutroEndereco,
                        Fronteira = destinatario.TipoPontoPassagem == TipoPontoPassagem.Fronteira,
                        TipoPonto = TipoPontoPassagem.Entrega
                    });

                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;
                    if (roteirizarGoogle)
                        respostaRoteirizacao = rota.RoteirizarGoogle(false, false, key);
                    else
                    {
                        Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                        {
                            AteOrigem = false,
                            Ordenar = false,
                            PontosNaRota = pontoNaRota
                        };
                        respostaRoteirizacao = rota.Roteirizar(opcoes);

                        if (respostaRoteirizacao.Distancia == 0 && respostaRoteirizacao.Status != "OK")
                        {
                            Servicos.Log.TratarErro($"Falha na roteirização para a lat {lat} lng {lng}, status: {respostaRoteirizacao.Status}", "FalhaRoteirizacaoOSM");
                            System.Threading.Thread.Sleep(2000);

                            respostaRoteirizacao = rota.Roteirizar(opcoes);
                            Servicos.Log.TratarErro($"Roteirização para a lat {lat} lng {lng} executada novamente, status de retorno: {respostaRoteirizacao.Status}", "FalhaRoteirizacaoOSM");
                        }
                        else
                        {
                            Servicos.Log.TratarErro($"Roteirização para a lat {lat} lng {lng}", "FalhaRoteirizacaoOSM");

                        }
                    }

                    pontosDaRota[i].distanciaDireta = (int)(respostaRoteirizacao.Distancia * 1000);
                }

                return JsonConvert.SerializeObject(pontosDaRota);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return pontos;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao AnalisarGerarRoteirizacaoComDesvioZonaExclusao(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao, string key, bool roteirizarGoogle, Repositorio.UnitOfWork unitOfWork, bool ateOrigem = false, bool ordenou = false)
        {
            Servicos.Embarcador.Logistica.RestricaoRodagem restricaoRodagem = new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork);
            if (!restricaoRodagem.IsPossuiRestricaoZonaExclusaoRota(respostaRoteirizacao.Polilinha))
                return respostaRoteirizacao;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNaRota = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(respostaRoteirizacao.PontoDaRota);
            if ((pontosNaRota?.Count ?? 0) < 2)
                return respostaRoteirizacao;

            //Se chegou aqui.. é porque possui restrição, então vamos validar item a item...
            // Para identificar entre qual "Parada" existe a restrição.... para desviar...
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> allWayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origemRota = pontosNaRota[0];

            allWayPoints.Add(origemRota);

            for (int i = 1; i < pontosNaRota.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem = pontosNaRota[i - 1];
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint destino = pontosNaRota[i];

                rota.Clear();
                rota.Add(origem);
                rota.Add(destino);

                if (ateOrigem && (origemRota.Lat == destino.Lat) && (origemRota.Lng == destino.Lng))
                    ordenou = false;

                Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoesRoteirizacao = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = ateOrigem, Ordenar = !ordenou, PontosNaRota = false };
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaTrecho = null;

                if (roteirizarGoogle)
                    respostaTrecho = rota.RoteirizarGoogle(true, true, key);
                else
                    respostaTrecho = rota.Roteirizar(opcoesRoteirizacao);

                if (respostaTrecho.Status == "OK")
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> desvios = restricaoRodagem.WayPointsDesvioRestricaoZonaExclusaoRota(respostaTrecho.Polilinha);
                    if ((desvios?.Count ?? 0) > 0)
                    {
                        rota.Clear();
                        rota.Add(origem);
                        rota.Add(desvios);
                        rota.Add(destino);

                        if (roteirizarGoogle)
                            respostaTrecho = rota.RoteirizarGoogle(true, true, key);
                        else
                            respostaTrecho = rota.Roteirizar(opcoesRoteirizacao);

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNoTrecho = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(respostaTrecho.PontoDaRota);
                        //Removendo a origem pois ela já está na lista de todos os pontos.
                        if (pontosNoTrecho.Count > 0)
                            pontosNoTrecho.RemoveAt(0);

                        //Adicionando os demais pontos na lista de todos os pontos...
                        allWayPoints.AddRange(pontosNoTrecho);
                    }
                    else
                        allWayPoints.Add(destino);
                }
                else
                    return respostaRoteirizacao;
            }

            //Agora vamos gerar uma nova roteirização com todos os pontos, já ordenados anteriormente...
            Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoesRoteirizacaoFinal = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = false, PontosNaRota = false };
            rota.Clear();
            rota.Add(allWayPoints);

            if (roteirizarGoogle)
                return rota.RoteirizarGoogle(false, false, key);
            else
                return rota.Roteirizar(opcoesRoteirizacaoFinal);

        }

        public static Dominio.Entidades.Embarcador.Logistica.TrechoBalsa AnalisarGerarRoteirizacaoComTrechoBalsa(Servicos.Embarcador.Logistica.Roteirizacao rota, ref List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos, ref Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            int indiceUltimo = rota.GetWayPoints().Count - 1;
            if (rota.GetWayPoints()[indiceUltimo].TipoPonto == TipoPontoPassagem.Retorno)
            {
                wayPointRetorno = rota.GetWayPoints()[indiceUltimo];
                rota.RemoveAt(indiceUltimo);
                indiceUltimo--;
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem = rota.GetWayPoints()[0];
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint destino = rota.GetWayPoints()[indiceUltimo];

            Servicos.Embarcador.Logistica.TrechoBalsa servicoTrechoBalsa = new Servicos.Embarcador.Logistica.TrechoBalsa(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
            {
                Latitude = origem.Lat,
                Longitude = origem.Lng
            };

            Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = servicoTrechoBalsa.TrechoBalsaRoteirizacao(wayPointOrigem, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
            {
                Latitude = destino.Lat,
                Longitude = destino.Lng
            });

            if (trechoBalsa != null)
            {
                wayPointDestinosRemovidos.Add(destino);
                rota.RemoveAt(indiceUltimo);

                //Analisar se existem mais entregas.. dentro do trecho balsa...
                indiceUltimo--;
                for (int i = indiceUltimo; i >= 0; i--)
                {
                    destino = rota.GetWayPoints()[i];
                    if (destino.TipoPonto == TipoPontoPassagem.Entrega)
                    {
                        Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa2 = servicoTrechoBalsa.TrechoBalsaRoteirizacao(wayPointOrigem, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                        {
                            Latitude = destino.Lat,
                            Longitude = destino.Lng
                        });
                        if (trechoBalsa.Codigo == trechoBalsa2?.Codigo)
                        {
                            wayPointDestinosRemovidos.Add(destino);
                            rota.RemoveAt(i);
                        }
                    }
                }

                System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                // Adicionado o porto de origem...
                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                {
                    Lat = Convert.ToDouble(trechoBalsa.PortoOrigem.Latitude.Replace(",", "."), provider),
                    Lng = Convert.ToDouble(trechoBalsa.PortoOrigem.Longitude.Replace(",", "."), provider),
                    Codigo = trechoBalsa.PortoOrigem.Codigo,
                    Descricao = trechoBalsa.PortoOrigem.Descricao,
                    Informacao = trechoBalsa.PortoOrigem.Descricao,
                    TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
                });
            }
            else if (wayPointRetorno != null)
                rota.Add(wayPointRetorno);

            return trechoBalsa;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao, Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos, Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno, string servidor, Repositorio.UnitOfWork unitOfWork)
        {
            if (trechoBalsa == null || (wayPointDestinosRemovidos?.Count ?? 0) == 0)
                return respostaRoteirizacao;

            string polilinha = respostaRoteirizacao.Polilinha;
            decimal distancia = respostaRoteirizacao.Distancia;
            string pontosDaRota = respostaRoteirizacao.PontoDaRota;

            Repositorio.Embarcador.Logistica.TempoBalsa repositorioTempoBalsa = new Repositorio.Embarcador.Logistica.TempoBalsa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.TempoBalsa> temposBalsa = repositorioTempoBalsa.BuscarPorTrechoBalsa(trechoBalsa.Codigo);
            Dominio.Entidades.Embarcador.Logistica.TempoBalsa tempoBalsaVigente = (from obj in temposBalsa
                                                                                   where obj.DataInicio <= DateTime.Now.Date && obj.DataFinal >= DateTime.Now.Date
                                                                                   select obj).FirstOrDefault();

            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            distancia += (int)(trechoBalsa.Distancia);
            // Vamos decifrar a polilinha, adicionar o Porto de Destino, depois o Endereço de destino.. e cifrar novamente.
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(pontosDaRota);
            int tempoExtra = (tempoBalsaVigente?.TempoGeral ?? 0) * 24 * 60;

            // Adicionado o Porto de Destino            
            pontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
            {
                Lat = Convert.ToDouble(trechoBalsa.PortoDestino.Latitude.Replace(",", "."), provider),
                Lng = Convert.ToDouble(trechoBalsa.PortoDestino.Longitude.Replace(",", "."), provider),
                Codigo = trechoBalsa.PortoDestino.Codigo,
                Descricao = trechoBalsa.PortoDestino.Descricao,
                Distancia = (int)trechoBalsa.Distancia,
                Index = pontos.Count,
                Informacao = trechoBalsa.PortoDestino.Descricao,
                Sequencia = pontos.Count,
                Tempo = tempoExtra,
                TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
            });

            // Adicionado o cliente final.
            pontos.AddRange(wayPointDestinosRemovidos);

            // Precisamos montar o retorno.. saindo do último ponto.. porto de destino.. porto de origem.... retorno..
            if (wayPointRetorno != null)
            {
                //Porto de destino (volta)....
                pontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                {
                    Lat = Convert.ToDouble(trechoBalsa.PortoDestino.Latitude.Replace(",", "."), provider),
                    Lng = Convert.ToDouble(trechoBalsa.PortoDestino.Longitude.Replace(",", "."), provider),
                    Codigo = trechoBalsa.PortoDestino.Codigo,
                    Descricao = trechoBalsa.PortoDestino.Descricao,
                    Index = pontos.Count,
                    Informacao = trechoBalsa.PortoDestino.Descricao,
                    Sequencia = pontos.Count,
                    TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
                });
                // Porto de origemmmm (volta)
                pontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                {
                    Lat = Convert.ToDouble(trechoBalsa.PortoOrigem.Latitude.Replace(",", "."), provider),
                    Lng = Convert.ToDouble(trechoBalsa.PortoOrigem.Longitude.Replace(",", "."), provider),
                    Codigo = trechoBalsa.PortoOrigem.Codigo,
                    Descricao = trechoBalsa.PortoOrigem.Descricao,
                    Distancia = (int)trechoBalsa.Distancia,
                    Index = pontos.Count,
                    Informacao = trechoBalsa.PortoOrigem.Descricao,
                    Sequencia = pontos.Count,
                    Tempo = tempoExtra,
                    TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
                });
                // Ponto de retorno (volta.)
                pontos.Add(wayPointRetorno);
            }
            pontosDaRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontos);

            respostaRoteirizacao.PontoDaRota = pontosDaRota;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinha);

            //Tentar roteirizar entre o trecho balsa do porto de destino e o ponto.
            bool roteirizou = false;
            try
            {
                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);
                rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                {
                    Codigo = trechoBalsa.Codigo,
                    Descricao = trechoBalsa.Descricao,
                    Lat = trechoBalsa.PortoDestino.Latitude.ToDouble(),
                    Lng = trechoBalsa.PortoDestino.Longitude.ToDouble(),
                    TipoPonto = TipoPontoPassagem.Balsa
                });
                rota.Add(wayPointDestinosRemovidos);
                var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                {
                    AteOrigem = false,
                    Ordenar = false,
                    PontosNaRota = false
                };
                var respostaRoteirizacaoTrechoPosBalsa = rota.Roteirizar(opcoes);
                if (respostaRoteirizacaoTrechoPosBalsa.Status?.ToUpper() == "OK")
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinhaTrechoPosBalsa = Servicos.Embarcador.Logistica.Polilinha.Decodificar(respostaRoteirizacaoTrechoPosBalsa.Polilinha);
                    distancia += respostaRoteirizacaoTrechoPosBalsa.Distancia;
                    tempoExtra += respostaRoteirizacaoTrechoPosBalsa.TempoMinutos;
                    coordenadasPolilinha.AddRange(coordenadasPolilinhaTrechoPosBalsa);
                    roteirizou = true;
                }
            }
            catch (Exception ex)
            {
                //Caso nao consiga roteirizar entre o porto de destino e o cliente final, vamos manter a linha reta na polilinha.
                Servicos.Log.TratarErro(ex, "TrechoBalsa");
                roteirizou = false;
            }

            if (!roteirizou)
            {
                coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(trechoBalsa.PortoDestino.Latitude, trechoBalsa.PortoDestino.Longitude));
                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointDestinoRemovido in wayPointDestinosRemovidos)
                    coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(wayPointDestinoRemovido.Lat, wayPointDestinoRemovido.Lng));
            }

            if (wayPointRetorno != null)
            {
                try
                {
                    Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);
                    rota.Add(wayPointDestinosRemovidos[wayPointDestinosRemovidos.Count - 1]);
                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Codigo = trechoBalsa.Codigo,
                        Descricao = trechoBalsa.Descricao,
                        Lat = trechoBalsa.PortoDestino.Latitude.ToDouble(),
                        Lng = trechoBalsa.PortoDestino.Longitude.ToDouble(),
                        TipoPonto = TipoPontoPassagem.Balsa
                    });
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                    {
                        AteOrigem = false,
                        Ordenar = false,
                        PontosNaRota = false
                    };
                    // Roteriizando da última entrega até o porto de destino..
                    var respostaRoteirizacaoTrechoPosBalsa = rota.Roteirizar(opcoes);
                    if (respostaRoteirizacaoTrechoPosBalsa.Status?.ToUpper() == "OK")
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinhaTrechoPosBalsa = Servicos.Embarcador.Logistica.Polilinha.Decodificar(respostaRoteirizacaoTrechoPosBalsa.Polilinha);
                        distancia += respostaRoteirizacaoTrechoPosBalsa.Distancia;
                        tempoExtra += respostaRoteirizacaoTrechoPosBalsa.TempoMinutos;
                        coordenadasPolilinha.AddRange(coordenadasPolilinhaTrechoPosBalsa);
                        roteirizou = true;
                    }
                }
                catch (Exception ex)
                {
                    //Caso nao consiga roteirizar entre o porto de destino e o cliente final, vamos manter a linha reta na polilinha.
                    Servicos.Log.TratarErro(ex, "TrechoBalsa");
                    roteirizou = false;
                }

                if (!roteirizou)
                {
                    coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(wayPointDestinosRemovidos[wayPointDestinosRemovidos.Count - 1].Lat, wayPointDestinosRemovidos[wayPointDestinosRemovidos.Count - 1].Lng));
                    coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(trechoBalsa.PortoDestino.Latitude, trechoBalsa.PortoDestino.Longitude));
                }

                // Última perna de retorno... porto de origem até a origem...
                try
                {
                    Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidor);
                    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
                    {
                        Codigo = trechoBalsa.Codigo,
                        Descricao = trechoBalsa.Descricao,
                        Lat = trechoBalsa.PortoOrigem.Latitude.ToDouble(),
                        Lng = trechoBalsa.PortoOrigem.Longitude.ToDouble(),
                        TipoPonto = TipoPontoPassagem.Balsa
                    });
                    rota.Add(wayPointRetorno);
                    var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                    {
                        AteOrigem = false,
                        Ordenar = false,
                        PontosNaRota = false
                    };
                    // Roteriizando da última entrega até o porto de destino..
                    var respostaRoteirizacaoTrechoPosBalsa = rota.Roteirizar(opcoes);
                    if (respostaRoteirizacaoTrechoPosBalsa.Status?.ToUpper() == "OK")
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinhaTrechoPosBalsa = Servicos.Embarcador.Logistica.Polilinha.Decodificar(respostaRoteirizacaoTrechoPosBalsa.Polilinha);
                        distancia += respostaRoteirizacaoTrechoPosBalsa.Distancia;
                        tempoExtra += respostaRoteirizacaoTrechoPosBalsa.TempoMinutos;
                        coordenadasPolilinha.AddRange(coordenadasPolilinhaTrechoPosBalsa);
                        roteirizou = true;
                    }
                }
                catch (Exception ex)
                {
                    //Caso nao consiga roteirizar entre o porto de destino e o cliente final, vamos manter a linha reta na polilinha.
                    Servicos.Log.TratarErro(ex, "TrechoBalsa");
                    roteirizou = false;
                }

                if (!roteirizou)
                {
                    coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(trechoBalsa.PortoOrigem.Latitude, trechoBalsa.PortoOrigem.Longitude));
                    coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(wayPointRetorno.Lat, wayPointRetorno.Lng));
                }
            }

            polilinha = Servicos.Embarcador.Logistica.Polilinha.Codificar(coordenadasPolilinha);

            respostaRoteirizacao.Polilinha = polilinha;
            respostaRoteirizacao.Distancia = distancia;
            respostaRoteirizacao.TempoMinutos += tempoExtra;
            respostaRoteirizacao.TempoHoras += (tempoExtra / 60 / 24);

            return respostaRoteirizacao;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto ObterClienteTipoPonto(Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem tipoPontoPassagem, bool coletaEquipamento, Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco, int sequenciaPreDefinida, long codigoClientePrimeiraEntrega = 0)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto clienteTipoPonto = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();
            clienteTipoPonto.Cliente = cliente.ClientePai != null && clienteOutroEndereco == null && tipoPontoPassagem == TipoPontoPassagem.Coleta ? cliente.ClientePai : cliente;
            clienteTipoPonto.TipoPontoPassagem = tipoPontoPassagem;
            clienteTipoPonto.SequenciaPreDefinida = sequenciaPreDefinida;
            clienteTipoPonto.ColetaEquipamento = coletaEquipamento;
            clienteTipoPonto.UsarOutroEndereco = clienteOutroEndereco != null ? true : false;
            clienteTipoPonto.PrimeiraEntrega = (tipoPontoPassagem != TipoPontoPassagem.Entrega || codigoClientePrimeiraEntrega == 0 ? false : (cliente.ClientePai ?? cliente).Codigo == codigoClientePrimeiraEntrega);

            if (clienteTipoPonto.UsarOutroEndereco)
            {
                clienteTipoPonto.ClienteOutroEndereco = clienteOutroEndereco;
                clienteTipoPonto.Codigo = clienteOutroEndereco.Codigo;
            }
            else
                clienteTipoPonto.Codigo = cliente?.CPF_CNPJ ?? 0;

            return clienteTipoPonto;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterCoordenadasPorCidade(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, /*string key,*/ Repositorio.UnitOfWork unitOfWork, bool atualizarLatitudePessoa = true)
        {
            if (localidade == null)
                return null;

            if ((localidade?.Latitude == null || localidade?.Longitude == null) || (localidade.Latitude == 0 || localidade.Longitude == 0))
                AtualizarCoordenadasCidade(localidade, configuracaoIntegracao, unitOfWork);

            if ((localidade?.Latitude != null && localidade?.Longitude != null) && (localidade.Latitude != 0 && localidade.Longitude != 0) && (atualizarLatitudePessoa))
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = localidade.Latitude.ToString().ToDouble(), Lng = localidade.Longitude.ToString().ToDouble() };

            return null;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterCoordenadasPorCep(int cep, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Repositorio.Localidades.Geo repositorioGeo = new AdminMultisoftware.Repositorio.Localidades.Geo(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo geo = repositorioGeo.BuscarPorCEP(cep);
            //Caso não tenha achado no CEP ou a LAT LNG do cep é "0"
            if (geo == null || (geo?.latitude ?? 0) == 0) return null;
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint((double)((geo?.latitude ?? 0) != 0 ? geo.latitude : geo.latitude_nominatim), (double)((geo?.latitude ?? 0) != 0 ? geo.longitude : geo.longitude_nominatim));
        }

        private static string FormataRuaGeocodificar(string endereco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro? tipoLogradouro)
        {
            string enderecoFinal = string.Empty;

            if (endereco.ToUpper().StartsWith("R ") || endereco.ToUpper().StartsWith("R. "))
                enderecoFinal += $"Rua {endereco.ToUpper().Replace("R ", "").Replace("R. ", "").Trim()}, ";
            else if (endereco.ToUpper().StartsWith("ROD ") || endereco.ToUpper().StartsWith("ROD. "))
                enderecoFinal += $"Rodovia {endereco.ToUpper().Replace("ROD ", "").Replace("ROD. ", "").Trim()}, ";
            else if (endereco.ToUpper().StartsWith("AV ") || endereco.ToUpper().StartsWith("AV. "))
                enderecoFinal += $"Avenida {endereco.ToUpper().Replace("AV ", "").Replace("AV. ", "").Trim()}, ";
            else if (endereco.ToUpper().StartsWith("EST ") || endereco.ToUpper().StartsWith("ESTR ") || endereco.ToUpper().StartsWith("EST. ") || endereco.ToUpper().StartsWith("ESTR. "))
                enderecoFinal += $"Estrada {endereco.ToUpper().Replace("EST ", "").Replace("ESTR ", "").Trim().Replace("EST. ", "").Trim().Replace("ESTR. ", "").Trim()}, ";
            else if (endereco.ToUpper().StartsWith("PCA ") || endereco.ToUpper().StartsWith("PCA. "))
                enderecoFinal += $"Praça {endereco.ToUpper().Replace("PCA ", "").Replace("PCA. ", "").Trim()}, ";
            else if (endereco.ToUpper().StartsWith("TV ") || endereco.ToUpper().StartsWith("TV. "))
                enderecoFinal += $"Travessa {endereco.ToUpper().Replace("TV ", "").Replace("TV. ", "").Trim()}, ";
            else if (!endereco.ToUpper().Contains("RUA") && !endereco.ToUpper().Contains("AVENIDA") && !endereco.ToUpper().Contains("Rodovia".ToUpper()))
                enderecoFinal += $"{(tipoLogradouro.HasValue ? TipoLogradouroHelper.ObterDescricao(tipoLogradouro ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua) + " " : "") + endereco.Trim()}, ";
            else
                enderecoFinal += $"{endereco.Trim()}, ";

            // Problema geocodificar nominatim....
            if (enderecoFinal.ToUpper().StartsWith("RUA BC "))
                enderecoFinal = enderecoFinal.Replace("RUA BC ", "RUA BECO ");
            else if (enderecoFinal.ToUpper().StartsWith("BC "))
                enderecoFinal = enderecoFinal.Replace("BC ", "BECO ");

            return enderecoFinal;
        }

        public static string ObterOutroEndereco(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco pessoa, ref string enderecoGoogle, GeoServiceGeocoding geoServiceGeocoding, ref string bairro)
        {
            //enderecoGoogle = "PAÍS BR, ";

            ////endereco += !string.IsNullOrWhiteSpace(pessoa?.Localidade?.Descricao) ? "CIDADE " + pessoa?.Localidade?.Descricao + ", " : pessoa?.Localidade.DescricaoCidadeEstado + ", ";

            ////endereco += !string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Descricao) ? "ESTADO " + pessoa?.Localidade?.Estado?.Descricao + ", " : pessoa?.Localidade.DescricaoCidadeEstado + ", ";

            //enderecoGoogle += !string.IsNullOrWhiteSpace(pessoa?.Localidade?.Descricao) ? " " + pessoa?.Localidade?.Descricao + " - " : pessoa?.Localidade.DescricaoCidadeEstado + ", ";

            //enderecoGoogle += !string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Descricao) ? pessoa?.Localidade?.Estado?.Descricao + ", " : pessoa?.Localidade.DescricaoCidadeEstado + ", ";

            //if (!string.IsNullOrWhiteSpace(pessoa.CEP))
            //    enderecoGoogle += "CEP " + pessoa?.CEP + ", ";

            //if (!string.IsNullOrWhiteSpace(pessoa?.Endereco))
            //    enderecoGoogle += $"ENDEREÇO {pessoa?.Endereco}, ";

            //if ((!string.IsNullOrWhiteSpace(pessoa?.Numero)) && (pessoa?.Numero != "S/N") && (pessoa?.Numero != "SN"))
            //    enderecoGoogle += pessoa?.Numero;// + ", ";


            enderecoGoogle = "";

            if (!string.IsNullOrWhiteSpace(pessoa?.Endereco))
                enderecoGoogle = FormataRuaGeocodificar(pessoa?.Endereco, pessoa.TipoLogradouro);    // enderecoGoogle += $"{pessoa?.Endereco}, ";

            if ((!string.IsNullOrWhiteSpace(pessoa?.Numero)) && (pessoa?.Numero != "S/N"))
                enderecoGoogle += pessoa?.Numero + ", ";

            if (!string.IsNullOrWhiteSpace(pessoa?.Bairro))
                enderecoGoogle += "bairro " + pessoa?.Bairro + ", ";

            enderecoGoogle += pessoa?.Localidade?.DescricaoCidadeEstado + ", ";

            if (!string.IsNullOrWhiteSpace(pessoa.CEP))
                enderecoGoogle += " " + pessoa?.CEP + ", ";

            enderecoGoogle += pessoa?.Localidade?.Pais?.Descricao + " ";


            if (geoServiceGeocoding == GeoServiceGeocoding.Nominatim)
            {
                //https://nominatim.org/release-docs/latest/api/Search/#parameters
                /*street=<housenumber> <streetname>
                    city=<city>
                    county=<county>
                    state=<state>
                    country=<country>
                    postalcode=<postalcode>
                */

                // Ex: rua rio grande do sul, 355, chapecó, sc, brasil, 89815-435
                string endereco = "";

                if (!string.IsNullOrWhiteSpace(pessoa?.Endereco))
                    endereco = FormataRuaGeocodificar(pessoa?.Endereco, pessoa.TipoLogradouro);

                if ((!string.IsNullOrWhiteSpace(pessoa?.Numero)) && (pessoa?.Numero != "S/N"))
                    if (!string.IsNullOrWhiteSpace(pessoa?.Numero.ObterSomenteNumeros()))
                        endereco += pessoa?.Numero.ObterSomenteNumeros().ToInt() + ", ";

                if (!string.IsNullOrWhiteSpace(pessoa?.Bairro))
                {
                    bairro = pessoa.Bairro.ToUpper().Replace("B ", "").Replace("BAIRRO ", "").Trim() + ", ";
                    endereco += bairro;
                }

                if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Descricao))
                {
                    endereco += pessoa?.Localidade?.Descricao.Trim();
                    if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Nome))
                        endereco += " - " + pessoa?.Localidade?.Estado?.Nome.Trim() + ", ";
                }
                else if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Nome))
                    endereco += pessoa?.Localidade?.Estado?.Nome.Trim() + ", ";

                endereco += (pessoa?.Localidade?.Pais?.Abreviacao ?? "BRASIL");

                // Se o endereço for null, vamos utilizar o CEP
                if (string.IsNullOrWhiteSpace(pessoa?.Endereco) && !string.IsNullOrWhiteSpace(pessoa.CEP))
                {
                    string cepLimpo = pessoa?.CEP.ObterSomenteNumeros();
                    if (cepLimpo.Length == 8)
                        cepLimpo = cepLimpo.Insert(5, "-");
                    endereco += ", " + cepLimpo;
                }

                return endereco.Trim();
            }

            return enderecoGoogle;
        }

        public static string ObterEndereco(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, ref string enderecoGoogle, ref string bairro)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google);

            enderecoGoogle = "";

            if (!string.IsNullOrWhiteSpace(pessoa?.Endereco))
                enderecoGoogle += FormataRuaGeocodificar(pessoa?.Endereco, pessoa.TipoLogradouro);  //enderecoGoogle += $"{pessoa?.Endereco}, ";

            if ((!string.IsNullOrWhiteSpace(pessoa?.Numero)) && (pessoa?.Numero != "S/N"))
                enderecoGoogle += pessoa?.Numero + ", ";

            if (!string.IsNullOrWhiteSpace(pessoa?.Bairro))
                enderecoGoogle += "bairro " + pessoa?.Bairro + ", ";

            enderecoGoogle += pessoa?.Localidade?.DescricaoCidadeEstado + ", ";

            if (!string.IsNullOrWhiteSpace(pessoa.CEP))
                enderecoGoogle += (!enderecoGoogle.EndsWith(" ") ? " " : "") + pessoa?.CEP + ", ";

            enderecoGoogle += pessoa?.Localidade?.Pais?.Descricao + " ";

            if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Nominatim)
            {
                //https://nominatim.org/release-docs/latest/api/Search/#parameters
                /*street=<housenumber> <streetname>
                    city=<city>
                    county=<county>
                    state=<state>
                    country=<country>
                    postalcode=<postalcode>
                */

                // Ex: rua rio grande do sul, 355, chapecó, sc, brasil, 89815-435
                string endereco = "";

                if (!string.IsNullOrWhiteSpace(pessoa?.Endereco))
                    endereco += FormataRuaGeocodificar(pessoa?.Endereco, pessoa.TipoLogradouro);

                if ((!string.IsNullOrWhiteSpace(pessoa?.Numero)) && (pessoa?.Numero != "S/N"))
                    if (!string.IsNullOrWhiteSpace(pessoa?.Numero.ObterSomenteNumeros()))
                        endereco += pessoa?.Numero.ObterSomenteNumeros().ToInt() + ", ";

                if (!string.IsNullOrWhiteSpace(pessoa?.Bairro))
                {
                    bairro = pessoa.Bairro.ToUpper().Replace("B ", "").Replace("BAIRRO ", "").Trim() + ", ";
                    endereco += bairro;
                }

                if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Descricao))
                {
                    endereco += pessoa?.Localidade?.Descricao.Trim();
                    if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Nome))
                        endereco += " - " + pessoa?.Localidade?.Estado?.Nome.Trim() + ", ";
                }
                else if (!string.IsNullOrWhiteSpace(pessoa?.Localidade?.Estado?.Nome))
                    endereco += pessoa?.Localidade?.Estado?.Nome.Trim() + ", ";

                endereco += (pessoa?.Pais?.Abreviacao ?? "BRASIL");

                // Se o endereço for null, vamos utilizar o CEP
                if (string.IsNullOrWhiteSpace(pessoa?.Endereco) && !string.IsNullOrWhiteSpace(pessoa.CEP))
                {
                    string cepLimpo = pessoa?.CEP.ObterSomenteNumeros();
                    if (cepLimpo.Length == 8)
                        cepLimpo = cepLimpo.Insert(5, "-");
                    endereco += ", " + cepLimpo;
                }

                return endereco.Trim();
            }

            return enderecoGoogle;
        }

        public static string ObterEndereco(Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosEndereco dadosEndereco, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, ref string enderecoGoogle, ref string bairro)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google);

            enderecoGoogle = "";

            if (!string.IsNullOrWhiteSpace(dadosEndereco?.endereco))
                enderecoGoogle += FormataRuaGeocodificar(dadosEndereco?.endereco, dadosEndereco?.TipoLogradouro);

            if ((!string.IsNullOrWhiteSpace(dadosEndereco?.numero)) && (dadosEndereco?.numero != "S/N"))
                enderecoGoogle += dadosEndereco?.numero + ", ";

            if (!string.IsNullOrWhiteSpace(dadosEndereco?.bairro))
                enderecoGoogle += "bairro " + dadosEndereco?.bairro + ", ";

            if (!string.IsNullOrWhiteSpace(dadosEndereco?.cidade))
            {
                enderecoGoogle += dadosEndereco?.cidade;
                if (!string.IsNullOrWhiteSpace(dadosEndereco?.estado))
                    enderecoGoogle += " - " + dadosEndereco?.estado;
                enderecoGoogle += ", ";
            }

            if (!string.IsNullOrWhiteSpace(dadosEndereco.cep))
                enderecoGoogle += " " + dadosEndereco?.cep + ", ";

            if (!string.IsNullOrWhiteSpace(dadosEndereco?.pais))
                enderecoGoogle += dadosEndereco?.pais + " ";

            if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Nominatim)
            {
                //https://nominatim.org/release-docs/latest/api/Search/#parameters
                /*street=<housenumber> <streetname>
                    city=<city>
                    county=<county>
                    state=<state>
                    country=<country>
                    postalcode=<postalcode>
                */

                // Ex: rua rio grande do sul, 355, chapecó, sc, brasil, 89815-435
                string endereco = "";

                if (!string.IsNullOrWhiteSpace(dadosEndereco?.endereco))
                    endereco += FormataRuaGeocodificar(dadosEndereco?.endereco, dadosEndereco?.TipoLogradouro);

                if ((!string.IsNullOrWhiteSpace(dadosEndereco?.numero)) && (dadosEndereco?.numero != "S/N"))
                    if (!string.IsNullOrWhiteSpace(dadosEndereco?.numero.ObterSomenteNumeros()))
                        endereco += dadosEndereco?.numero.ObterSomenteNumeros().ToInt() + ", ";

                if (!string.IsNullOrWhiteSpace(dadosEndereco?.bairro))
                {
                    bairro = dadosEndereco.bairro.ToUpper().Replace("B ", "").Replace("BAIRRO ", "").Trim() + ", ";
                    endereco += bairro;
                }

                if (!string.IsNullOrWhiteSpace(dadosEndereco?.cidade))
                {
                    endereco += dadosEndereco?.cidade.Trim();
                    if (!string.IsNullOrWhiteSpace(dadosEndereco?.estado))
                        endereco += " - " + dadosEndereco?.estado.Trim() + ", ";
                }
                else if (!string.IsNullOrWhiteSpace(dadosEndereco?.estado))
                    endereco += dadosEndereco?.estado.Trim() + ", ";

                endereco += (dadosEndereco?.pais ?? "BRASIL");

                // Se o endereço for null, vamos utilizar o CEP
                if (string.IsNullOrWhiteSpace(dadosEndereco?.endereco) && !string.IsNullOrWhiteSpace(dadosEndereco.cep))
                {
                    string cepLimpo = dadosEndereco?.cep.ObterSomenteNumeros();
                    if (cepLimpo.Length == 8)
                        cepLimpo = cepLimpo.Insert(5, "-");
                    endereco += ", " + cepLimpo;
                }

                return endereco.Trim();
            }

            return enderecoGoogle;
        }

        public static string ObterEnderecoLocalidade(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, ref string enderecoGoogle, bool cep = false)
        {
            GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? GeoServiceGeocoding.Google);

            if (!cep)
                enderecoGoogle = localidade.Pais?.Descricao + " " + localidade?.Descricao + " " + ((localidade?.Estado?.Descricao ?? "EX") == "EX" ? "" : localidade?.Estado?.Descricao);
            else
                enderecoGoogle = localidade.Pais?.Descricao + " " + localidade?.CEP + " " + (localidade?.CEP?.Length > 5 ? localidade.CEP.Substring(0, 5) : "");

            if (geoServiceGeocoding == GeoServiceGeocoding.Nominatim)
            {
                //https://nominatim.org/release-docs/latest/api/Search/#parameters
                /*street=<housenumber> <streetname>
                    city=<city>
                    county=<county>
                    state=<state>
                    country=<country>
                    postalcode=<postalcode>
                */
                string endereco = localidade?.Descricao + ((localidade?.Estado?.Descricao ?? "EX") == "EX" ? "" : ", " + localidade?.Estado?.Descricao) + (string.IsNullOrEmpty(localidade.Pais?.Descricao) ? "" : ", " + localidade.Pais?.Descricao);
                if (!cep)
                    return endereco;
                else
                    return endereco + (string.IsNullOrEmpty(localidade?.CEP) ? "" : "," + localidade?.CEP);
            }

            return enderecoGoogle;
        }

        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0.0;

            return Convert.ToDouble(value?.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static void SetarPrevisaoEntrega(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            if (!(carga?.HorarioCarregamentoInformadoNoPedido ?? false))
                return;

            if (carga.Rota == null)
                return;

            Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedido = (from cargaPedido in carga.Pedidos select cargaPedido).OrderByDescending(o => o.OrdemEntrega).ThenBy(o => o.Codigo).ToList();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = listaCargasPedido.LastOrDefault();


            if ((ultimaCargaPedido == null) || (carga.Rota.TempoDeViagemEmMinutos == 0))
                return;

            listaCargasPedido.Remove(ultimaCargaPedido);

            DateTime dataBase = carga.DataInicioViagemPrevista ?? cargaJanelaCarregamento.TerminoCarregamento;


            DateTime previsaoFinalEntrega = dataBase.AddMinutes(carga.Rota.TempoDeViagemEmMinutos);
            ultimaCargaPedido.Pedido.PrevisaoEntrega = previsaoFinalEntrega;
            repPedido.Atualizar(ultimaCargaPedido.Pedido);


            int TotalMinutos = 0;

            foreach (var cargaPedido in listaCargasPedido)
            {
                Dominio.Entidades.RotaFrete rota = ObterRotaPorOrigemDestino(cargaPedido?.ClienteColeta?.Localidade, cargaPedido?.ClienteEntrega?.Localidade, unitOfWork);

                if (rota == null)
                    return;

                TotalMinutos = rota.TempoDeViagemEmMinutos;
                DateTime novaDataPrevisao = previsaoFinalEntrega.AddMinutes(-TotalMinutos);

                if (novaDataPrevisao <= dataBase)
                    break;

                cargaPedido.Pedido.PrevisaoEntrega = novaDataPrevisao;
                repPedido.Atualizar(cargaPedido.Pedido);

            }
        }

        public static void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaAtual.Codigo);
            if (cargaRotaFrete != null)
            {
                cargaRotaFrete.Carga = cargaNova;
                repCargaRotaFrete.Atualizar(cargaRotaFrete);
            }
        }

        public static bool ValidarCoordenadas(string coordenada)
        {
            if (string.IsNullOrWhiteSpace(coordenada))
                return false;

            double.TryParse(coordenada.Replace(".", ","), out double cord);

            return cord != 0;
        }

        #endregion
    }
}
