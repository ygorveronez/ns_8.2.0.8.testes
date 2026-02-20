using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CIOTSigaFacil : RepositorioBase<Dominio.Entidades.CIOTSigaFacil>, Dominio.Interfaces.Repositorios.CIOTSigaFacil
    {
        public CIOTSigaFacil(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CIOTSigaFacil BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CIOTSigaFacil BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CIOTSigaFacil BuscarPorNumero(string numeroCIOT, int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusCIOT status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.NumeroCIOT == numeroCIOT && obj.Empresa.Codigo == codigoEmpresa && obj.Status == status select obj;

            return result.FirstOrDefault();
        }

        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoIntegradora == obj.Empresa.Configuracao.TipoIntegradoraCIOT select obj.Numero;

            return result.Max(o => (int?)o) ?? 0;
        }

        public int BuscarUltimoNumero(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT? integradora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoIntegradora == integradora select obj.Numero;

            return result.Max(o => (int?)o) ?? 0;
        }

        public int BuscarUltimoNSU(int codigoEmpresa)
        {
            var queryCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();
            var queryDocumentoCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var nsuCIOT = (from obj in queryCIOT where obj.Empresa.Codigo == codigoEmpresa select (int?)obj.NSU).Max() ?? 0;
            var nsuDocumento = (from obj in queryDocumentoCIOT where obj.CIOT.Empresa.Codigo == codigoEmpresa select (int?)obj.NSU).Max() ?? 0;

            return nsuCIOT > nsuDocumento ? nsuCIOT : nsuDocumento;
        }

        public List<Dominio.Entidades.CIOTSigaFacil> Consultar(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string placa, string ciot, Dominio.ObjetosDeValor.Enumerador.StatusCIOT? status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(o => o.Veiculo.Placa == placa);

            if (!string.IsNullOrWhiteSpace(ciot))
                result = result.Where(o => o.NumeroCIOT.Contains(ciot));

            if (status.HasValue)
            {
                if(status.Value == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado || status.Value == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    result = result.Where(o => o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado || o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento);
                else
                    result = result.Where(o => o.Status == status.Value);
            }

            return result.OrderByDescending(o => o.TipoIntegradora).ThenByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string placa, string ciot, Dominio.ObjetosDeValor.Enumerador.StatusCIOT? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(o => o.Veiculo.Placa == placa);

            if (!string.IsNullOrWhiteSpace(ciot))
                result = result.Where(o => o.NumeroCIOT.Contains(ciot));

            if (status.HasValue)
            {
                if (status.Value == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado || status.Value == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    result = result.Where(o => o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado || o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento);
                else
                    result = result.Where(o => o.Status == status.Value);
            }

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.ContratoTransporteRodoviario> RelatorioContratoTransporte(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query where obj.Codigo == codigoCIOT select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.ContratoTransporteRodoviario()
            {
                AnoVeiculo = o.Veiculo.AnoFabricacao,
                CodigoNaturezaCarga = o.NaturezaCarga.CodigoNatureza,
                CidadeContratante = o.Empresa.Localidade.Descricao,
                CidadeDestino = o.Destino.Descricao,
                CidadeMotorista = o.Motorista.Localidade.Descricao,
                CidadeOrigem = o.Origem.Descricao,
                CidadeTransportador = o.Transportador.Localidade.Descricao,
                CNHMotorista = o.Motorista.NumeroHabilitacao,
                CNPJContratante = o.Empresa.CNPJ,
                CPFCNPJTransportador = o.Transportador.CPF_CNPJ,
                CPFMotorista = o.Motorista.CPF,
                RGMotorista = o.Motorista.RG,
                DataNascimentoMotorista = o.Motorista.DataNascimento.HasValue ? o.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                PISMotorista = o.Motorista.PIS,
                DataEmissao = o.DataEmissao,
                DescricaoUFVeiculo = o.Veiculo.Estado.Nome,
                EnderecoContratante = o.Empresa.Endereco,
                EnderecoMotorista = o.Motorista.Endereco,
                EnderecoTransportador = o.Transportador.Endereco,
                NomeContratante = o.Empresa.RazaoSocial,
                NomeMotorista = o.Motorista.Nome,
                NomeTransportador = o.Transportador.Nome,
                NumeroCartaoMotorista = o.Motorista.NumeroCartao,
                NumeroCIOT = string.IsNullOrWhiteSpace(o.CodigoVerificadorCIOT) ? o.NumeroCIOT : o.NumeroCIOT + "/" + o.CodigoVerificadorCIOT,
                NumeroContratante = o.Empresa.Numero,
                NumeroContrato = o.NumeroContrato != null ? o.NumeroContrato : string.Empty ,
                NumeroTransportador = o.Transportador.Numero,
                NumeroViagem = o.Numero,
                PlacaVeiculo = o.Veiculo.Placa,
                RNTRCTransportador = o.Veiculo.RNTRC,
                TelefoneContratante = o.Empresa.Telefone,
                TelefoneMotorista = o.Motorista.Telefone,
                TelefoneTransportador = o.Transportador.Telefone1,
                TipoIntegradora = o.TipoIntegradora,
                TipoTransportador = o.Transportador.Tipo,
                UFContratante = o.Empresa.Localidade.Estado.Sigla,
                UFDestino = o.Destino.Estado.Sigla,
                UFMotorista = o.Motorista.Localidade.Estado.Sigla,
                UFOrigem = o.Origem.Estado.Sigla,
                UFTransportador = o.Transportador.Localidade.Estado.Sigla,
                UFVeiculo = o.Veiculo.Estado.Sigla,
                ValorAdiantamento = o.ValorAdiantamento != null ? o.ValorAdiantamento : 0,
                ValorFrete = o.ValorFrete != null ? o.ValorFrete : 0,
                ValorINSS = o.ValorINSS != null ? o.ValorINSS : 0,
                ValorIRRF = o.ValorIRRF != null ? o.ValorIRRF : 0,
                ValorSEST = o.ValorSEST != null ? o.ValorSEST : 0,
                ValorSENAT = o.ValorSENAT != null ? o.ValorSENAT : 0,
                ValorSeguro = o.ValorSeguro != null ? o.ValorSeguro : 0,
                ValorPedagio = o.ValorPedagio != null ? o.ValorPedagio : 0,
                ValorOperacao = o.ValorOperacao != null ? o.ValorOperacao : 0,
                ValorQuitacao = o.ValorQuitacao != null ? o.ValorQuitacao : 0,
                ValorBruto = o.ValorBruto != null ? o.ValorBruto : 0,
                //ValorLiquido = (o.ValorBruto != null ? o.ValorBruto : 0) - ((o.ValorIRRF != null ? o.ValorIRRF : 0) + (o.ValorINSS != null ? o.ValorINSS : 0) + (o.ValorSEST != null ? o.ValorSEST : 0) + (o.ValorSENAT != null ? o.ValorSENAT : 0)),
                ValorLiquido = o.ValorFrete - (o.ValorIRRF + o.ValorINSS + o.ValorSEST + o.ValorSENAT) - o.ValorAdiantamento,
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCIOT> RelatorioCIOT(int empresa, DateTime dataInicial, DateTime dataFinal, double cpfCnpjTransp, int motorista, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTSigaFacil>();

            var result = from obj in query
                         where obj.Empresa.Codigo == empresa
                         select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Date > dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Date <= dataFinal);

            if (cpfCnpjTransp > 0)
                result = result.Where(o => o.Transportador.CPF_CNPJ == cpfCnpjTransp);

            if (motorista > 0)
                result = result.Where(o => o.Motorista.Codigo == motorista);

            if (veiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == veiculo);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCIOT()
            {
                Codigo = o.Codigo,
                NumeroViagem = o.Numero,
                NumeroCIOT = o.NumeroCIOT,
                DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy"),
                Motorista = o.Motorista != null ? o.Motorista.Nome : string.Empty,
                Veiculo = o.Veiculo != null ? o.Veiculo.Placa : string.Empty,
                ValorFrete = o.ValorFrete,
                Impostos = o.ValorINSS + o.ValorIRRF + o.ValorSEST + o.ValorSENAT,
                SeguroPedagio = o.ValorSeguro + o.ValorPedagio,
                Adiantamento = o.ValorAdiantamento,
                PesoTotal = o.PesoBruto,
                TotalOperacao = o.ValorOperacao,
                TotalQuitacao = o.ValorQuitacao,
                CNPJ_Contratado = o.Transportador.CPF_CNPJ,
                Contratado = o.Transportador.Nome,
                Status = o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto ? "Aberto" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado ? "Autorizado" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado ? "Cancelado" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado ? "Encerrado" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente ? "Pendente" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado ? "Rejeitado" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento ? "Rejeitado Evento" :
                         o.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Salvo ? "Salvo" :
                         "Outro"
            }).ToList();
        }
    }
}
