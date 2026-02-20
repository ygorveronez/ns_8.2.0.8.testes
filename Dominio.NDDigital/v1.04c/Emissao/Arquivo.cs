using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Dominio.NDDigital.v104.Emissao
{
    public class Arquivo : ArquivoBase
    {
        #region Construtores

        public Arquivo(Stream arquivo)
            : base(arquivo)
        {
            this.CTes = new List<Registro11000>();

            this.LerArquivoEGerarRegistros();
        }

        #endregion

        #region Propriedades

        public Registro00000 R00000 { get; set; }

        public Registro00001 R00001 { get; set; }

        public List<Registro11000> CTes { get; set; }

        #endregion

        #region Métodos

        protected override void LerArquivoEGerarRegistros()
        {
            while (!this.ReaderArquivo.EndOfStream)
            {
                string linha = this.ReaderArquivo.ReadLine();
                Registro registro = this.InstanciarRegistro(linha);

                if (registro.Identificador == "00000")
                {
                    this.R00000 = (Registro00000)registro;
                }
                else if (registro.Identificador == "00001")
                {
                    this.R00001 = (Registro00001)registro;
                }
                else if (registro.Identificador == "10000")
                {
                    registro = this.InstanciarRegistro(this.ReaderArquivo.ReadLine());
                    this.CTes.Add(this.AdicionarCTe((Registro11000)registro));
                }
                else if (registro.Identificador == "11000")
                {
                    this.CTes.Add(this.AdicionarCTe((Registro11000)registro));
                }
            }

            this.ReaderArquivo.Dispose();
        }

        private Registro InstanciarRegistro(string registro)
        {
            switch (registro.Substring(0, 5))
            {
                case "00000":
                    return new Registro00000(registro);
                case "00001":
                    return new Registro00001(registro);
                case "10000":
                    return new Registro10000(registro);
                case "11000":
                    return new Registro11000(registro);
                case "11100":
                    return new Registro11100(registro);
                case "11110":
                    return new Registro11110(registro);
                case "11120":
                    return new Registro11120(registro);
                case "11121":
                    return new Registro11121(registro);
                case "11300":
                    return new Registro11300(registro);
                case "11310":
                    return new Registro11310(registro);
                case "11311":
                    return new Registro11311(registro);
                case "11320":
                    return new Registro11320(registro);
                case "11321":
                    return new Registro11321(registro);
                case "11322":
                    return new Registro11322(registro);
                case "11323":
                    return new Registro11323(registro);
                case "11327":
                    return new Registro11327(registro);
                case "11328":
                    return new Registro11328(registro);
                case "11329":
                    return new Registro11329(registro);
                case "11340":
                    return new Registro11340(registro);
                case "11350":
                    return new Registro11350(registro);
                case "12000":
                    return new Registro12000(registro);
                case "12010":
                    return new Registro12010(registro);
                case "12100":
                    return new Registro12100(registro);
                case "12110":
                    return new Registro12110(registro);
                case "12120":
                    return new Registro12120(registro);
                case "12121":
                    return new Registro12121(registro);
                case "12130":
                    return new Registro12130(registro);
                case "12140":
                    return new Registro12140(registro);
                case "12200":
                    return new Registro12200(registro);
                case "12210":
                    return new Registro12210(registro);
                case "12300":
                    return new Registro12300(registro);
                case "12310":
                    return new Registro12310(registro);
                case "12400":
                    return new Registro12400(registro);
                case "12410":
                    return new Registro12410(registro);
                case "12420":
                    return new Registro12420(registro);
                case "13000":
                    return new Registro13000(registro);
                case "13110":
                    return new Registro13110(registro);
                case "14000":
                    return new Registro14000(registro);
                case "14100":
                    return new Registro14100(registro);
                case "14120":
                    return new Registro14120(registro);
                case "14145":
                    return new Registro14145(registro);
                case "14160":
                    return new Registro14160(registro);
                case "14190":
                    return new Registro14190(registro);
                case "14200":
                    return new Registro14200(registro);
                case "14210":
                    return new Registro14210(registro);
                case "14300":
                    return new Registro14300(registro);
                case "15000":
                    return new Registro15000(registro);
                case "15100":
                    return new Registro15100(registro);
                case "15110":
                    return new Registro15110(registro);
                case "15200":
                    return new Registro15200(registro);
                case "15210":
                    return new Registro15210(registro);
                case "15300":
                    return new Registro15300(registro);
                case "15310":
                    return new Registro15310(registro);
                case "15320":
                    return new Registro15320(registro);
                case "15321":
                    return new Registro15321(registro);
                case "15322":
                    return new Registro15322(registro);
                case "15400":
                    return new Registro15400(registro);
                case "15900":
                    return new Registro15900(registro);
                case "16000":
                    return new Registro16000(registro);
                case "16200":
                    return new Registro16200(registro);
                case "16210":
                    return new Registro16210(registro);
                case "16300":
                    return new Registro15300(registro);
                case "16400":
                    return new Registro16400(registro);
                case "16410":
                    return new Registro16410(registro);
                case "16500":
                    return new Registro16500(registro);
                case "16600":
                    return new Registro16600(registro);
                case "21000":
                    return new Registro21000(registro);
                case "22050":
                    return new Registro22050(registro);
                case "22052":
                    return new Registro22052(registro);
                case "22056":
                    return new Registro22056(registro);
                case "22100":
                    return new Registro22100(registro);
                case "22110":
                    return new Registro22110(registro);
                case "22111":
                    return new Registro22111(registro);
                case "22112":
                    return new Registro22112(registro);
                case "22113":
                    return new Registro22113(registro);
                case "22120":
                    return new Registro22120(registro);
                case "23000":
                    return new Registro23000(registro);
                case "23100":
                    return new Registro23100(registro);
                case "23110":
                    return new Registro23110(registro);
                case "24000":
                    return new Registro24000(registro);
                case "24100":
                    return new Registro24100(registro);
                case "24120":
                    return new Registro24120(registro);
                case "24145":
                    return new Registro24145(registro);
                case "24160":
                    return new Registro24160(registro);
                case "24190":
                    return new Registro24190(registro);
                case "24200":
                    return new Registro24200(registro);
                case "24210":
                    return new Registro24210(registro);
                case "24300":
                    return new Registro24300(registro);
                case "25000":
                    return new Registro25000(registro);
                case "50000":
                    return new Registro50000(registro);
                case "50200":
                    return new Registro50200(registro);
                default:
                    return null;
            }
        }

        private Registro11000 AdicionarCTe(Registro11000 cte)
        {
            while (!this.ReaderArquivo.EndOfStream)
            {
                string linha = this.ReaderArquivo.ReadLine();
                Registro registro = this.InstanciarRegistro(linha);

                if (registro != null)
                {
                    if (registro.Identificador == "10000")
                        return cte;

                    switch (registro.Identificador)
                    {
                        case "11100":
                            cte.ide = (Registro11100)registro;
                            break;
                        case "11110":
                            cte.ide.toma03 = (Registro11110)registro;
                            break;
                        case "11120":
                            cte.ide.toma4 = (Registro11120)registro;
                            break;
                        case "11121":
                            cte.ide.toma4.enderToma = (Registro11121)registro;
                            break;
                        case "11300":
                            cte.compl = (Registro11300)registro;
                            break;
                        case "11310":
                            cte.compl.fluxo = (Registro11310)registro;
                            break;
                        case "11311":
                            cte.compl.pass = (Registro11311)registro;
                            break;
                        case "11320":
                            cte.compl.entrega = (Registro11320)registro;
                            break;
                        case "11321":
                            cte.compl.entrega.semData = (Registro11321)registro;
                            break;
                        case "11322":
                            cte.compl.entrega.comData = (Registro11322)registro;
                            break;
                        case "11323":
                            cte.compl.entrega.noPeriodo = (Registro11323)registro;
                            break;
                        case "11327":
                            cte.compl.entrega.semHora = (Registro11327)registro;
                            break;
                        case "11328":
                            cte.compl.entrega.comHora = (Registro11328)registro;
                            break;
                        case "11329":
                            cte.compl.entrega.noInter = (Registro11329)registro;
                            break;
                        case "11340":
                            cte.compl.obsCont.Add((Registro11340)registro);
                            break;
                        case "11350":
                            cte.compl.obsFisco.Add((Registro11350)registro);
                            break;
                        case "12000":
                            cte.emit = (Registro12000)registro;
                            break;
                        case "12010":
                            cte.emit.enderEmit = (Registro12010)registro;
                            break;
                        case "12100":
                            cte.rem = (Registro12100)registro;
                            break;
                        case "12110":
                            cte.rem.enderReme = (Registro12110)registro;
                            break;
                        case "12120":
                            cte.rem.infNF.Add((Registro12120)registro);
                            break;
                        case "12121":
                            cte.rem.infNF.Last().locRet = (Registro12121)registro;
                            break;
                        case "12130":
                            cte.rem.infNFe.Add((Registro12130)registro);
                            break;
                        case "12140":
                            cte.rem.infOutros.Add((Registro12140)registro);
                            break;
                        case "12200":
                            cte.exped = (Registro12200)registro;
                            break;
                        case "12210":
                            cte.exped.enderExped = (Registro12210)registro;
                            break;
                        case "12300":
                            cte.receb = (Registro12300)registro;
                            break;
                        case "12310":
                            cte.receb.enderReceb = (Registro12310)registro;
                            break;
                        case "12400":
                            cte.dest = (Registro12400)registro;
                            break;
                        case "12410":
                            cte.dest.enderDest = (Registro12410)registro;
                            break;
                        case "12420":
                            cte.dest.locEnt = (Registro12420)registro;
                            break;
                        case "13000":
                            cte.vPrest = (Registro13000)registro;
                            break;
                        case "13110":
                            cte.vPrest.comp.Add((Registro13110)registro);
                            break;
                        case "14000":
                            cte.imp = (Registro14000)registro;
                            break;
                        case "14100":
                            cte.imp.ICMS00 = (Registro14100)registro;
                            break;
                        case "14120":
                            cte.imp.ICMS20 = (Registro14120)registro;
                            break;
                        case "14145":
                            cte.imp.ICMS45 = (Registro14145)registro;
                            break;
                        case "14160":
                            cte.imp.ICMS60 = (Registro14160)registro;
                            break;
                        case "14190":
                            cte.imp.ICMS90 = (Registro14190)registro;
                            break;
                        case "14200":
                            cte.imp.ICMSOutraUF = (Registro14200)registro;
                            break;
                        case "14210":
                            cte.imp.ICMSSN = (Registro14210)registro;
                            break;
                        case "14300":
                            cte.imp.infAdFisco = (Registro14300)registro;
                            break;
                        case "15000":
                            cte.infCTeNorm = (Registro15000)registro;
                            break;
                        case "15100":
                            cte.infCTeNorm.infCarga = (Registro15100)registro;
                            break;
                        case "15110":
                            cte.infCTeNorm.infCarga.infQ.Add((Registro15110)registro);
                            break;
                        case "15200":
                            cte.infCTeNorm.contQt = (Registro15200)registro;
                            break;
                        case "15210":
                            cte.infCTeNorm.contQt.lacContQt = (Registro15210)registro;
                            break;
                        case "15300":
                            cte.infCTeNorm.docAnt.Add((Registro15300)registro);
                            break;
                        case "15310":
                            cte.infCTeNorm.docAnt.Last().emiDocAnt = (Registro15310)registro;
                            break;
                        case "15320":
                            cte.infCTeNorm.docAnt.Last().emiDocAnt.idDocAnt = (Registro15320)registro;
                            break;
                        case "15321":
                            cte.infCTeNorm.docAnt.Last().emiDocAnt.idDocAnt.idDocAntPap = (Registro15321)registro;
                            break;
                        case "15322":
                            cte.infCTeNorm.docAnt.Last().emiDocAnt.idDocAnt.idDocAntEle = (Registro15322)registro;
                            break;
                        case "15400":
                            cte.infCTeNorm.seg.Add((Registro15400)registro);
                            break;
                        case "15900":
                            cte.infCTeNorm.infModal = (Registro15900)registro;
                            break;
                        case "16000":
                            cte.infCTeNorm.infModal.rodo = (Registro16000)registro;
                            break;
                        case "16200":
                            cte.infCTeNorm.infModal.rodo.occ = (Registro16200)registro;
                            break;
                        case "16210":
                            cte.infCTeNorm.infModal.rodo.occ.emiOcc = (Registro16210)registro;
                            break;
                        case "16300":
                            cte.infCTeNorm.infModal.rodo.valePed = (Registro16300)registro;
                            break;
                        case "16400":
                            cte.infCTeNorm.infModal.rodo.veic.Add((Registro16400)registro);
                            break;
                        case "16410":
                            cte.infCTeNorm.infModal.rodo.veic[(cte.infCTeNorm.infModal.rodo.veic.Count() - 1)].prop = (Registro16410)registro;
                            break;
                        case "16500":
                            cte.infCTeNorm.infModal.rodo.lacRodo = (Registro16500)registro;
                            break;
                        case "16600":
                            cte.infCTeNorm.infModal.rodo.moto.Add((Registro16600)registro);
                            break;
                        case "21000":
                            cte.infCTeNorm.peri.Add((Registro21000)registro);
                            break;
                        case "22050":
                            cte.infCTeNorm.cobr = (Registro22050)registro;
                            break;
                        case "22052":
                            cte.infCTeNorm.cobr.fat = (Registro22052)registro;
                            break;
                        case "22056":
                            cte.infCTeNorm.cobr.dup.Add((Registro22056)registro);
                            break;
                        case "22100":
                            cte.infCTeNorm.infCteSub = (Registro22100)registro;
                            break;
                        case "22110":
                            cte.infCTeNorm.infCteSub.tomaICMS = (Registro22110)registro;
                            break;
                        case "22111":
                            cte.infCTeNorm.infCteSub.tomaICMS.refNFe = (Registro22111)registro;
                            break;
                        case "22112":
                            cte.infCTeNorm.infCteSub.tomaICMS.refNF = (Registro22112)registro;
                            break;
                        case "22113":
                            cte.infCTeNorm.infCteSub.tomaICMS.refCTe = (Registro22113)registro;
                            break;
                        case "22120":
                            cte.infCTeNorm.infCteSub.tomaNaoICMS = (Registro22120)registro;
                            break;
                        case "23000":
                            cte.infCteComp = (Registro23000)registro;
                            break;
                        case "23100":
                            cte.infCteComp.vPresComp = (Registro23100)registro;
                            break;
                        case "23110":
                            cte.infCteComp.vPresComp.compComp.Add((Registro23110)registro);
                            break;
                        case "24000":
                            cte.infCteComp.impComp = (Registro24000)registro;
                            break;
                        case "24100":
                            cte.infCteComp.impComp.ICMS00 = (Registro24100)registro;
                            break;
                        case "24120":
                            cte.infCteComp.impComp.ICMS20 = (Registro24120)registro;
                            break;
                        case "24145":
                            cte.infCteComp.impComp.ICMS45 = (Registro24145)registro;
                            break;
                        case "24160":
                            cte.infCteComp.impComp.ICMS60 = (Registro24160)registro;
                            break;
                        case "24190":
                            cte.infCteComp.impComp.ICMS90 = (Registro24190)registro;
                            break;
                        case "24200":
                            cte.infCteComp.impComp.ICMSOutraUF = (Registro24200)registro;
                            break;
                        case "24210":
                            cte.infCteComp.impComp.ICMSSN = (Registro24210)registro;
                            break;
                        case "24300":
                            cte.infCteComp.impComp.infAdFisco = (Registro24300)registro;
                            break;
                        case "25000":
                            cte.infCteAnu = (Registro25000)registro;
                            break;
                        case "50000":
                            cte.infIntegracao = (Registro50000)registro;
                            break;
                        case "50200":
                            cte.infIntegracao.infCarga = (Registro50200)registro;
                            break;
                    }
                }
            }

            return cte;
        }

        #endregion

    }
}
