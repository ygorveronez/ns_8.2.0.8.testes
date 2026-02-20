using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Localidades
{
    public class Geo
    {
        /// <summary>
        /// Este serviço recebe uma posição geográfica e um raio de proximidade, analisa todos os CEPs dentro do raio informado 
        /// e tenta geocodificar no servico Nominatim, e em caso de sucesso, atualiza a tabela cepbr_geo nas colunas 
        /// latitude_nominatim e longitude_nominatim.
        /// </summary>
        /// <param name="latitudeOrigem">Latitude de origem a ser analisada.</param>
        /// <param name="longitudeOrigem">Longitude de origem a ser analisada.</param>
        /// <param name="raioKm">Raio de distância a ser analizado.</param>
        /// <param name="configuracaoIntegracao">Configuração embarcador para o seriço Nominatim.</param>
        /// <param name="unitOfWorkAdmin">Conexão com o admim multisoftware.</param>
        /// <param name="atualizarSeExistir">Atualiza a posição nominatim se já existir no Bando de Dados.</param>
        public void AtualizarPosicaoGeograficaCepNominatim(decimal latitudeOrigem, decimal longitudeOrigem, decimal raioKm, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, bool atualizarSeExistir = true)
        {
            //Servicos.Embarcador.Localidades.Geo servicoGeo = new Servicos.Embarcador.Localidades.Geo();
            //servicoGeo.AtualizarPosicaoGeograficaCepNominatim((decimal)-27.4529057, (decimal)-48.4556768, 30, null, unitOfWorkAdmin, false);
            try
            {
                this.GerarLog($"1 - Iniciou {latitudeOrigem} {longitudeOrigem}.");
                AdminMultisoftware.Repositorio.Localidades.Geo repositorioGeo = new AdminMultisoftware.Repositorio.Localidades.Geo(unitOfWorkAdmin);

                List<AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo> listaCepsProximos = new List<AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo>();

                listaCepsProximos = repositorioGeo.BuscarProximosRaio(latitudeOrigem, longitudeOrigem, raioKm);

                if (!atualizarSeExistir)
                    listaCepsProximos.RemoveAll(x => x.latitude_nominatim != 0 && x.longitude_nominatim != 0);

                Servicos.Embarcador.Logistica.Nominatim.Service serviceNominatim = Servicos.Embarcador.Carga.CargaRotaFrete.ObterServiceNominatim(configuracaoIntegracao);

                this.GerarLog($"2 - Iniciando geocodificação de {listaCepsProximos.Count} ceps.");

                for (int i = 0; i < listaCepsProximos.Count; i++)
                {
                    try
                    {
                        string enderecoGeocodificarSemCep = string.Empty;
                        string enderecoGeocodificar = this.ObterEnderecoGeocodingNominatim(listaCepsProximos[i], ref enderecoGeocodificarSemCep, true);
                        Servicos.Embarcador.Logistica.Nominatim.RootObject geocoding = serviceNominatim.GeocodingQueryParameters(enderecoGeocodificar);
                        if (geocoding == null)
                            geocoding = serviceNominatim.GeocodingQueryParameters(enderecoGeocodificarSemCep);

                        if (geocoding != null)
                        {
                            listaCepsProximos[i].latitude_nominatim = geocoding.lat.ToDecimal();
                            listaCepsProximos[i].longitude_nominatim = geocoding.lon.ToDecimal();
                            listaCepsProximos[i].display_name_nominatim = geocoding.display_name;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar geocodificação de CEP próximo: {ex.ToString()}", "CatchNoAction");
                    }
                }

                listaCepsProximos.RemoveAll(x => x.cep == 0 || x.latitude_nominatim == 0 || x.longitude_nominatim == 0);

                this.GerarLog($"4 - Finalizou geocodificação de {listaCepsProximos.Count} ceps encontrados.");

                repositorioGeo.AtualizarPosicaoGeoNomitatim(listaCepsProximos);

                this.GerarLog($"5 - Finalizou geocodificação de {listaCepsProximos.Count} ceps encontrados.");
            }
            catch (Exception ex)
            {
                this.GerarLog($"6 - Erro em {latitudeOrigem} {longitudeOrigem}." + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString());
            }
        }

        public void AtualizarPosicaoGeograficaCepNominatim(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, bool atualizarSeExistir = true)
        {
            //Servicos.Embarcador.Localidades.Geo servicoGeo = new Servicos.Embarcador.Localidades.Geo();
            //servicoGeo.AtualizarPosicaoGeograficaCepNominatim(null, unitOfWorkAdmin, false);
            try
            {
                AdminMultisoftware.Repositorio.Localidades.Geo repositorioGeo = new AdminMultisoftware.Repositorio.Localidades.Geo(unitOfWorkAdmin);

                int qtdeCeps = repositorioGeo.Contar(false);
                int qtdePorPagina = 2000;
                int paginaAtual = 0;
                int totalPaginas = (int)Math.Ceiling((decimal)qtdeCeps / (decimal)qtdePorPagina);

                while (paginaAtual < totalPaginas)
                {
                    try
                    {
                        List<AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo> listaCeps = repositorioGeo.BuscarPaginado(paginaAtual * qtdePorPagina, qtdePorPagina, false);

                        this.GerarLog($"1 - Iniciou página {(paginaAtual + 1)} - {totalPaginas} de um total de {qtdeCeps} registros.");

                        if (!atualizarSeExistir)
                            listaCeps.RemoveAll(x => x.latitude_nominatim != 0 && x.longitude_nominatim != 0);

                        Servicos.Embarcador.Logistica.Nominatim.Service serviceNominatim = Servicos.Embarcador.Carga.CargaRotaFrete.ObterServiceNominatim(configuracaoIntegracao);

                        for (int i = 0; i < listaCeps.Count; i++)
                        {
                            try
                            {
                                string enderecoGeocodificarSemCep = string.Empty;
                                string enderecoGeocodificar = this.ObterEnderecoGeocodingNominatim(listaCeps[i], ref enderecoGeocodificarSemCep, true);
                                Servicos.Embarcador.Logistica.Nominatim.RootObject geocoding = serviceNominatim.GeocodingQueryParameters(enderecoGeocodificar, false);
                                if (geocoding == null)
                                    geocoding = serviceNominatim.GeocodingQueryParameters(enderecoGeocodificarSemCep);

                                if (geocoding != null)
                                {
                                    listaCeps[i].latitude_nominatim = geocoding.lat.ToDecimal();
                                    listaCeps[i].longitude_nominatim = geocoding.lon.ToDecimal();
                                    listaCeps[i].display_name_nominatim = geocoding.display_name;
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar geocodificação de CEP: {ex.ToString()}", "CatchNoAction");
                            }
                        }

                        listaCeps.RemoveAll(x => x.cep == 0 || x.latitude_nominatim == 0 || x.longitude_nominatim == 0);

                        repositorioGeo.AtualizarPosicaoGeoNomitatim(listaCeps);

                        this.GerarLog($"2 - Finalizou geocodificação de {listaCeps.Count} ceps encontrados.");
                    }
                    catch (Exception exi)
                    {
                        this.GerarLog($"3 - Erro página {(paginaAtual + 1)} - {totalPaginas}." + Environment.NewLine + exi.ToString());
                    }
                    finally
                    {
                        paginaAtual += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.GerarLog($"6 - Ocorreu um erro." + Environment.NewLine + ex.ToString());
            }
        }

        private void GerarLog(string msg)
        {
            Servicos.Log.TratarErro(msg, "AtualizarPosicaoGeograficaCepNominatim");
        }

        #region Métodos Privados

        private string ObterEnderecoGeocodingNominatim(AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo cepGeoEndereco, ref string enderecoGeocodificarSemCep, bool queryStringFormat)
        {
            // Ex: rua rio grande do sul, 355, chapecó, sc, brasil, 89815-435
            string endereco = "";

            if (!queryStringFormat)
            {
                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.tipo_logradouro))
                    endereco += cepGeoEndereco.tipo_logradouro;

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.logradouro))
                    endereco += (!string.IsNullOrWhiteSpace(endereco) ? " " : "") + cepGeoEndereco.logradouro + ", ";

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.cidade))
                    endereco += cepGeoEndereco.cidade + ", ";

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.uf))
                    endereco += cepGeoEndereco.uf + ", ";

                string postalCode = cepGeoEndereco.cep.ToString().Replace(",", "").Replace(".", "").Replace("-", "").Replace(" ", "");
                postalCode = postalCode.Insert(5, "-");

                endereco += "Brasil, " + postalCode;

                enderecoGeocodificarSemCep = endereco.Replace(", " + postalCode, "");

            }
            else
            {
                /*street=<housenumber> <streetname>
                city=<city>
                county=<county>
                state=<state>
                country=<country>
                postalcode=<postalcode> */

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.tipo_logradouro))
                    endereco += "street=" + cepGeoEndereco.tipo_logradouro;

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.logradouro))
                    endereco += (!string.IsNullOrWhiteSpace(endereco) ? " " : "street=") + cepGeoEndereco.logradouro;

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.cidade))
                    endereco += (!string.IsNullOrWhiteSpace(endereco) ? "&" : "") + "city=" + cepGeoEndereco.cidade;

                if (!string.IsNullOrWhiteSpace(cepGeoEndereco.uf))
                    endereco += (!string.IsNullOrWhiteSpace(endereco) ? "&" : "") + "state=" + cepGeoEndereco.uf;

                endereco += "&country=Brasil";

                string postalCode = cepGeoEndereco.cep.ToString().Replace(",", "").Replace(".", "").Replace("-", "").Replace(" ", "");
                postalCode = postalCode.Insert(5, "-");

                endereco += "&postalcode=" + postalCode;

                enderecoGeocodificarSemCep = endereco.Replace("&postalcode=" + postalCode, "");

            }

            return endereco.Trim();
        }

        #endregion
    }
}

