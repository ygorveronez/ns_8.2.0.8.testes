using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.Entidades;
using Dominio.Entidades.EFD;
using Dominio.Entidades.EFD.SPED;

namespace MultiSoftware.EFD
{
    public class SPED
    {
        #region Construtores

        public SPED(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, string versaoAtoCOTEPE, List<string> registrosNaoGerar, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.StringConexao = stringConexao;
            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.VersaoAtoCOTEPE = int.Parse(versaoAtoCOTEPE);
            this.Clientes = new List<Dominio.Entidades.Cliente>();
            this.ArquivoSPED = new ArquivoSPED();
            this.RegistrosNaoGerar = registrosNaoGerar;

            this.Produtos = new List<Produto>();
            this.UnidadesDeMedida = new List<UnidadeMedidaGeral>();
            this.CFOPs = new List<CFOP>();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            this.CTes = repCTe.BuscarTodosPorStatus(empresa.Codigo, dataInicial, dataFinal, new string[] { "A", "C", "D", "I" }, empresa.TipoAmbiente);

            int[] codigoCTes = (from obj in this.CTes select obj.Codigo).ToArray();

            Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unitOfWork);
            this.DocumentosEntrada = repDocumentoEntrada.BuscarPorStatusEModelos(empresa.Codigo, dataInicial, dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado, new string[] { "01", "1B", "04", "55", "65", "06", "29", "28", "21", "22" });

            int[] codigosDocumentosEntrada = (from obj in this.DocumentosEntrada select obj.Codigo).ToArray();

            Repositorio.ItemDocumentoEntrada repItemDocumentoEntrada = new Repositorio.ItemDocumentoEntrada(unitOfWork);
            this.ItensDocumentosEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigosDocumentosEntrada);

            Repositorio.ApuracaoICMS repApuracaoICMS = new Repositorio.ApuracaoICMS(unitOfWork);
            this.ApuracaoICMS = repApuracaoICMS.BuscarPorPeriodo(empresa.Codigo, dataInicial, dataFinal);
        }

        #endregion

        #region Propriedades

        private Repositorio.UnitOfWork UnitOfWork;

        private string StringConexao;

        private Dominio.Entidades.Empresa Empresa;

        private Dominio.Entidades.ApuracaoICMS ApuracaoICMS;

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes;

        private List<Dominio.Entidades.Cliente> Clientes;

        private List<Dominio.Entidades.DocumentoEntrada> DocumentosEntrada;

        private List<ItemDocumentoEntrada> ItensDocumentosEntrada;

        private List<Dominio.Entidades.Produto> Produtos;

        private List<C190> ListaC190 = new List<C190>();

        private List<C590> ListaC590 = new List<C590>();

        private List<Dominio.Entidades.CFOP> CFOPs { get; set; }

        private List<Dominio.Entidades.UnidadeMedidaGeral> UnidadesDeMedida;

        private DateTime DataInicial, DataFinal;

        private int VersaoAtoCOTEPE;

        private ArquivoSPED ArquivoSPED;

        private List<string> RegistrosNaoGerar;

        #endregion

        #region Métodos

        public System.IO.MemoryStream GerarSPED()
        {

            if (VersaoAtoCOTEPE >= 13)
                this.ArquivoSPED.Blocos.Add(this.ObterBlocoB());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoC());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoD());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoE());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoG());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoH());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoK());

            this.ArquivoSPED.Blocos.Add(this.ObterBloco1());

            this.ArquivoSPED.Blocos.Add(this.ObterBloco0()); //Sempre deixar para o fim o bloco 0, pois contém os dados utilizados nos outros blocos (clientes, observações, etc...)

            this.ArquivoSPED.Blocos.Add(this.ObterBloco9());

            return this.ArquivoSPED.ObterArquivo();
        }

        #region Bloco 0

        private Bloco ObterBloco0()
        {
            Bloco bloco0 = new Bloco("0");

            bloco0.Registros.Add(this.ObterRegistro0000());
            bloco0.Registros.Add(this.ObterRegistro0001());
            bloco0.Registros.Add(new _0990(bloco0.ObterTotalDeRegistros()));

            return bloco0;
        }

        private _0000 ObterRegistro0000()
        {
            _0000 registro0000 = new _0000();

            registro0000.DataFinal = this.DataFinal;
            registro0000.DataInicial = this.DataInicial;
            registro0000.Empresa = this.Empresa;
            registro0000.FinalidadeArquivo = Dominio.Enumeradores.FinalidadeDoArquivoSPED.RemessaOriginal;
            registro0000.PerfilDeApresentacao = this.Empresa.Configuracao.Perfil.ToString("G");
            registro0000.TipoDeAtividade = Dominio.Enumeradores.TipoDeAtividade.Outros;
            registro0000.VersaoAtoCOTEPE = this.VersaoAtoCOTEPE;

            return registro0000;
        }

        private _0001 ObterRegistro0001()
        {
            _0001 registro0001 = new _0001();

            registro0001.Registros.Add(this.ObterRegistro0005());

            if (this.Empresa.Contador != null)
                registro0001.Registros.Add(this.ObterRegistro0100());

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

        private Bloco ObterBlocoB()
        {
            Bloco blocoB = new Bloco("B");

            blocoB.Registros.Add(new B001());
            blocoB.Registros.Add(new B990(blocoB.ObterTotalDeRegistros()));

            return blocoB;
        }

        #endregion

        #region Bloco C

        private Bloco ObterBlocoC()
        {
            Bloco blocoC = new Bloco("C");

            C001 registro = new C001();

            registro.Registros.AddRange(this.ObterRegistrosC100());

            registro.Registros.AddRange(this.ObterRegistrosC500());

            blocoC.Registros.Add(registro);

            blocoC.Registros.Add(new C990(blocoC.ObterTotalDeRegistros()));

            return blocoC;
        }

        private List<C100> ObterRegistrosC100()
        {
            List<C100> registros = new List<C100>();

            string[] modelos = { "01", "1B", "04", "55", "65" };

            IEnumerable<Dominio.Entidades.DocumentoEntrada> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.DocumentoEntrada documento in documentos)
            {
                List<Dominio.Entidades.ItemDocumentoEntrada> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                C100 registroC100 = new C100();

                registroC100.DocumentoEntrada = documento;

                foreach (Dominio.Entidades.ItemDocumentoEntrada item in itens)
                {
                    C170 registroC170 = new C170();

                    registroC170.Item = item;
                    registroC170.versaoAtoCOTEPE = VersaoAtoCOTEPE;

                    registroC100.Registros.Add(registroC170);

                    this.AdicionarProduto(item.Produto);

                    this.AdicionarUnidadeMedidaGeral(item.UnidadeMedida);

                    this.AdicionarCFOP(item.CFOP);
                }
                var rangeC190 = from obj in itens
                                group new
                                {
                                    obj.AliquotaICMS,
                                    obj.CFOP.CodigoCFOP,
                                    obj.CST,
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
                                    obj.CST,
                                    obj.CFOP.CodigoCFOP
                                } into grupo
                                select new C190()
                                {
                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                    CFOP = grupo.Key.CodigoCFOP,
                                    CSTICMS = grupo.Key.CST,
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

            string[] modelos = { "06", "29", "28" };

            IEnumerable<Dominio.Entidades.DocumentoEntrada> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.DocumentoEntrada documento in documentos)
            {
                List<Dominio.Entidades.ItemDocumentoEntrada> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                C500 registroC500 = new C500();

                registroC500.DocumentoEntrada = documento;
                registroC500.versaoAtoCOTEPE = VersaoAtoCOTEPE;

                var rangeC590 = from obj in itens
                                group new
                                {
                                    obj.AliquotaICMS,
                                    obj.CFOP.CodigoCFOP,
                                    obj.CST,
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
                                    obj.CST,
                                    obj.CFOP.CodigoCFOP
                                } into grupo
                                select new C590()
                                {
                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                    CFOP = grupo.Key.CodigoCFOP,
                                    CSTICMS = grupo.Key.CST,
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

        private Bloco ObterBlocoD()
        {
            Bloco blocoD = new Bloco("D");

            blocoD.Registros.Add(this.ObterRegistroD001());

            blocoD.Registros.Add(new D990(blocoD.ObterTotalDeRegistros()));

            return blocoD;
        }

        private D001 ObterRegistroD001()
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(UnitOfWork);
            Repositorio.MovimentoDoFinanceiro repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(UnitOfWork);

            D001 registroD001 = new D001();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
            {
                registroD001.Registros.Add(this.ObterRegistroD100(cte));

                if (!cte.Status.Equals("D") && !cte.Status.Equals("I") && !cte.Status.Equals("C") && cte.Tomador != null)
                    if (!(from obj in this.Clientes where obj.CPF_CNPJ_SemFormato.Equals(cte.Tomador.CPF_CNPJ) select obj).Any())
                        this.Clientes.Add(repCliente.BuscarPorCPFCNPJ(double.Parse(!string.IsNullOrWhiteSpace(cte.Tomador.CPF_CNPJ) ? cte.Tomador.CPF_CNPJ : "99999999999999" )));
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

                registroD100.Registros.Add(this.ObterRegistroD190(cte));
            }

            return registroD100;
        }

        private D160 ObterRegistroD160(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(UnitOfWork);

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

            string[] modelos = { "21", "22" };

            IEnumerable<Dominio.Entidades.DocumentoEntrada> documentos = from obj in this.DocumentosEntrada where modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.DocumentoEntrada documento in documentos)
            {
                List<Dominio.Entidades.ItemDocumentoEntrada> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                D500 registroD500 = new D500();

                registroD500.Documento = documento;

                registroD500.Registros.AddRange(from obj in itens
                                                group new
                                                {
                                                    obj.AliquotaICMS,
                                                    obj.CFOP.CodigoCFOP,
                                                    obj.CST,
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
                                                    obj.CST,
                                                    obj.CFOP.CodigoCFOP
                                                }
                                                    into grupo
                                                select new D590()
                                                {
                                                    AliquotaICMS = grupo.Key.AliquotaICMS,
                                                    BaseCalculoICMS = grupo.Sum(o => o.BaseCalculoICMS),
                                                    BaseCalculoICMSST = grupo.Sum(o => o.BaseCalculoICMSST),
                                                    CFOP = grupo.Key.CodigoCFOP,
                                                    CSTICMS = grupo.Key.CST,
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

        private Bloco ObterBlocoE()
        {
            Bloco blocoE = new Bloco("E");

            E001 registroE001 = new E001();

            if (this.ApuracaoICMS != null)
            {
                registroE001.Registros.Add(this.ObterRegistroE100());
            }
            registroE001.Registros.Add(this.ObterRegistroE200());

            blocoE.Registros.Add(registroE001);

            blocoE.Registros.Add(new E990(blocoE.ObterTotalDeRegistros()));

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

        private Bloco ObterBlocoG()
        {
            Bloco blocoG = new Bloco("G");

            blocoG.Registros.Add(new G001());
            blocoG.Registros.Add(new G990(blocoG.ObterTotalDeRegistros()));

            return blocoG;
        }

        #endregion

        #region Bloco H

        private Bloco ObterBlocoH()
        {
            Bloco blocoH = new Bloco("H");

            blocoH.Registros.Add(new H001());
            blocoH.Registros.Add(new H990(blocoH.ObterTotalDeRegistros()));

            return blocoH;
        }

        #endregion

        #region Bloco K

        private Bloco ObterBlocoK()
        {
            Bloco blocoK = new Bloco("K");

            blocoK.Registros.Add(new K001());
            blocoK.Registros.Add(new K990(blocoK.ObterTotalDeRegistros()));

            return blocoK;
        }

        #endregion

        #region Bloco 1

        private Bloco ObterBloco1()
        {
            Bloco bloco1 = new Bloco("z1");

            _1001 registro1001 = new _1001();

            registro1001.Registros.Add(this.ObterRegistro1010());

            bloco1.Registros.Add(registro1001);

            bloco1.Registros.Add(new _1990(bloco1.ObterTotalDeRegistros()));

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

        private Bloco ObterBloco9()
        {
            Bloco bloco9 = new Bloco("z9");

            bloco9.Registros.Add(this.ObterRegistro9001());
            bloco9.Registros.Add(new _9990(bloco9.ObterTotalDeRegistros()));

            int totalRegistrosArquivo = (from obj in this.ArquivoSPED.Blocos select obj.ObterTotalDeRegistros()).Sum() + bloco9.ObterTotalDeRegistros();

            bloco9.Registros.Add(new _9999(totalRegistrosArquivo));

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

            foreach (Bloco bloco in this.ArquivoSPED.Blocos)
                this.ObterTotalizadoresPorIdentificador(bloco.Registros, ref totalizadores);

            foreach (var valor in totalizadores)
                listaRegistros9900.Add(new _9900() { Registro = valor.Key, Total = valor.Value });

            listaRegistros9900.Add(new _9900() { Registro = "9001", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9990", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9999", Total = 1 });
            listaRegistros9900.Add(new _9900() { Registro = "9900", Total = listaRegistros9900.Count() + 1 });

            listaRegistros9900 = listaRegistros9900.OrderBy(o => o.Registro).ToList();

            return listaRegistros9900;
        }

        private void ObterTotalizadoresPorIdentificador(List<Registro> registros, ref Dictionary<string, int> totalizadores)
        {
            foreach (Registro reg in registros)
            {
                if (totalizadores.ContainsKey(reg.Identificador))
                    totalizadores[reg.Identificador] += 1;
                else
                    totalizadores.Add(reg.Identificador, 1);

                this.ObterTotalizadoresPorIdentificador(reg.Registros, ref totalizadores);
            }
        }

        #endregion

        private void AdicionarUnidadeMedidaGeral(Dominio.Entidades.UnidadeMedidaGeral unidadeMedida)
        {
            if (!(from obj in this.UnidadesDeMedida where obj.Codigo == unidadeMedida.Codigo select obj).Any())
                this.UnidadesDeMedida.Add(unidadeMedida);
        }

        private void AdicionarProduto(Dominio.Entidades.Produto produto)
        {
            if (!(from obj in this.Produtos where obj.Codigo == produto.Codigo select obj).Any())
            {
                this.Produtos.Add(produto);

                this.AdicionarUnidadeMedidaGeral(produto.UnidadeMedida);
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
