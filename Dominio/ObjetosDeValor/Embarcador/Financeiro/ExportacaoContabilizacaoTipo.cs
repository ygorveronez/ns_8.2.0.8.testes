using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ExportacaoContabilizacaoTipo: IEquatable<ExportacaoContabilizacaoTipo>
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil TipoDocumento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao TipoMovimentoExportacao { get; set; }
        public int Codigo { get; set; }

        public bool Equals(ExportacaoContabilizacaoTipo other)
        {
            if (other.Codigo == Codigo && other.TipoDocumento == TipoDocumento && other.TipoMovimentoExportacao == TipoMovimentoExportacao)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashTipoDocumento = TipoDocumento.GetHashCode();
            int hashTipoMovimentoExportacao = TipoMovimentoExportacao.GetHashCode();
            int hashCodigo = Codigo.GetHashCode();

            return hashTipoDocumento ^ hashTipoMovimentoExportacao ^ hashCodigo;
        }
    }
}
