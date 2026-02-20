using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class DocumentoTransporteNatura : RepositorioBase<Dominio.Entidades.DocumentoTransporteNatura>, Dominio.Interfaces.Repositorios.DocumentoTransporteNatura
    {
        public DocumentoTransporteNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DocumentoTransporteNatura BuscarPorNumero(int codigoEmpresa, long numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.NumeroDT == numero select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoTransporteNatura BuscarPorNumeroNaoCancelado(int codigoEmpresa, long numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.NumeroDT == numero && obj.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoTransporteNatura> Consultar(int codigoEmpresa, long numeroDocumentoTransporte, DateTime dataInicial, DateTime dataFinal, int numeroNFe, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura? statusDT, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroDocumentoTransporte > 0)
                result = result.Where(o => o.NumeroDT == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (numeroNFe > 0)
            {
                var queryNFe = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

                result = result.Where(o => (from obj in queryNFe where obj.DocumentoTransporte.Codigo == o.Codigo && obj.Numero == numeroNFe select obj.DocumentoTransporte.Codigo).Contains(o.Codigo));
            }

            if (statusDT != null)
                result = result.Where(o => o.Status == statusDT);

            return result.OrderByDescending(o => o.NumeroDT).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, long numeroDocumentoTransporte, DateTime dataInicial, DateTime dataFinal, int numeroNFe, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura? statusDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroDocumentoTransporte > 0)
                result = result.Where(o => o.NumeroDT == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (numeroNFe > 0)
            {
                var queryNFe = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

                result = result.Where(o => (from obj in queryNFe where obj.DocumentoTransporte.Codigo == o.Codigo && obj.Numero == numeroNFe select obj.DocumentoTransporte.Codigo).Contains(o.Codigo));
            }

            if (statusDT != null)
                result = result.Where(o => o.Status == statusDT);

            return result.Count();
        }

        public Dominio.Entidades.DocumentoTransporteNatura BuscarPorCodigo(int codigoEmpresa, int codigoDocumentoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigoDocumentoTransporte select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoTransporteNatura BuscarPorCodigo(int codigoDocumentoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Codigo == codigoDocumentoTransporte select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoTransporteNatura> BuscarPorSituacao(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura situacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == situacao select obj;
            
            return result.OrderBy(o => o.NumeroDT).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.DocumentoTransporteNatura> BuscarPorSituacao(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Status == situacao select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(o => o.DataConsulta).ToList();
        }

        public int ContarPorSituacao(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoTransporteNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == situacao select obj;

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte> Relatorio(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, int numeroDT, int numeroCTe, int numeroNFSe, int numeroNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoVeiculo > 0)
                result = result.Where(o => o.DocumentoTransporte.Veiculo.Codigo == codigoVeiculo);

            if (codigoMotorista > 0)
                result = result.Where(o => o.DocumentoTransporte.Motorista.Codigo == codigoMotorista);

            if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoTransporte.DataEmissao >= dataInicial && o.DocumentoTransporte.DataEmissao < dataFinal.AddDays(1));
            else if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DocumentoTransporte.DataEmissao >= dataInicial);
            else if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoTransporte.DataEmissao < dataFinal.AddDays(1));

            if (numeroDT > 0)
                result = result.Where(o => o.DocumentoTransporte.NumeroDT == numeroDT);

            if (numeroCTe > 0)
                result = result.Where(o => o.DocumentoTransporte.NotasFiscais.Any(d => d.CTe.Numero == numeroCTe));

            if (numeroNFSe > 0)
                result = result.Where(o => o.DocumentoTransporte.NotasFiscais.Any(d => d.NFSe.Numero == numeroNFSe));

            if (numeroNFe > 0)
                result = result.Where(o => o.Numero == numeroNFe);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte()
            {
                Numero = o.DocumentoTransporte.NumeroDT,
                Data = o.DocumentoTransporte.DataEmissao,
                Valor = o.DocumentoTransporte.ValorFrete,
                Veiculo = o.DocumentoTransporte.Veiculo != null ? o.DocumentoTransporte.Veiculo.Placa : string.Empty,
                Motorista = o.DocumentoTransporte.Motorista != null ? o.DocumentoTransporte.Motorista.Nome : string.Empty,
                Status = o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado ? "Cancelado" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao ? "Em Digitação" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao ? "Em Emissão" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido ? "Emitido" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro ? "Erro" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Finalizado ? "Finalizado" :
                         o.DocumentoTransporte.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado ? "Retornado" : string.Empty,
                Notas = o.DocumentoTransporte.NumeroNotas,

                TipoDocumento = o.CTe != null ? "CT-e" : o.NFSe != null ? "NFs-e" : string.Empty,
                DocumentoNumero = o.CTe != null ? o.CTe.Numero : o.NFSe != null ? o.NFSe.Numero : 0,
                DocumentoSerie = o.CTe != null ? o.CTe.Serie.Numero.ToString() : o.NFSe != null ? o.NFSe.Serie.Numero.ToString() : string.Empty,
                DocumentoDataEmissao = o.CTe != null ? o.CTe.DataEmissao : o.NFSe != null ? o.NFSe.DataEmissao : DateTime.Today,
                DocumentoValor = o.CTe != null ? o.CTe.ValorAReceber : o.NFSe != null ? o.NFSe.ValorServicos : 0,

            })
            .OrderByDescending(o => o.Data)
            .Timeout(120)
            .ToList();
        }

    }
}
