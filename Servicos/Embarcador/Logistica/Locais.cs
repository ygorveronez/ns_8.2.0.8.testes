using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public class Locais : ServicoBase
    {
        #region Contrutores
        public Locais(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false };
            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true };
            return retorno;
        }
        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarLocais(string dados, (string Nome, string Guid) arquivoGerador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };
            int contador = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> locais = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais>();

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarLocaisLinha(linhas[i], arquivoGerador, locais, i, unitOfWork);
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    if (retornoLinha.processou)
                    {
                        unitOfWork.CommitChanges();
                        contador++;
                    }
                    else
                        unitOfWork.Rollback();
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            if (locais.Any())
            {
                try
                {
                    if (GeraPoligonos(locais, retornoImportacao, unitOfWork))
                    {
                        unitOfWork.CommitChanges();
                    }

                    unitOfWork.Rollback();
                }
                catch (Exception exception)
                {
                    unitOfWork.Rollback();
                    Log.TratarErro(exception);
                }
            }

            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        private bool GeraPoligonos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> locais, Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);
            StringBuilder paths = new StringBuilder();
            bool erro = false;
            foreach (IGrouping<string, Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> groupLocais in locais.GroupBy(x => x.Descricao))
            {
                ObterLongetudeLatitude(paths, repLocalidades, groupLocais, retornoImportacao, out erro);

                if (!string.IsNullOrEmpty(paths.ToString()) && !erro)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais local = groupLocais.FirstOrDefault();

                    string area = "[{\"type\":\"polygon\",\"zIndex\":1000000,\"fillColor\":\"#1E90FF\",\"fillOpacity\":0.45,\"paths\":[" + paths.ToString().TrimEnd(',') + "],\"strokeWeight\":0,\"strokeColor\":null,\"strokeOpacity\":null,\"content\":null}]";

                    AdicionarLocais(local.Descricao, local.Tipo, local.TipoArea, local.Observacao, area, local.CNPJFilial, unitOfWork);
                }
                paths.Clear();
            }
            return !erro;
        }

        private void ObterLongetudeLatitude(StringBuilder paths, Repositorio.Localidade repLocalidades, IGrouping<string, Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> groupLocais, Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, out bool erro)
        {
            erro = false;
            List<WayPoint> coordenadas = new List<WayPoint>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais item in groupLocais)
            {
                Dominio.Entidades.Localidade localidade;
                decimal? latitude = 0;
                decimal? longitude = 0;

                if (string.IsNullOrWhiteSpace(item.Latitude) || string.IsNullOrWhiteSpace(item.Longitude))
                    if (item.CodigoIBGE > 0)
                    {
                        localidade = repLocalidades.BuscarPorCodigoIBGE(item.CodigoIBGE);
                        if (localidade == null)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Municipio) && !string.IsNullOrWhiteSpace(item.UF))
                            {
                                localidade = repLocalidades.BuscarPorCidadeUF(item.Municipio, item.UF);
                                if (localidade != null)
                                {
                                    latitude = localidade.Latitude;
                                    longitude = localidade.Longitude;
                                }
                            }
                            else
                            {
                                GeraErroImportacao(groupLocais.ToList(), retornoImportacao, "O campo UF e Municipio são obrigatorios quando Tipo da Área é Poligono e o código do IBGE não é passado.");
                                erro = true;
                                break;
                            }
                        }
                        else
                        {
                            latitude = localidade.Latitude;
                            longitude = localidade.Longitude;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(item.Municipio) && !string.IsNullOrWhiteSpace(item.UF))
                        {
                            localidade = repLocalidades.BuscarPorCidadeUF(item.Municipio, item.UF);
                            if (localidade != null)
                            {
                                latitude = localidade.Latitude;
                                longitude = localidade.Longitude;
                            }
                        }
                        else
                        {
                            GeraErroImportacao(groupLocais.ToList(), retornoImportacao, "O campo UF e Municipio são obrigatorios quando Tipo da Área é Poligono e o código do IBGE não é passado.");
                            erro = true;
                            break;
                        }
                    }
                else
                {
                    coordenadas.Add(new WayPoint(item.Latitude.ToDouble(), item.Longitude.ToDouble()));
                    continue;
                }

                if (latitude != 0 && longitude != 0)
                {
                    coordenadas.Add(new WayPoint((double)latitude, (double)longitude));
                }
                else
                {
                    GeraErroImportacao(groupLocais.ToList(), retornoImportacao);
                    erro = true;
                    break;
                }

            }

            List<WayPoint> coordenadasOrdenadas = FiltrarPontosDeCanto(CalcularCascoConvexo(coordenadas), 150);


            List<double> latitudesOrdenadas = coordenadasOrdenadas.Select(c => c.Latitude).ToList();
            List<double> longitudesOrdenadas = coordenadasOrdenadas.Select(c => c.Longitude).ToList();

            for (int i = 0; i < latitudesOrdenadas.Count(); i++)
            {
                paths.Append("{\"lat\":" + latitudesOrdenadas[i].ToString().Replace(',', '.') + ",\"lng\":" + longitudesOrdenadas[i].ToString().Replace(',', '.') + "},");
            }

        }

        private double ProdutoCruzado(WayPoint O, WayPoint A, WayPoint B)
        {
            return (A.Longitude - O.Longitude) * (B.Latitude - O.Latitude) - (A.Latitude - O.Latitude) * (B.Longitude - O.Longitude);
        }

        private WayPoint EncontrarPontoMaisBaixo(List<WayPoint> points)
        {
            WayPoint maisBaixo = points[0];
            foreach (WayPoint p in points)
            {
                if (p.Latitude < maisBaixo.Latitude || (p.Latitude == maisBaixo.Latitude && p.Longitude < maisBaixo.Longitude))
                {
                    maisBaixo = p;
                }
            }
            return maisBaixo;
        }

        private List<WayPoint> CalcularCascoConvexo(List<WayPoint> points)
        {
            if (points.Count < 3)
                throw new Exception("São necessários pelo menos 3 pontos para calcular um poligno.");

            WayPoint maisBaixo = EncontrarPontoMaisBaixo(points);
            points.Remove(maisBaixo);

            points.Sort((a, b) =>
            {
                double angleA = Math.Atan2(a.Latitude - maisBaixo.Latitude, a.Longitude - maisBaixo.Longitude);
                double angleB = Math.Atan2(b.Latitude - maisBaixo.Latitude, b.Longitude - maisBaixo.Longitude);
                return angleA.CompareTo(angleB);
            });

            Stack<WayPoint> cascoConvexo = new Stack<WayPoint>();
            cascoConvexo.Push(maisBaixo);
            cascoConvexo.Push(points[0]);
            cascoConvexo.Push(points[1]);

            for (int i = 2; i < points.Count; i++)
            {
                WayPoint top = cascoConvexo.Pop();
                while (ProdutoCruzado(cascoConvexo.Peek(), top, points[i]) <= 0)
                {
                    top = cascoConvexo.Pop();
                }
                cascoConvexo.Push(top);
                cascoConvexo.Push(points[i]);
            }

            return cascoConvexo.ToList();
        }

        private List<WayPoint> FiltrarPontosDeCanto(List<WayPoint> hull, double anguloLimite)
        {
            List<WayPoint> cantos = new List<WayPoint>();

            for (int i = 0; i < hull.Count; i++)
            {
                WayPoint atual = hull[i];
                WayPoint proximo = hull[(i + 1) % hull.Count];
                WayPoint anterior = hull[(i - 1 + hull.Count) % hull.Count];

                double angulo = CalcularAngulo(anterior, atual, proximo);

                if (angulo < anguloLimite)
                {
                    cantos.Add(atual);
                }
            }

            return cantos;
        }

        private double CalcularAngulo(WayPoint a, WayPoint b, WayPoint c)
        {
            var ab = new WayPoint(b.Latitude - a.Latitude, b.Longitude - a.Longitude);
            var bc = new WayPoint(c.Latitude - b.Latitude, c.Longitude - b.Longitude);

            var produtoEscalar = ab.Latitude * bc.Latitude + ab.Longitude * bc.Longitude;
            var magnitudes = Math.Sqrt(ab.Latitude * ab.Latitude + ab.Longitude * ab.Longitude) *
                             Math.Sqrt(bc.Latitude * bc.Latitude + bc.Longitude * bc.Longitude);

            var argumento = produtoEscalar / magnitudes;
            argumento = Math.Max(-1.0, Math.Min(1.0, argumento));

            var angulo = Math.Acos(argumento);

            return angulo * (180 / Math.PI);
        }

        private void GeraErroImportacao(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> locais, Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, string menssagem = null)
        {
            foreach (var item in locais)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = retornoImportacao.Retornolinhas.Where(x => x.indice == item.Indice).FirstOrDefault();
                retornoLinha.processou = false;
                retornoLinha.mensagemFalha = menssagem ?? "Ocorreu uma falha ao processar o agrupamento";
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarLocaisLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais> locais, int indice, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaLocais = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                string descricao = "";
                TipoLocal tipoLocal = TipoLocal.AreaDeRisco;
                TipoArea tipoArea = TipoArea.Ponto;
                string observacao = "";
                string latitude = "";
                string longitude = "";
                string CNPJFilial = "";
                string UF = "";
                int codigoIBGE = 0;
                string municipio = "";
                var area = "[{\"type\":\"marker\",\"position\":{\"lat\":LATITUDE,\"lng\":LONGITUDE},\"title\":null,\"content\":null,\"draggable\":false,\"animation\":null}]";


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                if (colDescricao != null && !string.IsNullOrEmpty(colDescricao.Valor))
                    descricao = colDescricao.Valor;
                else
                    return RetornarFalhaLinha("A Descrição precisa conter um valor");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoLocal = (from obj in linha.Colunas where obj.NomeCampo == "TipoLocal" select obj).FirstOrDefault();
                if (colTipoLocal != null && !string.IsNullOrEmpty(colTipoLocal.Valor))
                    tipoLocal = ((string)colTipoLocal.Valor).ToEnum(TipoLocal.PontoDeApoio);
                else
                    return RetornarFalhaLinha("O Tipo do Local precisa ser definido");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                if (colObservacao != null && !string.IsNullOrEmpty(colObservacao.Valor))
                    observacao = colObservacao.Valor;
                else
                    return RetornarFalhaLinha("A Observação precisa conter um valor");


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoArea = (from obj in linha.Colunas where obj.NomeCampo == "TipoArea" select obj).FirstOrDefault();
                if (colTipoLocal != null && !string.IsNullOrEmpty(colTipoLocal.Valor))
                    tipoArea = ((string)colTipoArea.Valor).ToEnum(TipoArea.Ponto);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLatitude = (from obj in linha.Colunas where obj.NomeCampo == "Latitude" select obj).FirstOrDefault();
                if (colLatitude != null && !string.IsNullOrEmpty(colLatitude.Valor))
                {
                    latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(colLatitude.Valor.Replace(",", "."));
                    if (string.IsNullOrEmpty(latitude))
                        return RetornarFalhaLinha("A latitude está em um formato inválido");
                }
                else if (tipoArea != TipoArea.Poligono)
                    return RetornarFalhaLinha("A Latitude não pode estar vazia");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLongitude = (from obj in linha.Colunas where obj.NomeCampo == "Longitude" select obj).FirstOrDefault();
                if (colLongitude != null && !string.IsNullOrEmpty(colLongitude.Valor))
                {
                    longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(colLongitude.Valor.Replace(",", "."));
                    if (string.IsNullOrEmpty(longitude))
                        return RetornarFalhaLinha("A Longitude está em um formato inválido");
                }
                else if (tipoArea != TipoArea.Poligono)
                    return RetornarFalhaLinha("A Longitude não pode estar vazia");



                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJFILIAL = (from obj in linha.Colunas where obj.NomeCampo == "CNPJFilial" select obj).FirstOrDefault();
                if (colCNPJFILIAL != null && !string.IsNullOrEmpty(colCNPJFILIAL.Valor))
                {
                    CNPJFilial = Utilidades.String.OnlyNumbers(colCNPJFILIAL.Valor);
                    if (string.IsNullOrEmpty(CNPJFilial))
                        return RetornarFalhaLinha("O CNPJ Filial está em um formato inválido");
                }



                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodIBGE = (from obj in linha.Colunas where obj.NomeCampo == "CodIBGE" select obj).FirstOrDefault();
                if (colCodIBGE != null && !string.IsNullOrEmpty(colCodIBGE.Valor))
                {
                    codigoIBGE = Convert.ToInt32(colCodIBGE.Valor);
                    if (codigoIBGE < 0)
                        return RetornarFalhaLinha("O campo código IBGE tem que ser maior que 0");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMunicipio = (from obj in linha.Colunas where obj.NomeCampo == "Municipio" select obj).FirstOrDefault();
                if (colMunicipio != null && !string.IsNullOrEmpty(colMunicipio.Valor))
                {
                    municipio = (string)colMunicipio.Valor;
                    if (string.IsNullOrEmpty(municipio))
                        return RetornarFalhaLinha("O Município está em um formato inválido");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUF = (from obj in linha.Colunas where obj.NomeCampo == "UF" select obj).FirstOrDefault();
                if (colUF != null && !string.IsNullOrEmpty(colUF.Valor))
                {
                    UF = (string)colUF.Valor;
                    if (string.IsNullOrEmpty(UF))
                        return RetornarFalhaLinha("O campo UF está em um formato inválido");
                }

                if (tipoArea == TipoArea.Raio)
                {
                    area = "[{\"type\":\"circle\",\"zIndex\":1,\"fillColor\":\"#1E90FF\",\"fillOpacity\":0.45,\"radius\":17024.321258895805,\"center\":{\"lat\":LATITUDE,\"lng\":LONGITUDE},\"strokeWeight\":0,\"strokeColor\":null,\"strokeOpacity\":null,\"content\":null}]";
                }

                area = area.Replace("LATITUDE", latitude);
                area = area.Replace("LONGITUDE", longitude);

                if (tipoArea == TipoArea.Poligono)
                {
                    locais.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao.Locais()
                    {
                        TipoArea = tipoArea,
                        CodigoIBGE = codigoIBGE,
                        Descricao = descricao,
                        CNPJFilial = CNPJFilial,
                        Municipio = municipio,
                        Observacao = observacao,
                        Tipo = tipoLocal,
                        UF = UF,
                        Indice = indice,
                        Latitude = latitude,
                        Longitude = longitude,
                    });
                    return RetornarSucessoLinha(indice);
                }
                else
                {
                    retornoLinhaLocais = AdicionarLocais(descricao, tipoLocal, tipoArea, observacao, area, CNPJFilial, unitOfWork);
                }


                if (!string.IsNullOrWhiteSpace(retornoLinhaLocais.mensagemFalha))
                    return RetornarFalhaLinha(retornoLinhaLocais.mensagemFalha);

            }
            catch (Exception ex2)
            {
                Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + ex2.Message + ").");
            }

            return RetornarSucessoLinha(indice);

        }
        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha AdicionarLocais(string descricao, TipoLocal tipoLocal, TipoArea tipoArea, string observacao, string area, string CNPJFilial, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();



            Dominio.Entidades.Embarcador.Logistica.Locais locais = new Dominio.Entidades.Embarcador.Logistica.Locais()
            {
                Descricao = descricao,
                Tipo = tipoLocal,
                TipoArea = tipoArea,
                Observacao = observacao,
                Area = area,
            };

            if (!String.IsNullOrEmpty(CNPJFilial))
            {
                Dominio.Entidades.Embarcador.Filiais.Filial Filial = repFilial.BuscarPorCNPJ(CNPJFilial);
                if (Filial == null)
                {
                    return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                    {
                        codigo = 0,
                        mensagemFalha = "Local não inserido, favor valide os CNPJS Filiais informados"
                    };
                }
                locais.Filial = Filial;
            }

            repLocais.Inserir(locais, auditado, null, "Adicionado via importação de planilha");

            if (locais != null && locais.Codigo > 0)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                {
                    codigo = locais.Codigo,
                    mensagemFalha = retorno
                };
            }
            else
            {
                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                {
                    codigo = 0,
                    mensagemFalha = "Local não inserido, favor valide os dados informados"
                };
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoLocais(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Tipo Local (Área de risco = 1, Pernoite = 2, Micro Região Roteiriação = 3, Ponto de apoio = 4, Mudança = 5, Zona Exclusão Rota = 6)", Propriedade = "TipoLocal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Latitude", Propriedade = "Latitude", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Longitude", Propriedade = "Longitude", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "CNPJ Filial", Propriedade = "CNPJFilial" },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Município", Propriedade = "Municipio" },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "UF", Propriedade = "UF", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Cod_IBGE", Propriedade = "CodIBGE", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Tipo Area (Raio = 1, Polígono = 2, Ponto = 3)", Propriedade = "TipoArea", Tamanho = 200  },
            };

            return configuracoes;
        }

    }
}
