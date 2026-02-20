using System;
using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class FreteSubcontratado : RepositorioBase<Dominio.Entidades.FreteSubcontratado>, Dominio.Interfaces.Repositorios.FreteSubcontratado
    {
        public FreteSubcontratado(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FreteSubcontratado BuscaPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FreteSubcontratado BuscaPorParceiroCteTipo(int codigoEmpresa, int codigo, double cnpjParceiro, int numeroCTe, Dominio.Enumeradores.TipoFreteSubcontratado? tipo )
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo != codigo select obj;

            if (cnpjParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjParceiro);

            if (numeroCTe > 0)
                result = result.Where(o => o.NumeroCTe == numeroCTe);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.FreteSubcontratado> Consultar(int codigoEmpresa, string nomeParceiro, double cnpjParceiro, int numeroCTe, int numeroNFe, DateTime dataEntrada, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (cnpjParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjParceiro);

            if (!string.IsNullOrWhiteSpace(nomeParceiro))
                result = result.Where(o => o.Parceiro.Nome.Contains(nomeParceiro));

            if (numeroCTe > 0)
                result = result.Where(o => o.NumeroCTe == numeroCTe);

            if (numeroNFe > 0)
                result = result.Where(o => o.NumeroNFe == numeroNFe);

            if (dataEntrada > DateTime.MinValue)
                result = result.Where(o => o.DataEntrada == dataEntrada);

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string nomeParceiro, double cnpjParceiro, int numeroCTe, int numeroNFe, DateTime dataEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (cnpjParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjParceiro);

            if (!string.IsNullOrWhiteSpace(nomeParceiro))
                result = result.Where(o => o.Parceiro.Nome.Contains(nomeParceiro));

            if (numeroCTe > 0)
                result = result.Where(o => o.NumeroCTe == numeroCTe);

            if (numeroNFe > 0)
                result = result.Where(o => o.NumeroNFe == numeroNFe);

            if (dataEntrada > DateTime.MinValue)
                result = result.Where(o => o.DataEntrada == dataEntrada);

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioFretesSubcontratados> Relatorio(int codigoEmpresa, double cnpjPareiro, DateTime dataEntradaInicio, DateTime dataEntradaFim, DateTime dataEntregaInicio, DateTime dataEntregaFim, Dominio.Enumeradores.TipoFreteSubcontratado? tipo, Dominio.Enumeradores.StatusFreteSubcontratado? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (cnpjPareiro > 0f)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjPareiro);

            if (dataEntradaInicio != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada >= dataEntradaInicio.Date);

            if (dataEntradaFim != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada < dataEntradaFim.AddDays(1).Date);

            if (dataEntregaInicio != DateTime.MinValue)
                result = result.Where(o => o.DataEntrega >= dataEntregaInicio.Date);

            if (dataEntregaFim != DateTime.MinValue)
                result = result.Where(o => o.DataEntrega < dataEntregaFim.AddDays(1).Date);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            if (status != null)
                result = result.Where(o => o.Status == status);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioFretesSubcontratados()
            {
                CodigoFreteSubcontratado = o.Codigo,
                Filial = o.Filial,
                Parceiro = o.Parceiro.Nome,
                CTe = o.NumeroCTe,
                NFe = o.NumeroNFe,
                DataEntrada = o.DataEntrada,
                Remetente = o.Remetente.Nome,
                Destinatario = o.Destinatario.Nome,
                Cidade = o.LocalidadeDestino.Descricao + "-" + o.LocalidadeDestino.Estado.Sigla,
                Tipo = o.Tipo,
                Peso = o.Peso,
                Quantidade = o.Quantidade,
                ValorFrete = o.ValorFreteTotal,
                ValorICMS = o.ValorICMS,
                ValorTaxaAdicional = o.ValorTaxaAdicional,
                ValorFreteLiquido = o.ValorFreteLiquido,
                ValorTDA = o.ValorTDA,
                ValorTDE = o.ValorTDE,
                ValorCarroDedicado = o.ValorCarroDedicado,
                ValorComissao = o.ValorComissao,
                PercentualComissao = o.PercentualComissao,
                ValorTotalComissao = o.ValorComissao + o.ValorTDA + o.ValorTDE + o.ValorCarroDedicado,
                DataEntrega = o.DataEntrega,
                Observacao = o.Observacao,
                Motorista = o.Motorista.Nome
            }).ToList();
        }

        public List<Dominio.Entidades.FreteSubcontratado> ConsultarFretesAbertos(int codigoEmpresa, Dominio.Enumeradores.TipoFreteSubcontratado? tipo, DateTime dataInicial, DateTime dataFinal, double cnpjParceiro, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == Dominio.Enumeradores.StatusFreteSubcontratado.Aberto && obj.DataEntrega != null select obj;

            result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjParceiro);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(o => o.DataEntrega >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEntrega < dataFinal.AddDays(1).Date);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            if (maximoRegistros > 0)
                return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public int ContarFretesAbertos(int codigoEmpresa, Dominio.Enumeradores.TipoFreteSubcontratado? tipo, DateTime dataInicial, DateTime dataFinal, double cnpjParceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteSubcontratado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == Dominio.Enumeradores.StatusFreteSubcontratado.Aberto && obj.DataEntrega != null select obj;

            result = result.Where(o => o.Parceiro.CPF_CNPJ == cnpjParceiro);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(o => o.DataEntrega >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEntrega < dataFinal.AddDays(1).Date);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            return result.Count();
        }

    }
}
