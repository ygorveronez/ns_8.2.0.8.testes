using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Repositorio
{
    public class Duplicata : RepositorioBase<Dominio.Entidades.Duplicata>, Dominio.Interfaces.Repositorios.Duplicata
    {
        public Duplicata(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Duplicata BuscaPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Duplicata BuscaPorDocumento(int codigoEmpresa, string documento, Dominio.Enumeradores.TipoDuplicata tipoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Documento == documento && obj.Empresa.Codigo == codigoEmpresa && obj.Status != "I" && obj.Tipo == tipoDuplicata select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Duplicata BuscaPorNumero(int codigoEmpresa, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Numero == numero && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Duplicata BuscaPorDocumentoEntrada(int codigoEmpresa, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return result.FirstOrDefault();
        }

        public int BuscarUltimoCodigo(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa select (int?)obj.Codigo).Max();

            return result.HasValue ? result.Value : 0;
        }

        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa select (int?)obj.Numero).Max();

            return result.HasValue ? result.Value : 0;
        }


        public int ContarConsulta(int codigoEmpresa, DateTime dataLancamento, DateTime dataDocumento, int codigo, string nomePessoa, Dominio.Enumeradores.TipoDuplicata? tipoDuplicata, string documento, int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (cte > 0)
            {
                var queryDuplicataCTe = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();
                var resultDuplicataCTe = from obj in queryDuplicataCTe where obj.ConhecimentoDeTransporteEletronico.Numero == cte select obj.Duplicata;

                result = result.Where(o => resultDuplicataCTe.Contains(o));
            }

            if (!string.IsNullOrWhiteSpace(nomePessoa))
                result = result.Where(o => o.Pessoa.Nome.Contains(nomePessoa));

            if (dataLancamento != DateTime.MinValue)
                result = result.Where(o => o.DataLancamento == dataLancamento.Date);

            if (dataDocumento != DateTime.MinValue)
                result = result.Where(o => o.DataDocumento == dataDocumento);

            if (tipoDuplicata != null)
                result = result.Where(o => o.Tipo == tipoDuplicata.Value);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.Count();
        }

        public List<Dominio.Entidades.Duplicata> Consultar(int codigoEmpresa, DateTime dataLancamento, DateTime dataDocumento, int numero, string nomePessoa, Dominio.Enumeradores.TipoDuplicata? tipoDuplicata, string documento, int cte, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Duplicata>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (cte > 0)
            {
                var queryDuplicataCTe = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();
                var resultDuplicataCTe = from obj in queryDuplicataCTe where obj.ConhecimentoDeTransporteEletronico.Numero == cte select obj.Duplicata;

                result = result.Where(o => resultDuplicataCTe.Contains(o));
            }                

            if (!string.IsNullOrWhiteSpace(nomePessoa))
                result = result.Where(o => o.Pessoa.Nome.Contains(nomePessoa));

            if (dataLancamento != DateTime.MinValue)
                result = result.Where(o => o.DataLancamento == dataLancamento.Date);

            if (dataDocumento != DateTime.MinValue)
                result = result.Where(o => o.DataDocumento == dataDocumento);

            if (tipoDuplicata != null)
                result = result.Where(o => o.Tipo == tipoDuplicata.Value);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioDuplicatas> Relatorio(int codigoEmpresa, int codigoVeiculo1, int codigoVeiculo2, int codigoVeiculo3, string cpfCnpjPessoa, int codigoMotorista, DateTime dataLctoInicial, DateTime dataLctoFinal, DateTime dataVctoInicial, DateTime dataVctoFinal, Dominio.Enumeradores.TipoDuplicata? tipo, Dominio.Enumeradores.StatusDuplicata? statusPgto, int ordenacao, bool raizCNPJ, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Duplicata.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoVeiculo1 > 0)
                result = result.Where(o => o.Duplicata.Veiculo1.Codigo == codigoVeiculo1);

            if (codigoVeiculo2 > 0)
                result = result.Where(o => o.Duplicata.Veiculo2.Codigo == codigoVeiculo2);

            if (codigoVeiculo3 > 0)
                result = result.Where(o => o.Duplicata.Veiculo3.Codigo == codigoVeiculo3);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Duplicata.Motorista.Codigo == codigoMotorista);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Duplicata.Status == status);

            if (!string.IsNullOrWhiteSpace(cpfCnpjPessoa))
            {
                if (raizCNPJ)
                {
                    string cnpjPessoaInicio = cpfCnpjPessoa.Substring(0, 8) + "000000";
                    string cnpjPessoaFim = cpfCnpjPessoa.Substring(0, 8) + "999999";
                    double cnpjInicio, cnpjFim = 0;
                    double.TryParse(cnpjPessoaInicio, out cnpjInicio);
                    double.TryParse(cnpjPessoaFim, out cnpjFim);
                    result = result.Where(o => o.Duplicata.Pessoa.CPF_CNPJ >= cnpjInicio && o.Duplicata.Pessoa.CPF_CNPJ <= cnpjFim);
                }
                else
                    result = result.Where(o => o.Duplicata.Pessoa.CPF_CNPJ == double.Parse(cpfCnpjPessoa));
            }

            if (dataLctoInicial != DateTime.MinValue)
                result = result.Where(o => o.Duplicata.DataLancamento >= dataLctoInicial.Date);

            if (dataLctoFinal != DateTime.MinValue)
                result = result.Where(o => o.Duplicata.DataLancamento < dataLctoFinal.AddDays(1).Date);

            if (dataVctoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVcto >= dataVctoInicial.Date);

            if (dataVctoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVcto < dataVctoFinal.AddDays(1).Date);

            if (tipo != null)
                result = result.Where(o => o.Duplicata.Tipo == tipo.Value);

            if (statusPgto != null)
                result = result.Where(o => o.Status == statusPgto.Value);

            if (ordenacao == 0)
            {
                return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioDuplicatas()
                {
                    CodigoDuplicata = o.Duplicata.Codigo,
                    Numero = o.Duplicata.Numero,
                    Tipo = o.Duplicata.Tipo,
                    DataLancamento = o.Duplicata.DataLancamento,
                    Documento = o.Duplicata.Documento,
                    DataDocumento = o.Duplicata.DataDocumento,
                    CpfCnpjPessoa = o.Duplicata.Pessoa.CPF_CNPJ,
                    NomePessoa = o.Duplicata.Pessoa.Nome,
                    CpfMotorista = o.Duplicata.Motorista.CPF,
                    NomeMotorista = o.Duplicata.Motorista.Nome,
                    Veiculo1 = o.Duplicata.Veiculo1.Placa,
                    Veiculo2 = o.Duplicata.Veiculo2.Placa,
                    Veiculo3 = o.Duplicata.Veiculo3.Placa,
                    Valor = o.Duplicata.Valor,
                    Acrescimo = o.Duplicata.Acrescimo,
                    Desconto = o.Duplicata.Desconto,
                    Total = (o.Duplicata.Valor + o.Duplicata.Acrescimo - o.Duplicata.Desconto),
                    Observacao = o.Duplicata.Observacao,

                    CodigoParcela = o.Codigo,
                    Parcela = o.Parcela,
                    ValorParcela = o.Valor,
                    DataVcto = o.DataVcto,
                    ValorPgto = o.ValorPgto,
                    DataPgto = o.DataPgto,
                    ObservacaoBaixa = o.ObservacaoBaixa
                }).ToList();
            }
            else
            {
                return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioDuplicatas()
                {
                    CodigoDuplicata = o.Duplicata.Codigo,
                    Numero = o.Duplicata.Numero,
                    Tipo = o.Duplicata.Tipo,
                    DataLancamento = o.Duplicata.DataLancamento,
                    Documento = o.Duplicata.Documento,
                    DataDocumento = o.Duplicata.DataDocumento,
                    CpfCnpjPessoa = o.Duplicata.Pessoa.CPF_CNPJ,
                    NomePessoa = o.Duplicata.Pessoa.Nome,
                    CpfMotorista = o.Duplicata.Motorista.CPF,
                    NomeMotorista = o.Duplicata.Motorista.Nome,
                    Veiculo1 = o.Duplicata.Veiculo1.Placa,
                    Veiculo2 = o.Duplicata.Veiculo2.Placa,
                    Veiculo3 = o.Duplicata.Veiculo3.Placa,
                    Valor = o.Duplicata.Valor,
                    Acrescimo = o.Duplicata.Acrescimo,
                    Desconto = o.Duplicata.Desconto,
                    Total = (o.Duplicata.Valor + o.Duplicata.Acrescimo - o.Duplicata.Desconto),
                    Observacao = o.Duplicata.Observacao,

                    CodigoParcela = o.Codigo,
                    Parcela = o.Parcela,
                    ValorParcela = o.Valor,
                    DataVcto = o.DataVcto,
                    ValorPgto = o.ValorPgto,
                    DataPgto = o.DataPgto,
                    ObservacaoBaixa = o.ObservacaoBaixa
                }).OrderBy(o => o.NomePessoa).ToList();
            }
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeSemDuplicata> RelatorioCTeSemDuplicata(int codigoEmpresa, int codigoVeiculo1, int codigoVeiculo2, int codigoVeiculo3, string cpfCnpjPessoa, string cpfMotorista, DateTime dataLctoInicial, DateTime dataLctoFinal, DateTime dataVctoInicial, DateTime dataVctoFinal, Dominio.Enumeradores.TipoDuplicata? tipo, Dominio.Enumeradores.StatusDuplicata? statusPgto, int ordenacao, bool raizCNPJ)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == obj.Empresa.TipoAmbiente && obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;

            if (codigoVeiculo1 > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo1 select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (codigoVeiculo2 > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo2 select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (codigoVeiculo3 > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo3 select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (cpfMotorista != "")
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.CTE.Codigo == o.Codigo && obj.CPFMotorista == cpfMotorista select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (cpfCnpjPessoa != "")
            {
                if (raizCNPJ)
                {
                    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjPessoa.Substring(0, 8))) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjPessoa.Substring(0, 8))) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjPessoa.Substring(0, 8))) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjPessoa.Substring(0, 8))) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjPessoa.Substring(0, 8))));
                }
                else
                {
                    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjPessoa) ||
                                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjPessoa) ||
                                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjPessoa) ||
                                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjPessoa) ||
                                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjPessoa));
                }
            }

            if (dataLctoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataLctoInicial.Date);

            if (dataLctoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataLctoFinal.AddDays(1).Date);

            var queryDuplicataCtes = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();
            result = result.Where(o => !(from obj in queryDuplicataCtes where obj.Duplicata.Status == "A" && obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));

            if (ordenacao == 1)
            {
                return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTeSemDuplicata()
                {
                    Codigo = o.Codigo,
                    Numero = o.Numero,
                    Serie = o.Serie.Numero,
                    Status = o.Status,
                    DataEmissao = o.DataEmissao,
                    DataAutorizacao = o.DataRetornoSefaz,
                    CPFCNPJRemetente = o.Remetente.CPF_CNPJ,
                    Remetente = o.Remetente.Nome,
                    CPFCNPJDestinatario = o.Destinatario.CPF_CNPJ,
                    Destinatario = o.Destinatario.Nome,
                    Observacao = o.ObservacoesGerais,
                    PlacaVeiculo = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                    Motorista = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>() where obj.CTE.Codigo == o.Codigo select obj.NomeMotoristaCTe).FirstOrDefault(),
                    ValorICMS = o.ValorICMS,
                    ValorFrete = o.ValorFrete,
                    ValorAReceber = o.ValorAReceber,
                    ChaveCTe = o.Chave
                }).OrderBy(o => o.Remetente).ToList();
            }
            else if (ordenacao == 2)
            {
                return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTeSemDuplicata()
                {
                    Codigo = o.Codigo,
                    Numero = o.Numero,
                    Serie = o.Serie.Numero,
                    Status = o.Status,
                    DataEmissao = o.DataEmissao,
                    DataAutorizacao = o.DataRetornoSefaz,
                    CPFCNPJRemetente = o.Remetente.CPF_CNPJ,
                    Remetente = o.Remetente.Nome,
                    CPFCNPJDestinatario = o.Destinatario.CPF_CNPJ,
                    Destinatario = o.Destinatario.Nome,
                    Observacao = o.ObservacoesGerais,
                    PlacaVeiculo = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                    Motorista = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>() where obj.CTE.Codigo == o.Codigo select obj.NomeMotoristaCTe).FirstOrDefault(),
                    ValorICMS = o.ValorICMS,
                    ValorFrete = o.ValorFrete,
                    ValorAReceber = o.ValorAReceber,
                    ChaveCTe = o.Chave
                }).OrderBy(o => o.Destinatario).ToList();
            }
            else
            {
                return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTeSemDuplicata()
                {
                    Codigo = o.Codigo,
                    Numero = o.Numero,
                    Serie = o.Serie.Numero,
                    Status = o.Status,
                    DataEmissao = o.DataEmissao,
                    DataAutorizacao = o.DataRetornoSefaz,
                    CPFCNPJRemetente = o.Remetente.CPF_CNPJ,
                    Remetente = o.Remetente.Nome,
                    CPFCNPJDestinatario = o.Destinatario.CPF_CNPJ,
                    Destinatario = o.Destinatario.Nome,
                    Observacao = o.ObservacoesGerais,
                    PlacaVeiculo = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                    Motorista = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>() where obj.CTE.Codigo == o.Codigo select obj.NomeMotoristaCTe).FirstOrDefault(),
                    ValorICMS = o.ValorICMS,
                    ValorFrete = o.ValorFrete,
                    ValorAReceber = o.ValorAReceber,
                    ChaveCTe = o.Chave
                }).ToList();
            }
        }
        
        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicata> VisualizarDuplicataSalva()
        {
            return new List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicata>();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataCTes> VisualizarDuplicataCTes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

            var result = from obj in query where obj.Duplicata.Codigo == codigo select obj;

            result = result.OrderBy(o => o.ConhecimentoDeTransporteEletronico.Numero);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataCTes()
            {
                CodigoDuplicata = o.Duplicata.Codigo,
                CodigoCTe = o.ConhecimentoDeTransporteEletronico.Codigo,
                Numero = o.ConhecimentoDeTransporteEletronico.Numero,
                Serie = o.ConhecimentoDeTransporteEletronico.Serie.Numero,
                Data = o.ConhecimentoDeTransporteEletronico.DataEmissao,
                Remetente = o.ConhecimentoDeTransporteEletronico.Remetente.Nome,
                Destinatario = o.ConhecimentoDeTransporteEletronico.Destinatario.Nome,
                Veiculo1 = "",
                Motorista1 = "",
                Volume = 0,
                Peso = 0,
                ValorMercadoria = o.ConhecimentoDeTransporteEletronico.ValorTotalMercadoria,
                ValorFrete = o.ConhecimentoDeTransporteEletronico.ValorFrete,
                ValorAReceber = o.ConhecimentoDeTransporteEletronico.ValorAReceber,
                Notas = o.ConhecimentoDeTransporteEletronico.NumeroNotas,
                ValorICMS = o.ConhecimentoDeTransporteEletronico.ValorICMS
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataParcelas> VisualizarDuplicataParcelas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Duplicata.Codigo == codigo select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataParcelas()
            {
                CodigoDuplicata = o.Duplicata.Codigo,
                CodigoParcela = o.Codigo,
                Parcela = o.Parcela,
                DataVcto = o.DataVcto,
                ValorParcela = o.Valor,
                ValorPgto = o.ValorPgto,
                DataPgto = o.DataPgto,
                ObservacaoBaixa = o.ObservacaoBaixa
            }).ToList();
        }
    }
}
