# Sales Lab – Dynamics 365 CRM

Projeto de CRM de vendas desenvolvido no **Microsoft Dynamics 365**, utilizando **Dataverse** e **Model-Driven Apps**.

O objetivo do projeto é simular um cenário real de gestão de vendas, aplicando boas práticas de:

* Modelagem de dados no Dataverse
* Customizações client-side com JavaScript
* Implementação de lógica de negócio server-side com **Plugins em C#**
* Validação de regras de negócio em múltiplas camadas

Este projeto foi desenvolvido como parte do meu processo de aprendizado prático em **Dynamics 365 Development**, com foco na construção de um portfólio voltado para posições de **estágio ou nível júnior**.

---

# 🎯 Objetivo do Projeto

Criar um sistema de CRM funcional para controle de vendas, permitindo:

* Cadastro de clientes
* Cadastro de produtos
* Criação e gerenciamento de pedidos
* Relacionamento estruturado entre dados
* Validação de regras de negócio client-side e server-side

O projeto busca demonstrar conceitos fundamentais do desenvolvimento em Dynamics 365, incluindo arquitetura baseada em eventos e separação entre lógica de interface e lógica de servidor.

---

# 🧱 Modelagem de Dados (Dataverse)

## Tabela Cliente

Responsável por armazenar as informações dos clientes.

Campos principais:

* Nome
* Email
* Telefone

![Tabela Cliente](images/cliente.png)

---

## Tabela Produto

Armazena os produtos disponíveis para venda.

Campos principais:

* Nome do Produto
* Preço
* Estoque
* Status

![Tabela Produto](images/produto%20\(1\).png)

---

## Tabela Pedido

Representa os pedidos realizados pelos clientes.

Campos principais:

* Cliente
* Produto
* Quantidade
* Valor Total
* Status do Pedido

![Tabela Pedido](images/pedido.png)

---

# 🔗 Relacionamentos

* **Cliente (1:N) Pedido**
  Um cliente pode possuir vários pedidos.

* **Produto (1:N) Pedido**
  Um produto pode estar associado a vários pedidos.

---

# 🧩 Model-Driven App

Foi criado um **Model-Driven App** baseado nas tabelas do Dataverse, permitindo a navegação e gerenciamento dos dados dentro do Dynamics 365.

Funcionalidades disponíveis:

* Cadastro e edição de Clientes
* Cadastro e edição de Produtos
* Criação e acompanhamento de Pedidos

---

# ⚙️ Business Rules

Foram implementadas regras de negócio diretamente no formulário da entidade **Pedido**.

## ✔️ Regra 1 – Validação da Quantidade

Garante que a quantidade informada seja maior que zero.

Se a quantidade for menor ou igual a zero:

* O sistema exibe mensagem de erro
* O salvamento do registro é impedido

---

## ✔️ Regra 2 – Bloqueio de Campos ao Confirmar Pedido

Quando o status do pedido é alterado para **Confirmado**:

* Cliente, Produto e Quantidade são bloqueados para edição
* Caso o pedido volte para **Rascunho**, os campos são liberados novamente

---

# 💻 JavaScript (Client-Side Customization)

Foi implementado um módulo JavaScript personalizado no formulário de **Pedido**.

## Objetivos

* Calcular automaticamente o **Valor Total**
* Utilizar **Web API do Dynamics** para consultar o preço do produto
* Atualizar campos dinamicamente no formulário

## Eventos utilizados

* `OnChange` do campo **Produto**
* `OnChange` do campo **Quantidade**

## Fluxo da lógica

1. Usuário seleciona Produto e Quantidade
2. O sistema consulta o preço do produto via `Xrm.WebApi.retrieveRecord`
3. O valor total é calculado
4. O campo **Valor Total** é atualizado automaticamente

---

# 🔐 Plugins (Server-Side – C#)

Plugins foram implementados para garantir a aplicação das regras de negócio no **servidor**, independentemente das validações client-side.

## ✔️ Plugin 1 – Cálculo Automático do Total

Evento: **Create / Update – Pedido**
Estágio: **Pre-Operation**

Regras implementadas:

* Impede salvar pedido com quantidade menor ou igual a zero
* Recupera o preço do produto via `IOrganizationService`
* Calcula o total no servidor
* Atualiza o campo antes do registro ser persistido

---

## ✔️ Plugin 2 – Controle de Estoque

Evento: **Create / Update – Pedido**
Estágio: **Pre-Operation**

Regras implementadas:

* Detecta quando o pedido é confirmado
* Verifica disponibilidade de estoque
* Bloqueia confirmação caso o estoque seja insuficiente
* Atualiza automaticamente o estoque do produto

---

## ✔️ Plugin 3 – Processamento Pós-Confirmação

Evento: **Update – Pedido**
Estágio: **Post-Operation**

Responsabilidades:

* Detectar alteração de status para **Confirmado**
* Executar lógica adicional após persistência do registro
* Permitir extensão para integrações futuras ou logs

---

# 🏗️ Arquitetura do Projeto

O projeto foi estruturado utilizando múltiplas camadas de validação:

**Client Layer**

* Business Rules
* JavaScript no formulário

**Server Layer**

* Plugins em C#

**Data Layer**

* Dataverse

Essa abordagem garante que as regras de negócio sejam respeitadas independentemente da interface utilizada.

---

# ⚙️ Tecnologias Utilizadas

* Microsoft Dynamics 365
* Power Apps
* Dataverse
* Model-Driven Apps
* JavaScript
* C# Plugins

---

# 📌 Status do Projeto

✔️ Modelagem de dados
✔️ Relacionamentos
✔️ Model-Driven App
✔️ Business Rules
✔️ JavaScript
✔️ Plugins server-side

### Próximas evoluções

* Integração com Power Automate
* Implementação de Security Roles
* Melhorias arquiteturais

---

# 👤 Autor

Projeto desenvolvido por **Vinicius Ferreira**
com foco em aprendizado prático e desenvolvimento profissional em **Dynamics 365 Development**.
