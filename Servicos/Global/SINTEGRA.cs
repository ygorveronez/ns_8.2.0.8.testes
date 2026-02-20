using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades.SINTEGRA;

namespace Servicos
{
    public class SINTEGRA
    {
        #region Construtores

        public SINTEGRA(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, string codigoEstruturaArquivo, string codigoNaturezaOperacoes, string codigoFinalidadeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            this.ArquivoSINTEGRA = new ArquivoSINTEGRA();

            this.UnitOfWork = unitOfWork;
            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.CodigoEstruturaArquivo = codigoEstruturaArquivo;
            this.CodigoFinalidadeArquivo = codigoFinalidadeArquivo;
            this.CodigoNaturezaOperacoes = codigoNaturezaOperacoes;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(UnitOfWork);
            Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(UnitOfWork);
            Repositorio.ItemDocumentoEntrada repItemDocumentoEntrada = new Repositorio.ItemDocumentoEntrada(UnitOfWork);

            this.CTes = repCTe.BuscarTodosPorStatus(empresa.Codigo, dataInicial, dataFinal, new string[] { "A", "C" }, empresa.TipoAmbiente);

            this.DocumentosCTe = (from obj in this.CTes select obj.Documentos).SelectMany(o => o).ToList();

            this.DocumentosEntrada = repDocumentoEntrada.BuscarPorStatusEModelos(empresa.Codigo, dataInicial, dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado, new string[] { "01", "1A", "04", "06", "21", "22", "55" });

            this.ItensDocumentosEntrada = (from obj in this.DocumentosEntrada select obj.Itens).SelectMany(o => o).ToList();
        }

        #endregion

        #region Propriedades

        private readonly Repositorio.UnitOfWork UnitOfWork;

        private Dominio.Entidades.Empresa Empresa;

        private DateTime DataInicial, DataFinal;

        private string CodigoEstruturaArquivo, CodigoNaturezaOperacoes, CodigoFinalidadeArquivo;

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes;

        private List<Dominio.Entidades.DocumentosCTE> DocumentosCTe;

        private List<Dominio.Entidades.DocumentoEntrada> DocumentosEntrada;

        private List<Dominio.Entidades.ItemDocumentoEntrada> ItensDocumentosEntrada;

        private Dominio.Entidades.SINTEGRA.ArquivoSINTEGRA ArquivoSINTEGRA;

        #endregion

        #region Métodos

        public System.IO.MemoryStream Gerar()
        {
            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco10());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco11());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco50());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco54());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco70());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco71());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco75());

            this.ArquivoSINTEGRA.Blocos.Add(this.ObterBloco90()); //Sempre por ultimo pois é o totalizador dos registros gerados

            return this.ArquivoSINTEGRA.ObterArquivo();
        }

        private Bloco ObterBloco10()
        {
            Bloco bloco10 = new Bloco("10");

            bloco10.Registros.Add(new _10()
            {
                CodigoEstruturaArquivo = this.CodigoEstruturaArquivo,
                CodigoFinalidadeArquivo = this.CodigoFinalidadeArquivo,
                CodigoNaturezaOperacoes = this.CodigoNaturezaOperacoes,
                DataFinal = this.DataFinal,
                DataInicial = this.DataInicial,
                Empresa = this.Empresa,
            });

            return bloco10;
        }

        private Bloco ObterBloco11()
        {
            Bloco bloco11 = new Bloco("11");

            bloco11.Registros.Add(new _11()
            {
                Empresa = this.Empresa
            });

            return bloco11;
        }

        private Bloco ObterBloco50()
        {
            Bloco bloco50 = new Bloco("50");

            foreach (Dominio.Entidades.DocumentoEntrada documento in this.DocumentosEntrada)
            {
                List<Dominio.Entidades.ItemDocumentoEntrada> itens = (from obj in this.ItensDocumentosEntrada where obj.DocumentoEntrada.Codigo == documento.Codigo select obj).ToList();

                List<int> cfops = (from obj in itens select obj.CFOP.CodigoCFOP).Distinct().ToList();
                List<decimal> aliquotas = (from obj in itens select obj.AliquotaICMS).Distinct().ToList();

                foreach (int cfop in cfops)
                {
                    foreach (decimal aliquota in aliquotas)
                    {
                        if ((from obj in itens where obj.AliquotaICMS == aliquota && obj.CFOP.CodigoCFOP == cfop select obj).Any())
                        {
                            decimal baseICMS = (from obj in itens where obj.AliquotaICMS == aliquota && obj.CFOP.CodigoCFOP == cfop select obj.BaseCalculoICMS).Sum();
                            decimal valorTotal = (from obj in itens where obj.AliquotaICMS == aliquota && obj.CFOP.CodigoCFOP == cfop select obj.ValorTotal).Sum();

                            bloco50.Registros.Add(new _50()
                            {
                                Aliquota = aliquota,
                                BaseCalculoICMS = baseICMS,
                                CFOP = cfop,
                                DocumentoEntrada = documento,
                                ValorOutros = valorTotal > baseICMS ? valorTotal - baseICMS : 0m,
                                ValorTotal = valorTotal,
                                ValorTotalICMS = (from obj in itens where obj.AliquotaICMS == aliquota && obj.CFOP.CodigoCFOP == cfop select obj.ValorICMS).Sum()
                            });
                        }
                    }
                }
            }

            return bloco50;
        }

        private Bloco ObterBloco54()
        {
            Bloco bloco54 = new Bloco("54");

            bloco54.Registros.AddRange((from obj in this.ItensDocumentosEntrada
                                        orderby obj.Sequencial ascending
                                        select new _54()
                                        {
                                            Item = obj
                                        }).ToList());

            return bloco54;
        }

        private Bloco ObterBloco70()
        {
            Bloco bloco70 = new Bloco("70");

            bloco70.Registros.AddRange((from obj in this.CTes
                                        orderby obj.Numero ascending
                                        select new _70()
                                        {
                                            CTe = obj
                                        }));

            return bloco70;
        }

        private Bloco ObterBloco71()
        {
            Bloco bloco71 = new Bloco("71");

            bloco71.Registros.AddRange((from obj in this.DocumentosCTe
                                        orderby obj.CTE.Numero ascending
                                        select new _71()
                                        {
                                            Documento = obj
                                        }).ToList());

            return bloco71;
        }

        private Bloco ObterBloco75()
        {
            Bloco bloco75 = new Bloco("75");

            List<Dominio.Entidades.Produto> produtos = (from obj in this.ItensDocumentosEntrada select obj.Produto).Distinct().ToList();

            bloco75.Registros.AddRange((from obj in produtos
                                        select new _75()
                                        {
                                            DataFinal = this.DataFinal,
                                            DataInicial = this.DataInicial,
                                            Produto = obj,
                                            CST = (from item in this.ItensDocumentosEntrada where item.Produto.Codigo == obj.Codigo select item.CST).FirstOrDefault()
                                        }).ToList());

            return bloco75;
        }

        private Bloco ObterBloco90()
        {
            Bloco bloco90 = new Bloco("90");

            _90 registro = new _90();

            registro.Empresa = this.Empresa;
            registro.TotalRegistros90 = 1;

            int totalGeral = 2;

            foreach (Bloco bloco in this.ArquivoSINTEGRA.Blocos)
            {
                if (bloco.Identificador != "10" && bloco.Identificador != "11")
                {
                    int totalRegistros = bloco.ObterTotalDeRegistros();

                    registro.Totalizadores.Add(new KeyValuePair<string, int>(bloco.Identificador, totalRegistros));

                    totalGeral += totalRegistros;
                }
            }

            registro.Totalizadores.Add(new KeyValuePair<string, int>("99", totalGeral + 1));

            bloco90.Registros.Add(registro);

            return bloco90;
        }

        #endregion
    }
}
