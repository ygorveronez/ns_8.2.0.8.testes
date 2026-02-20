using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Servicos
{
    public class LeituraEDI : ServicoBase
    {
        #region Propriedades

        private Repositorio.UnitOfWork UnitOfWork { get; set; }

        private Dominio.Entidades.LayoutEDI Layout { get; set; }

        private Dominio.Entidades.Empresa Empresa { get; set; }

        private Stream Arquivo { get; set; }

        private int TabelaFreteValor { get; set; }

        private decimal ValorFrete { get; set; }

        private decimal ValorPedagio { get; set; }

        private decimal PercentualGris { get; set; }

        private decimal PercentualAdValorem { get; set; }

        private int CodigoVeiculoTracao { get; set; }

        private int CodigoVeiculoReboque { get; set; }

        private int CodigoMotorista { get; set; }

        private bool ManterEmDigitacao { get; set; }

        private bool IncluirICMS { get; set; }

        private bool LayoutPorIndice { get; set; }

        private Encoding Encondig { get; set; }

        private Dominio.Entidades.Usuario Usuario { get; set; }

        private List<string> LinhasArquivo { get; set; }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTesGerados { get; set; }

        private List<Dominio.Entidades.OcorrenciaDeCTe> OcorrenciasGeradas { get; set; }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> CargaOcorrenciasGeradas { get; set; }

        private List<Dominio.ObjetosDeValor.EDI.Fatura> Faturas { get; set; }

        private List<Dominio.Entidades.Abastecimento> Abastecimentos { get; set; }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> NotasFiscais { get; set; }

        private Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NotFis { get; set; }

        private Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN Ocoren { get; set; }

        private Dominio.ObjetosDeValor.EDI.TransportationPlann.EDITransportationPlann TransportationPlann { get; set; }

        #endregion

        #region Construtores

        public LeituraEDI(Dominio.Entidades.Empresa empresa, Dominio.Entidades.LayoutEDI layout, Stream arquivo, Repositorio.UnitOfWork unitOfWork, int codigoTabelaFreteValor = 0, decimal valorFrete = 0, decimal valorPedagio = 0, decimal percentualGris = 0, decimal percentualAdValorem = 0, int codigoVeiculoTracao = 0, int codigoVeiculoReboque = 0, int codigoMotorista = 0, bool manterEmDigitacao = true, bool incluirICMS = true, Encoding encondig = null, Dominio.Entidades.Usuario usuario = null) : base(unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.Layout = layout;
            this.Empresa = empresa;
            this.Arquivo = arquivo;
            this.TabelaFreteValor = codigoTabelaFreteValor;

            this.LayoutPorIndice = this.Layout.CamposPorIndices;

            this.ValorFrete = valorFrete;
            this.ValorPedagio = valorPedagio;
            this.PercentualGris = percentualGris;
            this.PercentualAdValorem = percentualAdValorem;
            this.CodigoVeiculoTracao = codigoVeiculoTracao;
            this.CodigoVeiculoReboque = codigoVeiculoReboque;
            this.CodigoMotorista = codigoMotorista;
            this.ManterEmDigitacao = manterEmDigitacao;
            this.IncluirICMS = incluirICMS;
            this.Encondig = encondig;
            this.Usuario = usuario;

            this.LerArquivo();
        }

        public LeituraEDI(Dominio.Entidades.Empresa empresa, Dominio.Entidades.LayoutEDI layout, Stream arquivo, int codigoTabelaFreteValor = 0, decimal valorFrete = 0, decimal valorPedagio = 0, decimal percentualGris = 0, decimal percentualAdValorem = 0, int codigoVeiculoTracao = 0, int codigoVeiculoReboque = 0, int codigoMotorista = 0, bool manterEmDigitacao = true, bool incluirICMS = true, Encoding encondig = null, Dominio.Entidades.Usuario usuario = null, Repositorio.UnitOfWork unitOfWork = null) : base()
        {
            this.UnitOfWork = unitOfWork;
            this.Layout = layout;
            this.Empresa = empresa;
            this.Arquivo = arquivo;
            this.TabelaFreteValor = codigoTabelaFreteValor;

            this.LayoutPorIndice = this.Layout.CamposPorIndices;

            this.ValorFrete = valorFrete;
            this.ValorPedagio = valorPedagio;
            this.PercentualGris = percentualGris;
            this.PercentualAdValorem = percentualAdValorem;
            this.CodigoVeiculoTracao = codigoVeiculoTracao;
            this.CodigoVeiculoReboque = codigoVeiculoReboque;
            this.CodigoMotorista = codigoMotorista;
            this.ManterEmDigitacao = manterEmDigitacao;
            this.IncluirICMS = incluirICMS;
            this.Encondig = encondig;
            this.Usuario = usuario;

            this.LerArquivo();
        }        
        #endregion

        #region Métodos Globais

        public T LerEDI<T>()
        {
            var _class = (T)Activator.CreateInstance(typeof(T));
            int indice = 0;

            this.LerCampoRegistroRecursivo(_class, ref indice, "", "");

            return _class;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> LerNotasFiscais(bool naoInserirNotaDuplicada)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> ListaNotas = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

            List<string> LinhasBkp = this.LinhasArquivo;

            for (var i = 0; i < LinhasBkp.Count(); i++)
            {
                this.LinhasArquivo = null;
                LinhasArquivo = new List<string>();
                LinhasArquivo.Add(LinhasBkp[i]);

                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                int indice = 0;
                this.LerCampoRegistroRecursivo(notaFiscal, ref indice, "", "");

                bool inserirNota = true;
                if (naoInserirNotaDuplicada)
                    inserirNota = string.IsNullOrWhiteSpace(notaFiscal.Chave) || (from obj in ListaNotas where obj.Chave == notaFiscal.Chave select obj).Count() == 0;

                ListaNotas.Add(notaFiscal);
            }

            return ListaNotas;
        }

        public Dominio.ObjetosDeValor.EDI.RPS.RetornoNotaServico LerRetornoNotaServico()
        {
            Dominio.ObjetosDeValor.EDI.RPS.RetornoNotaServico retornoNotaServico = new Dominio.ObjetosDeValor.EDI.RPS.RetornoNotaServico();
            int indice = 0;
            this.LerCampoRegistroRecursivo(retornoNotaServico, ref indice, "", "");
            return retornoNotaServico;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> GerarCTes()
        {
            this.LerRegistrosCTe();

            if (TabelaFreteValor > 0)
                this.CalcularFreteCTe();
            else if (ValorFrete > 0 || ValorPedagio > 0 || PercentualGris > 0 || PercentualAdValorem > 0)
                this.AtualizarValoresCTe();

            if (CodigoVeiculoTracao > 0 || CodigoVeiculoReboque > 0 || CodigoMotorista > 0)
                this.AtualizarVeiculosMotorista();

            if (!ManterEmDigitacao)
                this.EmitiCTes();

            return this.CTesGerados;
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> GerarOcorrencias()
        {
            this.OcorrenciasGeradas = new List<Dominio.Entidades.OcorrenciaDeCTe>();
            this.LerRegistrosOcorencia();
            return this.OcorrenciasGeradas;
        }
        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> GerarCargaOcorrencias(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia)
        {
            this.CargaOcorrenciasGeradas = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            this.LerRegistrosCargaOcorrencia(tipoOcorrencia);
            return this.CargaOcorrenciasGeradas;
        }
        public List<Dominio.ObjetosDeValor.EDI.Fatura> GerarFaturasNatura()
        {
            this.Faturas = new List<Dominio.ObjetosDeValor.EDI.Fatura>();

            this.LerRegistrosFaturaNatura();

            return this.Faturas;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento GerarAbastecimentos()
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = new Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento();

            retornoAbastecimento.Abastecimentos = new List<Dominio.Entidades.Abastecimento>();
            retornoAbastecimento.MsgRetorno = "";
            this.Abastecimentos = new List<Dominio.Entidades.Abastecimento>();

            retornoAbastecimento.MsgRetorno = this.LerRegistrosAbastecimentos();
            retornoAbastecimento.Abastecimentos = this.Abastecimentos;

            return retornoAbastecimento;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> GerarNotasFiscais()
        {
            this.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            this.LerRegistrosNotasFiscais();
            return this.NotasFiscais;
        }

        public Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN GerarOcoren()
        {
            this.Ocoren = new Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN();
            int indice = 0;
            this.LerCampoRegistroRecursivo(this.Ocoren, ref indice, "", "");
            return this.Ocoren;
        }

        public Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis GerarNotasFis()
        {
            this.NotFis = new Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis();
            int indice = 0;
            this.LerCampoRegistroRecursivo(this.NotFis, ref indice, "", "");
            return this.NotFis;
        }

        public Dominio.ObjetosDeValor.EDI.TransportationPlann.EDITransportationPlann GerarTransportationPlann()
        {
            this.TransportationPlann = new Dominio.ObjetosDeValor.EDI.TransportationPlann.EDITransportationPlann();
            int indice = 0;
            this.LerCampoRegistroRecursivo(this.TransportationPlann, ref indice, "", "");
            return this.TransportationPlann;
        }

        public Dominio.ObjetosDeValor.EDI.Pedido.Pedido LerPedido()
        {
            Dominio.ObjetosDeValor.EDI.Pedido.Pedido pedido = new Dominio.ObjetosDeValor.EDI.Pedido.Pedido();
            int indice = 0;
            this.LerCampoRegistroRecursivo(pedido, ref indice, "", "");
            return pedido;
        }

        public Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio LerPREFAT()
        {
            Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio prefat = new Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio();
            int indice = 0;
            this.LerCampoRegistroRecursivo(prefat, ref indice, "", "");
            return prefat;
        }

        public void CalcularFreteCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
                Repositorio.FretePorValor repFretePorValor = new Repositorio.FretePorValor(unidadeDeTrabalho);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                Dominio.Entidades.FretePorValor fretePorValor = repFretePorValor.BuscarPorCodigo(this.Empresa.Codigo, this.TabelaFreteValor);

                if (fretePorValor != null)
                {
                    decimal valorTotalNFes, valorSobreNF, valorPedagio;
                    valorTotalNFes = this.CTesGerados.Sum(x => x.ValorTotalMercadoria);
                    valorPedagio = 0m;
                    valorSobreNF = 0m;

                    if (fretePorValor.TipoRateio == Dominio.Enumeradores.TipoRateioTabelaFreteValor.ValorNotaFiscal)
                    {
                        for (var i = 0; i < this.CTesGerados.Count(); i++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.CTesGerados[i].Codigo);
                            cte.ValorFrete = (cte.ValorTotalMercadoria / valorTotalNFes) * fretePorValor.ValorMinimoFrete;

                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual && fretePorValor.PercentualSobreNF > 0)
                            {
                                cte.ValorFrete = cte.ValorFrete;
                                valorSobreNF = (cte.ValorTotalMercadoria * (fretePorValor.PercentualSobreNF / 100));
                            }
                            else
                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF && fretePorValor.PercentualSobreNF > 0)
                            {
                                cte.ValorFrete = (cte.ValorTotalMercadoria * (fretePorValor.PercentualSobreNF / 100));
                                valorSobreNF = 0;
                            }
                            else
                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                            {
                                cte.ValorFrete = fretePorValor.ValorMinimoFrete > cte.ValorFrete ? fretePorValor.ValorMinimoFrete : cte.ValorFrete;
                                valorSobreNF = 0;
                            }

                            valorPedagio = (cte.ValorTotalMercadoria / valorTotalNFes) * fretePorValor.ValorPedagio;

                            cte.ValorFrete = cte.ValorFrete;

                            if (cte.ValorFrete > 0)
                            {
                                List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentesCTe = repComponentePrestacao.BuscarPorCTe(cte.Codigo);
                                foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteCTe in listaComponentesCTe)
                                {
                                    if (componenteCTe.Nome == "FRETE VALOR")
                                    {
                                        componenteCTe.Valor = Math.Round(cte.ValorFrete, 2, MidpointRounding.ToEven);
                                        repComponentePrestacao.Atualizar(componenteCTe);
                                    }
                                }
                            }

                            cte.ValorPrestacaoServico = cte.ValorFrete;
                            cte.ValorAReceber = cte.ValorFrete;

                            if (valorPedagio > 0)
                            {
                                Dominio.Entidades.ComponentePrestacaoCTE componentePedagioCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                componentePedagioCTe.CTE = cte;
                                componentePedagioCTe.IncluiNaBaseDeCalculoDoICMS = true;
                                componentePedagioCTe.IncluiNoTotalAReceber = true;
                                componentePedagioCTe.Nome = "PEDAGIO";
                                componentePedagioCTe.Valor = Math.Round(valorPedagio, 2, MidpointRounding.ToEven);
                                cte.ValorPrestacaoServico += componentePedagioCTe.Valor;
                                cte.ValorAReceber += componentePedagioCTe.Valor;

                                repComponentePrestacao.Inserir(componentePedagioCTe);
                            }

                            if (valorSobreNF > 0)
                            {
                                Dominio.Entidades.ComponentePrestacaoCTE componenteAdicionalValorCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                componenteAdicionalValorCTe.CTE = cte;
                                componenteAdicionalValorCTe.IncluiNaBaseDeCalculoDoICMS = true;
                                componenteAdicionalValorCTe.IncluiNoTotalAReceber = true;
                                componenteAdicionalValorCTe.Nome = "AD VALOREM";
                                componenteAdicionalValorCTe.Valor = Math.Round(valorSobreNF, 2, MidpointRounding.ToEven);
                                cte.ValorPrestacaoServico += componenteAdicionalValorCTe.Valor;
                                cte.ValorAReceber += componenteAdicionalValorCTe.Valor;

                                repComponentePrestacao.Inserir(componenteAdicionalValorCTe);
                            }

                            if (cte.Empresa.Configuracao != null && Empresa.Configuracao.ICMSIsento)
                            {
                                cte.CST = "40";
                                svcCTe.SetarCFOPENaturezaPorTabelaDeAliquotas(ref cte, unidadeDeTrabalho);
                                svcCTe.CalcularICMS(ref cte, unidadeDeTrabalho);
                            }
                            else
                            {
                                svcCTe.CalcularICMSPorTabelaDeAliquotas(ref cte, cte.SimplesNacional, unidadeDeTrabalho);
                            }

                            if (cte.AliquotaICMS > 0)
                            {
                                cte.BaseCalculoICMS = cte.ValorAReceber;
                                cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                                if (fretePorValor.IncluiICMS == Dominio.Enumeradores.IncluiICMSFrete.Sim)
                                {
                                    cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                                    cte.PercentualICMSIncluirNoFrete = 100;

                                    cte.BaseCalculoICMS = cte.ValorAReceber / (1 - (cte.AliquotaICMS / 100));
                                    cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                                    decimal valorImpostoIncluso = Math.Round(cte.BaseCalculoICMS - cte.ValorPrestacaoServico, 2, MidpointRounding.ToEven);
                                    if (valorImpostoIncluso > 0)
                                    {
                                        Dominio.Entidades.ComponentePrestacaoCTE componenteImpostoCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                        componenteImpostoCTe.CTE = cte;
                                        componenteImpostoCTe.IncluiNaBaseDeCalculoDoICMS = false;
                                        componenteImpostoCTe.IncluiNoTotalAReceber = false;
                                        componenteImpostoCTe.Nome = "IMPOSTOS";
                                        componenteImpostoCTe.Valor = valorImpostoIncluso;

                                        repComponentePrestacao.Inserir(componenteImpostoCTe);
                                    }

                                    cte.ValorPrestacaoServico = cte.BaseCalculoICMS;
                                    cte.ValorAReceber = cte.BaseCalculoICMS;
                                }
                            }
                            repCTe.Atualizar(cte);

                            unidadeDeTrabalho.FlushAndClear();
                        }
                    }
                    else if (fretePorValor.TipoRateio == Dominio.Enumeradores.TipoRateioTabelaFreteValor.Peso)
                    {

                    }
                    else if (fretePorValor.TipoRateio == Dominio.Enumeradores.TipoRateioTabelaFreteValor.Nenhum)
                    {
                        for (var i = 0; i < this.CTesGerados.Count(); i++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.CTesGerados[i].Codigo);
                            cte.ValorFrete = fretePorValor.ValorMinimoFrete;

                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual && fretePorValor.PercentualSobreNF > 0)
                            {
                                cte.ValorFrete = cte.ValorFrete;
                                valorSobreNF = (cte.ValorTotalMercadoria * (fretePorValor.PercentualSobreNF / 100));
                            }
                            else
                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF && fretePorValor.PercentualSobreNF > 0)
                            {
                                cte.ValorFrete = (cte.ValorTotalMercadoria * (fretePorValor.PercentualSobreNF / 100));
                                valorSobreNF = 0;
                            }
                            else
                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                            {
                                cte.ValorFrete = fretePorValor.ValorMinimoFrete > cte.ValorFrete ? fretePorValor.ValorMinimoFrete : cte.ValorFrete;
                                valorSobreNF = 0;
                            }

                            valorPedagio = fretePorValor.ValorPedagio;

                            cte.ValorFrete = cte.ValorFrete;

                            if (cte.ValorFrete > 0)
                            {
                                List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentesCTe = repComponentePrestacao.BuscarPorCTe(cte.Codigo);
                                foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteCTe in listaComponentesCTe)
                                {
                                    if (componenteCTe.Nome == "FRETE VALOR")
                                    {
                                        componenteCTe.Valor = Math.Round(cte.ValorFrete, 2, MidpointRounding.ToEven);
                                        repComponentePrestacao.Atualizar(componenteCTe);
                                    }
                                }
                            }

                            cte.ValorPrestacaoServico = cte.ValorFrete;
                            cte.ValorAReceber = cte.ValorFrete;

                            if (valorPedagio > 0)
                            {
                                Dominio.Entidades.ComponentePrestacaoCTE componentePedagioCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                componentePedagioCTe.CTE = cte;
                                componentePedagioCTe.IncluiNaBaseDeCalculoDoICMS = true;
                                componentePedagioCTe.IncluiNoTotalAReceber = true;
                                componentePedagioCTe.Nome = "PEDAGIO";
                                componentePedagioCTe.Valor = Math.Round(valorPedagio, 2, MidpointRounding.ToEven);
                                cte.ValorPrestacaoServico += componentePedagioCTe.Valor;
                                cte.ValorAReceber += componentePedagioCTe.Valor;

                                repComponentePrestacao.Inserir(componentePedagioCTe);
                            }

                            if (valorSobreNF > 0)
                            {
                                Dominio.Entidades.ComponentePrestacaoCTE componenteAdicionalValorCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                componenteAdicionalValorCTe.CTE = cte;
                                componenteAdicionalValorCTe.IncluiNaBaseDeCalculoDoICMS = true;
                                componenteAdicionalValorCTe.IncluiNoTotalAReceber = true;
                                componenteAdicionalValorCTe.Nome = "AD VALOREM";
                                componenteAdicionalValorCTe.Valor = Math.Round(valorSobreNF, 2, MidpointRounding.ToEven);
                                cte.ValorPrestacaoServico += componenteAdicionalValorCTe.Valor;
                                cte.ValorAReceber += componenteAdicionalValorCTe.Valor;

                                repComponentePrestacao.Inserir(componenteAdicionalValorCTe);
                            }

                            if (cte.Empresa.Configuracao != null && Empresa.Configuracao.ICMSIsento)
                            {
                                cte.CST = "40";
                                svcCTe.SetarCFOPENaturezaPorTabelaDeAliquotas(ref cte, unidadeDeTrabalho);
                                svcCTe.CalcularICMS(ref cte, unidadeDeTrabalho);
                            }
                            else
                            {
                                svcCTe.CalcularICMSPorTabelaDeAliquotas(ref cte, cte.SimplesNacional, unidadeDeTrabalho);
                            }

                            if (cte.AliquotaICMS > 0)
                            {
                                cte.BaseCalculoICMS = cte.ValorAReceber;
                                cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                                if (fretePorValor.IncluiICMS == Dominio.Enumeradores.IncluiICMSFrete.Sim)
                                {
                                    cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                                    cte.PercentualICMSIncluirNoFrete = 100;

                                    cte.BaseCalculoICMS = cte.ValorAReceber / (1 - (cte.AliquotaICMS / 100));
                                    cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                                    decimal valorImpostoIncluso = Math.Round(cte.BaseCalculoICMS - cte.ValorPrestacaoServico, 2, MidpointRounding.ToEven);
                                    if (valorImpostoIncluso > 0)
                                    {
                                        Dominio.Entidades.ComponentePrestacaoCTE componenteImpostoCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                                        componenteImpostoCTe.CTE = cte;
                                        componenteImpostoCTe.IncluiNaBaseDeCalculoDoICMS = false;
                                        componenteImpostoCTe.IncluiNoTotalAReceber = false;
                                        componenteImpostoCTe.Nome = "IMPOSTOS";
                                        componenteImpostoCTe.Valor = valorImpostoIncluso;

                                        repComponentePrestacao.Inserir(componenteImpostoCTe);
                                    }

                                    cte.ValorPrestacaoServico = cte.BaseCalculoICMS;
                                    cte.ValorAReceber = cte.BaseCalculoICMS;
                                }
                            }
                            repCTe.Atualizar(cte);

                            unidadeDeTrabalho.FlushAndClear();
                        }
                    }
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public void AtualizarValoresCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                decimal quantidadeCTes = 0;

                quantidadeCTes = this.CTesGerados.Count();

                for (var i = 0; i < this.CTesGerados.Count(); i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.CTesGerados[i].Codigo);

                    decimal valorFreteCTe = 0;
                    decimal valorPedagioCTe = 0;
                    decimal valorGrisCTe = 0;
                    decimal valorAdValoremCTe = 0;

                    if (ValorFrete > 0)
                    {
                        valorFreteCTe = ValorFrete / quantidadeCTes;
                        valorFreteCTe = Math.Round(valorFreteCTe, 2, MidpointRounding.ToEven);
                    }

                    if (ValorPedagio > 0)
                    {
                        valorPedagioCTe = ValorPedagio / quantidadeCTes;
                        valorPedagioCTe = Math.Round(valorPedagioCTe, 2, MidpointRounding.ToEven);
                    }

                    if (PercentualGris > 0)
                    {
                        valorGrisCTe = (cte.ValorTotalMercadoria * (PercentualGris / 100));
                        valorGrisCTe = Math.Round(valorGrisCTe, 2, MidpointRounding.ToEven);
                    }

                    if (PercentualAdValorem > 0)
                    {
                        valorAdValoremCTe = (cte.ValorTotalMercadoria * (PercentualAdValorem / 100));
                        valorAdValoremCTe = Math.Round(valorAdValoremCTe, 2, MidpointRounding.ToEven);
                    }

                    if (valorFreteCTe > 0)
                    {
                        cte.ValorFrete = valorFreteCTe;

                        bool inseriuComponente = false;
                        List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentesCTe = repComponentePrestacao.BuscarPorCTe(cte.Codigo);
                        foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteCTe in listaComponentesCTe)
                        {
                            if (componenteCTe.Nome == "FRETE VALOR")
                            {
                                componenteCTe.Valor = Math.Round(cte.ValorFrete, 2, MidpointRounding.ToEven);
                                cte.ValorPrestacaoServico = cte.ValorFrete;
                                cte.ValorAReceber = cte.ValorFrete;

                                repComponentePrestacao.Atualizar(componenteCTe);
                                inseriuComponente = true;
                            }
                        }
                        if (!inseriuComponente)
                        {
                            Dominio.Entidades.ComponentePrestacaoCTE componenteFreteValor = new Dominio.Entidades.ComponentePrestacaoCTE();
                            componenteFreteValor.CTE = cte;
                            componenteFreteValor.IncluiNaBaseDeCalculoDoICMS = true;
                            componenteFreteValor.IncluiNoTotalAReceber = true;
                            componenteFreteValor.Nome = "FRETE VALOR";
                            componenteFreteValor.Valor = cte.ValorFrete;
                            cte.ValorPrestacaoServico = cte.ValorFrete;
                            cte.ValorAReceber = cte.ValorFrete;

                            repComponentePrestacao.Inserir(componenteFreteValor);
                        }
                    }

                    if (valorPedagioCTe > 0)
                    {
                        Dominio.Entidades.ComponentePrestacaoCTE componentePedagioCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                        componentePedagioCTe.CTE = cte;
                        componentePedagioCTe.IncluiNaBaseDeCalculoDoICMS = true;
                        componentePedagioCTe.IncluiNoTotalAReceber = true;
                        componentePedagioCTe.Nome = "PEDAGIO";
                        componentePedagioCTe.Valor = valorPedagioCTe;
                        cte.ValorPrestacaoServico += componentePedagioCTe.Valor;
                        cte.ValorAReceber += componentePedagioCTe.Valor;

                        repComponentePrestacao.Inserir(componentePedagioCTe);
                    }

                    if (valorGrisCTe > 0)
                    {
                        Dominio.Entidades.ComponentePrestacaoCTE componenteGris = new Dominio.Entidades.ComponentePrestacaoCTE();
                        componenteGris.CTE = cte;
                        componenteGris.IncluiNaBaseDeCalculoDoICMS = true;
                        componenteGris.IncluiNoTotalAReceber = true;
                        componenteGris.Nome = "GRIS";
                        componenteGris.Valor = valorGrisCTe;
                        cte.ValorPrestacaoServico += componenteGris.Valor;
                        cte.ValorAReceber += componenteGris.Valor;

                        repComponentePrestacao.Inserir(componenteGris);
                    }

                    if (valorAdValoremCTe > 0)
                    {
                        Dominio.Entidades.ComponentePrestacaoCTE componenteAdValorem = new Dominio.Entidades.ComponentePrestacaoCTE();
                        componenteAdValorem.CTE = cte;
                        componenteAdValorem.IncluiNaBaseDeCalculoDoICMS = true;
                        componenteAdValorem.IncluiNoTotalAReceber = true;
                        componenteAdValorem.Nome = "AD VALOREM";
                        componenteAdValorem.Valor = valorAdValoremCTe;
                        cte.ValorPrestacaoServico += componenteAdValorem.Valor;
                        cte.ValorAReceber += componenteAdValorem.Valor;

                        repComponentePrestacao.Inserir(componenteAdValorem);
                    }

                    if (cte.Empresa.Configuracao != null && Empresa.Configuracao.ICMSIsento)
                    {
                        cte.CST = "40";
                        svcCTe.SetarCFOPENaturezaPorTabelaDeAliquotas(ref cte, unidadeDeTrabalho);
                        svcCTe.CalcularICMS(ref cte, unidadeDeTrabalho);
                    }
                    else
                    {
                        if (IncluirICMS)
                        {
                            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                            cte.PercentualICMSIncluirNoFrete = 100;
                        }
                        else
                        {
                            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
                        }
                        svcCTe.CalcularICMSPorTabelaDeAliquotas(ref cte, cte.SimplesNacional, unidadeDeTrabalho);
                    }

                    repCTe.Atualizar(cte);

                    unidadeDeTrabalho.FlushAndClear();
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public void AtualizarVeiculosMotorista()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.VeiculoCTE repVeiculoCTE = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                Repositorio.MotoristaCTE repMotoristaCTE = new Repositorio.MotoristaCTE(unidadeDeTrabalho);


                for (var i = 0; i < this.CTesGerados.Count(); i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.CTesGerados[i].Codigo);

                    Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorCodigo(CodigoVeiculoTracao);
                    Dominio.Entidades.Veiculo veiculoReboque = repVeiculo.BuscarPorCodigo(CodigoVeiculoReboque);
                    Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(CodigoMotorista);

                    if (veiculoTracao != null || veiculoReboque != null)
                    {
                        List<Dominio.Entidades.VeiculoCTE> listaVeiculos = repVeiculoCTE.BuscarPorCTe(cte.Codigo);
                        if (listaVeiculos.Count > 0)
                        {
                            foreach (Dominio.Entidades.VeiculoCTE veiculo in listaVeiculos)
                                repVeiculoCTE.Deletar(veiculo);
                        }

                        if (veiculoTracao != null)
                        {
                            Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();
                            veiculoCTe.CTE = cte;
                            veiculoCTe.Veiculo = veiculoTracao;
                            veiculoCTe.SetarDadosVeiculo(veiculoTracao);
                            repVeiculoCTE.Inserir(veiculoCTe);
                        }

                        if (veiculoReboque != null)
                        {
                            Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();
                            veiculoCTe.CTE = cte;
                            veiculoCTe.Veiculo = veiculoReboque;
                            veiculoCTe.SetarDadosVeiculo(veiculoReboque);
                            repVeiculoCTE.Inserir(veiculoCTe);
                        }

                        cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;
                        repCTe.Atualizar(cte);
                    }

                    if (motorista != null)
                    {
                        List<Dominio.Entidades.MotoristaCTE> listaMotoristas = repMotoristaCTE.BuscarPorCTe(cte.Codigo);
                        if (listaMotoristas.Count > 0)
                        {
                            foreach (Dominio.Entidades.MotoristaCTE motoristaCte in listaMotoristas)
                                repMotoristaCTE.Deletar(motoristaCte);
                        }

                        Dominio.Entidades.MotoristaCTE motoristaCTE = new Dominio.Entidades.MotoristaCTE();
                        motoristaCTE.CTE = cte;
                        motoristaCTE.CPFMotorista = motorista.CPF;
                        motoristaCTE.NomeMotorista = motorista.Nome;
                        repMotoristaCTE.Inserir(motoristaCTE);
                    }

                    unidadeDeTrabalho.FlushAndClear();
                }

            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public void EmitiCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                for (var i = 0; i < this.CTesGerados.Count(); i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.CTesGerados[i].Codigo);

                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                    if (svcCTe.Emitir(cte.Codigo))
                    {
                        if (!svcCTe.AdicionarCTeNaFilaDeConsulta(cte, unidadeDeTrabalho))
                        {
                            cte.Status = "R";
                            cte.MensagemStatus = null;
                            cte.MensagemRetornoSefaz = "Sefaz indisponível, favor aguardar e emitir CTe novamente.";
                            repCTe.Atualizar(cte);
                        }
                    }
                    unidadeDeTrabalho.FlushAndClear();
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void LerArquivo()
        {
            this.LinhasArquivo = new List<string>();
            System.Text.Encoding encoding = this.Encondig;

            if (encoding == null)
                encoding = Servicos.Arquivo.GetEncodingOrDefault(this.Arquivo);
            else
                this.Arquivo.Position = 0;

            StreamReader sr = new StreamReader(this.Arquivo, encoding);


            while (!sr.EndOfStream)
                this.LinhasArquivo.Add(sr.ReadLine());

            LinhasArquivo.RemoveAll(obj => string.IsNullOrWhiteSpace(obj));

        }

        private void LerCampoRegistroDinamico(dynamic data, dynamic lista, ref int indice)
        {
            for (int i = indice; i < this.LinhasArquivo.Count(); i++)
            {
                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();
                if (campos.Count() > 0)
                {

                }
            }
        }

        private void GetInstacia(object target, string propertyName)
        {
            object obj = target;
            PropertyInfo propertyToGet = target.GetType().GetProperty(propertyName);
            target = propertyToGet.GetValue(target, null);
            if (target == null)
                target = Activator.CreateInstance(propertyToGet.PropertyType);
            PropertyInfo propertyToSet = obj.GetType().GetProperty(propertyName);
            propertyToSet.SetValue(obj, target, null);
        }

        private dynamic LerCampoRecursivoObjeto(Type type, dynamic data, dynamic campos, string linha)
        {
            if (data == null)
                data = Activator.CreateInstance(type);

            List<string> linhaExplodidada = new List<string>();
            if (this.LayoutPorIndice)
                linhaExplodidada = linha.Split(this.Layout.Separador[0]).ToList();

            int posicao = 0;
            foreach (var campo in campos)
            {
                if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                {
                    var prop = campo.PropriedadeObjeto;
                    object valor = null;

                    if (this.LayoutPorIndice)
                    {
                        int indiceCampo = (int)campo.Indice;
                        string textoIndiceExplosao = string.Empty;

                        if (indiceCampo < linhaExplodidada.Count)
                            textoIndiceExplosao = linhaExplodidada[indiceCampo];

                        valor = this.LerCampo(campo, textoIndiceExplosao);
                    }
                    else
                    {
                        int somaTamanhoCampos = (int)campo.QuantidadeCaracteres + (int)campo.QuantidadeDecimais + (int)campo.QuantidadeInteiros;
                        string substLinha = string.Empty;

                        if (linha.Length >= (posicao + somaTamanhoCampos))
                            substLinha = linha.Substring(posicao, somaTamanhoCampos);
                        else if (linha.Length > posicao)
                            substLinha = linha.Substring(posicao, linha.Length - posicao);

                        valor = this.LerCampo(campo, substLinha);
                    }
                    //var tamanho = posicao + campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;

                    //object valor = this.LerCampo(campo, linha.Length >= tamanho ? linha.Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)) : linha.Length > posicao ? linha.Substring(posicao, linha.Length - posicao) : string.Empty);
                    this.SetNestedPropertyValueRecursive(data, campo.PropriedadeObjeto, valor, campo.Mascara);
                }
                if (!this.LayoutPorIndice)
                    posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
            }
            return data;
        }

        private void LerCampoRegistroRecursivo(dynamic data, ref int indice, string propriedade, string identficardorRegistro, string registroRecursivo = "")
        {
            string campoErro = string.Empty;
            try
            {
                campoErro = propriedade;
                RuntimeHelpers.EnsureSufficientExecutionStack();

                if (indice == this.LinhasArquivo.Count || this.LinhasArquivo[indice].Trim().Count() == 0)
                    return;

                int index = indice;
                var campos = (from obj in this.Layout.Campos where obj.Status == "A" && obj.IdentificadorRegistro.Equals(this.LinhasArquivo[index].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                if (campos.Count() > 0)
                {
                    if (campos[0].IdentificadorRegistroPai != identficardorRegistro)
                        return;

                    if (!string.IsNullOrWhiteSpace(registroRecursivo) && registroRecursivo != campos[0].IdentificadorRegistro)
                        return;

                    if (!string.IsNullOrWhiteSpace(campos[0].PropriedadeObjetoPai) && propriedade != campos[0].PropriedadeObjetoPai)
                    {
                        GetInstacia(data, campos[0].PropriedadeObjetoPai);
                        PropertyInfo propertyToGet = data.GetType().GetProperty(campos[0].PropriedadeObjetoPai);
                        var filho = propertyToGet.GetValue(data, null);
                        LerCampoRegistroRecursivo(filho, ref indice, campos[0].PropriedadeObjetoPai, identficardorRegistro);
                        LerCampoRegistroRecursivo(data, ref indice, propriedade, identficardorRegistro);
                    }
                    else
                    {
                        indice++;
                        if (campos[0].Repetir)
                        {
                            Type type = data.GetType().GetGenericArguments()[0];
                            var filho = LerCampoRecursivoObjeto(type, null, campos, this.LinhasArquivo[index]);
                            data.Add(filho);

                            if (!string.IsNullOrWhiteSpace(campos[0].IdentificadorRegistro))
                                LerCampoRegistroRecursivo(filho, ref indice, propriedade, campos[0].IdentificadorRegistro);

                            LerCampoRegistroRecursivo(data, ref indice, propriedade, identficardorRegistro, campos[0].IdentificadorRegistro);
                        }
                        else
                        {
                            LerCampoRecursivoObjeto(null, data, campos, this.LinhasArquivo[index]);
                            LerCampoRegistroRecursivo(data, ref indice, propriedade, campos[0].IdentificadorRegistro);
                        }
                    }
                }
                else
                {
                    indice++;
                    LerCampoRegistroRecursivo(data, ref indice, propriedade, identficardorRegistro);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("Erro Lendo EDI " + campoErro + " " + e.Message);
                throw;
            }
        }

        private string LerRegistrosAbastecimentos()
        {
            string MsgRetorno = "";

            IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(UnitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(UnitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(UnitOfWork);

            string inicioRegistros = "";
            string dataAbastecimento;
            for (int i = 0; i < this.LinhasArquivo.Count(); i++)
            {

                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                if (campos.Count() > 0)
                {

                    if (string.IsNullOrWhiteSpace(inicioRegistros) && !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico && obj.PropriedadeObjeto != null select obj).Any())
                    {
                        inicioRegistros = campos[0].IdentificadorRegistro;
                    }

                    if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico select obj).Any())
                    {
                        Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                        int posicao = 0;

                        foreach (var campo in campos)
                        {
                            if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico && !string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            {
                                object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                this.SetNestedPropertyValue(abastecimento, campo.PropriedadeObjeto, valor, campo.Mascara);
                            }
                            posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                        }

                        abastecimento.Litros = decimal.Parse(abastecimento.LitrosEDI.Trim().Replace(".", ","));
                        decimal valorTotal = decimal.Parse(abastecimento.ValorTotalEDI.Trim().Replace(".", ","));
                        abastecimento.ValorUnitario = valorTotal / abastecimento.Litros;

                        if (abastecimento.DataFormatadaEDI.Contains("-"))
                        {
                            dataAbastecimento = abastecimento.DataFormatadaEDI.Split('-')[2].Substring(0, 2) + '/';
                            dataAbastecimento = dataAbastecimento + abastecimento.DataFormatadaEDI.Split('-')[1].Substring(0, 2) + '/';
                            dataAbastecimento = dataAbastecimento + abastecimento.DataFormatadaEDI.Substring(0, 4) + ' ';
                            dataAbastecimento = dataAbastecimento + abastecimento.DataFormatadaEDI.Split('-')[2].Substring(2, 2) + ':';
                            dataAbastecimento = dataAbastecimento + abastecimento.DataFormatadaEDI.Split(':')[1] + ':';
                            dataAbastecimento = dataAbastecimento + abastecimento.DataFormatadaEDI.Split(':')[2].Substring(0, 2);
                        }
                        else
                        {
                            dataAbastecimento = abastecimento.DataFormatadaEDI.Substring(0, 2) + "/" + abastecimento.DataFormatadaEDI.Substring(2, 2) + "/" + abastecimento.DataFormatadaEDI.Substring(4, 4);
                        }
                        abastecimento.Data = DateTime.Parse(dataAbastecimento);

                        abastecimento.Pago = false;
                        abastecimento.Situacao = "F";
                        abastecimento.Status = "A";
                        abastecimento.DataAlteracao = DateTime.Now;

                        abastecimento.Veiculo = repVeiculo.BuscarPorPlaca(abastecimento.PlacaVeiculoEDI);
                        abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(abastecimento.CNPJPostoEDI);
                        abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(abastecimento.CNPJPostoEDI, abastecimento.CodigoProdutoEDI);

                        if (abastecimento.Veiculo == null)
                        {
                            if (!MsgRetorno.Contains(abastecimento.PlacaVeiculoEDI))
                                MsgRetorno = MsgRetorno + "- Veículo: " + abastecimento.PlacaVeiculoEDI + " não cadastrado.<br/>";
                        }
                        else if (abastecimento.Posto == null)
                        {
                            if (!MsgRetorno.Contains(Convert.ToString(abastecimento.CNPJPostoEDI)))
                                MsgRetorno = MsgRetorno + "- Posto: " + abastecimento.NomePosto + " CNPJ: " + abastecimento.CNPJPostoEDI + " não cadastrado.<br/>";
                        }
                        else if (abastecimento.Produto == null)
                        {
                            if (!MsgRetorno.Contains(abastecimento.CodigoProdutoEDI))
                                MsgRetorno = MsgRetorno + "- Posto: " + abastecimento.NomePosto + " CNPJ: " + abastecimento.CNPJPostoEDI + " Código de Integração: " + abastecimento.CodigoProdutoEDI + " - " + abastecimento.DescricaoProdutoEDI + " não cadastrado.<br/>";
                        }
                        else if (abastecimento.Litros > abastecimento.Veiculo.CapacidadeTanque && abastecimento.Veiculo.CapacidadeTanque > 0)
                        {
                            MsgRetorno = MsgRetorno + "- Litros abastecidos no veículo " + abastecimento.Veiculo.Placa + " é maior que sua Capacidade de Tanque (" + abastecimento.Veiculo.CapacidadeTanque.ToString() + ").";
                        }
                        else
                        {
                            if (abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                            else
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                            this.Abastecimentos.Add(abastecimento);
                        }
                    }
                }
            }
            return MsgRetorno;
        }

        private void LerRegistrosFaturaNatura()
        {
            IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();

            Dominio.ObjetosDeValor.EDI.Fatura fatura = null;

            string inicioRegistros = string.Empty, inicioRegistros2 = string.Empty;

            bool novaFatura = true;
            for (int i = 0; i < this.LinhasArquivo.Count(); i++)
            {
                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                if (campos.Count() > 0)
                {
                    if (string.IsNullOrWhiteSpace(inicioRegistros) && !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Fatura && obj.PropriedadeObjeto != null select obj).Any())
                    {
                        inicioRegistros = campos[0].IdentificadorRegistro;
                    }

                    var registrosFilhos = (from obj in campos where obj.IdentificadorRegistroPai.Equals(obj.IdentificadorRegistro) orderby obj.Ordem select obj).Distinct();

                    if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Fatura select obj).Any())
                    {
                        novaFatura = true;
                        fatura = new Dominio.ObjetosDeValor.EDI.Fatura();
                        fatura.faturaCTes = new List<Dominio.ObjetosDeValor.EDI.FaturaCTe>();

                        int posicao = 0;

                        foreach (var campo in campos)
                        {
                            if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Fatura && !string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            {
                                object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                this.SetNestedPropertyValue(fatura, campo.PropriedadeObjeto, valor, campo.Mascara);
                            }

                            posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                        }
                    }

                    if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.FaturaCTe && fatura != null select obj).Any())
                    {
                        Dominio.ObjetosDeValor.EDI.FaturaCTe faturaCTe = new Dominio.ObjetosDeValor.EDI.FaturaCTe();

                        int posicao = 0;
                        foreach (var campo in campos)
                        {
                            if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.FaturaCTe)
                            {
                                object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                this.SetNestedPropertyValue(faturaCTe, campo.PropriedadeObjeto, valor, campo.Mascara);
                            }

                            posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                        }

                        fatura.faturaCTes.Add(faturaCTe);

                    }

                    if (fatura != null && novaFatura)
                    {
                        this.Faturas.Add(fatura);
                        novaFatura = false;
                    }
                }
            }
        }

        private void LerRegistrosOcorencia()
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            try
            {
                IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = null;
                Dominio.Entidades.DocumentosCTE documento = null;

                //List<Dominio.Entidades.DocumentosCTE> documentos = new List<Dominio.Entidades.DocumentosCTE>();

                //unidadeDeTrabalho.Start();

                string inicioRegistros = string.Empty, inicioRegistros2 = string.Empty;

                for (int i = 0; i < this.LinhasArquivo.Count(); i++)
                {
                    var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                    if (campos.Count() > 0)
                    {
                        if (string.IsNullOrWhiteSpace(inicioRegistros) && !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia && obj.PropriedadeObjeto != null && obj.PropriedadeObjeto.Contains("Remetente") select obj).Any())
                        {
                            inicioRegistros = campos[0].IdentificadorRegistro;
                        }

                        var registrosFilhos = (from obj in campos where obj.IdentificadorRegistroPai.Equals(obj.IdentificadorRegistro) orderby obj.Ordem select obj).Distinct();

                        if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia select obj).Any())
                        {
                            ocorrencia = new Dominio.Entidades.OcorrenciaDeCTe();
                            ocorrencia.Ocorrencia = new Dominio.Entidades.TipoDeOcorrenciaDeCTe();

                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia && !string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    this.SetNestedPropertyValue(ocorrencia, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                        {
                            documento = new Dominio.Entidades.DocumentosCTE();
                            int posicao = 0;
                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    this.SetNestedPropertyValue(documento, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (ocorrencia != null)
                        {
                            if (ocorrencia.CTe == null)
                            {
                                if (documento.CTE == null)
                                {
                                    Dominio.Entidades.DocumentosCTE docCTe = repDocumentosCTE.BuscarPorNumeroSerieNFe(int.Parse(documento.Numero).ToString(), int.Parse(documento.Serie).ToString(), documento.CNPJRemetente, this.Empresa.Codigo);
                                    if (docCTe != null)
                                        ocorrencia.CTe = docCTe.CTE;
                                }
                                else
                                    ocorrencia.CTe = documento.CTE;
                            }
                            if (ocorrencia.CTe != null)
                            {
                                ocorrencia.DataDeCadastro = DateTime.Now;
                                if (ocorrencia.Ocorrencia.Codigo == 0)
                                {
                                    ocorrencia.Ocorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigoProceda(ocorrencia.Ocorrencia.CodigoProceda);
                                }
                                if (ocorrencia.Ocorrencia != null)
                                {
                                    repOcorrenciaDeCTe.Inserir(ocorrencia);
                                    this.OcorrenciasGeradas.Add(ocorrencia);
                                }
                            }

                        }

                    }
                }
                //unidadeDeTrabalho.CommitChanges();
            }
            catch
            {
                //unidadeDeTrabalho.Rollback();
                throw;
            }
        }
        private void LerRegistrosCargaOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia)
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte repCargaOcorrenciaObservacaoContribuinte = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Embarcador.CargaOcorrencia.Ocorrencia();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            try
            {
                IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null;
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = null;

                unidadeDeTrabalho.Start();

                string inicioRegistros = string.Empty, inicioRegistros2 = string.Empty;

                for (int i = 0; i < this.LinhasArquivo.Count(); i++)
                {
                    var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                    if (campos.Count() > 0)
                    {
                        if (string.IsNullOrWhiteSpace(inicioRegistros) && !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia && obj.PropriedadeObjeto != null && obj.PropriedadeObjeto.Contains("Remetente") select obj).Any())
                        {
                            inicioRegistros = campos[0].IdentificadorRegistro;
                        }

                        var registrosFilhos = (from obj in campos where obj.IdentificadorRegistroPai.Equals(obj.IdentificadorRegistro) orderby obj.Ordem select obj).Distinct();


                        if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFe select obj).Any())
                        {
                            notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                            int posicao = 0;
                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFe)
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    this.SetNestedPropertyValue(notaFiscal, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (notaFiscal != null)
                        {
                            Dominio.Entidades.DocumentosCTE docCTe = repDocumentosCTE.BuscarPorNumeroSerieNFe(notaFiscal.Numero.ToString(), notaFiscal.Serie, null, 0);
                            if (docCTe != null)
                            {
                                cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                                DateTime dataEmissao = DateTime.Now;
                                DateTime.TryParseExact(notaFiscal.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao);

                                cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao;
                                cargaOcorrencia.DataOcorrencia = DateTime.Now;
                                cargaOcorrencia.DataAlteracao = DateTime.Now;
                                cargaOcorrencia.ValorOcorrencia = notaFiscal.ValorFreteLiquido;
                                cargaOcorrencia.ValorOcorrenciaOriginal = notaFiscal.ValorFreteLiquido;
                                cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
                                cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unidadeDeTrabalho);
                                cargaOcorrencia.Carga = docCTe.CTE.CargaCTes.FirstOrDefault().Carga;
                                cargaOcorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                                cargaOcorrencia.ObservacaoCTe = notaFiscal.NumeroReferenciaEDI ?? "";
                                cargaOcorrencia.OrigemOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga;
                                repCargaOcorrencia.Inserir(cargaOcorrencia);

                                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte cargaOcorrenciaObservacaoContribuinte = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte()
                                {
                                    Identificador = "XcaracAd",
                                    Texto = ((string)notaFiscal.InformacoesComplementares) ?? "",
                                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe.Contribuinte,
                                    Ocorrencia = cargaOcorrencia
                                };
                                repCargaOcorrenciaObservacaoContribuinte.Inserir(cargaOcorrenciaObservacaoContribuinte);

                                string mensagemRetorno = string.Empty;
                                if (!serOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, docCTe.CTE.CargaCTes.ToList(), null, ref mensagemRetorno, unidadeDeTrabalho, TipoServicoMultisoftware.MultiTMS, Usuario, configuracaoEmbarcador, null, "", false, false, null))
                                    throw new ControllerException(mensagemRetorno);

                                this.CargaOcorrenciasGeradas.Add(cargaOcorrencia);
                            }
                        }
                    }
                }
                unidadeDeTrabalho.CommitChanges();
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }
        private void LerRegistrosCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                List<Dominio.Entidades.DocumentosCTE> documentos = new List<Dominio.Entidades.DocumentosCTE>();

                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                Dominio.Entidades.ParticipanteCTe remetente = new Dominio.Entidades.ParticipanteCTe();
                Dominio.Entidades.ParticipanteCTe destinatario = new Dominio.Entidades.ParticipanteCTe();

                Dominio.Entidades.Veiculo veiculo = null;
                List<Dominio.Entidades.ObservacaoContribuinteCTE> listaObsCont = new List<Dominio.Entidades.ObservacaoContribuinteCTE>();

                unidadeDeTrabalho.Start();

                string inicioRegistros = string.Empty, inicioRegistros2 = string.Empty;
                bool inicio = false, gerar = true;

                for (int i = 0; i < this.LinhasArquivo.Count(); i++)
                {
                    var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(this.LinhasArquivo[i].Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                    if (campos.Count() > 0)
                    {
                        gerar = true;

                        if (inicioRegistros2 == "313" && campos[0].IdentificadorRegistro == "333")
                            inicioRegistros2 = campos[0].IdentificadorRegistro;

                        if (inicioRegistros2 == "333" && campos[0].IdentificadorRegistro == "313")
                            inicioRegistros2 = campos[0].IdentificadorRegistro;

                        if (inicio)
                        {
                            inicioRegistros2 = campos[0].IdentificadorRegistro;
                            inicio = false;
                            gerar = false;
                        }

                        if (campos[0].IdentificadorRegistro == "317" || campos[0].IdentificadorRegistro == "307")
                            gerar = false;

                        if (campos[0].IdentificadorRegistro == "000" && (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.ObsCont select obj).Any())
                        {
                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.ObsCont)
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));
                                    Dominio.Entidades.ObservacaoContribuinteCTE obsCont = new Dominio.Entidades.ObservacaoContribuinteCTE();
                                    this.SetNestedPropertyValue(obsCont, campo.PropriedadeObjeto, valor, campo.Mascara);
                                    if (!string.IsNullOrWhiteSpace(campo.ValorFixo))
                                        obsCont.Identificador = campo.ValorFixo;
                                    listaObsCont.Add(obsCont);
                                }
                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(inicioRegistros) && campos[0].IdentificadorRegistro != "000" && !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe && obj.PropriedadeObjeto != null && obj.PropriedadeObjeto.Contains("Remetente") select obj).Any())
                        {
                            inicioRegistros = campos[0].IdentificadorRegistro;
                        }
                        else if ((campos[0].IdentificadorRegistro == inicioRegistros || campos[0].IdentificadorRegistro != inicioRegistros2) && cte != null && !inicio && gerar)
                        {
                            if (remetente != null && cte.Remetente == null)
                                cte.Remetente = remetente;
                            else if (cte.Remetente != null)
                                remetente = cte.Remetente;

                            if (destinatario != null && cte.Destinatario == null)
                                cte.Destinatario = destinatario;
                            else if (cte.Destinatario != null)
                                destinatario = cte.Destinatario;

                            this.SalvarCTe(ref cte, ref documentos, ref veiculo, listaObsCont, unidadeDeTrabalho);

                            cte = null;
                        }

                        var registrosFilhos = (from obj in campos where obj.IdentificadorRegistroPai.Equals(obj.IdentificadorRegistro) orderby obj.Ordem select obj).Distinct();

                        if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe select obj).Any())
                        {
                            if (cte == null)
                                cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe && !string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    if (campo.PropriedadeObjeto == "ValorFrete")
                                        valor = (decimal)valor + cte.ValorFrete;

                                    if (campo.PropriedadeObjeto == "ValorAReceber")
                                        valor = (decimal)valor + cte.ValorAReceber;

                                    this.SetNestedPropertyValue(cte, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (cte != null && (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                        {
                            Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();

                            int posicao = 0;
                            var numeroDocumento = string.Empty;

                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    this.SetNestedPropertyValue(documento, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }

                            if (documento.Numero != null)
                                numeroDocumento = documento.Numero;
                            else if (documento.Numero == null && documento.ChaveNFE != null)
                                numeroDocumento = documento.ChaveNFE.Substring(25, 9);

                            if (!string.IsNullOrWhiteSpace(numeroDocumento) && !(from obj in documentos where obj.Numero.Contains(int.Parse(numeroDocumento).ToString()) select obj).Any())
                                documentos.Add(documento);
                            else if (campos[0].IdentificadorRegistro == "333" || campos[0].IdentificadorRegistro == "307")
                            {
                                Dominio.Entidades.DocumentosCTE documentoChave = (from obj in documentos where obj.Numero.Contains(int.Parse(numeroDocumento).ToString()) select obj).FirstOrDefault();
                                documentos.Remove(documentoChave);
                                documentoChave.ChaveNFE = documento.ChaveNFE;
                                documentos.Add(documentoChave);
                            }
                        }

                        if (cte != null && (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Veiculo select obj).Any())
                        {
                            if (veiculo == null)
                                veiculo = new Dominio.Entidades.Veiculo();

                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Veiculo)
                                {
                                    object valor = this.LerCampo(campo, this.LinhasArquivo[i].Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));

                                    this.SetNestedPropertyValue(veiculo, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }
                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (campos[0].IdentificadorRegistro == inicioRegistros || campos[0].IdentificadorRegistro == "311")
                            inicio = true;

                    }
                }

                if (cte != null)
                {
                    if (cte.Remetente == null)
                        cte.Remetente = remetente;

                    if (cte.Destinatario == null)
                        cte.Destinatario = destinatario;

                    this.SalvarCTe(ref cte, ref documentos, ref veiculo, listaObsCont, unidadeDeTrabalho);
                }

                unidadeDeTrabalho.CommitChanges();
            }
            catch
            {
                unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        private void LerRegistrosNotasFiscais()
        {
            string linhaAtual = string.Empty;
            string campoAtual = string.Empty;

            try
            {
                IEnumerable<string> registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem select obj.IdentificadorRegistro).Distinct();

                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = null;
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                string inicioRegistros = string.Empty, inicioSubRegistros = string.Empty, inicioSubRegistros2 = string.Empty;
                bool inicio = false, gerar = true;
                int indiceSubRegistro = 0, indiceSubRegistro2 = 0;

                for (int i = 0; i < this.LinhasArquivo.Count(); i++)
                {
                    linhaAtual = this.LinhasArquivo[i];
                    if (linhaAtual == "\u001a")
                        break;

                    var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro.Equals(linhaAtual.Substring(0, (obj.IdentificadorRegistro.Length))) orderby obj.Ordem select obj).ToList();

                    if (campos.Count() > 0)
                    {
                        Dominio.Entidades.CampoEDI primeiroCampo = campos[0];

                        gerar = true;

                        if (inicioSubRegistros == primeiroCampo.IdentificadorRegistroPai)
                        {
                            //aqui vai começar a tratativa do terceiro nível de sub-registros, ex: Remetente > Destinatario > Notas Fiscais
                            if (string.IsNullOrWhiteSpace(inicioSubRegistros2) && primeiroCampo.Repetir)
                            {
                                inicioSubRegistros2 = primeiroCampo.IdentificadorRegistro;
                                indiceSubRegistro2 = i;
                            }
                            else if (!string.IsNullOrWhiteSpace(inicioSubRegistros2))
                            {
                                if (inicioSubRegistros2 == primeiroCampo.IdentificadorRegistro)
                                {
                                    if (indiceSubRegistro2 > 0 && indiceSubRegistro2 != i)
                                    {
                                        if (emitente != null && notaFiscal.Emitente == null)
                                            notaFiscal.Emitente = emitente;
                                        else if (notaFiscal.Emitente != null)
                                            emitente = notaFiscal.Emitente;

                                        if (destinatario != null && notaFiscal.Destinatario == null)
                                            notaFiscal.Destinatario = destinatario;
                                        else if (notaFiscal.Destinatario != null)
                                            destinatario = notaFiscal.Destinatario;

                                        NotasFiscais.Add(notaFiscal);

                                        notaFiscal = null;
                                    }

                                    indiceSubRegistro2 = i;
                                }
                            }
                        }
                        else
                        {
                            if (inicio)
                            {
                                inicioSubRegistros = primeiroCampo.IdentificadorRegistro;
                                inicio = false;
                                gerar = false;
                                indiceSubRegistro = 0;
                                indiceSubRegistro2 = 0;
                            }

                            if (inicioSubRegistros == primeiroCampo.IdentificadorRegistro) //tá no começo dos sub-registros (1 registro para cada nota fiscal)
                            {
                                if (indiceSubRegistro > 0 && indiceSubRegistro != i)
                                {
                                    if (emitente != null && notaFiscal.Emitente == null)
                                        notaFiscal.Emitente = emitente;
                                    else if (notaFiscal.Emitente != null)
                                        emitente = notaFiscal.Emitente;

                                    if (destinatario != null && notaFiscal.Destinatario == null)
                                        notaFiscal.Destinatario = destinatario;
                                    else if (notaFiscal.Destinatario != null)
                                        destinatario = notaFiscal.Destinatario;

                                    NotasFiscais.Add(notaFiscal);

                                    notaFiscal = null;
                                }

                                indiceSubRegistro = i;
                            }

                            if (string.IsNullOrWhiteSpace(inicioRegistros) && (primeiroCampo.Repetir || !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFe && obj.PropriedadeObjeto != null && obj.PropriedadeObjeto.Contains("Emitente.") select obj).Any()))
                            {
                                inicioRegistros = primeiroCampo.IdentificadorRegistro;
                            }
                            else if ((primeiroCampo.IdentificadorRegistro == inicioRegistros || primeiroCampo.IdentificadorRegistro != inicioSubRegistros) && notaFiscal != null && !inicio && gerar)
                            {
                                if (emitente != null && notaFiscal.Emitente == null)
                                    notaFiscal.Emitente = emitente;
                                else if (notaFiscal.Emitente != null)
                                    emitente = notaFiscal.Emitente;

                                if (destinatario != null && notaFiscal.Destinatario == null)
                                    notaFiscal.Destinatario = destinatario;
                                else if (notaFiscal.Destinatario != null)
                                    destinatario = notaFiscal.Destinatario;

                                NotasFiscais.Add(notaFiscal);

                                notaFiscal = null;
                            }
                        }

                        if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFe select obj).Any())
                        {
                            if (notaFiscal == null)
                                notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                campoAtual = campo.Descricao;
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFe && !string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                                {
                                    int tamanhoCampo = campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                                    if (posicao + tamanhoCampo > linhaAtual.Length)
                                        tamanhoCampo = linhaAtual.Length - posicao;

                                    object valor = this.LerCampo(campo, linhaAtual.Substring(posicao, tamanhoCampo));
                                    this.SetNestedPropertyValue(notaFiscal, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }
                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }
                        }

                        if (notaFiscal != null && (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFeVolumes select obj).Any())
                        {
                            Dominio.ObjetosDeValor.Embarcador.NFe.Volume volume = new Dominio.ObjetosDeValor.Embarcador.NFe.Volume();

                            int posicao = 0;

                            foreach (var campo in campos)
                            {
                                campoAtual = campo.Descricao;
                                if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NFeVolumes)
                                {
                                    int tamanhoCampo = campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                                    if (posicao + tamanhoCampo > linhaAtual.Length)
                                        tamanhoCampo = linhaAtual.Length - posicao;

                                    object valor = this.LerCampo(campo, linhaAtual.Substring(posicao, (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros)));
                                    this.SetNestedPropertyValue(volume, campo.PropriedadeObjeto, valor, campo.Mascara);
                                }

                                posicao += campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros;
                            }

                            if (notaFiscal.Volumes == null)
                                notaFiscal.Volumes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume>();

                            notaFiscal.Volumes.Add(volume);

                        }

                        if (primeiroCampo.IdentificadorRegistro == inicioRegistros)
                        {
                            inicio = true;
                        }
                    }
                }

                if (notaFiscal != null)
                {
                    if (notaFiscal.Emitente == null)
                        notaFiscal.Emitente = emitente;

                    if (notaFiscal.Destinatario == null)
                        notaFiscal.Destinatario = destinatario;

                    NotasFiscais.Add(notaFiscal);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao ler EDI Linha " + linhaAtual);
                Servicos.Log.TratarErro("Falha ao ler EDI Campo " + campoAtual);
                throw;
            }
        }

        private string processarExpressaoEDI(Dominio.Entidades.CampoEDI campo, string valor)
        {
            string expressao = campo.Expressao;
            List<string> condicoes = new List<string>();
            List<string> retornos = new List<string>();

            string retorno = "";
            string conteudo = "";
            bool inicio = false;
            for (int i = 0; i < expressao.Length; i++)
            {
                if (expressao[i] != ')' && !inicio)
                {
                    conteudo += expressao[i];
                }
                else
                {
                    if (!inicio)
                    {
                        conteudo += expressao[i];
                        inicio = true;
                    }
                    else
                    {
                        if (expressao[i] != '(')
                        {
                            retorno += expressao[i];
                            if (i == expressao.Length - 1)
                            {
                                condicoes.Add(conteudo);
                                retornos.Add(retorno);
                            }
                        }
                        else
                        {
                            condicoes.Add(conteudo);
                            retornos.Add(retorno);
                            conteudo = "";
                            retorno = "";
                            inicio = false;

                            conteudo += expressao[i];
                        }
                    }
                }
            }

            for (int iCondi = 0; iCondi <= condicoes.Count - 1; iCondi++)
            {
                string condicao = condicoes[iCondi];

                string campoEDI = "";
                string operacao = "";
                string valorCompara = "";
                string tipocomparacao = "";

                bool comparacao = false;
                for (int i = 0; i < condicao.Length; i++)
                {
                    if (campoEDI.Trim() == "#campo")
                    {
                        if (condicao[i] != '"' && valorCompara == "")
                        {
                            operacao += condicao[i];
                        }
                        else
                        {
                            if (condicao[i] == '#' || condicao[i] == '&' || condicao[i] == '|' || condicao[i] == ')')
                            {
                                valorCompara = valorCompara.Replace("\"", "").TrimEnd().TrimStart();
                                if (operacao.Trim() == "==")
                                {
                                    if (valor == valorCompara)
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                if (operacao.Trim() == "!=")
                                {
                                    if (valor != valorCompara)
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                if (operacao.Trim() == ">=")
                                {
                                    if (decimal.Parse(valor) >= decimal.Parse(valorCompara))
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                if (operacao.Trim() == "<=")
                                {
                                    if (decimal.Parse(valor) <= decimal.Parse(valorCompara))
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                if (operacao.Trim() == ">")
                                {
                                    if (decimal.Parse(valor) > decimal.Parse(valorCompara))
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                if (operacao.Trim() == "<")
                                {
                                    if (decimal.Parse(valor) < decimal.Parse(valorCompara))
                                    {
                                        if ((tipocomparacao != "&&" || comparacao == true))
                                            comparacao = true;
                                    }
                                }
                                operacao = "";
                                valorCompara = "";
                                campoEDI = "";
                                campoEDI += condicao[i];
                            }
                            else
                            {
                                valorCompara += condicao[i];
                            }
                        }
                    }
                    else
                    {
                        if (campoEDI.Trim() == "&&" || campoEDI.Trim() == "||")
                        {
                            tipocomparacao = campoEDI.Trim();
                            campoEDI = "";
                        }
                        else
                        {
                            if (condicao[i] != '(')
                                campoEDI += condicao[i];
                        }

                    }
                }
                if (comparacao || condicao.Trim() == "()")
                {
                    if (retornos[iCondi] != "#campo")
                        valor = retornos[iCondi];
                    break;
                }
            }
            return valor;
        }

        private object LerCampo(Dominio.Entidades.CampoEDI campo, string valor)
        {

            if (!string.IsNullOrEmpty(campo.Expressao))
            {
                valor = processarExpressaoEDI(campo, valor);
            }

            switch (campo.Tipo)
            {
                case Dominio.Enumeradores.TipoCampoEDI.Alfanumerico:

                    return valor.Trim();

                case Dominio.Enumeradores.TipoCampoEDI.DataEHora:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        DateTime data = DateTime.MinValue;

                        if (!string.IsNullOrWhiteSpace(campo.Mascara))
                        {
                            DateTime.TryParseExact(valor, campo.Mascara, null, System.Globalization.DateTimeStyles.None, out data);
                        }
                        else
                        {
                            DateTime.TryParseExact(valor, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out data);
                        }

                        return data;
                    }
                    else
                    {
                        return null;
                    }

                case Dominio.Enumeradores.TipoCampoEDI.Decimal:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                        NumberStyles style = NumberStyles.Any;

                        valor = valor.Trim();

                        int tamanhoTotalCampo = campo.QuantidadeDecimais + campo.QuantidadeInteiros;

                        if (tamanhoTotalCampo > 0)//quando for separado por indice e não souber o tamanho do campo deve deixar zerado no layout com isso o sistema irá apenas converter oque vem.
                        {
                            if (valor.Length < tamanhoTotalCampo)
                                valor = new string('0', tamanhoTotalCampo - valor.Length) + valor;

                            decimal dec = 0m;
                            valor = valor.Replace(",", "");

                            decimal.TryParse(valor.Substring(0, valor.Length - campo.QuantidadeDecimais) + "," + valor.Substring(valor.Length - campo.QuantidadeDecimais, campo.QuantidadeDecimais), style, cultura, out dec);

                            return dec;
                        }
                        else
                        {
                            return Utilidades.Decimal.Converter(valor);
                        }
                    }
                    else
                    {
                        return 0m;
                    }

                case Dominio.Enumeradores.TipoCampoEDI.Numerico:

                    int i = 0;

                    int.TryParse(valor.Trim(), out i);

                    return i;

                default:

                    return null;
            }
        }

        private object SetNestedPropertyValueRecursive(object target, string propertyName, object value, string mask = "")
        {
            object obj = target;

            string[] properties = propertyName.Split('.');
            List<KeyValuePair<PropertyInfo, object>> valuesOfProperties = new List<KeyValuePair<PropertyInfo, object>>();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(properties[i]);

                target = propertyToGet.GetValue(target, null);

                if (target == null)
                    target = Activator.CreateInstance(propertyToGet.PropertyType);

                valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToGet, target));
            }

            PropertyInfo propertyToSet = target.GetType().GetProperty(properties.Last());

            if (propertyToSet.PropertyType.FullName.StartsWith("System"))
            {
                if (propertyToSet.PropertyType.FullName.Contains("DateTime") && !string.IsNullOrWhiteSpace(mask) && (value == null || value.GetType() != typeof(DateTime)))
                {
                    DateTime data = DateTime.MinValue;

                    if (value == null)
                        value = data;
                    else if (DateTime.TryParseExact((string)value, mask, null, System.Globalization.DateTimeStyles.None, out data))
                        value = data;
                    else if (propertyToSet.PropertyType.FullName.Contains("Nullable"))
                        value = null;
                    else
                        value = DateTime.Now;
                }
                else
                {
                    if (propertyToSet.PropertyType.FullName.Contains("Int"))
                    {
                        if (!string.IsNullOrWhiteSpace(value.ToString()))
                            value = Convert.ChangeType(value, propertyToSet.PropertyType);
                        else
                            value = 0;
                    }
                    else if (propertyToSet.PropertyType.FullName.Contains("Decimal"))
                    {
                        if (!string.IsNullOrWhiteSpace(value.ToString()))
                            value = Convert.ChangeType(value, propertyToSet.PropertyType);
                        else
                            value = (decimal)0;
                    }
                    else if (propertyToSet.PropertyType.FullName.Contains("String"))
                    {
                        value = Convert.ChangeType(value?.ToString().Trim() ?? "", propertyToSet.PropertyType);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, propertyToSet.PropertyType);
                    }


                }
            }
            else
            {
                if (propertyToSet.PropertyType.FullName.Contains("Enumeradores"))
                {
                    if (value.GetType() == typeof(string))
                    {
                        int n;
                        bool isNumeric = int.TryParse(value.ToString(), out n);
                        value = !string.IsNullOrWhiteSpace(value.ToString()) ? isNumeric ? n : value : 0;
                    }
                }
            }

            propertyToSet.SetValue(target, value, null);

            valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToSet, value));

            for (var i = valuesOfProperties.Count() - 1; i > 0; i--)
                valuesOfProperties[i].Key.SetValue(valuesOfProperties[i - 1].Value, valuesOfProperties[i].Value);

            valuesOfProperties[0].Key.SetValue(obj, valuesOfProperties[0].Value);

            return obj;
        }

        private object SetNestedPropertyValue(object target, string propertyName, object value, string mask = "")
        {
            object obj = target;

            string[] properties = propertyName.Split('.');
            List<KeyValuePair<PropertyInfo, object>> valuesOfProperties = new List<KeyValuePair<PropertyInfo, object>>();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(properties[i]);

                target = propertyToGet.GetValue(target, null);

                if (target == null)
                    target = Activator.CreateInstance(propertyToGet.PropertyType);

                valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToGet, target));
            }

            PropertyInfo propertyToSet = target.GetType().GetProperty(properties.Last());

            if (propertyToSet.PropertyType.FullName.StartsWith("System"))
            {
                if (propertyToSet.PropertyType.FullName.Contains("DateTime") && !string.IsNullOrWhiteSpace(mask))
                {
                    DateTime data = DateTime.MinValue;

                    if (DateTime.TryParseExact((string)value, mask, null, System.Globalization.DateTimeStyles.None, out data))
                        value = data;
                    else if (propertyToSet.PropertyType.FullName.Contains("Nullable"))
                        value = null;
                    else
                        value = DateTime.Now;
                }
                else
                {
                    value = Convert.ChangeType(value, propertyToSet.PropertyType);
                }
            }

            propertyToSet.SetValue(target, value, null);

            valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToSet, value));

            for (var i = valuesOfProperties.Count() - 1; i > 0; i--)
                valuesOfProperties[i].Key.SetValue(valuesOfProperties[i - 1].Value, valuesOfProperties[i].Value);

            valuesOfProperties[0].Key.SetValue(obj, valuesOfProperties[0].Value);

            return obj;
        }

        private void SalvarDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentosCTE repDocumento = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                if (documento.DataEmissao == DateTime.MinValue)
                    documento.DataEmissao = DateTime.Now;

                if (string.IsNullOrWhiteSpace(documento.Numero) && !string.IsNullOrWhiteSpace(documento.ChaveNFE))
                    documento.Numero = int.Parse(documento.ChaveNFE.Substring(25, 9)).ToString();

                if (documento.CTE == null)
                    documento.CTE = cte;

                if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                {
                    documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("55");
                }
                else
                {
                    documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("99");
                    documento.Descricao = "Nota Fiscal";
                }

                repDocumento.Inserir(documento);
            }
        }

        private void SalvarCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ref List<Dominio.Entidades.DocumentosCTE> documentos, ref Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.ObservacaoContribuinteCTE> listaObsCont, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeDeTrabalho);

            Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

            Dominio.Entidades.MotoristaCTE motorista = null;

            if (veiculo != null && !string.IsNullOrWhiteSpace(veiculo.Placa))
            {
                veiculo = repVeiculo.BuscarPorPlaca(this.Empresa.Codigo, veiculo.Placa);
                if (veiculo != null)
                {
                    Dominio.Entidades.Usuario motoristaPrincipal = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                    if (motoristaPrincipal != null)
                    {
                        motorista = new Dominio.Entidades.MotoristaCTE();
                        motorista.CPFMotorista = motoristaPrincipal.CPF;
                        motorista.NomeMotorista = motoristaPrincipal.Nome;
                    }
                    else if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                    {
                        Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorPlaca(this.Empresa.Codigo, veiculo.VeiculosTracao.FirstOrDefault().Placa);
                        motoristaPrincipal = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                        if (motoristaPrincipal != null)
                        {
                            motorista = new Dominio.Entidades.MotoristaCTE();
                            motorista.CPFMotorista = motoristaPrincipal.CPF;
                            motorista.NomeMotorista = motoristaPrincipal.Nome;
                        }
                    }
                }
            }

            cte.Empresa = this.Empresa;

            if (cte.Remetente != null && cte.Remetente.Localidade != null && cte.Remetente.Localidade.Estado != null)
            {
                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.DadosCliente dadosRemetente = null;

                double cpfCnpjRemetente;
                double.TryParse(cte.Remetente.CPF_CNPJ, out cpfCnpjRemetente);

                remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

                if (remetente == null)
                {
                    Dominio.Entidades.Localidade localidadeRemetente = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(cte.Remetente.Localidade.Descricao), cte.Remetente.Localidade.Estado.Sigla);

                    if (localidadeRemetente == null)
                    {
                        localidadeRemetente = repLocalidade.BuscarPorCodigoIBGE(cte.Remetente.Localidade.CodigoIBGE);
                        if (localidadeRemetente == null && !string.IsNullOrWhiteSpace(cte.Remetente.CEP) && cte.Remetente.CEP.Length > 3)
                        {
                            localidadeRemetente = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(cte.Remetente.CEP.Substring(0, 3)));
                        }
                        if (localidadeRemetente == null)
                            throw new Exception("A localidade do remetente não foi encontrada para salvar o CT-e (NOTFIS). " + cte.Remetente.Localidade.Descricao + " " + cte.Remetente.Localidade.Estado.Sigla + " " + cte.Remetente.Localidade.CodigoIBGE);
                    }

                    remetente = new Dominio.Entidades.Cliente();
                    remetente.CPF_CNPJ = cpfCnpjRemetente;
                    remetente.Atividade = Atividade.ObterAtividade(this.Empresa.Codigo, cte.Remetente.CPF_CNPJ.Length == 14 ? "J" : "F", StringConexao);
                    remetente.Bairro = cte.Remetente.Bairro;
                    remetente.CEP = cte.Remetente.CEP;
                    remetente.Complemento = cte.Remetente.Complemento;
                    remetente.DataCadastro = DateTime.Now;
                    remetente.Endereco = cte.Remetente.Endereco;
                    remetente.IE_RG = cte.Remetente.IE_RG;
                    remetente.InscricaoMunicipal = cte.Remetente.InscricaoMunicipal;
                    remetente.InscricaoSuframa = cte.Remetente.InscricaoSuframa;
                    remetente.Localidade = localidadeRemetente;
                    remetente.Nome = cte.Remetente.Nome;
                    remetente.NomeFantasia = cte.Remetente.NomeFantasia;
                    remetente.Numero = cte.Remetente.Numero;

                    remetente.Telefone1 = cte.Remetente.Telefone1 ?? string.Empty;
                    remetente.Telefone2 = cte.Remetente.Telefone2 ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(remetente.Telefone1) && remetente.Telefone1.Length > 20)
                        remetente.Telefone1 = remetente.Telefone1.Substring(0, 20);

                    if (!string.IsNullOrWhiteSpace(remetente.Telefone2) && remetente.Telefone2.Length > 20)
                        remetente.Telefone2 = remetente.Telefone2.Substring(0, 20);

                    remetente.Tipo = cte.Remetente.CPF_CNPJ.Length == 14 ? "J" : "F";

                    if (remetente.Tipo == "J" && remetente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(remetente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            remetente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    remetente.Ativo = true;
                    repCliente.Inserir(remetente);
                }

                dadosRemetente = repDadosCliente.Buscar(this.Empresa.Codigo, cpfCnpjRemetente);
                cte.SetarParticipante(remetente, Dominio.Enumeradores.TipoTomador.Remetente, null, dadosRemetente);
            }
            else
            {
                throw new Exception("O remetente/estado/cidade não consta no arquivo (NOTFIS).");
            }

            double inscricaoDestinatario = 0;
            double.TryParse(Utilidades.String.OnlyNumbers(cte.Destinatario.IE_RG), out inscricaoDestinatario);
            if (inscricaoDestinatario == 0)
                cte.Destinatario.IE_RG = "ISENTO";

            if (string.IsNullOrWhiteSpace(cte.Destinatario.Numero))
                cte.Destinatario.Numero = "S/N";

            if (cte.Destinatario != null && cte.Destinatario.Localidade != null && cte.Destinatario.Localidade.Estado != null)
            {
                Dominio.Entidades.Cliente destinatario = null;
                Dominio.Entidades.DadosCliente dadosDestinatario = null;

                double cpfCnpjDestinatario;
                double.TryParse(cte.Destinatario.CPF_CNPJ, out cpfCnpjDestinatario);

                destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

                if (destinatario == null)
                {
                    Dominio.Entidades.Localidade localidadeDestinatario = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(cte.Destinatario.Localidade.Descricao), cte.Destinatario.Localidade.Estado.Sigla);

                    if (localidadeDestinatario == null)
                    {
                        localidadeDestinatario = repLocalidade.BuscarPorCodigoIBGE(cte.Destinatario.Localidade.CodigoIBGE);
                        if (localidadeDestinatario == null && !string.IsNullOrWhiteSpace(cte.Destinatario.CEP) && cte.Destinatario.CEP.Length > 3)
                        {
                            localidadeDestinatario = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(cte.Destinatario.CEP.Substring(0, 3)));
                        }
                        if (localidadeDestinatario == null)
                            throw new Exception("A localidade do destinatario não foi encontrada para salvar o CT-e (NOTFIS). " + cte.Destinatario.Localidade.Descricao + " " + cte.Destinatario.Localidade.Estado.Sigla + " " + cte.Destinatario.Localidade.CodigoIBGE);
                    }

                    destinatario = new Dominio.Entidades.Cliente();
                    destinatario.CPF_CNPJ = cpfCnpjDestinatario;

                    destinatario.Bairro = cte.Destinatario.Bairro;
                    destinatario.CEP = cte.Destinatario.CEP;
                    destinatario.Complemento = cte.Destinatario.Complemento;
                    destinatario.DataCadastro = DateTime.Now;
                    destinatario.Endereco = cte.Destinatario.Endereco;
                    destinatario.IE_RG = cte.Destinatario.IE_RG;
                    destinatario.InscricaoMunicipal = cte.Destinatario.InscricaoMunicipal;
                    destinatario.InscricaoSuframa = cte.Destinatario.InscricaoSuframa;
                    destinatario.Localidade = localidadeDestinatario;
                    destinatario.Nome = cte.Destinatario.Nome;
                    destinatario.NomeFantasia = cte.Destinatario.NomeFantasia;
                    destinatario.Numero = cte.Destinatario.Numero == "" ? "S/N" : cte.Destinatario.Numero;

                    destinatario.Telefone1 = cte.Destinatario.Telefone1 ?? string.Empty;
                    destinatario.Telefone2 = cte.Destinatario.Telefone2 ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(destinatario.Telefone1) && destinatario.Telefone1.Length > 20)
                        destinatario.Telefone1 = destinatario.Telefone1.Substring(0, 20);

                    if (!string.IsNullOrWhiteSpace(destinatario.Telefone2) && destinatario.Telefone2.Length > 20)
                        destinatario.Telefone2 = destinatario.Telefone2.Substring(0, 20);

                    if (Utilidades.Validate.ValidarCNPJ(cte.Destinatario.CPF_CNPJ))
                    {
                        destinatario.Atividade = Atividade.ObterAtividade(this.Empresa.Codigo, "J", StringConexao);
                        destinatario.Tipo = "J";
                    }
                    else if (Utilidades.Validate.ValidarCPF(cte.Destinatario.CPF_CNPJ.Length > 11 ? cte.Destinatario.CPF_CNPJ.Substring(cte.Destinatario.CPF_CNPJ.Length - 11, 11) : cte.Destinatario.CPF_CNPJ))
                    {
                        destinatario.Atividade = Atividade.ObterAtividade(this.Empresa.Codigo, "F", StringConexao);
                        destinatario.Tipo = "F";
                        destinatario.IE_RG = "ISENTO";
                    }
                    else
                    {
                        destinatario.Atividade = Atividade.ObterAtividade(this.Empresa.Codigo, cte.Destinatario.CPF_CNPJ.Length == 14 ? "J" : "F", StringConexao);
                        destinatario.Tipo = cte.Destinatario.CPF_CNPJ.Length == 14 ? "J" : "F";
                    }
                    if (destinatario.Tipo == "J" && destinatario.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(destinatario.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            destinatario.GrupoPessoas = grupoPessoas;
                        }
                    }
                    destinatario.Ativo = true;
                    repCliente.Inserir(destinatario);
                }

                dadosDestinatario = repDadosCliente.Buscar(this.Empresa.Codigo, cpfCnpjDestinatario);
                cte.SetarParticipante(destinatario, Dominio.Enumeradores.TipoTomador.Destinatario, null, dadosDestinatario);
            }
            else
            {
                throw new Exception("O destinatário/estado/cidade não consta no arquivo (NOTFIS).");
            }

            cte.LocalidadeEmissao = cte.Empresa.Localidade;
            cte.LocalidadeInicioPrestacao = cte.Remetente.Localidade;
            cte.LocalidadeTerminoPrestacao = cte.Destinatario.Localidade;
            if (cte.Empresa.Localidade.Estado.Sigla != cte.LocalidadeInicioPrestacao.Estado.Sigla)
                cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
            else
                cte.SimplesNacional = cte.Empresa.OptanteSimplesNacional ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.DataEmissao = DateTime.Now;
            cte.DataPrevistaEntrega = DateTime.Now.AddDays(cte.Empresa.Configuracao.DiasParaEntrega);
            cte.ExibeICMSNaDACTE = true;
            cte.IncluirICMSNoFrete = cte.Empresa.Configuracao != null ? cte.Empresa.Configuracao.IncluirICMSNoFrete : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false);
            cte.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("57");
            cte.PercentualICMSIncluirNoFrete = 100;
            cte.ProdutoPredominante = cte.Empresa.Configuracao.ProdutoPredominante;
            cte.OutrasCaracteristicasDaCarga = cte.Empresa.Configuracao.OutrasCaracteristicas;
            cte.Retira = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.RNTRC = cte.Empresa.RegistroANTT;

            Dominio.Entidades.EmpresaSerie serieCTe = null;
            if (cte.LocalidadeInicioPrestacao.Estado.Sigla != cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                serieCTe = cte.Empresa.Configuracao.SerieInterestadual;
            else
                serieCTe = cte.Empresa.Configuracao.SerieIntraestadual;

            if (this.Usuario != null && this.Usuario.Series != null && this.Usuario.Series.Count > 0)
            { //Verifica se usuário possui a série configurada, caso não possua utiliza a série do seu usuário
                if ((from o in this.Usuario.Series where o.Status == "A" && o.Tipo == Dominio.Enumeradores.TipoSerie.CTe && o.Numero == serieCTe.Numero select o).Count() == 0)
                    serieCTe = (from o in this.Usuario.Series where o.Status == "A" && o.Tipo == Dominio.Enumeradores.TipoSerie.CTe select o).FirstOrDefault();
            }

            cte.Serie = serieCTe;
            cte.TipoAmbiente = cte.Empresa.TipoAmbiente;
            cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
            cte.TipoEmissao = "0";
            cte.TipoImpressao = cte.Empresa.Configuracao.TipoImpressao;
            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;

            cte.ValorTotalMercadoria = (from obj in documentos select obj.Valor).Sum();
            string versao = "4.00";
            if (cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.VersaoCTe))
                versao = cte.Empresa.Configuracao.VersaoCTe;
            else if (cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.VersaoCTe))
                versao = cte.Empresa.EmpresaPai.Configuracao.VersaoCTe;
            cte.Versao = versao;
            cte.Status = "S";
            cte.Cancelado = "N";

            svcCTe.SetarCFOPENaturezaPorTabelaDeAliquotas(ref cte, unidadeDeTrabalho);

            cte.Numero = svcCTe.ObterProximoNumero(cte, repCTe);

            Servicos.CTe.SetarTomadorPagadorCTe(ref cte);

            repCTe.Inserir(cte);

            bool carregouTributacaoCTeSemelhante = false;

            if (cte.ValorFrete > 0)
            {
                cte.ValorAReceber = cte.ValorFrete;
                cte.ValorPrestacaoServico = cte.ValorFrete;

                if (svcCTe.CarregarTributacaoCTeSemelhante(ref cte, unidadeDeTrabalho))
                    carregouTributacaoCTeSemelhante = true;
            }
            else if (cte.ValorAReceber > 0)
            {
                decimal aliquotaICMS = 0;

                if (!svcCTe.CarregarTributacaoCTeSemelhante(ref cte, unidadeDeTrabalho))
                {
                    Dominio.Entidades.Aliquota aliquota = svcCTe.ObterAliquota(cte, unidadeDeTrabalho);
                    if (aliquota != null)
                        aliquotaICMS = aliquota.Percentual;
                }
                else
                {
                    carregouTributacaoCTeSemelhante = true;
                    aliquotaICMS = cte.AliquotaICMS;
                }

                if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && aliquotaICMS > 0)
                {
                    if (cte.CST == "60")
                    {
                        cte.ValorFrete = cte.ValorAReceber;
                        cte.ValorPrestacaoServico = Math.Round(cte.ValorAReceber + (cte.ValorAReceber * (aliquotaICMS / 100)), 2, MidpointRounding.ToEven);
                    }
                    else
                    {
                        cte.ValorFrete = Math.Round(cte.ValorAReceber - (cte.ValorAReceber * (aliquotaICMS / 100)), 2, MidpointRounding.ToEven);
                        cte.ValorPrestacaoServico = cte.ValorAReceber;
                    }
                }
                else
                {
                    cte.ValorFrete = cte.ValorAReceber;
                    cte.ValorPrestacaoServico = cte.ValorAReceber;
                }
            }

            this.SalvarDocumentos(cte, documentos, unidadeDeTrabalho);

            this.SalvarQuantidadeDaCarga(cte, documentos, unidadeDeTrabalho);

            this.SalvarComponentesDaPrestacao(cte, unidadeDeTrabalho);

            this.SalvarSeguros(cte, unidadeDeTrabalho);

            this.SalvarVeiculo(ref cte, veiculo, unidadeDeTrabalho);

            this.SalvarVeiculoVinculado(cte, veiculo, unidadeDeTrabalho);

            this.SalvarMotorista(cte, motorista, unidadeDeTrabalho);

            this.SalvarObservacaoContribuinte(cte, listaObsCont, unidadeDeTrabalho);

            if (carregouTributacaoCTeSemelhante)
            {
                svcCTe.CalcularInclusaoICMSNoFrete(ref cte, cte.ValorFrete, cte.ValorFrete, 0, cte.AliquotaICMS, unidadeDeTrabalho);
                svcCTe.CalcularICMS(ref cte, unidadeDeTrabalho);
            }
            else
                svcCTe.CalcularICMSPorTabelaDeAliquotas(ref cte, cte.SimplesNacional, unidadeDeTrabalho);

            svcCTe.SetarObservacaoAvancadaPorRegraICMS(ref cte, unidadeDeTrabalho);
            svcCTe.SetarXCampoVeiculo(cte, unidadeDeTrabalho);
            svcCTe.AdicionarResponsavelSeguroObsContribuinte(cte, unidadeDeTrabalho);

            documentos = new List<Dominio.Entidades.DocumentosCTE>();
            veiculo = null;

            if (this.CTesGerados == null)
                this.CTesGerados = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.CTesGerados.Add(cte);
        }

        private void SalvarVeiculoVinculado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Veiculo veiculo, UnitOfWork unidadeDeTrabalho)
        {
            if (veiculo != null)
            {
                Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    {
                        Dominio.Entidades.VeiculoCTE reboqueCTe = new Dominio.Entidades.VeiculoCTE();
                        reboqueCTe.CTE = cte;
                        reboqueCTe.Veiculo = reboque;
                        reboqueCTe.SetarDadosVeiculo(reboque);
                        repVeiculoCTe.Inserir(reboqueCTe);
                    }
                }
                else if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.VeiculoCTE tracaoCTe = new Dominio.Entidades.VeiculoCTE();
                    tracaoCTe.CTE = cte;
                    tracaoCTe.Veiculo = veiculo.VeiculosTracao.FirstOrDefault();
                    tracaoCTe.SetarDadosVeiculo(veiculo.VeiculosTracao.FirstOrDefault());
                    repVeiculoCTe.Inserir(tracaoCTe);
                }
            }
        }

        private void SalvarVeiculo(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Veiculo veiculo, UnitOfWork unidadeDeTrabalho)
        {
            if (veiculo != null)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.VeiculoCTE repVeiculoCte = new Repositorio.VeiculoCTE(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoCTE veiculoCte = new Dominio.Entidades.VeiculoCTE();
                veiculoCte.SetarDadosVeiculo(veiculo);
                veiculoCte.CTE = cte;
                veiculoCte.Veiculo = veiculo;
                repVeiculoCte.Inserir(veiculoCte);

                cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;
                repCte.Atualizar(cte);
            }
        }

        private void SalvarMotorista(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.MotoristaCTE motorista, UnitOfWork unidadeDeTrabalho)
        {
            if (motorista != null)
            {
                Repositorio.MotoristaCTE repMotoristaCTE = new Repositorio.MotoristaCTE(unidadeDeTrabalho);
                motorista.CTE = cte;
                repMotoristaCTE.Inserir(motorista);
            }
        }

        private void SalvarObservacaoContribuinte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.ObservacaoContribuinteCTE> listaObsCont, UnitOfWork unidadeDeTrabalho)
        {
            if (listaObsCont != null && listaObsCont.Count > 0)
            {
                Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
                foreach (Dominio.Entidades.ObservacaoContribuinteCTE obsCont in listaObsCont)
                {
                    Dominio.Entidades.ObservacaoContribuinteCTE obsContCTe = new Dominio.Entidades.ObservacaoContribuinteCTE();
                    obsContCTe.CTE = cte;
                    obsContCTe.Descricao = obsCont.Descricao;
                    obsContCTe.Identificador = obsCont.Identificador;
                    repObservacaoContribuinteCTE.Inserir(obsContCTe);
                }
            }
        }

        private void SalvarQuantidadeDaCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.DocumentosCTE> documentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.InformacaoCargaCTE repInfoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            Dominio.Entidades.InformacaoCargaCTE infoCarga = new Dominio.Entidades.InformacaoCargaCTE();

            infoCarga.CTE = cte;
            infoCarga.Quantidade = (from obj in documentos select obj.Peso).Sum();
            infoCarga.Tipo = "Kilograma";
            infoCarga.UnidadeMedida = "01";

            repInfoCarga.Inserir(infoCarga);

            decimal quantidadeVolumes = 0;
            quantidadeVolumes = (from obj in documentos select obj.Volume).Sum();
            if (quantidadeVolumes > 0)
            {
                Dominio.Entidades.InformacaoCargaCTE infoCargaVolumes = new Dominio.Entidades.InformacaoCargaCTE();

                infoCargaVolumes.CTE = cte;
                infoCargaVolumes.Quantidade = quantidadeVolumes;
                infoCargaVolumes.Tipo = "Unidade";
                infoCargaVolumes.UnidadeMedida = "03";

                repInfoCarga.Inserir(infoCargaVolumes);
            }

        }

        private void SalvarComponentesDaPrestacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);

            Dominio.Entidades.ComponentePrestacaoCTE componente = new Dominio.Entidades.ComponentePrestacaoCTE();

            componente.CTE = cte;
            componente.Nome = "FRETE VALOR";
            componente.Valor = cte.ValorFrete;
            componente.IncluiNaBaseDeCalculoDoICMS = false;
            componente.IncluiNoTotalAReceber = true;

            repComponentePrestacao.Inserir(componente);
        }

        private void SalvarSeguros(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.SeguroCTE repSeguro = new Repositorio.SeguroCTE(unidadeDeTrabalho);

            bool inseriuSeguro = false;

            List<Dominio.Entidades.SeguroCTE> listaSeguros = repSeguro.BuscarPorCTe(conhecimento.Codigo);

            string tomador = conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? conhecimento.Remetente.CPF_CNPJ : conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? conhecimento.Destinatario.CPF_CNPJ : "0";
            double cnpjTomador = 0;
            double.TryParse(tomador, out cnpjTomador);

            Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
            List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceSeguro.BuscarPorCliente(conhecimento.Empresa.Codigo, conhecimento.Empresa.EmpresaPai != null ? conhecimento.Empresa.Codigo : 0, cnpjTomador);
            if (listaApolices.Count > 0)
            {
                Dominio.Entidades.SeguroCTE seguro = new Dominio.Entidades.SeguroCTE();

                if (listaSeguros.Count > 0)
                {
                    foreach (Dominio.Entidades.SeguroCTE seguroCte in listaSeguros)
                        repSeguro.Deletar(seguroCte);
                }

                seguro.CTE = conhecimento;
                seguro.NumeroApolice = listaApolices[0].NumeroApolice;
                seguro.NomeSeguradora = listaApolices[0].NomeSeguradora;
                seguro.CNPJSeguradora = listaApolices[0].CNPJSeguradora;
                if (listaApolices[0].Responsavel >= 0)
                    seguro.Tipo = (Dominio.Enumeradores.TipoSeguro)listaApolices[0].Responsavel;
                else
                    seguro.Tipo = conhecimento.Empresa.Configuracao.ResponsavelSeguro != null ?
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Destinatario ? Dominio.Enumeradores.TipoSeguro.Destinatario :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoSeguro.Emitente_CTE :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Expedidor ? Dominio.Enumeradores.TipoSeguro.Expedidor :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Recebedor ? Dominio.Enumeradores.TipoSeguro.Recebedor :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Remetente ? Dominio.Enumeradores.TipoSeguro.Remetente :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Tomador_Servico ? Dominio.Enumeradores.TipoSeguro.Tomador_Servico : Dominio.Enumeradores.TipoSeguro.Remetente
                                  : Dominio.Enumeradores.TipoSeguro.Remetente;

                repSeguro.Inserir(seguro);
                inseriuSeguro = true;
            }
            else
            {
                if (listaSeguros.Count == 0)
                {
                    Dominio.Entidades.SeguroCTE seguro = new Dominio.Entidades.SeguroCTE();

                    seguro.CTE = conhecimento;
                    seguro.Tipo = conhecimento.Empresa.Configuracao.ResponsavelSeguro != null ?
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Destinatario ? Dominio.Enumeradores.TipoSeguro.Destinatario :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoSeguro.Emitente_CTE :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Expedidor ? Dominio.Enumeradores.TipoSeguro.Expedidor :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Recebedor ? Dominio.Enumeradores.TipoSeguro.Recebedor :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Remetente ? Dominio.Enumeradores.TipoSeguro.Remetente :
                                  conhecimento.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Tomador_Servico ? Dominio.Enumeradores.TipoSeguro.Tomador_Servico : Dominio.Enumeradores.TipoSeguro.Remetente
                                  : Dominio.Enumeradores.TipoSeguro.Remetente;
                    if (conhecimento.Empresa.Configuracao.ApoliceSeguro != null)
                    {
                        seguro.NumeroApolice = conhecimento.Empresa.Configuracao.ApoliceSeguro.NumeroApolice;
                        seguro.NomeSeguradora = conhecimento.Empresa.Configuracao.ApoliceSeguro.NomeSeguradora;
                        seguro.CNPJSeguradora = conhecimento.Empresa.Configuracao.ApoliceSeguro.CNPJSeguradora;
                    }
                    repSeguro.Inserir(seguro);
                    inseriuSeguro = true;
                }
            }
            if (!inseriuSeguro)
            {
                Dominio.Entidades.SeguroCTE seguro = new Dominio.Entidades.SeguroCTE();

                seguro.CTE = conhecimento;
                seguro.NomeSeguradora = string.Empty;
                seguro.NumeroApolice = string.Empty;
                seguro.NumeroAverbacao = string.Empty;
                seguro.Tipo = Dominio.Enumeradores.TipoSeguro.Remetente;
                seguro.Valor = 0m;

                repSeguro.Inserir(seguro);
            }

        }

        #endregion
    }
}
