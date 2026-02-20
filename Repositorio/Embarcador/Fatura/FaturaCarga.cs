using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaCarga : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>
    {
        public FaturaCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemCargaoFatura(int codigoFatura, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.Carga.Codigo == codigoCarga select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCarga BuscarPorCargaFatura(int codigoCarga, int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Fatura.Codigo == codigoFatura select obj;
            return result.FirstOrDefault();
        }

        public int QuantidadeDocumentosCarga(int codigoFatura, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int codigoCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura == fatura select obj;
            if (codigoCarga > 0)
                resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public int QuantidadeDocumentosFaturadosCarga(int codigoFatura, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int codigoCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura == fatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            if (codigoCarga > 0)
                resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public int QuantidadeDocumentosParcialmenteFaturadosCarga(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido select obj;

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public decimal ValorConhecimentos(int codigoFatura, int codigoCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            if (codigoCarga > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Sum(obj => obj.ConhecimentoDeTransporteEletronico.ValorAReceber);
            else
                return 0;
        }

        public decimal ValorICMSConhecimentos(int codigoFatura, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int codigoCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            if (codigoCarga > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Sum(obj => obj.ConhecimentoDeTransporteEletronico.ValorICMS);
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> BuscarPorNumeroCTe(int numeroCTe, int codigoFatura, int numeroCTeFinal, string numeroPedido, string numeroOcorrencia, decimal aliquotaICMS, Dominio.Enumeradores.TipoCTE tipoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resut = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;

            if (numeroCTe > 0 && numeroCTeFinal == 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe);
            else if (numeroCTe > 0 && numeroCTeFinal > 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero >= numeroCTe && obj.ConhecimentoDeTransporteEletronico.Numero <= numeroCTeFinal);
            else if (numeroCTe == 0 && numeroCTeFinal > 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTeFinal);

            if ((int)tipoCTe >= 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.TipoCTE == tipoCTe);

            if (aliquotaICMS > 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.AliquotaICMS.Equals(aliquotaICMS));

            long vNumeroPedido = 0;
            long.TryParse(numeroPedido, out vNumeroPedido);
            if (vNumeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(vNumeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(vNumeroPedido) select obj;
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resut = resut.Where(obj =>
                    resultAvon.Select(a => a.Carga).Contains(obj.Carga) ||
                    resultNatura.Select(a => a.Carga).Contains(obj.Carga) ||
                    resultCargaPedido.Select(a => a.Carga).Contains(obj.Carga));
            }
            else if (!string.IsNullOrWhiteSpace(numeroPedido))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resut = resut.Where(obj => resultCargaPedido.Select(a => a.Carga).Contains(obj.Carga));
            }

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                resut = resut.Where(o => o.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(numeroOcorrencia)));

            return resut.Distinct().ToList();
        }

        public int ContarPorNumeroCTe(int numeroCTe, int codigoFatura, int numeroCTeFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resut = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;

            if (numeroCTe > 0 && numeroCTeFinal == 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe);
            else if (numeroCTe > 0 && numeroCTeFinal > 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero >= numeroCTe && obj.ConhecimentoDeTransporteEletronico.Numero <= numeroCTeFinal);
            else if (numeroCTe == 0 && numeroCTeFinal > 0)
                resut = resut.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTeFinal);

            return resut.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(string codigoCargaEmbarcador, string numeroOcorrencia, int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var resultFatura = from obj in queryFatura where obj.Fatura.Codigo == codigoFatura select obj;
            //query = resultFatura.Join(query, vei => vei.Carga.Codigo, emp => emp.Codigo, (vei, emp) => emp);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                resultFatura = resultFatura.Where(obj => obj.Carga.SituacaoCarga == situacao);

            if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador) && codigoCargaEmbarcador != "0")
                resultFatura = resultFatura.Where(obj => obj.Carga.CodigoCargaEmbarcador.Contains(codigoCargaEmbarcador));

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                resultFatura = resultFatura.Where(o => o.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(numeroOcorrencia)));

            return resultFatura.Select(o => o.Carga).Fetch(o => o.DadosSumarizados).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string codigoCargaEmbarcador, string numeroOcorrencia, int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacao)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var resultFatura = from obj in queryFatura where obj.Fatura.Codigo == codigoFatura select obj;
            //query = resultFatura.Join(query, vei => vei.Carga.Codigo, emp => emp.Codigo, (vei, emp) => emp);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                resultFatura = resultFatura.Where(obj => obj.Carga.SituacaoCarga == situacao);

            if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador) && codigoCargaEmbarcador != "0")
                resultFatura = resultFatura.Where(obj => obj.Carga.CodigoCargaEmbarcador.Contains(codigoCargaEmbarcador));

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                resultFatura = resultFatura.Where(o => o.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(numeroOcorrencia)));

            return resultFatura.Distinct().Count();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga SituacaoCarga(int codigoFatura, int codigoCarga)
        {
            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var resultFatura = from obj in queryFatura where obj.Fatura.Codigo == codigoFatura && obj.Carga.Codigo == codigoCarga select obj.StatusFaturaCarga;
            if (resultFatura.Count() > 0)
                return resultFatura.FirstOrDefault();
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada;
        }

        public int QuantidadeDocumentosCarga(int codigoFatura, int codigoCarga, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            if (codigoCarga > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public int QuantidadeTotalDocumentosCarga(int codigoFatura, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;
            if (codigoCarga > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public int QuantidadeDocumentosParcialmenteFaturadosCarga(int codigoFatura, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido select obj;
            if (codigoCarga > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> ConsultarConhecimento(int numeroCTe, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && obj.Fatura.Codigo == fatura.Codigo && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento select obj;
            if (numeroCTe > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe);

            resultConhecimento = resultConhecimento.Fetch(o => o.ConhecimentoDeTransporteEletronico).ThenFetch(o => o.Serie);

            if (propOrdenacao != null && dirOrdenacao != null)
                return resultConhecimento.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
            else
                return resultConhecimento.Distinct().ToList();
        }

        public int ContarConsultarConhecimento(int numeroCTe, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultConhecimento = from obj in query where obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && obj.Fatura.Codigo == fatura.Codigo && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento select obj;
            if (numeroCTe > 0)
                resultConhecimento = resultConhecimento.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe);

            if (resultConhecimento.Count() > 0)
                return resultConhecimento.Distinct().Count();
            else
                return 0;
        }

        public List<int> BuscarNumeroFaturaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.StatusFaturaCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTeSemFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int numeroCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where !obj.Carga.CargaTransbordo && obj.CTe.Fatura == null && obj.CTe.Numero == numeroCTe && !obj.CTe.FaturasTMS.Any(o => o.Fatura.Codigo == fatura.Codigo) select obj;

            if (fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && fatura.Cliente != null)
            {

                result = result.Where(obj => obj.CTe.DataEmissao >= fatura.DataInicial.Value.Date && obj.CTe.DataEmissao < fatura.DataFinal.Value.AddDays(1).Date
                                             && obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == fatura.Cliente.CPF_CNPJ
                                             && obj.CTe.Status == "A");
            }
            else if (fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && fatura.GrupoPessoas != null && fatura.GrupoPessoas.Clientes != null)
            {
                result = result.Where(obj => obj.CTe.DataEmissao >= fatura.DataInicial.Value.Date && obj.CTe.DataEmissao < fatura.DataFinal.Value.AddDays(1).Date
                                             && obj.CTe.TomadorPagador.Cliente.GrupoPessoas.Codigo == fatura.GrupoPessoas.Codigo
                                             && obj.CTe.Status == "A");
            }
            else
                result = result.Where(obj => obj.Codigo < 0);

            return result.ToList();
        }
    }
}
