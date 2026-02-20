using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos
{
    public class CIOT : ServicoBase
    {
        public CIOT(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public void Emitir(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            switch (ciot.TipoIntegradora)
            {
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil:
                    SigaFacil svcSigaFacil = new SigaFacil(unidadeDeTrabalho);
                    svcSigaFacil.EmitirCIOT(codigoCIOT, unidadeDeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCard:
                    PamCard svcPamCard = new PamCard(unidadeDeTrabalho);
                    svcPamCard.EmitirCIOT(codigoCIOT);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura:
                    PamCard svcPamCardAbertura = new PamCard(unidadeDeTrabalho);
                    svcPamCardAbertura.EmitirCIOTAbertura(codigoCIOT);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete:
                    EFrete svcEFrete = new EFrete(unidadeDeTrabalho);
                    svcEFrete.EmitirCIOT(codigoCIOT);
                    break;
                default:
                    throw new Exception("Emissão para a integradora não implementada.");
            }

            this.SalvarMovimentoDoFinanceiro(codigoCIOT, unidadeDeTrabalho);
        }

        public void Salvar(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            switch (ciot.TipoIntegradora)
            {
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura:
                    PamCard svcPamCardAbertura = new PamCard(unidadeDeTrabalho);
                    svcPamCardAbertura.AtualizarParcelasCIOT(codigoCIOT);
                    break;
                default:
                    throw new Exception("Emissão para a integradora não implementada.");
            }

            this.SalvarMovimentoDoFinanceiro(codigoCIOT, unidadeDeTrabalho);
        }

        public void Cancelar(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            switch (ciot.TipoIntegradora)
            {
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil:
                    SigaFacil svcSigaFacil = new SigaFacil(unidadeDeTrabalho);
                    svcSigaFacil.CancelarCIOT(codigoCIOT, unidadeDeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCard:
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura:
                    PamCard svcPamCard = new PamCard(unidadeDeTrabalho);
                    svcPamCard.CancelarCIOT(codigoCIOT);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete:
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura:
                    EFrete svcEFrete = new EFrete(unidadeDeTrabalho);
                    svcEFrete.CancelarCIOT(codigoCIOT);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad:
                    TruckPad svcTruckPad = new TruckPad(unidadeDeTrabalho);
                    svcTruckPad.CancelarCIOTAberto(codigoCIOT, unidadeDeTrabalho);
                    break;
                default:
                    throw new Exception("Emissão para a integradora não implementada.");
            }

            this.SalvarMovimentoDoFinanceiro(codigoCIOT, unidadeDeTrabalho);
        }

        public void Encerrar(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            switch (ciot.TipoIntegradora)
            {
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete:
                    EFrete svcEFrete = new EFrete(unidadeDeTrabalho);
                    svcEFrete.EncerrarCIOT(codigoCIOT);
                    break;
                case Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura:
                    PamCard PamCardAbertura = new PamCard(unidadeDeTrabalho);
                    PamCardAbertura.EncerrarCIOT(codigoCIOT);
                    break;
                default:
                    throw new Exception("Encerramento não implementado para a integradora.");
            }
        }

        public object GerarCIOTPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, double cnpjTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            Dominio.Entidades.CIOTSigaFacil ciot = null;

            ciot = new Dominio.Entidades.CIOTSigaFacil();

            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
            Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

            Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente();

            ciot.TipoIntegradora = cte.Empresa.Configuracao.TipoIntegradoraCIOT.Value;

            ciot.CategoriaTransportador = Dominio.Enumeradores.CategoriaTransportadorANTT.NaoTAC;
            //ciot.NaturezaCarga = repNaturezaCargaANTT.BuscarPorCodigo(codigoNaturezaCarga);
            ciot.DataEmissao = DateTime.Now;
            ciot.DataInicioViagem = DateTime.Today;
            ciot.DataTerminoViagem = DateTime.Today;
            ciot.Destino = cte.LocalidadeTerminoPrestacao;
            ciot.DocumentosObrigatorios = Dominio.Enumeradores.DocumentosObrigatorios.ExigeCTRC;
            ciot.Empresa = cte.Empresa;
            if (cte.Motoristas != null && cte.Motoristas.FirstOrDefault().CPFMotorista != "")
                ciot.Motorista = repUsuario.BuscarPorCPF(cte.Empresa.Codigo, cte.Motoristas.FirstOrDefault().CPFMotorista, "M");
            if (ciot.Motorista != null)
                ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;
            ciot.Origem = cte.LocalidadeInicioPrestacao;
            ciot.RegraAdiantamento = Dominio.Enumeradores.RegraQuitacaoAdiantamento.Posto;
            ciot.RegraQuitacao = Dominio.Enumeradores.RegraQuitacaoQuitacao.Posto;
            ciot.TipoViagem = Dominio.Enumeradores.TipoViagem.PorPeso;
            if (cnpjTransportador != 0)
                ciot.Transportador = repCliente.BuscarPorCPFCNPJ(cnpjTransportador);
            if (cte.Veiculos != null && cte.Veiculos.FirstOrDefault().Placa != "")
                ciot.Veiculo = repVeiculo.BuscarPorPlaca(cte.Veiculos.FirstOrDefault().Placa);
            if (cte.Empresa.Configuracao.TipoPagamentoCIOT != null)
                ciot.TipoPagamento = cte.Empresa.Configuracao.TipoPagamentoCIOT.Value;

            if (ciot.Transportador != null)
            {
                dadosCliente = repDadosCliente.Buscar(cte.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;
            }
            ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

            ciot.Numero = repCIOT.BuscarUltimoNumero(cte.Empresa.Codigo) + 1;

            repCIOT.Inserir(ciot);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentos = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            documentos.Add(cte);

            if (ciot.Motorista != null)
                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);
            this.SalvarDocumentosDoCIOT(ciot, documentos, unidadeDeTrabalho);

            return ciot;

        }

        public Dominio.Entidades.CIOTSigaFacil GerarCIOT(Dominio.ObjetosDeValor.CIOT.CIOT ciotIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
            Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente();

            Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa = !String.IsNullOrWhiteSpace(empresa.Configuracao?.CodigoIntegradorEFrete) ? empresa.Configuracao : empresa.EmpresaPai.Configuracao;
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(0, ciotIntegracao.CodigosCTes);


            Dominio.Entidades.CIOTSigaFacil ciot = new Dominio.Entidades.CIOTSigaFacil();
            ciot.TipoIntegradora = configuracaoEmpresa.TipoIntegradoraCIOT.Value;
            ciot.TipoPagamento = ciotIntegracao.TipoPagamento.Value;
            ciot.CategoriaTransportador = ciotIntegracao.CategoriaTransportadorANTT.HasValue ? ciotIntegracao.CategoriaTransportadorANTT.Value : Dominio.Enumeradores.CategoriaTransportadorANTT.NaoTAC;
            ciot.NaturezaCarga = repNaturezaCargaANTT.BuscarPorNatureza(ciotIntegracao.NaturezaCarga);
            ciot.DocumentosObrigatorios = Dominio.Enumeradores.DocumentosObrigatorios.ExigeCTRC;
            ciot.Empresa = empresa;
            ciot.RegraAdiantamento = ciotIntegracao.RegraQuitacaoAdiantamento.HasValue ? ciotIntegracao.RegraQuitacaoAdiantamento.Value : Dominio.Enumeradores.RegraQuitacaoAdiantamento.Filial;
            ciot.RegraQuitacao = ciotIntegracao.RegraQuitacaoQuitacao.HasValue ? ciotIntegracao.RegraQuitacaoQuitacao.Value : Dominio.Enumeradores.RegraQuitacaoQuitacao.Filial;
            ciot.TipoViagem = Dominio.Enumeradores.TipoViagem.PorPeso;

            ciot.DataEmissao = DateTime.Now;
            DateTime dataInicio;
            if (!DateTime.TryParseExact(ciotIntegracao.DataInicio, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicio))
                if (!DateTime.TryParseExact(ciotIntegracao.DataInicio, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataInicio))
                    if (!DateTime.TryParseExact(ciotIntegracao.DataInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio))
                        dataInicio = DateTime.Now;
            ciot.DataInicioViagem = dataInicio;
            DateTime dataFim;
            if (!DateTime.TryParseExact(ciotIntegracao.DataFim, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFim))
                if (!DateTime.TryParseExact(ciotIntegracao.DataFim, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataFim))
                    if (!DateTime.TryParseExact(ciotIntegracao.DataFim, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim))
                        dataFim = DateTime.Now;
            ciot.DataTerminoViagem = dataFim;
            ciot.Origem = ctes.FirstOrDefault().LocalidadeInicioPrestacao;
            ciot.Destino = ctes.FirstOrDefault().LocalidadeTerminoPrestacao;

            //ciotIntegracao.TipoViagemANTT
            double.TryParse(empresa.CNPJ, out double cnpjEmpresa);

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjEmpresa);
            if (cliente == null)
            {
                cliente = Servicos.Embarcador.Pessoa.Pessoa.Converter(empresa, unidadeDeTrabalho);
                repCliente.Inserir(cliente);
            }
            ciot.Transportador = cliente;
            if (ciotIntegracao.Veiculo != null)
                ciot.Veiculo = repVeiculo.BuscarPorPlaca(ciotIntegracao.Veiculo.Placa);
            if (ciotIntegracao.Motorista != null)
                ciot.Motorista = repUsuario.BuscarPorCPF(empresa.Codigo, ciotIntegracao.Motorista.CPF, "M");
            if (ciot.Motorista != null)
                ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;

            if (ciot.Transportador != null)
            {
                dadosCliente = repDadosCliente.Buscar(empresa.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;
            }
            ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;
            ciot.Numero = repCIOT.BuscarUltimoNumero(empresa.Codigo) + 1;
            repCIOT.Inserir(ciot);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentos = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                documentos.Add(cte);

            if (ciot.Motorista != null)
                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);
            this.SalvarDocumentosDoCIOT(ciot, documentos, unidadeDeTrabalho);

            return ciot;
        }

        public void VincularCTeCIOTEFrete(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT[] tipoAbertura = new Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT[] {
                    Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura,
                    Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura
                };
                if (!string.IsNullOrWhiteSpace(cte.CIOT) && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.TipoIntegradoraCIOT.HasValue && tipoAbertura.Contains(cte.Empresa.Configuracao.TipoIntegradoraCIOT.Value))
                {
                    Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorNumero(cte.CIOT, cte.Empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto);

                    if (ciot != null)
                    {
                        Dominio.Entidades.CTeCIOTSigaFacil CTeCIOT = new Dominio.Entidades.CTeCIOTSigaFacil();

                        string numeroNotaFiscal = repDocumentosCTe.BuscarNumeroNotaFiscal(cte.Codigo);
                        //string unidade = repInformacaoCarga.ObterUnidade(cte.Codigo);
                        //decimal peso = repInformacaoCarga.ObterPesoTotal(cte.Codigo);
                        //string especie = "";

                        //switch (unidade)
                        //{
                        //    case "00":
                        //        especie = "M3";
                        //        break;
                        //    case "01":
                        //        especie = "KG";
                        //        break;
                        //    case "02":
                        //        especie = "TO";
                        //        break;
                        //    case "03":
                        //        especie = "UN";
                        //        break;
                        //    case "04":
                        //        especie = "LT";
                        //        break;
                        //    default:
                        //        especie = "KG";
                        //        break;
                        //}

                        string especie = string.Empty;
                        decimal peso = repInformacaoCarga.ObterPesoKg(cte.Codigo) > 0 ? repInformacaoCarga.ObterPesoKg(cte.Codigo) : 0;//
                        if (peso > 0)
                            especie = "KG";
                        else
                        {
                            peso = repInformacaoCarga.ObterPesoTotal(cte.Codigo);
                            especie = this.PegaEspeciaPorUnidade(cte.Codigo, unidadeDeTrabalho);
                        }

                        CTeCIOT.CTe = cte;
                        CTeCIOT.CIOT = ciot;
                        CTeCIOT.EspecieMercadoria = especie;
                        CTeCIOT.NumeroNotaFiscal = string.IsNullOrWhiteSpace(numeroNotaFiscal) ? 0 : int.Parse(numeroNotaFiscal);
                        CTeCIOT.PercentualTolerancia = 0;
                        CTeCIOT.PercentualToleranciaSuperior = 0;
                        CTeCIOT.PesoBruto = peso;
                        CTeCIOT.PesoLotacao = 0;
                        CTeCIOT.QuantidadeMercadoria = (int)Math.Round(peso, 0, MidpointRounding.ToEven);
                        CTeCIOT.RecalculoFrete = 0;
                        CTeCIOT.TipoPeso = 0;
                        CTeCIOT.TipoQuebra = 0;
                        CTeCIOT.TipoTolerancia = 0;
                        CTeCIOT.ValorAdiantamento = 0;
                        CTeCIOT.ValorCartaoPedagio = 0;
                        CTeCIOT.ValorFrete = cte.ValorAReceber;
                        CTeCIOT.ValorINSS = 0;
                        CTeCIOT.ValorIRRF = 0;
                        CTeCIOT.ValorTotalMercadoria = cte.ValorTotalMercadoria;
                        CTeCIOT.ValorMercadoriaKG = peso > 0 ? (cte.ValorTotalMercadoria / peso) : 0;
                        CTeCIOT.ValorOutrosDescontos = 0;
                        CTeCIOT.ValorPedagio = 0;
                        CTeCIOT.ValorAbastecimento = 0;
                        CTeCIOT.ValorSeguro = 0;
                        CTeCIOT.ValorSENAT = 0;
                        CTeCIOT.ValorSEST = 0;
                        CTeCIOT.ValorTarifaEmissaoCartao = 0;
                        CTeCIOT.ValorTarifaFrete = 0;
                        CTeCIOT.ValorTotalMercadoria = 0;

                        repCTeCIOT.Inserir(CTeCIOT);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao vincular CTe ao CIOT: " + ex);
            }
        }

        public void DesvincularCTeCIOTEFrete(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (!string.IsNullOrWhiteSpace(cte.CIOT) && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura)
                {
                    Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorNumero(cte.CIOT, cte.Empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto);

                    if (ciot != null)
                    {
                        Dominio.Entidades.CTeCIOTSigaFacil CTeCIOT = repCTeCIOT.BuscarCTePorCIOT(ciot.Codigo, cte.Codigo);

                        if (CTeCIOT != null)
                            repCTeCIOT.Deletar(CTeCIOT);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao desvincular CTe ao CIOT: " + ex);
            }
        }

        #region Métodos Privados

        private string PegaEspeciaPorUnidade(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            string especie;
            switch (repInformacaoCarga.ObterUnidade(codigo))
            {
                case "00": especie = "M3"; break;
                case "01": especie = "KG"; break;
                case "02": especie = "TO"; break;
                case "03": especie = "UN"; break;
                case "04": especie = "LT"; break;
                default: especie = "KG"; break;
            }

            return especie;
        }

        private void SalvarMovimentoDoFinanceiro(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            if (ciot != null && ciot.Empresa.Configuracao != null && ciot.Empresa.Configuracao.PlanoPagamentoMotorista != null)
            {
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(codigoCIOT);

                if (ctes != null && ctes.Count() > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);

                    List<Dominio.Entidades.MovimentoDoFinanceiro> movimentos = repMovimento.BuscarPorCIOT(ciot.Empresa.Codigo, ciot.Codigo);

                    foreach (Dominio.Entidades.MovimentoDoFinanceiro movimento in movimentos)
                        repMovimento.Deletar(movimento);

                    double cpfCnpjMotorista = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ciot.Motorista.CPF), out cpfCnpjMotorista);

                    Dominio.Entidades.Cliente motorista = repCliente.BuscarPorCPFCNPJ(cpfCnpjMotorista);

                    if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    {
                        decimal valorAdiantamento = (from obj in ctes select obj.ValorAdiantamento).Sum();
                        decimal valorFrete = (from obj in ctes select obj.ValorFrete - obj.ValorAdiantamento - obj.ValorCartaoPedagio - obj.ValorINSS - obj.ValorIRRF - obj.ValorPedagio - obj.ValorSeguro - obj.ValorSENAT - obj.ValorSEST - obj.ValorTarifaEmissaoCartao - obj.ValorOutrosDescontos).Sum();

                        if (valorAdiantamento > 0)
                        {
                            Dominio.Entidades.MovimentoDoFinanceiro movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                            movimento.Data = ciot.DataEmissao;
                            movimento.DataPagamento = ciot.DataInicioViagem;
                            movimento.CIOTs = new List<Dominio.Entidades.CIOTSigaFacil>();
                            movimento.CIOTs.Add(ciot);
                            movimento.Empresa = ciot.Empresa;
                            movimento.Observacao = string.Concat("Referente ao adiantamento do CIOT nº ", ciot.Numero, " (", ciot.NumeroCIOT, ").");
                            movimento.Valor = valorAdiantamento;
                            movimento.Veiculo = ciot.Veiculo;
                            movimento.Pessoa = motorista;
                            movimento.PlanoDeConta = ciot.Empresa.Configuracao.PlanoPagamentoMotorista;
                            movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                            movimento.Documento = ciot.Numero + " (" + ciot.NumeroCIOT + ")";

                            repMovimento.Inserir(movimento);
                        }

                        Dominio.Entidades.MovimentoDoFinanceiro movimentoFrete = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimentoFrete.Data = ciot.DataEmissao;
                        movimentoFrete.CIOTs = new List<Dominio.Entidades.CIOTSigaFacil>();
                        movimentoFrete.CIOTs.Add(ciot);
                        movimentoFrete.Empresa = ciot.Empresa;
                        movimentoFrete.Observacao = string.Concat("Referente ao frete do CIOT nº ", ciot.Numero, " (", ciot.NumeroCIOT, ").");
                        movimentoFrete.Valor = valorFrete;
                        movimentoFrete.Veiculo = ciot.Veiculo;
                        movimentoFrete.Pessoa = motorista;
                        movimentoFrete.PlanoDeConta = ciot.Empresa.Configuracao.PlanoPagamentoMotorista;
                        movimentoFrete.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                        movimentoFrete.Documento = ciot.Numero + " (" + ciot.NumeroCIOT + ")";

                        repMovimento.Inserir(movimentoFrete);
                    }
                }
            }
        }

        private void SalvarCliente(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            double cpfCnpj = 0f;
            double.TryParse(Utilidades.String.OnlyNumbers(motorista.CPF), out cpfCnpj);

            if (cpfCnpj > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

                    cliente = new Dominio.Entidades.Cliente();
                    cliente.Atividade = repAtividade.BuscarPorCodigo(7);
                    cliente.Bairro = motorista.Bairro;
                    cliente.CEP = motorista.CEP;
                    cliente.Complemento = motorista.Complemento;
                    cliente.CPF_CNPJ = cpfCnpj;
                    cliente.DataCadastro = DateTime.Now;
                    cliente.DataNascimento = motorista.DataNascimento;
                    cliente.Email = motorista.Email;
                    cliente.Endereco = motorista.Endereco;
                    cliente.EstadoRG = motorista.EstadoRG;
                    cliente.IE_RG = motorista.RG;
                    cliente.Localidade = motorista.Localidade;
                    cliente.Nome = motorista.Nome;
                    cliente.OrgaoEmissorRG = motorista.OrgaoEmissorRG;
                    cliente.Sexo = motorista.Sexo;
                    cliente.Telefone1 = motorista.Telefone;
                    cliente.Tipo = motorista.CPF.Length == 14 ? "J" : "F";
                    cliente.Ativo = true;
                    repCliente.Inserir(cliente);
                }
            }
        }

        private void SalvarDocumentosDoCIOT(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (documentos != null)
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

                for (var i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.CTeCIOTSigaFacil documento = null;

                    documento = new Dominio.Entidades.CTeCIOTSigaFacil();

                    string numeroNotaFiscal = repDocumentosCTe.BuscarNumeroNotaFiscal(documentos[i].Codigo);

                    documento.CIOT = ciot;
                    documento.CTe = repCTe.BuscarPorCodigo(documentos[i].Codigo);
                    documento.EspecieMercadoria = "UN";
                    documento.ExigePesoChegada = Dominio.Enumeradores.ExigePesoChegada.Nao;
                    documento.NumeroNotaFiscal = string.IsNullOrWhiteSpace(numeroNotaFiscal) ? 0 : int.Parse(numeroNotaFiscal);
                    documento.PercentualTolerancia = 0m;
                    documento.PercentualToleranciaSuperior = 0m;
                    documento.PesoBruto = repInformacaoCargaCTe.ObterPesoKg(documentos[i].Codigo);
                    documento.PesoLotacao = 0;
                    documento.QuantidadeMercadoria = repInformacaoCargaCTe.ObterQuantidadeUnitaria(documentos[i].Codigo);
                    documento.RecalculoFrete = Dominio.Enumeradores.RecalculoFrete.CobraDiferenca;
                    documento.TipoPeso = Dominio.Enumeradores.TipoPeso.PesoCarregado;
                    documento.TipoQuebra = Dominio.Enumeradores.TipoQuebra.Integral;
                    documento.TipoTolerancia = Dominio.Enumeradores.TipoTolerancia.Percentual;
                    documento.ValorAdiantamento = 0m;
                    documento.ValorCartaoPedagio = 0m;
                    documento.ValorFrete = documentos[i].ValorAReceber;
                    documento.ValorINSS = 0m;
                    documento.ValorIRRF = 0m;
                    documento.ValorMercadoriaKG = 0m;
                    documento.ValorOutrosDescontos = 0m;
                    documento.ValorPedagio = 0m;
                    documento.ValorSeguro = 0m;
                    documento.ValorSENAT = 0m;
                    documento.ValorSEST = 0m;
                    documento.ValorTarifaEmissaoCartao = 0m;
                    documento.ValorTarifaFrete = 0m;
                    documento.ValorTotalMercadoria = documentos[i].ValorTotalMercadoria;

                    if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                        documento.NSU = documentos[i].Empresa.Configuracao.ProximoNSUSigaFacil;

                    repCTeCIOT.Inserir(documento);

                }
            }
        }

        #endregion
    }
}
