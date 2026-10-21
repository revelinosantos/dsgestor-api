# Módulo de Ocorrências

Implementação adicionada ao DsGestor.Api para registrar e acompanhar ocorrências de entrega ligadas às notas fiscais do WinThor.

## Componentes adicionados

- Entidades: `Ocorrencia`, `OcorrenciaItem`, `OcorrenciaHistorico`, `MotivoOcorrencia`, `Motorista` e `NotaFiscalOcorrenciaView`.
- Repositories e interfaces do módulo.
- Mapeamentos Oracle no `DsGestorDbContext`.
- Injeção de dependência no `Program.cs`.
- Controllers de ocorrências, itens, motivos, motoristas e pesquisa de notas fiscais.
- Controle de status e gravação de histórico.

## Fluxo de status

```text
RASCUNHO -> ABERTA -> ENTREGUE_MOTORISTA -> RETORNADA
         -> CREDITO_CONFERIDO -> FINALIZADA
```

Ocorrências não finalizadas podem ser canceladas por `ADMIN` ou `GERENTE`.

## Endpoints principais

- `GET /api/notas-fiscais-ocorrencia?numeroNota=804917&codigoFilial=1&serie=5`
- `GET /api/motivos-ocorrencia`
- `GET /api/motoristas`
- `POST /api/ocorrencias/draft`
- `PUT /api/ocorrencias/{id}`
- `POST /api/ocorrencias/{id}/itens`
- `POST /api/ocorrencias/{id}/abrir`
- `POST /api/ocorrencias/{id}/status`
- `POST /api/ocorrencias/{id}/confirmar-credito`
- `POST /api/ocorrencias/{id}/cancelar`

## Observação sobre a regra de unicidade

A constraint `UK_DSG_OCORR_ITEM_NF (NUMTRANSVENDA, NUMTRANSITEM)` impede que o mesmo item da nota fiscal seja usado em mais de uma ocorrência, inclusive depois de cancelamento.

## Validação

Os arquivos foram conferidos estaticamente e as dependências foram registradas. O ambiente usado para preparar o pacote não possui o SDK .NET instalado; portanto, execute na máquina de desenvolvimento:

```bash
dotnet restore
dotnet build DsGestor.sln
```

