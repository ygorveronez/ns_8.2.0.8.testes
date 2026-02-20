using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.Infolog
{
    public class IntegracaoInfolog
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public IntegracaoInfolog(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracaoInfolog ?? false) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoInfolog))
                throw new ServicoException("Não existe configuração de integração disponível para a Infolog.");

            return configuracaoIntegracao;
        }

        private ServicoInfolog.planBean ObterPlanoViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            ServicoInfolog.planBean planoViagem = new ServicoInfolog.planBean();

            planoViagem.Cabecalho = ObterPlanoViagemCabecalho(configuracaoIntegracao);
            planoViagem.Motoristas = ObterPlanoViagemMotoristas(carga);
            planoViagem.PlanoStatus = ObterPlanoViagemStatus(carga);
            planoViagem.PlanoValores = ObterPlanoViagemValores(carga);
            planoViagem.Transportadora = ObterPlanoViagemTransportadora(carga);
            planoViagem.Veiculos = ObterPlanoViagemVeiculos(carga);

            return planoViagem;
        }

        private ServicoInfolog.planBean ObterPlanoViagemCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            ServicoInfolog.planBean planoViagem = new ServicoInfolog.planBean();

            planoViagem.Cabecalho = ObterPlanoViagemCabecalhoCancelamento(cancelamentoCargaIntegracao, configuracaoIntegracao);

            return planoViagem;
        }

        private ServicoInfolog.criarPlanoViagem ObterPlanoViagemAdicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            ServicoInfolog.criarPlanoViagem planoViagemAdicionar = new ServicoInfolog.criarPlanoViagem();

            planoViagemAdicionar.Plano = ObterPlanoViagem(carga, configuracaoIntegracao);
            planoViagemAdicionar.Itinerario = ObterPlanoViagemItinerario(carga);
            planoViagemAdicionar.Usuario = ObterPlanoViagemUsuario(configuracaoIntegracao);

            return planoViagemAdicionar;
        }

        private ServicoInfolog.criarPlanoViagem ObterPlanoViagemCancelar(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            ServicoInfolog.criarPlanoViagem planoViagemCancelar = new ServicoInfolog.criarPlanoViagem();

            planoViagemCancelar.Plano = ObterPlanoViagemCancelamento(cancelamentoCargaIntegracao, configuracaoIntegracao);
            planoViagemCancelar.Usuario = ObterPlanoViagemUsuario(configuracaoIntegracao);

            return planoViagemCancelar;
        }

        private ServicoInfolog.planHeaderBean ObterPlanoViagemCabecalho(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new ServicoInfolog.planHeaderBean()
            {
                CodigoOperacao = !string.IsNullOrWhiteSpace(configuracaoIntegracao?.CodigoOperacaoInfolog) ? configuracaoIntegracao.CodigoOperacaoInfolog : "385",
                NomeProcesso = "INSERT"
            };
        }

        private ServicoInfolog.planHeaderBean ObterPlanoViagemCabecalhoCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            string protocoloIntegracaoCarga = ObterProtocoloIntegracaoCarga(cancelamentoCargaIntegracao);

            return new ServicoInfolog.planHeaderBean()
            {
                CodigoOperacao = !string.IsNullOrWhiteSpace(configuracaoIntegracao?.CodigoOperacaoInfolog) ? configuracaoIntegracao.CodigoOperacaoInfolog : "385",
                IdentificadorPlano = protocoloIntegracaoCarga,
                NomeProcesso = "DELETE"
            };
        }

        private ServicoInfolog.pointBean ObterPlanoViagemDestino(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres, int ordem, List<int> pedidosEnviados)
        {
            Dominio.Entidades.Cliente cliente = entrega.Cliente;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = entrega.Pedidos?.Where(o => !pedidosEnviados.Contains(o.CargaPedido.Pedido.Codigo)).Select(o => o.CargaPedido.Pedido)?.FirstOrDefault();
            if (pedido == null)
                pedido = entrega.Pedidos?.Select(o => o.CargaPedido.Pedido)?.FirstOrDefault();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from o in cargaCTes where o.CTe.Destinatario.Cliente.CPF_CNPJ == (entrega.Cliente?.CPF_CNPJ ?? 0d) select o.CTe).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacres = (from o in cargaLacres where o.TipoLacre != null && !string.IsNullOrWhiteSpace(o.TipoLacre.CodigoIntegracao) && o.Cliente.CPF_CNPJ == (entrega.Cliente?.CPF_CNPJ ?? 0d) select o).ToList();
            var lacresPorTipo = lacres.GroupBy(obj => obj.TipoLacre.CodigoIntegracao).Select(obj => new { CodigoIntegracao = obj.Key, Numero = string.Join("-", obj.Select(o => o.Numero)) }).ToList();

            bool enderecoSecundario = ((pedido?.UsarOutroEnderecoDestino ?? false) && pedido?.EnderecoDestino != null);
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoPedido = enderecoSecundario ? pedido.EnderecoDestino : null;

            ServicoInfolog.pointBean planoViagemDestino = new ServicoInfolog.pointBean()
            {
                Carga = "FALSE",
                Descarga = "TRUE",
                Entidade = new ServicoInfolog.relationalBean()
                {
                    Documento = new ServicoInfolog.documentoBean()
                    {
                        Numero = cliente?.CPF_CNPJ_SemFormato,
                        Tipo = enderecoSecundario ? "3" : (cliente?.Tipo == "F" ? "2" : "1")
                    },
                    Cidade = enderecoSecundario ? enderecoPedido.Localidade?.Descricao?.Left(50) ?? string.Empty : cliente?.Localidade?.Descricao.Left(50) ?? "",
                    Estado = enderecoSecundario ? enderecoPedido.Localidade?.Estado?.Sigla ?? string.Empty : cliente?.Localidade?.Estado?.Sigla ?? "",
                    Ibge = enderecoSecundario ? enderecoPedido.Localidade?.CodigoIBGE.ToString() ?? string.Empty : cliente?.Localidade?.CodigoIBGE.ToString() ?? "",
                    Logradouro = new ServicoInfolog.logradouroBean()
                    {
                        Bairro = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(enderecoSecundario ? enderecoPedido.Bairro?.Left(30) ?? string.Empty : cliente?.Bairro.Left(30))) ?? "",
                        Cep = enderecoSecundario ? enderecoPedido.CEP?.ObterSomenteNumeros() ?? string.Empty : cliente?.CEP.ObterSomenteNumeros() ?? "",
                        Complemento = enderecoSecundario ? enderecoPedido.Complemento?.Left(20) ?? string.Empty : cliente?.Complemento.Left(20) ?? "",
                        Endereco = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(enderecoSecundario ? enderecoPedido.Endereco?.Left(50) ?? string.Empty : cliente?.Endereco.Left(50))) ?? "",
                        Numero = enderecoSecundario ? enderecoPedido.Numero?.Left(10) ?? string.Empty : cliente?.Numero.Left(10) ?? ""
                    },
                    Nome = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(cliente?.Nome.Left(30))) ?? "",
                    NomeFantasia = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(cliente?.NomeFantasia.Left(30))) ?? "",
                    Pais = cliente?.Localidade?.Pais?.Nome.Left(20) ?? ""
                },
                Ordem = ordem,
                OrdemSpecified = true,
                PrevisaoEventos = new ServicoInfolog.previsaoEventosRoteiroBean()
                {
                    Ch = (pedido?.PrevisaoEntrega ?? entrega.DataPrevista)?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Fv = (pedido?.PrevisaoEntrega ?? entrega.DataPrevista)?.AddHours(2).ToString("dd/MM/yyyy HH:mm") ?? ""
                }
            };

            int totalDocumentosNotasFiscais = 0;
            int volumesSemClassificacao = 0;
            int volumesRevenda = 0;
            int volumesNaoRevenda = 0;

            if (entrega.NotasFiscais?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal notaFiscal in entrega.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal;

                    if (!xmlNotaFiscal.nfAtiva)
                        continue;

                    if (xmlNotaFiscal.Volumes > 0)
                    {
                        if (!xmlNotaFiscal.ClassificacaoNFe.HasValue || xmlNotaFiscal.ClassificacaoNFe == ClassificacaoNFe.SemClassificacao)
                            volumesSemClassificacao += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.Revenda)
                            volumesRevenda += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.NaoRevenda)
                            volumesNaoRevenda += xmlNotaFiscal.Volumes;
                    }
                }

                if (volumesSemClassificacao > 0)
                    totalDocumentosNotasFiscais++;

                if (volumesRevenda > 0)
                    totalDocumentosNotasFiscais++;

                if (volumesNaoRevenda > 0)
                    totalDocumentosNotasFiscais++;
            }

            int totalDocumentos = ctes.Count + lacresPorTipo.Count + totalDocumentosNotasFiscais;

            if (totalDocumentos > 0)
            {
                planoViagemDestino.Documentos = new ServicoInfolog.fiscalDocumentBean[totalDocumentos];
                int contadorDocumentos = 0;

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    planoViagemDestino.Documentos[contadorDocumentos] = new ServicoInfolog.fiscalDocumentBean()
                    {
                        Codigo = cte.Numero.ToString(),
                        Tipo = "5"
                    };

                    contadorDocumentos++;
                }

                foreach (var lacre in lacresPorTipo)
                {
                    planoViagemDestino.Documentos[contadorDocumentos] = new ServicoInfolog.fiscalDocumentBean()
                    {
                        Codigo = lacre.Numero,
                        Tipo = lacre.CodigoIntegracao
                    };

                    contadorDocumentos++;
                }

                if (volumesSemClassificacao > 0)
                {
                    planoViagemDestino.Documentos[contadorDocumentos] = new ServicoInfolog.fiscalDocumentBean()
                    {
                        Codigo = volumesSemClassificacao.ToString("n0"),
                        Tipo = "28"
                    };

                    contadorDocumentos++;
                }

                if (volumesRevenda > 0)
                {
                    planoViagemDestino.Documentos[contadorDocumentos] = new ServicoInfolog.fiscalDocumentBean()
                    {
                        Codigo = volumesRevenda.ToString("n0"),
                        Tipo = "30"
                    };

                    contadorDocumentos++;
                }

                if (volumesNaoRevenda > 0)
                {
                    planoViagemDestino.Documentos[contadorDocumentos] = new ServicoInfolog.fiscalDocumentBean()
                    {
                        Codigo = volumesNaoRevenda.ToString("n0"),
                        Tipo = "31"
                    };

                    contadorDocumentos++;
                }
            }

            if (!string.IsNullOrWhiteSpace(entrega.Carga.TipoDeCarga?.IdentificacaoMercadoriaInfolog))
            {

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                decimal valorTotalMercadoriaPorDestinatario = repositorioPedidoXMLNotaFiscal.ObterValorMercadoriaPorCargaEDestinatario(entrega.Carga.Codigo, entrega.Cliente?.CPF_CNPJ ?? 0d);

                if (valorTotalMercadoriaPorDestinatario == 0)
                    valorTotalMercadoriaPorDestinatario = entrega?.Pedidos?.Sum(obj => obj.CargaPedido.Pedido.ValorTotalNotasFiscais) ?? 0;


                bool tipoFreteSpot = (
                (entrega.Carga.TabelaFrete == null) ||
                (entrega.Carga.TabelaFrete.TipoFreteTabelaFrete == TipoFreteTabelaFrete.NaoInformado) ||
                (entrega.Carga.TabelaFrete.TipoFreteTabelaFrete == TipoFreteTabelaFrete.Spot)
            );

                planoViagemDestino.GrupoNormaMercadorias = new ServicoInfolog.normaBean[1];
                planoViagemDestino.GrupoNormaMercadorias[0] = new ServicoInfolog.normaBean()
                {
                    CodigoMercadoria = entrega.Carga.TipoDeCarga.IdentificacaoMercadoriaInfolog,
                    GrupoNorma = tipoFreteSpot ? "182" : "168",
                    ValorMercadoria = valorTotalMercadoriaPorDestinatario.ToString("F2", CultureInfo.CreateSpecificCulture("pt-BR"))
                };
            }

            return planoViagemDestino;
        }

        private ServicoInfolog.pointBean[] ObterPlanoViagemDestinos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> coletas)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo, false, false, true);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(carga.Codigo);

            List<ServicoInfolog.pointBean> planoViagemDestinos = new List<ServicoInfolog.pointBean>();

            List<int> codigosPedidos = new List<int>();
            int ordemDestinos = 0;
            for (int i = 0; i < coletas.Count; i++)
            {
                planoViagemDestinos.Add(ObterPlanoViagemDestino(coletas[i], cargaCTes, cargaLacres, ordem: (++ordemDestinos), codigosPedidos));
                codigosPedidos.Add(coletas[i].Pedidos?.Select(o => o.CargaPedido.Pedido.Codigo)?.FirstOrDefault() ?? 0);
            }
            codigosPedidos = new List<int>();
            for (int i = 0; i < entregas.Count; i++)
            {
                if (planoViagemDestinos.Count > 0 && IsDestinoIgualAnterior(planoViagemDestinos[ordemDestinos - 1], entregas[i].Cliente))
                    continue;

                planoViagemDestinos.Add(ObterPlanoViagemDestino(entregas[i], cargaCTes, cargaLacres, ordem: (++ordemDestinos), codigosPedidos));
                codigosPedidos.Add(entregas[i].Pedidos?.Select(o => o.CargaPedido.Pedido.Codigo)?.FirstOrDefault() ?? 0);
            }

            return planoViagemDestinos.ToArray();
        }

        private ServicoInfolog.itineraryBean ObterPlanoViagemItinerario(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasEColetas = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (entregasEColetas.Count == 0)
                throw new ServicoException("Não existem destinatários informados para a carga.");

            ServicoInfolog.itineraryBean itinerario = new ServicoInfolog.itineraryBean();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = (from o in entregasEColetas where !o.Coleta orderby o.Ordem select o).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> coletas = (from o in entregasEColetas where o.Coleta orderby o.Ordem select o).ToList();

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta = (from o in entregasEColetas where o.Coleta orderby o.Ordem select o).FirstOrDefault();
            coletas.Remove(primeiraColeta);

            itinerario.Destinos = ObterPlanoViagemDestinos(carga, entregas, coletas);

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                itinerario.Origem = ObterPlanoViagemOrigemTMS(carga, primeiraColeta);
            else
                itinerario.Origem = ObterPlanoViagemOrigem(carga, primeiraColeta);

            return itinerario;
        }

        private ServicoInfolog.driversBean ObterPlanoViagemMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();

            if (motorista == null)
                throw new ServicoException("Não existe um motorista informado para a carga.");

            ServicoInfolog.driversBean planoViagemMotoristas = new ServicoInfolog.driversBean();

            planoViagemMotoristas.PrimeiroMotorista = new ServicoInfolog.driverBean()
            {
                Documento = new ServicoInfolog.documentoBean()
                {
                    Numero = motorista.CPF.ObterSomenteNumeros(),
                    Tipo = "2"
                },
                Nome = motorista.Nome.Left(30),
                Tipo = "FROTA"
            };

            return planoViagemMotoristas;
        }

        private ServicoInfolog.pointBean ObterPlanoPorCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente clienteOrigem)
        {

            ServicoInfolog.pointBean planoViagemOrigem = new ServicoInfolog.pointBean()
            {
                Carga = "TRUE",
                Descarga = "FALSE",
                Entidade = new ServicoInfolog.relationalBean()
                {
                    Documento = new ServicoInfolog.documentoBean()
                    {
                        Numero = clienteOrigem.CPF_CNPJ_SemFormato ?? "",
                        Tipo = "1"
                    },
                    Cidade = clienteOrigem.Localidade?.Descricao.Left(50) ?? "",
                    Estado = clienteOrigem.Localidade?.Estado?.Sigla ?? "",
                    Ibge = clienteOrigem.Localidade?.CodigoIBGE.ToString() ?? "",
                    Logradouro = new ServicoInfolog.logradouroBean()
                    {
                        Bairro = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.Bairro.Left(30))) ?? "",
                        Cep = clienteOrigem.CEP.ObterSomenteNumeros() ?? "",
                        Complemento = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.Complemento.Left(20))) ?? "",
                        Endereco = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.Endereco.Left(50))) ?? "",
                        Numero = clienteOrigem.Numero.Left(10) ?? ""
                    },
                    Nome = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.Nome.Left(30))) ?? "",
                    NomeFantasia = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.NomeFantasia.Left(30))) ?? "",
                    Pais = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(clienteOrigem.Localidade?.Pais?.Nome.Left(20))) ?? ""
                },
                Ordem = 0,
                PrevisaoEventos = new ServicoInfolog.previsaoEventosRoteiroBean()
                {
                    Iv = carga.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""
                }
            };

            return planoViagemOrigem;
        }

        private ServicoInfolog.pointBean ObterPlanoPorFilial(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente clienteFilial)
        {
            ServicoInfolog.pointBean planoViagemOrigem = new ServicoInfolog.pointBean()
            {
                Carga = "TRUE",
                Descarga = "FALSE",
                Entidade = new ServicoInfolog.relationalBean()
                {
                    Documento = new ServicoInfolog.documentoBean()
                    {
                        Numero = carga.Filial.CNPJ.ObterSomenteNumeros() ?? "",
                        Tipo = "1"
                    },
                    Cidade = carga.Filial.Localidade?.Descricao.Left(50) ?? "",
                    Estado = carga.Filial.Localidade?.Estado?.Sigla ?? "",
                    Ibge = carga.Filial.Localidade?.CodigoIBGE.ToString() ?? "",
                    Logradouro = new ServicoInfolog.logradouroBean()
                    {
                        Bairro = clienteFilial.Bairro.Left(30) ?? "",
                        Cep = clienteFilial.CEP.ObterSomenteNumeros() ?? "",
                        Complemento = clienteFilial.Complemento.Left(20) ?? "",
                        Endereco = clienteFilial.Endereco.Left(50) ?? "",
                        Numero = clienteFilial.Numero.Left(10) ?? ""
                    },
                    Nome = carga.Filial.Descricao.Left(30) ?? "",
                    NomeFantasia = carga.Filial.Descricao.Left(30) ?? "",
                    Pais = carga.Filial.Localidade?.Pais?.Nome.Left(20) ?? ""
                },
                Ordem = 0,
                PrevisaoEventos = new ServicoInfolog.previsaoEventosRoteiroBean()
                {
                    Iv = carga.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""
                }
            };

            return planoViagemOrigem;
        }

        private ServicoInfolog.pointBean ObterPlanoViagemOrigem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta)
        {
            if (carga.Filial == null)
                throw new ServicoException("Não existe uma filial informada para a carga.");

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente clienteFilial = repositorioCliente.BuscarPorCPFCNPJ(carga.Filial.CNPJ.ToDouble());
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = primeiraColeta?.Pedidos?.Select(o => o.CargaPedido.Pedido)?.FirstOrDefault();

            ServicoInfolog.pointBean planoViagemOrigem = primeiraColeta != null ? ObterPlanoPorCliente(carga, primeiraColeta.Cliente) : ObterPlanoPorFilial(carga, clienteFilial);

            if (pedido?.DataPrevisaoSaida != null)
                planoViagemOrigem.PrevisaoEventos.Iv = pedido.DataPrevisaoSaida.Value.AddHours(-1).ToString("dd/MM/yyyy HH:mm") ?? "";

            planoViagemOrigem.Documentos = new ServicoInfolog.fiscalDocumentBean[1];

            planoViagemOrigem.Documentos[0] = new ServicoInfolog.fiscalDocumentBean()
            {
                Codigo = $"{carga.Filial.CodigoFilialEmbarcador}{carga.CodigoCargaEmbarcador}",
                Tipo = "7"
            };

            return planoViagemOrigem;
        }

        private ServicoInfolog.pointBean ObterPlanoViagemOrigemTMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = primeiraColeta?.Pedidos?.Select(o => o.CargaPedido)?.FirstOrDefault();

            if (cargaPedido == null)
                cargaPedido = carga.Pedidos.FirstOrDefault();

            if (cargaPedido == null)
                return null;

            Dominio.Entidades.Cliente clienteOrigem = cargaPedido.PontoPartida;

            if (clienteOrigem == null)
                clienteOrigem = cargaPedido.Pedido.Remetente;

            if (clienteOrigem == null)
                return null;

            ServicoInfolog.pointBean planoViagemOrigem = ObterPlanoPorCliente(carga, clienteOrigem);

            if (cargaPedido.Pedido.DataPrevisaoSaida.HasValue)
                planoViagemOrigem.PrevisaoEventos.Iv = cargaPedido.Pedido.DataPrevisaoSaida.Value.AddHours(-1).ToString("dd/MM/yyyy HH:mm");

            planoViagemOrigem.Documentos = new ServicoInfolog.fiscalDocumentBean[1];

            planoViagemOrigem.Documentos[0] = new ServicoInfolog.fiscalDocumentBean()
            {
                Codigo = $"{carga.CodigoCargaEmbarcador}",
                Tipo = "7"
            };

            return planoViagemOrigem;
        }

        private ServicoInfolog.planoStatusBean ObterPlanoViagemStatus(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new ServicoInfolog.planoStatusBean()
            {
                TipoPlano = string.IsNullOrWhiteSpace(carga.TipoOperacao?.TipoPlanoInfolog) ? "DISTRIBUICAO LOJA" : carga.TipoOperacao.TipoPlanoInfolog,
                TipoRastreamento = "SATELITE"
            };
        }

        private ServicoInfolog.transportBean ObterPlanoViagemTransportadora(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Empresa == null)
                throw new ServicoException("Não existe um transportador informado para a carga.");

            ServicoInfolog.transportBean planoViagemTransportadora = new ServicoInfolog.transportBean()
            {
                Entidade = new ServicoInfolog.relationalBean()
                {
                    Documento = new ServicoInfolog.documentoBean()
                    {
                        Numero = carga.Empresa.CNPJ_SemFormato,
                        Tipo = "1"
                    },
                    Cidade = carga.Empresa.Localidade?.Descricao.Left(50) ?? "",
                    Estado = carga.Empresa.Localidade?.Estado?.Sigla ?? "",
                    Ibge = carga.Empresa.Localidade?.CodigoIBGE.ToString() ?? "",
                    Logradouro = new ServicoInfolog.logradouroBean()
                    {
                        Bairro = carga.Empresa.Bairro.Left(30) ?? "",
                        Cep = carga.Empresa.CEP.ObterSomenteNumeros() ?? "",
                        Complemento = carga.Empresa.Complemento.Left(20) ?? "",
                        Endereco = carga.Empresa.Endereco.Left(50) ?? "",
                        Numero = carga.Empresa.Numero.Left(10) ?? ""
                    },
                    Nome = carga.Empresa.RazaoSocial.Left(30) ?? "",
                    NomeFantasia = carga.Empresa.NomeFantasia.Left(30) ?? "",
                    Pais = carga.Empresa.Localidade?.Pais?.Nome.Left(20) ?? ""
                }
            };

            return planoViagemTransportadora;
        }

        private ServicoInfolog.userBean ObterPlanoViagemUsuario(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new ServicoInfolog.userBean()
            {
                Nome = configuracaoIntegracao.UsuarioInfolog,
                Senha = configuracaoIntegracao.SenhaInfolog
            };
        }

        private ServicoInfolog.planValueBean ObterPlanoViagemValores(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            decimal valorTotalMercadoria = repositorioPedidoXMLNotaFiscal.ObterValorMercadoriaPorCarga(carga.Codigo);
            if (valorTotalMercadoria == 0)
                valorTotalMercadoria = repCargaPedido.BuscarValorTotalNotasFiscais(carga.Codigo);

            return new ServicoInfolog.planValueBean()
            {
                ValorTotal = valorTotalMercadoria.ToString("F2", CultureInfo.CreateSpecificCulture("pt-BR"))
            };
        }

        private ServicoInfolog.vehicleBean ObterPlanoViagemVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return null;

            return new ServicoInfolog.vehicleBean()
            {
                Placa = veiculo.Placa
            };
        }

        private ServicoInfolog.vehiclesBean ObterPlanoViagemVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            ServicoInfolog.vehiclesBean planoViagemVeiculos = new ServicoInfolog.vehiclesBean();

            planoViagemVeiculos.PrimeiroVeiculos = ObterPlanoViagemVeiculo(carga.Veiculo);

            if (carga.VeiculosVinculados?.Count > 0)
            {
                planoViagemVeiculos.Carretas = new ServicoInfolog.vehicleBean[carga.VeiculosVinculados.Count];

                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                    planoViagemVeiculos.Carretas[i] = ObterPlanoViagemVeiculo(carga.VeiculosVinculados.ElementAt(i));
            }

            return planoViagemVeiculos;
        }

        private string ObterProtocoloIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repositorioCargaIntegracao.BuscarPorCargaETipoIntegracao(cancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, cancelamentoCargaIntegracao.TipoIntegracao.Codigo);

            if (string.IsNullOrWhiteSpace(cargaIntegracao?.Protocolo))
                throw new ServicoException("Naõ existe o protocolo de integração da carga com a Infolog");

            return cargaIntegracao.Protocolo;
        }

        private System.ServiceModel.BasicHttpBinding ObterBinding(string url)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = new TimeSpan(0, 1, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return binding;
        }

        private bool IsDestinoIgualAnterior(ServicoInfolog.pointBean planoViagemDestino, Dominio.Entidades.Cliente cliente)
        {
            if (planoViagemDestino.Entidade.Documento.Numero != cliente.CPF_CNPJ_SemFormato)
                return false;

            planoViagemDestino.Carga = "TRUE";
            planoViagemDestino.Descarga = "TRUE";

            planoViagemDestino.PrevisaoEventos.Iv = planoViagemDestino.PrevisaoEventos.Fv;
            planoViagemDestino.PrevisaoEventos.Fv = null;

            return true;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoInfolog.criarPlanoViagem planoViagemCancelar = ObterPlanoViagemCancelar(cancelamentoCargaIntegracao, configuracaoIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoInfolog.PlanWebServiceClient planWebServiceClient = new ServicoInfolog.PlanWebServiceClient(ObterBinding(configuracaoIntegracao.URLIntegracaoInfolog), new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLIntegracaoInfolog));

                planWebServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoInfolog.criarPlanoViagemResponse retorno = planWebServiceClient.criarPlanoViagem(planoViagemCancelar);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                cancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cancelamentoCargaIntegracao.NumeroTentativas++;

                if (retorno.Resultado.Codigo == "00")
                {
                    cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cancelamentoCargaIntegracao.Protocolo = retorno.Resultado.IdentificadorPlano;
                }
                else
                {
                    cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cancelamentoCargaIntegracao.ProblemaIntegracao = (retorno.Resultado.Codigo == "-1") ? "CÓDIGO DE ERRO NÃO ENCONTRADO" : retorno.Resultado.Mensagem;
                }

                servicoArquivoTransacao.Adicionar(cancelamentoCargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Infolog";

                servicoArquivoTransacao.Adicionar(cancelamentoCargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCancelamentoCargaIntegracao.Atualizar(cancelamentoCargaIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoInfolog.criarPlanoViagem planoViagemAdicionar = ObterPlanoViagemAdicionar(cargaIntegracao.Carga, configuracaoIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoInfolog.PlanWebServiceClient planWebServiceClient = new ServicoInfolog.PlanWebServiceClient(ObterBinding(configuracaoIntegracao.URLIntegracaoInfolog), new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLIntegracaoInfolog));

                planWebServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoInfolog.criarPlanoViagemResponse retorno = planWebServiceClient.criarPlanoViagem(planoViagemAdicionar);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retorno.Resultado.Codigo == "00")
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaIntegracao.Protocolo = retorno.Resultado.IdentificadorPlano;
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = (retorno.Resultado.Codigo == "-1") ? "ITINERÁRIO POSSUI DESTINOS SEQUENCIADOS" : retorno.Resultado.Mensagem;
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Infolog";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        #endregion
    }
}
