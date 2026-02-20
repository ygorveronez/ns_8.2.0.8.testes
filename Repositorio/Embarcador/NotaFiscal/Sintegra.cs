using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class Sintegra : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra>
    {
        public Sintegra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.Sintegra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra> Consultar(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal statusArquivoSintegra, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial == dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal == dataFinal);

            if ((int)statusArquivoSintegra > 0)
                result = result.Where(obj => obj.StatusArquivo == statusArquivoSintegra);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal statusArquivoSintegra, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial == dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal == dataFinal);

            if ((int)statusArquivoSintegra > 0)
                result = result.Where(obj => obj.StatusArquivo == statusArquivoSintegra);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.Count();
        }

        public byte[] ObterTXTGerado(Dominio.Entidades.Embarcador.NotaFiscal.Sintegra spedFiscal)
        {
            if (!string.IsNullOrWhiteSpace(spedFiscal.CaminhoArquivo))
            {
                byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(spedFiscal.CaminhoArquivo);
                return data;
            }
            else
                return null;
        }
    }
}
