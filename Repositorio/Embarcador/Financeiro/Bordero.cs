using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    public class Bordero : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Bordero>
    {
        public Bordero(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.Bordero BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Bordero>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public int ObterUltimoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Bordero>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Bordero> Consultar(int numero, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero? situacao, int codigoTitulo, int numeroCTe, string numeroCarga, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, decimal valorACobrarInicial, decimal valorACobrarFinal, decimal valorTotalACobrarInicial, decimal valorTotalACobrarFinal, TipoPessoa tipoPessoa, int codigoGrupoPessoas, double cpfCnpjPessoa, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Bordero>();

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataVencimentoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataVencimento >= dataVencimentoInicial.Date);

            if (dataVencimentoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataVencimento < dataVencimentoFinal.AddDays(1).Date);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Titulos.Any(t => t.Titulo.Documentos.Any(d => d.Carga.CodigoCargaEmbarcador == numeroCarga || d.CTe.CargaCTes.Any(c => c.Carga.CodigoCargaEmbarcador == numeroCarga))));

            if(numeroCTe > 0)
                query = query.Where(o => o.Titulos.Any(t => t.Titulo.Documentos.Any(d => d.Carga.CargaCTes.Any(cc => cc.CTe.Numero == numeroCTe) || d.CTe.Numero == numeroCTe)));

            if (tipoPessoa == TipoPessoa.GrupoPessoa && codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else if (tipoPessoa == TipoPessoa.Pessoa && cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjPessoa);
            
            if(valorACobrarInicial > 0m)
                query = query.Where(o => o.ValorACobrar >= valorACobrarInicial);

            if (valorACobrarFinal > 0m)
                query = query.Where(o => o.ValorACobrar <= valorACobrarFinal);

            if (valorTotalACobrarInicial > 0m)
                query = query.Where(o => o.ValorTotalACobrar >= valorTotalACobrarInicial);

            if (valorTotalACobrarFinal > 0m)
                query = query.Where(o => o.ValorTotalACobrar <= valorTotalACobrarFinal);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            return query.Fetch(o => o.GrupoPessoas)
                        .Fetch(o => o.Cliente)
                        .OrderBy(propOrdenar + " " + dirOrdenar)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int numero, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero? situacao, int codigoTitulo, int numeroCTe, string numeroCarga, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, decimal valorACobrarInicial, decimal valorACobrarFinal, decimal valorTotalACobrarInicial, decimal valorTotalACobrarFinal, TipoPessoa tipoPessoa, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Bordero>();

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataVencimentoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataVencimento >= dataVencimentoInicial.Date);

            if (dataVencimentoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataVencimento < dataVencimentoFinal.AddDays(1).Date);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Titulos.Any(t => t.Titulo.Documentos.Any(d => d.Carga.CodigoCargaEmbarcador == numeroCarga || d.CTe.CargaCTes.Any(c => c.Carga.CodigoCargaEmbarcador == numeroCarga))));

            if (numeroCTe > 0)
                query = query.Where(o => o.Titulos.Any(t => t.Titulo.Documentos.Any(d => d.Carga.CargaCTes.Any(cc => cc.CTe.Numero == numeroCTe) || d.CTe.Numero == numeroCTe)));

            if (tipoPessoa == TipoPessoa.GrupoPessoa && codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else if (tipoPessoa == TipoPessoa.Pessoa && cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjPessoa);

            if (valorACobrarInicial > 0m)
                query = query.Where(o => o.ValorACobrar >= valorACobrarInicial);

            if (valorACobrarFinal > 0m)
                query = query.Where(o => o.ValorACobrar <= valorACobrarFinal);

            if (valorTotalACobrarInicial > 0m)
                query = query.Where(o => o.ValorTotalACobrar >= valorTotalACobrarInicial);

            if (valorTotalACobrarFinal > 0m)
                query = query.Where(o => o.ValorTotalACobrar <= valorTotalACobrarFinal);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            return query.Count();
        }
    }
}
