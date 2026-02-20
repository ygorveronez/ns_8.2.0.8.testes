using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Servicos
{
    public class PamCard
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public PamCard(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EmitirCIOT(int codigoCIOT)
        {
            // Repositorios
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);

            // Busca CIOT
            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
            bool utilizaTagPedagio = !string.IsNullOrWhiteSpace(ciot.Veiculo.NumeroCompraValePedagio);
            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

            // Gera campos para integracao
            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);
            List<ServicoPamCard.fieldTO> campos = this.GeraCamposPamCard(ciot, ctes);

            // Executa requisicao
            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "InsertFreightContract",
                    fields = campos.ToArray()
                }
            };

            ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                // Log de erro
                Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                AdicionaLog(inspector, ciot);
                throw;
            }

            // Log de sucesso
            AdicionaLog(inspector, ciot);

            ciot.CodigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
            ciot.MensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

            if (ciot.CodigoRetorno == "0")
            {
                string numeroCIOT = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.numero") select obj.value).FirstOrDefault();
                string codigoViagem = (from obj in retorno.@return where obj.key.Equals("viagem.id") select obj.value).FirstOrDefault();

                ciot.NumeroCIOT = numeroCIOT;
                ciot.CodigoCIOTIntegradora = codigoViagem;
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
            }
            else
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
            }

            repCIOT.Atualizar(ciot);

            if (utilizaTagPedagio && ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado) //Se tem valor de pedágio
                IntegrarPagamentoPedagio(ciot);
        }

        public void EmitirCIOTAbertura(int codigoCIOT)
        {
            // Repositorios
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);

            // Busca CIOT e dados
            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
            bool utilizaTagPedagio = !string.IsNullOrWhiteSpace(ciot.Veiculo.NumeroCompraValePedagio);
            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

            // Gera campos para integracao
            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = CTEAbertura(ciot);
            ciot.PesoBruto = 10;
            ciot.ValorTotalMercadoria = 1;
            ciot.ValorPedagio = 10;
            ciot.ValorAdiantamento = 0;
            ciot.ValorAbastecimento = 0;
            ciot.ValorFrete = 0;
            ciot.ValorBruto = ciot.ValorFrete /*+ ciot.ValorAdiantamento*/ + ciot.ValorINSS + ciot.ValorIRRF + ciot.ValorSEST + ciot.ValorSENAT;

            List<ServicoPamCard.fieldTO> campos = this.GeraCamposPamCard(ciot, ctes);

            // Executa requisicao
            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "InsertFreightContract",
                    fields = campos.ToArray()
                }
            };

            ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                // Log de erro
                Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                AdicionaLog(inspector, ciot);
                throw;
            }

            ciot.PesoBruto = 0;
            ciot.ValorTotalMercadoria = 0;
            ciot.ValorPedagio = 0;
            ciot.ValorAdiantamento = 0;
            ciot.ValorAbastecimento = 0;
            ciot.ValorFrete = 0;
            ciot.ValorBruto = 0;

            // Log de sucesso
            AdicionaLog(inspector, ciot);

            // Pega dados de retorno
            ciot.CodigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
            ciot.MensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

            if (ciot.CodigoRetorno == "0")
            {
                string numeroCIOT = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.numero") select obj.value).FirstOrDefault();
                string codigoViagem = (from obj in retorno.@return where obj.key.Equals("viagem.id") select obj.value).FirstOrDefault();

                ciot.NumeroCIOT = numeroCIOT;
                ciot.CodigoCIOTIntegradora = codigoViagem;
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
            }
            else
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
            }

            repCIOT.Atualizar(ciot);

            if (utilizaTagPedagio && ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado) //Se tem valor de pedágio
                IntegrarPagamentoPedagio(ciot);
        }

        public void AtualizarParcelasCIOT(int codigoCIOT)
        {
            // Repositorios
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            // Busca CIOT
            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

            //Busca Matriz
            Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT; //repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

            // Gera campos para integracao
            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            // Seta o ID do CIOT
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.CodigoCIOTIntegradora });

            // Preenche valores
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = empresaMatriz != null ? empresaMatriz.CNPJ : ciot.Empresa.CNPJ });
            if (empresaMatriz != null)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Empresa.CNPJ });
            }

            SetaCamposParcela(ref campos, ciot);
            SetaCamposFrete(ref campos, ciot, ctes, true, null);

            // Executa requisicao
            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "UpdateValuesFreightContract",
                    fields = campos.ToArray()
                }
            };

            ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                // Log de erro
                Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                AdicionaLog(inspector, ciot);
                throw;
            }

            // Log de sucesso
            AdicionaLog(inspector, ciot);

            ciot.CodigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
            ciot.MensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

            if (ciot.CodigoRetorno == "0")
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
            else
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;

            repCIOT.Atualizar(ciot);
        }

        private void SetaCamposParcela(ref List<ServicoPamCard.fieldTO> campos, Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            int quantidadeParcelas = 0;
            decimal valorAdiantamento = ciot.ValorAdiantamento;
            decimal valorAbastecimento = ciot.ValorAbastecimento;
            decimal valorFrete = ciot.ValorFrete - valorAdiantamento;

            if (valorAdiantamento > 0 && !ciot.PossuiAdiantamentoAbertura) //Conforme e-mail da Pancard em 27/04/2020 o adiantamento no fechamento deve ser igual ao da abertura, removido:  && !ciot.PossuiAdiantamentoAbertura em 12/06 retornado a validação
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorAdiantamento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
            }
            else if (ciot.PossuiAdiantamentoAbertura)
            {
                //quantidadeParcelas += 1;
            }

            if (valorAbastecimento > 0)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorAbastecimento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "4" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
            }

            quantidadeParcelas += 1;

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = "0" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela1.favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "1" });

            quantidadeParcelas += 1;

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorFrete.ToString("0.00", cultura) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela1.favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
        }

        private void SetaCamposFrete(ref List<ServicoPamCard.fieldTO> campos, Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes, bool opcaoAbertura, decimal? valorEstimado)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            int quantidadeFrete = 0;

            // Quando for PamCard com abertura e esta abrindo um ciot, o valor bruto é o valor estmado
            decimal valorBruto;
            if (opcaoAbertura && valorEstimado.HasValue)
                valorBruto = valorEstimado.Value;
            else
                valorBruto = opcaoAbertura ? ciot.ValorBruto : (from obj in ctes select obj.ValorFrete + obj.ValorIRRF + obj.ValorINSS + obj.ValorSEST + obj.ValorSENAT + obj.ValorAdiantamento).Sum();
            decimal valorIRRPF = opcaoAbertura ? ciot.ValorIRRF : (from obj in ctes select obj.ValorIRRF).Sum();
            decimal valorINSS = opcaoAbertura ? ciot.ValorINSS : (from obj in ctes select obj.ValorINSS).Sum();
            decimal valorSESTSENAT = opcaoAbertura ? ciot.ValorSEST + ciot.ValorSENAT : (from obj in ctes select obj.ValorSEST + obj.ValorSENAT).Sum();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = valorBruto.ToString("0.00", cultura) });

            if (valorIRRPF > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorIRRPF.ToString("0.00", cultura) });
            }

            if (valorINSS > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorINSS.ToString("0.00", cultura) });
            }

            if (valorSESTSENAT > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorSESTSENAT.ToString("0.00", cultura) });
            }

            if (quantidadeFrete > 0)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });
        }

        private List<ServicoPamCard.fieldTO> GeraCamposPamCard(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            bool opcaoAbertura = ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura;

            // Configuracao e Instancias
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.CIOTCidadesPedagio repCIOTCidadesPedagio = new Repositorio.CIOTCidadesPedagio(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            // Dados de preenchimento
            List<Dominio.Entidades.CIOTCidadesPedagio> cidadesPedagio = repCIOTCidadesPedagio.BuscaPorCIOT(ciot.Codigo);

            //Busca Matriz
            Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT; //repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

            // Campos para geracao da requisição
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = empresaMatriz != null ? empresaMatriz.CNPJ : ciot.Empresa.CNPJ });
            if (empresaMatriz != null)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Empresa.CNPJ });
            }
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id.cliente", value = ciot.Numero.ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contrato.numero", value = ciot.Numero.ToString() });

            campos.AddRange(ObterVeiculos(ciot));
            campos.AddRange(ObterFavorecidos(ciot));

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.numero.controle", value = "" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.partida", value = ciot.DataInicioViagem.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.termino", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.rota.id", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.rota.nome", value = ciot.Origem.Descricao + "/" + ciot.Origem.Estado.Sigla + " à " + ciot.Destino.Descricao + "/" + ciot.Destino.Estado.Sigla });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.origem.cidade.ibge", value = string.Format("{0:0000000}", ciot.Origem.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.destino.cidade.ibge", value = string.Format("{0:0000000}", ciot.Destino.CodigoIBGE) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.ponto.qtde", value = cidadesPedagio.Count().ToString() });
            for (int numeroCidadePedagio = 1, qtdCP = cidadesPedagio.Count; numeroCidadePedagio <= qtdCP; numeroCidadePedagio++)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.ponto" + numeroCidadePedagio.ToString() + ".cidade.ibge", value = cidadesPedagio[numeroCidadePedagio - 1].Localidade.CodigoIBGE.ToString() });

            bool utilizaTagPedagio = !string.IsNullOrWhiteSpace(ciot.Veiculo.NumeroCompraValePedagio); //Verificar se veículo tem numero do cartão de pedágio
            var valorPedagio = opcaoAbertura ? ciot.ValorPedagio : (from obj in ctes select obj.ValorPedagio).Sum();
            if (utilizaTagPedagio)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.solucao.id", value = "6" });
                if (valorPedagio > 0)
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.valor", value = valorPedagio.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.roteirizar", value = "S" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.idavolta", value = ciot.PedagioIdaVolta == Dominio.Enumeradores.OpcaoSimNao.Sim ? "S" : "N" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.praca", value = "N" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.utiliza.saldo", value = "" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.cartao.numero", value = "" });
            }
            else
            {
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.solucao.id", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.valor", value = valorPedagio.ToString("0.00", cultura) });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.status.id", value = "8" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.roteirizar", value = "N" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.praca", value = "N" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.utiliza.saldo", value = "" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.cartao.numero", value = "" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.natureza", value = ciot.NaturezaCarga.CodigoNatureza });
            var pesoBruto = opcaoAbertura ? ciot.PesoBruto : (from obj in ctes select obj.PesoBruto).Sum();
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.peso", value = pesoBruto.ToString("0.00", cultura) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.perfil.id", value = "1" });

            campos.AddRange(this.ObterDocumentos(ctes, cultura));

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.complementar.qtde", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.complementarN.tipo", value = "" });
            int quantidadeParcelas = 0;
            decimal valorAdiantamento = opcaoAbertura ? ciot.ValorAdiantamento : (from obj in ctes select obj.ValorAdiantamento).Sum();
            decimal valorAbastecimento = opcaoAbertura ? ciot.ValorAbastecimento : (from obj in ctes select obj.ValorAbastecimento).Sum();
            decimal valorFrete = opcaoAbertura ? ciot.ValorFrete : (from obj in ctes select obj.ValorFrete).Sum();


            if (valorAdiantamento > 0 && ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCard)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorAdiantamento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "3" });
            }
            else if (ciot.PossuiAdiantamentoAbertura && ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = ciot.ValorAdiantamentoAbertura.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataInicioViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
            }

            if (opcaoAbertura)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = opcaoAbertura ? "1" : "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = (ciot.ValorEstimado - ciot.ValorAdiantamentoAbertura).ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela1.favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "1" });
            }

            if (!opcaoAbertura)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorFrete.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela1.favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "2" });
            }

            if (valorAbastecimento > 0)
            {
                quantidadeParcelas += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = valorAbastecimento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".subtipo", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = ciot.DataTerminoViagem.ToString("dd/MM/yyyy") });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.Empresa.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.tipo.id", value = ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Motorista ? "3" : ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado ? "1" : "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".numero.cliente", value = "4" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });

            SetaCamposFrete(ref campos, ciot, ctes, opcaoAbertura, ciot.ValorEstimado);

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.prazo", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.indicador", value = "S" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.entrega.ressalva", value = "N" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.origem.pagamento", value = ciot.RegraQuitacao == Dominio.Enumeradores.RegraQuitacaoQuitacao.Filial ? "1" : "2" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.comprovacao.observacao", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.desconto.tipo", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.desconto.faixa.qtde", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.desconto.faixaN.ate", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.desconto.faixaN.percentual", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.quitacao.desconto.tolerancia", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.valorunitario", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.diferencafrete.credito", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.diferencafrete.debito", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.diferencafrete.tarifamotorista", value = "" });
            //
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.caminho", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.uf", value = "" });

            return campos;
        }

        public void EncerrarCIOT(int codigoCIOT)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            //Busca Matriz
            Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT; //repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = empresaMatriz != null ? empresaMatriz.CNPJ : ciot.Empresa.CNPJ });
            if (empresaMatriz != null)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Empresa.CNPJ });
            }
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id.cliente", value = ""});
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.CodigoCIOTIntegradora });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.NumeroCIOT });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = ciot.ValorBruto.ToString("0.00", cultura) });
            int quantidadeFrete = 0;
            decimal valorSESTSENAT = ciot.ValorSEST + ciot.ValorSENAT;

            if (ciot.ValorIRRF > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = ciot.ValorIRRF.ToString("0.00", cultura) });
            }

            if (ciot.ValorINSS > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = ciot.ValorINSS.ToString("0.00", cultura) });
            }

            if (valorSESTSENAT > 0)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorSESTSENAT.ToString("0.00", cultura) });
            }

            if (quantidadeFrete > 0)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.peso ", value = ""});

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "CloseFreightContract",
                    fields = campos.ToArray()
                }
            };

            ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                // Log de erro
                Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                AdicionaLog(inspector, ciot);
                throw;
            }

            // Log de sucesso
            AdicionaLog(inspector, ciot);

            ciot.CodigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
            ciot.MensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

            if (ciot.CodigoRetorno == "0")
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
            else
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;

            repCIOT.Atualizar(ciot);
        }

        public void IntegrarPagamentoPedagio(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                //Busca Matriz
                Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT; //repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

                List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.CodigoCIOTIntegradora });
                //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id.cliente", value = ""});
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = empresaMatriz != null ? empresaMatriz.CNPJ : ciot.Empresa.CNPJ });
                if (empresaMatriz != null)
                {
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Empresa.CNPJ });
                }

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.NumeroCIOT });

                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

                ServicoPamCard.execute execute = new ServicoPamCard.execute()
                {
                    arg0 = new ServicoPamCard.requestTO()
                    {
                        context = "PayToll",
                        fields = campos.ToArray()
                    }
                };

                ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

                try
                {
                    retorno = svcPamCard.execute(execute);
                }
                catch (Exception ex)
                {
                    // Log de erro
                    Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                    AdicionaLog(inspector, ciot);
                    throw;
                }

                // Log de sucesso
                AdicionaLog(inspector, ciot);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro PANCARD ao enviar PagamentoPedagio - " + ex.Message);
            }
        }

        public void CancelarCIOT(int codigoCIOT)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            //Busca Matriz
            Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT; //repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.CodigoCIOTIntegradora });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id.cliente ", value = ciot.Numero.ToString() });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = empresaMatriz != null ? empresaMatriz.CNPJ : ciot.Empresa.CNPJ });
            if (empresaMatriz != null)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Empresa.CNPJ });
            }
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.cancelamento.motivo", value = ciot.MotivoCancelamento });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.NumeroCIOT });

            ServicoPamCard.execute execute = new ServicoPamCard.execute();

            execute.arg0 = new ServicoPamCard.requestTO();

            execute.arg0.context = "CancelTrip";

            execute.arg0.fields = campos.ToArray();

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(ciot, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = svcPamCard.execute(execute);

            ciot.CodigoRetornoCancelamento = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
            ciot.MensagemRetornoCancelamento = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

            if (ciot.CodigoRetornoCancelamento == "0")
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado;

            repCIOT.Atualizar(ciot);
        }

        #endregion

        #region Métodos Privados

        private List<ServicoPamCard.fieldTO> ObterFavorecidos(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.qtde", value = "2" });

            campos.AddRange(this.ObterMotorista(ciot));
            campos.AddRange(this.ObterTransportador(ciot));

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterMotorista(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.tipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento.qtde", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.numero", value = Utilidades.String.OnlyNumbers(ciot.Motorista.CPF) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.tipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.numero", value = Utilidades.String.OnlyNumbers(ciot.Motorista.RG) });

            if (ciot.Motorista.EstadoRG != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.uf", value = ciot.Motorista.EstadoRG.Sigla });

            if (ciot.Motorista.OrgaoEmissorRG != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.emissor.id", value = ciot.Motorista.OrgaoEmissorRG.Value.ToString("d") });

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.emissao.data", value = "" });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nome", value = Utilidades.String.Left(ciot.Motorista.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.naturalidade.ibge", value = string.Format("{0:0000000}", ciot.Motorista.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.sexo", value = "M" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.logradouro", value = Utilidades.String.Left(ciot.Motorista.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.numero", value = "0" });

            if (!string.IsNullOrWhiteSpace(ciot.Motorista.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.complemento", value = Utilidades.String.Left(ciot.Motorista.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.bairro", value = Utilidades.String.Left(ciot.Motorista.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", ciot.Motorista.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(ciot.Motorista.CEP) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.propriedade.tipo.id", value = "1" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.reside.desde", value = "" });

            if (!string.IsNullOrWhiteSpace(ciot.Motorista.Telefone))
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.ddd", value = "0" + ciot.Motorista.Telefone?.Split(' ')[0]?.Replace("(", "").Replace(")", "") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.numero", value = ciot.Motorista.Telefone?.Split(' ')[1]?.Replace("-", "") });
            }
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.celular.operadora.id", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.celular.ddd", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.celular.numero", value = "" });

            if (!string.IsNullOrWhiteSpace(ciot.Motorista.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.email", value = ciot.Motorista.Email });

            if (ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.meio.pagamento", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.cartao.numero", value = ciot.NumeroCartaoMotorista });
            }
            else if (ciot.TipoFavorecido == Dominio.Enumeradores.TipoFavorecido.Contratado)
            {
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(_unitOfWork);

                Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);

                if (dadosTransportador != null && dadosTransportador.Banco != null)
                {
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.meio.pagamento", value = "2" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.banco", value = string.Format("{0:0000}", dadosTransportador.Banco.Numero) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia", value = dadosTransportador.Agencia });
                    if (!string.IsNullOrWhiteSpace(dadosTransportador.DigitoAgencia))
                        campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia.digito", value = dadosTransportador.DigitoAgencia });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.numero", value = dadosTransportador.NumeroConta });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.tipo", value = dadosTransportador.TipoConta == Dominio.ObjetosDeValor.Enumerador.TipoConta.Poupanca ? "2" : "1" });
                }
            }
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.banco", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia.digito", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.numero", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.tipo", value = "" });

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.empresa.nome", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.empresa.cnpj", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.empresa.rntrc", value = "" });

            if (ciot.Motorista.DataNascimento.HasValue)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.data.nascimento", value = ciot.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterTransportador(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.tipo", value = "1" });
            if (ciot.Veiculo != null && ciot.Veiculo.Proprietario != null && Utilidades.String.OnlyNumbers(ciot.Veiculo.Proprietario.CPF_CNPJ_SemFormato) == Utilidades.String.OnlyNumbers(ciot.Transportador.CPF_CNPJ_SemFormato))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento.qtde", value = "2" });
            else
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento.qtde", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento1.tipo", value = ciot.Transportador.Tipo == "F" ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento1.numero", value = Utilidades.String.OnlyNumbers(ciot.Transportador.CPF_CNPJ_SemFormato) });

            if (ciot.Veiculo != null && ciot.Veiculo.Proprietario != null && Utilidades.String.OnlyNumbers(ciot.Veiculo.Proprietario.CPF_CNPJ_SemFormato) == Utilidades.String.OnlyNumbers(ciot.Transportador.CPF_CNPJ_SemFormato))
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento2.tipo", value = "6" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento2.numero", value = string.Format("{0:00000000}", ciot.Veiculo.RNTRC) });
            }

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento1.uf", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento1.emissor.id", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.documento1.emissao.data", value = "" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.nome", value = Utilidades.String.Left(ciot.Transportador.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.naturalidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.sexo", value = "M" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.logradouro", value = Utilidades.String.Left(ciot.Transportador.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.numero", value = "0" });

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.complemento", value = Utilidades.String.Left(ciot.Transportador.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.bairro", value = Utilidades.String.Left(ciot.Transportador.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.cidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.cep", value = Utilidades.String.OnlyNumbers(ciot.Transportador.CEP) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.propriedade.tipo.id", value = "1" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.endereco.reside.desde", value = "" });

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Telefone1))
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.telefone.ddd", value = "0" + ciot.Transportador.Telefone1?.Split(' ')[0]?.Replace("(", "").Replace(")", "") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.telefone.numero", value = ciot.Transportador.Telefone1?.Split(' ')[1]?.Replace("-", "") });
            }

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.telefone.ddd", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.telefone.numero", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.celular.operadora.id", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.celular.ddd", value = "" });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.celular.numero", value = "" });
            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.email", value = ciot.Transportador.Email.Split(';').FirstOrDefault() });

            if (ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito)
            {
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(_unitOfWork);

                Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);

                if (dadosTransportador != null && dadosTransportador.Banco != null)
                {
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.meio.pagamento", value = "2" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.conta.banco", value = string.Format("{0:0000}", dadosTransportador.Banco.Numero) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.conta.agencia", value = dadosTransportador.Agencia });
                    if (!string.IsNullOrWhiteSpace(dadosTransportador.DigitoAgencia))
                        campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.conta.agencia.digito", value = dadosTransportador.DigitoAgencia });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.conta.numero", value = dadosTransportador.NumeroConta });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.conta.tipo", value = dadosTransportador.TipoConta == Dominio.ObjetosDeValor.Enumerador.TipoConta.Poupanca ? "2" : "1" });
                }
            }
            else
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.meio.pagamento", value = "1" });
                if (!string.IsNullOrWhiteSpace(ciot.NumeroCartaoTransportador))
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.cartao.numero", value = ciot.NumeroCartaoTransportador });
                else
                {
                    if (ciot.Transportador.CPF_CNPJ_SemFormato == ciot.Motorista.CPF)
                        campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.cartao.numero", value = ciot.NumeroCartaoMotorista });
                }
            }

            //if (ciot.Veiculo.Proprietario != null && ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito)
            //{
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.empresa.cnpj", value = Utilidades.String.OnlyNumbers(ciot.Transportador.CPF_CNPJ_SemFormato) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.empresa.rntrc", value = string.Format("{0:00000000}", ciot.Veiculo.RNTRC.ToString()) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.empresa.nome", value = Utilidades.String.Left(ciot.Transportador.Nome, 40) });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido2.data.nascimento", value = "" });
            //}

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterVeiculos(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.qtde", value = (1 + ciot.Veiculo.VeiculosVinculados.Count()).ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo1.placa", value = ciot.Veiculo.Placa });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo1.rntrc", value = string.Format("{0:00000000}", ciot.Veiculo.RNTRC) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = (ciot.Veiculo.TipoDoVeiculo != null && !string.IsNullOrWhiteSpace(ciot.Veiculo.TipoDoVeiculo.CodigoIntegracao)) ? ciot.Veiculo.TipoDoVeiculo.CodigoIntegracao : "4" });

            for (var i = 0; i < ciot.Veiculo.VeiculosVinculados.Count(); i++)
            {
                string posicao = (2 + i).ToString();

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo" + posicao + ".placa", value = ciot.Veiculo.VeiculosVinculados[i].Placa });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo" + posicao + ".rntrc", value = string.Format("{0:00000000}", ciot.Veiculo.VeiculosVinculados[i].RNTRC) });
            }

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterDocumentos(List<Dominio.Entidades.CTeCIOTSigaFacil> ctes, System.Globalization.CultureInfo cultura)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.qtde", value = ctes.Count.ToString() });
            string prefixo = "";
            for (var i = 0; i < ctes.Count; i++)
            {
                prefixo = "viagem.documento" + (i + 1).ToString() + ".";
                Dominio.Entidades.CTeCIOTSigaFacil cte = ctes[i];

                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "tipo", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "numero", value = cte.CTe.Numero.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "serie", value = cte.CTe.Serie.Numero.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "quantidade", value = cte.QuantidadeMercadoria.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "especie", value = cte.EspecieMercadoria });
                //campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "cubagem", value = "" });
                //campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "natureza", value = "" });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "peso", value = cte.PesoBruto.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "mercadoria.valor", value = cte.ValorTotalMercadoria.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "pessoafiscal.qtde", value = "2" });

                campos.AddRange(this.ObterPessoaDocumento(i, 1, "1", cte.CTe.Remetente));
                campos.AddRange(this.ObterPessoaDocumento(i, 2, "2", cte.CTe.Destinatario));
            }

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterPessoaDocumento(int indiceDocumento, int indicePessoa, string tipo, Dominio.Entidades.ParticipanteCTe participante)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".tipo", value = tipo });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".codigo", value = "" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".documento.tipo", value = participante.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".documento.numero", value = participante.CPF_CNPJ });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".nome", value = Utilidades.String.Left(participante.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.logradouro", value = Utilidades.String.Left(participante.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.numero", value = Utilidades.String.Left(participante.Numero, 5) });

            if (!string.IsNullOrWhiteSpace(participante.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.complemento", value = Utilidades.String.Left(participante.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.bairro", value = Utilidades.String.Left(participante.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.cidade.ibge", value = string.Format("{0:0000000}", participante.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + indicePessoa.ToString() + ".endereco.cep", value = Utilidades.String.OnlyNumbers(participante.CEP) });

            return campos;
        }

        private ServicoPamCard.WSTransacionalClient ObterClientPamCard(Dominio.Entidades.CIOTSigaFacil ciot, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior)
        {
            ServicoPamCard.WSTransacionalClient svcPamCard = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoPamCard.WSTransacionalClient, ServicoPamCard.WSTransacional>(TipoWebServiceIntegracao.Pamcard_WSTransacional, out inspectorBehavior);

            svcPamCard.ClientCredentials.ClientCertificate.Certificate = new X509Certificate2(ciot.Empresa.NomeCertificado, ciot.Empresa.SenhaCertificado, X509KeyStorageFlags.MachineKeySet);

            return svcPamCard;
        }

        private void AdicionaLog(Servicos.Models.Integracao.InspectorBehavior inspector, Dominio.Entidades.CIOTSigaFacil ciot)
        {
            Repositorio.CIOTSigaFacilLogXML repCIOTSigaFacilLogXML = new Repositorio.CIOTSigaFacilLogXML(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacilLogXML log = new Dominio.Entidades.CIOTSigaFacilLogXML()
            {
                CIOT = ciot,
                DataHora = DateTime.Now,
                Requisicao = inspector.LastRequestXML,
                Resposta = inspector.LastResponseXML
            };

            repCIOTSigaFacilLogXML.Inserir(log);
        }

        private List<Dominio.Entidades.CTeCIOTSigaFacil> CTEAbertura(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            Dominio.Entidades.CTeCIOTSigaFacil cte = new Dominio.Entidades.CTeCIOTSigaFacil()
            {
                CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico()
                {
                    Numero = 1,
                    Serie = new Dominio.Entidades.EmpresaSerie() { Numero = 1 },
                    Remetente = new Dominio.Entidades.ParticipanteCTe()
                    {
                        Tipo = ciot.Transportador.Tipo == "F" ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica,
                        CPF_CNPJ = ciot.Transportador.CPF_CNPJ_SemFormato,
                        Nome = ciot.Transportador.Nome,
                        Endereco = ciot.Transportador.Endereco,
                        Numero = ciot.Transportador.Numero,
                        Bairro = ciot.Transportador.Bairro,
                        CEP = ciot.Transportador.CEP,
                        Localidade = ciot.Transportador.Localidade
                    },
                    Destinatario = new Dominio.Entidades.ParticipanteCTe()
                    {
                        Tipo = ciot.Transportador.Tipo == "F" ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica,
                        CPF_CNPJ = ciot.Transportador.CPF_CNPJ_SemFormato,
                        Nome = ciot.Transportador.Nome,
                        Endereco = ciot.Transportador.Endereco,
                        Numero = ciot.Transportador.Numero,
                        Bairro = ciot.Transportador.Bairro,
                        CEP = ciot.Transportador.CEP,
                        Localidade = ciot.Transportador.Localidade
                    }
                },
                QuantidadeMercadoria = 1,
                EspecieMercadoria = "UN",
                PesoBruto = 10,
                ValorTotalMercadoria = 1,
                ValorPedagio = 10,
                ValorAdiantamento = 10,
                ValorAbastecimento = 10,
                ValorFrete = 10
            };

            return new List<Dominio.Entidades.CTeCIOTSigaFacil>() { cte };
        }
        
        #endregion
    }
}
