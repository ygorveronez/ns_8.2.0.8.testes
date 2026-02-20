using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class ImportacaoCTeEmitidoForaEmbarcador
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ImportacaoCTeEmitidoForaEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos 

        public Dominio.ObjetosDeValor.Embarcador.CTe.RetornoImportacaoCTeEmitidoForaEmbarcador ImportarCTeEmitidoForaEmbarcador(Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.RetornoImportacaoCTeEmitidoForaEmbarcador retorno = new Dominio.ObjetosDeValor.Embarcador.CTe.RetornoImportacaoCTeEmitidoForaEmbarcador();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = usuario,
                Empresa = usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(_unitOfWork);
            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> codigosLinhasGerar = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarCodigosLinhasPendentesGeracaoImportacaoCTeEmitidoForaEmbarcador(importacaoCTeEmitidoForaEmbarcador.Codigo);
            List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> colunasGerar = repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna.BuscarPorImportacaoPendentesGeracaoCTeEmitidoForaEmbarcador(importacaoCTeEmitidoForaEmbarcador.Codigo);
            int contador = 0;

            try
            {
                for (int i = 0; i < codigosLinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha linha = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarPorCodigo(codigosLinhasGerar[i], false);

                    _unitOfWork.Start();
                    linha.Situacao = SituacaoImportacaoCTeEmitidoForaEmbarcador.Processando;
                    repImportacaoCTeEmitidoForaEmbarcadorLinha.Atualizar(linha);
                    _unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = ConverterParaImportacao(colunas);

                    _unitOfWork.Start();

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarCTeEmitidoForaEmbarcadorLinha(dadosLinha, configuracaoTMS);

                        if (retornoLinha.contar)
                        {
                            if (retornoLinha.codigo > 0)
                            {
                                linha.CTeEmitidoForaEmbarcador = new Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador() { Codigo = retornoLinha.codigo };
                                linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso;
                                linha.Mensagem = "CTe importado.";
                                repImportacaoCTeEmitidoForaEmbarcadorLinha.Atualizar(linha);
                                contador++;
                            }
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            _unitOfWork.Start();
                            linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro;
                            linha.Mensagem = retornoLinha.mensagemFalha;
                            repImportacaoCTeEmitidoForaEmbarcadorLinha.Atualizar(linha);
                        }

                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        _unitOfWork.Start();
                        linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro;
                        linha.Mensagem = ex.Message;
                        repImportacaoCTeEmitidoForaEmbarcadorLinha.Atualizar(linha);
                        _unitOfWork.CommitChanges();
                    }
                }

                retorno.TotalCTesEmitidosForaEmbarcador = contador;
                retorno.Sucesso = contador > 0;
                retorno.Mensagem = contador > 0 ? "Importação de CTe finalizada com sucesso." : "Não foi possível importar todos os CTes, verifique no histórico. ";
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarCTeEmitidoForaEmbarcadorLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaCTeEmitidoForaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                int tipo = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipo = (from obj in linha.Colunas where obj.NomeCampo == "Tipo" select obj).FirstOrDefault();
                if (colTipo != null)
                    tipo = ((string)colTipo.Valor).ToInt();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "Numero" select obj).FirstOrDefault();
                int numero = 0;
                if (colNumero != null)
                    numero = ((string)colNumero.Valor).ToInt();
                if (numero == 0)
                    return RetornarFalhaLinha("O número não foi informado");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerie = (from obj in linha.Colunas where obj.NomeCampo == "Serie" select obj).FirstOrDefault();
                int serie = 0;
                if (colSerie != null)
                    serie = ((string)colSerie.Valor).ToInt();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChave = (from obj in linha.Colunas where obj.NomeCampo == "Chave" select obj).FirstOrDefault();
                string chaveNFe = string.Empty;
                if (colChave != null)
                    chaveNFe = (string)colChave.Valor;
                if (serie == 0 && Utilidades.Validate.ValidarChave(chaveNFe))
                    serie = Utilidades.Chave.ObterSerie(chaveNFe).ToInt();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissao = (from obj in linha.Colunas where obj.NomeCampo == "DataEmissao" select obj).FirstOrDefault();
                DateTime? dataEmissao = null;
                if (colDataEmissao != null)
                    dataEmissao = ((string)colDataEmissao.Valor).ToNullableDateTime();
                if (!dataEmissao.HasValue)
                    return RetornarFalhaLinha("A data de emissão não foi informada");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coltransportador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTransportador" select obj).FirstOrDefault();
                Dominio.Entidades.Empresa empresa = null;
                if (coltransportador != null)
                {
                    string somenteNumeros = Utilidades.String.OnlyNumbers(coltransportador.Valor);
                    if (!string.IsNullOrWhiteSpace(somenteNumeros))
                    {
                        if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 6 || configuracaoTMS.Pais == TipoPais.Exterior))
                        {
                            string cnpjTransportador = configuracaoTMS.Pais != TipoPais.Exterior ? long.Parse(somenteNumeros).ToString("d14") : somenteNumeros;
                            empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                            if (empresa == null || empresa.Status == "I")
                                return RetornarFalhaLinha("A Empresa informada não existe base Multisoftware");
                        }
                        else
                            return RetornarFalhaLinha("Registro ignorado na Importação", true);
                    }
                }
                if (empresa == null)
                    return RetornarFalhaLinha("O transportador não foi informado");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJRemetente" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente remetente = null;
                if (colRemetente != null)
                {
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colRemetente.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                    {
                        double cpfCNPJRemetente = double.Parse(somenteNumeros);
                        remetente = repCliente.BuscarPorCPFCNPJ(cpfCNPJRemetente);
                        if (remetente == null) return RetornarFalhaLinha("O remetente informado não está cadastrado na base da Multisoftware");
                    }
                    else
                        return RetornarFalhaLinha("Registro ignorado na Importação", true);
                }
                if (remetente == null)
                    return RetornarFalhaLinha("O remetente não foi informado");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CNPJDestinatario" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente destinatario = null;
                if (colDestinatario != null)
                {
                    double cpfCNPJDestinatario = 0;
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colDestinatario.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                    {
                        cpfCNPJDestinatario = double.Parse(somenteNumeros);
                        destinatario = repCliente.BuscarPorCPFCNPJ(cpfCNPJDestinatario);
                        if (destinatario == null) return RetornarFalhaLinha("O destinatário informado não está cadastrado na base da Multisoftware");
                    }
                    else
                        return RetornarFalhaLinha("Registro Ignorado na Importação", true);
                }
                if (destinatario == null)
                    return RetornarFalhaLinha("O destinatário não foi informado");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRecebedor = (from obj in linha.Colunas where obj.NomeCampo == "CNPJRecebedor" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente recebedor = null;
                if (colRecebedor != null && colRecebedor.Valor != null)
                {
                    double cpfCNPJRecebedor = Utilidades.String.OnlyNumbers((string)colRecebedor.Valor).ToDouble();
                    if (cpfCNPJRecebedor > 0d)
                    {
                        recebedor = repCliente.BuscarPorCPFCNPJ(cpfCNPJRecebedor);
                        if (recebedor == null) return RetornarFalhaLinha("O recebedor informado não está cadastrado na base da Multisoftware");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTomador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTomador" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente tomador = null;
                if (colTomador != null && colTomador.Valor != null)
                {
                    double cpfCNPJTomador = Utilidades.String.OnlyNumbers((string)colTomador.Valor).ToDouble();
                    if (cpfCNPJTomador > 0d)
                    {
                        tomador = repCliente.BuscarPorCPFCNPJ(cpfCNPJTomador);
                        if (tomador == null) return RetornarFalhaLinha("O tomador informado não está cadastrado na base da Multisoftware");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExpedidor = (from obj in linha.Colunas where obj.NomeCampo == "CNPJExpedidor" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente expedidor = null;
                if (colExpedidor != null && colExpedidor.Valor != null)
                {
                    double cpfCNPJExpedidor = Utilidades.String.OnlyNumbers((string)colExpedidor.Valor).ToDouble();
                    if (cpfCNPJExpedidor > 0d)
                    {
                        expedidor = repCliente.BuscarPorCPFCNPJ(cpfCNPJExpedidor);
                        if (expedidor == null) return RetornarFalhaLinha("O expedidor informado não está cadastrado na base da Multisoftware");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFOrigem" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeOrigem" select obj).FirstOrDefault();
                Dominio.Entidades.Localidade origem = null;
                if (colUFOrigem != null && colLocalidadeOrigem != null)
                {
                    string ufOrigem = colUFOrigem.Valor;
                    string localidade = Utilidades.String.RemoveDiacritics(colLocalidadeOrigem.Valor);
                    origem = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), ufOrigem.Trim());

                    if (origem == null)
                        return RetornarFalhaLinha("Não foi encontrado localidade de origem da base Multisoftware");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeDestino" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFDestino = (from obj in linha.Colunas where obj.NomeCampo == "UFDestino" select obj).FirstOrDefault();
                Dominio.Entidades.Localidade destino = null;
                if (colLocalidadeDestino != null && colUFDestino != null)
                {
                    string localidade = Utilidades.String.RemoveDiacritics(colLocalidadeDestino.Valor);
                    string ufDestino = colUFDestino.Valor;
                    destino = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), ufDestino.Trim());

                    if (destino == null)
                        return RetornarFalhaLinha("Não foi encontrado localidade de destino na base Multisoftware");
                }

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                if (colTipoOperacao != null)
                {
                    tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao((string)colTipoOperacao.Valor);

                    if (tipoOperacao == null) return RetornarFalhaLinha("O tipo de operação informado não existe na base Multisoftware");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeNFe = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeNFe" select obj).FirstOrDefault();
                int quantidadeNFe = 0;
                if (colQuantidadeNFe != null)
                    quantidadeNFe = ((string)colQuantidadeNFe.Valor).ToInt();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeCTe = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeCTe" select obj).FirstOrDefault();
                int quantidadeCTe = 0;
                if (colQuantidadeCTe != null)
                    quantidadeCTe = ((string)colQuantidadeCTe.Valor).ToInt();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFrete = (from obj in linha.Colunas where obj.NomeCampo == "ValorFrete" select obj).FirstOrDefault();
                decimal valorFrete = 0;
                if (colValorFrete != null && colValorFrete.Valor != null)
                    valorFrete = Utilidades.Decimal.Converter((string)colValorFrete.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorTotalMercadoria = (from obj in linha.Colunas where obj.NomeCampo == "ValorTotalMercadoria" select obj).FirstOrDefault();
                decimal valorTotalMercadoria = 0;
                if (colValorTotalMercadoria != null && colValorTotalMercadoria.Valor != null)
                    valorTotalMercadoria = Utilidades.Decimal.Converter((string)colValorTotalMercadoria.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoBaseCalculo = (from obj in linha.Colunas where obj.NomeCampo == "PesoBaseCalculo" select obj).FirstOrDefault();
                decimal pesoBaseCalculo = 0;
                if (colPesoBaseCalculo != null && colPesoBaseCalculo.Valor != null)
                    pesoBaseCalculo = Utilidades.Decimal.Converter((string)colPesoBaseCalculo.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBaseICMS = (from obj in linha.Colunas where obj.NomeCampo == "BaseICMS" select obj).FirstOrDefault();
                decimal baseICMS = 0;
                if (colBaseICMS != null && colBaseICMS.Valor != null)
                    baseICMS = Utilidades.Decimal.Converter((string)colBaseICMS.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAliquotaICMS = (from obj in linha.Colunas where obj.NomeCampo == "AliquotaICMS" select obj).FirstOrDefault();
                decimal aliquotaICMS = 0;
                if (colAliquotaICMS != null && colAliquotaICMS.Valor != null)
                    aliquotaICMS = Utilidades.Decimal.Converter((string)colAliquotaICMS.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorICMS = (from obj in linha.Colunas where obj.NomeCampo == "ValorICMS" select obj).FirstOrDefault();
                decimal valorICMS = 0;
                if (colValorICMS != null && colValorICMS.Valor != null)
                    valorICMS = Utilidades.Decimal.Converter((string)colValorICMS.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTotalImpostos = (from obj in linha.Colunas where obj.NomeCampo == "TotalImpostos" select obj).FirstOrDefault();
                decimal totalImpostos = 0;
                if (colTotalImpostos != null && colTotalImpostos.Valor != null)
                    totalImpostos = Utilidades.Decimal.Converter((string)colTotalImpostos.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTotalFrete = (from obj in linha.Colunas where obj.NomeCampo == "TotalFrete" select obj).FirstOrDefault();
                decimal totalFrete = 0;
                if (colTotalFrete != null && colTotalFrete.Valor != null)
                    totalFrete = Utilidades.Decimal.Converter((string)colTotalFrete.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFretePeso = (from obj in linha.Colunas where obj.NomeCampo == "FretePeso" select obj).FirstOrDefault();
                decimal fretePeso = 0;
                if (colFretePeso != null && colFretePeso.Valor != null)
                    fretePeso = Utilidades.Decimal.Converter((string)colFretePeso.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrisAdv = (from obj in linha.Colunas where obj.NomeCampo == "GrisAdv" select obj).FirstOrDefault();
                decimal grisAdv = 0;
                if (colGrisAdv != null && colGrisAdv.Valor != null)
                    grisAdv = Utilidades.Decimal.Converter((string)colGrisAdv.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colImposto = (from obj in linha.Colunas where obj.NomeCampo == "Imposto" select obj).FirstOrDefault();
                decimal imposto = 0;
                if (colImposto != null && colImposto.Valor != null)
                    imposto = Utilidades.Decimal.Converter((string)colImposto.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedagio = (from obj in linha.Colunas where obj.NomeCampo == "Pedagio" select obj).FirstOrDefault();
                decimal pedagio = 0;
                if (colPedagio != null && colPedagio.Valor != null)
                    pedagio = Utilidades.Decimal.Converter((string)colPedagio.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTaxas = (from obj in linha.Colunas where obj.NomeCampo == "Taxas" select obj).FirstOrDefault();
                decimal taxas = 0;
                if (colTaxas != null && colTaxas.Valor != null)
                    taxas = Utilidades.Decimal.Converter((string)colTaxas.Valor.Replace("-", ""));

                Dominio.ObjetosDeValor.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorAdicionar importacaoCTeEmitidoForaEmbarcadorAdicionar = new Dominio.ObjetosDeValor.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorAdicionar()
                {
                    Tipo = tipo,
                    Numero = numero,
                    Serie = serie,
                    Chave = chaveNFe,
                    DataEmissao = dataEmissao,
                    Empresa = empresa,
                    Remetente = remetente,
                    Destinatario = destinatario,
                    Recebedor = recebedor,
                    Tomador = tomador,
                    Expedidor = expedidor,
                    MunicipioInicio = origem,
                    MunicipioFim = destino,
                    TipoOperacao = tipoOperacao,
                    QuantidadeCTe = quantidadeCTe,
                    QuantidadeNFe = quantidadeNFe,
                    ValorFrete = valorFrete,
                    ValorTotalMercadoria = valorTotalMercadoria,
                    PesoBaseCalculo = pesoBaseCalculo,
                    BaseICMS = baseICMS,
                    AliquotaICMS = aliquotaICMS,
                    ValorICMS = valorICMS,
                    TotalImpostos = totalImpostos,
                    TotalFrete = totalFrete,
                    FretePeso = fretePeso,
                    GrisAdv = grisAdv,
                    Imposto = imposto,
                    Pedagio = pedagio,
                    Taxas = taxas
                };

                retornoLinhaCTeEmitidoForaEmbarcador = AdicionarImportacaoCTeEmitidoForaEmbarcador(importacaoCTeEmitidoForaEmbarcadorAdicionar);
                if (!string.IsNullOrWhiteSpace(retornoLinhaCTeEmitidoForaEmbarcador.mensagemFalha))
                    return RetornarFalhaLinha(retornoLinhaCTeEmitidoForaEmbarcador.mensagemFalha);
            }

            catch (BaseException excecao)
            {
                return RetornarFalhaLinha(excecao.Message);
            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha." + "(" + ex2.Message + ")");
            }

            return RetornarSucessoLinha(retornoLinhaCTeEmitidoForaEmbarcador?.codigo ?? 0);
        }

        public bool GerarImportacaoCTeEmitidoForaEmbarcador(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, string nomeArquivo, string dadosArquivo, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dadosArquivo);

            if (linhas.Count == 0)
            {
                retorno.MensagemAviso = "Nenhuma linha encontrada na planilha";
                return false;
            }

            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(_unitOfWork);
            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(_unitOfWork);
            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna(_unitOfWork);

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcador = new Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = linhas.Count,
                Usuario = usuario,
                DataImportacao = DateTime.Now
            };

            repImportacaoCTeEmitidoForaEmbarcador.Inserir(importacaoCTeEmitidoForaEmbarcador, auditado);

            for (int i = 0; i < importacaoCTeEmitidoForaEmbarcador.QuantidadeLinhas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinhaArquivo = linhas[i];

                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha linha = new Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha()
                {
                    ImportacaoCTeEmitidoForaEmbarcador = importacaoCTeEmitidoForaEmbarcador,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente
                };

                repImportacaoCTeEmitidoForaEmbarcadorLinha.Inserir(linha);

                for (int j = 0; j < dadosLinhaArquivo.Colunas.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColunaArquivo = dadosLinhaArquivo.Colunas[j];

                    Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna coluna = new Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = dadosColunaArquivo.NomeCampo,
                        Valor = (string)dadosColunaArquivo.Valor
                    };
                    if (string.IsNullOrWhiteSpace(coluna.Valor))
                        coluna.Valor = "";

                    repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna.Inserir(coluna);
                }
            }

            _unitOfWork.CommitChanges();

            retorno.MensagemAviso = "Planilha adicionada com sucesso à fila de processamento.";
            retorno.Total = linhas.Count;
            retorno.Importados = linhas.Count;

            return true;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha AdicionarImportacaoCTeEmitidoForaEmbarcador(Dominio.ObjetosDeValor.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorAdicionar importacaoCTeEmitidoForaEmbarcadorAdicionar)
        {
            Repositorio.Embarcador.CTe.CTeEmitidoForaEmbarcador repCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.CTeEmitidoForaEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador cteEmitidoForaEmbarcador = repCTeEmitidoForaEmbarcador.BuscarPorNumero(importacaoCTeEmitidoForaEmbarcadorAdicionar.Numero, importacaoCTeEmitidoForaEmbarcadorAdicionar.Serie, importacaoCTeEmitidoForaEmbarcadorAdicionar.Empresa.Codigo, importacaoCTeEmitidoForaEmbarcadorAdicionar.Chave);

            string retorno = "";

            bool inserir = false;
            if (cteEmitidoForaEmbarcador == null)
            {
                cteEmitidoForaEmbarcador = new Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador();
                inserir = true;
            }

            cteEmitidoForaEmbarcador.Tipo = importacaoCTeEmitidoForaEmbarcadorAdicionar.Tipo;
            cteEmitidoForaEmbarcador.Numero = importacaoCTeEmitidoForaEmbarcadorAdicionar.Numero;
            cteEmitidoForaEmbarcador.Serie = importacaoCTeEmitidoForaEmbarcadorAdicionar.Serie;
            cteEmitidoForaEmbarcador.Chave = importacaoCTeEmitidoForaEmbarcadorAdicionar.Chave;
            cteEmitidoForaEmbarcador.DataEmissao = importacaoCTeEmitidoForaEmbarcadorAdicionar.DataEmissao;
            cteEmitidoForaEmbarcador.Empresa = importacaoCTeEmitidoForaEmbarcadorAdicionar.Empresa;
            cteEmitidoForaEmbarcador.Remetente = importacaoCTeEmitidoForaEmbarcadorAdicionar.Remetente;
            cteEmitidoForaEmbarcador.Destinatario = importacaoCTeEmitidoForaEmbarcadorAdicionar.Destinatario;
            cteEmitidoForaEmbarcador.Recebedor = importacaoCTeEmitidoForaEmbarcadorAdicionar.Recebedor;
            cteEmitidoForaEmbarcador.Tomador = importacaoCTeEmitidoForaEmbarcadorAdicionar.Tomador;
            cteEmitidoForaEmbarcador.Expedidor = importacaoCTeEmitidoForaEmbarcadorAdicionar.Expedidor;
            cteEmitidoForaEmbarcador.MunicipioInicio = importacaoCTeEmitidoForaEmbarcadorAdicionar.MunicipioInicio;
            cteEmitidoForaEmbarcador.MunicipioFim = importacaoCTeEmitidoForaEmbarcadorAdicionar.MunicipioFim;
            cteEmitidoForaEmbarcador.TipoOperacao = importacaoCTeEmitidoForaEmbarcadorAdicionar.TipoOperacao;
            cteEmitidoForaEmbarcador.QuantidadeCTe = importacaoCTeEmitidoForaEmbarcadorAdicionar.QuantidadeCTe;
            cteEmitidoForaEmbarcador.QuantidadeNFe = importacaoCTeEmitidoForaEmbarcadorAdicionar.QuantidadeNFe;
            cteEmitidoForaEmbarcador.ValorFrete = importacaoCTeEmitidoForaEmbarcadorAdicionar.ValorFrete;
            cteEmitidoForaEmbarcador.ValorTotalMercadoria = importacaoCTeEmitidoForaEmbarcadorAdicionar.ValorTotalMercadoria;
            cteEmitidoForaEmbarcador.PesoBaseCalculo = importacaoCTeEmitidoForaEmbarcadorAdicionar.PesoBaseCalculo;
            cteEmitidoForaEmbarcador.BaseICMS = importacaoCTeEmitidoForaEmbarcadorAdicionar.BaseICMS;
            cteEmitidoForaEmbarcador.AliquotaICMS = importacaoCTeEmitidoForaEmbarcadorAdicionar.AliquotaICMS;
            cteEmitidoForaEmbarcador.ValorICMS = importacaoCTeEmitidoForaEmbarcadorAdicionar.ValorICMS;
            cteEmitidoForaEmbarcador.TotalImpostos = importacaoCTeEmitidoForaEmbarcadorAdicionar.TotalImpostos;
            cteEmitidoForaEmbarcador.TotalFrete = importacaoCTeEmitidoForaEmbarcadorAdicionar.TotalFrete;
            cteEmitidoForaEmbarcador.FretePeso = importacaoCTeEmitidoForaEmbarcadorAdicionar.FretePeso;
            cteEmitidoForaEmbarcador.GrisAdv = importacaoCTeEmitidoForaEmbarcadorAdicionar.GrisAdv;
            cteEmitidoForaEmbarcador.Imposto = importacaoCTeEmitidoForaEmbarcadorAdicionar.Imposto;
            cteEmitidoForaEmbarcador.Pedagio = importacaoCTeEmitidoForaEmbarcadorAdicionar.Pedagio;
            cteEmitidoForaEmbarcador.Taxas = importacaoCTeEmitidoForaEmbarcadorAdicionar.Taxas;

            if (inserir)
                repCTeEmitidoForaEmbarcador.Inserir(cteEmitidoForaEmbarcador);
            else
                repCTeEmitidoForaEmbarcador.Atualizar(cteEmitidoForaEmbarcador);

            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
            {
                codigo = cteEmitidoForaEmbarcador.Codigo,
                mensagemFalha = retorno
            };

        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> colunas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha()
            {
                Colunas = colunas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna()
                {
                    NomeCampo = o.NomeCampo,
                    Valor = o.Valor
                }).ToList()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        #endregion
    }
}