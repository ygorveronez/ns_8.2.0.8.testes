# --- Config -------------------------------------------------------------------
$script:TargetVersion = "8.2.0.8"   # <-- mude aqui quando quiser

$allowedDirs = "EmissaoCTe.API", "EmissaoCTe.Integracao", "EmissaoCTe.Relatorio", "EmissaoCTe.WebAdmin", "EmissaoCTe.WebApp", "ReportApi", "SGT.Mobile", "SGT.WebAdmin", "SGT.WebService", "SGT.WebService.REST", "SGT.WebAdmin.ProcessamentoDocumentoTransporte", "SGT.WebAdmin.ProcessarArquivoXMLNotaFiscalIntegracao", "SGT.WebAdmin.ProcessarCalculoDeFrete", "SGT.WebAdmin.ProcessarDocumentosService", "SGT.WebAdmin.ProcessarValePedagio", "LeitorCanhotoOCR", "SGT.GerenciadorApp", "Monitoramento", "IntegradorDanoneViaEDIService", "SGT.ProcessadorTarefas"

# --- Helpers: detecção de encoding -------------------------------------------

function Test-IsUtf8 {
    param([byte[]]$Bytes)
    try {
        $utf8Strict = New-Object System.Text.UTF8Encoding($false, $true) # throwOnInvalidBytes
        [void]$utf8Strict.GetString($Bytes)
        return $true
    } catch { return $false }
}

function Get-FileEncodingSmart {
    param([string]$Path)

    # 1) BOM rápido
    try {
        $fs = [System.IO.File]::Open($Path, 'Open', 'Read', 'ReadWrite')
        try {
            $bom = New-Object byte[] 4
            $read = $fs.Read($bom, 0, 4)
            $sig = [BitConverter]::ToString($bom,0,$read)
            switch -regex ($sig) {
                '^EF-BB-BF'    { return New-Object System.Text.UTF8Encoding($true) }            # UTF-8 BOM
                '^FF-FE-00-00' { return New-Object System.Text.UTF32Encoding($false, $true) }   # UTF-32 LE
                '^00-00-FE-FF' { return New-Object System.Text.UTF32Encoding($true,  $true) }   # UTF-32 BE
                '^FF-FE'       { return New-Object System.Text.UnicodeEncoding($false, $true) } # UTF-16 LE
                '^FE-FF'       { return New-Object System.Text.UnicodeEncoding($true,  $true) } # UTF-16 BE
            }
        } finally { $fs.Dispose() }
    } catch { }

    # 2) Se for XML, tente pegar o encoding do cabeçalho
    $ext = [System.IO.Path]::GetExtension($Path)
    if ($ext -match '^\.(csproj|xml|config|props|targets)$') {
        # Ler poucos bytes pra achar o header
        $first = [System.Text.Encoding]::ASCII.GetString([System.IO.File]::ReadAllBytes($Path)[0..([Math]::Min(4095, ([System.IO.File]::ReadAllBytes($Path).Length-1)))])
        if ($first -match '(?i)<\?xml[^>]*encoding\s*=\s*["'']([^"''>]+)["'']') {
            try { return [System.Text.Encoding]::GetEncoding($matches[1]) } catch { }
        }
    }

    # 3) Se não tiver BOM, tente UTF-8; se inválido, use Default (ANSI/Windows-1252)
    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if (Test-IsUtf8 $bytes) {
        return New-Object System.Text.UTF8Encoding($false) # UTF-8 sem BOM
    } else {
        return [System.Text.Encoding]::Default             # ANSI (ex.: Windows-1252)
    }
}

function Read-AllText {
    param([string]$Path, [System.Text.Encoding]$Encoding)
    return [System.IO.File]::ReadAllText($Path, $Encoding)
}

function Write-AllText {
    param([string]$Path, [string]$Content, [System.Text.Encoding]$Encoding)
    [System.IO.File]::WriteAllText($Path, $Content, $Encoding)
}

function Update-TextFileSafely {
    param(
        [string]$Path,
        [ScriptBlock]$Transform
    )
    if (-not (Test-Path $Path -PathType Leaf)) { return $false }

    $enc = Get-FileEncodingSmart -Path $Path
    $content = Read-AllText -Path $Path -Encoding $enc

    try {
        $newContent = & $Transform $content
    } catch {
        Write-Warning "Falha ao processar '$Path': $($_.Exception.Message)"
        return $false
    }

    if ($null -eq $newContent -or $newContent -ceq $content) { return $false }

    $temp = "$Path.tmp"
    try {
        Write-AllText -Path $temp -Content $newContent -Encoding $enc
        if (Test-Path $Path) { Remove-Item $Path -Force }
        Move-Item $temp $Path -Force
        Write-Host "Atualizado: $Path"
        return $true
    } catch {
        Write-Warning "Erro ao gravar '$Path': $($_.Exception.Message)"
        if (Test-Path $temp) { Remove-Item $temp -Force }
        return $false
    }
}

# --- Transformações (regex robustas) ------------------------------------------
# Mantidas as correções de versão (aceitam vazio, ignoram //)

$assemblyTransform = {
    param($text)

    $patAssemblyVersion     = '(?m)^(?!\s*//)\s*(\[\s*assembly\s*:\s*AssemblyVersion\s*\(\s*")([^"]*)("?\s*\)\s*\])'
    $patAssemblyFileVersion = '(?m)^(?!\s*//)\s*(\[\s*assembly\s*:\s*AssemblyFileVersion\s*\(\s*")([^"]*)("?\s*\)\s*\])'

    $text = [regex]::Replace($text, $patAssemblyVersion,     { param($m) $m.Groups[1].Value + $script:TargetVersion + $m.Groups[3].Value })
    $text = [regex]::Replace($text, $patAssemblyFileVersion, { param($m) $m.Groups[1].Value + $script:TargetVersion + $m.Groups[3].Value })
    return $text
}

$csprojTransform = {
    param($text)

    $patAssemblyVersion = '(<AssemblyVersion>\s*)([^<]*)(\s*</AssemblyVersion>)'
    $patFileVersion     = '(<FileVersion>\s*)([^<]*)(\s*</FileVersion>)'
    $patVersion         = '(<Version>\s*)([^<]*)(\s*</Version>)'  # opcional

    $text = [regex]::Replace($text, $patAssemblyVersion, { param($m) $m.Groups[1].Value + $script:TargetVersion + $m.Groups[3].Value })
    $text = [regex]::Replace($text, $patFileVersion,     { param($m) $m.Groups[1].Value + $script:TargetVersion + $m.Groups[3].Value })
    $text = [regex]::Replace($text, $patVersion,         { param($m) $m.Groups[1].Value + $script:TargetVersion + $m.Groups[3].Value })
    return $text
}

# --- Execução -----------------------------------------------------------------
foreach ($dir in $allowedDirs) {
    $currentDir = Join-Path $PSScriptRoot $dir
    Write-Host "Verificando: $currentDir"
    if (-not (Test-Path $currentDir -PathType Container)) { Write-Host "  Diretório não encontrado."; continue }

    $assemblyFile = Join-Path $currentDir "Properties\AssemblyInfo.cs"
    if (Test-Path $assemblyFile -PathType Leaf) {
        [void](Update-TextFileSafely -Path $assemblyFile -Transform $assemblyTransform)
    }

    Get-ChildItem -Path $currentDir -Filter "*.csproj" -File -ErrorAction SilentlyContinue | ForEach-Object {
        [void](Update-TextFileSafely -Path $_.FullName -Transform $csprojTransform)
    }
}

Write-Host "Operação concluída!"
