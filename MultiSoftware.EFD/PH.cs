using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.Entidades;
using Dominio.Entidades.EFD;
using Dominio.Entidades.EFD.PH;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace MultiSoftware.EFD
{
    public class PH
    {
        #region Construtores

        public PH(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, int codigoCarga, int codigoDocumentoEntrada, int codigoNotaSaida, string versaoAtoCOTEPE, List<string> registrosNaoGerar, Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.VersaoAtoCOTEPE = int.Parse(versaoAtoCOTEPE);
            this.Clientes = new List<Dominio.Entidades.Cliente>();
            this.ArquivoPH = new ArquivoPH();
            this.RegistrosNaoGerar = registrosNaoGerar;
            this.TipoEmissao = "Docs";
            this.CodigoDocumento = 1;
            if (codigoCarga > 0)
            {
                this.TipoEmissao = "AutorizacaoCarga";
                this.CodigoDocumento = codigoCarga;
            }

            this.Produtos = new List<Produto>();
            this.UnidadesDeMedida = new List<UnidadeMedidaGeral>();
            this.CFOPs = new List<CFOP>();

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            this.CTes = repCargaCTe.BuscarCTePorCarga(codigoCarga);

            int[] codigoCTes = null;
            if (this.CTes != null)
                codigoCTes = (from obj in this.CTes select obj.Codigo).ToArray();

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
            this.ItensDocumentosEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);

            //Repositorio.ApuracaoICMS repApuracaoICMS = new Repositorio.ApuracaoICMS(StringConexao);
            this.ApuracaoICMS = null;// repApuracaoICMS.BuscarPorPeriodo(empresa.Codigo, dataInicial, dataFinal);
        }

        #endregion

        #region Propriedades

        private Repositorio.UnitOfWork unitOfWork;

        private Dominio.Entidades.Empresa Empresa;

        private Dominio.Entidades.ApuracaoICMS ApuracaoICMS;

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes;

        private List<Dominio.Entidades.Cliente> Clientes;

        private List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> DocumentosEntrada;

        private List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> ItensDocumentosEntrada;

        private List<Dominio.Entidades.Produto> Produtos;

        private List<C190> ListaC190 = new List<C190>();

        private List<C590> ListaC590 = new List<C590>();

        private List<Dominio.Entidades.CFOP> CFOPs { get; set; }

        private List<Dominio.Entidades.UnidadeMedidaGeral> UnidadesDeMedida;

        private DateTime DataInicial, DataFinal;

        private int VersaoAtoCOTEPE;

        private ArquivoPH ArquivoPH;

        private List<string> RegistrosNaoGerar;

        private string TipoEmissao;
        private int CodigoDocumento;


        #endregion

        #region Métodos

        public System.IO.MemoryStream GerarPH()
        {

            if (VersaoAtoCOTEPE >= 13)
                this.ArquivoPH.Blocos.Add(this.ObterBlocoB());

            this.ArquivoPH.Blocos.Add(this.ObterBlocoC());

            this.ArquivoPH.Blocos.Add(this.ObterBlocoD());

            //this.ArquivoPH.Blocos.Add(this.ObterBlocoE());

            //this.ArquivoPH.Blocos.Add(this.ObterBlocoG());

            //this.ArquivoPH.Blocos.Add(this.ObterBlocoH());

            //this.ArquivoPH.Blocos.Add(this.ObterBlocoK());

            //this.ArquivoPH.Blocos.Add(this.ObterBloco1());

            this.ArquivoPH.Blocos.Add(this.ObterBloco0()); //Sempre deixar para o fim o bloco 0, pois contém os dados utilizados nos outros blocos (clientes, observações, etc...)

            this.ArquivoPH.Blocos.Add(this.ObterBloco9());

            return this.ArquivoPH.ObterArquivo();
        }

        #region Bloco 0

        private BlocoPH ObterBloco0()
        {
            BlocoPH bloco0 = new BlocoPH("0");

            bloco0.RegistrosPH.Add(this.ObterRegistro0000());
            bloco0.RegistrosPH.Add(this.ObterRegistro0001());
            //bloco0.RegistrosPH.Add(new _0990(bloco0.ObterTotalDeRegistros()));

            return bloco0;
        }

        private _0000 ObterRegistro0000()
        {
            _0000 registro0000 = new _0000();

            registro0000.DataFinal = this.DataFinal;
            registro0000.DataInicial = this.DataInicial;
            registro0000.Empresa = this.Empresa;
            registro0000.FinalidadeArquivo = Dominio.Enumeradores.FinalidadeDoArquivoPH.RemessaOriginal;
            registro0000.PerfilDeApresentacao = "B";
            registro0000.TipoDeAtividade = Dominio.Enumeradores.TipoDeAtividade.Outros;
            registro0000.VersaoAtoCOTEPE = this.VersaoAtoCOTEPE;
            registro0000.IDE_PH = "_SPED_PH_";
            registro0000.ID_UNICO = this.TipoEmissao + "_" + this.CodigoDocumento.ToString("D");

            return registro0000;
        }

        private _0001 ObterRegistro0001()
        {
            _0001 registro0001 = new _0001();

            //registro0001.Registros.Add(this.ObterRegistro0005());

            //if (this.Empresa.Contador != null)
            //    registro0001.Registros.Add(this.ObterRegistro0100());

            foreach (Dominio.Entidades.Cliente cliente in this.Clientes)
                registro0001.Registros.Add(this.ObterRegistro0150(cliente));

            registro0001.Registros.AddRange(this.ObterRegistros0190());

            registro0001.Registros.AddRange(this.ObterRegistros0200());

            registro0001.Registros.AddRange(this.ObterRegistros0400());

            return registro0001;
        }

        private _0005 ObterRegistro0005()
        {
            _0005 registro0005 = new _0005();

            registro0005.Empresa = this.Empresa;

            return registro0005;
        }

        private _0100 ObterRegistro0100()
        {
            _0100 registro0100 = new _0100();

            registro0100.Contador = this.Empresa.Contador;
            registro0100.CRC = this.Empresa.CRCContador;

            return registro0100;
        }

        private _0150 ObterRegistro0150(Dominio.Entidades.Cliente cliente)
        {
            _0150 registro0150 = new _0150();

            registro0150.Cliente = cliente;

            return registro0150;
        }

        private List<_0190> ObterRegistros0190()
        {
            List<_0190> registros = new List<_0190>();

            registros.AddRange((from obj in this.UnidadesDeMedida select new _0190() { UnidadeMedida = obj }).ToList());

            return registros;
        }

        private List<_0200> ObterRegistros0200()
        {
            List<_0200> registros = new List<_0200>();

            registros.AddRange((from obj in this.Produtos select new _0200() { Produto = obj }).ToList());

            return registros;
        }

        private List<_0400> ObterRegistros0400()
        {
            List<_0400> registros = new List<_0400>();

            registros.AddRange((from obj in this.CFOPs select new _0400() { CFOP = obj }).ToList());

            return registros;
        }

        #endregion

        #region Bloco B

        private BlocoPH ObterBlocoB()
        {
            BlocoPH blocoB = new BlocoPH("B");

            blocoB.RegistrosPH.Add(new B001());
            blocoB.RegistrosPH.Add(new B990(blocoB.ObterTotalDeRegistros()));

            return blocoB;
        }

        #endregion

        #region Bloco C

        private BlocoPH ObterBlocoC()
        {
            BlocoPH blocoC = new BlocoPH("C");

            C001 registro = new C001();

            registro.Registros.AddRange(this.ObterRegistrosC100());

            registro.Registros.AddRange(this.ObterRegistrosC500());

            blocoC.RegistrosPH.Add(registro);

            blocoC.RegistrosPH.Add(new C990(blocoC.ObterTotalDeRegistros()));

            return blocoC;
        }

        private List<C100> ObterRegistrosC100()
        {
            List<C100> registros = new List<C100>();

            if (this.DocumentosEntrada == null)
                return registros;

            string[] modelos = { "01", "1B", "04", "55", "65" };

            IEnumerable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                C100 registroC100 = new C100();

                registroC100.DocumentoEntrada = documento;

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
                {
                    C170 registroC170 = new C170();

                    registroC170.Item = item;
                    registroC170.versaoAtoCOTEPE = VersaoAtoCOTEPE;

                    registroC100.Registros.Add(registroC170);

                    this.AdicionarProduto(item.Produto);

                    this.AdicionarUnidadeMedidaGeral(UnidadeDeMedidaHelper.ObterSigla(item.UnidadeMedida), UnidadeDeMedidaHelper.ObterDescricao(item.UnidadeMedida));

                    this.AdicionarCFOP(item.CFOP);
                }
                var rangeC190 = from obj in itens
                                group new
                                {
                                    obj.AliquotaICMS,
                                    obj.CFOP.CodigoCFOP,
                                    obj.CSTICMS,
                                    obj.ValorTotal,
                                    obj.ValorICMS,
                                    obj.ValorIPI,
                                    obj.ValorICMSST,
                                    obj.BaseCalculoICMS,
                                    obj.BaseCalculoICMSST,
                                    obj.Desconto,
                                    obj.OutrasDespesas,
                                    obj.ValorFrete
                                }
                                by new
                                {
                                    obj.AliquotaICMS,
                                    obj.CSTICMS,
                                    obj.CFOP.CodigoCFOP
                                } into grupo
                                select new C190()
                                {
                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                    CFOP = grupo.Key.CodigoCFOP,
                                    CSTICMS = grupo.Key.CSTICMS,
                                    ValorICMS = grupo.Sum(o => o.ValorICMS),
                                    ValorIPI = grupo.Sum(o => o.ValorIPI),
                                    ValorICMSST = grupo.Sum(o => o.BaseCalculoICMSST) > 0 ? grupo.Sum(o => o.ValorICMSST) : 0,
                                    ValorOperacao = grupo.Sum(o => o.ValorTotal - o.Desconto + o.OutrasDespesas + o.ValorICMSST + o.ValorFrete),
                                    ValorReducaoBaseCalculo = 0m
                                };

                registroC100.Registros.AddRange(rangeC190);

                this.ListaC190.AddRange(rangeC190);

                registros.Add(registroC100);

                this.AdicionarCliente(documento.Fornecedor);
            }

            return registros;
        }

        private List<C500> ObterRegistrosC500()
        {
            List<C500> registros = new List<C500>();
            if (this.DocumentosEntrada == null)
                return registros;

            string[] modelos = { "06", "29", "28" };

            IEnumerable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                C500 registroC500 = new C500();

                registroC500.DocumentoEntrada = documento;
                registroC500.versaoAtoCOTEPE = VersaoAtoCOTEPE;

                var rangeC590 = from obj in itens
                                group new
                                {
                                    obj.AliquotaICMS,
                                    obj.CFOP.CodigoCFOP,
                                    obj.CSTICMS,
                                    obj.ValorTotal,
                                    obj.ValorICMS,
                                    obj.ValorIPI,
                                    obj.ValorICMSST,
                                    obj.BaseCalculoICMS,
                                    obj.BaseCalculoICMSST,
                                    obj.Desconto,
                                    obj.OutrasDespesas,
                                    obj.ValorFrete
                                }
                                by new
                                {
                                    obj.AliquotaICMS,
                                    obj.CSTICMS,
                                    obj.CFOP.CodigoCFOP
                                } into grupo
                                select new C590()
                                {
                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                    CFOP = grupo.Key.CodigoCFOP,
                                    CSTICMS = grupo.Key.CSTICMS,
                                    ValorICMS = grupo.Sum(o => o.ValorICMS),
                                    ValorIPI = grupo.Sum(o => o.ValorIPI),
                                    ValorICMSST = grupo.Sum(o => o.BaseCalculoICMSST) > 0 ? grupo.Sum(o => o.ValorICMSST) : 0,
                                    ValorOperacao = grupo.Sum(o => o.ValorTotal - o.Desconto + o.OutrasDespesas + o.ValorICMSST + o.ValorFrete),
                                    ValorReducaoBaseCalculo = 0m
                                };

                registroC500.Registros.AddRange(rangeC590);

                this.ListaC590.AddRange(rangeC590);

                registros.Add(registroC500);

                this.AdicionarCliente(documento.Fornecedor);
            }

            return registros;
        }

        #endregion

        #region Bloco D

        private BlocoPH ObterBlocoD()
        {
            BlocoPH blocoD = new BlocoPH("D");

            blocoD.RegistrosPH.Add(this.ObterRegistroD001());

            blocoD.RegistrosPH.Add(new D990(blocoD.ObterTotalDeRegistros()));

            return blocoD;
        }

        private D001 ObterRegistroD001()
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.MovimentoDoFinanceiro repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(unitOfWork);

            D001 registroD001 = new D001();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
            {
                registroD001.Registros.Add(this.ObterRegistroD100(cte));

                //if (!cte.Status.Equals("D") && !cte.Status.Equals("I") && !cte.Status.Equals("C") && cte.Tomador != null)
                if (!(from obj in this.Clientes where obj.CPF_CNPJ_SemFormato.Equals(cte.TomadorPagador.CPF_CNPJ) select obj).Any())
                    this.Clientes.Add(repCliente.BuscarPorCPFCNPJ(double.Parse(!string.IsNullOrWhiteSpace(cte.TomadorPagador.CPF_CNPJ) ? cte.TomadorPagador.CPF_CNPJ : "99999999999999")));
            }

            registroD001.Registros.AddRange(this.ObterRegistrosD500());

            return registroD001;
        }

        private D100 ObterRegistroD100(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            D100 registroD100 = new D100();

            registroD100.CTe = cte;
            registroD100.versaoAtoCOTEPE = VersaoAtoCOTEPE;

            if (cte.Status.Equals("A"))
            {
                if (!this.RegistrosNaoGerar.Contains("D160"))
                    registroD100.Registros.Add(this.ObterRegistroD160(cte));

                //registroD100.Registros.Add(this.ObterRegistroD190(cte));
            }

            return registroD100;
        }

        private D160 ObterRegistroD160(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unitOfWork);

            D160 registroD160 = new D160();

            registroD160.CTe = cte;

            List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentos.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                if (documento.ModeloDocumentoFiscal != null &&
                    (cte.CFOP.CodigoCFOP != 5359 &&
                     cte.CFOP.CodigoCFOP != 6359) &&
                    (documento.ModeloDocumentoFiscal.Numero == "01" ||
                    documento.ModeloDocumentoFiscal.Numero == "1B" ||
                    documento.ModeloDocumentoFiscal.Numero == "04" ||
                    documento.ModeloDocumentoFiscal.Numero == "55"))
                    registroD160.Registros.Add(this.ObterRegistroD162(documento));
            }

            return registroD160;
        }

        private D162 ObterRegistroD162(Dominio.Entidades.DocumentosCTE documento)
        {
            D162 registroD162 = new D162();

            registroD162.Documento = documento;

            return registroD162;
        }

        private D190 ObterRegistroD190(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            D190 registroD190 = new D190();

            registroD190.CTe = cte;

            return registroD190;
        }

        private List<D500> ObterRegistrosD500()
        {
            List<D500> registros = new List<D500>();
            if (this.DocumentosEntrada == null)
                return registros;

            string[] modelos = { "21", "22" };

            IEnumerable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                D500 registroD500 = new D500();

                registroD500.Documento = documento;

                registroD500.Registros.AddRange(from obj in itens
                                                group new
                                                {
                                                    obj.AliquotaICMS,
                                                    obj.CFOP.CodigoCFOP,
                                                    obj.CSTICMS,
                                                    obj.ValorTotal,
                                                    obj.ValorICMS,
                                                    obj.ValorIPI,
                                                    obj.ValorICMSST,
                                                    obj.BaseCalculoICMS,
                                                    obj.BaseCalculoICMSST,
                                                    obj.Desconto,
                                                    obj.OutrasDespesas,
                                                    obj.ValorFrete
                                                }
                                                by new
                                                {
                                                    obj.AliquotaICMS,
                                                    obj.CSTICMS,
                                                    obj.CFOP.CodigoCFOP
                                                }
                                                    into grupo
                                                select new D590()
                                                {
                                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                                    CFOP = grupo.Key.CodigoCFOP,
                                                    CSTICMS = grupo.Key.CSTICMS,
                                                    ValorICMS = grupo.Sum(o => o.ValorICMS),
                                                    ValorICMSST = grupo.Sum(o => o.BaseCalculoICMSST) > 0 ? grupo.Sum(o => o.ValorICMSST) : grupo.Sum(o => o.ValorICMSST),
                                                    ValorOperacao = grupo.Sum(o => o.ValorTotal - o.Desconto + o.OutrasDespesas + o.ValorICMSST + o.ValorFrete),
                                                    ValorReducaoBaseCalculo = 0m
                                                });

                registros.Add(registroD500);

                this.AdicionarCliente(documento.Fornecedor);
            }

            return registros;
        }

        #endregion

        #region Bloco E

        private BlocoPH ObterBlocoE()
        {
            BlocoPH blocoE = new BlocoPH("E");

            E001 registroE001 = new E001();

            if (this.ApuracaoICMS != null)
            {
                registroE001.Registros.Add(this.ObterRegistroE100());
            }
            registroE001.Registros.Add(this.ObterRegistroE200());

            blocoE.RegistrosPH.Add(registroE001);

            blocoE.RegistrosPH.Add(new E990(blocoE.ObterTotalDeRegistros()));

            return blocoE;
        }

        private E100 ObterRegistroE100()
        {
            E100 registroE100 = new E100();

            registroE100.DataFinal = this.DataFinal;
            registroE100.DataInicial = this.DataInicial;

            registroE100.Registros.Add(this.ObterRegistroE110());

            return registroE100;
        }

        private E110 ObterRegistroE110()
        {
            E110 registroE110 = new E110();

            registroE110.ValorTotalCreditos = this.ApuracaoICMS.ValorCreditos;
            registroE110.ValorTotalDebitos = this.ApuracaoICMS.ValorDebitos;
            registroE110.ValorICMSRecolher = this.ApuracaoICMS.ValorICMSRecolher;
            registroE110.ValorSaldoCredorAnterior = this.ApuracaoICMS.ValorCreditosPeriodoAnterior;
            registroE110.ValorSaldoCredorTransportar = this.ApuracaoICMS.ValorSaldoCredorTransportar;
            registroE110.ValorSaldoDevedorApurado = this.ApuracaoICMS.ValorICMSRecolher;

            return registroE110;
        }

        private E200 ObterRegistroE200()
        {
            E200 registroE200 = new E200();

            registroE200.Empresa = this.Empresa;
            registroE200.DataFinal = this.DataFinal;
            registroE200.DataInicial = this.DataInicial;

            E210 registroE210 = this.ObterRegistroE210();
            registroE200.Registros.Add(registroE210);

            if (registroE210.ImpostoARecolher > 0)
                registroE200.Registros.Add(this.ObterRegistroE250(registroE210.ImpostoARecolher, registroE210.ValorRecolhidoExtraApuracao));

            return registroE200;
        }

        private E210 ObterRegistroE210()
        {
            E210 registroE210 = new E210();

            decimal VL_SLD_CRED_ANT_ST = 0;
            decimal VL_DEVOL_ST = 0;
            decimal VL_RESSARC_ST = 0;
            decimal VL_OUT_CRED_ST = 0;
            decimal VL_AJ_CREDITOS_ST = 0;
            decimal VL_RETENCAO_ST = 0;
            decimal VL_OUT_DEB_ST = 0;
            decimal VL_AJ_DEBITOS_ST = 0;
            decimal VL_SLD_DEV_ANT_ST = 0;
            decimal VL_DEDUCOES_ST = 0;
            decimal VL_ICMS_RECOL_ST = 0;
            decimal VL_SLD_CRED_ST_TRANSPORTAR = 0;
            decimal DEB_ESP_ST = 0;

            string[] cfopDevolucao = new string[14] { "1410", "1411", "1414", "1415", "1660", "1661", "1662", "2410", "2411", "2414", "2415", "2660", "2661", "2662" };
            string[] cfopCredito = new string[15] { "1410", "1411", "1414", "1415", "1660", "1661", "1662", "2410", "2411", "2414", "2415", "2660", "2661", "2662", "2403" };
            string[] cfopRessarcimento = new string[2] { "1603", "2603" };

            foreach (C190 itemC190 in this.ListaC190)
            {
                string cfop = itemC190.CFOP.ToString();
                decimal valorICMSST = itemC190.ValorICMSST;

                if (cfopDevolucao.Contains(cfop))
                    VL_DEVOL_ST = VL_DEVOL_ST + valorICMSST;

                if (cfopRessarcimento.Contains(cfop))
                    VL_RESSARC_ST = VL_RESSARC_ST + valorICMSST;

                if ((cfop.StartsWith("1") || cfop.StartsWith("2")) && !(cfopCredito.Contains(cfop)))
                    VL_OUT_CRED_ST = VL_OUT_CRED_ST + valorICMSST;

                if (cfop.StartsWith("5") || cfop.StartsWith("6"))
                    VL_RETENCAO_ST = VL_RETENCAO_ST + valorICMSST;
            }

            VL_SLD_DEV_ANT_ST = (VL_RETENCAO_ST + VL_OUT_DEB_ST + VL_AJ_DEBITOS_ST) - (VL_SLD_CRED_ANT_ST + VL_DEVOL_ST + VL_RESSARC_ST + VL_OUT_CRED_ST + VL_AJ_CREDITOS_ST);
            VL_ICMS_RECOL_ST = VL_SLD_DEV_ANT_ST - VL_DEDUCOES_ST;

            VL_SLD_CRED_ST_TRANSPORTAR = (VL_SLD_CRED_ANT_ST + VL_DEVOL_ST + VL_RESSARC_ST + VL_OUT_CRED_ST + VL_AJ_CREDITOS_ST) - (VL_RETENCAO_ST + VL_OUT_DEB_ST + VL_AJ_DEBITOS_ST);


            registroE210.ValorSaldoCredor = VL_SLD_CRED_ANT_ST > 0 ? VL_SLD_CRED_ANT_ST : 0;
            registroE210.ValorTotalDevolucao = VL_DEVOL_ST > 0 ? VL_DEVOL_ST : 0;
            registroE210.ValorTotalRessarcimentos = VL_RESSARC_ST > 0 ? VL_RESSARC_ST : 0;
            registroE210.ValorAjustesEstornosCreditos = VL_OUT_CRED_ST > 0 ? VL_OUT_CRED_ST : 0;
            registroE210.ValorTotalAjustesCreditos = VL_AJ_CREDITOS_ST > 0 ? VL_AJ_CREDITOS_ST : 0;
            registroE210.ValorTotalICMSRetido = VL_RETENCAO_ST > 0 ? VL_RETENCAO_ST : 0;
            registroE210.ValorAjustesEstornosDebitos = VL_OUT_DEB_ST > 0 ? VL_OUT_DEB_ST : 0;
            registroE210.ValorTotalAjustesDebitos = VL_AJ_DEBITOS_ST > 0 ? VL_AJ_DEBITOS_ST : 0;
            registroE210.ValorSaldoCredorAnterior = VL_SLD_DEV_ANT_ST > 0 ? VL_SLD_DEV_ANT_ST : 0;
            registroE210.ValorSaldoAjustesDeducao = VL_DEDUCOES_ST > 0 ? VL_DEDUCOES_ST : 0;
            registroE210.ImpostoARecolher = VL_ICMS_RECOL_ST > 0 ? VL_ICMS_RECOL_ST : 0;
            registroE210.ValorSaldoCredorTransportar = VL_SLD_CRED_ST_TRANSPORTAR > 0 ? VL_SLD_CRED_ST_TRANSPORTAR : 0;
            registroE210.ValorRecolhidoExtraApuracao = DEB_ESP_ST > 0 ? DEB_ESP_ST : 0;
            registroE210.IndicadorDeMovimento = (VL_SLD_CRED_ANT_ST != 0 || VL_DEVOL_ST != 0 || VL_RESSARC_ST != 0 || VL_OUT_CRED_ST != 0 || VL_AJ_CREDITOS_ST != 0 || VL_RETENCAO_ST != 0 ||
                                                 VL_OUT_DEB_ST != 0 || VL_AJ_DEBITOS_ST != 0 || VL_SLD_DEV_ANT_ST != 0 || VL_DEDUCOES_ST != 0 || VL_ICMS_RECOL_ST != 0 || VL_SLD_CRED_ST_TRANSPORTAR != 0 || DEB_ESP_ST != 0);

            return registroE210;
        }

        private E250 ObterRegistroE250(decimal VL_ICMS_RECOL_ST, decimal DEB_ESP_ST)
        {
            E250 registroEE250 = new E250();

            DateTime DT_VCTO = new DateTime(this.DataFinal.Year, this.DataFinal.Month, 20).AddMonths(1);
            string MES_REF = (this.DataFinal.Month < 10 ? "0" : "") + this.DataFinal.Month.ToString() + this.DataFinal.Year.ToString();

            registroEE250.CodigoObrigacaoRecolher = "002";
            registroEE250.ValorObrigacaoRecolher = VL_ICMS_RECOL_ST + DEB_ESP_ST;
            registroEE250.DataObrigacaoRecolher = DT_VCTO;
            registroEE250.CodigoReceitaObrigacao = 0;
            registroEE250.NumeroProcesso = "";
            registroEE250.IndicadorOrigemProcesso = "";
            registroEE250.DescricaoResumidaProcesso = "";
            registroEE250.DescricaoComplementarObrigacoes = "";
            registroEE250.MesReferencia = MES_REF;

            return registroEE250;
        }

        #endregion

        #region Bloco G

        private BlocoPH ObterBlocoG()
        {
            BlocoPH blocoG = new BlocoPH("G");

            blocoG.RegistrosPH.Add(new G001());
            blocoG.RegistrosPH.Add(new G990(blocoG.ObterTotalDeRegistros()));

            return blocoG;
        }

        #endregion

        #region Bloco H

        private BlocoPH ObterBlocoH()
        {
            BlocoPH blocoH = new BlocoPH("H");

            blocoH.RegistrosPH.Add(new H001());
            blocoH.RegistrosPH.Add(new H990(blocoH.ObterTotalDeRegistros()));

            return blocoH;
        }

        #endregion

        #region Bloco K

        private BlocoPH ObterBlocoK()
        {
            BlocoPH blocoK = new BlocoPH("K");

            blocoK.RegistrosPH.Add(new K001());
            blocoK.RegistrosPH.Add(new K990(blocoK.ObterTotalDeRegistros()));

            return blocoK;
        }

        #endregion

        #region Bloco 1

        private BlocoPH ObterBloco1()
        {
            BlocoPH bloco1 = new BlocoPH("z1");

            _1001 registro1001 = new _1001();

            registro1001.Registros.Add(this.ObterRegistro1010());

            bloco1.RegistrosPH.Add(registro1001);

            bloco1.RegistrosPH.Add(new _1990(bloco1.ObterTotalDeRegistros()));

            return bloco1;
        }

        private _1010 ObterRegistro1010()
        {
            _1010 registro1010 = new _1010();
            registro1010.versaoAtoCOTEPE = VersaoAtoCOTEPE;

            return registro1010;
        }

        #endregion

        #region Bloco 9

        private BlocoPH ObterBloco9()
        {
            BlocoPH bloco9 = new BlocoPH("z9");

            //bloco9.RegistrosPH.Add(this.ObterRegistro9001());
            bloco9.RegistrosPH.Add(new _9990(bloco9.ObterTotalDeRegistros()));

            int totalRegistrosArquivo = (from obj in this.ArquivoPH.Blocos select obj.ObterTotalDeRegistros()).Sum() + bloco9.ObterTotalDeRegistros();

            bloco9.RegistrosPH.Add(new _9999(totalRegistrosArquivo));

            return bloco9;
        }

        private _9001 ObterRegistro9001()
        {
            _9001 registro9001 = new _9001();

            registro9001.Registros.AddRange(this.ObterRegistros9900());

            return registro9001;
        }

        private List<_9900> ObterRegistros9900()
        {
            List<_9900> listaRegistros9900 = new List<_9900>();
            Dictionary<string, int> totalizadores = new Dictionary<string, int>();

            foreach (BlocoPH bloco in this.ArquivoPH.Blocos)
                this.ObterTotalizadoresPorIdentificador(bloco.RegistrosPH, ref totalizadores);

            foreach (var valor in totalizadores)
                listaRegistros9900.Add(new _9900() { Registro = valor.Key, Total = valor.Value });

            listaRegistros9900.Add(new _9900() { Registro = "9001", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9990", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9999", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9900", Total = listaRegistros9900.Count() + 1 });

            listaRegistros9900 = listaRegistros9900.OrderBy(o => o.Registro).ToList();

            return listaRegistros9900;
        }

        private void ObterTotalizadoresPorIdentificador(List<RegistroPH> registros, ref Dictionary<string, int> totalizadores)
        {
            foreach (RegistroPH reg in registros)
            {
                if (totalizadores.ContainsKey(reg.Identificador))
                    totalizadores[reg.Identificador] += 1;
                else
                    totalizadores.Add(reg.Identificador, 1);

                this.ObterTotalizadoresPorIdentificador(reg.Registros, ref totalizadores);
            }
        }

        #endregion

        private void AdicionarUnidadeMedidaGeral(string sigla, string descricao)
        {
            if (!(from obj in this.UnidadesDeMedida where obj.Sigla == sigla select obj).Any())
            {
                this.UnidadesDeMedida.Add(new UnidadeMedidaGeral()
                {
                    Sigla = sigla,
                    Descricao = descricao
                }
                );
            }
        }

        private void AdicionarProduto(Dominio.Entidades.Produto produto)
        {
            if (!(from obj in this.Produtos where obj.Codigo == produto.Codigo select obj).Any())
            {
                this.Produtos.Add(produto);

                this.AdicionarUnidadeMedidaGeral(UnidadeDeMedidaHelper.ObterSigla(produto.UnidadeDeMedida), UnidadeDeMedidaHelper.ObterDescricao(produto.UnidadeDeMedida));
            }
        }

        private void AdicionarCliente(Dominio.Entidades.Cliente cliente)
        {
            if (!(from obj in this.Clientes where obj.CPF_CNPJ_SemFormato.Equals(cliente.CPF_CNPJ_SemFormato) select obj).Any())
                this.Clientes.Add(cliente);
        }

        private void AdicionarCFOP(Dominio.Entidades.CFOP cfop)
        {
            if (!(from obj in this.CFOPs where obj.CodigoCFOP == cfop.CodigoCFOP select obj).Any())
                this.CFOPs.Add(cfop);
        }

        #endregion
    }
}
