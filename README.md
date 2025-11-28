# WpfApp -- Sistema de Cadastro de Pessoas, Produtos e Pedidos

Aplicação desktop desenvolvida em **C# com WPF (.NET Framework 4.6)**
para cadastro, gerenciamento e consulta de **Pessoas**, **Produtos** e
**Pedidos**, utilizando **MVVM**, persistência em **JSON**, e
manipulação dos dados via **LINQ**.

Este projeto foi criado como solução para o **Teste Técnico --
Desenvolvedor C#**.

------------------------------------------------------------------------

## 📦 Tecnologias Utilizadas

-   **.NET Framework 4.6**
-   **WPF (Windows Presentation Foundation)**
-   **MVVM (Model-View-ViewModel)**
-   **Persistência em JSON via IDataStore + JsonDataStore**
-   **LINQ para filtragem e manipulação dos dados**
-   **ObservableCollection e ObservableRangeCollection**
-   **XAML + CodeBehind mínimo**

------------------------------------------------------------------------

## 📁 Estrutura de Pastas

    WpfApp/
    ├── Models/
    ├── Views/
    ├── ViewModels/
    ├── Services/
    │   ├── JsonDataStore.cs
    │   ├── Repositories/
    ├── Data/
    ├── Resources/
    └── README.md

------------------------------------------------------------------------

## 🚀 Como Executar

### 1. Pré-requisitos

-   Windows 10 ou superior
-   Visual Studio 2019 ou superior
-   .NET Framework 4.6 instalado

### 2. Clonar o repositório

    git clone https://github.com/wesleysantana/wpfapp.git

### 3. Abrir a solução

Abra:

    WpfApp.sln

### 4. Executar

Pressione **F5** no Visual Studio.

A aplicação cria a pasta `/Data` automaticamente caso não exista.

------------------------------------------------------------------------

## 🧩 Funcionalidades Implementadas

### ✅ 1. Cadastro de Pessoas

Inclui:

-   Filtros por nome e CPF
-   Inclusão, edição, exclusão
-   Máscara e validação de CPF
-   Exibição dos pedidos da pessoa
-   Ações: marcar pedido como Pago, Enviado, Recebido

### ✅ 2. Cadastro de Produtos

Filtros:

-   Nome
-   Código
-   Valor mínimo e máximo

Ações:

-   Incluir
-   Editar
-   Salvar
-   Excluir

### ✅ 3. Pedidos

Sistema completo de pedidos:

-   Seleção de pessoa
-   Forma de pagamento
-   Adicionar produtos ao pedido
-   Cálculo de subtotal e total
-   Finalização do pedido
-   Cancelamento

### ✅ 4. Consulta de Pedidos

Filtros:

-   Pessoa
-   Data inicial e final
-   Forma de pagamento
-   Status múltiplos

Grid:

-   ID
-   Pessoa
-   Data
-   Status
-   Valor total

Rodapé:

-   Total de pedidos filtrados
-   Valor total somado
-   Botão "Novo Pedido"

------------------------------------------------------------------------

## 🔧 Persistência dos Dados

Arquivos JSON:

    Data/
    ├── pessoas.json
    ├── produtos.json
    └── pedidos.json

Manipulação via LINQ e repositórios.

------------------------------------------------------------------------

## 🧪 Validações

-   CPF validado
-   Campos obrigatórios sinalizados
-   Quantidade mínima nas linhas de pedido
-   Somente pedidos finalizados são persistidos

------------------------------------------------------------------------

## ✅ Atendimento ao Teste Técnico

Todos os itens solicitados foram implementados:

✔ CRUD de Pessoas\
✔ CRUD de Produtos\
✔ Pedidos completos\
✔ Consulta avançada\
✔ Persistência JSON\
✔ MVVM corretamente estruturado\
✔ README detalhado (este arquivo)

------------------------------------------------------------------------

## 📄 Licença

Projeto desenvolvido exclusivamente para avaliação técnica.
