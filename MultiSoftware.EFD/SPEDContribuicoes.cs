using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.Entidades.EFD;
using Dominio.Entidades.EFD.SPEDContribuicoes;

namespace MultiSoftware.EFD
{
    public class SPEDContribuicoes
    {
        #region Construtores

        public SPEDContribuicoes(Dominio.Entidades.Empresa empresa,
                                 DateTime dataInicial,
                                 DateTime dataFinal,
                                 string versao,
                                 Dominio.Enumeradores.IndicadorDeSituacaoEspecial situacaoEspecial,
                                 Dominio.Enumeradores.TipoDeAtividadeSPEDContribuicoes tipoAtividade,
                                 Dominio.Enumeradores.TipoEscrituracao tipoEscrituracao,
                                 Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns indicadorApropriacaoCreditos,
                                 Dominio.Enumeradores.IndicadorDoTipoDeContribuicaoApuradaNoPeriodo indicadorTipoContribuicao,
                                 Repositorio.UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.Versao = versao;
            this.SituacaoEspecial = situacaoEspecial;
            this.TipoAtividade = tipoAtividade;
            this.TipoEscrituracao = tipoEscrituracao;
            this.IndicadorApropriacaoCreditos = indicadorApropriacaoCreditos;
            this.IndicadorTipoContribuicao = indicadorTipoContribuicao;

            this.ArquivoSPED = new ArquivoSPED();
            this.Clientes = new List<Dominio.Entidades.Cliente>();
            this.Produtos = new List<Dominio.Entidades.Produto>();
            this.UnidadesDeMedida = new List<Dominio.Entidades.UnidadeMedidaGeral>();
            this.CFOPs = new List<Dominio.Entidades.CFOP>();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);
            this.CTes = repCTe.BuscarTodosPorStatus(empresa.Codigo, dataInicial, dataFinal, new string[] { "A", "C", "D", "I" }, empresa.TipoAmbiente);

            Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(UnitOfWork);
            this.DocumentosEntrada = repDocumentoEntrada.BuscarPorStatusEModelos(empresa.Codigo, dataInicial, dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado, new string[] { "01", "1B", "04", "55", "65", "06", "29", "28", "21", "22" });

            foreach (Dominio.Entidades.Empresa filial in empresa.Filiais)
            {
                this.CTes.AddRange(repCTe.BuscarTodosPorStatus(filial.Codigo, dataInicial, dataFinal, new string[] { "A", "C", "D", "I" }, filial.TipoAmbiente));
                this.DocumentosEntrada = repDocumentoEntrada.BuscarPorStatusEModelos(filial.Codigo, dataInicial, dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado, new string[] { "01", "1B", "04", "55", "65", "06", "29", "28", "21", "22" });
            }

            int[] codigosDocumentosEntrada = (from obj in this.DocumentosEntrada select obj.Codigo).ToArray();

            Repositorio.ItemDocumentoEntrada repItemDocumentoEntrada = new Repositorio.ItemDocumentoEntrada(UnitOfWork);
            this.ItensDocumentosEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigosDocumentosEntrada);
        }

        #endregion

        #region Propriedades

        private readonly Repositorio.UnitOfWork UnitOfWork;

        private Dominio.Entidades.Empresa Empresa;

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes;

        private List<Dominio.Entidades.Cliente> Clientes;

        private List<Dominio.Entidades.DocumentoEntrada> DocumentosEntrada;

        private List<Dominio.Entidades.ItemDocumentoEntrada> ItensDocumentosEntrada;

        private List<Dominio.Entidades.Produto> Produtos;

        private List<Dominio.Entidades.CFOP> CFOPs;

        private List<Dominio.Entidades.UnidadeMedidaGeral> UnidadesDeMedida;

        private DateTime DataInicial, DataFinal;

        private string Versao;

        private ArquivoSPED ArquivoSPED;

        private Dominio.Enumeradores.IndicadorDeSituacaoEspecial SituacaoEspecial;

        private Dominio.Enumeradores.TipoDeAtividadeSPEDContribuicoes TipoAtividade;

        private Dominio.Enumeradores.TipoEscrituracao TipoEscrituracao;

        private Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns IndicadorApropriacaoCreditos;

        private Dominio.Enumeradores.IndicadorDoTipoDeContribuicaoApuradaNoPeriodo IndicadorTipoContribuicao;

        #endregion

        #region Métodos

        public System.IO.MemoryStream GerarSPED()
        {

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoC());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoD());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoF());

            this.ArquivoSPED.Blocos.Add(this.ObterBlocoM());

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
            registro0000.IndicadorSituacaoEspecial = this.SituacaoEspecial;
            registro0000.TipoDeAtividade = this.TipoAtividade;
            registro0000.TipoEscrituracao = this.TipoEscrituracao;
            registro0000.Versao = this.Versao;

            return registro0000;
        }

        private _0001 ObterRegistro0001()
        {
            _0001 registro0001 = new _0001();

            if (this.Empresa.Contador != null)
                registro0001.Registros.Add(this.ObterRegistro0100());

            registro0001.Registros.Add(this.ObterRegistro0110());

            if ((this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo ||
                 this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaNosRegimesNaoCumulativoECumulativo) &&
                 this.IndicadorApropriacaoCreditos == Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns.RateioProporcional)
                registro0001.Registros.Add(this.ObterRegistro0111());

            registro0001.Registros.AddRange(this.ObterRegistros0140());

            registro0001.Registros.AddRange(this.ObterRegistros0150());

            registro0001.Registros.AddRange(this.ObterRegistros0190());

            registro0001.Registros.AddRange(this.ObterRegistros0200());

            registro0001.Registros.AddRange(this.ObterRegistros0400());

            return registro0001;
        }

        private _0100 ObterRegistro0100()
        {
            _0100 registro0100 = new _0100();

            registro0100.Contador = this.Empresa.Contador;
            registro0100.CRC = this.Empresa.CRCContador;

            return registro0100;
        }

        private _0110 ObterRegistro0110()
        {
            _0110 registro0110 = new _0110();

            registro0110.CriterioDeEscrituracaoEApuracao = this.Empresa.Configuracao.CriterioEscrituracaoEApuracao;
            registro0110.IncidenciaTributariaNoPeriodo = this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo;
            registro0110.MetodoDeApropriacaoDeCreditosComuns = this.IndicadorApropriacaoCreditos;
            registro0110.TipoDeContribuicaoApuradaNoPeriodo = this.IndicadorTipoContribuicao;

            return registro0110;
        }

        private _0111 ObterRegistro0111()
        {
            _0111 registro0111 = new _0111();

            if (this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeNaoCumulativo)
                registro0111.ReceitaBrutaNaoCumulativa_TributadaNoMercadoInterno = (from obj in this.CTes select obj.ValorAReceber).Sum() - (from obj in this.DocumentosEntrada select obj.ValorTotal).Sum();

            if (this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo)
                registro0111.ReceitaBrutaCumulativa = (from obj in this.CTes select obj.ValorAReceber).Sum() - (from obj in this.DocumentosEntrada select obj.ValorTotal).Sum();

            registro0111.ReceitaBrutaTotal = registro0111.ReceitaBrutaCumulativa +
                                             registro0111.ReceitaBrutaNaoCumulativa_TributadaNoMercadoInterno +
                                             registro0111.ReceitaBrutaNaoCumulativa_NaoTributadaNoMercadoInterno +
                                             registro0111.ReceitaBrutaNaoCumulativa_Exportacao;

            return registro0111;
        }

        private List<_0140> ObterRegistros0140()
        {
            List<_0140> registros = new List<_0140>();

            registros.Add(new _0140() { Empresa = this.Empresa });

            foreach (Dominio.Entidades.Empresa filial in this.Empresa.Filiais)
            {
                if ((from obj in this.CTes where obj.Empresa.Codigo == filial.Codigo select obj).Any() || (from obj in this.DocumentosEntrada where obj.Empresa.Codigo == filial.Codigo select obj).Any())
                    registros.Add(new _0140() { Empresa = filial });
            }

            return registros;
        }

        private List<_0150> ObterRegistros0150()
        {
            return (from obj in this.Clientes select new _0150() { Cliente = obj }).ToList();
        }

        private List<_0190> ObterRegistros0190()
        {
            return (from obj in this.UnidadesDeMedida select new _0190() { UnidadeMedida = obj }).ToList();
        }

        private List<_0200> ObterRegistros0200()
        {
            return (from obj in this.Produtos select new _0200() { Produto = obj }).ToList();
        }

        private List<_0400> ObterRegistros0400()
        {
            return (from obj in this.CFOPs select new _0400() { CFOP = obj }).ToList();
        }

        #endregion

        #region Bloco C

        private Bloco ObterBlocoC()
        {
            Bloco blocoC = new Bloco("C");

            C001 registro = new C001();

            //Se Regime Cumulativo e Regime de Caixa não gerar bloco C
            if (!(this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                  this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCaixa_EscrituracaoConsolidada) &&
                !(this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCompetencia_EscrituracaoConsolidada))
            {
                registro.Registros.AddRange(this.ObterRegistrosC010());
            }

            blocoC.Registros.Add(registro);

            blocoC.Registros.Add(new C990(blocoC.ObterTotalDeRegistros()));

            return blocoC;
        }

        private List<C010> ObterRegistrosC010()
        {
            List<C010> registros = new List<C010>();

            C010 registro = this.ObterRegistroC010(this.Empresa);

            if (registro != null)
                registros.Add(registro);

            foreach (Dominio.Entidades.Empresa filial in this.Empresa.Filiais)
            {
                registro = this.ObterRegistroC010(filial);

                if (registro != null)
                    registros.Add(registro);
            }

            return registros;
        }

        private C010 ObterRegistroC010(Dominio.Entidades.Empresa empresa)
        {
            if ((from obj in this.DocumentosEntrada where obj.Empresa.Codigo == empresa.Codigo select obj).Any())
            {
                C010 registroC010 = new C010() { Empresa = empresa };

                registroC010.Registros.AddRange(this.ObterRegistrosC100(empresa));

                return registroC010;
            }
            else
            {
                return null;
            }
        }

        private List<C100> ObterRegistrosC100(Dominio.Entidades.Empresa empresa)
        {
            List<C100> registros = new List<C100>();

            string[] modelos = { "01", "1B", "04", "55", "65" };

            IEnumerable<Dominio.Entidades.DocumentoEntrada> documentos = from obj in this.DocumentosEntrada where obj.Empresa.Codigo == empresa.Codigo && modelos.Contains(obj.Modelo.Numero) select obj;

            foreach (Dominio.Entidades.DocumentoEntrada documento in documentos)
            {
                List<Dominio.Entidades.ItemDocumentoEntrada> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                C100 registroC100 = new C100();

                registroC100.DocumentoEntrada = documento;

                foreach (Dominio.Entidades.ItemDocumentoEntrada item in itens)
                {
                    C170 registroC170 = new C170();

                    registroC170.Item = item;

                    registroC100.Registros.Add(registroC170);

                    this.AdicionarProduto(item.Produto);

                    this.AdicionarUnidadeMedidaGeral(item.UnidadeMedida);

                    this.AdicionarCFOP(item.CFOP);
                }

                registros.Add(registroC100);

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
            D001 registroD001 = new D001();

            //Se Regime Cumulativo e Regime de Caixa não gerar bloco D
            if (!(this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                  this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCaixa_EscrituracaoConsolidada) &&
                !(this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                  this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCompetencia_EscrituracaoConsolidada))
            {
                registroD001.Registros.AddRange(this.ObterRegistrosD010());
            }

            return registroD001;
        }

        private List<D010> ObterRegistrosD010()
        {
            List<D010> registros = new List<D010>();

            D010 registro = this.ObterRegistroD010(this.Empresa);

            if (registro != null)
                registros.Add(registro);

            foreach (Dominio.Entidades.Empresa filial in this.Empresa.Filiais)
            {
                registro = this.ObterRegistroD010(filial);

                if (registro != null)
                    registros.Add(registro);
            }

            return registros;
        }

        public D010 ObterRegistroD010(Dominio.Entidades.Empresa empresa)
        {
            if ((from obj in this.DocumentosEntrada where obj.Empresa.Codigo == empresa.Codigo && obj.Modelo.Equals("57") select obj).Any() ||
                (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj).Any())
            {
                D010 registro = new D010() { Empresa = empresa };

                registro.Registros.AddRange(this.ObterRegistrosD100(empresa));

                registro.Registros.AddRange(this.ObterRegistrosD200(empresa));

                return registro;
            }
            else
            {
                return null;
            }
        }

        private List<D100> ObterRegistrosD100(Dominio.Entidades.Empresa empresa)
        {
            List<Dominio.Entidades.DocumentoEntrada> documentos = (from obj in this.DocumentosEntrada where obj.Empresa.Codigo == empresa.Codigo && obj.Modelo.Numero.Equals("57") select obj).ToList();

            List<D100> registros = new List<D100>();

            foreach (Dominio.Entidades.DocumentoEntrada documento in documentos)
            {
                registros.Add(new D100()
                {
                    CTe = documento
                });

                this.AdicionarCliente(documento.Fornecedor);
            }

            return registros;
        }

        private List<D200> ObterRegistrosD200(Dominio.Entidades.Empresa empresa)
        {
            decimal aliquotaPIS = this.Empresa.Configuracao.AliquotaPIS.HasValue ? this.Empresa.Configuracao.AliquotaPIS.Value : 0m;
            decimal aliquotaCOFINS = this.Empresa.Configuracao.AliquotaCOFINS.HasValue ? this.Empresa.Configuracao.AliquotaCOFINS.Value : 0m;

            string cstPIS = this.Empresa.Configuracao.CSTPIS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTPIS.Value) : "";
            string cstCOFINS = this.Empresa.Configuracao.CSTCOFINS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTCOFINS.Value) : "";

            List<D200> registros = (from obj in this.CTes
                                    where obj.Status.Equals("A") && obj.Empresa.Codigo == empresa.Codigo
                                    group obj by new
                                    {
                                        Modelo = obj.ModeloDocumentoFiscal.Numero,
                                        Situacao = obj.SituacaoSPED,
                                        Serie = obj.Serie.Numero,
                                        CFOP = obj.CFOP.CodigoCFOP,
                                        DataReferencia = obj.DataEmissao.Value.Date
                                    } into grupo
                                    select new D200()
                                    {
                                        CFOP = grupo.Key.CFOP,
                                        DataReferencia = grupo.Key.DataReferencia,
                                        Modelo = grupo.Key.Modelo,
                                        NumeroFinal = grupo.Max(o => o.Numero),
                                        NumeroInicial = grupo.Min(o => o.Numero),
                                        Serie = grupo.Key.Serie.ToString(),
                                        Situacao = grupo.Key.Situacao,
                                        ValorTotalDescontos = 0m,
                                        ValorTotalDocumentos = grupo.Sum(o => o.ValorAReceber),
                                        Registros = new List<Registro>()
                                  {
                                      new D201() { 
                                           AliquotaPIS = aliquotaPIS,
                                           CST_PIS = cstPIS,
                                           ValorBaseCalculoPIS =  grupo.Sum(o => o.ValorAReceber),
                                           ValorPIS = grupo.Sum(o => o.ValorAReceber) * (aliquotaPIS / 100),
                                           ValorTotalItens = grupo.Sum(o => o.ValorAReceber)
                                       },
                                       new D205(){
                                            AliquotaCOFINS = aliquotaCOFINS,
                                            CST_COFINS = cstCOFINS,
                                            ValorBaseCalculoCOFINS = grupo.Sum(o => o.ValorAReceber),
                                            ValorCOFINS =  grupo.Sum(o => o.ValorAReceber) * (aliquotaCOFINS / 100),
                                            ValorTotalItens = grupo.Sum(o => o.ValorAReceber)
                                       }
                                  }
                                    }).ToList();

            return registros;
        }

        #endregion

        #region Bloco F

        private Bloco ObterBlocoF()
        {
            Bloco blocoF = new Bloco("F");

            F001 registroF001 = new F001();

            registroF001.Registros.AddRange(this.ObterRegistrosF010());

            blocoF.Registros.Add(registroF001);

            blocoF.Registros.Add(new F990(blocoF.ObterTotalDeRegistros()));

            return blocoF;
        }

        private List<F010> ObterRegistrosF010()
        {
            List<F010> registros = new List<F010>();

            F010 registro = this.ObterRegistroF010(this.Empresa);

            if (registro != null)
                registros.Add(registro);

            foreach (Dominio.Entidades.Empresa filial in this.Empresa.Filiais)
            {
                registro = this.ObterRegistroF010(filial);

                if (registro != null)
                    registros.Add(registro);
            }

            return registros;
        }

        private F010 ObterRegistroF010(Dominio.Entidades.Empresa empresa)
        {
            if ((from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj).Any())
            {
                F010 registro = new F010() { Empresa = empresa };

                if (this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                    this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCaixa_EscrituracaoConsolidada)
                {
                    registro.Registros.AddRange(this.ObterRegistrosF500(empresa));
                    registro.Registros.AddRange(this.ObterRegistrosF525(empresa));
                }
                else if (this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
                         this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCompetencia_EscrituracaoConsolidada)
                {
                    registro.Registros.AddRange(this.ObterRegistrosF550(empresa));
                }
                else
                {
                    return null;
                }

                return registro;
            }
            else
            {
                return null;
            }
        }

        private List<F500> ObterRegistrosF500(Dominio.Entidades.Empresa empresa)
        {
            List<F500> registros = new List<F500>();

            List<int> cfops = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj.CFOP.CodigoCFOP).Distinct().ToList();

            decimal aliquotaCOFINS = this.Empresa.Configuracao.AliquotaCOFINS.HasValue ? this.Empresa.Configuracao.AliquotaCOFINS.Value : 0m;
            decimal aliquotaPIS = this.Empresa.Configuracao.AliquotaPIS.HasValue ? this.Empresa.Configuracao.AliquotaPIS.Value : 0m;
            string cstCOFINS = this.Empresa.Configuracao.CSTCOFINS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTCOFINS.Value) : "";
            string cstPIS = this.Empresa.Configuracao.CSTPIS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTPIS.Value) : "";

            foreach (int cfop in cfops)
            {
                decimal baseCalculo = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") && obj.CFOP.CodigoCFOP == cfop select obj.ValorAReceber).Sum();

                registros.Add(new F500()
                {
                    AliquotaCOFINS = aliquotaCOFINS,
                    AliquotaPIS = aliquotaPIS,
                    CFOP = cfop,
                    CST_COFINS = cstCOFINS,
                    CST_PIS = cstPIS,
                    Modelo = "57",
                    ValorBaseCalculoCOFINS = baseCalculo,
                    ValorBaseCalculoPIS = baseCalculo,
                    ValorCOFINS = baseCalculo * aliquotaCOFINS / 100,
                    ValorPIS = baseCalculo * aliquotaPIS / 100,
                    ValorTotalReceitaRecebida = baseCalculo
                });
            }

            return registros;
        }

        private List<F525> ObterRegistrosF525(Dominio.Entidades.Empresa empresa)
        {
            List<F525> registros = (from obj in this.CTes
                                    where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A")
                                    select new F525()
                                        {
                                            ValorTotalReceitaRecebida = obj.ValorAReceber,
                                            CST_COFINS = this.Empresa.Configuracao.CSTCOFINS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTCOFINS.Value) : "",
                                            CST_PIS = this.Empresa.Configuracao.CSTPIS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTPIS.Value) : "",
                                            ValorReceitaDetalhada = obj.ValorAReceber,
                                            NumeroDocumento = obj.Numero.ToString(),
                                            IndicadorComposicao = "03"
                                        }).ToList();

            return registros;
        }

        private List<F550> ObterRegistrosF550(Dominio.Entidades.Empresa empresa)
        {
            List<F550> registros = new List<F550>();

            List<int> cfops = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj.CFOP.CodigoCFOP).Distinct().ToList();

            decimal aliquotaCOFINS = this.Empresa.Configuracao.AliquotaCOFINS.HasValue ? this.Empresa.Configuracao.AliquotaCOFINS.Value : 0m;
            decimal aliquotaPIS = this.Empresa.Configuracao.AliquotaPIS.HasValue ? this.Empresa.Configuracao.AliquotaPIS.Value : 0m;
            string cstCOFINS = this.Empresa.Configuracao.CSTCOFINS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTCOFINS.Value) : "";
            string cstPIS = this.Empresa.Configuracao.CSTPIS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTPIS.Value) : "";

            foreach (int cfop in cfops)
            {
                decimal baseCalculo = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") && obj.CFOP.CodigoCFOP == cfop select obj.ValorAReceber).Sum();

                registros.Add(new F550()
                {
                    AliquotaCOFINS = aliquotaCOFINS,
                    AliquotaPIS = aliquotaPIS,
                    CFOP = cfop,
                    CST_COFINS = cstCOFINS,
                    CST_PIS = cstPIS,
                    Modelo = "57",
                    ValorBaseCalculoCOFINS = baseCalculo,
                    ValorBaseCalculoPIS = baseCalculo,
                    ValorCOFINS = baseCalculo * aliquotaCOFINS / 100,
                    ValorPIS = baseCalculo * aliquotaPIS / 100,
                    ValorTotalDaReceitaAuferida = baseCalculo
                });
            }

            return registros;
        }

        #endregion

        #region Bloco M

        private Bloco ObterBlocoM()
        {
            Bloco blocoM = new Bloco("M");

            blocoM.Registros.Add(new M001());
            blocoM.Registros.Add(new M990(blocoM.ObterTotalDeRegistros()));

            return blocoM;
        }

        #endregion

        #region Bloco 1

        private Bloco ObterBloco1()
        {
            Bloco bloco1 = new Bloco("z1");

            bloco1.Registros.Add(this.ObterRegistro1001());

            bloco1.Registros.Add(new _1990(bloco1.ObterTotalDeRegistros()));

            return bloco1;
        }

        private _1001 ObterRegistro1001()
        {
            _1001 registro = new _1001();

            if (this.Empresa.Configuracao.IncidenciaTributariaNoPeriodo == Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo &&
               (this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCaixa_EscrituracaoConsolidada ||
                this.Empresa.Configuracao.CriterioEscrituracaoEApuracao == Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado.RegimeDeCompetencia_EscrituracaoConsolidada))
                registro.Registros.AddRange(this.ObterRegistros1900());

            return registro;
        }

        private List<_1900> ObterRegistros1900()
        {
            List<_1900> registros = new List<_1900>();

            _1900 registro = this.ObterRegistro1900(this.Empresa);

            if (registro != null)
                registros.Add(registro);

            foreach (Dominio.Entidades.Empresa filial in this.Empresa.Filiais)
            {
                registro = this.ObterRegistro1900(filial);

                if (registro != null)
                    registros.Add(registro);
            }

            return registros;
        }

        private _1900 ObterRegistro1900(Dominio.Entidades.Empresa empresa)
        {
            if ((from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj).Any())
            {
                _1900 registro = new _1900()
                {
                    CNPJ = empresa.CNPJ,
                    Modelo = "57",
                    ValorTotalReceita = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj.ValorAReceber).Sum(),
                    Situacao = "00",
                    CST_PIS = this.Empresa.Configuracao.CSTPIS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTPIS.Value) : "",
                    CST_COFINS = this.Empresa.Configuracao.CSTCOFINS.HasValue ? string.Format("{0:00}", (int)this.Empresa.Configuracao.CSTCOFINS.Value) : "",
                    QuantidadeTotalDocumentos = (from obj in this.CTes where obj.Empresa.Codigo == empresa.Codigo && obj.Status.Equals("A") select obj).Count()
                };

                return registro;
            }
            else
            {
                return null;
            }
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
